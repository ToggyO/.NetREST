using System.Net;

namespace NetREST.Common.Errors
{
    public static class ErrorCodes
    {
        public const HttpStatusCode UnprocessableEntity = (HttpStatusCode) 422;

        public static class Common
        {
            public const string NotFound = "not_found";
            public const string UnprocessableEntity = "unprocessable_entity";
            public const string FieldInvalidLength = "common.field_invalid_length";
            public const string FieldInvalid = "common.field_invalid";
            public const string FieldNotValidChars = "common.field_not_valid_chars";
            public const string FieldDuplicate = "common.field_duplicate";
            public const string BadParameters = "common.bad_parameters";
        }
        
        public static class Global
        {
            public const string BusinessConflict = "business_conflict";
        }
        
        public static class Business
        {
            public const string EmailExists = "bus.email_already_exists";
            public const string InvalidEmail = "bus.invalid_email";
            public const string UserDoesNotExists = "bus.user_does_not_exists";
            public const string UserIsDeleted = "bus.user_is_deleted";
            public const string PasswordChangeRequestInvalid = "bus.password_change_request_invalid";
        }

        
        public static class Security
        {
            public const string Unauthorized = "security_error";
            public const string PermissionDenied = "permission_error";
            public const string Forbidden = "forbidden";
            public const string FieldAccessToken = "accessToken";
            public const string AuthDataInvalid = "sec.auth_data_invalid";
            public const string TokenInvalid = "sec.token_invalid";
            public const string AccessTokenInvalid = "sec.access_token_invalid";
            public const string AccessTokenExpired = "sec.access_token_expired";
            public const string GoogleTokenInvalid = "sec.google_token_invalid";
            public const string RefreshTokenInvalid = "sec.refresh_token_invalid";
            public const string RefreshTokenExpired = "sec.refresh_token_expired";
            public const string ConfirmationCodeInvalid = "sec.confirmation_code_invalid";
        }
        
        
        public static class System
        {
            public const string InternalError = "internal_error";
        }
    }
}