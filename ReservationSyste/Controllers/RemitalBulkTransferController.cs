using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ReservationSyste.RemtaPayloadModes;
using ReservationSyste.RemtaPayloadModes.Responses;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Text.Json.Nodes;

namespace ReservationSyste.Controllers
{
    //[ApiController]
    //[Route("[api/controller]")]
    public class RemitalBulkTransferController : Controller
    {
        private readonly IConfiguration _configuration;

        public RemitalBulkTransferController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<ActionResult<Response>> Remita()
        {
            Response restSharpResponse = new Response();
            try
            {
                var accessToken = await GetAccessToken();
                var bankList = await GetActiveBanks(accessToken);
                restSharpResponse = await RemitaRestSharp(accessToken);
                //var httpRemita = await RemitaHttp(accessToken);
            }
            catch (Exception)
            {
                throw;
            }
            return View(restSharpResponse);
        }

        public async Task<string> GetAccessToken()
        {
            var url = _configuration["RemitaDemoSettings:accessTokenUrl"];
            var client = new RestClient(url);
            var request = new RestRequest(url, Method.Post);
            request.AddHeader("Content-Type", "application/json");
            var body = new
            {
                username = _configuration["RemitaDemoSettings:username"],
                password = _configuration["RemitaDemoSettings:password"],
            };
            var bodyy = JsonConvert.SerializeObject(body);
            request.AddBody(bodyy, "application/json");
            RestResponse response = await client.ExecuteAsync<ApiResponse>(request);
            var output = response.Content;
            var resp = JsonConvert.DeserializeObject<ApiResponse>(output);

            return resp.data[0].accessToken;
        }

        public async Task<List<Bank>> GetActiveBanks(string accessToken)
        {
            var url2 = _configuration["RemitaDemoSettings:bankListUrl"];
            var authenticator = new JwtAuthenticator(accessToken);
            var options = new RestClientOptions(url2)
            {
                Authenticator = authenticator
            };
            var client2 = new RestClient(options);
            var request2 = new RestRequest(url2, Method.Get);
            RestResponse response2 = await client2.ExecuteAsync<BankList>(request2);
            var output2 = response2.Content;
            var resp2 = JsonConvert.DeserializeObject<BankList>(output2);

            return resp2.data.banks.ToList(); ;

        }

        public async Task<Response> RemitaRestSharp(string accessToken)
        {
            var options = new RestClientOptions("https://remitademo.net")
            {
                MaxTimeout = -1,
            };
            var url1 = "https://remitademo.net/remita/exapp/api/v1/send/api/rpgsvc/v3/rpg/bulk/payment";
            var client1 = new RestClient(url1);
            var request1 = new RestRequest(url1, Method.Post);
            request1.AddHeader("Cache-Control", "no-cache");
            request1.AddHeader("Authorization", "Bearer " + accessToken);
            var body = Payloads.GetRemitalBulkPayloads();
            request1.AddJsonBody(Payloads.GetRemitalBulkPayloads());
           // var response1 = client1.Execute(request1);
            var response = await client1.ExecuteAsync(request1);
            var jSonResponse = JsonConvert.DeserializeObject<Response>(response.Content);

            return jSonResponse;
        }

        public async Task<HttpResponseMessage> RemitaHttp(string accessToken)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://remitademo.net/remita/exapp/api/v1/send/api/rpgsvc/v3/rpg/bulk/payment");
            request.Headers.Add("Authorization", $"Bearer {accessToken}");
            //request.Headers.Add("ContentType", "application/json");
            var content = new StringContent("{\r\n    \"batchRef\": \""+ Payloads.Generate().ToString()  +"\",\r\n    \"totalAmount\": 4500,\r\n    \"sourceAccount\": \"8909090989\",\r\n    \"sourceAccountName\": \"ABC\",\r\n    \"sourceBankCode\": \"058\",\r\n    \"currency\": \"NGN\",\r\n    \"sourceNarration\": \"Bulk Transfer\",\r\n    \"originalAccountNumber\": \"8909090989\", \r\n    \"originalBankCode\": \"058\", \r\n    \"customReference\": \"{{customerReference}}\",\r\n    \"transactions\": [\r\n        {\r\n            \"amount\": 2500,\r\n            \"transactionRef\":\"{{transRef1}}\",\r\n            \"destinationAccount\": \"0037475942\",\r\n            \"destinationAccountName\": \"Kelvin John\",\r\n            \"destinationBankCode\": \"058\",\r\n            \"destinationNarration\": \"Bulk Transfer\"\r\n        },\r\n        {\r\n            \"amount\": 1500,\r\n            \"transactionRef\":\"{{transRef2}}\",\r\n            \"destinationAccount\": \"0037475942\",\r\n            \"destinationAccountName\": \"Martin John\",\r\n            \"destinationBankCode\": \"058\",\r\n            \"destinationNarration\": \"Bulk Transfer\"\r\n        },\r\n        {\r\n            \"amount\": 500,\r\n            \"transactionRef\":\"{{transRef3}}\",\r\n            \"destinationAccount\": \"0037475942\",\r\n            \"destinationAccountName\": \"Mike John\",\r\n            \"destinationBankCode\": \"058\",\r\n            \"destinationNarration\": \"Bulk Transfer\"\r\n        }\r\n    ]\r\n}", null, "text/plain");
            request.Content = content;
            var body = Payloads.GetRemitalBulkPayloads();
            var contentBody = JsonConvert.SerializeObject(body);
           //request.Content = new StringContent(content);

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            Console.WriteLine(await response.Content.ReadAsStringAsync());

            return response;
        }

    }

}
