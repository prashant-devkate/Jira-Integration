using GcpDeployment.Models;
using GcpDeployment.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GcpDeployment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JiraController : ControllerBase
    {
        private readonly JiraService _jiraService;

        public JiraController(JiraService jiraService)
        {
            _jiraService = jiraService;
        }

        [HttpGet("projects")]
        public async Task<IActionResult> GetProjects()
        {
            try
            {
                var projects = await _jiraService.GetProjectsAsync();
                return Ok(projects);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("issues")]
        public async Task<IActionResult> CreateIssue([FromBody] JiraIssueRequest request)
        {
            try
            {
                var result = await _jiraService.CreateIssueAsync(
                    request.ProjectKey,
                    request.Summary,
                    request.Description,
                    request.IssueType
                );

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("issues/createdByMe")]
        public async Task<IActionResult> GetIssuesCreatedByUser()
        {
            try
            {
                var issues = await _jiraService.GetIssuesCreatedByUserAsync();

                var result = issues.Select(i => new
                {
                    i.Id,
                    i.Key,
                    Summary = i.Fields?.Summary,
                    IssueType = i.Fields?.Issuetype?.Name
                });

                return Ok(issues);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("issues/all")]
        public async Task<IActionResult> GetAllIssues()
        {
            try
            {
                var issues = await _jiraService.GetAllIssuesAsync();
                return Ok(issues);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}