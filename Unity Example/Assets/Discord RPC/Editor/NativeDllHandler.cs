using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class DiscordNativeInstall  {

    const bool ENABLED = false;

	const string PLUGIN_PATH_86_64 = "Discord RPC/Plugins/x86_64";
	const string PLUGIN_PATH_86 = "Discord RPC/Plugins/x86";
	const string PLUGIN_NAME = "DiscordRPC.Native.dll";

	static DiscordNativeInstall()
	{        
        if (PlayerSettings.GetApiCompatibilityLevel(BuildTargetGroup.Standalone) == ApiCompatibilityLevel.NET_2_0_Subset)
        {
            var result = EditorUtility.DisplayDialog("Incompatible API Level", "You are currently using the .NET 2.0 Subset in this project. Discord RPC is incompatible with this version and requires the full version.\r\n\r\n" +
                "Failure to change to the full version of .NET 2.0 may break builds and hard crash the game.\r\n\r\n" + 
                "Would you like to upgrade the project to .NET 2.0 now?", "Yes", "No");

            if (result)
            {
                PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.Standalone, ApiCompatibilityLevel.NET_2_0);
                Debug.Log("Converted project to .NET 2.0 successfully");
            }
            else
            {
                Debug.LogError("Discord RPC is unable to work in a .NET 2.0 SUBSET enviroment. Builds may not work and may hardcrash if not fixed. Please manually fix by changing player settings.");
            }
        }

#pragma warning disable 0162
        if (!ENABLED) return;
        
		//We are not windows, cannot do anything
#if !UNITY_STANDALONE_WIN
		Debug.LogError("Cannot use the discord library because the natives do not support non-windows platforms yet.");
		CleanRoot();
		return;
#endif
		
		//Copy the library over
		CopyLibrary();

#pragma warning restore 0162

	}

	private static void CopyLibrary()
	{
#if UNITY_EDITOR_64
		CopyLibrary(PLUGIN_PATH_86_64, PLUGIN_NAME);
#else
		CopyLibrary(PLUGIN_PATH_86, PLUGIN_NAME);
#endif
	}

	private static void CleanRoot()
	{
		string lib = Path.Combine(Application.dataPath + "/../", PLUGIN_NAME);
		if (File.Exists(lib))
		{
			try
			{
				Debug.Log("Attempting to delete the old native '" + lib + "'");
				File.Delete(lib);
			}
			catch(Exception e)
			{
				Debug.LogError("Failed to delete the old native: " + e.Message);
			}
		}
	}

	private static void CopyLibrary(string path, string file)
	{
		//Prepare the paths
		string sourcePath = Path.Combine(Application.dataPath, path);
		string destPath = Application.dataPath + "/../";

		string sourceFile = Path.Combine(sourcePath, file);
		string destFile = Path.Combine(destPath, file);

		//Make sure the path exists
		if (!File.Exists(sourceFile))
		{
			Debug.LogWarning("Since you are below Unity 2017, the DLL needs to be copied over. Could not find the native dll '" + sourceFile + "' to copy. Make sure you build the library before use!");
		}

		//Make sure the file doesn't already exist
		if (File.Exists(destFile))
		{
			//They have both been modified at the last time, probably best to just leave if.
			if (File.GetLastWriteTime(sourceFile) == File.GetLastWriteTime(destFile))
				return;
			
			//Give a warning about restarting
			Debug.Log("Since you are below Unity 2017, the DLL needs to be copied over. The file '" + destFile + "' already exists. Will override but will require a unity restart to take effect");
		}
		else
		{
			Debug.Log("Since you are below Unity 2017, the DLL needs to be copied over. The file '" + destFile + "' is being copied to work with your current unity settings");
		}

		//Copy the file
		try
		{
			File.Copy(sourceFile, destFile, true);
			File.SetAttributes(destFile, File.GetAttributes(sourceFile) & ~FileAttributes.ReadOnly);
			Debug.Log("Copy succesful");
		}
		catch (Exception e)
		{
			Debug.LogError("A exception occured while trying to copy the native: " + e.Message);
		}
	}
}
