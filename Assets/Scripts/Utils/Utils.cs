using UnityEngine;

public enum PrefabList
{
    StartingCell
}

public static class Utils
{
    public static T GetPrefabByName<T>(PrefabList prefabItem) where T: class
    {
        string name = prefabItem.ToString();
        return Resources.Load("Prefabs/" + name, typeof(T)) as T;
    }
}
