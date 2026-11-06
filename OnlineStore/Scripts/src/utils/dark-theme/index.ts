import $ from "jquery";
import {fromEvent} from "rxjs";
import {map} from "rxjs/operators";

type Theme = "dark" | "light";

const THEME_STORAGE_KEY = "theme";

function getStoredTheme(): Theme {
    return localStorage.getItem(THEME_STORAGE_KEY) === "dark" ? "dark" : "light";
}

function applyTheme(theme: Theme) {
    document.documentElement.setAttribute("data-bs-theme", theme);
    $("#dark-theme-on").prop("hidden", theme !== "dark");
    $("#dark-theme-off").prop("hidden", theme === "dark");
}

export function initDarkTheme() {
    // Apply the theme immediately, before the DOM loads, so the page doesn't flash the light theme
    document.documentElement.setAttribute("data-bs-theme", getStoredTheme());

    $(() => {
        const $checkbox = $("#dark-theme-checkbox");
        const theme = getStoredTheme();

        $checkbox.prop("checked", theme === "dark");
        applyTheme(theme);

        fromEvent($checkbox, "change").pipe(
            map((): Theme => $checkbox.is(":checked") ? "dark" : "light")
        ).subscribe(theme => {
            localStorage.setItem(THEME_STORAGE_KEY, theme);
            applyTheme(theme);
        });
    });
}
