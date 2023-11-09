namespace MagicVilla_Utility
{
    public static class SD
    {
        public enum ApiType
        {
            GET,
            POST, 
            PUT, 
            DELETE
        };

        public static string SessionToken = "JWTToken";
        public static string CurrentApiVersion = "v2";
        public static string Admin = "admin";
        public static string Customer = "customer";

    }
}