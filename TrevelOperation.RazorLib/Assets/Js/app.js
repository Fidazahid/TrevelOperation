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