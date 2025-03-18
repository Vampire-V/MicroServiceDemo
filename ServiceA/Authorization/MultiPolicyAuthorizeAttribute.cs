using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ServiceA.Authorization
{
    [AttributeUsage(
        AttributeTargets.Class | AttributeTargets.Method,
        AllowMultiple = false,
        Inherited = false
    )]
    public class MultiPolicyAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[] _policies;

        public MultiPolicyAuthorizeAttribute(params string[] policies)
        {
            _policies = policies;
        }

        public async void OnAuthorization(AuthorizationFilterContext context)
        {
            var authorizationService =
                context.HttpContext.RequestServices.GetRequiredService<IAuthorizationService>();
            var user = context.HttpContext.User;

            foreach (var policy in _policies)
            {
                var result = await authorizationService.AuthorizeAsync(user, policy);
                if (result.Succeeded)
                {
                    return; // หากผ่าน Policy ใด Policy หนึ่ง อนุญาตให้เข้าถึง
                }
            }

            // หากไม่ผ่าน Policy ใดเลย
            context.Result = new ForbidResult();
        }
    }
}
