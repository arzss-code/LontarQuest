using System;

[Serializable]
public class ObjectiveData
{
    public ObjectiveType type;

    public string description;

    public int target;

    [NonSerialized]
    public int current;
}