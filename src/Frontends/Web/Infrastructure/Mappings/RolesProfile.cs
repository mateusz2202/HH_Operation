using BlazorApp.Application.Requests.Identity;
using BlazorApp.Application.Responses.Identity;
using System.Linq;

namespace BlazorApp.Client.Infrastructure.Mappings;


public static class RoleProfile
{
    public static PermissionResponse ToPermissionResponse(this PermissionRequest permissionRequest)
    {
        return new PermissionResponse()
        {
            RoleId = permissionRequest.RoleId,
            RoleClaims = permissionRequest?.RoleClaims?.Select(x => x.ToRoleClaimResponse()).ToList()
        };
    }
    public static PermissionRequest ToPermissionRequest(this PermissionResponse permissionResponse)
    {
        return new PermissionRequest()
        {
            RoleId = permissionResponse.RoleId,
            RoleClaims = permissionResponse?.RoleClaims?.Select(x => x.ToRoleClaimResponse()).ToList()
        };
    }

    public static RoleClaimResponse ToRoleClaimResponse(this RoleClaimRequest roleClaimRequest)
    {
        return new RoleClaimResponse()
        {
            Id = roleClaimRequest.Id,
            RoleId = roleClaimRequest.RoleId,
            Type = roleClaimRequest.Type,
            Value = roleClaimRequest.Value,
            Description = roleClaimRequest.Description,
            Group = roleClaimRequest.Group,
            Selected = roleClaimRequest.Selected
        };
    }
    public static RoleClaimRequest ToRoleClaimResponse(this RoleClaimResponse roleClaimResponse)
    {
        return new RoleClaimRequest()
        {
            Id = roleClaimResponse.Id,
            RoleId = roleClaimResponse.RoleId,
            Type = roleClaimResponse.Type,
            Value = roleClaimResponse.Value,
            Description = roleClaimResponse.Description,
            Group = roleClaimResponse.Group,
            Selected = roleClaimResponse.Selected
        };
    }
}