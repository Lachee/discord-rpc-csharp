using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class DiscordNativeInstall  {
	
	const string PLUGIN_PATH_86_64 = "Discord RPC/Plugins/x64";
	const string PLUGIN_PATH_86 = "Discord RPC/Plugins/x86";
	const string PLUGIN_NAME = "DiscordRPC.Native.dll";

	static DiscordNativeInstall()
	{

#pragma warning disable 0162

		//In SUBSET mode, this is bad
#if NET_2_0_SUBSET
		Debug.LogError("Cannot use the discord library because .NET 2.0 SUBSET is being used. Please switch to either .NET 2.0 or .NET 4.6 in the player settings.");
		CleanRoot();
		return;
#endif

		//We are not windows, cannot do anything
#if !UNITY_STANDALONE_WIN
		Debug.LogError("Cannot use the discord library because the natives do not support non-windows platforms yet.");
		CleanRoot();
		return;
#endif

		//Make sure we are below 2017
#if !UNITY_2017_1_OR_NEWER
		Debug.Log("Since you are below Unity 2017, the DLL needs to be copied over");
		CopyLibrary();
		return;
#endif

		//Clean the root.
		CleanRoot();
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
		string lib = Path.Combine(Application.dataPath, PLUGIN_NAME);
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
		string destPath = Application.dataPath;

		string sourceFile = Path.Combine(sourcePath, file);
		string destFile = Path.Combine(destPath, file);

		//Make sure the path exists
		if (!File.Exists(sourceFile))
		{
			Debug.LogWarning("Could not find the native dll '" + sourceFile + "' to copy. Make sure you build the library before use!");
		}

		//Make sure the file doesn't already exist
		if (File.Exists(destFile))
		{
			//They have both been modified at the last time, probably best to just leave if.
			if (File.GetLastWriteTime(sourceFile) == File.GetLastWriteTime(destFile))
				return;

			//Give a warning about restarting
			Debug.Log("The file '" + destFile + "' already exists. Will override but will require a unity restart to take effect");
		}
		else
		{
			Debug.Log("The file '" + destFile + "' is being copied to work with your current unity settings");
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
