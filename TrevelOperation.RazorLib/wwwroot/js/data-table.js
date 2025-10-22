window.dataTable = {
    startResize: function (tableElement, columnName, dotNetRef) {
        let isResizing = false;
        let startX = 0;
        let startWidth = 0;
        let columnElement = null;

        // Find the column header element
        const headers = tableElement.querySelectorAll('th[data-column]');
        for (let header of headers) {
            if (header.getAttribute('data-column') === columnName) {
                columnElement = header;
                break;
            }
        }

        if (!columnElement) return;

        const resizeHandle = columnElement.querySelector('.resize-handle');
        if (!resizeHandle) return;

        function initResize(e) {
            isResizing = true;
            startX = e.clientX;
            startWidth = parseInt(window.getComputedStyle(columnElement).width, 10);
            document.addEventListener('mousemove', doResize);
            document.addEventListener('mouseup', stopResize);
            e.preventDefault();
        }

        function doResize(e) {
            if (!isResizing) return;
            
            const width = startWidth + e.clientX - startX;
            const newWidth = Math.max(50, width); // Minimum width of 50px
            
            columnElement.style.width = newWidth + 'px';
            
            // Update corresponding cells in the body
            const columnIndex = Array.from(columnElement.parentNode.children).indexOf(columnElement);
            const rows = tableElement.querySelectorAll('tbody tr');
            rows.forEach(row => {
                const cell = row.children[columnIndex];
                if (cell) {
                    cell.style.width = newWidth + 'px';
                }
            });

            // Notify Blazor component
            dotNetRef.invokeMethodAsync('UpdateColumnWidth', columnName, newWidth);
        }

        function stopResize() {
            if (isResizing) {
                isResizing = false;
                document.removeEventListener('mousemove', doResize);
                document.removeEventListener('mouseup', stopResize);
                dotNetRef.invokeMethodAsync('EndResize');
            }
        }

        resizeHandle.addEventListener('mousedown', initResize);
    },

    makeTableSortable: function (tableElement) {
        // Enable drag and drop for column reordering
        const headers = tableElement.querySelectorAll('th');
        headers.forEach(header => {
            header.draggable = true;
            
            header.addEventListener('dragstart', function(e) {
                e.dataTransfer.setData('text/plain', this.getAttribute('data-column'));
                this.classList.add('dragging');
            });

            header.addEventListener('dragover', function(e) {
                e.preventDefault();
            });

            header.addEventListener('drop', function(e) {
                e.preventDefault();
                const draggedColumn = e.dataTransfer.getData('text/plain');
                const targetColumn = this.getAttribute('data-column');
                
                if (draggedColumn !== targetColumn) {
                    // Notify Blazor component about column reorder
                    console.log(`Reorder: ${draggedColumn} to ${targetColumn}`);
                }
            });

            header.addEventListener('dragend', function() {
                this.classList.remove('dragging');
            });
        });
    },

    exportTableToCsv: function (tableElement, filename) {
        const rows = [];
        const table = tableElement;
        
        // Get headers
        const headers = [];
        table.querySelectorAll('thead th').forEach(th => {
            headers.push(th.textContent.trim());
        });
        rows.push(headers.join(','));
        
        // Get data rows
        table.querySelectorAll('tbody tr').forEach(tr => {
            const row = [];
            tr.querySelectorAll('td').forEach(td => {
                let text = td.textContent.trim();
                // Escape commas and quotes
                if (text.includes(',') || text.includes('"')) {
                    text = '"' + text.replace(/"/g, '""') + '"';
                }
                row.push(text);
            });
            rows.push(row.join(','));
        });
        
        // Create and download file
        const csvContent = rows.join('\n');
        const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
        const link = document.createElement('a');
        const url = URL.createObjectURL(blob);
        link.setAttribute('href', url);
        link.setAttribute('download', filename || 'export.csv');
        link.style.visibility = 'hidden';
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    },

    highlightSearchTerms: function (tableElement, searchTerm) {
        if (!searchTerm) {
            // Remove existing highlights
            tableElement.querySelectorAll('.search-highlight').forEach(el => {
                el.outerHTML = el.innerHTML;
            });
            return;
        }

        const cells = tableElement.querySelectorAll('tbody td');
        cells.forEach(cell => {
            const text = cell.textContent;
            const highlightedText = text.replace(
                new RegExp(`(${searchTerm})`, 'gi'),
                '<span class="search-highlight">$1</span>'
            );
            if (highlightedText !== text) {
                cell.innerHTML = highlightedText;
            }
        });
    },

    saveTableState: function (tableId, state) {
        localStorage.setItem(`table_state_${tableId}`, JSON.stringify(state));
    },

    loadTableState: function (tableId) {
        const state = localStorage.getItem(`table_state_${tableId}`);
        return state ? JSON.parse(state) : null;
    },

    setupKeyboardNavigation: function (tableElement) {
        tableElement.addEventListener('keydown', function(e) {
            const activeCell = document.activeElement;
            if (!activeCell || !activeCell.closest('td')) return;

            const currentRow = activeCell.closest('tr');
            const currentCellIndex = Array.from(currentRow.children).indexOf(activeCell.closest('td'));

            switch (e.key) {
                case 'ArrowUp':
                    e.preventDefault();
                    const prevRow = currentRow.previousElementSibling;
                    if (prevRow) {
                        const prevCell = prevRow.children[currentCellIndex];
                        if (prevCell) {
                            const input = prevCell.querySelector('input, select, textarea');
                            if (input) input.focus();
                        }
                    }
                    break;
                
                case 'ArrowDown':
                    e.preventDefault();
                    const nextRow = currentRow.nextElementSibling;
                    if (nextRow) {
                        const nextCell = nextRow.children[currentCellIndex];
                        if (nextCell) {
                            const input = nextCell.querySelector('input, select, textarea');
                            if (input) input.focus();
                        }
                    }
                    break;
                
                case 'Tab':
                    // Default tab behavior for horizontal navigation
                    break;
                
                case 'Escape':
                    e.preventDefault();
                    activeCell.blur();
                    break;
            }
        });
    }
};

// CSS for search highlighting
const style = document.createElement('style');
style.textContent = `
    .search-highlight {
        background-color: yellow;
        font-weight: bold;
    }
    
    .dragging {
        opacity: 0.5;
    }
    
    th[draggable="true"] {
        cursor: move;
    }
    
    th[draggable="true"]:hover {
        background-color: rgba(0, 123, 255, 0.1);
    }
`;
document.head.appendChild(style);