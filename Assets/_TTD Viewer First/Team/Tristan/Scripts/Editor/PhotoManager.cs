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
        // original = GameObject.FindWithTag("MainCamera");
    }

    #endregion
    #region Debug Tools & Utility
    #endregion
    #region GUI
    void OnGUI()
    {
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

        m_photoFolder = EditorGUILayout.TextField("Path", m_photoFolder);

        if (GUILayout.Button("Screenshot"))
        {
            // Create Photo Folder
            Directory.CreateDirectory(m_photoFolder);

            foreach (KeyValuePair<string, List<Transform>> item in m_snapPositions)
            {
                // Create directory for current Scene
                string sceneName = Path.GetFileName(item.Key);
                string relScenePath = m_photoFolder + "/" + sceneName;
                Directory.CreateDirectory(relScenePath);
                EditorSceneManager.OpenScene(item.Key);
                //Create Camera
                GameObject obj = new GameObject();
                Camera cam = obj.AddComponent(typeof(Camera)) as Camera;
                // Loop between each position
                int index = 0;
                foreach (Transform t in item.Value)
                {
                    // Set new camera position & Take screenshot
                    Debug.Log(t.position);

                    cam.transform.position = t.position;
                    cam.transform.rotation = t.rotation;
                    cam.transform.localScale = t.localScale;
                    TakeScreenshot(cam, relScenePath + "/shoot" + index + ".png");
                    index++;
                }
            }
            //Camera.DestroyImmediate(m_cam);
        }
        if (GUILayout.Button("Reset Dict"))
        {
            m_snapPositions.Clear();
        }
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

    //
    // TODO: Demander à Eloi s'il s'agit d'un snippet.
    //

    /// <summary>
    /// Take a screenshot using the given camera and save it to the given path.
    /// </summary>
    /// <param name="_camera">The camera to take screenshot from.</param>
    /// <param name="_filepath">The location to save the screenshot to.</param>
    public void TakeScreenshot(Camera _camera, string _filepath)
    {
        int width = _camera.pixelWidth;
        int height = _camera.pixelHeight;

        Texture2D screenShot = new Texture2D(width, height, TextureFormat.RGB24, false);
        RenderTexture rt = new RenderTexture(width, height, 24);

        // Camera will render its image in rt
        _camera.targetTexture = rt;

            _camera.Render();

            RenderTexture.active = rt;

                // Save the image displayed on the active RenderTexture
                screenShot.ReadPixels(new Rect(0, 0, width, height), 0, 0);

            // Added to avoid errors
            RenderTexture.active = null;

        _camera.targetTexture = null;

        DestroyImmediate(rt);

        byte[] bytes = screenShot.EncodeToPNG();

        File.WriteAllBytes(_filepath, bytes);
    }

    #endregion
    #region Private and Protected Members
    //private Camera m_cam;
    private GameObject original;
    private string m_path;
    #endregion
}