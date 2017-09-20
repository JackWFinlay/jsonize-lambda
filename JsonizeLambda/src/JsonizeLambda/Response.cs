using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonizeLambda
{
    public class Response
    {
        [JsonProperty(Order = 1)]
        public bool isBase64Encoded = false;

        [JsonProperty(Order = 2)]
        public Headers headers {get; set;}

        [JsonProperty(Order = 3)]
        public int statusCode = 200;

        [JsonProperty(Order = 4)]
        public string body {get; set;}

        public Response(int statusCode, string format, string body){

            if (format == "json")
            {
                headers = new Headers();
            } else {
                headers = null;
            }

            this.body = body;
            this.statusCode = statusCode;
        }
    }
}