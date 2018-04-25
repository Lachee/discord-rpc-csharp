using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DiscordManager))]
public class DiscordManagerEditor : Editor
{
	const string BAD_COMPILE = "The current build platform is not supported by this version of Discord Rich Presence. A native library is requried from the offical Discord Rich Presence library. Future versions of DiscordRPC-Sharp will support Linux and Mac.\n\nFor convience, the properties will still be editable, but no attempt to connect to Discord will be made.";


#if !(UNITY_WSA || UNITY_WSA_10_0 || UNITY_STANDALONE_WIN)
	public override void OnInspectorGUI()
	{
		EditorGUILayout.HelpBox(BAD_COMPILE, MessageType.Error);
		base.OnInspectorGUI();
	}
#endif
}
