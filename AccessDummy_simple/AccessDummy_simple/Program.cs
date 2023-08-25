using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AccessDummy_simple
{
    class Program
    {
        static void Main(string[] args)
        {
            string APIKey = "c5ffc916-b984-4c12-9d0d-5fc8344ba5fb";
            string host_name = "bjcdev";
            string gateway = $"https://{host_name}:55051/gateway/service.svc/interop/rest/process";
            string request = "{\"__type\":\"GetMachineListRequest:http://services.varian.com/AriaWebConnect/Link\"}";
            string sResponse = "";
            using (HttpClient httpClient = new HttpClient(new HttpClientHandler
            {
                UseDefaultCredentials = true,
                
            }))
            {
                httpClient.Timeout = TimeSpan.FromSeconds(120);
                httpClient.DefaultRequestHeaders.Add("ApiKey", APIKey);
                var task = httpClient.PostAsync(gateway,
                    new StringContent(request, Encoding.UTF8, "application/json"));
                Task.WaitAll(task);
                var responseTask = task.Result.Content.ReadAsStringAsync();
                Task.WaitAll(responseTask);
                sResponse = responseTask.Result;
                Console.WriteLine(sResponse);
            }
            Console.ReadLine();
    }
}
}
