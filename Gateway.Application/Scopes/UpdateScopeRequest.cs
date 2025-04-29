namespace Gateway.Application.Scopes;

public class UpdateScopeRequest
{
    public string DisplayName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}