using System.Net;

namespace MagicVilla_VillaAPI.Models
{
    public class APIResponse
    {
        public APIResponse()
        {
            ErrorMessages = new List<string>();
            
        }
        public HttpStatusCode Status { get; set; }
        public bool IsSuccess { get; set; } = true;
        public List<string> ErrorMessages { get; set; }
        public object Result { get; set; }

        public bool SuccessValueHandler(HttpStatusCode statusCode)
        {
            int code = ((int)statusCode) / 100;
            if ( !code.Equals(2))
            return false;

            return true;
        }
    }
}
