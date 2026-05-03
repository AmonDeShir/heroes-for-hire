using UnityEngine;

public class ResourceIconAttribute : PropertyAttribute
{
    public string Folder;

    public ResourceIconAttribute(string folder)
    {
        Folder = folder;
    }
}