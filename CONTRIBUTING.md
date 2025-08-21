# Welcome to the Discord RPC C# Contribution Guide
Thank you for wanting to contribute to this project! All contributions are welcome, but please make note of some of the rules and guidelines I have set out in this file to get your changes accepted.

## 🥲 Unwanted Contributions
All contributions are welcome and I am happy for any contribution. However, there are some things that will not be accepted:
- Spelling only fixes (rude to only contribute to something copilot could do)
- Complete or large rewrites (unwanted work load to review)
- Dependency substitutions / removals / additions (these require a issue and discussion first)
- Support for features only provided by custom Discord clients
- Obviously AI additions

## 🤓 Some Points
- All namespaces must retain `DiscordRPC` as the root.
- Do not include licensing headers. The codebase is already licensed.
- All public methods, fields, and properties must be documented.
- If adding new Rich Presence fields:
    - Add an example of them being used in the `DiscordRPC.Example`
    - Add the check to the Rich Presence equality
    - Make sure not having it doesnt crash the app
- Any change must work for .NET Framework 4.5
    - This minimum is to support Unity3D 2020.X+, MonoGame, and Godot. These engines do not use the latest .NET library or use a custom implementation of the runtime.
    - Language features are capped to `C# 7.3` to support Unity
- Any changes must work for Linux, MacOS, and Windows. 
- If contributing Linux fixes, specify the setup in the PR and why its needed.
- Follow existing styling in the project.
    - 1 Tab Indentation ( yes, im disgusted too, but at the time this was the VS default )
    - Brackets on new lines
    - Bracketless IFS are allowed.
    - Use the gaurd paradigm
    - Getters with `=>` are allowed and so are string interpolation with `$"{value}"`
        - Planned to update the rest of the codebase with them.

## The Entities/ folder
I plan to move all objects into their own namespace called `DiscordRPC.Entity`, however this is a major breaking change and will likely be a `v2.0.0` release.
For now, to ease in maintainability, entities are within the `Entity/` folder but **do not have the correct namespace**. This is intentional.

> [!NOTE]
> Plans to also update the root namespace to either `Lachee.DiscordRPC`, `Lachee.DiscordIPC`, or simply `Lachee.Discord.RPC`. 
> A discussion is needed for the merit of a change like this.

## 🫡 Getting Started
1. Make sure .NET is installed
2. Install Visual Studio or Visual Studio Code
3. Fork and clone the projec
4. Create a patch branch on your fork: `checkout -b patch-1` 
5. Make Changes
6. Test it works
7. Create a PR from your `patch-1` to `lachee/master` (or the default branch if that ever changes)

See the README for more about building and testing your version. 

## 📦 Building & Testing

[![Release 📦](https://github.com/Lachee/discord-rpc-csharp/actions/workflows/release.yml/badge.svg)](https://github.com/Lachee/discord-rpc-csharp/actions/workflows/release.yml) [![Documentation 📚](https://github.com/Lachee/discord-rpc-csharp/actions/workflows/documentation.yml/badge.svg)](https://github.com/Lachee/discord-rpc-csharp/actions/workflows/documentation.yml)

```
dotnet build -c Release
```
> [!NOTE]
> This is a stub. More indepth build/testing guides will be available at some point. 

## 😴 Licensing
By submitting a PR, you are agreeing that your code is licensed under the same as defined in the accompying license file of the project. You transfer all rights to the code to the project and its community in perpetuity and retain authorship credit to the code.