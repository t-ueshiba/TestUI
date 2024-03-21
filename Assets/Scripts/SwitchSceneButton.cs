using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchSceneButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick(string scene_name)
    {
        Debug.Log("*** Switch to " + scene_name);
        MySceneManager.SwitchScene(scene_name);
    }
}
