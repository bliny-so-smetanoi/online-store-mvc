import { defer, Observable } from 'rxjs';

import { showToast } from "../toaster";
import { loadResources, t } from "../localization";

export interface HttpClientOptions {
    baseUrl?: string;
    headers?: Record<string, string>;
}

export class HttpClient {

    private baseUrl: string;

    private defaultHeaders: Record<string, string>;

    constructor(options: HttpClientOptions = {}) {

        this.baseUrl = options.baseUrl ?? '/';

        this.defaultHeaders = options.headers ?? {
            'Content-Type': 'application/json'
        };

        loadResources();
    }

    private buildUrl(url: string): string {

        if (url.indexOf('http') === 0) {
            return url;
        }

        return this.baseUrl + url;
    }

    private async request<T>(
        url: string,
        options: RequestInit = {}
    ): Promise<T> {

        const response = await fetch(
            this.buildUrl(url),
            {
                method: options.method,

                headers: options.headers,

                body: options.body,
            }
        );

        if (!response.ok) {

            const errorText = await response.text();

            showToast(
                t('Error'),
                errorText,
                'error'
            );

            throw new Error(
                `HTTP ${response.status}: ${errorText}`
            );
        }

        const contentType =
            response.headers.get('content-type');

        if (
            contentType &&
            contentType.indexOf('application/json') !== -1
        ) {

            return (await response.json()) as T;
        }

        return (await response.text()) as unknown as T;
    }

    // =========================================
    // GET
    // =========================================

    get<T>(
        url: string,
        headers?: Record<string, string>
    ): Promise<T> {

        return this.request<T>(
            url,
            {
                method: 'GET',

                headers: headers ?? this.defaultHeaders
            }
        );
    }

    get$<T>(
        url: string,
        headers?: Record<string, string>
    ): Observable<T> {

        return defer(() =>
            this.get<T>(url, headers)
        );
    }

    // =========================================
    // POST JSON
    // =========================================

    post<T>(
        url: string,
        body?: unknown,
        headers?: Record<string, string>
    ): Promise<T> {

        return this.request<T>(
            url,
            {
                method: 'POST',

                body: JSON.stringify(body),

                headers: headers ?? this.defaultHeaders
            }
        );
    }

    post$<T>(
        url: string,
        body?: unknown,
        headers?: Record<string, string>
    ): Observable<T> {

        return defer(() =>
            this.post<T>(
                url,
                body,
                headers
            )
        );
    }

    // =========================================
    // POST FORMDATA
    // =========================================

    postFormData<T>(
        url: string,
        formData: FormData,
        headers?: Record<string, string>
    ): Promise<T> {

        // IMPORTANT:
        // do NOT set Content-Type manually
        // browser will set multipart/form-data boundary

        return this.request<T>(
            url,
            {
                method: 'POST',

                body: formData,

                headers: headers
            }
        );
    }

    postFormData$<T>(
        url: string,
        formData: FormData,
        headers?: Record<string, string>
    ): Observable<T> {

        return defer(() =>
            this.postFormData<T>(
                url,
                formData,
                headers
            )
        );
    }

    putFormData$<T>(url: string, formData: FormData): Observable<T> {
        return defer(() =>
            this.request<T>(url, {
                method: "PUT",
                body: formData,
                headers: {}
            })
        );
    }
}