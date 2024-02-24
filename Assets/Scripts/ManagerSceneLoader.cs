using UnityEngine;
using UnityEngine.SceneManagement;

public class ManagerSceneLoader : MonoBehaviour
{
    private static bool _loaded { get; set; }

    void Awake()
    {
        if (_loaded)
            return;

        _loaded = true;
        SceneManager.LoadScene("ManagerScene", LoadSceneMode.Additive);
        Debug.Log("*** ManagerScene loaded");
    }
}

