using UnityEngine;

public class ResourceIconAttribute : PropertyAttribute
{
    public string[] Folders;

    public ResourceIconAttribute(params string[] folders)
    {
        Folders = folders;
    }
}

