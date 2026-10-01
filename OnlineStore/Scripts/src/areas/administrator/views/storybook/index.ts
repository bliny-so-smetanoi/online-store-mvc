import $ from "jquery";
import { fromEvent, of } from "rxjs";
import { catchError, exhaustMap, finalize, map } from "rxjs/operators";
import { HttpClient } from "../../../../utils/httpClient"; // path change if needed
import { showToast } from "../../../../utils/toaster";
import { t } from "../../../../utils/localization";
import {PagedTable} from "../../../../shared/pagedTable";
import {loaderSpinner} from "../../../../utils/loaderSpinner";
import dayjs from "dayjs";
import {TodoItem} from "./dtos/todo.item";
import {showModal} from "../../../../utils/modal";

interface AddTodoDto {
    title: string;
}

interface CreateProductResponse {
    success?: boolean;
    message?: string;
}

const httpClient = new HttpClient();

$(() => {
    const form = document.getElementById("addTodoForm") as HTMLFormElement;
    const submitButton = document.getElementById("addTodoBtn") as HTMLButtonElement;

    const $todoTitle = $("#storybookView_title");
    const $todoTitleError = $("#titleError");

    function clearValidation(): void {
        $todoTitle.removeClass("is-invalid");

        $todoTitleError.text("");
    }

    function validate(): boolean {
        clearValidation();

        const nameKk = (($todoTitle.val() as string) || "").trim();

        let isValid = true;

        if (!nameKk) {
            $todoTitle.addClass("is-invalid");
            $todoTitleError.text("Title is required.");
            isValid = false;
        }

        return isValid;
    }

    function buildDto(): AddTodoDto {
        return {
            title: (($todoTitle.val() as string) || "").trim()
        };
    }

    function resetForm(): void {
        form.reset();
        clearValidation();
    }

    fromEvent(form, "submit")
        .pipe(
            map((event: Event) => {
                event.preventDefault();
                return validate();
            }),
            exhaustMap((isValid: boolean) => {
                if (!isValid) {
                    return of<CreateProductResponse>({
                        success: false,
                        message: "Validation error"
                    });
                }

                const dto = buildDto();
                submitButton.disabled = true;

                return httpClient.post$<CreateProductResponse>(
                    "Administrator/Storybook/AddTodo/",
                    dto
                ).pipe(
                    catchError((error: Error) => {
                        console.error(error);

                        return of<CreateProductResponse>({
                            success: false,
                            message: error.message || "Unexpected error"
                        });
                    }),
                    finalize(() => {
                        submitButton.disabled = false;
                    })
                );
            })
        )
        .subscribe((response: CreateProductResponse) => {
            if (response.success === false) {
                if (response.message && response.message !== "Validation error") {
                    showToast(t("Error"), response.message, 'error');
                }
                return;
            }

            showModal('Saved!', '<p>Saved successfully!</p>', () => of(null));
            resetForm();
        });
});


const table = new PagedTable<TodoItem>({
    tbodySelector: '#about-table-body',
    paginationSelector: '#about-pagination',
    searchSelector: '.column-search',
    url: 'Administrator/Storybook/GetList/',
    pageSize: 10,
    emptyText: 'No data',
    errorText: 'Error loading data',
    loader: isLoading => loaderSpinner(isLoading),
    request: <TResponse>(url: string) => httpClient.get$<TResponse>(url),
    renderRow: item => `
        <tr>
            <td>${item.id}</td>
            <td>${item.title}</td>
            <td>${dayjs(item.created).format('DD-MM-YYYY HH:mm:ss')}</td>
        </tr>
    `
});

table.init();