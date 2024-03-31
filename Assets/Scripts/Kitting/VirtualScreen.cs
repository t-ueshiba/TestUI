using UnityEngine;
using NepSensorMsgs;
using NepGeometryMsgs;
using Nep;
using Newtonsoft.Json;
using System;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]

public class VirtualScreen : MonoBehaviour
{
    // NEP messages from ROS
    public class NepMeshTriangle
    {
        public uint[] vertex_indices { get; set; }

        NepMeshTriangle()
        {
            vertex_indices = new uint[3];
        }
    }

    public class NepMesh
    {
        public NepMeshTriangle[] triangles { get; set; }
        public NepPoint[]        vertices  { get; set; }
    }

    public class NepTexturedMesh
    {
        public Header   header { get; set; }
        public NepMesh  mesh   { get; set; }
        public double[] u      { get; set; }
        public double[] v      { get; set; }
    }

    // Member variables
    private NepCallback  _texturedMeshCallback;
    private Mesh         _mesh;
    private NepCallback  _imageCallback;
    private Texture2D    _texture;
    
    // Member functions
    private void Start()
    {
        var scene_name = gameObject.scene.name;
        _texturedMeshCallback = NepGlobal.Node.NewCallback(scene_name + "/screen_mesh", TexturedMeshCallback, NepGlobal.IP);
        _texturedMeshCallback.Start();

        _imageCallback = NepGlobal.Node.NewCallback(scene_name + "/image", ImageCallback, NepGlobal.IP);
        _imageCallback.Start();
    }

    private void Update()
    {
        GetComponent<MeshRenderer>().material.SetTexture("_MainTex", _texture);
        GetComponent<MeshFilter>().mesh = _mesh;
    }

    private void OnDestroy()
    {
        Destroy(_texture);
    }

    private void TexturedMeshCallback(string message)
    {
        if (_mesh != null)
            return;

        _mesh = new Mesh();
        _mesh.name = "ScreenMesh";

        //Debug.Log(message);
        var textured_mesh = JsonConvert.DeserializeObject<NepTexturedMesh>(message);
        
        var mesh_vertices = textured_mesh.mesh.vertices;
        var vertices = new Vector3[mesh_vertices.Length];
        for (int i = 0; i < mesh_vertices.Length; ++i)
            vertices[i] = NepGlobal.NepToUnity(mesh_vertices[i]);
        _mesh.SetVertices(vertices);

        var mesh_triangles = textured_mesh.mesh.triangles;
        var triangles = new int[3*mesh_triangles.Length];
        for (int i = 0; i < mesh_triangles.Length; ++i)
        {
            // The order of vertices in a triangle should be counter-clockwise.
            triangles[3*i    ] = (int)mesh_triangles[i].vertex_indices[0];
            triangles[3*i + 1] = (int)mesh_triangles[i].vertex_indices[2];
            triangles[3*i + 2] = (int)mesh_triangles[i].vertex_indices[1];
        }
        _mesh.SetTriangles(triangles, 0);

        var uv = new Vector2[textured_mesh.u.Length];
        for (int i = 0; i < textured_mesh.u.Length; ++i)
        {
            // Direction of v-axis is upward.
            uv[i].x = (float)textured_mesh.u[i];
            uv[i].y = (float)(1.0 - textured_mesh.v[i]);
        }
        _mesh.SetUVs(0, uv);

        _mesh.RecalculateBounds();
        _mesh.RecalculateNormals();
        _mesh.RecalculateTangents();
    }

    private void ImageCallback(string message)
    {
        if (_texture == null)
            _texture = new Texture2D(2, 2, TextureFormat.RGBA32, true);

        _texture.LoadImage(Convert.FromBase64String(message));
        _texture.Apply();
    }
}
