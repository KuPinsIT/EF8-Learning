using System.Net.Http.Headers;
using Azure;
using EF8Example.Models;
using EF8Example.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace EF8Example.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExternalController: ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly GitHubService _gitHubService;
        private readonly IGitHubClient _gitHubClient;
        public ExternalController(IHttpClientFactory httpClientFactory, GitHubService gitHubService, IGitHubClient gitHubClient)
        {
            _httpClientFactory = httpClientFactory;
            _gitHubService = gitHubService;
            _gitHubClient = gitHubClient;
        }

        [HttpGet("basic")]
        public async Task<ActionResult> GetBasic()
        {
            var client = _httpClientFactory.CreateClient();

            client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/vnd.github.v3+json");
            client.DefaultRequestHeaders.Add(HeaderNames.UserAgent, "HttpRequestsSample");

            var response = await client.GetFromJsonAsync<IReadOnlyList<GitHubBranch>>("https://api.github.com/repos/dotnet/AspNetCore.Docs/branches");
            return Ok(response);
        }

        [HttpGet("name-clients")]
        public async Task<ActionResult> GetNameClients()
        {
            var client = _httpClientFactory.CreateClient("GitHub");
            var response = await client.GetFromJsonAsync<IReadOnlyList<GitHubBranch>>("repos/dotnet/AspNetCore.Docs/branches");
            return Ok(response);
        }

        [HttpGet("type-clients")]
        public async Task<ActionResult> GetTypeClients()
        {
            var result = await _gitHubService.GetAspNetCoreDocsBranchesAsync();
            return Ok(result);
        }

        [HttpGet("general-clients")]
        public async Task<ActionResult> GetGeneralClients()
        {
            var result = await _gitHubClient.GetAspNetCoreDocsBranchesAsync();
            return Ok(result);
        }
    }
}
