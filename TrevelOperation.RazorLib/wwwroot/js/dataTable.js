// DataTable JavaScript Helpers
window.dataTable = {
    resizeState: {
        isResizing: false,
        startX: 0,
        startWidth: 0,
        columnName: '',
        component: null
    },

    dragState: {
        isDragging: false,
        draggedElement: null,
        draggedIndex: -1,
        placeholder: null
    },

    // Column Resizing
    startResize: function (tableElement, columnName, dotNetComponent) {
        const th = tableElement.querySelector(`th[data-column="${columnName}"]`);
        if (!th) return;

        this.resizeState.isResizing = true;
        this.resizeState.startX = event.clientX;
        this.resizeState.startWidth = th.offsetWidth;
        this.resizeState.columnName = columnName;
        this.resizeState.component = dotNetComponent;

        document.addEventListener('mousemove', this.handleResize.bind(this));
        document.addEventListener('mouseup', this.stopResize.bind(this));
        
        // Prevent text selection during resize
        document.body.style.userSelect = 'none';
        document.body.style.cursor = 'col-resize';
    },

    handleResize: function (e) {
        if (!this.resizeState.isResizing) return;

        const diff = e.clientX - this.resizeState.startX;
        const newWidth = Math.max(50, this.resizeState.startWidth + diff);

        if (this.resizeState.component) {
            this.resizeState.component.invokeMethodAsync('UpdateColumnWidth', 
                this.resizeState.columnName, 
                newWidth);
        }
    },

    stopResize: function () {
        if (!this.resizeState.isResizing) return;

        this.resizeState.isResizing = false;
        
        if (this.resizeState.component) {
            this.resizeState.component.invokeMethodAsync('EndResize');
        }

        document.removeEventListener('mousemove', this.handleResize.bind(this));
        document.removeEventListener('mouseup', this.stopResize.bind(this));
        
        // Restore normal cursor and selection
        document.body.style.userSelect = '';
        document.body.style.cursor = '';
    },

    // Column Drag and Drop Reordering
    initializeDragDrop: function (containerSelector) {
        const container = document.querySelector(containerSelector);
        if (!container) return;

        const items = container.querySelectorAll('.column-item');
        items.forEach((item, index) => {
            item.setAttribute('draggable', 'true');
            item.dataset.index = index;
            
            item.addEventListener('dragstart', this.handleDragStart.bind(this));
            item.addEventListener('dragover', this.handleDragOver.bind(this));
            item.addEventListener('drop', this.handleDrop.bind(this));
            item.addEventListener('dragend', this.handleDragEnd.bind(this));
        });
    },

    handleDragStart: function (e) {
        this.dragState.isDragging = true;
        this.dragState.draggedElement = e.target.closest('.column-item');
        this.dragState.draggedIndex = parseInt(this.dragState.draggedElement.dataset.index);
        
        e.dataTransfer.effectAllowed = 'move';
        e.dataTransfer.setData('text/html', this.dragState.draggedElement.innerHTML);
        
        this.dragState.draggedElement.style.opacity = '0.5';
    },

    handleDragOver: function (e) {
        if (!this.dragState.isDragging) return;
        
        e.preventDefault();
        e.dataTransfer.dropEffect = 'move';

        const target = e.target.closest('.column-item');
        if (!target || target === this.dragState.draggedElement) return;

        const rect = target.getBoundingClientRect();
        const midpoint = rect.top + rect.height / 2;
        
        if (e.clientY < midpoint) {
            target.parentNode.insertBefore(this.dragState.draggedElement, target);
        } else {
            target.parentNode.insertBefore(this.dragState.draggedElement, target.nextSibling);
        }
    },

    handleDrop: function (e) {
        e.preventDefault();
        e.stopPropagation();
        return false;
    },

    handleDragEnd: function (e) {
        this.dragState.isDragging = false;
        this.dragState.draggedElement.style.opacity = '';
        
        // Update all indices after reordering
        const container = this.dragState.draggedElement.closest('.column-manager');
        if (container) {
            const items = container.querySelectorAll('.column-item');
            items.forEach((item, index) => {
                item.dataset.index = index;
            });
        }
        
        this.dragState.draggedElement = null;
        this.dragState.draggedIndex = -1;
    },

    // Export to CSV
    exportToCsv: function (filename, data) {
        const csv = this.convertToCSV(data);
        const blob = new Blob([csv], { type: 'text/csv;charset=utf-8;' });
        this.downloadFile(blob, filename);
    },

    // Export to Excel (CSV format that Excel can open)
    exportToExcel: function (filename, data) {
        const csv = this.convertToCSV(data);
        const blob = new Blob([csv], { type: 'application/vnd.ms-excel' });
        this.downloadFile(blob, filename);
    },

    convertToCSV: function (objArray) {
        if (!objArray || !objArray.length) return '';

        const array = typeof objArray !== 'object' ? JSON.parse(objArray) : objArray;
        const headers = Object.keys(array[0]);
        
        let csv = headers.map(h => this.escapeCSV(h)).join(',') + '\r\n';
        
        array.forEach(item => {
            const row = headers.map(header => {
                const value = item[header];
                return this.escapeCSV(this.formatValue(value));
            });
            csv += row.join(',') + '\r\n';
        });
        
        return csv;
    },

    escapeCSV: function (value) {
        if (value === null || value === undefined) return '';
        
        const stringValue = String(value);
        
        // If the value contains comma, newline, or quote, wrap in quotes and escape quotes
        if (stringValue.includes(',') || stringValue.includes('\n') || stringValue.includes('"')) {
            return '"' + stringValue.replace(/"/g, '""') + '"';
        }
        
        return stringValue;
    },

    formatValue: function (value) {
        if (value === null || value === undefined) return '';
        
        // Handle dates
        if (value instanceof Date) {
            return this.formatDate(value);
        }
        
        // Handle date strings
        if (typeof value === 'string' && /^\d{4}-\d{2}-\d{2}/.test(value)) {
            const date = new Date(value);
            if (!isNaN(date.getTime())) {
                return this.formatDate(date);
            }
        }
        
        return value;
    },

    formatDate: function (date) {
        const day = String(date.getDate()).padStart(2, '0');
        const month = String(date.getMonth() + 1).padStart(2, '0');
        const year = date.getFullYear();
        return `${day}/${month}/${year}`;
    },

    downloadFile: function (blob, filename) {
        const link = document.createElement('a');
        if (link.download !== undefined) {
            const url = URL.createObjectURL(blob);
            link.setAttribute('href', url);
            link.setAttribute('download', filename);
            link.style.visibility = 'hidden';
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
            URL.revokeObjectURL(url);
        }
    },

    // Local Storage helpers
    saveToLocalStorage: function (key, data) {
        try {
            localStorage.setItem(key, JSON.stringify(data));
            return true;
        } catch (e) {
            console.error('Error saving to localStorage:', e);
            return false;
        }
    },

    loadFromLocalStorage: function (key) {
        try {
            const data = localStorage.getItem(key);
            return data ? JSON.parse(data) : null;
        } catch (e) {
            console.error('Error loading from localStorage:', e);
            return null;
        }
    },

    removeFromLocalStorage: function (key) {
        try {
            localStorage.removeItem(key);
            return true;
        } catch (e) {
            console.error('Error removing from localStorage:', e);
            return false;
        }
    }
};

// Initialize on document ready
document.addEventListener('DOMContentLoaded', function () {
    console.log('DataTable JavaScript initialized');
});
