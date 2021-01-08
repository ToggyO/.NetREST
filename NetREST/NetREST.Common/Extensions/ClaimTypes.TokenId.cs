using System;
using System.Security.Claims;

namespace NetREST.Common.Extensions
{
    public static partial class ClaimTypes
    {
        public static string TokenId => "token_id";

        public static Guid? GetTokenId(this ClaimsPrincipal principal)
            => Guid.TryParse(principal.FindFirst(TokenId)?.Value, out Guid result) ? result : (Guid?)null;
    }
}