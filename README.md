# Discord Rich Presence

[![Codacy Badge](https://api.codacy.com/project/badge/Grade/a3fc8999eb734774bff83179fee2409e)](https://app.codacy.com/app/Lachee/discord-rpc-csharp?utm_source=github.com&utm_medium=referral&utm_content=Lachee/discord-rpc-csharp&utm_campaign=badger)

This is a C# implementation of the [Discord RPC](https://github.com/discordapp/discord-rpc) library which was originally written in C++. You will be able to set the Rich Presence of your game directly in C# without an DllImport nonsence. 

Please note that Join/Spectate features are not yet supported by this library, but they are planned in the future.


# Installation

Installation is super easy. You just need to build the project with .NET 4.6
**Dependencies:**
 - Newtonsoft.Json 
- (For Unity) Unity 5 with .NET 3.5 (aka .NET 2). Please note that .NET 2 Subset will **NOT** work.

# Cake Build

The project can be build using the provided [Cake](https://cakebuild.net/) build script.
To build the project simply execute the build Powershell script:
```
.\build.ps1
```
This will build the project with the Release configuration without creating a Nuget package.

## Build script parameters
The following parameters can be passed to the build script:
1. **`target`**  
Valid parameters are:
- `Default` has the same effect as executing the script without the parameter
- `NugetBuild` will build a Nuget pacakge and attempt to push it to the Nuget repository (This is geared for running from a build server like AppVeyor)
2. **`ScriptArgs`**  
Can be used to pass one or both of the following build arguments:
- `buildCounter` is used to know how many builds the server has generated and is used to generate a CI version number for [continuous Nuget releases](https://www.xavierdecoster.com/post/2013/04/29/semantic-versioning-auto-incremented-nuget-package-versions.html) - Defaults to 0
- `buildType` can be used to pass the MSBuild configuration that should be execute - Defaults to `Release`

To run the script on a build server, pass the following line:
```
.\build.ps1 -target NugetBuild -ScriptArgs '-buildCounter=1','-buildType="Release"'
```

## Enviroment Variables
In order to push the Nuget package to the Nuget Repository the following enviroment variables have to be provided:
1. `apiKey`  
nuget.org [API key](https://docs.microsoft.com/en-us/nuget/create-packages/publish-a-package#create-api-keys)

**Note**
```
.\build.ps1 -target NugetBuild -ScriptArgs '-buildCounter=1','-buildType="Debug"'
```
will not generate or push the Nuget pacakge as we do not want to push Debug builds to Nuget


## Usage

This usage is out of date. Please check the example project provided as a rough guide. 
_want to help out and improve on this documentation? Check out https://github.com/Lachee/discord-rpc-csharp/issues/9 _

**Always remember to Dispose of the client on shutdown!**

For a quick start:
```csharp
//TODO: Update the quick start
```
