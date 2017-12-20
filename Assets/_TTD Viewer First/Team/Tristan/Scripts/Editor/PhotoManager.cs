using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEditor.SceneManagement;
using System;

public class PhotoManager : EditorWindow
{

    #region Public Members
    static public string m_photoFolder = "ScreenGallery";
    //public Path photopath = Path.("ScreenGallery");
    static public Dictionary<string, List<Transform>> m_snapPositions = new Dictionary<string, List<Transform>>();
    static public Camera camera;
    #endregion
    #region Public Void
    // Add menu item named "ScreenShotWindow" to the Window menu
    [MenuItem("Window/ScreenshotWindow")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(PhotoManager));
    }

    #endregion
    #region System

    private void OnEnable()
    {
        original = GameObject.FindWithTag("MainCamera");
    }

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
        m_photoFolder = EditorGUILayout.TextField("Path", m_photoFolder);
        if (GUILayout.Button("Screenshot"))
        {
            // Create Photo Folder
            Directory.CreateDirectory(m_photoFolder);

            m_cam = Camera.Instantiate(original.GetComponent<Camera>());
            Transform m_camTransform = m_cam.GetComponent<Transform>();
            Debug.Log(m_snapPositions);
            foreach (KeyValuePair<string, List<Transform>> item in m_snapPositions)
            {
                Debug.Log("--");
                Debug.Log(item.Key);
                Debug.Log(item.Value);
                Debug.Log(item.Value.Count);
                Debug.Log("--");

                
                string path = Path.GetFileName(m_photoFolder + "/" + item.Key);
                
                string sceneName = Path.GetFileName(item.Key);
                Debug.Log(sceneName);
                Directory.CreateDirectory(m_photoFolder + "/" + sceneName);

                //EditorSceneManager.OpenScene(item.Key);
                int index = 0;
                foreach (Transform transform in item.Value)
                {
                    Debug.Log(transform);
                    m_camTransform = transform;
                    TakeScreenshot(m_cam, m_photoFolder + "/" + sceneName +  "/shoot" + index + ".png");
                }
            }
            Camera.DestroyImmediate(m_cam);
        }
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

    public void TakeScreenshot(Camera _camera, string _filepath)
    {
        RenderTexture rt = new RenderTexture(_camera.pixelWidth, _camera.pixelHeight, 24);
        _camera.targetTexture = rt;

        Texture2D screenShot = new Texture2D(_camera.pixelWidth, _camera.pixelHeight, TextureFormat.RGB24, false);
        _camera.Render();

        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, _camera.pixelWidth, _camera.pixelHeight), 0, 0);

        _camera.targetTexture = null;

        RenderTexture.active = null; // JC: added to avoid errors
        DestroyImmediate(rt);

        byte[] bytes = screenShot.EncodeToPNG();

        System.IO.File.WriteAllBytes(_filepath, bytes);
    }

    #endregion
    #region Private and Protected Members
    private Camera m_cam;
    private GameObject original;
    private string m_path;
    #endregion
}