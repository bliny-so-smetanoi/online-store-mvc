import { fromEvent, Observable, of } from 'rxjs';
import {
    debounceTime,
    map,
    tap,
    switchMap,
    catchError,
    filter as rxFilter,
    finalize
} from 'rxjs/operators';
import $ from 'jquery';

export interface PagedResult<T> {
    items: T[];
    page: number;
    totalPages: number;
}

export interface PagedTableConfig<T> {
    tbodySelector: string;
    paginationSelector: string;
    searchSelector: string;
    url: string;
    pageSize?: number;
    emptyText?: string;
    errorText?: string;
    renderRow: (item: T) => string;
    loader?: (isLoading: boolean) => void;
    request: <TResponse>(url: string) => Observable<TResponse>;
}

export class PagedTable<T> {
    private readonly $tbody: JQuery;
    private readonly $pagination: JQuery;
    private readonly $searchInputs: JQuery;
    private readonly url: string;
    private readonly pageSize: number;
    private readonly emptyText: string;
    private readonly errorText: string;
    private readonly renderRowFn: (item: T) => string;
    private readonly loader?: (isLoading: boolean) => void;
    private readonly request: <TResponse>(url: string) => Observable<TResponse>;

    private currentPage = 1;
    private currentFilters: Record<string, string> = {};

    constructor(config: PagedTableConfig<T>) {
        this.$tbody = $(config.tbodySelector);
        this.$pagination = $(config.paginationSelector);
        this.$searchInputs = $(config.searchSelector);
        this.url = config.url;
        this.pageSize = config.pageSize ?? 10;
        this.emptyText = config.emptyText ?? 'No data';
        this.errorText = config.errorText ?? 'Error loading data';
        this.renderRowFn = config.renderRow;
        this.loader = config.loader;
        this.request = config.request;
    }

    public init(): void {
        this.bindFilters();
        this.bindPagination();
        this.reload();
    }

    public reload(): void {
        this.fetchWithFilters(this.currentFilters, this.currentPage)
            .subscribe(result => this.render(result));
    }

    public reloadFirstPage(): void {
        this.currentPage = 1;
        this.reload();
    }

    public setPage(page: number): void {
        this.currentPage = page;
    }

    public getCurrentPage(): number {
        return this.currentPage;
    }

    public getCurrentFilters(): Record<string, string> {
        return { ...this.currentFilters };
    }

    private bindFilters(): void {
        fromEvent(this.$searchInputs, 'keyup').pipe(
            debounceTime(300),
            map(() => this.collectFilters()),
            tap(filters => {
                this.currentFilters = filters;
                this.currentPage = 1;
            }),
            switchMap(filters => this.fetchWithFilters(filters, this.currentPage))
        ).subscribe(result => this.render(result));
    }

    private bindPagination(): void {
        fromEvent(this.$pagination, 'click').pipe(
            rxFilter((event: Event) => {
                const target = event.target as HTMLElement;
                const link = target.closest('.page-link[data-page]') as HTMLElement | null;

                if (!link) {
                    return false;
                }

                const li = link.closest('.page-item');
                if (li?.classList.contains('disabled') || li?.classList.contains('active')) {
                    return false;
                }

                const page = parseInt(link.dataset.page || '1', 10);
                return !Number.isNaN(page) && page > 0;
            }),
            map((event: Event) => {
                event.preventDefault();
                const target = event.target as HTMLElement;
                const link = target.closest('.page-link[data-page]') as HTMLElement;
                return parseInt(link.dataset.page || '1', 10);
            }),
            tap(page => {
                this.currentPage = page;
            }),
            switchMap(page => this.fetchWithFilters(this.currentFilters, page))
        ).subscribe(result => this.render(result));
    }

    private collectFilters(): Record<string, string> {
        const filters: Record<string, string> = {};

        this.$searchInputs.each((_, el) => {
            const $el = $(el);
            const field = String($el.data('field') ?? '');
            const value = String($el.val() ?? '').trim();

            if (field && value) {
                filters[field] = value;
            }
        });

        return filters;
    }

    private fetchWithFilters(filters: Record<string, string>, page: number): Observable<PagedResult<T> | null> {
        this.loader?.(true);

        const params = new URLSearchParams();

        Object.entries(filters).forEach(([key, value]) => {
            params.append(key, value);
        });

        params.append('page', page.toString());
        params.append('pageSize', this.pageSize.toString());

        const requestUrl = `${this.url}?${params.toString()}`;

        return this.request<PagedResult<T>>(requestUrl).pipe(
            catchError(err => {
                console.error(err);
                return of(null);
            }),
            finalize(() => this.loader?.(false))
        );
    }

    private render(paged: PagedResult<T> | null): void {
        if (!paged) {
            this.$tbody.html(
                `<tr><td colspan="100%" class="text-center text-muted">${this.errorText}</td></tr>`
            );
            this.$pagination.empty();
            return;
        }

        this.renderTableBody(paged.items);
        this.renderPagination(paged.page, paged.totalPages);
    }

    private renderTableBody(items: T[]): void {
        if (!items || items.length === 0) {
            this.$tbody.html(
                `<tr><td colspan="100%" class="text-center text-muted">${this.emptyText}</td></tr>`
            );
            return;
        }

        const rows = items.map(item => this.renderRowFn(item)).join('');
        this.$tbody.html(rows);
    }

    private renderPagination(page: number, totalPages: number): void {
        if (totalPages <= 1) {
            this.$pagination.empty();
            return;
        }

        let html = '';

        const prevDisabled = page === 1 ? ' disabled' : '';
        const nextDisabled = page === totalPages ? ' disabled' : '';

        html += `
            <li class="page-item${prevDisabled}">
                <a class="page-link" href="#" data-page="${page - 1}">&laquo;</a>
            </li>`;

        for (let p = 1; p <= totalPages; p++) {
            const active = p === page ? ' active' : '';
            html += `
                <li class="page-item${active}">
                    <a class="page-link" href="#" data-page="${p}">${p}</a>
                </li>`;
        }

        html += `
            <li class="page-item${nextDisabled}">
                <a class="page-link" href="#" data-page="${page + 1}">&raquo;</a>
            </li>`;

        this.$pagination.html(html);
    }
}