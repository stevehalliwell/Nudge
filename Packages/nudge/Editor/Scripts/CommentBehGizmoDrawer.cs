using UnityEditor;
using UnityEngine;

namespace AID.Editor
{
    public static class CommentBehGizmoDrawer
    {
        [DrawGizmo(GizmoType.Selected | GizmoType.NonSelected, typeof(CommentBeh))]
        public static void DrawGizmoForCommentBeh(CommentBeh commentBeh, GizmoType gizmoType)
        {
            var nudgeSettings = NudgeSettings.GetOrCreateSettings();

            if (!nudgeSettings.showHidden && commentBeh.comment.Hidden && (gizmoType & GizmoType.Selected) == 0)
                return;

            Gizmos.DrawIcon(
                commentBeh.transform.position,
                commentBeh.comment.IsTask ? nudgeSettings.commentTaskGizmoPath : nudgeSettings.sceneCommentGizmoPath,
                true);

            if (commentBeh.comment.LinkedObject != null)
            {
                Transform linkedTransform = null;
                if (commentBeh.comment.LinkedObject is GameObject)
                {
                    linkedTransform = (commentBeh.comment.LinkedObject as GameObject).transform;
                }
                else if (commentBeh.comment.LinkedObject is Component)
                {
                    linkedTransform = (commentBeh.comment.LinkedObject as Component).transform;
                }

                if (linkedTransform != null &&
                    ((gizmoType & GizmoType.Selected) != 0 || NudgeSettings.GetOrCreateSettings().drawLinkedConnection))
                {
                    Gizmos.DrawIcon(
                        linkedTransform.position,
                        commentBeh.comment.IsTask ? nudgeSettings.commentTaskLinkedGizmoPath : nudgeSettings.commentLinkedGizmoPath,
                        true);

                    var prevCol = Gizmos.color;
                    Gizmos.color = prevCol * nudgeSettings.linkedTint;
                    Gizmos.DrawLine(commentBeh.transform.position, linkedTransform.position);
                    Gizmos.color = prevCol;
                }
            }
        }
    }
}