using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayWallOffsetPerDisplayUV : MonoBehaviour
{
    public Vector2
        offset = new (0, 0),
        size = new (1, 1);

    void Start()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;

        List<Vector2> uvs = new();
        mesh.GetUVs(0, uvs);

        for (int i = 0; i < uvs.Count; i++)
        {
            Vector2 uv = uvs[i];
            uv.Scale(size);
            uv += offset;
            uvs[i] = uv;
        }
        mesh.SetUVs(0, uvs);
    }
}
