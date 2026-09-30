namespace Resource;

public static class CatalogResources
{
    public static string Get(string key) =>
        SharedResources.ResourceManager.GetString(key, SharedResources.Culture) ?? key;
}