import $ from "jquery";
import {fromEvent} from "rxjs";
import {filter, map} from "rxjs/operators";

$(() => {
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
});
