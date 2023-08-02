using EF8Example.Models;
using Microsoft.Net.Http.Headers;

namespace EF8Example.Services
{
    public class GitHubService
    {
        private readonly HttpClient _httpClient;

        public GitHubService(HttpClient httpClient)
        {
            _httpClient = httpClient;

            //_httpClient.BaseAddress = new Uri("https://api.github.com/");

            // using Microsoft.Net.Http.Headers;
            // The GitHub API requires two headers.
            _httpClient.DefaultRequestHeaders.Add(
                HeaderNames.Accept, "application/vnd.github.v3+json");
            _httpClient.DefaultRequestHeaders.Add(
                HeaderNames.UserAgent, "HttpRequestsSample");
        }

        public async Task<IReadOnlyList<GitHubBranch>> GetAspNetCoreDocsBranchesAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<IReadOnlyList<GitHubBranch>>("repos/dotnet/AspNetCore.Docs/branches");
            return response;
        }
    }
}
