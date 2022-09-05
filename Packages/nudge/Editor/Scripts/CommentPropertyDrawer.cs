using UnityEditor;
using UnityEngine;

namespace AID.Nudge
{
    [CustomPropertyDrawer(typeof(Comment))]
    public class CommentPropertyDrawer : PropertyDrawer
    {
        private const string HiddenPropName = "hidden";
        private readonly GUIContent HiddenPropLabel = new GUIContent("Comment Hidden");

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var isHidden = property.FindPropertyRelative(HiddenPropName).boolValue;

            if (isHidden)
                return EditorGUIUtility.singleLineHeight;

            property.isExpanded = true;
            return EditorGUI.GetPropertyHeight(property, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var hiddenProp = property.FindPropertyRelative(HiddenPropName);

            if (hiddenProp.boolValue)
            {
                EditorGUI.PropertyField(position, hiddenProp, HiddenPropLabel);
                return;
            }
            property.isExpanded = true;
            EditorGUI.PropertyField(position, property, label, true);
        }
    }
}
