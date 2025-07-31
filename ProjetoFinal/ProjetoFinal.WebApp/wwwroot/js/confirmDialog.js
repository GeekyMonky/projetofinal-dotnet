window.showMudBlazorStyleConfirm = (itemType, itemName) => {
    return new Promise((resolve) => {
        // Create overlay
        const overlay = document.createElement('div');
        overlay.className = 'custom-mud-dialog-overlay';

        // Create dialog
        const dialog = document.createElement('div');
        dialog.className = 'custom-mud-dialog';

        // Title
        const title = document.createElement('div');
        title.className = 'custom-mud-dialog-title';
        title.textContent = 'Confirm Delete';

        // Content (centered, stacked)
        const content = document.createElement('div');
        content.className = 'custom-mud-dialog-content';
        content.style.display = 'block';
        content.style.textAlign = 'center';
        content.style.whiteSpace = 'normal';

        // Message with special handling for products
        let messageHtml = `
            <div style="margin-bottom: 24px;">Are you sure you want to delete this ${itemType}?</div>
            <div style="margin-bottom: 24px; font-weight: bold;">${itemName}</div>
        `;
        
        // Add special warning for products (case-insensitive)
        if (itemType && itemType.toLowerCase() === 'product') {
            messageHtml += `
                <div style="margin-bottom: 24px; color: #dc3545; font-style: italic;">By deleting this product you are also deleting all the stock movements associated with it.</div>
            `;
        }
        
        messageHtml += `
            <div style="margin-bottom: 0;">This action cannot be undone.</div>
        `;
        
        content.innerHTML = messageHtml;

        // Actions
        const actions = document.createElement('div');
        actions.className = 'custom-mud-dialog-actions';

        // Cancel button
        const cancelBtn = document.createElement('button');
        cancelBtn.className = 'custom-mud-button custom-mud-button-default';
        cancelBtn.textContent = 'CANCEL';
        cancelBtn.onclick = () => {
            overlay.classList.remove('show');
            setTimeout(() => {
                if (document.body.contains(overlay)) {
                    document.body.removeChild(overlay);
                }
                resolve(false);
            }, 300);
        };

        // Delete button
        const deleteBtn = document.createElement('button');
        deleteBtn.className = 'custom-mud-button custom-mud-button-error';
        deleteBtn.textContent = 'DELETE';
        deleteBtn.onclick = () => {
            overlay.classList.remove('show');
            setTimeout(() => {
                if (document.body.contains(overlay)) {
                    document.body.removeChild(overlay);
                }
                resolve(true);
            }, 300);
        };

        // Assemble dialog
        actions.appendChild(cancelBtn);
        actions.appendChild(deleteBtn);
        dialog.appendChild(title);
        dialog.appendChild(content);
        dialog.appendChild(actions);
        overlay.appendChild(dialog);

        // Add to body
        document.body.appendChild(overlay);

        // Animate
        setTimeout(() => {
            overlay.classList.add('show');
        }, 10);

        // Overlay click closes
        overlay.onclick = (e) => {
            if (e.target === overlay) {
                cancelBtn.click();
            }
        };

        // ESC key closes
        const handleEsc = (e) => {
            if (e.key === 'Escape') {
                document.removeEventListener('keydown', handleEsc);
                cancelBtn.click();
            }
        };
        document.addEventListener('keydown', handleEsc);
    });
};