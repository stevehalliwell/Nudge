using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AID.Editor
{
    public static class CommentBehGizmoDrawer
    {
        [DrawGizmo(GizmoType.Selected | GizmoType.NonSelected, typeof(CommentBeh))]
        public static void DrawGizmoForMyScript(CommentBeh commentBeh, GizmoType gizmoType)
        {
            var nudgeSettings = NudgeSettings.GetOrCreateSettings();

            if (!nudgeSettings.showHidden && commentBeh.comment.hidden && (gizmoType & GizmoType.Selected ) == 0)
                return;
            
            Gizmos.DrawIcon(
                commentBeh.transform.position,
                commentBeh.comment.isTask ? nudgeSettings.commentTaskGizmoPath : nudgeSettings.sceneCommentGizmoPath, 
                true);

            if (commentBeh.comment.linkedObject != null)
            {
                Transform linkedTransform = null;
                if (commentBeh.comment.linkedObject is GameObject)
                {
                    linkedTransform = (commentBeh.comment.linkedObject as GameObject).transform;
                }
                else if (commentBeh.comment.linkedObject is Component)
                {
                    linkedTransform = (commentBeh.comment.linkedObject as Component).transform;
                }

                if (linkedTransform != null && 
                    ( (gizmoType & GizmoType.Selected) != 0 || NudgeSettings.GetOrCreateSettings().drawLinkedConnection) )
                {
                    Gizmos.DrawIcon(
                        linkedTransform.position,
                        commentBeh.comment.isTask ? nudgeSettings.commentTaskLinkedGizmoPath : nudgeSettings.commentLinkedGizmoPath, 
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