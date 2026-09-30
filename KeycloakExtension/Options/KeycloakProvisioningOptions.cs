namespace KeycloakExtension.Options;

public sealed class KeycloakProvisioningOptions
{
    public string BaseUrl { get; set; } = default!;
    public string Realm { get; set; } = default!;

    public string AdminClientId { get; set; } = default!;
    public string AdminClientSecret { get; set; } = default!;

    public string UserTokenClientId { get; set; } = default!;
    public string UserTokenClientSecret { get; set; } = default!;

    public string VisitorRealmRole { get; set; } = "visitor";
}