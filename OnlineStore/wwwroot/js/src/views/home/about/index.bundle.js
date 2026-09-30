/*
 * ATTENTION: The "eval" devtool has been used (maybe by default in mode: "development").
 * This devtool is neither made for production nor for readable output files.
 * It uses "eval()" calls to create a separate source file in the browser devtools.
 * If you are trying to read the output file, select a different devtool (https://webpack.js.org/configuration/devtool/)
 * or disable the default devtool with "devtool: false".
 * If you are looking for production-ready output files, see mode: "production" (https://webpack.js.org/configuration/mode/).
 */
/******/ (() => { // webpackBootstrap
/******/ 	"use strict";
/******/ 	var __webpack_modules__ = ({

/***/ "./src/views/home/about/index.ts":
/*!***************************************!*\
  !*** ./src/views/home/about/index.ts ***!
  \***************************************/
/***/ (() => {

eval("\n// import $ from \"jquery\";\n// import {\n//     catchError,\n//     fromEvent,\n//     map,\n//     of,\n//     switchMap,\n// } from \"rxjs\";\n// import {HttpClient} from \"../../../utils/httpClient\";\n// import {loaderSpinner} from \"../../../utils/loaderSpinner\";\n// import {showModal} from \"../../../utils/modal\";\n// import dayjs from \"dayjs\";\n// import {TodoItem} from \"./dtos/todo.item\";\n// import {loadResources, t} from \"../../../utils/localization\";\n// import {PagedTable} from \"../../../shared/pagedTable\";\n//\n// const api = new HttpClient();\n//\n// const sendTitle = $('#viewAbout-sendTitle');\n//\n// interface CreateResult {\n//     success: boolean;\n//     error?: unknown;\n// }\n//\n// const table = new PagedTable<TodoItem>({\n//     tbodySelector: '#about-table-body',\n//     paginationSelector: '#about-pagination',\n//     searchSelector: '.column-search',\n//     url: 'Home/GetList',\n//     pageSize: 10,\n//     emptyText: 'No data',\n//     errorText: 'Error loading data',\n//     loader: isLoading => loaderSpinner(isLoading),\n//     request: <TResponse>(url: string) => api.get$<TResponse>(url),\n//     renderRow: item => `\n//         <tr>\n//             <td>${item.id}</td>\n//             <td>${item.title}</td>\n//             <td>${dayjs(item.created).format('DD-MM-YYYY HH:mm:ss')}</td>\n//         </tr>\n//     `\n// });\n//\n// table.init();\n//\n// fromEvent(sendTitle, 'click').pipe(\n//     map(() => String($('#viewAbout-textValue').val() ?? '').trim()),\n//     switchMap(value => {\n//         if (!value) {\n//             return of({\n//                 success: false,\n//                 error: 'Title is required'\n//             } as CreateResult);\n//         }\n//\n//         return api.post$('Home/CreateItem/', { title: value }).pipe(\n//             map(() => ({ success: true } as CreateResult)),\n//             catchError(err => {\n//                 console.error(err);\n//                 return of({\n//                     success: false,\n//                     error: err\n//                 } as CreateResult);\n//             })\n//         );\n//     })\n// ).subscribe(result => {\n//     if (result.success) {\n//         showModal('Saved!', '<p>Saved successfully!</p>', () => of(null));\n//         $('#viewAbout-textValue').val('');\n//         table.reloadFirstPage();\n//         return;\n//     }\n//\n//     showModal(t('Error'), `<p>${String(result.error ?? 'Unknown error')}</p>`, () => of(null));\n// });\n\n\n//# sourceURL=webpack://scripts/./src/views/home/about/index.ts?");

/***/ })

/******/ 	});
/************************************************************************/
/******/ 	
/******/ 	// startup
/******/ 	// Load entry module and return exports
/******/ 	// This entry module can't be inlined because the eval devtool is used.
/******/ 	var __webpack_exports__ = {};
/******/ 	__webpack_modules__["./src/views/home/about/index.ts"]();
/******/ 	
/******/ })()
;