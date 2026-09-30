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

/***/ "./src/utils/localization/index.ts":
/*!*****************************************!*\
  !*** ./src/utils/localization/index.ts ***!
  \*****************************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

eval("__webpack_require__.r(__webpack_exports__);\n/* harmony export */ __webpack_require__.d(__webpack_exports__, {\n/* harmony export */   loadResources: () => (/* binding */ loadResources),\n/* harmony export */   t: () => (/* binding */ t)\n/* harmony export */ });\nlet cachedResources = null;\nasync function loadResources() {\n    var _a;\n    const locale = (_a = getLocaleFromCookie()) !== null && _a !== void 0 ? _a : \"kk\";\n    const url = `/Culture/GetResources?culture=${encodeURIComponent(locale)}`;\n    const response = await fetch(url, {\n        headers: {\n            \"Accept\": \"application/json\"\n            // optionally: \"Current-Language\": locale // если ты это читаешь на бэке\n        }\n    });\n    if (!response.ok) {\n        throw new Error(`Failed to load resources: ${response.status}`);\n    }\n    const data = (await response.json());\n    cachedResources = data;\n    return data;\n}\n// simple helper, like i18n.t()\nfunction t(key, fallback) {\n    var _a, _b;\n    if (!cachedResources) {\n        // если не загрузили – можно вернуть ключ или fallback\n        return fallback !== null && fallback !== void 0 ? fallback : key;\n    }\n    return (_b = (_a = cachedResources[key]) !== null && _a !== void 0 ? _a : fallback) !== null && _b !== void 0 ? _b : key;\n}\nfunction getLocaleFromCookie() {\n    const match = document.cookie.match(/(?:^|;\\s*)culture=([^;]+)/);\n    return match ? decodeURIComponent(match[1]) : null;\n}\n\n\n//# sourceURL=webpack://scripts/./src/utils/localization/index.ts?");

/***/ })

/******/ 	});
/************************************************************************/
/******/ 	// The require scope
/******/ 	var __webpack_require__ = {};
/******/ 	
/************************************************************************/
/******/ 	/* webpack/runtime/define property getters */
/******/ 	(() => {
/******/ 		// define getter functions for harmony exports
/******/ 		__webpack_require__.d = (exports, definition) => {
/******/ 			for(var key in definition) {
/******/ 				if(__webpack_require__.o(definition, key) && !__webpack_require__.o(exports, key)) {
/******/ 					Object.defineProperty(exports, key, { enumerable: true, get: definition[key] });
/******/ 				}
/******/ 			}
/******/ 		};
/******/ 	})();
/******/ 	
/******/ 	/* webpack/runtime/hasOwnProperty shorthand */
/******/ 	(() => {
/******/ 		__webpack_require__.o = (obj, prop) => (Object.prototype.hasOwnProperty.call(obj, prop))
/******/ 	})();
/******/ 	
/******/ 	/* webpack/runtime/make namespace object */
/******/ 	(() => {
/******/ 		// define __esModule on exports
/******/ 		__webpack_require__.r = (exports) => {
/******/ 			if(typeof Symbol !== 'undefined' && Symbol.toStringTag) {
/******/ 				Object.defineProperty(exports, Symbol.toStringTag, { value: 'Module' });
/******/ 			}
/******/ 			Object.defineProperty(exports, '__esModule', { value: true });
/******/ 		};
/******/ 	})();
/******/ 	
/************************************************************************/
/******/ 	
/******/ 	// startup
/******/ 	// Load entry module and return exports
/******/ 	// This entry module can't be inlined because the eval devtool is used.
/******/ 	var __webpack_exports__ = {};
/******/ 	__webpack_modules__["./src/utils/localization/index.ts"](0, __webpack_exports__, __webpack_require__);
/******/ 	
/******/ })()
;