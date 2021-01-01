namespace NetREST.Common.Errors
{
    public static class ErrorMessages
    {
        public static class Common
        {
            public const string UnprocessableEntity = "Invalid entity or business request";
            public const string FieldInvalid = "Invalid field";
            public const string NotFound = "Not found";
            public const string BadParameters = "Invalid input parameters";
        }

        public static class Business
        {
            public const string InvalidEmail = "Email is invalid";
            public const string EmailExists = "This email is already registered. Sign in or use different email to register";
            public const string UserDoesNotExists = "User does not exists";
            public const string PasswordChangeRequestInvalid = "Password change request is invalid";
            public const string PasswordChangeCodeInvalid = "Code for change password invalid";
        }
        
        public static class Security
        {
            public const string Unauthorized = "Unauthorized";
            public const string PermissionDenied = "Permission denied";
            public const string AuthDataInvalid = "Auth data invalid";
            public const string TokenInvalid = "Token invalid";
            public const string AccessTokenInvalid = "Access token invalid";
            public const string AccessTokenExpired = "Access token expired";
            public const string GoogleTokenInvalid = "Google token invalid";
            public const string RefreshTokenInvalid = "Refresh token invalid";
            public const string RefreshTokenExpired = "Refresh token expired";
        }
        
        public static class System
        {
            public const string InternalError = "Internal error";
        }
    }
}