import $ from "jquery";
import {EMPTY, from, fromEvent} from "rxjs";
import {catchError, exhaustMap, filter, finalize, map, tap} from "rxjs/operators";
import {postCartProduct} from "../../../utils/cart";
import {showToast} from "../../../utils/toaster";

$(() => {
    const $details = $("#product-details");
    const $addToCart = $("#product-add-to-cart");
    const $mainImage = $("#product-main-image");
    const $noImage = $("#product-no-image");
    const $thumbnails = $("#product-thumbnails");

    fromEvent($mainImage, "error").subscribe(() => {
        $mainImage.prop("hidden", true);
        $noImage.prop("hidden", false);
    });

    fromEvent<MouseEvent>($thumbnails, "click").pipe(
        map(event => $(event.target as HTMLElement).closest<HTMLButtonElement>("button[data-image-url]")),
        filter($button => $button.length > 0)
    ).subscribe($button => {
        $thumbnails.find("button[data-image-url]")
            .removeClass("border-primary")
            .attr("aria-pressed", "false");
        $button.addClass("border-primary").attr("aria-pressed", "true");

        $noImage.prop("hidden", true);
        $mainImage.prop("hidden", false).attr("src", String($button.data("image-url")));
    });

    // Добавление в корзину: сначала вход визитора (модальное окно из _Layout), затем запрос AddToCart
    fromEvent($addToCart, "click").pipe(
        exhaustMap(() => from(window.requestVisitorAuth()).pipe(
            filter(authenticated => authenticated),
            exhaustMap(() => {
                $addToCart.prop("disabled", true).find(".spinner-border").prop("hidden", false);

                return postCartProduct($details.data("add-to-cart-url"), String($addToCart.data("product-id"))).pipe(
                    tap(() => showToast($details.data("cart-title"), $details.data("added-message"), "success")),
                    catchError((error: Error) => {
                        showToast($details.data("error-title"), error.message || $details.data("error-title"), "error");
                        return EMPTY;
                    }),
                    finalize(() => $addToCart.prop("disabled", false).find(".spinner-border").prop("hidden", true))
                );
            })
        ))
    ).subscribe();
});
