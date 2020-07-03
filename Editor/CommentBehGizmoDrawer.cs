using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AID.Editor
{
    public static class CommentBehGizmoDrawer
    {
        [DrawGizmo(GizmoType.Active | GizmoType.NonSelected, typeof(CommentBeh))]
        public static void DrawGizmoForMyScript(CommentBeh commentBeh, GizmoType gizmoType)
        {
            Gizmos.DrawIcon(commentBeh.transform.position, "Packages/Nudge/Gizmos/CommentBeh Icon.png", true);
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
                    Gizmos.DrawIcon(linkedTransform.position, "Packages/Nudge/Gizmos/CommentBehLink Icon.png", true);
                    var prevCol = Gizmos.color;
                    Gizmos.color = Color.grey;
                    Gizmos.DrawLine(commentBeh.transform.position, linkedTransform.position);
                    Gizmos.color = prevCol;
                }
            }
        }
    }
}