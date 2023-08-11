using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ReservationSyste.Models;
using ReservationSyste.RemtaPayloadModes;
using ReservationSyste.RemtaPayloadModes.CheckStatusResponse;
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
                var customList = CustomBankList.GetCustomBankList();
                var accessToken = await GetAccessToken();
                var bankList = await GetActiveBanks(accessToken);
                
                var serializedObj = JsonConvert.SerializeObject(bankList.banks.data.banks);
              var result =  CustomBankList.CompareBankList(customList, bankList.banks.data.banks.ToList());
                restSharpResponse = await RemitaRestSharp(accessToken);
                var checkStatus = await GetCheckRemitalStatus(restSharpResponse.data.batchRef, accessToken);
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

        public async Task<(BankList banks, string token)> GetActiveBanks(string accessToken)
        {
            var url2 = _configuration["RemitaDemoSettings:bankListUrl"];
            var authenticator = new JwtAuthenticator(accessToken);
            var options = new RestClientOptions(url2)
            {
                Authenticator = authenticator
            };
            var client2 = new RestClient(options);
            var request2 = new RestRequest(url2, Method.Get);
            var response2 = await client2.ExecuteAsync<BankList>(request2);
            var output2 = response2.Content;
            var resp2 = JsonConvert.DeserializeObject<BankList>(output2);
            var bankList = resp2.data.banks.ToList();
            return (resp2, accessToken) ;

        }

       // [HttpGet("GetBulkRemitaStatus")]
        public async Task<StatusResponse> GetCheckRemitalStatus(string bachRef, string accessToken)
        {
            var url = "https://remitademo.net/remita/exapp/api/v1/send/api/rpgsvc/v3/rpg/bulk/payment/status";
            var options = new RestClientOptions("https://remitademo.net")
            {
                MaxTimeout = -1,
            };
            var client1 = new RestClient(url);
            var request1 = new RestRequest($"{url}/{bachRef}", Method.Get);
            //request1.AddHeader("Cache-Control", "no-cache");
            request1.AddHeader("Authorization", "Bearer " + accessToken);
            var response = await client1.ExecuteAsync(request1);

            var jSonResponse = JsonConvert.DeserializeObject<StatusResponse>(response.Content);

            return jSonResponse;
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

//context.Database.ExecuteSqlRaw("UPDATE [Employees] SET [Salary] = [Salary] + 1000");
//"102643195"  "335322213"
//"8909090989222"
//"8909090989333"
//"8909090989444"

//eyJhbGciOiJSUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICJnVU1WRmp5eWpZVUx1OTA1cTFSSFc3YTcwOUc3ZGI3LTJNWEdreWxreFdzIn0.eyJleHAiOjE2ODQ0MDUyMDksImlhdCI6MTY4NDQwMTYwOSwianRpIjoiMWZhNTgyZGItMTA2ZS00MDBkLTgxMGQtZDM1NDBkN2FkZjBiIiwiaXNzIjoiaHR0cDovLzEwLjEuMS44Mjo5MTgwL2tleWNsb2FrL3JlbWl0YS9leGFwcC9hcGkvdjEvcmVkZ2F0ZS9hdXRoL3JlYWxtcy9yZW1pdGEiLCJhdWQiOlsiZGlzY292ZXJ5LXNlcnZlciIsImFjY291bnQiXSwic3ViIjoiMGUwM2UwZDYtY2IzMC00YjIxLWJhNTYtNGI1MmM5ZmQ3ZjkzIiwidHlwIjoiQmVhcmVyIiwiYXpwIjoicmVtaXRhdWFhLXNlcnZpY2UiLCJzZXNzaW9uX3N0YXRlIjoiNjk3MzhiNTUtMmQ5Yy00YmIzLWI0MmEtMjUxN2FjNGVmZDc2IiwiYWNyIjoiMSIsImFsbG93ZWQtb3JpZ2lucyI6WyJodHRwczovL2xvZ2luLnJlbWl0YS5uZXQiXSwicmVhbG1fYWNjZXNzIjp7InJvbGVzIjpbIm9mZmxpbmVfYWNjZXNzIiwidW1hX2F1dGhvcml6YXRpb24iXX0sInJlc291cmNlX2FjY2VzcyI6eyJkaXNjb3Zlcnktc2VydmVyIjp7InJvbGVzIjpbIm1hbmFnZS1hY2NvdW50Iiwidmlldy1wcm9maWxlIl19LCJhY2NvdW50Ijp7InJvbGVzIjpbIm1hbmFnZS1hY2NvdW50IiwibWFuYWdlLWFjY291bnQtbGlua3MiLCJ2aWV3LXByb2ZpbGUiXX19LCJzY29wZSI6ImVtYWlsIHByb2ZpbGUiLCJlbWFpbF92ZXJpZmllZCI6dHJ1ZSwibmFtZSI6IjAxMSAwMTEiLCJwcmVmZXJyZWRfdXNlcm5hbWUiOiJ4cjNxbDZ5cmpvdXNoNTdjIiwiZ2l2ZW5fbmFtZSI6IjAxMSIsImZhbWlseV9uYW1lIjoiMDExIiwib3JnYW5pc2F0aW9uLWlkIjoiQ1dHREVNTyIsImVtYWlsIjoiMDExIn0.N5Fi8YP2T5TCKuDGPRkEL53JnKiPWZi27cY6daimYmVLXQz_wzCvKiHDwch3_tBnRBKYO60snBAz7C0VmDtJi58byoDcPQQXyg3utQ-1mGC07yxRY1xVKm_26M2adJQKV5eYvTWqxQOYpIJBKiFeadAmtaXll9y1LlTQQxfAzIrDR3-oU8Y_mpWxZrrhoQYBx_t3Cleizzw0txVBYGUKjg8MbqGrfRRQsEN0Kj8MpJATiV2hHZG_gW3Fop9OhsYJvTWuz_krWzvSGYQpzADmb5C5xYQkfS8MLYdhiizlJL3Cr-awoGldYc9rLN6CZWzVnss6ruEgVsTAJf39jRlboQ
