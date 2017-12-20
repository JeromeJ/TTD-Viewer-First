using System.IO;
using UnityEngine;

public class ParsingJSON 
{
    public static void Pull(string _path, System.Object _data)
    {
        JsonUtility.FromJsonOverwrite(File.ReadAllText(_path), _data);
    }

    public static void Push(string _path, System.Object _data)
    {
        File.WriteAllText(_path, JsonUtility.ToJson(_data));
    }
}