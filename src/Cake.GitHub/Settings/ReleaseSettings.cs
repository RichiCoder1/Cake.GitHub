using Cake.Core.IO;

namespace Cake.GitHub.Settings
{
    public class ReleaseSettings
    {
        public string RepositoryName { get; set; }
        public string ReleaseName { get; set; }
        public string TagName { get; set; }
        public string Owner { get; set; }
        public string Body { get; set; }
        public string TargetCommitish { get; set; }
        public bool Prerealease { get; set; }
        public bool Draft { get; set; }
    }

    public class ReleaseFileSettings
    {
        public string FileName { get; set; }
        public FilePath FilePath { get; set; }
        public string ContentType { get; set; }
    }
}
