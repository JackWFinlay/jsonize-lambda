using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Amazon.Lambda.Core;

using JackWFinlay.Jsonize;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace JsonizeLambda
{
    public class Function
    {
        /// <summary>
        /// Returns the requested json
        /// </summary>
        /// <param name="request">The Request</param>
        /// <param name="context">The Lambda Context</param>
        /// <returns>The requested URL as Json either as a JObject or String</returns>
        public async Task<dynamic> FunctionHandlerAsync(Request request, ILambdaContext context)
        {
            string urlExists;
            string url = WebUtility.UrlDecode(request.queryStringParameters
                .TryGetValue("url", out urlExists) ? urlExists : "");

            string formatExists;
            string format = request.queryStringParameters
                .TryGetValue("format", out formatExists) ? formatExists : "json";

            string emptyTextNodeHandlingExists;
            string emptyTextNodeHandling = request.queryStringParameters
                .TryGetValue("emptyTextNodeHandling", out emptyTextNodeHandlingExists) ? emptyTextNodeHandlingExists : "ignore";

            string nullValueHandlingExists;
            string nullValueHandling = request.queryStringParameters
                .TryGetValue("nullValueHandling", out nullValueHandlingExists) ? nullValueHandlingExists : "ignore";

            string textTrimHandlingExists;
            string textTrimHandling = request.queryStringParameters
                .TryGetValue("textTrimHandling", out textTrimHandlingExists) ? textTrimHandlingExists : "trim";

            string classAttributeHandlingExists;
            string classAttributeHandling = request.queryStringParameters
                .TryGetValue("textTrimHandling", out classAttributeHandlingExists) ? classAttributeHandlingExists : "array";

            string renderJavascriptExists;
            string renderJavascript = request.queryStringParameters
                .TryGetValue("renderJavascript", out renderJavascriptExists) ? renderJavascriptExists : "false";

            string body;
            int responseCode;
            try
            {
                body = await JsonConvert.SerializeObjectAsync(await Convert(url: url,
                                    format: format,
                                    emptyTextNodeHandling: emptyTextNodeHandling,
                                    nullValueHandling: nullValueHandling,
                                    textTrimHandling: textTrimHandling,
                                    classAttributeHandling: classAttributeHandling,
                                    renderJavascript: renderJavascript
                                    ));

                responseCode = 200;
            }
            catch(Exception e)
            {
                body = e.ToString();
                responseCode = 500;
            }

            Response response = new Response(responseCode, format, body);
            return response;
        }

        public async Task<dynamic> Convert(string url, string format,
                                            string emptyTextNodeHandling,
                                            string nullValueHandling,
                                            string textTrimHandling,
                                            string classAttributeHandling,
                                            string renderJavascript)
        {

            try
            {
                Jsonize jsonize;
                if (renderJavascript.ToLower().Equals("true"))
                {
                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Add("url", url);
                        var response = await client.GetAsync((Environment.GetEnvironmentVariable("DepthchargeRenderUrl") ?? Environment.GetEnvironmentVariable("APPSETTING_DepthchargeRenderUrl")) + "/api/render");
                        var html = await response.Content.ReadAsStringAsync();
                        jsonize = Jsonize.FromHtmlString(html);
                    }

                }
                else
                {
                    jsonize = await Jsonize.FromHttpUrl(url);
                }
                JsonizeConfiguration jsonizeConfiguration = new JsonizeConfiguration()
                {
                    EmptyTextNodeHandling = emptyTextNodeHandling.ToLower().Equals("ignore") ? EmptyTextNodeHandling.Ignore : EmptyTextNodeHandling.Include,
                    NullValueHandling = nullValueHandling.ToLower().Equals("ignore") ? JackWFinlay.Jsonize.NullValueHandling.Ignore : JackWFinlay.Jsonize.NullValueHandling.Include,
                    TextTrimHandling = textTrimHandling.ToLower().Equals("trim") ? TextTrimHandling.Trim : TextTrimHandling.Include,
                    ClassAttributeHandling = classAttributeHandling.ToLower().Equals("array") ? ClassAttributeHandling.Array : ClassAttributeHandling.String
                };

                if (format.ToLower().Equals("json"))
                {
                    JObject json = jsonize.ParseHtmlAsJson(jsonizeConfiguration);
                    return json;
                }
                else
                {
                    string jsonString = jsonize.ParseHtmlAsJsonString(jsonizeConfiguration);
                    return jsonString;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
