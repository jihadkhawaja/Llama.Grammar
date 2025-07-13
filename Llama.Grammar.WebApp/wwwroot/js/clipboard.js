window.clipboardFunctions = {
    writeText: async function (text) {
        try {
            if (navigator.clipboard && window.isSecureContext) {
                await navigator.clipboard.writeText(text);
                return { success: true, message: 'Text copied to clipboard!' };
            } else {
                // Fallback for older browsers or non-HTTPS
                const textArea = document.createElement('textarea');
                textArea.value = text;
                textArea.style.position = 'fixed';
                textArea.style.opacity = '0';
                document.body.appendChild(textArea);
                textArea.focus();
                textArea.select();
                
                try {
                    const successful = document.execCommand('copy');
                    document.body.removeChild(textArea);
                    
                    if (successful) {
                        return { success: true, message: 'Text copied to clipboard!' };
                    } else {
                        return { success: false, message: 'Failed to copy text to clipboard.' };
                    }
                } catch (err) {
                    document.body.removeChild(textArea);
                    return { success: false, message: 'Failed to copy text to clipboard.' };
                }
            }
        } catch (err) {
            return { success: false, message: 'Failed to copy text to clipboard: ' + err.message };
        }
    }
};