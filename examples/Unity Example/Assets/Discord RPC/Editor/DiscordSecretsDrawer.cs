using UnityEditor;
using UnityEngine;

namespace DiscordRPC.UnityEditor
{
	[CustomPropertyDrawer(typeof(DiscordSecrets))]
	public class DiscordSecretsDrawer : PropertyDrawer
	{
		public const float keySize = 150;
		public const int lines = 3;

		public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
		{
			SerializedProperty joinsecret = prop.FindPropertyRelative("joinSecret");
			SerializedProperty spectatesecret = prop.FindPropertyRelative("spectateSecret");

			float h2 = pos.height / lines;
			EditorGUI.LabelField(pos, label);

			int indent = EditorGUI.indentLevel++;
			{
				EditorGUI.PropertyField(new Rect(pos.x, pos.y + h2 * 1 - 4, pos.width, h2), joinsecret);
				EditorGUI.PropertyField(new Rect(pos.x, pos.y + h2 * 2 -2 , pos.width, h2), spectatesecret);

			}
			EditorGUI.indentLevel = indent;

		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return (base.GetPropertyHeight(property, label) * lines) + 10;
		}
	}
}
