export function setupListManager(toggleListButton, instructorListDiv, formDiv, toggleButton, editingIdInput, formElement) {
    if (!toggleListButton) return;

    toggleListButton.addEventListener("click", function () {
        if (instructorListDiv.style.display === "none" || instructorListDiv.style.display === "") {
            instructorListDiv.style.display = "block";
            formDiv.style.display = "none";
            toggleButton.textContent = "Yeni Instructor Ekle";
            toggleListButton.textContent = "Kapat";
            formElement.reset();
            if (editingIdInput) editingIdInput.value = "";
        } else {
            instructorListDiv.style.display = "none";
            toggleListButton.textContent = "Instructor Listesi";
        }
    });
}