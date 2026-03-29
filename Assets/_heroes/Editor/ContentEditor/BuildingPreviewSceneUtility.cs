using System.IO;
using Heroes.Content.Definitions.Buildings;
using Heroes.Tools.BuildingCreator;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Heroes.Editor.ContentEditor
{
    public static class BuildingPreviewSceneUtility
    {
        private const string ScenePath = "Assets/_heroes/Content/Scenes/ContentPreview.unity";

        public static BuildingCreatorPreview GetOrCreatePreview(bool openScene)
        {
            var preview = Object.FindObjectOfType<BuildingCreatorPreview>();
            if (preview != null)
            {
                return preview;
            }

            if (openScene)
            {
                OpenOrCreateScene();
                preview = Object.FindObjectOfType<BuildingCreatorPreview>();
            }

            return preview;
        }

        public static void OpenOrCreateScene()
        {
            if (File.Exists(ScenePath))
            {
                EditorSceneManager.OpenScene(ScenePath);
                return;
            }

            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            var root = new GameObject("ContentPreview");
            var previewComponent = root.AddComponent<BuildingCreatorPreview>();

            var previewContainer = new GameObject("PreviewRoot");
            previewContainer.transform.SetParent(root.transform, false);

            var camera = Camera.main;
            if (camera == null)
            {
                var cameraGo = new GameObject("Preview Camera");
                camera = cameraGo.AddComponent<Camera>();
                camera.transform.position = new Vector3(0.5f, 0.9f, 2f);
                camera.transform.rotation = Quaternion.Euler(16f, -170f, 0f);
            }

            if (camera.GetComponent<ContentPreviewGridOverlay>() == null)
            {
                var overlay = camera.gameObject.AddComponent<ContentPreviewGridOverlay>();
                overlay.GetType();
            }

            var so = new SerializedObject(previewComponent);
            so.FindProperty("captureCamera").objectReferenceValue = camera;
            so.FindProperty("previewRoot").objectReferenceValue = previewContainer.transform;
            so.ApplyModifiedPropertiesWithoutUndo();

            EditorSceneManager.SaveScene(scene, ScenePath);
        }

        public static GameObject SetPreviewFromPrefab(BuildingCreatorPreview preview, GameObject prefab)
        {
            if (preview == null || preview.PreviewRoot == null)
            {
                return null;
            }

            ClearPreview(preview);
            if (prefab == null)
            {
                return null;
            }

            var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, preview.PreviewRoot);
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localRotation = Quaternion.identity;
            instance.transform.localScale = Vector3.one;
            return instance;
        }

        public static GameObject EnsurePreviewObject(BuildingCreatorPreview preview)
        {
            if (preview == null || preview.PreviewRoot == null)
            {
                return null;
            }

            if (preview.PreviewRoot.childCount > 0)
            {
                return preview.PreviewRoot.GetChild(0).gameObject;
            }

            var go = new GameObject("PreviewObject");
            go.transform.SetParent(preview.PreviewRoot, false);
            return go;
        }

        public static void ClearPreview(BuildingCreatorPreview preview)
        {
            if (preview == null || preview.PreviewRoot == null)
            {
                return;
            }

            for (var i = preview.PreviewRoot.childCount - 1; i >= 0; i--)
            {
                Object.DestroyImmediate(preview.PreviewRoot.GetChild(i).gameObject);
            }
        }

        public static GameObject SavePrefabFromPreview(BuildingDefinition definition, BuildingCreatorPreview preview)
        {
            if (definition == null || preview == null || preview.PreviewRoot == null)
            {
                return null;
            }

            if (preview.PreviewRoot.childCount == 0)
            {
                return null;
            }

            var instance = preview.PreviewRoot.GetChild(0).gameObject;
            var folder = "Assets/_heroes/Content/Prefabs/Buildings";
            EnsureFolder(folder);

            var safeName = string.IsNullOrWhiteSpace(definition.Id) ? definition.name : definition.Id;
            var path = AssetDatabase.GenerateUniqueAssetPath($"{folder}/{safeName}.prefab");

            var existingPath = AssetDatabase.GetAssetPath(definition.Prefab);
            if (!string.IsNullOrWhiteSpace(existingPath))
            {
                path = existingPath;
            }

            var prefab = PrefabUtility.SaveAsPrefabAsset(instance, path);
            if (prefab != null)
            {
                var so = new SerializedObject(definition);
                so.FindProperty("prefab").objectReferenceValue = prefab;
                so.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(definition);
            }

            return prefab;
        }


        public static void AlignPreviewCameraToSceneView(BuildingCreatorPreview preview)
        {
            if (preview == null || preview.CaptureCamera == null)
            {
                return;
            }

            var sceneView = UnityEditor.SceneView.lastActiveSceneView;
            if (sceneView == null || sceneView.camera == null)
            {
                return;
            }

            var camera = preview.CaptureCamera;
            var source = sceneView.camera;

            camera.transform.position = source.transform.position;
            camera.transform.rotation = source.transform.rotation;
            camera.orthographic = source.orthographic;
            camera.fieldOfView = source.fieldOfView;
            camera.orthographicSize = source.orthographicSize;
            camera.nearClipPlane = source.nearClipPlane;
            camera.farClipPlane = source.farClipPlane;
        }

        private static void EnsureFolder(string folderPath)
        {
            if (AssetDatabase.IsValidFolder(folderPath))
            {
                return;
            }

            Directory.CreateDirectory(folderPath);
            AssetDatabase.Refresh();
        }
    }
}
