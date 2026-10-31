import $ from "jquery";
import {EMPTY, fromEvent} from "rxjs";
import {catchError, filter, finalize, map, mergeMap, tap} from "rxjs/operators";
import {postCartProduct} from "../../../utils/cart";
import {showToast} from "../../../utils/toaster";

$(() => {
    const $cart = $("#visitor-cart");
    const $items = $("#cart-items");
    const $empty = $("#cart-empty");
    const $totalQuantity = $("#cart-total-quantity");
    const $totalPrice = $("#cart-total-price");
    const priceFormat = new Intl.NumberFormat("en-US", {minimumFractionDigits: 2, maximumFractionDigits: 2});

    $items.find("img").each((_, image) => {
        fromEvent(image, "error").subscribe(() => {
            const $image = $(image);
            $image.prop("hidden", true);
            $image.siblings("[data-product-no-image]").prop("hidden", false);
        });
    });

    function updateSummary() {
        let quantity = 0;
        let price = 0;
        $items.find("[data-cart-item]").each((_, item) => {
            const itemQuantity = Number($(item).data("quantity"));
            quantity += itemQuantity;
            price += itemQuantity * Number($(item).data("price"));
        });

        $totalQuantity.text(quantity);
        $totalPrice.text(priceFormat.format(price));
        $empty.prop("hidden", quantity > 0);
    }

    fromEvent<MouseEvent>($items, "click").pipe(
        map(event => $(event.target as HTMLElement).closest<HTMLButtonElement>("button[data-cart-remove]")),
        filter($button => $button.length > 0 && !$button.prop("disabled")),
        mergeMap($button => {
            const $item = $button.closest("[data-cart-item]");
            $button.prop("disabled", true).find(".spinner-border").prop("hidden", false);

            return postCartProduct($cart.data("remove-url"), String($item.data("product-id"))).pipe(
                tap(() => {
                    $item.remove();
                    updateSummary();
                    showToast($cart.data("title"), $cart.data("removed-message"), "success");
                }),
                catchError((error: Error) => {
                    showToast($cart.data("error-title"), error.message || $cart.data("error-title"), "error");
                    return EMPTY;
                }),
                finalize(() => $button.prop("disabled", false).find(".spinner-border").prop("hidden", true))
            );
        })
    ).subscribe();
});
