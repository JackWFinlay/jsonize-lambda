using Newtonsoft.Json;

namespace JsonizeLambda
{
    public class Headers
    {
        [JsonProperty(PropertyName = "Content-Type")]
        public string contentType;

        [JsonProperty(PropertyName = "Access-Control-Allow-Origin")]
        public string access = "*";

        public Headers(string format)
        {
            if (format.Equals("json"))
            {
                this.contentType = "application/json";
            }
        }
    }
}