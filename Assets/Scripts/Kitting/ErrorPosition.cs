using UnityEngine;
using NepSensorMsgs;
using NepGeometryMsgs;
using Nep;
using Newtonsoft.Json;

public class ErrorPosition : MonoBehaviour
{
    // NEP messages from ROS
    public class NepPoseStamped
    {
        public Header  header { get; set; }
        public NepPose pose   { get; set; }
    }

    public class RequestHelp
    {
        public string         robot_name { get; set; }
        public string         item_id    { get; set; }
        public NepPoseStamped pose       { get; set; }
        public int            request    { get; set; }
        public string         message    { get; set; }
    }

    // Member variables
    private NepCallback _requestHelpCallback;
    private Vector3     _position = new Vector3(0f, -1f, 0f);
    private Quaternion  _rotation = new Quaternion(0f, 0f, 0f, 1f);
    public static int   request;

    // Member functions
    private void Start()
    {
        var scene_name = gameObject.scene.name;
        _requestHelpCallback = NepGlobal.Node.NewCallback(scene_name + "/request_help", RequestHelpCallback, NepGlobal.IP);
        _requestHelpCallback.Start();

        transform.position = _position;
        transform.rotation = _rotation;
    }

    private void Update()
    {
        transform.position = _position;
        transform.rotation = _rotation;
       // Debug.Log("*** ErrorPosition: position=" + transform.position + ", rotation=" + transform.rotation);
    }

    private void RequestHelpCallback(string message)
    {
        var request_help = JsonConvert.DeserializeObject<RequestHelp>(message);
        //if (request_help.request != 0)
        //    Debug.Log("*** RequestHelpCallback(): " + message);

        _position = NepGlobal.NepToUnity(request_help.pose.pose.position);
        _rotation = NepGlobal.NepToUnity(request_help.pose.pose.orientation);

        var scene_name = gameObject.scene.name;
        UIManager.Instance.SetMessage(scene_name, request_help.message);

        request = request_help.request;
    }
}
