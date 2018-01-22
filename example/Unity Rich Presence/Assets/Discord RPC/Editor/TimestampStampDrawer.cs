using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(UnityPresence.Time))]
public class TimestampStampDrawer : PropertyDrawer
{
	// Draw the property inside the given rect
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		// Using BeginProperty / EndProperty on the parent property means that
		// prefab override logic works on the entire property.
		EditorGUI.BeginProperty(position, label, property);

		// Draw label
		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

		// Don't make child fields be indented
		var indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;

		// Calculate rects
		float buttonWidth = 75;
		var amountRect = new Rect(position.x, position.y, position.width - buttonWidth - 5, position.height);
		var nowRect = new Rect(position.x + amountRect.width + 5, position.y, buttonWidth, position.height);
		
		// Draw fields - passs GUIContent.none to each so they are drawn without labels
		EditorGUI.PropertyField(amountRect, property.FindPropertyRelative("timestamp"), GUIContent.none);
		if (GUI.Button(nowRect, "Set Now"))
			property.FindPropertyRelative("timestamp").longValue = new UnityPresence.Time(System.DateTime.UtcNow);

		// Set indent back to what it was
		EditorGUI.indentLevel = indent;

		EditorGUI.EndProperty();
	}
}
