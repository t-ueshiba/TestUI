using UnityEngine;
using Nep;
using NepGeometryMsgs;

public class NepGlobal : SingletonMonoBehaviour<NepGlobal>
{
    public static Vector3 NepToUnity(NepVector3 nep_vector)
    {
        var vector = new Vector3();
        vector.x = (float)nep_vector.x;
        vector.z = (float)nep_vector.y;
        vector.y = (float)nep_vector.z;

        return vector;
    }

    public static Vector3 NepToUnity(NepPoint nep_point)
    {
        var point = new Vector3();
        point.x = (float)nep_point.x;
        point.z = (float)nep_point.y;
        point.y = (float)nep_point.z;

        return point;
    }

    public static Quaternion NepToUnity(NepQuaternion nep_quaternion)
    {
        var quaternion = new Quaternion();
        quaternion.x = (float)nep_quaternion.x;
        quaternion.z = (float)nep_quaternion.y;
        quaternion.y = (float)nep_quaternion.z;
        quaternion.w = -(float)nep_quaternion.w;

        return quaternion;
    }

    public static NepVector3 UnityToNep(Vector3 vector)
    {
        var nep_vector = new NepVector3();
        nep_vector.x = (double)vector.x;
        nep_vector.z = (double)vector.y;
        nep_vector.y = (double)vector.z;

        return nep_vector;
    }

    public static NepPoint UnityToNepPoint(Vector3 point)
    {
        var nep_point = new NepPoint();
        nep_point.x = (double)point.x;
        nep_point.z = (double)point.y;
        nep_point.y = (double)point.z;

        return nep_point;
    }

    public static NepQuaternion UnityToNep(Quaternion quaternion)
    {
        var nep_quaternion = new NepQuaternion();
        nep_quaternion.x = (double)quaternion.x;
        nep_quaternion.z = (double)quaternion.y;
        nep_quaternion.y = (double)quaternion.z;
        nep_quaternion.w = -(double)quaternion.w;

        return nep_quaternion;
    }

    // Member variables
    [SerializeField] string  ip;   // IP address of NEP master
    private NepNode          _node;
    private NepConfiguration _conf;

    // Member functions
    protected override void Awake()
    {
        _node = new NepNode("unity_test");
        _conf = _node.Hybrid(ip);
    }

    private void Update()
    {
        _node.Update();
    }

    protected override void OnDestroy()
    {
        _node.Close();
        _node.Dispose();
    }

    public static string IP { get { return Instance.ip; } }
    public static NepNode Node { get { return Instance._node; } }
    public static NepConfiguration Conf { get { return Instance._conf; } }
}
