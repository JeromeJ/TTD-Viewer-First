using System.IO;
using UnityEngine;

public class ParsingJSON 
{

    public bool _check;

    public static bool Pull(string _path, System.Object _data)
    {
        if (File.Exists("test.json"))
        {
            JsonUtility.FromJsonOverwrite(File.ReadAllText(_path), _data);
           _check = true;
        }
        else
        {

        }
    }

    public static void Push(string _path, System.Object _data)
    {
        File.WriteAllText(_path, JsonUtility.ToJson(_data));
    }
}