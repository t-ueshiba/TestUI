using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MySceneManager : MonoBehaviour
{
    private static List<string> _scene_names = new List<string>() { "Kitting", "Assembly" };
    private static string _current_scene_name = null;

    void Awake()
    {
        foreach (var scene_name in _scene_names)
            if (!SceneManager.GetSceneByName(scene_name).IsValid())
            {
                SceneManager.LoadScene(scene_name, LoadSceneMode.Additive);
                Debug.Log("*** " + scene_name + " loaded");
            }
    }

    // Start is called before the first frame update
    void Start()
    {
            foreach (var scene_name in _scene_names)
            SetSceneVisibility(scene_name, scene_name == _current_scene_name);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void SwitchScene(string scene_name)
    {
        if (!_scene_names.Contains(scene_name))
        {
            Debug.Log("Scene[" + scene_name + "] not found!");
            return;
        }

        if (scene_name == _current_scene_name)
            return;

        if (_current_scene_name != null)
            SetSceneVisibility(_current_scene_name, false);
        
        _current_scene_name = scene_name;
        SetSceneVisibility(_current_scene_name, true);
    }

    private static void SetSceneVisibility(string scene_name, bool enable)
    {
        var root_objects = SceneManager.GetSceneByName(scene_name).GetRootGameObjects();
        foreach (var root_object in root_objects)
            root_object.SetActive(enable);
    }

    public void SetMessage(string scene_name, string message)
    {
        var text = transform.parent.Find(scene_name + "Message");
        Debug.Log("*** name=" + text.name);
    }

    public void OnClick(string scene_name)
    {
        Debug.Log("*** Switch to " + scene_name);
        SwitchScene(scene_name);
        SetMessage(scene_name, "Hello");
    }
}
