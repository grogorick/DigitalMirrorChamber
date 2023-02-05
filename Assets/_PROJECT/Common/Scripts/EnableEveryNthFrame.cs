using System.Collections.Generic;
using UnityEngine;

public class EnableEveryNthFrame : MonoBehaviour
{
    public int nthFrame = 2;
    public int frameOffset = 0;
    public List<GameObject> gameObjects;

    private int frame = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (nthFrame == 1)
        {
            Debug.LogError("### MY | EnableEveryNthFrame | n set to 1. Disabling component");
            gameObject.SetActive(false);
        }
        foreach (var obj in gameObjects)
        {
            obj.SetActive(false);
        }
        frame = frameOffset;
    }

    // Update is called once per frame
    void Update()
    {
        if (++frame >= nthFrame)
        {
            frame = 0;
            foreach (var obj in gameObjects)
            {
                obj.SetActive(true);
            }
        }
        else if (frame == 1)
        {
            foreach (var obj in gameObjects)
            {
                obj.SetActive(false);
            }
        }
    }
}
