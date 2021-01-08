using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using NetREST.BLL.Factories;
using NetREST.Common.Enums;
using NetREST.Common.Response;

namespace NetREST.API.Filters
{
    public class AuthorizationFilter : AuthorizeAttribute, IAsyncAuthorizationFilter
    {
        private ITokensFactory _tokensFactory;

        public AuthorizationFilter()
        {
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme;
        }
        
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            _tokensFactory = (ITokensFactory) context.HttpContext.RequestServices.GetRequiredService(typeof(ITokensFactory));

            if (context.ActionDescriptor is ControllerActionDescriptor descriptor)
            {
                var attributes = descriptor.MethodInfo.CustomAttributes;
                if (attributes.Any(x => x.AttributeType == typeof(AllowAnonymousAttribute)))
                    return;
            }

            var authHeader = (String) context.HttpContext.Request.Headers["Authorization"];
            TokenStatus tokenStatus = TokenStatus.Valid;

            if (!string.IsNullOrEmpty(authHeader))
            {
                var token = GetCleanToken(authHeader);
                tokenStatus = await ValidateToken(token);

                if (tokenStatus == TokenStatus.Valid)
                    return;
            }

            var error = GetErrorByTokenStatus(tokenStatus);

            context.Result = new ObjectResult(error)
            {
                StatusCode = (int) error.HttpStatusCode,
            };
        }

        private ErrorResponse GetErrorByTokenStatus(TokenStatus tokenStatus) =>
            tokenStatus switch
            {
                TokenStatus.Expired => new AccessTokenExpiredErrorResponse(),
                _ => new AccessTokenInvalidErrorResponse(),
            };

        private static string GetCleanToken(string authHeader)
        {
            var index = authHeader.IndexOf("Bearer ", StringComparison.CurrentCultureIgnoreCase);
            return index < 0 ? authHeader : authHeader.Remove(index, "Bearer ".Length);
        }

        private async Task<TokenStatus> ValidateToken(string token) =>
            await Task.Run(() => _tokensFactory.ValidateToken(token, out _));
    }
}