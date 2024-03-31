using UnityEngine;

public class ErrorAllert : MonoBehaviour
{
    // Define the position and rotation values for the error mark.
    private static Vector3 _position_hidden = new Vector3(0f, -1f, 0f);
    private static Vector3 _position_error  = new Vector3(0f, 1f, 0f);
    
    // Start is called before the first frame update
    void Start()
    {
        transform.position = _position_hidden;
        transform.rotation = new Quaternion(0f, 0f, 0f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = (ErrorPosition.request == 1 ? _position_error : _position_hidden);
    }
}
