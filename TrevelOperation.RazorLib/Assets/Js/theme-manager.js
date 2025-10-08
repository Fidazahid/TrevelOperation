/**
 * Theme Manager
 * A utility for managing theme switching with smooth transitions
 */

// Store references
let themeProviderRef = null;
const THEME_STORAGE_KEY = 'travel-expense-theme-preference';
const DARK_THEME = 'dark';
const LIGHT_THEME = 'light';

const themeManager = {
    /**
     * Initialize the theme manager
     * @param {Object} dotNetRef - Reference to the .NET ThemeProvider component
     * @param {String} defaultTheme - The default theme to use if no preference is stored
     */
    initialize: function (dotNetRef, defaultTheme = LIGHT_THEME) {
        themeProviderRef = dotNetRef;

        // Get stored theme or use default
        const storedTheme = localStorage.getItem(THEME_STORAGE_KEY);
        const initialTheme = storedTheme || defaultTheme;

        // Apply the theme without animation on initial load
        this.applyThemeWithoutAnimation(initialTheme);

        // Ensure body has proper background
        this.fixBackgroundColors();

        // Listen for system preference changes
        this.listenForSystemPreferenceChanges();

        // Update UI state if needed
        if (themeProviderRef) {
            themeProviderRef.invokeMethodAsync('NotifyThemeChanged', initialTheme);
        }
    },

    /**
     * Get the current active theme
     * @returns {String} The current theme ('dark' or 'light')
     */
    getCurrentTheme: function () {
        const htmlEl = document.documentElement;
        return htmlEl.getAttribute('data-theme') || DARK_THEME;
    },

    /**
     * Set a specific theme with animation
     * @param {String} theme - The theme to set ('dark' or 'light')
     */
    setTheme: function (theme) {
        if (theme !== DARK_THEME && theme !== LIGHT_THEME) {
            console.error(`Invalid theme: ${theme}. Must be 'dark' or 'light'`);
            return;
        }

        const currentTheme = this.getCurrentTheme();
        if (currentTheme === theme) return;

        this.applyThemeWithAnimation(theme);
        this.fixBackgroundColors();
        localStorage.setItem(THEME_STORAGE_KEY, theme);

        if (themeProviderRef) {
            themeProviderRef.invokeMethodAsync('NotifyThemeChanged', theme);
        }
    },

    /**
     * Toggle between dark and light themes
     */
    toggleTheme: function () {
        const currentTheme = this.getCurrentTheme();
        const newTheme = currentTheme === DARK_THEME ? LIGHT_THEME : DARK_THEME;
        this.setTheme(newTheme);
    },

    /**
     * Fix background colors to ensure proper theme display
     */
    fixBackgroundColors: function () {
        const body = document.body;
        const html = document.documentElement;
        
        // Remove any conflicting background styles
        body.style.backgroundColor = '';
        html.style.backgroundColor = '';
        
        // Force proper theme classes
        const currentTheme = this.getCurrentTheme();
        if (currentTheme === LIGHT_THEME) {
            body.classList.remove('dark');
            body.classList.add('light');
        } else {
            body.classList.remove('light');
            body.classList.add('dark');
        }
    },

    /**
     * Apply theme with smooth animation
     * @param {String} theme - The theme to apply
     */
    applyThemeWithAnimation: function (theme) {
        const htmlEl = document.documentElement;
        htmlEl.setAttribute('data-theme', theme);
    },

    /**
     * Apply theme without animation (for initial load)
     * @param {String} theme - The theme to apply
     */
    applyThemeWithoutAnimation: function (theme) {
        document.documentElement.setAttribute('data-theme', theme);
    },

    /**
     * Listen for system preference changes
     */
    listenForSystemPreferenceChanges: function () {
        if (window.matchMedia) {
            const mediaQuery = window.matchMedia('(prefers-color-scheme: dark)');

            // Check if user has a stored preference, don't override if they do
            if (!localStorage.getItem(THEME_STORAGE_KEY)) {
                const theme = mediaQuery.matches ? DARK_THEME : LIGHT_THEME;
                this.applyThemeWithoutAnimation(theme);
                this.fixBackgroundColors();
            }

            // Listen for changes
            mediaQuery.addEventListener('change', (e) => {
                // Only respond to system changes if no user preference is stored
                if (!localStorage.getItem(THEME_STORAGE_KEY)) {
                    const theme = e.matches ? DARK_THEME : LIGHT_THEME;
                    this.applyThemeWithAnimation(theme);
                    this.fixBackgroundColors();
                }
            });
        }
    }
};

// Make it globally available
window.themeManager = themeManager;

export default themeManager;