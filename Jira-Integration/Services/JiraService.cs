using Newtonsoft.Json;
using GcpDeployment.Models;
using System.Text;
using System.Text.Json;
using System.Net.Http.Json;
using System.Net.Http.Headers;


namespace GcpDeployment.Services
{
    public class JiraService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public JiraService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<List<JiraProjectModel>> GetProjectsAsync()
        {
            var email = _configuration["Jira:Email"];
            var apiToken = _configuration["Jira:ApiToken"];
            var jiraUrl = _configuration["Jira:Url"];

            var authToken = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{email}:{apiToken}"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);

            var response = await _httpClient.GetAsync($"{jiraUrl}/rest/api/3/project");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<JiraProjectModel>>(json);
            }

            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Error: {response.StatusCode} - {error}");
        }
        public async Task<string> CreateIssueAsync(string projectKey, string summary, string description, string issueType = "Task")
        {
            var email = _configuration["Jira:Email"];
            var apiToken = _configuration["Jira:ApiToken"];
            var jiraUrl = _configuration["Jira:Url"];

            var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{email}:{apiToken}"));
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authToken);

            var payload = new
            {
                fields = new
                {
                    project = new { key = projectKey },
                    summary = summary,
                    description = new
                    {
                        type = "doc",
                        version = 1,
                        content = new[]
                        {
                            new
                            {
                                type = "paragraph",
                                content = new[]
                                {
                                    new
                                    {
                                        type = "text",
                                        text = description
                                    }
                                }
                            }
                        }
                    },
                    issuetype = new { name = issueType }
                }
            };

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{jiraUrl}/rest/api/3/issue", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to create issue: {response.StatusCode} - {error}");
            }

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<List<JiraIssue>> GetIssuesCreatedByUserAsync()
        {
            var email = _configuration["Jira:Email"];
            var apiToken = _configuration["Jira:ApiToken"];
            var jiraUrl = _configuration["Jira:Url"];

            var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{email}:{apiToken}"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);

            var jql = "project = \"Gcp-Deployment\" AND resolution = Unresolved AND reporter = currentUser()";
            var url = $"{jiraUrl}/rest/api/3/search?jql={Uri.EscapeDataString(jql)}";

            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var issueResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<JiraSearchResponse>(json);
                return issueResponse.Issues;
            }

            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Error fetching issues: {response.StatusCode} - {error}");
        }

        public async Task<List<JiraIssue>> GetAllIssuesAsync()
        {
            var email = _configuration["Jira:Email"];
            var apiToken = _configuration["Jira:ApiToken"];
            var jiraUrl = _configuration["Jira:Url"];

            var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{email}:{apiToken}"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);

            var jql = "project = GCPD ORDER BY created DESC";
            var url = $"{jiraUrl}/rest/api/3/search?jql={Uri.EscapeDataString(jql)}";

            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var issueResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<JiraSearchResponse>(json);
                return issueResponse.Issues;
            }

            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Error fetching issues: {response.StatusCode} - {error}");
        }


    }
}