module.exports = {
    content: ["Pages/**/*.razor", "Shared/**/*.razor"],
    theme: {
        extend: {},
    },
     safelist: [
        'loading',
        'loading-spinner',
        'loading-lg',
    ],
    daisyui: {
        themes: ["light","dark"],
    },
    plugins: [require("@tailwindcss/typography"), require("daisyui")],
}

