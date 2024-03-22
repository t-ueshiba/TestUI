using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManagerSceneLoader : MonoBehaviour
{
    private static bool _loaded { get; set; }

    void Awake()
    {
        if (_loaded)
            return;

        _loaded = true;
        SceneManager.LoadScene("UIManager", LoadSceneMode.Additive);
        Debug.Log("*** UIManager scene loaded");
        
    }

    private void Start()
    {
        UIManager.Instance.GoToScene(gameObject.scene.name);
    }
}

