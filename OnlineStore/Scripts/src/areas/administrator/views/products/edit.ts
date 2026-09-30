import $ from "jquery";
import { fromEvent, of } from "rxjs";
import {
    catchError,
    exhaustMap,
    finalize,
    map
} from "rxjs/operators";

import { HttpClient } from "../../../../utils/httpClient";
import { showToast } from "../../../../utils/toaster";
import { t } from "../../../../utils/localization";

interface ProductImageResponse {
    content: string;
    fileName: string;
    size: number;
}

interface ProductForUpdateResponse {
    id: string;
    name: string;
    description: string;
    price: number;
    quantity: number;
    images: ProductImageResponse[];
}

interface UpdateProductResponse {
    success?: boolean;
    message?: string;
}

const httpClient = new HttpClient();

$(() => {

    const form =
        document.getElementById("updateProductForm") as HTMLFormElement;

    const submitButton =
        document.getElementById("updateProductBtn") as HTMLButtonElement;

    const fileInput =
        document.getElementById("formFileMultiple") as HTMLInputElement;

    const previewContainer =
        document.getElementById("imagePreview") as HTMLDivElement;

    const productId =
        ($("#productId").val() as string || "").trim();

    const $name =
        $("#administratorProductsView_name");

    const $nameError =
        $("#nameError");

    const $description =
        $("#administratorProductsView_description");

    const $descriptionError =
        $("#descriptionError");

    const $price =
        $("#administratorProductsView_price");

    const $priceError =
        $("#priceError");

    const $quantity =
        $("#administratorProductsView_quantity");

    const $quantityError =
        $("#quantityError");

    function clearValidation(): void {

        $name.removeClass("is-invalid");
        $description.removeClass("is-invalid");
        $price.removeClass("is-invalid");
        $quantity.removeClass("is-invalid");

        $nameError.text("");
        $descriptionError.text("");
        $priceError.text("");
        $quantityError.text("");
    }

    function validate(): boolean {

        clearValidation();

        const name =
            (($name.val() as string) || "").trim();

        const description =
            (($description.val() as string) || "").trim();

        const price =
            (($price.val() as string) || "").trim();

        const quantity =
            (($quantity.val() as string) || "").trim();

        let isValid =
            true;

        if (!name) {
            $name.addClass("is-invalid");
            $nameError.text("Name is required.");
            isValid = false;
        }

        if (!description) {
            $description.addClass("is-invalid");
            $descriptionError.text("Description is required.");
            isValid = false;
        }

        if (!price) {
            $price.addClass("is-invalid");
            $priceError.text("Price is required.");
            isValid = false;
        }

        if (price && isNaN(+price)) {
            $price.addClass("is-invalid");
            $priceError.text("Price must be a number.");
            isValid = false;
        }

        if (!quantity) {
            $quantity.addClass("is-invalid");
            $quantityError.text("Quantity is required.");
            isValid = false;
        }

        if (quantity && isNaN(+quantity)) {
            $quantity.addClass("is-invalid");
            $quantityError.text("Quantity must be a number.");
            isValid = false;
        }

        return isValid;
    }

    function getMimeType(fileName: string): string {

        const extension =
            fileName
                .split(".")
                .pop()
                ?.toLowerCase();

        if (extension === "png") {
            return "image/png";
        }

        if (extension === "webp") {
            return "image/webp";
        }

        if (extension === "gif") {
            return "image/gif";
        }

        if (extension === "svg") {
            return "image/svg+xml";
        }

        return "image/jpeg";
    }

    function createImageElement(
        src: string,
        alt: string
    ): HTMLImageElement {

        const image =
            document.createElement("img");

        image.src =
            src;

        image.alt =
            alt;

        image.className =
            "rounded border";

        image.style.width =
            "140px";

        image.style.height =
            "140px";

        image.style.objectFit =
            "cover";

        return image;
    }

    function renderExistingImages(
        images: ProductImageResponse[]
    ): void {

        previewContainer.innerHTML =
            "";

        images.forEach(imageItem => {

            if (!imageItem.content) {
                return;
            }

            const wrapper =
                document.createElement("div");

            wrapper.className =
                "position-relative";

            const mimeType =
                getMimeType(imageItem.fileName || "");

            const imageSrc =
                `data:${mimeType};base64,${imageItem.content}`;

            const image =
                createImageElement(
                    imageSrc,
                    imageItem.fileName || "Product image"
                );

            wrapper.appendChild(image);

            previewContainer.appendChild(wrapper);
        });
    }

    function renderSelectedImages(): void {

        previewContainer.innerHTML =
            "";

        const files =
            Array.from(fileInput.files || []);

        files.forEach(file => {

            if (!file.type.startsWith("image/")) {
                return;
            }

            const imageUrl =
                URL.createObjectURL(file);

            const wrapper =
                document.createElement("div");

            wrapper.className =
                "position-relative";

            const image =
                createImageElement(
                    imageUrl,
                    file.name
                );

            image.onload =
                () => {

                    URL.revokeObjectURL(imageUrl);
                };

            wrapper.appendChild(image);

            previewContainer.appendChild(wrapper);
        });
    }

    function buildFormData(): FormData {

        const formData =
            new FormData();

        formData.append(
            "Name",
            (($name.val() as string) || "").trim()
        );

        formData.append(
            "Description",
            (($description.val() as string) || "").trim()
        );

        formData.append(
            "Price",
            (($price.val() as string) || "").trim()
        );

        formData.append(
            "Quantity",
            (($quantity.val() as string) || "").trim()
        );

        const files =
            Array.from(fileInput.files || []);

        files.forEach(file => {

            formData.append(
                "Images",
                file
            );
        });

        return formData;
    }

    function fillForm(
        product: ProductForUpdateResponse
    ): void {

        $name.val(product.name);

        $description.val(product.description);

        $price.val(product.price);

        $quantity.val(product.quantity);

        renderExistingImages(
            product.images || []
        );
    }

    function loadProduct(): void {

        httpClient
            .get$<ProductForUpdateResponse>(
                `Administrator/Products/GetForUpdate/${productId}`
            )
            .pipe(
                catchError((error: Error) => {

                    console.error(error);

                    showToast(
                        t("Error"),
                        error.message ||
                        "Error loading product.",
                        "error"
                    );

                    return of<ProductForUpdateResponse | null>(null);
                })
            )
            .subscribe(product => {

                if (!product) {
                    return;
                }

                fillForm(product);
            });
    }

    if (!productId) {

        showToast(
            t("Error"),
            "Product id is empty.",
            "error"
        );

        submitButton.disabled =
            true;

        return;
    }

    fromEvent(fileInput, "change")
        .subscribe(() => {

            renderSelectedImages();
        });

    fromEvent(form, "submit")
        .pipe(

            map((event: Event) => {

                event.preventDefault();

                return validate();
            }),

            exhaustMap((isValid: boolean) => {

                if (!isValid) {

                    return of<UpdateProductResponse>({
                        success: false,
                        message: "Validation error"
                    });
                }

                submitButton.disabled =
                    true;

                return httpClient
                    .putFormData$<UpdateProductResponse>(
                        `Administrator/Products/Update/${productId}`,
                        buildFormData()
                    )
                    .pipe(

                        catchError((error: Error) => {

                            console.error(error);

                            return of<UpdateProductResponse>({
                                success: false,
                                message:
                                    error.message ||
                                    "Unexpected error"
                            });
                        }),

                        finalize(() => {

                            submitButton.disabled =
                                false;
                        })
                    );
            })
        )
        .subscribe((response: UpdateProductResponse) => {

            if (response.success === false) {

                if (
                    response.message &&
                    response.message !== "Validation error"
                ) {

                    showToast(
                        t("Error"),
                        response.message,
                        "error"
                    );
                }

                return;
            }

            showToast(
                t("Success"),
                response.message ||
                "Product updated successfully."
            );
        });

    loadProduct();
});