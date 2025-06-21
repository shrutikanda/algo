using Microsoft.SemanticKernel;
using System;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;



namespace LD.MultiAgents.AgentPlugins
{


    public class SalesPlugin : BasePlugin
    {

        private readonly HttpClient client = new HttpClient();

     
        public SalesPlugin(ILogger<BasePlugin> logger, string userId)
     : base(logger, userId)
        {
        }

        [KernelFunction("GetLoanInformation")]
        [Description("Get Loan Information by LoanId")]
        public async Task<LoanInformation?> GetLoanInformationByLoanIdAsync(string loanId)
        {
            string baseUrl = "https://ldapi-qa1.loandepotdev.works";
            Console.WriteLine($"get_loan_information_by_empowerloanId: {baseUrl}");

            var request = new HttpRequestMessage(HttpMethod.Get,
                $"{baseUrl}/EPS-Lending/api/v1/lending/mortgage/loans/{loanId}?lPart=All&bPart=All");

            request.Headers.Add("Ocp-Apim-Subscription-Key", "7f704ffd1bd64c289425e4c26cc39744");
            request.Headers.Add("Authorization", "ApiKey Key=AWB56kDtSGmP3_i-xdvxtpBOaQAy5zQrOUh4wYQWTDpC2SrJzRof-vqt_k-Fu7Lizo7t6M1iPuGVSAJeX5-LAY76QBZTTQ9QV58FCHWlUJjc1FVPsB5QnZ-f-Qx0Q_tsoIgA5UZh14GXzNxOM5MQGk6qSS-bYzQqm53uPBWW5auHR7LyXvGUh42EJGl-5bN4RBlEzINKBGUXSBCtlZNjBJSBXIESgohFwKHN5eK7UQhaZa5JPp-23QVWNrHZv6IC1g2");
            request.Headers.Add("LendingUserName", "epsimrserviceuser");
            request.Headers.Add("LendingSessionId", Guid.NewGuid().ToString());

            try
            {
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<LoanInformation>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return null;
        }
    }


    public class LoanInformation
    {
        public bool IsLead { get; set; }
        public string? LeadNumber { get; set; }
        public string? GenesysLeadId { get; set; }
        public string? AppLeadSource { get; set; }
        public string? OriginationChannel { get; set; }
        public bool IsEmployeeLoan { get; set; }
        public bool IsTestLoan { get; set; }
        public DateTime LastModified { get; set; }
        public string? TrackingId { get; set; }
        public bool IsGovernmentLoan { get; set; }
        public bool IsUsingFeeEngine { get; set; }
        public bool HypotheticalFeeEngineFlag { get; set; }
        public bool IsLDEConsentTrackingEligible { get; set; }
        public bool IsRedesignedURLA { get; set; }
        public int RedesignedURLAVersionOverride { get; set; }
        public double TotalLiabilities { get; set; }
        public double OtherCreditsFeesLenderCredits { get; set; }
    }

}