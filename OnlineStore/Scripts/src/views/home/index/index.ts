import type {PagedResult} from "../../../shared/pagedTable";

interface Product {
    id: string;
    name: string;
    description: string;
    price: number;
    imageId: string | null;
}

const catalog = document.querySelector<HTMLElement>("#product-catalog")!;
const list = document.querySelector<HTMLDivElement>("#product-list")!;
const template = document.querySelector<HTMLTemplateElement>("#product-card-template")!;
const form = document.querySelector<HTMLFormElement>("#product-search")!;
const searchInput = document.querySelector<HTMLInputElement>("#product-search-input")!;
const showMore = document.querySelector<HTMLButtonElement>("#products-show-more")!;
const retry = document.querySelector<HTMLButtonElement>("#products-retry")!;
const loading = document.querySelector<HTMLElement>("#products-loading")!;
const empty = document.querySelector<HTMLElement>("#products-empty")!;
const error = document.querySelector<HTMLElement>("#products-error")!;
const priceFormat = new Intl.NumberFormat("en-US", {minimumFractionDigits: 2, maximumFractionDigits: 2});

let page = 0;
let totalPages = 0;
let searchString = "";
let activeRequest: AbortController | null = null;

function renderProducts(products: Product[]): DocumentFragment {
    const fragment = document.createDocumentFragment();

    products.forEach(product => {
        const card = template.content.cloneNode(true) as DocumentFragment;
        card.querySelector<HTMLElement>("[data-product-name]")!.textContent = product.name;
        card.querySelector<HTMLElement>("[data-product-description]")!.textContent = product.description;
        card.querySelector<HTMLElement>("[data-product-price]")!.textContent = priceFormat.format(product.price);

        if (product.imageId) {
            const image = card.querySelector<HTMLImageElement>("img")!;
            const placeholder = card.querySelector<HTMLElement>("[data-product-no-image]")!;
            const imageUrl = new URL(catalog.dataset.imageUrl!, window.location.href);
            imageUrl.searchParams.set("id", product.imageId);
            image.alt = product.name;
            image.addEventListener("error", () => {
                image.hidden = true;
                placeholder.hidden = false;
            });
            image.src = imageUrl.toString();
            image.hidden = false;
            placeholder.hidden = true;
        }

        fragment.appendChild(card);
    });

    return fragment;
}

async function loadNextPage(): Promise<void> {
    if (activeRequest) return;

    const request = new AbortController();
    activeRequest = request;
    loading.hidden = false;
    error.hidden = true;
    empty.hidden = true;
    showMore.disabled = true;
    list.setAttribute("aria-busy", "true");

    const url = new URL(catalog.dataset.productsUrl!, window.location.href);
    url.searchParams.set("page", String(page + 1));
    url.searchParams.set("searchString", searchString);

    try {
        const response = await fetch(url.toString(), {
            signal: request.signal,
            headers: {"Accept": "application/json"}
        });
        if (!response.ok) throw new Error(`HTTP ${response.status}`);

        const result: PagedResult<Product> = await response.json();
        if (activeRequest !== request) return;

        list.appendChild(renderProducts(result.items));
        page = result.page;
        totalPages = result.totalPages;
        empty.hidden = list.childElementCount > 0;
    } catch {
        if (activeRequest !== request) return;
        error.hidden = false;
    } finally {
        if (activeRequest === request) {
            activeRequest = null;
            loading.hidden = true;
            showMore.disabled = false;
            showMore.hidden = page >= totalPages || !error.hidden;
            list.setAttribute("aria-busy", "false");
        }
    }
}

form.addEventListener("submit", event => {
    event.preventDefault();
    activeRequest?.abort();
    activeRequest = null;
    searchString = searchInput.value.trim();
    page = 0;
    totalPages = 0;
    list.replaceChildren();
    showMore.hidden = true;
    void loadNextPage();
});

showMore.addEventListener("click", () => void loadNextPage());
retry.addEventListener("click", () => void loadNextPage());
void loadNextPage();
