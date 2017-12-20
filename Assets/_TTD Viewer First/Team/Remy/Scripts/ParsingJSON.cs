using System.IO;
using UnityEngine;

public class ParsingJSON 
{
    public static bool Pull(string _path, System.Object _data)
    {
        bool exist = File.Exists("test.json");

        if (exist)
        {
            JsonUtility.FromJsonOverwrite(File.ReadAllText(_path), _data);
        }

        return exist;
    }

    public static void Push(string _path, System.Object _data)
    {
        File.WriteAllText(_path, JsonUtility.ToJson(_data));
    }
}