export type ResourcesMap = Record<string, string>;

let cachedResources: ResourcesMap | null = null;

export async function loadResources(): Promise<ResourcesMap> {
    const locale = getLocaleFromCookie() ?? "kk";
    const url = `/Culture/GetResources?culture=${encodeURIComponent(locale)}`;

    const response = await fetch(url, {
        headers: {
            "Accept": "application/json"
            // optionally: "Current-Language": locale // if the backend reads it
        }
    });

    if (!response.ok) {
        throw new Error(`Failed to load resources: ${response.status}`);
    }

    const data = (await response.json()) as ResourcesMap;
    cachedResources = data;
    return data;
}

// simple helper, like i18n.t()
export function t(key: string, fallback?: string): string {
    if (!cachedResources) {
        // if not loaded – return the key or the fallback
        return fallback ?? key;
    }

    return cachedResources[key] ?? fallback ?? key;
}

function getLocaleFromCookie(): string | null {
    const match = document.cookie.match(/(?:^|;\s*)culture=([^;]+)/);
    return match ? decodeURIComponent(match[1]) : null;
}
