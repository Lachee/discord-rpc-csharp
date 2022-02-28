using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DiscordRPC.UnityEditor 
{
	[CustomPropertyDrawer(typeof(CharacterLimitAttribute))]
	public class CharacterLimitAttributeDrawer : PropertyDrawer
	{
		// Draw the property inside the given rect
		public override void OnGUI(Rect pos, SerializedProperty property, GUIContent label)
		{
			// First get the attribute since it contains the range for the slider
			CharacterLimitAttribute range = attribute as CharacterLimitAttribute;

			if (property.propertyType != SerializedPropertyType.String)
			{
				EditorGUI.HelpBox(pos, "The CharLimit property is only valid on strings.", MessageType.Error);
				return;
			}

			//Store the size of the limit and the original colour
			Color original = GUI.color;

			//Make the box red if we are too big
			int remaining = range.max - property.stringValue.Length;
			if (remaining < 0) GUI.color = Color.red;

			//prepare the remaining label
			//string remainingLabel = property.stringValue.Length + "/" + range.max;
			string remainingLabel = remaining.ToString();

			//Draw the label
		
			var remainingStyle = new GUIStyle() { alignment = TextAnchor.MiddleCenter };
			var remainingContent = new GUIContent(remainingLabel, "Characters remaining in the text");

			float remainingSize = 50;
			float textSize = pos.width - remainingSize - 5;

			Rect textRect = new Rect(pos.x, pos.y, textSize, pos.height);
			Rect labelRect = new Rect(pos.x + textSize + 5, pos.y, remainingSize, pos.height);

			//GUI.Box(textRect, GUIContent.none);
			GUI.Box(labelRect, GUIContent.none);

			//Draw the text field and the remaining contents field
			GUI.Label(labelRect, remainingContent, remainingStyle);
			EditorGUI.PropertyField(textRect, property, label);
		
			if (range.enforce && property.stringValue.Length > range.max)
				property.stringValue = property.stringValue.Substring(0, range.max);

			GUI.color = original;
		}
	}
}