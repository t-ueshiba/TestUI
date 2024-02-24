using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MySceneManager : MonoBehaviour
{
    private static List<string> _scene_names = new List<string> {"PickingScene", "AssemblyScene"};

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
        {
            var root_objects = SceneManager.GetSceneByName(scene_name).GetRootGameObjects();
            foreach (var root_object in root_objects)
                root_object.SetActive(false);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
