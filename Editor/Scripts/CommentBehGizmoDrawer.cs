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

            if ((gizmoType & GizmoType.Selected) != 0 || nudgeSettings.drawLinkedConnection)
            {
                AttemptToDrawLine(commentBeh.transform, commentBeh.comment.PrimaryLinkedObject, commentBeh.comment, nudgeSettings);

                foreach (var item in commentBeh.comment.AdditionalLinkedObjects)
                {
                    AttemptToDrawLine(commentBeh.transform, item, commentBeh.comment, nudgeSettings);
                }
            }
        }

        private static void AttemptToDrawLine(Transform from, UnityEngine.Object targetObj, Comment comment, NudgeSettings nudgeSettings)
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
                        comment.IsTask ? nudgeSettings.commentTaskLinkedGizmoPath : nudgeSettings.commentLinkedGizmoPath,
                        true);

                    var prevCol = Gizmos.color;
                    Gizmos.color = prevCol * nudgeSettings.linkedTint;
                    Gizmos.DrawLine(from.position, linkedTransform.position);
                    Gizmos.color = prevCol;
                }
            }
        }
    }
}