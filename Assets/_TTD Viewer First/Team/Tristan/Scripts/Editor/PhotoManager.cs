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

    public Dictionary<string, List<PseudoTransform>> _snapPositions = new Dictionary<string, List<PseudoTransform>>();

    public Dictionary<string, List<PseudoTransform>> m_snapPositions
    {
        get { return _snapPositions; }
        set
        {
            _snapPositions = value;
            ParsingJSON.Push("test.json", this);
        }
    }

    public Camera _camera;

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

            foreach (KeyValuePair<string, List<PseudoTransform>> item in m_snapPositions)
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
                foreach (PseudoTransform t in item.Value)
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

    public KeyValuePair<string, PseudoTransform> SaveCurrentPosition(Camera _camera)
    {
        string currentScene = SceneManager.GetActiveScene().path;

        List<PseudoTransform> positions;

        if (!m_snapPositions.TryGetValue(currentScene, out positions))
        {
            positions = new List<PseudoTransform>();
            m_snapPositions.Add(currentScene, positions);
        }

            PseudoTransform newPos = new PseudoTransform() {
            position = _camera.transform.position,
            rotation = _camera.transform.rotation,
            localScale = _camera.transform.localScale,

        };

        positions.Add(newPos);

        return new KeyValuePair<string, PseudoTransform>(currentScene, newPos);
    }

    public void DebugLogCamera(Dictionary<string, List<PseudoTransform>> _positions)
    {
        foreach (KeyValuePair<string, List<PseudoTransform>> pos in _positions)
        {
            for (int i = 0; i < pos.Value.Count; i++)
            {
                DebugLogCamera(pos.Key, pos.Value[i]);
            }
        }
    }

    public void DebugLogCamera(KeyValuePair<string, PseudoTransform> _pos)
    {
        DebugLogCamera(_pos.Key, _pos.Value);
    }

    public void DebugLogCamera(string _scene, PseudoTransform _position)
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
    
    #endregion
}

public class PseudoTransform
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 localScale;
}