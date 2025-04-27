import { setupFormManager } from "./modules/formManager.js";
import { setupListManager } from "./modules/listManager.js";
import { setupEditButtons } from "./modules/editManager.js";
import { setupAlertManager } from "./modules/alertManager.js";
import { getPageParams } from "./modules/pageParams.js";

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

    const { showForm, showList, showFormParam, showListParam, pageNumber, searchTerm } = getPageParams();

    if (formDiv) formDiv.style.display = "none";
    if (instructorListDiv) instructorListDiv.style.display = "none";

    if (showForm === "True" || showFormParam === "True") {
        formDiv.style.display = "block";
        instructorListDiv.style.display = "none";
        toggleButton.textContent = "Kapat";
        toggleListButton.textContent = "Instructor Listesi";
        cancelButton.style.display = "none";
    } else if (showList === "True" || showListParam === "True" || pageNumber !== null || (searchTerm && searchTerm.trim() !== "")) {
        instructorListDiv.style.display = "block";
        formDiv.style.display = "none";
        toggleButton.textContent = "Yeni Instructor Ekle";
        toggleListButton.textContent = "Kapat";
    }

    setupFormManager(toggleButton, toggleListButton, formDiv, formElement, instructorListDiv, editingIdInput, formTitle, submitButton, cancelButton);
    setupListManager(toggleListButton, instructorListDiv, formDiv, toggleButton, editingIdInput, formElement);
    setupEditButtons(formDiv, instructorListDiv, toggleButton, toggleListButton, formElement, editingIdInput, formTitle, submitButton, cancelButton);
    setupAlertManager(messageDiv);
});
