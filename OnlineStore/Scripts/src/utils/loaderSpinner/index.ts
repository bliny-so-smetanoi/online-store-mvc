import $ from 'jquery';
export function loaderSpinner(show: boolean) {
    const spinner = $("#loader-spinner");
    if (show) {
        spinner.removeClass('d-none');
    }
    else {
        spinner.addClass('d-none');
    }
}  