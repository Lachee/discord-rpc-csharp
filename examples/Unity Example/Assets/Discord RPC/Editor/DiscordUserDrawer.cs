using UnityEditor;
using UnityEngine;

namespace DiscordRPC.UnityEditor
{
	[CustomPropertyDrawer(typeof(DiscordUser))]
	public class DiscordUserDrawer : PropertyDrawer
	{
		public const float keySize = 150;
		public const int lines = 10;

		public bool debugRectangles = false;
		
		public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
		{
			int indent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;
			{
				SerializedProperty foldout = prop.FindPropertyRelative("e_foldout");
				SerializedProperty avatar = prop.FindPropertyRelative("_avatar");
				SerializedProperty name = prop.FindPropertyRelative("_username");
				SerializedProperty discrim = prop.FindPropertyRelative("_discriminator");
				SerializedProperty snowflake = prop.FindPropertyRelative("_snowflake");

				SerializedProperty cacheSize = prop.FindPropertyRelative("_cacheSize");
				SerializedProperty cacheFormat = prop.FindPropertyRelative("_cacheFormat");
				SerializedProperty avatarHash = prop.FindPropertyRelative("_avatarHash");



				string displayName = "@" + name.stringValue + "#" + discrim.intValue.ToString("D4");

				Rect imageRectangle = new Rect(16, 16, 108, 108);
				imageRectangle.position += pos.position;

				Rect usernameRectangle = new Rect(imageRectangle.xMax + 10, pos.y + 16, pos.width - (imageRectangle.width + 26), 16);
				Rect snowflakeRectange = usernameRectangle; snowflakeRectange.y += 16;

				Rect cacheSizeRectangle = snowflakeRectange; cacheSizeRectangle.y += 32;
				Rect avatarHashRectangle = cacheSizeRectangle; avatarHashRectangle.y += 16;

				//Draw a rect covering everything
				DrawRect(pos, Color.red);

				//Draw the label then the left over space it gave us
				if (foldout.boolValue = EditorGUI.Foldout(pos, foldout.boolValue, label))
				{
					DrawAvatar(imageRectangle, avatar);

					DrawRect(usernameRectangle, Color.green);
					DrawRect(snowflakeRectange, Color.white);

					DrawRect(cacheSizeRectangle, Color.blue);
					DrawRect(avatarHashRectangle, Color.cyan);

					EditorGUI.LabelField(usernameRectangle, new GUIContent(displayName));

					if (snowflake.longValue != 0)
						EditorGUI.LabelField(snowflakeRectange, new GUIContent("(" + snowflake.longValue.ToString() + ")"));

					EditorGUI.LabelField(cacheSizeRectangle, new GUIContent(cacheSize.intValue + " x " + cacheSize.intValue + ", " + cacheFormat.enumNames[cacheFormat.enumValueIndex]));
					EditorGUI.LabelField(avatarHashRectangle, new GUIContent(avatarHash.stringValue));
				}
			}
			EditorGUI.indentLevel = indent;
		}

		/// <summary>Draws the avatar box </summary>
		private void DrawAvatar(Rect position, SerializedProperty avatarProperty)
		{ 
			//Draw the backing colour
			EditorGUI.HelpBox(position, "", MessageType.None);

			//Draw the avatar if we have one
			if (avatarProperty != null && avatarProperty.objectReferenceValue != null)
				EditorGUI.DrawTextureTransparent(position, avatarProperty.objectReferenceValue as Texture2D, ScaleMode.ScaleToFit);
		}

		private void DrawRect(Rect rect, Color color)
		{
			if (!debugRectangles) return;
			EditorGUI.DrawRect(rect, color);
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			SerializedProperty foldout = property.FindPropertyRelative("e_foldout");
			float baseHeight = base.GetPropertyHeight(property, label);

			if (!foldout.boolValue) return baseHeight;
			return baseHeight + 108 + 6;
		}
	}
}
