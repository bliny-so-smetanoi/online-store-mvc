import $ from "jquery";
import {EMPTY, fromEvent, merge} from "rxjs";
import {catchError, exhaustMap, finalize, map, startWith, switchMap, take, takeUntil, tap} from "rxjs/operators";
import {fromFetch} from "rxjs/fetch";
import type {PagedResult} from "../../../shared/pagedTable";

interface Product {
    id: string;
    name: string;
    description: string;
    price: number;
    imageId: string | null;
}

$(() => {
    const $catalog = $("#product-catalog");
    const $list = $("#product-list");
    const $template = $("#product-card-template");
    const $form = $("#product-search");
    const $searchInput = $("#product-search-input");
    const $showMore = $("#products-show-more");
    const $retry = $("#products-retry");
    const $loading = $("#products-loading");
    const $empty = $("#products-empty");
    const $error = $("#products-error");
    const priceFormat = new Intl.NumberFormat("en-US", {minimumFractionDigits: 2, maximumFractionDigits: 2});

    function renderProducts(products: Product[]): JQuery<HTMLElement> {
        let $cards = $<HTMLElement>();

        products.forEach(product => {
            const $card = $($template.prop("content") as DocumentFragment).children().clone();
            $card.find("[data-product-name]").text(product.name);
            $card.find("[data-product-description]").text(product.description.length > 250
                ? `${product.description.slice(0, 249)}…`
                : product.description);
            $card.find("[data-product-price]").text(priceFormat.format(product.price));

            if (product.imageId) {
                const $image = $card.find("img");
                const $placeholder = $card.find("[data-product-no-image]");
                const imageUrl = new URL($catalog.data("image-url"), window.location.href);
                imageUrl.searchParams.set("id", product.imageId);
                $image.attr("alt", product.name);
                merge(fromEvent<Event>($image, "load"), fromEvent<Event>($image, "error")).pipe(
                    take(1),
                    takeUntil(fromEvent($form, "submit"))
                ).subscribe(event => {
                    if (event.type === "error") {
                        $image.prop("hidden", true);
                        $placeholder.prop("hidden", false);
                    }
                });
                $image.attr("src", imageUrl.toString()).prop("hidden", false);
                $placeholder.prop("hidden", true);
            }

            $cards = $cards.add($card);
        });

        return $cards;
    }

    fromEvent<Event>($form, "submit").pipe(
        tap(event => event.preventDefault()),
        map(() => String($searchInput.val() || "").trim()),
        startWith(String($searchInput.val() || "").trim()),
        switchMap(searchString => {
            let page = 0;
            let totalPages = 0;
            $list.empty();
            $showMore.prop("hidden", true);

            return merge(fromEvent($showMore, "click"), fromEvent($retry, "click")).pipe(
                startWith(null),
                exhaustMap(() => {
                    $loading.prop("hidden", false);
                    $error.prop("hidden", true);
                    $empty.prop("hidden", true);
                    $showMore.prop("disabled", true);
                    $list.attr("aria-busy", "true");

                    const url = new URL($catalog.data("products-url"), window.location.href);
                    url.searchParams.set("page", String(page + 1));
                    url.searchParams.set("searchString", searchString);

                    return fromFetch<PagedResult<Product>>(url.toString(), {
                        headers: {"Accept": "application/json"},
                        selector: response => {
                            if (!response.ok) throw new Error(`HTTP ${response.status}`);
                            return response.json();
                        }
                    }).pipe(
                        tap(result => {
                            $list.append(renderProducts(result.items));
                            page = result.page;
                            totalPages = result.totalPages;
                            $empty.prop("hidden", $list.children().length > 0);
                        }),
                        catchError(() => {
                            $error.prop("hidden", false);
                            return EMPTY;
                        }),
                        finalize(() => {
                            $loading.prop("hidden", true);
                            $showMore.prop("disabled", false)
                                .prop("hidden", page >= totalPages || !$error.prop("hidden"));
                            $list.attr("aria-busy", "false");
                        })
                    );
                })
            );
        })
    ).subscribe();
});
