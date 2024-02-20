using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCube : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var mesh = GetComponent<MeshFilter>().mesh;

        foreach (var vertex in mesh.vertices)
        {
            Debug.Log("(" + vertex.x + "," + vertex.y + "," + vertex.z + ")");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
