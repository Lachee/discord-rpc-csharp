using UnityEditor;
using UnityEngine;

namespace DiscordRPC.UnityEditor
{
	[CustomPropertyDrawer(typeof(DiscordParty))]
	public class DiscordPartyDrawer : PropertyDrawer
	{
		public const float keySize = 150;
		public const int lines = 3;

		public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
		{
			SerializedProperty identifer = prop.FindPropertyRelative("identifer");
			SerializedProperty size = prop.FindPropertyRelative("size");
			SerializedProperty sizeMax = prop.FindPropertyRelative("maxSize");

			float h2 = pos.height / lines;
			EditorGUI.LabelField(pos, label);

			int indent = EditorGUI.indentLevel++;
			{
				EditorGUI.PropertyField(new Rect(pos.x, pos.y + h2 * 1, pos.width, h2), identifer, new GUIContent("Identifier", "The unique ID for the party."));
				EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(identifer.stringValue));
				{
					float min = size.intValue;
					float max = Mathf.Max(min, sizeMax.intValue);
					float limitMax = Mathf.Max(max + 1, 8);

					float fieldSize = 22;
					Rect sliderRect = new Rect(pos.x, pos.y + h2 * 2 + 2, pos.width - (fieldSize + 5) * 2, h2);
					Rect fieldRectA = new Rect(pos.x + sliderRect.width + 5, pos.y + h2 * 2 + 2, fieldSize, h2);
					Rect fieldRectB = new Rect(pos.x + sliderRect.width + fieldSize + 10f, pos.y + h2 * 2 + 2, fieldSize, h2);

					EditorGUI.MinMaxSlider(sliderRect, new GUIContent("Size / Max Size", "The current size of the party"), ref min, ref max, 0, limitMax);
					size.intValue = Mathf.FloorToInt(min);
					sizeMax.intValue = Mathf.FloorToInt(max);

					EditorGUI.indentLevel = 0;
					size.intValue = EditorGUI.IntField(fieldRectA, GUIContent.none, size.intValue);
					sizeMax.intValue = EditorGUI.IntField(fieldRectB, GUIContent.none, sizeMax.intValue);

				}
				EditorGUI.EndDisabledGroup();

			}
			EditorGUI.indentLevel = indent;

		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return (base.GetPropertyHeight(property, label) * lines) + 4;
		}
	}
}
