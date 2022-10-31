using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyTextureScript : MonoBehaviour
{
    public Texture source, target;

    void Update()
    {
        Graphics.CopyTexture(source, target);
    }
}
