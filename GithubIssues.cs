using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Octokit;

namespace Library
{

    public class GithubIssues
    {
        GitHubClient github;
        public GithubIssues()
        {
            github = new GitHubClient(new ProductHeaderValue("GithubPlanningPoker"));

        }

        public void Login(string user, string password)
        {
            github.Credentials = new Credentials(user, password);
        }

        public void Login(string token)
        {
            github.Credentials = new Credentials(token);
        }

        public async Task<Issue> PostIssue(string title, string content, string username, string repo)
        {
            if (github.Credentials.AuthenticationType == AuthenticationType.Anonymous)
                throw new Octokit.AuthorizationException();

            var issuesclient = github.Issue;
            var repository = await github.Repository.Get(username, repo);
            NewIssue n = new NewIssue(title) { Body = content };
            return await issuesclient.Create(repository.Owner.Login, repository.Name, n);
        }
    }
}
