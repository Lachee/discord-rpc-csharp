using UnityEditor;
using UnityEngine;

namespace DiscordRPC.UnityEditor
{
	[CustomPropertyDrawer(typeof(DiscordTimestamp))]
	public class DiscordTimestampDrawer : PropertyDrawer
	{
		public const float buttonWidth = 50f;

		public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
		{
			SerializedProperty timestamp = prop.FindPropertyRelative("timestamp");

			// Draw curve
			EditorGUI.PropertyField(new Rect(pos.x, pos.y, pos.width - buttonWidth - 5f, pos.height), timestamp, label);
			if (GUI.Button(new Rect(pos.x + pos.width - buttonWidth, pos.y, buttonWidth, pos.height), new GUIContent("Now", "Sets the time to the current time")))
			{
				timestamp.longValue = new DiscordTimestamp(Time.time).timestamp;
			}
		}
	}
}
