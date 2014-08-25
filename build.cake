Func<string, bool> hasEnvVar = varName => !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(varName));
Func<string, string> getEnvVar = varName => Environment.GetEnvironmentVariable(varName); // Get arguments passed to the script.

var target = Argument("target", "All");
var configuration = Argument("configuration", getEnvVar("Configuration") ?? "Debug");

// Define directories.
var projectName = "Cake.GitHub";
var sourceDir = "./src";
var buildDir = sourceDir + "/" + projectName + "/bin/" + configuration;
var buildResultDir = "./build/";
var testResultsDir = buildResultDir + "/tests";
var nugetDir = buildResultDir + "/nuget";
var binDir = buildResultDir + "/bin";

var solutionFile = sourceDir + "/" + projectName + ".sln";

//////////////////////////////////////////////////////////////////////////

Task("Clean")
	.Description("Cleans the build and output directories.")
	.Does(() =>
{
	CleanDirectories(new DirectoryPath[] {
		buildResultDir, binDir, testResultsDir, nugetDir});
});

Task("Restore-NuGet-Packages")
	.Description("Restores all NuGet packages in solution.")
	.IsDependentOn("Clean")
	.Does(() =>
{
	NuGetRestore(solutionFile);
});

Task("Build")
	.Description("Builds the Cake source code.")
	.IsDependentOn("Restore-NuGet-Packages")
	.Does(() =>
{
	MSBuild(solutionFile, settings =>
		settings.SetConfiguration(configuration)
			.UseToolVersion(MSBuildToolVersion.NET45));
});

Task("Run-Unit-Tests")
	.Description("Runs unit tests.")
	.IsDependentOn("Build")
	.Does(() =>
{
	XUnit("./src/**/bin/" + configuration + "/*.Tests.dll", new XUnitSettings {
		OutputDirectory = testResultsDir,
		XmlReport = true,
		HtmlReport = true
	});
});


Task("Copy-Files")
	.Description("Copy files to the output directory.")
	.IsDependentOn("Run-Unit-Tests")
	.Does(() =>
{
	CopyFileToDirectory(buildDir + "/Cake.GitHub.dll", binDir);
	CopyFileToDirectory(buildDir + "/Cake.GitHub.pdb", binDir);
    CopyFileToDirectory(buildDir + "/Cake.Core.dll", binDir);
    CopyFileToDirectory(buildDir + "/Cake.Core.xml", binDir);
    CopyFiles(new FilePath[] { "LICENSE", "README.md" }, binDir);
});

Task("All")
	.Description("Default Target")
	.IsDependentOn("Copy-Files");

//////////////////////////////////////////////////////////////////////////

RunTarget(target);