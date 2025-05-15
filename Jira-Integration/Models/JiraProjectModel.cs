using System.Text.Json;

namespace GcpDeployment.Models
{
    public class JiraProjectModel
    {
        public string Id { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        public string Self { get; set; }
        public string ProjectTypeKey { get; set; }
        public bool Simplified { get; set; }
        public string Style { get; set; }
        public bool IsPrivate { get; set; }
        public string Uuid { get; set; }
        public string EntityId { get; set; }
        public AvatarUrls AvatarUrls { get; set; }
    }

    public class AvatarUrls
    {
        public string Size48x48 { get; set; }
        public string Size24x24 { get; set; }
        public string Size16x16 { get; set; }
        public string Size32x32 { get; set; }

        [Newtonsoft.Json.JsonProperty("48x48")]
        public string _48x48 { set { Size48x48 = value; } }

        [Newtonsoft.Json.JsonProperty("24x24")]
        public string _24x24 { set { Size24x24 = value; } }

        [Newtonsoft.Json.JsonProperty("16x16")]
        public string _16x16 { set { Size16x16 = value; } }

        [Newtonsoft.Json.JsonProperty("32x32")]
        public string _32x32 { set { Size32x32 = value; } }
    }

    public class JiraIssueRequest
    {
        public string ProjectKey { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public string IssueType { get; set; } = "Task";
    }

    public class JiraSearchResponse
    {
        public List<JiraIssue> Issues { get; set; }
    }

    public class JiraIssue
    {
        public string Id { get; set; }
        public string Key { get; set; }
        public JiraIssueFields Fields { get; set; }
    }
    public class JiraIssueFields
    {
        public string Summary { get; set; }
        public object Description { get; set; }
        public JiraIssueType Issuetype { get; set; }
    }

    public class JiraIssueType
    {
        public string Name { get; set; }
    }
}