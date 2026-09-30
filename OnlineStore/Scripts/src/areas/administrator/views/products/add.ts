import $ from "jquery";
import { fromEvent, of } from "rxjs";
import { catchError, exhaustMap, finalize, map } from "rxjs/operators";

import { HttpClient } from "../../../../utils/httpClient";
import { showToast } from "../../../../utils/toaster";
import { t } from "../../../../utils/localization";

interface CreateProductResponse {
    success?: boolean;
    message?: string;
}

const httpClient = new HttpClient();

$(() => {

    const form = document.getElementById("addProductForm") as HTMLFormElement;

    const submitButton =
        document.getElementById("addProductBtn") as HTMLButtonElement;

    const fileInput =
        document.getElementById("formFileMultiple") as HTMLInputElement;

    const previewContainer =
        document.getElementById("imagePreview") as HTMLDivElement;

    const $name = $("#administratorProductsView_name");
    const $nameError = $("#nameError");

    const $description = $("#administratorProductsView_description");
    const $descriptionError = $("#descriptionError");

    const $price = $("#administratorProductsView_price");
    const $priceError = $("#priceError");

    const $quantity = $("#administratorProductsView_quantity");
    const $quantityError = $("#quantityError");
    
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
        
        let isValid = true;

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
        
        if (!quantity){
            $quantity.addClass("is-invalid");
            $quantityError.text("Quantity is required.");
            isValid = false;
        }

        if (isNaN(+quantity)) {
            $quantity.addClass("is-invalid");
            $quantityError.text("Quantity is not number.");
            isValid = false;
        }
        
        if (isNaN(+price)) {

            $price.addClass("is-invalid");

            $priceError.text("Price must be a number.");

            isValid = false;
        }

        return isValid;
    }

    function renderImagePreview(): void {

        previewContainer.innerHTML = "";

        const files = Array.from(fileInput.files || []);

        files.forEach(file => {

            if (!file.type.startsWith("image/")) {
                return;
            }

            const imageUrl = URL.createObjectURL(file);

            const wrapper = document.createElement("div");

            wrapper.className =
                "position-relative";

            const image = document.createElement("img");

            image.src = imageUrl;

            image.alt = file.name;

            image.className =
                "rounded border";

            image.style.width = "140px";
            image.style.height = "140px";
            image.style.objectFit = "cover";

            image.onload = () => {
                URL.revokeObjectURL(imageUrl);
            };

            wrapper.appendChild(image);

            previewContainer.appendChild(wrapper);
        });
    }

    function buildFormData(): FormData {

        const formData = new FormData();

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
        )

        const files = Array.from(fileInput.files || []);

        files.forEach(file => {

            formData.append(
                "Images",
                file
            );
        });

        return formData;
    }

    function resetForm(): void {

        form.reset();

        previewContainer.innerHTML = "";

        clearValidation();
    }

    fromEvent(fileInput, "change")
        .subscribe(() => {

            renderImagePreview();
        });

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

                const formData = buildFormData();

                submitButton.disabled = true;

                return httpClient.postFormData$<CreateProductResponse>(
                    "Administrator/Products/AddProduct",
                    formData
                )
                    .pipe(

                        catchError((error: Error) => {

                            console.error(error);

                            return of<CreateProductResponse>({
                                success: false,
                                message:
                                    error.message ||
                                    "Unexpected error"
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
                "Product added successfully."
            );

            resetForm();
        });
});