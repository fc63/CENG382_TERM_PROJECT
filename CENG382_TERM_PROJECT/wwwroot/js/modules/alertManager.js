export function setupAlertManager(messageDiv) {
    if (!messageDiv) return;

    setTimeout(function () {
        messageDiv.style.display = "none";
    }, 20000);
}
