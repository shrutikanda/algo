using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;



namespace AzureOpenAI.Chat.Services
{
    public static class DCRM
    {       

        private static readonly HttpClient client = new HttpClient();
        
        private static string ENTERPRISE_CONVERSATIONSERVICE_BASEURL = "https://enterprise-conversationservice.dv1.loandepotdev.io/api/v1";

      
        public static async Task<string> GetCustomerServiceInformationByEmailAsync(string email)
        {
          
            string clientId = "77d2e34e-9638-4fa5-97d5-4007736b127e";
            string clientSecret = "dgi8Q~uXana7f.PK2.Y_14PTeWchez09EjYSccIR";
            string tenantId = "19479f88-8eac-45d2-a1bf-69d33854a3fa";
            string scope = "https://d365-customerservice-qa1.crm.dynamics.com/.default";
            string baseUrl = "https://d365-customerservice-qa1.crm.dynamics.com/api/data/v9.2/";

            string token = await GetTokenAsync(clientId, clientSecret, tenantId, scope);

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            string query = $"contacts?$select=fullname&$filter=emailaddress1 eq '{email}'";
            Console.WriteLine($"get_customer_service_information_by_email: {baseUrl}");

            var response = await client.GetAsync(baseUrl + query);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                // Replace with actual logic to extract OriginationCase if known
                if (root.TryGetProperty("OriginationCase", out var originationCase))
                    return originationCase.ToString();

                return json;
            }
            else
            {
                throw new Exception($"API request failed: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
            }
        }

        public static async Task<string?> GetLoanInformationByEmpowerLoanIdAsync(string empowerLoanId)
        {
            string baseUrl = "https://ldapi-qa1.loandepotdev.works";
            Console.WriteLine($"get_loan_information_by_empowerloanId: {baseUrl}");

            var request = new HttpRequestMessage(HttpMethod.Get,
                $"{baseUrl}/EPS-Lending/api/v1/lending/mortgage/loans/{empowerLoanId}?lPart=All&bPart=All");

            // ✅ These are fine:
            request.Headers.Add("Ocp-Apim-Subscription-Key", "7f704ffd1bd64c289425e4c26cc39744");
            request.Headers.Add("Authorization", "ApiKey Key=AWB56kDtSGmP3_i-xdvxtpBOaQAy5zQrOUh4wYQWTDpC2SrJzRof-vqt_k-Fu7Lizo7t6M1iPuGVSAJeX5-LAY76QBZTTQ9QV58FCHWlUJjc1FVPsB5QnZ-f-Qx0Q_tsoIgA5UZh14GXzNxOM5MQGk6qSS-bYzQqm53uPBWW5auHR7LyXvGUh42EJGl-5bN4RBlEzINKBGUXSBCtlZNjBJSBXIESgohFwKHN5eK7UQhaZa5JPp-23QVWNrHZv6IC1g2");            
            request.Headers.Add("LendingUserName", "epsimrserviceuser");
            request.Headers.Add("LendingSessionId", Guid.NewGuid().ToString());

            try
            {
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                   var results =  await response.Content.ReadAsStringAsync();
                    return results;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return null;
        }


        public static async Task<string> GetConversationSentimentAsync(string conversationId)
        {
            string conversationApiEndpoint = ENTERPRISE_CONVERSATIONSERVICE_BASEURL;
            Console.WriteLine($"conversationapi: {conversationApiEndpoint}");

            string url = $"{conversationApiEndpoint}/analysis/Conversation/{conversationId}";

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                string error = await response.Content.ReadAsStringAsync();
                return $"{{\"error\": \"{error}  {conversationApiEndpoint}\"}}";
            }
        }

        // Stubbed token acquisition. You would use MSAL or IdentityClient here.
        private static async Task<string> GetTokenAsync(string clientId, string clientSecret, string tenantId, string scope)
        {
            var tokenUrl = $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token";
            var content = new FormUrlEncodedContent(new[]
            {
            new KeyValuePair<string, string>("client_id", clientId),
            new KeyValuePair<string, string>("scope", scope),
            new KeyValuePair<string, string>("client_secret", clientSecret),
            new KeyValuePair<string, string>("grant_type", "client_credentials")
        });

            var response = await client.PostAsync(tokenUrl, content);
            var responseBody = await response.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(responseBody);
            return jsonDoc.RootElement.GetProperty("access_token").GetString();
        }
    }
}