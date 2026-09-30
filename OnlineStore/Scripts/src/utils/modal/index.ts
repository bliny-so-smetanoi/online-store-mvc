import $ from "jquery";
import  Modal  from "bootstrap/js/dist/modal";
import { fromEvent, Observable } from "rxjs";
import { switchMap, take, finalize } from "rxjs/operators";

export function showModal(
    title: string = "",
    bodyHtml: string = "",
    onSave$: () => Observable<unknown>
): void {
    const modalEl = document.getElementById("exampleModal") as HTMLElement;
    const $modal = $(modalEl);

    // Set title & body
    $modal.find(".modal-title").text(title);
    $modal.find(".modal-body").html(bodyHtml);

    // Get Save button (Bootstrap primary button)
    const saveBtn = $modal.find(".btn-primary")[0] as HTMLButtonElement;

    // Important: remove any old click handlers
    $(saveBtn).off("click");

    // Create Bootstrap modal instance
    const bsModal = Modal.getOrCreateInstance(modalEl);

    // RxJS: listen to first click on Save
    fromEvent(saveBtn, "click")
        .pipe(
            take(1), // only first click
            switchMap(() => {
                saveBtn.disabled = true;

                // Call user function that returns Observable
                return onSave$().pipe(
                    finalize(() => {
                        saveBtn.disabled = false;
                    })
                );
            })
        )
        .subscribe({
            next: () => {
                // On success -> hide modal
                bsModal.hide();
            },
            error: (err) => {
                // Here you can use your showToast() if you want
                console.error("Error in onSave$():", err);
            }
        });

    // Show modal
    bsModal.show();
}
