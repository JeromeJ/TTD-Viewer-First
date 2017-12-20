using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class ScreenShotWindow : EditorWindow
{

    #region Public Members
    public string m_photoFolder;
    static public Dictionary<string, List<Transform>> m_snapPositions = new Dictionary<string, List<Transform>>();
    static public Camera camera;
    #endregion
    #region Public Void
    // Add menu item named "ScreenShotWindow" to the Window menu
    [MenuItem("Window/ScreenshotWindow")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(ScreenShotWindow));
    }

    #endregion
    #region System
    #endregion
    #region Debug Tools & Utility
    #endregion
    #region GUI
    void OnGUI()
    {
        //Jerome part
        EditorGUILayout.BeginHorizontal();

        camera = EditorGUILayout.ObjectField("Camera", camera, typeof(Camera), true) as Camera;

        if (GUILayout.Button("Get scene camera"))
            camera = GetCameraScene();

        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Save this position") && camera != null)
        {
            SaveCurrentPosition(camera);

            ListCameras();
        }
        //Jerome end


        // Remy part
        //if (GUILayout.Button("Screenshot"))
        //{
        //    // Create Photo Folder
        //    Directory.CreateDirectory(m_photoFolder);

        //    m_cam = Camera.Instantiate(original.GetComponent<Camera>(), new Vector3(0, 0, 0), Quaternion.FromToRotation(new Vector3(0, 0, 0), new Vector3(0, 0, 1)));
        //    Transform m_camTransform = m_cam.GetComponent<Transform>();
        //    foreach (KeyValuePair<string, List<Transform>> item in m_snapPositions)
        //    {
        //        string path = m_photoFolder + "/" + item.Key;
        //        Directory.CreateDirectory(path);
        //        //if (Scene.path != item.Key)
        //        //{

        //        SceneManager.LoadScene(item.Key);
        //        foreach (Transform transform in item.Value)
        //        {
        //            m_camTransform = transform;
        //            ScreenCapture.CaptureScreenshot("test.png");
        //            //Debug.Log("T(coucou): oucou ");
        //        }


        //        //}

        //    }
        //}
    // Remy end

    }
    #endregion
    #region Class Methods


    public Camera GetCameraScene()
    {
        return SceneView.lastActiveSceneView.camera;
    }

    public void SaveCurrentPosition(Camera _camera)
    {
        string currentScene = SceneManager.GetActiveScene().path;

        List<Transform> positions;

        if (!m_snapPositions.TryGetValue(currentScene, out positions))
        {
            positions = new List<Transform>();
            m_snapPositions.Add(currentScene, positions);
        }

        positions.Add(_camera.transform);
    }

    public void ListCameras()
    {
        foreach (KeyValuePair<string, List<Transform>> pos in m_snapPositions)
        {
            for (int i = 0; i < pos.Value.Count; i++)
            {
                Debug.Log(pos.Key + " " + pos.Value[i].position);
            }
        }
    }
    #endregion
    #region Private and Protected Members
    private Camera m_cam;
    private GameObject original = GameObject.FindWithTag("MainCamera");
    private string m_path;
    #endregion
}