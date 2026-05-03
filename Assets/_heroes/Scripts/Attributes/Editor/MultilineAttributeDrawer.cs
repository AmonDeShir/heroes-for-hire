using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(MultilineAttribute))]
public class MultilineAttributeDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var field = new TextField(property.displayName);
        var attr = (MultilineAttribute)attribute;
        
        field.BindProperty(property);
        
        field.AddToClassList(TextField.alignedFieldUssClassName);

        field.multiline = true;
        field.style.height = 18 * attr.lines;
        field.style.flexGrow = 1;

        return field;
    }
}