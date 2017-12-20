using System.IO;
using UnityEngine;

public class ParsingJSON 
{

    

    public static bool Pull(string _path, System.Object _data)
    {
        bool _check;
        if (File.Exists("test.json"))
        {
            JsonUtility.FromJsonOverwrite(File.ReadAllText(_path), _data);
           _check = true;
        }
        else
        {
            _check = false;
        }
        return _check;
    }

    public static void Push(string _path, System.Object _data)
    {
        File.WriteAllText(_path, JsonUtility.ToJson(_data));
    }
}