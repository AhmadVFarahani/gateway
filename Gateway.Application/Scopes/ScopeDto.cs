using Gateway.Application.Base;

namespace Gateway.Application.Scopes;

public class ScopeDto:BaseDto
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
