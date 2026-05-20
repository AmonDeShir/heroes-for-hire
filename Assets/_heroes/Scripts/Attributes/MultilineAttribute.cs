using UnityEngine;

public class MultilineAttribute : PropertyAttribute
{
    public int lines;
    
    public MultilineAttribute(int lines = 2)
    {
        this.lines = lines;
    }
}


