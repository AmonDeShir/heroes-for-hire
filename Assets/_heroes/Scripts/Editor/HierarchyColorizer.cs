using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public static class HierarchyColorizer
{
    private static readonly Color TextColor = new Color(1f, 0.6f, 0f);

    static HierarchyColorizer()
    {
        EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
    }

    private static void OnHierarchyGUI(int instanceID, Rect selectionRect)
    {
        var obj = EditorUtility.EntityIdToObject(instanceID) as GameObject;
        
        if (obj == null)
        {
            return;
        }

        if (obj.name.StartsWith("---"))
        {
            EditorGUI.DrawRect(selectionRect, EditorGUIUtility.isProSkin ? new Color(0.22f, 0.22f, 0.22f) : new Color(0.76f, 0.76f, 0.76f));

            var style = new GUIStyle(EditorStyles.label);
            
            style.normal.textColor = TextColor;
            style.fontStyle = FontStyle.Bold;

            EditorGUI.LabelField(selectionRect, obj.name, style);
        }
    }
}
