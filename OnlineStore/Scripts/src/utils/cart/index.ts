import {Observable} from "rxjs";
import {fromFetch} from "rxjs/fetch";
import {getRequestToken} from "../visitorAuth";

export class CartRequestError extends Error {
    constructor(message: string, public readonly status: number) {
        super(message);
    }
}

// POST {productId} в AddToCart/RemoveFromCart. Ошибки сервера (400/401 с текстом) приходят как CartRequestError
export function postCartProduct(url: string, productId: string): Observable<void> {
    return fromFetch(url, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "RequestVerificationToken": getRequestToken()
        },
        body: JSON.stringify({productId}),
        selector: async response => {
            if (!response.ok) {
                throw new CartRequestError(await response.text(), response.status);
            }
        }
    });
}
