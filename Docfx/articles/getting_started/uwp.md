> [!NOTE]
> This article is a stub and is not complete. 
>
> Please help by [expanding it](https://github.com/Lachee/discord-rpc-csharp/blob/master/Docfx/articles/advance/uwp.md).

# UWP / .NET MAUI / WIN UI 3

For now, the library doesn't work on UWP applications until we find the issue and fix it.

In order to make this library work with the WIN UI 3 related applications such as .NET MAUI, you need to define `runFullTrust` Capability inside `Package.appxmanifest`.

Here is an example of how to add `runFullTrust` to your WIN UI 3 application:

`Package.appxmanifest`:

```xml
<?xml version="1.0" encoding="utf-8"?>

<Package
  xmlns="http://schemas.microsoft.com/appx/manifest/foundation/windows10"
  xmlns:uap="http://schemas.microsoft.com/appx/manifest/uap/windows10"
  xmlns:rescap="http://schemas.microsoft.com/appx/manifest/foundation/windows10/restrictedcapabilities"
  IgnorableNamespaces="uap rescap">
  ...
    <Capabilities>
	    <rescap:Capability Name="runFullTrust" />
    </Capabilities>
</Package>
```

If you use .NET MAUI or WIN UI 3 template for C#, it automatically puts `runFullTrust` capability.

