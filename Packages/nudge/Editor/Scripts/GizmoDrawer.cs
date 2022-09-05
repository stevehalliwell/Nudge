using UnityEditor;
using UnityEngine;

namespace AID.Nudge
{
    public static class GizmoDrawer
    {
        [DrawGizmo(GizmoType.Pickable | GizmoType.Selected | GizmoType.NonSelected, typeof(CommentGameObject))]
        public static void DrawGizmo(CommentGameObject commentGO, GizmoType gizmoType)
        {
            var settings = Settings.instance;
            if (!settings.showHidden && commentGO.comment.hidden && (gizmoType & GizmoType.Selected) == 0)
                return;

            Gizmos.DrawIcon(
                commentGO.transform.position,
                commentGO.comment.isTask ? settings.commentTaskGizmoPath : settings.sceneCommentGizmoPath,
                true);

            if ((gizmoType & GizmoType.Selected) != 0 || settings.drawLinkedConnection)
            {
                foreach (var item in commentGO.comment.linkedObjects)
                {
                    AttemptToDrawLine(commentGO.transform, item, commentGO.comment, settings);
                }
            }

            if (!commentGO.hidesTextInSceneViewport && !commentGO.comment.hidden)
            {
                DrawString(commentGO, settings);
            }
        }

        static void AttemptToDrawLine(Transform from, UnityEngine.Object targetObj, Comment comment, Settings settings)
        {
            if (targetObj != null)
            {
                Transform linkedTransform = null;
                if (targetObj is GameObject)
                {
                    linkedTransform = (targetObj as GameObject).transform;
                }
                else if (targetObj is Component)
                {
                    linkedTransform = (targetObj as Component).transform;
                }

                if (linkedTransform != null)
                {
                    Gizmos.DrawIcon(
                        linkedTransform.position,
                        comment.isTask ? settings.commentTaskLinkedGizmoPath : settings.commentLinkedGizmoPath,
                        true);

                    var prevCol = Gizmos.color;
                    Gizmos.color = prevCol * settings.linkedTint;
                    Gizmos.DrawLine(from.position, linkedTransform.position);
                    Gizmos.color = prevCol;
                }
            }
        }

        static public void DrawString(CommentGameObject commentGO, Settings settings)
        {
            var view = UnityEditor.SceneView.currentDrawingSceneView;
            if (!view)
                return;
            Vector3 worldPosition = commentGO.transform.position;
            string text = commentGO.comment.body;
            Color normalTextColor = commentGO.normalTextColor;
            Color hoverTextColor = commentGO.hoverTextColor;
            if (commentGO.comment.isTask)
            {
                normalTextColor *= settings.isTaskTint;
                hoverTextColor *= settings.isTaskTint;
            }
            Vector2 anchor = commentGO.anchor;
            float textSize = commentGO.textSize;
            Vector3 screenPosition = view.camera.WorldToScreenPoint(worldPosition);
            if (screenPosition.y < 0 || screenPosition.y > view.camera.pixelHeight || screenPosition.x < 0 || screenPosition.x > view.camera.pixelWidth || screenPosition.z < 0)
                return;
            var pixelRatio = UnityEditor.HandleUtility.GUIPointToScreenPixelCoordinate(Vector2.right).x - UnityEditor.HandleUtility.GUIPointToScreenPixelCoordinate(Vector2.zero).x;
            UnityEditor.Handles.BeginGUI();
            var style = new GUIStyle(GUI.skin.label)
            {
                fontSize = (int)textSize,
                normal = new GUIStyleState() { textColor = normalTextColor },
                hover = new GUIStyleState() { textColor = hoverTextColor }
            };
            Vector2 size = style.CalcSize(new GUIContent(text)) * pixelRatio;
            var alignedPosition =
                ((Vector2)screenPosition +
                size * ((anchor + Vector2.left + Vector2.up) / 2f)) * (Vector2.right + Vector2.down) +
                Vector2.up * view.camera.pixelHeight;
            GUI.Label(new Rect(alignedPosition / pixelRatio, size / pixelRatio), text, style);
            UnityEditor.Handles.EndGUI();
        }
    }
}
