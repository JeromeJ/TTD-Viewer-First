using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

class CameraSaver : EditorWindow
{
    // Use static so it persists when closing the window
    // (Doesn't persist on Unity restart)
    static public Dictionary<string, List<Transform>> snapPositions = new Dictionary<string, List<Transform>>();
    static public Camera camera;

    [MenuItem("Tools/Capture positions")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(CameraSaver));
    }

    public Camera GetCameraScene()
    {
        return SceneView.lastActiveSceneView.camera;
    }

    public KeyValuePair<string, Transform> SaveCurrentPosition(Camera _camera)
    {
        string currentScene = SceneManager.GetActiveScene().path;

        List<Transform> positions;

        if (!snapPositions.TryGetValue(currentScene, out positions))
        {
            positions = new List<Transform>();
            snapPositions.Add(currentScene, positions);
        }

        positions.Add(_camera.transform);

        return new KeyValuePair<string, Transform>(currentScene, _camera.transform);
    }

    public void DebugLogCamera(Dictionary<string, List<Transform>> _positions)
    {
        foreach (KeyValuePair<string, List<Transform>> pos in _positions)
        {
            for (int i = 0; i < pos.Value.Count; i++)
            {
                DebugLogCamera(pos.Key, pos.Value[i]);
            }
        }
    }

    public void DebugLogCamera(KeyValuePair<string, Transform> _pos)
    {
        DebugLogCamera(_pos.Key, _pos.Value);
    }

    public void DebugLogCamera(string _scene, Transform _position)
    {
        Debug.Log(_scene + " " + _position.position);
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();

        camera = EditorGUILayout.ObjectField("Camera", camera, typeof(Camera), true) as Camera;

        if (GUILayout.Button("Get scene camera"))
            camera = GetCameraScene();

        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Save this position") && camera != null)
        {
            DebugLogCamera(SaveCurrentPosition(camera));
        }
    }

    private void Reset()
    {

    }
}