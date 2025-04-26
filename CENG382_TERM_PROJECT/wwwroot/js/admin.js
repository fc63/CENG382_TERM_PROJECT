document.addEventListener("DOMContentLoaded", function () {
    const toggleButton = document.getElementById("toggleButton");
    const formDiv = document.getElementById("instructorForm");
    const formElement = document.getElementById("instructorFormElement");
    const toggleListButton = document.getElementById("toggleListButton");
    const instructorListDiv = document.getElementById("instructorListDiv");
    const editingIdInput = document.getElementById("editingId");
    const formTitle = document.getElementById("formTitle");
    const submitButton = document.getElementById("submitButton");
    const cancelButton = document.getElementById("cancelButton");
    const messageDiv = document.querySelector(".alert");

    if (formDiv) {
        formDiv.style.display = "none";
    }

    if (toggleButton) {
        toggleButton.addEventListener("click", function () {
            if (formDiv.style.display === "none" || formDiv.style.display === "") {
                formDiv.style.display = "block";
                instructorListDiv.style.display = "none";
                toggleButton.textContent = "Kapat";
                toggleListButton.textContent = "Instructor Listesi";
                if (cancelButton) cancelButton.style.display = "none";
                if (editingIdInput) editingIdInput.value = "";
                if (formTitle) formTitle.textContent = "Yeni Instructor Ekle";
                if (submitButton) submitButton.textContent = "Instructor Ekle";
                formElement.reset();
            } else {
                formDiv.style.display = "none";
                formElement.reset();
                if (editingIdInput) editingIdInput.value = "";
                toggleButton.textContent = "Yeni Instructor Ekle";
            }
        });
    }

    if (instructorListDiv) {
        instructorListDiv.style.display = "none";
    }

    if (toggleListButton) {
        toggleListButton.addEventListener("click", function () {
            if (instructorListDiv.style.display === "none" || instructorListDiv.style.display === "") {
                instructorListDiv.style.display = "block";
                formDiv.style.display = "none";
                toggleButton.textContent = "Yeni Instructor Ekle";
                toggleListButton.textContent = "Kapat";
                if (editingIdInput) editingIdInput.value = "";
                formElement.reset();
            } else {
                instructorListDiv.style.display = "none";
                toggleListButton.textContent = "Instructor Listesi";
            }
        });
    }
    if (cancelButton) {
        cancelButton.addEventListener("click", function () {
            if (toggleButton) toggleButton.style.display = "inline-block";
            if (toggleListButton) toggleListButton.style.display = "inline-block";
            formDiv.style.display = "none";
            instructorListDiv.style.display = "block";
            toggleButton.textContent = "Yeni Instructor Ekle";
            toggleListButton.textContent = "Kapat";
            formElement.reset();
            if (editingIdInput) editingIdInput.value = "";
            if (formTitle) formTitle.textContent = "Yeni Instructor Ekle";
            if (submitButton) submitButton.textContent = "Instructor Ekle";
            if (cancelButton) cancelButton.style.display = "none";
        });
    }

    document.querySelectorAll(".edit-btn").forEach(function (button) {
        button.addEventListener("click", function () {
            if (toggleButton) toggleButton.style.display = "none";
            if (toggleListButton) toggleListButton.style.display = "none";
            const id = this.getAttribute("data-id");
            const name = this.getAttribute("data-name");
            const email = this.getAttribute("data-email");

            formDiv.style.display = "block";
            instructorListDiv.style.display = "none";
            toggleButton.textContent = "Yeni Instructor Ekle";
            toggleListButton.textContent = "Instructor Listesi";

            formElement.querySelector('input[name="FullName"]').value = name;
            formElement.querySelector('input[name="Email"]').value = email;
            formElement.querySelector('input[name="Password"]').value = "";

            if (editingIdInput) editingIdInput.value = id;
            if (formTitle) formTitle.textContent = "Instructor Güncelle";
            if (submitButton) submitButton.textContent = "Güncelle";
            if (cancelButton) cancelButton.style.display = "inline-block";
        });
    });

    if (messageDiv) {
        setTimeout(function () {
            messageDiv.style.display = "none";
        }, 20000);
    }
});
