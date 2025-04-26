document.addEventListener("DOMContentLoaded", function() {
    const toggleButton = document.getElementById("toggleButton");
    const formDiv = document.getElementById("instructorForm");
    const formElement = document.getElementById("instructorFormElement");

    if (formDiv) {
        formDiv.style.display = "none";
    }

    if (toggleButton) {
        toggleButton.addEventListener("click", function() {
            if (formDiv.style.display === "none" || formDiv.style.display === "") {
                formDiv.style.display = "block";
                toggleButton.textContent = "Kapat";
            } else {
                formDiv.style.display = "none";
                formElement.reset();
                toggleButton.textContent = "Yeni Instructor Ekle";
            }
        });
    }
});
