using UnityEditor;
using UnityEngine;

namespace DiscordRPC.UnityEditor
{
	[CustomPropertyDrawer(typeof(DiscordEvent))]
	public class DiscordEventDrawer : PropertyDrawer
	{
		private bool INCLUDE_NONE = false;

		public const float keySize = 150;
		public const int lines = 3;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			DiscordEvent propval = (DiscordEvent)property.intValue;
			DiscordEvent newval = DiscordEvent.None;

			Rect buttonPos;
			int offset = INCLUDE_NONE ? 0 : 1;
			float buttonWidth = (position.width - EditorGUIUtility.labelWidth) / (4 - offset);

			EditorGUI.LabelField(new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height), label);
			EditorGUI.BeginProperty(position, label, property);
			{

				if (INCLUDE_NONE)
				{
					buttonPos = new Rect(position.x + EditorGUIUtility.labelWidth + buttonWidth * 0, position.y, buttonWidth, position.height);
					if (GUI.Toggle(buttonPos, propval == DiscordEvent.None, "None", EditorStyles.miniButtonLeft))
						newval = DiscordEvent.None;
				}

				buttonPos = new Rect(position.x + EditorGUIUtility.labelWidth + buttonWidth * (1 - offset), position.y, buttonWidth, position.height);
				if (GUI.Toggle(buttonPos, (propval & DiscordEvent.Join) == DiscordEvent.Join, "Join", INCLUDE_NONE ? EditorStyles.miniButtonMid : EditorStyles.miniButtonLeft))
					newval |= DiscordEvent.Join;

				buttonPos = new Rect(position.x + EditorGUIUtility.labelWidth + buttonWidth * (2 - offset), position.y, buttonWidth, position.height);
				if (GUI.Toggle(buttonPos, (propval & DiscordEvent.Spectate) == DiscordEvent.Spectate, "Spectate", EditorStyles.miniButtonMid))
					newval |= DiscordEvent.Spectate;

				buttonPos = new Rect(position.x + EditorGUIUtility.labelWidth + buttonWidth * (3 - offset), position.y, buttonWidth, position.height);
				if (GUI.Toggle(buttonPos, (propval & DiscordEvent.JoinRequest) == DiscordEvent.JoinRequest, "Invites", EditorStyles.miniButtonRight))
					newval |= DiscordEvent.JoinRequest;

				property.intValue = (int)newval;
			}
			EditorGUI.EndProperty();

		}
	}
}
