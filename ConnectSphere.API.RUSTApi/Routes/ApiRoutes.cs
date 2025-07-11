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
            public const string CreatePerson = "create";
            public const string PersonById = "get-by-id/{personId}";
            public const string GetPersons = "get-all-persons";
            public const string UpdatePerson = "update/{personId}";
            public const string DeletePerson = "delete/{personId}";
        }

        public static class  CountryRoutes
        {
            public const string GetByCountryID = "get-by-id/{countryId}";
            public const string GetByCountryCode = "get-by-code/{countryCode}";
            public const string GetByCountryName = "get-by-name/{name}";
        }


    }
}
