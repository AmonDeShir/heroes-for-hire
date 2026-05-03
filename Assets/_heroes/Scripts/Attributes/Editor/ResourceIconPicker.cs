using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Linq;
using System.Collections.Generic;
using UnityEditor.UIElements;
using System.Collections;

public class ResourceIconPicker : EditorWindow
{
    private static SerializedProperty _targetProperty;
    private static string _resourceFolder;

    private List<string> _iconPaths = new();
    private ScrollView _scrollView;
    private ToolbarSearchField _searchField;
    
    private IEnumerator _loaderRoutine;

    public static void Show(string folder, SerializedProperty property)
    {
        _targetProperty = property;
        _resourceFolder = folder;

        var window = GetWindow<ResourceIconPicker>(true, "Select Icon");
        window.minSize = new Vector2(400, 500);
        window.LoadIcons();
        window.RefreshList("");
        window.Show();
    }

    private void OnEnable()
    {
        EditorApplication.update += UpdateLoader;
    }

    private void OnDisable()
    {
        EditorApplication.update -= UpdateLoader;
    }

    private void UpdateLoader()
    {
        if (_loaderRoutine != null && _loaderRoutine.MoveNext() == false)
        {
            _loaderRoutine = null;
        }
    }

    public void CreateGUI()
    {
        var toolbar = new Toolbar();
        toolbar.style.height = 30;

        _searchField = new ToolbarSearchField();
        _searchField.RegisterValueChangedCallback(evt => RefreshList(evt.newValue));
        _searchField.style.flexGrow = 1;
        
        toolbar.Add(_searchField);
        rootVisualElement.Add(toolbar);

        _scrollView = new ScrollView();
        _scrollView.contentContainer.style.flexDirection = FlexDirection.Row;
        _scrollView.contentContainer.style.flexWrap = Wrap.Wrap;
        _scrollView.contentContainer.style.paddingTop = 10;
        
        rootVisualElement.Add(_scrollView);
    }

    private void LoadIcons()
    {
        var folderPath = $"Assets/Resources/{_resourceFolder}";
        if (!AssetDatabase.IsValidFolder(folderPath)) return;

        var guids = AssetDatabase.FindAssets("t:Texture2D t:VectorImage", new[] { folderPath });
        
        _iconPaths = guids
            .Select(AssetDatabase.GUIDToAssetPath)
            .Where(path => path.Contains($"/Resources/{_resourceFolder}/"))
            .Select(path => {
                var idx = path.IndexOf("Resources/", StringComparison.Ordinal);
                var cleanPath = path.Substring(idx + 10);
                return System.IO.Path.ChangeExtension(cleanPath, null);
            })
            .Distinct()
            .ToList();
    }

    private void RefreshList(string filter)
    {
        _loaderRoutine = null;
        _scrollView.Clear();

        var filtered = string.IsNullOrWhiteSpace(filter) 
            ? _iconPaths
            : _iconPaths.Where(p => p.ToLower().Contains(filter.ToLower())).ToList();
        
        _loaderRoutine = LoadIconsStepByStep(filtered);
    }

    private IEnumerator LoadIconsStepByStep(List<string> paths)
    {
        const int itemsPerFrame = 10;
        int count = 0;

        foreach (var path in paths)
        {
            var btn = CreateIconButton(path);
            if (btn != null)
            {
                _scrollView.Add(btn);
            }

            count++;
            
            if (count >= itemsPerFrame)
            {
                count = 0;
                yield return null; 
            }
        }
    }

    private VisualElement CreateIconButton(string path)
    {
        var container = new Button(() => SelectIcon(path))
        {
            tooltip = path,
            style = {
                width = 70, height = 70, marginRight = 4, marginBottom = 4,
                backgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.1f),
            }
        };

        var iconPreview = new VisualElement { style = { flexGrow = 1 } };
        
        var vector = Resources.Load<VectorImage>(path);
        
        if (vector != null)
        {
            iconPreview.style.backgroundImage = new StyleBackground(vector);
        }
        else 
        {
            var tex = Resources.Load<Texture2D>(path);
            if (tex != null) iconPreview.style.backgroundImage = new StyleBackground(tex);
            else return null;
        }

        container.Add(iconPreview);
        return container;
    }

    private void SelectIcon(string path)
    {
        _targetProperty.stringValue = path;
        _targetProperty.serializedObject.ApplyModifiedProperties();
        Close();
    }
}