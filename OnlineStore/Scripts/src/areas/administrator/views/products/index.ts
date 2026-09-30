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
    created: string;
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
            <td>${item.description}</td>            
            <td>${item.price}</td>            
            <td>${item.quantity}</td>
            <td>${dayjs(item.created).format('DD-MM-YYYY HH:mm:ss')}</td>
            <td><a href="/Administrator/Products/Edit/${item.id}">Edit</a></td>
        </tr>
    `
});

table.init();