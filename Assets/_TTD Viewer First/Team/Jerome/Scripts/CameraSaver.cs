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

    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();

        camera = EditorGUILayout.ObjectField("Camera", camera, typeof(Camera), true) as Camera;

        if (GUILayout.Button("Get scene camera"))
            camera = SceneView.lastActiveSceneView.camera;

        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Save this position") && camera != null)
        {
            string currentScene = SceneManager.GetActiveScene().path;

            List<Transform> positions;

            if(!snapPositions.TryGetValue(currentScene, out positions))
            {
                positions = new List<Transform>();
                snapPositions.Add(currentScene, positions);
            }

            positions.Add(camera.transform);

            foreach(KeyValuePair<string, List<Transform>> pos in snapPositions)
            {
                for(int i = 0; i < pos.Value.Count; i++)
                {
                    Debug.Log(pos.Key + " " + pos.Value[i].position);
                }
            }
        }
    }
}