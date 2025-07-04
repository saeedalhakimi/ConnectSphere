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
            public const string PersonById = "{personId:guid}";
        }


    }
}
