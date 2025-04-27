import { setupFormManager } from "./modules/formManager.js";
import { setupListManager } from "./modules/listManager.js";
import { setupEditButtons } from "./modules/editManager.js";
import { setupAlertManager } from "./modules/alertManager.js";
import { setupPageState } from "./modules/pageManager.js";

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

	setupPageState(formDiv, instructorListDiv, toggleButton, toggleListButton, cancelButton);
    setupFormManager(toggleButton, toggleListButton, formDiv, formElement, instructorListDiv, editingIdInput, formTitle, submitButton, cancelButton);
    setupListManager(toggleListButton, instructorListDiv, formDiv, toggleButton, editingIdInput, formElement);
    setupEditButtons(formDiv, instructorListDiv, toggleButton, toggleListButton, formElement, editingIdInput, formTitle, submitButton, cancelButton);
    setupAlertManager(messageDiv);
});
