using System;
using System.IO;
using Cake.Core.IO;
using Cake.GitHub.Settings;
using Octokit;
using FileMode = System.IO.FileMode;

namespace Cake.GitHub.Builders
{
    public sealed class ReleaseBuilder
    {
        private readonly IFileSystem _fileSystem;

        private readonly IReleasesClient _releasesClient;

        public readonly ReleaseSettings Settings;

        public readonly ReleaseFileSettings FileSettings;

        internal ReleaseBuilder(IFileSystem fileSystem, IReleasesClient client)
            : this(fileSystem, client, new ReleaseSettings())
        {
        }

        internal ReleaseBuilder(IFileSystem fileSystem, IReleasesClient client,
            ReleaseSettings settings)
            : this(fileSystem, client, settings, new ReleaseFileSettings())
        {
        }

        internal ReleaseBuilder(IFileSystem fileSystem, IReleasesClient client, 
            ReleaseSettings settings, ReleaseFileSettings fileSettings)
        {
            _fileSystem = fileSystem;
            _releasesClient = client;
            Settings = settings;
            FileSettings = fileSettings;
        }

        public ReleaseBuilder Owner(string owner)
        {
            Settings.Owner = owner;
            return this;
        }

        public ReleaseBuilder Name(string name)
        {
            Settings.ReleaseName = name;
            return this;
        }

        public ReleaseBuilder TagName(string tagName)
        {
            Settings.TagName = tagName;
            return this;
        }

        public ReleaseBuilder Prerealease(bool prerelease)
        {
            Settings.Prerealease = prerelease;
            return this;
        }

        public ReleaseBuilder Body(string body)
        {
            Settings.Body = body;
            return this;
        }

        public ReleaseBuilder Draft(bool draft)
        {
            Settings.Draft = draft;
            return this;
        }

        public ReleaseBuilder TargetCommitish(string targetCommitish)
        {
            Settings.TargetCommitish = targetCommitish;
            return this;
        }

        public ReleaseBuilder FileName(string fileName)
        {
            FileSettings.FileName = fileName;
			return this;
        }

        public ReleaseBuilder File(FilePath file)
        {
            FileSettings.FilePath = file;
            if (string.IsNullOrWhiteSpace(FileSettings.FileName))
                FileSettings.FileName = file.GetFilename().FullPath;
            return this;
        }

        public ReleaseBuilder ContentType(string contentType)
        {
            FileSettings.ContentType = contentType;
            return this;
        }

        public void CreateRelease()
        {
            if(string.IsNullOrWhiteSpace(Settings.TagName))
                throw new ArgumentException("Must provide a tagname!");

            if (string.IsNullOrWhiteSpace(Settings.RepositoryName))
                throw new ArgumentException("Must provide a repository name!");

            if (string.IsNullOrWhiteSpace(Settings.ReleaseName))
                throw new ArgumentException("Must provide a release name!");

            if (string.IsNullOrWhiteSpace(Settings.Owner))
                throw new ArgumentException("Must provide an owner!");

            var releaseData = new ReleaseUpdate(Settings.TagName)
            {
                Body = Settings.Body,
                Prerelease = Settings.Prerealease,
                Draft = Settings.Draft,
                Name = Settings.ReleaseName,
                TargetCommitish = Settings.TargetCommitish
            };

            var release = 
                _releasesClient.Create(Settings.Owner, Settings.RepositoryName, releaseData).Result;


            if (FileSettings.FilePath != null)
            {
                _releasesClient.UploadAsset(release, new ReleaseAssetUpload
                {
                    FileName = FileSettings.FileName,
                    ContentType = FileSettings.ContentType,
                    RawData = _fileSystem
                                .GetFile(FileSettings.FilePath)
                                .Open(FileMode.Open, FileAccess.Read, FileShare.None)
                }).Wait();
            }
        }
    }
}
