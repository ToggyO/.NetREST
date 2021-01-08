using System;
using System.Security.Claims;

namespace NetREST.Common.Extensions
{
    public static partial class ClaimTypes
    {
        public static string UserId => "user_id";

        public static int? GetUserId(this ClaimsPrincipal principal)
            => Int32.TryParse(principal.FindFirst(UserId)?.Value, out int result) ? result : (int?)null;
    }
}