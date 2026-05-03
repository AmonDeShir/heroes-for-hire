using System;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[CustomPropertyDrawer(typeof(GUIDAttribute))]
public class GUIDAttributeDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var container = new VisualElement();
        
        container.style.flexDirection = FlexDirection.Row;

        if (property.propertyType != SerializedPropertyType.String)
        {
            return new Label("Use [GUIDAttribute] with string.");
        }

        var textField = new TextField(property.displayName);
        textField.BindProperty(property);
        
        textField.AddToClassList(TextField.alignedFieldUssClassName);
        textField.style.flexGrow = 1;

        var button = new Button(() =>
        {
            property.stringValue = Guid.NewGuid().ToString();
            property.serializedObject.ApplyModifiedProperties();
        });

        button.text = "Random";
        button.style.marginLeft = 4;
        button.style.height = StyleKeyword.Auto;

        container.Add(textField);
        container.Add(button);

        return container;
    }
}