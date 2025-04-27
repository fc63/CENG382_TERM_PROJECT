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
