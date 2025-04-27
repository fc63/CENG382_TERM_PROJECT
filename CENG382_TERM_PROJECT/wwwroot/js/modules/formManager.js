export function setupFormManager(toggleButton, toggleListButton, formDiv, formElement, instructorListDiv, editingIdInput, formTitle, submitButton, cancelButton) {
    if (toggleButton) {
        toggleButton.addEventListener("click", function () {
            if (formDiv.style.display === "none" || formDiv.style.display === "") {
                formDiv.style.display = "block";
                instructorListDiv.style.display = "none";
                toggleButton.textContent = "Kapat";
                toggleListButton.textContent = "Instructor Listesi";
                if (formTitle) formTitle.textContent = "Yeni Instructor Ekle";
                if (submitButton) submitButton.textContent = "Instructor Ekle";
                if (cancelButton) cancelButton.style.display = "none";
                formElement.reset();
                if (editingIdInput) editingIdInput.value = "";
            } else {
                formDiv.style.display = "none";
                toggleButton.textContent = "Yeni Instructor Ekle";
                toggleListButton.textContent = "Instructor Listesi";
                formElement.reset();
                if (editingIdInput) editingIdInput.value = "";
                if (cancelButton) cancelButton.style.display = "none";
            }
        });
    }

    if (cancelButton) {
        cancelButton.addEventListener("click", function () {
            if (toggleButton) toggleButton.style.display = "inline-block";
            if (toggleListButton) toggleListButton.style.display = "inline-block";
            if (formDiv) formDiv.style.display = "none";
            if (instructorListDiv) instructorListDiv.style.display = "block";
            toggleButton.textContent = "Yeni Instructor Ekle";
            toggleListButton.textContent = "Kapat";
            formElement.reset();
            if (editingIdInput) editingIdInput.value = "";
            if (formTitle) formTitle.textContent = "Yeni Instructor Ekle";
            if (submitButton) submitButton.textContent = "Instructor Ekle";
            if (cancelButton) cancelButton.style.display = "none";
        });
    }
}
