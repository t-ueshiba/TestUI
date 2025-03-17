using UnityEngine;
using ROSNEPConnector;

public class GoToSceneButton : MonoBehaviour
{
    public void OnClick(string scene_name)
    {
        Debug.Log("*** Switch to " + scene_name);
        UIManager.GoToScene(scene_name);
        UIManager.SetMessage(scene_name, "Hello");
    }
}
