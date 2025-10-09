// Import functionality
import themeManager from './theme-manager';

// Make functions globally available
window.themeManager = themeManager;

// File download helper
window.downloadFile = (fileName, contentType, content) => {
    const blob = new Blob([content], { type: contentType });
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = fileName;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    window.URL.revokeObjectURL(url);
};

// CSV file validation helper
window.validateCsvFile = (file) => {
    return {
        isValid: file.type === 'text/csv' || file.name.toLowerCase().endsWith('.csv'),
        size: file.size,
        name: file.name
    };
};

// Toast notification helper
window.showToast = (type, message) => {
    // Create toast element
    const toast = document.createElement('div');
    toast.className = `alert alert-${type} shadow-lg fixed top-4 right-4 w-96 z-[9999] animate-fade-in`;
    
    // Set icon based on type
    let icon = '';
    switch(type) {
        case 'success':
            icon = '<svg xmlns="http://www.w3.org/2000/svg" class="stroke-current shrink-0 h-6 w-6" fill="none" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" /></svg>';
            break;
        case 'error':
            icon = '<svg xmlns="http://www.w3.org/2000/svg" class="stroke-current shrink-0 h-6 w-6" fill="none" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 14l2-2m0 0l2-2m-2 2l-2-2m2 2l2 2m7-2a9 9 0 11-18 0 9 9 0 0118 0z" /></svg>';
            break;
        case 'warning':
            icon = '<svg xmlns="http://www.w3.org/2000/svg" class="stroke-current shrink-0 h-6 w-6" fill="none" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" /></svg>';
            break;
        case 'info':
            icon = '<svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" class="stroke-current shrink-0 w-6 h-6"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"></path></svg>';
            break;
    }
    
    toast.innerHTML = `
        <div>
            ${icon}
            <span>${message}</span>
        </div>
    `;
    
    document.body.appendChild(toast);
    
    // Auto remove after 5 seconds
    setTimeout(() => {
        toast.classList.add('animate-fade-out');
        setTimeout(() => {
            document.body.removeChild(toast);
        }, 300);
    }, 5000);
};