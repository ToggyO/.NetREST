namespace NetREST.Common.Errors
{
    public static class ErrorFields
    {
        public static class User
        {
            public const string Id = "id";
            public const string Name = "name";
            public const string Email = "email";
            public const string Token = "token";
            public const string GoogleToken = "googleToken";
            public const string Password = "password";
        }

        public static class Company
        {
            public const string Id = "id";
            public const string Name = "name";
        }

        public static class Team
        {
            public const string Id = "id";
            public const string Name = "name";
        }


        public static class Project
        {
            public const string Id = "id";
            public const string Name = "name";
        }


        public static class Resource
        {
            public const string FileName = "filename";
            //public const string Path = "path";
        }
    }
}