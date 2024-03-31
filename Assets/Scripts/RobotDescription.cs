#define MILLIMETERS

using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using NepSensorMsgs;
using NepGeometryMsgs;
using Nep;
using Newtonsoft.Json;

public class RobotDescription : MonoBehaviour
{
    // NEP messages from ROS
    public class NepSolidPrimitive
    {
        public byte type { set; get; }
        public double[] dimensions { set; get; }
    }

    public class NepColorRGBA
    {
        public float r { set; get; }
        public float g { set; get; }
        public float b { set; get; }
        public float a { set; get; }
    }

    public class NepMaterial
    {
        public NepMaterial()
        {
            color = new NepColorRGBA();
        }

        public string name { set; get; }
        public NepColorRGBA color { set; get; }
        public UInt32 texture_height { set; get; }
        public UInt32 texture_width { set; get; }
        public byte[] texture_data { set; get; }
    }

    public class NepTransformStamped
    {
        public NepTransformStamped()
        {
            header = new Header();
            transform = new NepTransform();
            transform.translation = new NepVector3();
            transform.rotation = new NepQuaternion();
        }

        public Header header { set; get; }
        public string child_frame_id { set; get; }
        public NepTransform transform { set; get; }
    }

    public class NepLink
    {
        public NepLink()
        {
            transform = new NepTransformStamped();
            origin = new NepPose();
            primitive = new NepSolidPrimitive();
            material = new NepMaterial();
        }

        public NepTransformStamped transform { set; get; }
        public NepPose origin { set; get; }
        public NepSolidPrimitive primitive { set; get; }
        public byte[] data { set; get; }
        public NepMaterial material { set; get; }
    }

    public class NepTFMessage
    {
        public NepTransformStamped[] transforms { set; get; }
    }

    public class NepGetLinksReq
    {
    }

    public class NepGetLinksRes
    {
        public NepLink[] links { set; get; }
    }

    public class Link
    {
        // member variables
        private GameObject _link;
        private GameObject _element;

        // public member functions
        public Link(Transform parent_transform, NepLink nep_link)
        {
            _link = new GameObject();
            _link.name = nep_link.transform.child_frame_id;
            _link.transform.name = _link.name;
            _link.transform.SetParent(parent_transform);
            _link.transform.localPosition = NepGlobal.NepToUnity(nep_link.transform.transform.translation);
            _link.transform.localRotation = NepGlobal.NepToUnity(nep_link.transform.transform.rotation);

            var nep_scale = new NepVector3();

            switch (nep_link.primitive.type)
            {
                case 0:     // Mesh
                    nep_scale.x = (float)nep_link.primitive.dimensions[0];
                    nep_scale.y = (float)nep_link.primitive.dimensions[1];
                    nep_scale.z = (float)nep_link.primitive.dimensions[2];
                    _element = new GameObject();
                    _element.AddComponent<MeshRenderer>();
                    _element.AddComponent<MeshFilter>();
                    break;
                case 1:     // Box
                    nep_scale.x = (float)nep_link.primitive.dimensions[0];
                    nep_scale.y = (float)nep_link.primitive.dimensions[1];
                    nep_scale.z = (float)nep_link.primitive.dimensions[2];
                    _element = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    break;
                case 2:     // Sphere
                    nep_scale.x = (float)(2.0 * nep_link.primitive.dimensions[0]);
                    nep_scale.y = nep_scale.x;
                    nep_scale.z = nep_scale.x;
                    _element = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    break;
                default:    // Cylinder
                    nep_scale.x = (float)(2.0 * nep_link.primitive.dimensions[0]);
                    nep_scale.y = nep_scale.x;
                    nep_scale.z = (float)(0.5 * nep_link.primitive.dimensions[1]);
                    _element = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    break;
            }

            _element.name = _link.name + "/element";
            _element.transform.name = _element.name;
            _element.transform.SetParent(_link.transform);
            _element.transform.localScale = NepGlobal.NepToUnity(nep_scale);
            _element.transform.localPosition = NepGlobal.NepToUnity(nep_link.origin.position);
            _element.transform.localRotation = NepGlobal.NepToUnity(nep_link.origin.orientation);

            if (nep_link.material.texture_height == 0)
            {
                _element.GetComponent<MeshRenderer>().material.SetColor("_Color",
                                                                        new Color(nep_link.material.color.r,
                                                                                  nep_link.material.color.g,
                                                                                  nep_link.material.color.b,
                                                                                  nep_link.material.color.a));
            }
            else
            {
                // Create texture from incoming texture data
                var texture = new Texture2D((int)nep_link.material.texture_width, (int)nep_link.material.texture_height, TextureFormat.RGB24, true);
                texture.SetPixelData(nep_link.material.texture_data, 0);
                texture.Apply();

                // Create cubemap
                var cubemap_width = Math.Max(4 * ((texture.width - 1) / 4 + 1), 4 * ((texture.height - 1) / 4 + 1));
                Debug.Log("*** cubemap_width=" + cubemap_width);
                var cubemap = new Cubemap(cubemap_width, TextureFormat.RGBA32, false);
                Graphics.ConvertTexture(texture, 0, cubemap, (int)CubemapFace.PositiveX);
                Graphics.ConvertTexture(texture, 0, cubemap, (int)CubemapFace.NegativeX);
                Graphics.ConvertTexture(texture, 0, cubemap, (int)CubemapFace.PositiveY);
                Graphics.ConvertTexture(texture, 0, cubemap, (int)CubemapFace.NegativeY);
                Graphics.ConvertTexture(texture, 0, cubemap, (int)CubemapFace.PositiveZ);
                Graphics.ConvertTexture(texture, 0, cubemap, (int)CubemapFace.NegativeZ);
                cubemap.Apply();

                var shader = Shader.Find("Hidden/CubeCopy");
                var material = new Material(shader);
                if (material == null)
                {
                    Debug.Log("*** Failed to create material!");
                    return;
                }
                material.SetTexture("_MainTex", cubemap);
                _element.GetComponent<MeshRenderer>().material = material;
            }

            if (nep_link.primitive.type == 0)    // If the element type is mesh, create a mesh from raw data.
            {
                var magicstr = Encoding.UTF8.GetString(nep_link.data, 0, 5);
                if (magicstr == "<?xml" || magicstr == "<COLL")
                    _link.transform.root.GetComponent<DAELoader>().CreateMeshFromDAE(_element, nep_link.data);
                else if (magicstr == "solid")
                    _link.transform.root.GetComponent<STLLoader>().CreateMeshFromAsciiSTL(_element, nep_link.data);
                else
                    _link.transform.root.GetComponent<STLLoader>().CreateMeshFromBinarySTL(_element, nep_link.data);
            }
        }

