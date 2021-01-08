namespace NetREST.Common.Errors
{
    public static class ErrorMessages
    {
        public static class Common
        {
            public const string UnprocessableEntity = "Invalid entity or business request";
            public const string FieldInvalid = "Invalid field";
            public const string NotFound = "Not found";
            public const string BadParameters = "Bad parameters";
        }

        public static class Business
        {
            public const string InvalidEmail = "Email is invalid";
            public const string EmailExists = "This email is already registered. Sign in or use different email to register";
            public const string UserDoesNotExists = "User does not exists";
            public const string UserIsDeleted = "You don't have access to Anova system - your account is deleted";
            public const string UserIsBlocked = "You cannot request profile information from Research site(s) - your account is blocked";
            public const string InvalidRegistrationStep = "User already completed this step";
            public const string PasswordChangeRequestInvalid = "Password change request is invalid";
            public const string PasswordChangeCodeInvalid = "Code for change password invalid";
        }

        public static class Global
        {
            public const string BusinessConflict = "Business Conflict";
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
            public const string InviteInvalid = "Invite is invalid";
            public const string ConfirmationCodeInvalid = "Code is invalid";
        }

        public static class System
        {
            public const string InternalError = "Internal error";
        }
    }
}