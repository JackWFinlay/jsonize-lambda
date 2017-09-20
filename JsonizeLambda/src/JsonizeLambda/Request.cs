using System.Collections.Generic;
using Newtonsoft.Json;

namespace JsonizeLambda
{
    public class Request
    {
        [JsonProperty("queryStringParameters")]
        public Dictionary<string, string> queryStringParameters { get; set; }
    }
}