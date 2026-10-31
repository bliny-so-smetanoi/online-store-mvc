import $ from "jquery";
import Modal from "bootstrap/js/dist/modal";
import {EMPTY, fromEvent, Observable} from "rxjs";
import {fromFetch} from "rxjs/fetch";
import {catchError, exhaustMap, finalize, map, take, takeUntil, tap} from "rxjs/operators";
import {t} from "../localization";

type Step = "phone" | "code";

declare global {
    interface Window {
        requestVisitorAuth: () => Promise<boolean>;
    }
}

let pending: Promise<boolean> | null = null;

export function isVisitorAuthenticated(): boolean {
    return document.body.dataset.authenticated === "true";
}

// Открывает модальное окно входа. Promise завершается true после входа, false если окно закрыли.
export function requestVisitorAuth(): Promise<boolean> {
    if (isVisitorAuthenticated()) {
        return Promise.resolve(true);
    }

    if (!pending) {
        pending = openLoginModal().then(authenticated => {
            pending = null;
            return authenticated;
        });
    }

    return pending;
}

export function initVisitorAuth() {
    window.requestVisitorAuth = requestVisitorAuth;

    $(() => {
        fromEvent($("#visitor-login-button"), "click")
            .subscribe(() => requestVisitorAuth());
    });
}

function markAuthenticated() {
    document.body.dataset.authenticated = "true";
    $("#visitor-login-item").prop("hidden", true);
    $("#visitor-logout-item").prop("hidden", false);
}

function openLoginModal(): Promise<boolean> {
    const modalEl = document.getElementById("visitor-login-modal");
    if (!modalEl) {
        return Promise.resolve(false);
    }

    return new Promise(resolve => {
        const $modal = $(modalEl);
        const $form = $modal.find("#visitor-login-form");
        const $phone = $modal.find("#visitor-login-phone");
        const $code = $modal.find("#visitor-login-code");
        const $codeGroup = $modal.find("#visitor-login-code-group");
        const $error = $modal.find("#visitor-login-error");
        const $submit = $modal.find("#visitor-login-submit");
        const token = String($form.find("input[name='__RequestVerificationToken']").val() ?? "");
        const bsModal = Modal.getOrCreateInstance(modalEl);
        const hidden$ = fromEvent(modalEl, "hidden.bs.modal").pipe(take(1));
        let step: Step = "phone";
        let authenticated = false;

        function setStep(next: Step) {
            step = next;
            $codeGroup.prop("hidden", next !== "code");
            $phone.prop("readonly", next === "code");
            $submit.find("[data-submit-label]").text($submit.data(next === "code" ? "label-login" : "label-request"));
            $error.prop("hidden", true).text("");
            $code.val("");
            (next === "code" ? $code : $phone).trigger("focus");
        }

        function setBusy(busy: boolean) {
            $submit.prop("disabled", busy);
            $submit.find(".spinner-border").prop("hidden", !busy);
        }

        function post(url: string, body: unknown): Observable<void> {
            return fromFetch(url, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "RequestVerificationToken": token
                },
                body: JSON.stringify(body),
                selector: async response => {
                    if (!response.ok) {
                        throw new Error(await response.text());
                    }
                }
            });
        }

        fromEvent<SubmitEvent>($form, "submit").pipe(
            tap(event => event.preventDefault()),
            exhaustMap(() => {
                const phone = String($phone.val() ?? "").trim();
                const code = String($code.val() ?? "").trim();
                const request$ = step === "phone"
                    ? post($modal.data("request-code-url"), {phone}).pipe(map((): Step | "done" => "code"))
                    : post($modal.data("login-url"), {phone, code}).pipe(map((): Step | "done" => "done"));

                setBusy(true);
                return request$.pipe(
                    catchError((error: Error) => {
                        $error.text(error.message || t("Error")).prop("hidden", false);
                        return EMPTY;
                    }),
                    finalize(() => setBusy(false))
                );
            }),
            takeUntil(hidden$)
        ).subscribe(result => {
            if (result === "code") {
                setStep("code");
                return;
            }

            authenticated = true;
            markAuthenticated();
            bsModal.hide();
        });

        fromEvent($modal.find("#visitor-login-change-phone"), "click").pipe(
            takeUntil(hidden$)
        ).subscribe(() => setStep("phone"));

        fromEvent(modalEl, "shown.bs.modal").pipe(
            take(1),
            takeUntil(hidden$)
        ).subscribe(() => $phone.trigger("focus"));

        hidden$.subscribe(() => resolve(authenticated));

        setStep("phone");
        setBusy(false);
        bsModal.show();
    });
}
