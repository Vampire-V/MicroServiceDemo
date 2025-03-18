using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

namespace ServiceA.Authorization
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public string PermissionName { get; }
        public string Action { get; set; }

        public PermissionRequirement(string permissionName, string action)
        {
            PermissionName = permissionName;
            Action = action;
        }
    }

    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PermissionHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement
        )
        {
            // bool hasPermission = false;
            //------------
            // ดึง Header ที่มี Permissions
            var headers = _httpContextAccessor.HttpContext?.Request.Headers;
            if (headers == null || !headers.ContainsKey("X-Claim-Permissions"))
            {
                context.Fail();
                return Task.CompletedTask;
            }
            // ดึงและ Deserialize Permissions จาก Header
            var permissionsJson = headers["X-Claim-Permissions"].ToString();
            var permissions = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(
                permissionsJson
            );
            // ตรวจสอบว่า User มีสิทธิ์ตาม Requirement หรือไม่
            if (
                permissions != null
                && permissions.TryGetValue(requirement.PermissionName, out var actions)
                && actions.Contains(requirement.Action)
            )
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
            return Task.CompletedTask;
        }
    }
}
