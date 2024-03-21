using UnityEngine;
using TMPro;

public class SwitchSceneButton : MonoBehaviour
{
    public void SetMessage(string scene_name, string message)
    {
        var msg = transform.parent.Find(scene_name + "Message").GetComponent<TMP_Text>();
        msg.text = message;
        Debug.Log("*** name=" + msg.name);
    }

    public void OnClick()
    {
        Debug.Log("*** Switch to " + name);
        UIManager.Instance.SwitchScene(name);
        SetMessage(name, "Hello");
    }
}
