using UnityEngine;

public class GoToSceneButton : MonoBehaviour
{
    public void OnClick(string scene_name)
    {
        Debug.Log("*** Switch to " + scene_name);
        UIManager.Instance.GoToScene(scene_name);
        UIManager.Instance.SetMessage(scene_name, "Hello");
    }
}
