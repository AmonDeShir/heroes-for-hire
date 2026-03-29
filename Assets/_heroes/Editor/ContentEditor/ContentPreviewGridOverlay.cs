using UnityEngine;
#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
#endif

namespace Heroes.Editor.ContentEditor
{
    [ExecuteAlways]
    public class ContentPreviewGridOverlay : MonoBehaviour
    {
        [SerializeField] private Camera targetCamera;

        [Header("Line Settings")]
        [SerializeField] private Color lineColor = new(1f, 0.2f, 0.2f, 0.8f);

        [Header("Deadzone Settings")]
        [SerializeField] private Color deadzoneFillColor = new(1f, 0f, 0f, 0.08f);
        [SerializeField] private Vector2 deadzoneSize = new(0.5f, 0.5f);
        [SerializeField] private Vector2 deadzoneOffset = Vector2.zero;

#if UNITY_EDITOR
        private void OnEnable()
        {
            if (targetCamera == null)
            {
                targetCamera = GetComponent<Camera>();
            }
        }

        private static Texture2D lineTexture;

        private void OnGUI()
        {
            if (targetCamera == null || Application.isPlaying)
            {
                return;
            }

            var w = Screen.width;
            var h = Screen.height;

            if (lineTexture == null)
            {
                lineTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                lineTexture.SetPixel(0, 0, Color.white);
                lineTexture.Apply();
            }

            var dzW = deadzoneSize.x * w;
            var dzH = deadzoneSize.y * h;
            var centerX = w / 2 + deadzoneOffset.x * w;
            var centerY = h / 2 + deadzoneOffset.y * h;

            var deadzoneRect = new Rect(centerX - dzW / 2, centerY - dzH / 2, dzW, dzH);

            DrawRect(deadzoneRect, deadzoneFillColor);

            DrawLine(new Vector2(w / 2, 0), new Vector2(w / 2, h), lineColor);
            DrawLine(new Vector2(0, h / 2), new Vector2(w, h / 2), lineColor);

            DrawLine(new Vector2(deadzoneRect.xMin, 0), new Vector2(deadzoneRect.xMin, h), lineColor);
            DrawLine(new Vector2(deadzoneRect.xMax, 0), new Vector2(deadzoneRect.xMax, h), lineColor);
            DrawLine(new Vector2(0, deadzoneRect.yMin), new Vector2(w, deadzoneRect.yMin), lineColor);
            DrawLine(new Vector2(0, deadzoneRect.yMax), new Vector2(w, deadzoneRect.yMax), lineColor);
        }

        private void OnDrawGizmos()
        {
            if (targetCamera == null || Application.isPlaying)
            {
                return;
            }

            Handles.BeginGUI();

            var gameViewSize = GetGameViewSize();
            var w = gameViewSize.x;
            var h = gameViewSize.y;

            var dzW = deadzoneSize.x * w;
            var dzH = deadzoneSize.y * h;
            var centerX = w / 2 + deadzoneOffset.x * w;
            var centerY = h / 2 + deadzoneOffset.y * h;

            var deadzoneRect = new Rect(centerX - dzW / 2, centerY - dzH / 2, dzW, dzH);

            Handles.color = deadzoneFillColor;
            Handles.DrawSolidRectangleWithOutline(deadzoneRect, deadzoneFillColor, Color.clear);

            Handles.color = lineColor;
            Handles.DrawLine(new(w / 2, 0), new(w / 2, h));
            Handles.DrawLine(new(0, h / 2), new(w, h / 2));

            Handles.DrawLine(new(deadzoneRect.xMin, 0), new(deadzoneRect.xMin, h));
            Handles.DrawLine(new(deadzoneRect.xMax, 0), new(deadzoneRect.xMax, h));
            Handles.DrawLine(new(0, deadzoneRect.yMin), new(w, deadzoneRect.yMin));
            Handles.DrawLine(new(0, deadzoneRect.yMax), new(w, deadzoneRect.yMax));

            Handles.EndGUI();
        }

        private Vector2 GetGameViewSize()
        {
            var t = typeof(global::UnityEditor.Editor).Assembly.GetType("UnityEditor.GameView");
            var method = t.GetMethod("GetSizeOfMainGameView", BindingFlags.NonPublic | BindingFlags.Static);
            var res = method.Invoke(null, null);
            return (Vector2)res;
        }

        private static void DrawLine(Vector2 from, Vector2 to, Color color)
        {
            var delta = to - from;
            var angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
            var length = delta.magnitude;

            var matrix = GUI.matrix;
            GUI.color = color;
            GUIUtility.RotateAroundPivot(angle, from);
            GUI.DrawTexture(new Rect(from.x, from.y, length, 1f), lineTexture);
            GUI.matrix = matrix;
            GUI.color = Color.white;
        }

        private static void DrawRect(Rect rect, Color color)
        {
            var prev = GUI.color;
            GUI.color = color;
            GUI.DrawTexture(rect, lineTexture);
            GUI.color = prev;
        }
#endif
    }
}
