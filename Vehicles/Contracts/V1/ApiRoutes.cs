


//we use routes in a separated class cause we use it in responces and routes to controller methods
namespace Vehicles.Contracts.V1
{
    public static class ApiRoutes
    {

        public const string imgsPath = @"assets\vehicleImgs\";

        public const string Root = "api";

        public const string Version = "v1";

        public const string Base = Root + "/" + Version;

        public static class Vehicles
        {
            
            public const string GetAll = Base + "/vehicles";
            public const string Default = GetAll+"/default";
            public const string GetCarsByOwner = GetAll+"/get_cars_by_owner";

            public const string Update = GetAll + "/update/{id:int}";

            public const string Delete = GetAll + "/delete/{id:int}";

            public const string Get = GetAll + "/{id:int}";

            public const string Create = GetAll + "/create";

            public const string GetImage = GetAll + "getImage/{Brand}&{UniqueNumber}";
        }
        
        public static class Owners
        {
            public const string GetAll = Base + "/owners";
            public const string Default = GetAll+"/default";
            
            public const string GetOwnersByCarUniqueNumber = GetAll+"/vehicle-unique-number/{uniqueNumber}";
            public const string Get = GetAll + "/{id}";
            
            //public const string Create = GetAll + "/create";
            //public const string Update = GetAll + "/update/{id:int}";
            
            //public const string Delete = GetAll + "/delete/{id:int}";
        }

        public static class Identity
        {
            public const string Login = Base + "/identity/login";
            
            public const string Register = Base + "/identity/register";
            
            public const string Refresh = Base + "/identity/refresh";
            public const string Delete = Base + "/identity/delete";
            public const string GetUser = Base + "/identity/get-user";
            public const string RevokeToken = Base + "/identity/revoke";
            
            public const string FacebookAuth = Base + "/identity/auth/fb";
        }
        public static class ChatRoutes
        {
            public const string Send = Base + "/chat/send";
        }
    }
}