using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using services.varian.com.Patient.Documents;
using VMS.OIS.ARIAExternal.WebServices.Documents.Contracts;

namespace AccessDummy
{
    class Program
    {
        static void Main(string[] args)
        {
            var hostName = "tbox";
            var port = "55051";
            var gatewayUrl = $"https://{hostName}:{port}/Gateway/service.svc/interop/rest/Process";
            var dockey = "112dfc6d-5712-4654-92ab-c03c0d8546f6";//documents
            var aakey = "c5ffc916-b984-4c12-9d0d-5fc8344ba5fb";//AA
            var authenticationHttpHandler = new HttpClientHandler { UseDefaultCredentials = true, PreAuthenticate = true };
            var httpClient = HttpClientFactory.Create(authenticationHttpHandler);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.Timeout = TimeSpan.FromSeconds(120);

            var requestJson = "{\"__type\":\"GetDocumentsRequest:http://services.varian.com/Patient/Documents\",\"PatientId\":{\"ID1\":\"007\",\"PtId\":null}}";
            //var requestJson = "{\"__type:\":\"DocumentsRequest:http://services.varian.com/Patient.Documents\",
            //var requestJson = "{\"__type\":\"DocumentsRequest:http://services.varian.com/Patient.Documents\",\"PatientId\":{\"ID1\":\"007\",\"PtId\":null}}";
            //var requestJson = "{\"__type\":\"GetPatientsRequest:http://services.varian.com/AriaWebConnect/Link\",\"Attributes\":null,\"PatientId1\":\"007\"}";
            //var requestJson = "{\"__type\":\"GetPatientsRequest:http://services.varian.com/AriaWebConnect/Link\",\"PatientId1\":{\"Value\":\"007\"}}";
           // var requestJson = "{\"__type\":\"GetLicenseFeaturesRequest:http://services.varian.com/Foundation/SF/Infrastructure\",\"IncludeExpired\":true }";
            //var requestJson = "{\"__type\":\"GetLicenseFeaturesRequest:http://services.varian.com/Foundation/SF/Infrastructure\",\"IncludeExpired\":{\"Value\":true}}";
            //var requestJson = "{\"__type\":\"ServiceMetadataRequest:http://services.varian.com/Foundation/SF/Infrastructure\",\"IncludeExpired\":{\"Value\":\"true\"}";
            string responseJson = GetJsonResponse(gatewayUrl, requestJson, dockey, httpClient);
            DocumentsResponse dr = JsonConvert.DeserializeObject<DocumentsResponse>(responseJson);
            DocumentResponse dr1 = dr.Documents[0];
            GetDocumentRequest drequest = new GetDocumentRequest()
            {
                Attributes = null,
                PatientId = new VMS.OIS.ARIALocal.WebServices.Document.Contracts.PatientIdentifier
                {
                    PtId = dr1.PtId
                },
                PatientVisitId = dr1.PtVisitId,
                VisitNoteId =dr1.PtVisitNoteId// dr.Documents[0].VisitNoteId
            };
            var requestDoc = "{\"__type\":\"GetDocumentRequest:http://services.varian.com/Patient/Documents\"," + JsonConvert.SerializeObject(drequest).TrimStart('{');
            GetJsonResponse(gatewayUrl, requestDoc, dockey, httpClient);

            Console.ReadLine();
        }

        public static string GetJsonResponse(string gatewayUrl, string requestJson, string akey, HttpClient httpClient)
        {
            using (var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri(gatewayUrl),
                Method = HttpMethod.Post,
                Content = new StringContent(requestJson, Encoding.UTF8, "application/json")
            })
            {
                requestMessage.Headers.Add("ApiKey", akey);
                var responseMessage = httpClient.SendAsync(requestMessage).Result;
                // TODO: Handle error cases.
                if (!responseMessage.IsSuccessStatusCode)
                {
                    var message = responseMessage.Content.ReadAsStringAsync().Result;

                    switch (responseMessage.StatusCode)
                    {
                        case HttpStatusCode.Unauthorized:
                            // ErrorCode + ErrorMessage
                            break;
                        case HttpStatusCode.BadRequest:
                            // ErrorCode + ErrorMessage + ParameterName
                            break;
                    }


                    throw new InvalidOperationException(message);
                }

                var responseJson = responseMessage.Content.ReadAsStringAsync().Result;

                Console.WriteLine(responseJson);
                return responseJson;

            }
        }
    }
}
