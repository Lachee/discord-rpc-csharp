// Addins
#addin nuget:?package=Cake.FileHelpers
#addin nuget:?package=Cake.Git
#addin nuget:?package=Cake.VersionReader

// Adjustable Variables
var projectName = "DiscordRPC";

// Arguments
var target = Argument ("target", "Default");
var buildType = Argument<string>("buildType", "Release");
var buildCounter = Argument<int>("buildCounter", 0);
var buildTag = Argument<string>("buildTag", "v1.0");
var signCertificate = Argument<string>("signCertificate", "certificate.pfx");
var signPassword = Argument<string>("signPassword", "");

// Project Variables
var asm = string.Format("./{0}/Properties/AssemblyInfo.cs", projectName);
var sln = string.Format("./{0}/{0}.sln", projectName);
var releaseFolder = string.Format("./{0}/bin/{1}", projectName, buildType);
var releaseDll = "/DiscordRPC.dll";
var nuspecFile = string.Format("./{0}/{0}.nuspec", projectName);

// Execution Variables
var major_version = buildTag.Trim('v');
var version = major_version + "." + buildCounter.ToString() + ".0";
var ciVersion = major_version + ".0-CI00000";
var runningOnTeamCity = false;
var runningOnAppVeyor = false;


// Find out if we are running on a Build Server
Task ("DiscoverBuildDetails")
	.Does (() =>
	{
		runningOnTeamCity = TeamCity.IsRunningOnTeamCity;
		Information("Running on TeamCity: " + runningOnTeamCity);
		runningOnAppVeyor = AppVeyor.IsRunningOnAppVeyor;
		Information("Running on AppVeyor: " + runningOnAppVeyor);
	});

// Outputs Argument values so they are visible in the build log
Task ("OutputVariables")
	.Does (() =>
	{
		Information("BuildType: " + buildType);
		Information("BuildCounter: " + buildCounter);
		Information("BuildTag: " + buildTag);
		Information("MajorVersion: " + major_version);
		Information("BuildVersion: " + version);
		Information("CIVersion: " + ciVersion);
	});

Task("SetVersion")
   .Does(() => {
	   //version = major_version + "." + buildCounter.ToString() + ".0";
	   Information("Version: " + version);
       ReplaceRegexInFiles(asm,  "(?<=AssemblyVersion\\(\")(.+?)(?=\"\\))", version);
       ReplaceRegexInFiles(asm,  "(?<=Version\\(\")(.+?)(?=\"\\))", version);
   });

// Builds the code
Task ("Build")
	.Does (() => {

		//Build 64bit versions of the solution
		MSBuild (sln, new MSBuildSettings 
					{
						Verbosity = Verbosity.Quiet,
						Configuration = buildType
					}.WithProperty("build", buildCounter.ToString()));
					
						
		var file = MakeAbsolute(Directory(releaseFolder)) + releaseDll;
		version = GetVersionNumber(file);
		ciVersion = GetVersionNumberWithContinuesIntegrationNumberAppended(file, buildCounter);
		Information("Version: " + version);
		Information("CI Version: " + ciVersion);
		PushVersion(ciVersion);
	});

// Create Nuget Package
Task ("Nuget")
    .WithCriteria (buildType == "Release")
	.Does (() => {
		CreateDirectory ("./nupkg/");
		ReplaceRegexInFiles(nuspecFile, "0.0.0", version);
		
		NuGetPack (nuspecFile, new NuGetPackSettings { 
			Verbosity = NuGetVerbosity.Detailed,
			OutputDirectory = "./nupkg/"
		});	
	});

// Restore Nuget packages
Task ("NugetRestore")
	.Does (() => {
		var blockText = "Nuget Restore";
		StartBlock(blockText);
		NuGetRestore(sln);
		EndBlock(blockText);
	});

//Push to Nuget
Task ("Push")
	.WithCriteria (buildType == "Release")
	.Does (() => {
		// Get the newest (by last write time) to publish
		var newestNupkg = GetFiles ("nupkg/*.nupkg")
			.OrderBy (f => new System.IO.FileInfo (f.FullPath).LastWriteTimeUtc)
			.LastOrDefault();
		var apiKey = EnvironmentVariable("NugetKey");
		NuGetPush (newestNupkg, new NuGetPushSettings { 
			Verbosity = NuGetVerbosity.Detailed,
			Source = "https://www.nuget.org/api/v2/package/",
			ApiKey = apiKey
		});
	});
Task ("Sign")
    .IsDependentOn("Build")
    .Does(() => {
        if (!String.IsNullOrEmpty(signPassword))
        {
		    Information("Signing Assembly");
            var file = MakeAbsolute(Directory(releaseFolder)) + releaseDll;
            var asmb = new FilePath(file);
            Sign(asmb, new SignToolSignSettings {
                Description = "Signed by Lachee during a automated build",
                TimeStampUri = new Uri("http://timestamp.digicert.com"),
                CertPath = signCertificate,
                Password = signPassword
            });
        }
        else
        {
		    Information("Skipping Signing");
        }
    });

Task ("Default")
	.IsDependentOn ("OutputVariables")
	.IsDependentOn ("DiscoverBuildDetails")
	.IsDependentOn ("NugetRestore")
	.IsDependentOn ("SetVersion")
	.IsDependentOn ("Build");
Task ("NugetBuild")
	.IsDependentOn ("OutputVariables")
	.IsDependentOn ("DiscoverBuildDetails")
	.IsDependentOn ("NugetRestore")
	.IsDependentOn ("SetVersion")
	.IsDependentOn ("Build")
	.IsDependentOn ("Sign")
    .IsDependentOn ("Nuget");
Task ("NugetBuildPush")
	.IsDependentOn ("OutputVariables")
	.IsDependentOn ("DiscoverBuildDetails")
	.IsDependentOn ("NugetRestore")
	.IsDependentOn ("SetVersion")
	.IsDependentOn ("Build")
	.IsDependentOn ("Sign")
    .IsDependentOn ("Nuget")
    .IsDependentOn ("Push");
    
RunTarget (target);

// Code to start a TeamCity log block
public void StartBlock(string blockName)
{
	if(runningOnTeamCity)
	{
		TeamCity.WriteStartBlock(blockName);
	}
}

// Code to start a TeamCity build block
public void StartBuildBlock(string blockName)
{
	if(runningOnTeamCity)
	{
		TeamCity.WriteStartBuildBlock(blockName);
	}
}

// Code to end a TeamCity log block
public void EndBlock(string blockName)
{
	if(runningOnTeamCity)
	{
		TeamCity.WriteEndBlock(blockName);
	}
}

// Code to end a TeamCity build block
public void EndBuildBlock(string blockName)
{
	if(runningOnTeamCity)
	{
		TeamCity.WriteEndBuildBlock(blockName);
	}
}

// Code to push the Version number to the build system
public void PushVersion(string version)
{
	if(runningOnTeamCity)
	{
		TeamCity.SetBuildNumber(version);
	}
	if(runningOnAppVeyor)
	{
		Information("Pushing version to AppVeyor: " + version);
		AppVeyor.UpdateBuildVersion(version);
	}
}

// Code to push the Test Results to AppVeyor for display purposess
public void PushTestResults(string filePath)
{
	var file = MakeAbsolute(File(filePath));
	if(runningOnAppVeyor)
	{
		AppVeyor.UploadTestResults(file, AppVeyorTestResultsType.NUnit3);
	}
}