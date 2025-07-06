using Gateway.Domain.Enums;

namespace Gateway.Domain.Entities;

public class User:BaseEntity
{
    
    public string UserName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public UserType UserType { get; set; } = UserType.Individual;

  
    public long CompanyId { get; set; }
    public Company Company { get; set; } = default!;

    public long RoleId { get; set; }
    public Role Role { get; set; } = default!;

    public ICollection<AccessPolicy> AccessPolicies { get; set; } = new List<AccessPolicy>();
    public ICollection<ApiKey> ApiKeys { get; set; } = new List<ApiKey>();


    public void addAccessPolicy(AccessPolicy access)
    {
        if (AccessPolicies.Any(c=>c.ApiKeyId==access.ApiKeyId && c.ScopeId==access.ScopeId))
        {
            throw new KeyNotFoundException("Duplicate Access Policy");
        }
        AccessPolicies.Add(access);
    }
    public void deleteAccessPolicy(long accessPolicyId)
    {
        var accessPolicy = AccessPolicies.FirstOrDefault(s => s.Id == accessPolicyId)
            ?? throw new KeyNotFoundException("Access Policy not found");
        AccessPolicies.Remove(accessPolicy);
    }
    //public void updateAcccessPolicy(long id, ApiKey, DateTime? endDate, bool autoRenew, bool isActive)
    //{

    //}

}
