import $ from "jquery";
import {EMPTY, fromEvent} from "rxjs";
import {fromFetch} from "rxjs/fetch";
import {catchError, exhaustMap, filter, finalize, tap} from "rxjs/operators";
import {getRequestToken} from "../../../utils/visitorAuth";
import {showToast} from "../../../utils/toaster";

type PlaceOrderRequest = {
    country: string;
    city: string;
    street: string;
    postalCode: string;
    cardHolderName: string;
    cardNumber: string;
    expirationDate: string;
    cvv: string;
};

$(() => {
    const $checkout = $("#checkout");
    const $form = $("#checkout-form");
    const form = $form.get(0) as HTMLFormElement | undefined;
    if (!form) {
        return;
    }

    const $submit = $("#checkout-submit");
    const $error = $("#checkout-error");

    // Номер карты: только цифры, группами по 4
    fromEvent<InputEvent>($("#checkout-card-number"), "input").subscribe(event => {
        const input = event.target as HTMLInputElement;
        const digits = input.value.replace(/\D/g, "").slice(0, 19);
        input.value = digits.replace(/(\d{4})(?=\d)/g, "$1 ");
    });

    // Срок действия: MM/YY
    fromEvent<InputEvent>($("#checkout-expiration"), "input").subscribe(event => {
        const input = event.target as HTMLInputElement;
        const digits = input.value.replace(/\D/g, "").slice(0, 4);
        input.value = digits.length > 2 ? `${digits.slice(0, 2)}/${digits.slice(2)}` : digits;
    });

    fromEvent<InputEvent>($("#checkout-cvv"), "input").subscribe(event => {
        const input = event.target as HTMLInputElement;
        input.value = input.value.replace(/\D/g, "").slice(0, 4);
    });

    function readForm(): PlaceOrderRequest {
        const value = (name: string) => String($form.find(`[name='${name}']`).val() ?? "").trim();
        return {
            country: value("country"),
            city: value("city"),
            street: value("street"),
            postalCode: value("postalCode"),
            cardHolderName: value("cardHolderName"),
            cardNumber: value("cardNumber"),
            expirationDate: value("expirationDate"),
            cvv: value("cvv")
        };
    }

    function setBusy(busy: boolean) {
        $submit.prop("disabled", busy).find(".spinner-border").prop("hidden", !busy);
    }

    fromEvent<SubmitEvent>(form, "submit").pipe(
        tap(event => event.preventDefault()),
        filter(() => {
            form.classList.add("was-validated");
            return form.checkValidity();
        }),
        exhaustMap(() => {
            setBusy(true);
            $error.prop("hidden", true).text("");

            return fromFetch($checkout.data("place-order-url"), {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "RequestVerificationToken": getRequestToken()
                },
                body: JSON.stringify(readForm()),
                selector: async response => {
                    if (!response.ok) {
                        throw new Error(await response.text());
                    }
                }
            }).pipe(
                tap(() => {
                    form.reset();
                    $("#checkout-content").remove();
                    $("#checkout-success").prop("hidden", false);
                }),
                catchError((error: Error) => {
                    const message = error.message || $checkout.data("error-title");
                    $error.text(message).prop("hidden", false);
                    showToast($checkout.data("error-title"), message, "error");
                    return EMPTY;
                }),
                finalize(() => setBusy(false))
            );
        })
    ).subscribe();
});
