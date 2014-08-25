using System;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.IO;
using Cake.GitHub.Builders;

namespace Cake.GitHub
{
    /// <summary>
    /// Contains functionality related to running AliaSql.
    /// </summary>
    [CakeAliasCategory("GitHub")]
    public static class GitHubExtensions
    {
        [CakeMethodAlias]
        public static void GitHub(this ICakeContext context,
            string headerValue,
            Action<GitHubClientBuilder> gitHubConfigurationTask)
        {
            var builder = new GitHubClientBuilder(context.FileSystem, headerValue);
            gitHubConfigurationTask(builder);
        }
    }
}
