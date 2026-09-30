// import $ from "jquery";
// import {
//     catchError,
//     fromEvent,
//     map,
//     of,
//     switchMap,
// } from "rxjs";
// import {HttpClient} from "../../../utils/httpClient";
// import {loaderSpinner} from "../../../utils/loaderSpinner";
// import {showModal} from "../../../utils/modal";
// import dayjs from "dayjs";
// import {TodoItem} from "./dtos/todo.item";
// import {loadResources, t} from "../../../utils/localization";
// import {PagedTable} from "../../../shared/pagedTable";
//
// const api = new HttpClient();
//
// const sendTitle = $('#viewAbout-sendTitle');
//
// interface CreateResult {
//     success: boolean;
//     error?: unknown;
// }
//
// const table = new PagedTable<TodoItem>({
//     tbodySelector: '#about-table-body',
//     paginationSelector: '#about-pagination',
//     searchSelector: '.column-search',
//     url: 'Home/GetList',
//     pageSize: 10,
//     emptyText: 'No data',
//     errorText: 'Error loading data',
//     loader: isLoading => loaderSpinner(isLoading),
//     request: <TResponse>(url: string) => api.get$<TResponse>(url),
//     renderRow: item => `
//         <tr>
//             <td>${item.id}</td>
//             <td>${item.title}</td>
//             <td>${dayjs(item.created).format('DD-MM-YYYY HH:mm:ss')}</td>
//         </tr>
//     `
// });
//
// table.init();
//
// fromEvent(sendTitle, 'click').pipe(
//     map(() => String($('#viewAbout-textValue').val() ?? '').trim()),
//     switchMap(value => {
//         if (!value) {
//             return of({
//                 success: false,
//                 error: 'Title is required'
//             } as CreateResult);
//         }
//
//         return api.post$('Home/CreateItem/', { title: value }).pipe(
//             map(() => ({ success: true } as CreateResult)),
//             catchError(err => {
//                 console.error(err);
//                 return of({
//                     success: false,
//                     error: err
//                 } as CreateResult);
//             })
//         );
//     })
// ).subscribe(result => {
//     if (result.success) {
//         showModal('Saved!', '<p>Saved successfully!</p>', () => of(null));
//         $('#viewAbout-textValue').val('');
//         table.reloadFirstPage();
//         return;
//     }
//
//     showModal(t('Error'), `<p>${String(result.error ?? 'Unknown error')}</p>`, () => of(null));
// });