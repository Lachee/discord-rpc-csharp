using UnityEditor;
using UnityEngine;

namespace DiscordRPC.UnityEditor
{
	[CustomPropertyDrawer(typeof(DiscordAsset))]
	public class DiscordAssetDrawer : PropertyDrawer
	{
		public const float keySize = 150;
		public const int lines = 3;

		public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
		{
			SerializedProperty image = prop.FindPropertyRelative("image");
			SerializedProperty tooltip = prop.FindPropertyRelative("tooltip");
			SerializedProperty snowflake = prop.FindPropertyRelative("snowflake");

			float h2 = pos.height / lines;
			EditorGUI.LabelField(pos, label);
			
			if (snowflake.longValue > 0)
				EditorGUI.LabelField(pos, new GUIContent(" "), new GUIContent("(" + snowflake.longValue.ToString() + ")", "The unique Snowflake ID of the image. May not be accurate to the current image key."));
			
			int indent = EditorGUI.indentLevel++;
			{
				EditorGUI.PropertyField(new Rect(pos.x, pos.y + h2 * 1, pos.width, h2), image, new GUIContent("Image Key", "The key of the image uploaded to the discord app page."));
				EditorGUI.PropertyField(new Rect(pos.x, pos.y + h2 * 2 + 2, pos.width, h2), tooltip, new GUIContent("Tooltip", "The tooltip to be displayed for the image."));
			}
			EditorGUI.indentLevel = indent;

		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return (base.GetPropertyHeight(property, label) * lines) + 4;
		}
	}
}
