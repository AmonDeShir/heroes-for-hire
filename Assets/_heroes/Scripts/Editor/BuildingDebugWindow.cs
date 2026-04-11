using System.Collections.Generic;
using Heroes.Game.Buildings;
using Registry;
using UnityEditor;
using UnityEngine;

namespace Heroes.Editor
{
    public class BuildingDebugWindow : EditorWindow
    {
        private Vector2 _scroll;
        private bool _autoRefresh = true;

        [MenuItem("Heroes/Debug/Building Viewer")]
        private static void Open()
        {
            GetWindow<BuildingDebugWindow>("Buildings");
        }

        private void OnEnable()
        {
            EditorApplication.update += HandleEditorUpdate;
        }

        private void OnDisable()
        {
            EditorApplication.update -= HandleEditorUpdate;
        }

        private void HandleEditorUpdate()
        {
            if (_autoRefresh)
            {
                Repaint();
            }
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            _autoRefresh = EditorGUILayout.Toggle("Auto Refresh", _autoRefresh);
            if (GUILayout.Button("Refresh", GUILayout.Width(80)))
            {
                Repaint();
            }
            EditorGUILayout.EndHorizontal();

            var buildings = Registry<BuildingFacade>.All();
            var list = new List<BuildingFacade>(buildings);

            EditorGUILayout.Space(6);
            EditorGUILayout.LabelField("Active Buildings", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Count", list.Count.ToString());

            _scroll = EditorGUILayout.BeginScrollView(_scroll);

            foreach (var building in list)
            {
                DrawBuilding(building);
            }

            EditorGUILayout.EndScrollView();
        }

        private static void DrawBuilding(BuildingFacade building)
        {
            if (building == null || building.Model == null || building.Definition == null)
            {
                EditorGUILayout.LabelField("<null>");
                return;
            }

            var model = building.Model;
            var health = model.Health;

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Name", building.Definition.DisplayName);
            EditorGUILayout.LabelField("Id", model.InstanceId);
            EditorGUILayout.LabelField("State", model.State.ToString());
            EditorGUILayout.LabelField("Stage", model.ConstructionStage.ToString());
            EditorGUILayout.LabelField("HP", $"{health.Current:0}/{health.Max:0}");
            EditorGUILayout.EndVertical();
        }
    }
}
