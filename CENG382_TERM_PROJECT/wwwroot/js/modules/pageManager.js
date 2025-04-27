export function getPageParams() {
    const showForm = document.getElementById("showForm")?.value;
    const showList = document.getElementById("showList")?.value;
    const urlParams = new URLSearchParams(window.location.search);
    return {
        showFormParam: urlParams.get('showForm'),
        showListParam: urlParams.get('showList'),
        pageNumber: urlParams.get('pageNumber'),
        searchTerm: urlParams.get('searchTerm'),
        showForm,
        showList
    };
}

export function setupPageState(formDiv, instructorListDiv, toggleButton, toggleListButton, cancelButton) {
    const { showForm, showList, showFormParam, showListParam, pageNumber, searchTerm } = getPageParams();

    if (formDiv) formDiv.style.display = "none";
    if (instructorListDiv) instructorListDiv.style.display = "none";

    if (showForm === "True" || showFormParam === "True") {
        formDiv.style.display = "block";
        instructorListDiv.style.display = "none";
        if (toggleButton) toggleButton.textContent = "Kapat";
        if (toggleListButton) toggleListButton.textContent = "Instructor Listesi";
        if (cancelButton) cancelButton.style.display = "none";
    } else if (showList === "True" || showListParam === "True" || pageNumber !== null || (searchTerm && searchTerm.trim() !== "")) {
        instructorListDiv.style.display = "block";
        formDiv.style.display = "none";
        if (toggleButton) toggleButton.textContent = "Yeni Instructor Ekle";
        if (toggleListButton) toggleListButton.textContent = "Kapat";
    }
}
