import {PagedTable} from "../../../../shared/pagedTable";
import {TodoItem} from "../storybook/dtos/todo.item";
import {loaderSpinner} from "../../../../utils/loaderSpinner";
import dayjs from "dayjs";
import {HttpClient} from "../../../../utils/httpClient";

const httpClient = new HttpClient();

interface Product {
    id: string;
    name: string;
    description: string;
    price: number;
    quantity: number;
    categoryName: string | null;
    created: string;
}

function escapeHtml(value: string): string {
    const element = document.createElement("span");
    element.textContent = value;
    return element.innerHTML;
}

const table = new PagedTable<Product>({
    tbodySelector: '#about-table-body',
    paginationSelector: '#about-pagination',
    searchSelector: '.column-search',
    url: 'Administrator/Products/GetList',
    pageSize: 10,
    emptyText: 'No data',
    errorText: 'Error loading data',
    loader: isLoading => loaderSpinner(isLoading),
    request: <TResponse>(url: string) => httpClient.get$<TResponse>(url),
    renderRow: item => `
        <tr>
            <td>${item.id}</td>
            <td>${item.name}</td>            
            <td>${item.price}</td>            
            <td>${item.quantity}</td>
            <td>${escapeHtml(item.categoryName || "")}</td>
            <td>${dayjs(item.created).format('DD-MM-YYYY HH:mm:ss')}</td>
            <td><a class="btn btn-sm btn-outline-primary" href="/Administrator/Products/Edit/${item.id}">Edit</a></td>
        </tr>
    `
});

table.init();