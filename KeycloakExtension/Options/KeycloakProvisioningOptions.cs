namespace KeycloakExtension.Options;

public sealed class KeycloakProvisioningOptions
{
    public string BaseUrl { get; set; } = default!;
    public string Realm { get; set; } = default!;

    // Keycloak URL reachable from the browser (differs from the internal BaseUrl behind a reverse proxy); empty — BaseUrl
    public string? PublicUrl { get; set; }

    public string AdminConsoleUrl =>
        $"{(string.IsNullOrWhiteSpace(PublicUrl) ? BaseUrl : PublicUrl).TrimEnd('/')}/admin/master/console/#/{Realm}";

    public string AdminClientId { get; set; } = default!;
    public string AdminClientSecret { get; set; } = default!;

    public string UserTokenClientId { get; set; } = default!;
    public string UserTokenClientSecret { get; set; } = default!;

    public string VisitorRealmRole { get; set; } = "visitor";

    // Keycloak User Profile requires an email; visitors sign in by phone, so those without one get a technical email
    public string VisitorEmailDomain { get; set; } = "visitors.onlinestore.local";
}