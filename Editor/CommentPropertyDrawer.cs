using UnityEditor;
using UnityEngine;

namespace AID
{
    [CustomPropertyDrawer(typeof(Comment))]
    public class CommentPropertyDrawer : PropertyDrawer
    {
        private static float BodyPropHeight(SerializedProperty property)
        {
            return EditorStyles.textArea.CalcSize(new GUIContent(property.FindPropertyRelative("body").stringValue)).y;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) + EditorGUIUtility.singleLineHeight * 3 + BodyPropHeight(property);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var bodyH = BodyPropHeight(property);

            SerializedProperty p;

            EditorGUI.BeginProperty(position, label, property);
            position.height = EditorGUIUtility.singleLineHeight;

            p = property.FindPropertyRelative("isTask");
            EditorGUI.PropertyField(position, p);
            position.y += EditorGUI.GetPropertyHeight(p);

            p = property.FindPropertyRelative("priority");
            EditorGUI.PropertyField(position, p);
            position.y += EditorGUI.GetPropertyHeight(p);

            p = property.FindPropertyRelative("body");
            var innerposition = EditorGUI.PrefixLabel(position, new GUIContent("Body"));
            innerposition.height = bodyH;
            p.stringValue = EditorGUI.TextArea(innerposition, p.stringValue);
            position.y += bodyH;

            p = property.FindPropertyRelative("linkedObject");
            EditorGUI.PropertyField(position, p);
            position.y += EditorGUI.GetPropertyHeight(p);

            p = property.FindPropertyRelative("hidden");
            EditorGUI.PropertyField(position, p);
            position.y += EditorGUI.GetPropertyHeight(p);
            EditorGUI.EndProperty();
        }
    }
}