using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEditor.SceneManagement;
using System;

[Serializable]
public class PhotoManager : EditorWindow
{
    #region Public Members
    public string _photoFolder = "ScreenGallery";

    public string m_photoFolder
    {
        get { return _photoFolder; }
        set
        {
            _photoFolder = value;
            ParsingJSON.Push("test.json", this);
        }
    }

    private Dictionary<string, List<Transform>> _snapPositions = new Dictionary<string, List<Transform>>();

    public Dictionary<string, List<Transform>> m_snapPositions
    {
        get { return _snapPositions; }
        set
        {
            _snapPositions = value;
            ParsingJSON.Push("test.json", this);
        }
    }

    private Camera _camera;

    public Camera camera
    {
        get { return _camera; }
        set
        {
            _camera = value;
            ParsingJSON.Push("test.json", this);
        }
    }
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
        ParsingJSON.Pull("test.json", this);
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
            DebugLogCamera(SaveCurrentPosition(camera));
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
                GameObject.DestroyImmediate(obj);
            }
        }

        if (GUILayout.Button("Reset Dict"))
        {
            m_snapPositions.Clear();
        }

        if (GUILayout.Button("Print Dict"))
        {
            DebugLogCamera(m_snapPositions);
        }
    }

    #endregion
    #region Class Methods


    public Camera GetCameraScene()
    {
        return SceneView.lastActiveSceneView.camera;
    }

    public KeyValuePair<string, Transform> SaveCurrentPosition(Camera _camera)
    {
        string currentScene = SceneManager.GetActiveScene().path;

        List<Transform> positions;

        if (!m_snapPositions.TryGetValue(currentScene, out positions))
        {
            positions = new List<Transform>();
            m_snapPositions.Add(currentScene, positions);
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