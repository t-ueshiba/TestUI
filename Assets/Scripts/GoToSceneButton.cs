using UnityEngine;

public class GoToSceneButton : MonoBehaviour
{
    public void OnClick()
    {
        Debug.Log("*** Switch to " + name);
        UIManager.Instance.GoToScene(name);
        UIManager.Instance.SetMessage(name, "Hello");
    }
}
