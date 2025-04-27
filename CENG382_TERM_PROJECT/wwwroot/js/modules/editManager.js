export function setupEditButtons(formDiv, instructorListDiv, toggleButton, toggleListButton, formElement, editingIdInput, formTitle, submitButton, cancelButton) {
    document.querySelectorAll(".edit-btn").forEach(function (button) {
        button.addEventListener("click", function () {
            let searchInput = document.querySelector('input[name="SearchTerm"]');
            if (!searchInput) {
                searchInput = document.createElement("input");
                searchInput.type = "hidden";
                searchInput.name = "SearchTerm";
                formElement.appendChild(searchInput);
            }
            searchInput.value = "";

            formDiv.style.display = "block";
            instructorListDiv.style.display = "none";
            if (toggleButton) toggleButton.style.display = "none";
            if (toggleListButton) toggleListButton.style.display = "none";

            const id = this.getAttribute("data-id");
            const name = this.getAttribute("data-name");
            const email = this.getAttribute("data-email");

            formElement.querySelector('input[name="FullName"]').value = name;
            formElement.querySelector('input[name="Email"]').value = email;
            formElement.querySelector('input[name="Password"]').value = "";

            if (editingIdInput) editingIdInput.value = id;
            if (formTitle) formTitle.textContent = "Instructor Güncelle";
            if (submitButton) submitButton.textContent = "Güncelle";
            if (cancelButton) cancelButton.style.display = "inline-block";
        });
    });
}
