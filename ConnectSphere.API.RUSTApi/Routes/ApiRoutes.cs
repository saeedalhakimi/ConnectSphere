namespace ConnectSphere.API.RUSTApi.Routes
{
    public static class ApiRoutes
    {
        public const string BaseRoute = "api/v{version:apiVersion}/[controller]";

        /// <summary>
        /// Routes related to Person management.
        /// </summary>
        public static class PersonRoutes
        {
            public const string GetById = "{personId}";
            public const string GetAll = ""; // GET on Base
            public const string Create = ""; // POST on Base
            public const string Update = "{personId}"; // PUT on Base/{id}
            public const string Delete = "{personId}"; // DELETE on Base/{id}

            // Subresource
            public const string GovernmentalInfo = "{personId}/governmental-info"; // POST on Base/{personId}/governmental-info

        }

        public static class  CountryRoutes
        {
            // GET /countries/{countryId}
            public const string ById = "{countryId}";

            // GET /countries/by-code/{countryCode}
            public const string ByCode = "by-code/{countryCode}";

            // GET /countries/by-name/{name}
            public const string ByName = "by-name/{name}";
        }


    }
}
