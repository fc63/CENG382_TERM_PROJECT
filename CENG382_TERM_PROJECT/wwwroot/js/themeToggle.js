document.addEventListener("DOMContentLoaded", function () {
    const currentTheme = localStorage.getItem("theme") || "light";
    document.documentElement.setAttribute("data-theme", currentTheme);

    const toggleButton = document.getElementById("themeToggle");
    if (toggleButton) {
        toggleButton.addEventListener("click", () => {
            const newTheme = document.documentElement.getAttribute("data-theme") === "light" ? "dark" : "light";
            document.documentElement.setAttribute("data-theme", newTheme);
            localStorage.setItem("theme", newTheme);
        });
    }
});
