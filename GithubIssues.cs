using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Octokit;

namespace Library
{

    public class Github
    {
        GitHubClient github;
        public Github()
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

        public async void PostIssue(string title, string content, string username, string repo)
        {
            if (github.Credentials.AuthenticationType == AuthenticationType.Anonymous)
                throw new Octokit.AuthorizationException();

            var issuesclient = github.Issue;
            var repository = await github.Repository.Get(username, repo);
            NewIssue n = new NewIssue(title) { Body = content };
            await issuesclient.Create(repository.Owner.Login, repository.Name, n);
        }

        public bool UserExists(string name)
        {
            try
            {
                var g = github.User.Get(name).Result;
            }
            catch (AggregateException e)
            {
                return false;
            }
            return true;
        }

        public bool RepoExists(string name, string repository)
        {
            try
            {
                var g = github.Repository.Get(name, repository).Result;
            }
            catch (AggregateException e)
            {
                return false;
            }
            return true;
        }

        public bool CanPublishToRepo(string name, string repository)
        {
            var g = github.Repository.Get(name, repository).Result;
            var h = g.Permissions;
            return h.Admin;
        }

    }
}