        // private member functions
        private void OnDestroy()
        {
            foreach (var material in _element.GetComponent<MeshRenderer>().materials)
                Destroy(material);
            Destroy(_element.GetComponent<MeshFilter>().mesh);
        }
    }

    // Member variables
    [SerializeField] string model;
    private List<Link> _links;
    private NepPublisher _getLinksReqPub;
    private NepCallback _getLinksResCallback;
    private NepCallback _transformsCallback;

    // Member functions
    private void Start()
    {
        _links = new List<Link>();

        if (model == "")
        {
            var scene_name = gameObject.scene.name;
            _getLinksReqPub = NepGlobal.Node.NewPub(scene_name + "/get_links", "json", NepGlobal.Conf);
            _getLinksResCallback = NepGlobal.Node.NewCallback(scene_name + "/links", GetLinksResCallback, NepGlobal.IP);
            _getLinksResCallback.Start();

            _transformsCallback = NepGlobal.Node.NewCallback(scene_name + "/transforms", TransformsCallback, NepGlobal.IP);
            _transformsCallback.Start();

            // Wait 1.0sec before sending request for links.
            StartCoroutine(DelayMethod(1.0f, () =>
            {
                _getLinksReqPub.Publish(new NepGetLinksReq());
                Debug.Log("Service[" + scene_name + "/get_links] requested");
            }));
        }
        else
        {
            // Only for testing.
            var link = new NepLink();
            link.transform.header.frame_id = "base_link";
            link.transform.child_frame_id = "tmp_link";
            link.transform.transform.translation.x = 0;
            link.transform.transform.translation.y = 0;
            link.transform.transform.translation.z = 0;
            link.transform.transform.rotation.x = 0;
            link.transform.transform.rotation.y = 0;
            link.transform.transform.rotation.z = 0;
            link.transform.transform.rotation.w = 1;

            link.origin.position = new NepPoint();
            link.origin.position.x = 0;
            link.origin.position.y = 0;
            link.origin.position.z = 0;
            link.origin.orientation = new NepQuaternion();
            link.origin.orientation.x = 0;
            link.origin.orientation.y = 0;
            link.origin.orientation.z = 0;
            link.origin.orientation.w = 1;

            link.primitive.type = 0; // 0: Mesh, 1: Box, 2: Sphere, 3: Cylinder
            link.primitive.dimensions = new double[3];
#if MILLIMETERS
            link.primitive.dimensions[0] = 0.001;
            link.primitive.dimensions[1] = 0.001;
            link.primitive.dimensions[2] = 0.001;
#else
            link.primitive.dimensions[0] = 1;
            link.primitive.dimensions[1] = 1;
            link.primitive.dimensions[2] = 1;
#endif
            link.material.name = "cyan";
            link.material.color.r = 0;
            link.material.color.g = 1;
            link.material.color.b = 0;
            link.material.color.a = 1;

            //link.data = File.ReadAllBytes("/Users/ueshiba/work/ur5e/wrist2.dae");
            //link.data = File.ReadAllBytes("C:/Users/user1/Documents/TRanbo.stl");
            link.data = File.ReadAllBytes(model);

            _links.Add(new Link(transform, link));
        }
    }

    private IEnumerator DelayMethod(float waitTime, Action action)
    {
        yield return new WaitForSeconds(waitTime);
        action();
    }

    private static Transform FindTransform(Transform tfm, string name)
    {
        if (tfm.name == name)
            return tfm;

        foreach (Transform child in tfm)
        {
            var found = RobotDescription.FindTransform(child, name);
            if (found != null)
                return found;
        }
        return null;
    }

    private void GetLinksResCallback(string message)
    {
        //Debug.Log(message);
        var nep_links = JsonConvert.DeserializeObject<NepGetLinksRes>(message).links;

        if (nep_links == null)
            return;

        _links.Clear();

        foreach (var nep_link in nep_links)
        {
            //Debug.Log("*** parent=" + nep_link.transform.header.frame_id + ", me=" + nep_link.transform.child_frame_id);
            var parent_transform = RobotDescription.FindTransform(transform, nep_link.transform.header.frame_id);
            if (parent_transform == null)
                parent_transform = transform;
            _links.Add(new Link(parent_transform, nep_link));
        }
    }

    private void TransformsCallback(string message)
    {
        //Debug.Log(message);
        var nep_tf_message = JsonConvert.DeserializeObject<NepTFMessage>(message);

        foreach (var nep_transform in nep_tf_message.transforms)
        {
            var found = RobotDescription.FindTransform(transform, nep_transform.child_frame_id);

            if (found != null)
            {
                found.localPosition = NepGlobal.NepToUnity(nep_transform.transform.translation);
                found.localRotation = NepGlobal.NepToUnity(nep_transform.transform.rotation);
            }
        }
    }
}
