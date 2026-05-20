using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(ResourceIconAttribute))]
public class ResourceIconDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var root = new VisualElement { style = { marginBottom = 6 } };
        var attr = (ResourceIconAttribute)attribute;

        var textField = new TextField(property.displayName);

        textField.BindProperty(property); 
        textField.AddToClassList(TextField.alignedFieldUssClassName);
        textField.style.flexGrow = 1;

        var preview = new VisualElement
        {
            style = { width = 40, height = 40, marginLeft = 10 }
        };

        textField.RegisterValueChangedCallback(evt =>
        {
            UpdatePreview(preview, evt.newValue);
        });

        var bottomRow = new VisualElement
        {
            style = { flexDirection = FlexDirection.Row, justifyContent = Justify.FlexEnd, marginTop = 2 }
        };

        var button = new Button(() => ResourceIconPicker.Show(attr.Folders, property))
        {
            text = "Select"
        };
        
        button.style.height = StyleKeyword.Auto;
        button.style.alignSelf = Align.Center;

        UpdatePreview(preview, property.stringValue);

        root.Add(textField);
        bottomRow.Add(button);
        bottomRow.Add(preview);
        root.Add(bottomRow);

        return root;
    }

    private void UpdatePreview(VisualElement element, string path)
    {
        element.style.backgroundImage = null;

        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        var vector = Resources.Load<VectorImage>(path);
        
        if (vector != null) {
            element.style.backgroundImage = new StyleBackground(vector);
            
            return;
        }

        var tex = Resources.Load<Texture2D>(path);
        
        if (tex != null)
        {
            element.style.backgroundImage = new StyleBackground(tex);
        }
    }
}

