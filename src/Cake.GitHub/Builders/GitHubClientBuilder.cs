using System.Collections.Generic;
using Cake.Core.IO;
using Cake.GitHub.Settings;
using Octokit;

namespace Cake.GitHub.Builders
{
    public sealed class GitHubClientBuilder
    {
        internal readonly GitHubClient Client;

        private readonly IFileSystem _fileSystem;

        internal GitHubClientBuilder(IFileSystem fileSystem, string headerValue) 
        {
            Client = new GitHubClient(new ProductHeaderValue(headerValue));
            _fileSystem = fileSystem;
        }

        public GitHubClientBuilder WithOAuthKey(string token)
        {
            Client.Credentials = new Credentials(token);
            return this;
        }

        public GitHubClientBuilder WithBasic(string username, string password)
        {
            Client.Credentials = new Credentials(username, password);
            return this;
        }

        #region ReleaseClient
        public ReleaseBuilder StartRelease()
        {
            return new ReleaseBuilder(_fileSystem, Client.Release);
        }

        public ReleaseBuilder StartRelease(ReleaseSettings settings)
        {
            return new ReleaseBuilder(_fileSystem, Client.Release, settings);
        }

        public ReleaseBuilder StartRelease(ReleaseSettings settings, ReleaseFileSettings fileSettings)
        {
            return new ReleaseBuilder(_fileSystem, Client.Release, settings, fileSettings);
        }

        public IReadOnlyList<Release> GetReleases(string owner, string repositoryName)
        {
            return Client.Release.GetAll(owner, repositoryName).Result;
        }

        #endregion
    }
}
