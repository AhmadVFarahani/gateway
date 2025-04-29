namespace Gateway.Application.AccessPolicies.Dtos;

public class CreateAccessPolicyRequest
{
    public long? UserId { get; set; }
    public long? ApiKeyId { get; set; }
    public long ScopeId { get; set; }
}