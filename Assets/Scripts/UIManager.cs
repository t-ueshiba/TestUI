using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class UIManager : SingletonMonoBehaviour<UIManager>
{
    private List<string> _scene_names = new List<string>() { "Kitting", "Toyota1" };
    private string _current_scene_name = null;

    protected override void Awake()
    {
        foreach (var scene_name in _scene_names)
            if (!SceneManager.GetSceneByName(scene_name).IsValid())
            {
                SceneManager.LoadScene(scene_name, LoadSceneMode.Additive);
                Debug.Log("*** " + scene_name + " loaded");

            }
    }

    // Start is called before the first frame update
    private void Start()
    {
        foreach (var scene_name in _scene_names)
            SetSceneVisibility(scene_name, scene_name == _current_scene_name);
    }

    public void SetMessage(string scene_name, string message="")
    {
        foreach (var text in GetComponentsInChildren<TMP_Text>())
            if (text.transform.parent.name == scene_name)
            {
                text.text = message;
                return;
            }
        Debug.Log("*** No component found with name[" + scene_name + "]");
    }

    public void GoToScene(string scene_name)
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
        
        SetSceneVisibility(scene_name, true);

        foreach (var button in GetComponentsInChildren<Button>())
            button.enabled = (button.name != scene_name);

        _current_scene_name = scene_name;
    }

    private void SetSceneVisibility(string scene_name, bool visibility)
    {
        var root_objects = SceneManager.GetSceneByName(scene_name).GetRootGameObjects();
        foreach (var root_object in root_objects)
            root_object.SetActive(visibility);
    }
}
