using Oculus.Avatar2;
using System;
using System.Collections.Generic;
using UnityEngine;

public class RemoteControl : MonoBehaviour
{
    [Header("Avatar")]
    public GameObject avatarObject;

    public GameObject leftHandSkeleton;
    public GameObject rightHandSkeleton;
    public bool updateLeftHandPose = false;
    public bool updateRightHandPose = false;

    OvrAvatarCustomHandPose[] customHandPoses;


    private void Start()
    {
        // cache y-pos of mirror cameras in scene
        foreach (var cam in webcams)
            cam.maxY = cam.cameraHandle.transform.localPosition.y;


        // init animation caches
        for (int i = 0; i < (int)ANIM.COUNT; i++)
            anims.Add((ANIM)i, new());


        customHandPoses = avatarObject.GetComponents<OvrAvatarCustomHandPose>();
    }

    private void Update()
    {
        if (updateLeftHandPose)
        {
            updateLeftHandPose = false;
            customHandPoses[0].UpdateHandPose();
        }
        if (updateRightHandPose)
        {
            updateRightHandPose = false;
            customHandPoses[1].UpdateHandPose();
        }
    }

    [Header("Spinning Platform")]
    public GameObject platformObject;
    public float rotationSpeed = .01f;

    public void action_rotateToDisplayWall()
    {
        anims[ANIM.rotatePlatform].animate(0);
    }
    public void action_rotateToMirrorAvatar()
    {
        anims[ANIM.rotatePlatform].animate(180);
    }


    [Serializable]
    public class WebCam
    {
        public GameObject cameraHandle;
        public float minY;

        [System.NonSerialized]
        public float maxY;
    }
    [Header("Mirror Webcams")]
    public List<WebCam> webcams;
    public float moveCameraSpeed = .01f;

    public void action_moveCamerasUp()
    {
        anims[ANIM.moveCameras].animateDiff(-.25f);
    }
    public void action_moveCamerasDown()
    {
        anims[ANIM.moveCameras].animateDiff(.25f);
    }


    class Anim
    {
        public float alpha = 1, speed = 0, start = 0, end = 0, result = 0;
        public bool changed = false;

        public void animate(float endValue)
        {
            start = result;
            alpha = 0;
            end = endValue;
        }

        public void animateDiff(float diffValue, float clampMin = 0, float clampMax = 1)
        {
            animate(Mathf.Clamp(result + diffValue, clampMin, clampMax));
        }

        public void step()
        {
            if (alpha != 1)
            {
                alpha = Mathf.Min(1, alpha + speed);
                result = Mathf.SmoothStep(start, end, alpha);
                changed = true;
            }
        }
    }
    Dictionary<ANIM, Anim> anims = new();
    enum ANIM { rotatePlatform, moveCameras, COUNT }

    private void FixedUpdate()
    {
        // run all animations
        anims[ANIM.rotatePlatform].speed = rotationSpeed;
        anims[ANIM.moveCameras].speed = moveCameraSpeed;
        foreach (var anim in anims.Values)
            anim.step();

        // platform rotation
        var rot = anims[ANIM.rotatePlatform];
        if (rot.changed)
            platformObject.transform.rotation = Quaternion.Euler(0, rot.result, 0);

        // webcam movement
        var mc = anims[ANIM.moveCameras];
        if (mc.changed)
        {
            foreach (var cam in webcams)
            {
                var pos = cam.cameraHandle.transform.localPosition;
                pos.y = Mathf.Lerp(cam.maxY, cam.minY, mc.result);
                cam.cameraHandle.transform.localPosition = pos;
            }
        }
    }


    //float source = 0;
    //float target = 0;
    //float rotation = 0;
    //float speed = 1;

    //void FixedUpdate()
    //{
    //    if (OVRInput.GetActiveController() != OVRInput.Controller.Hands)
    //    {
    //        //if (Mathf.Abs(rotation) < rotationStartThresh)
    //        {
    //            OVRInput.GetDown(OVRInput.Button.PrimaryThumbstick);
    //            Vector2 thumbstick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
    //            if (thumbstick.x != 0)
    //            {
    //                source = (rotation += thumbstick.x);
    //            }
    //        }
    //        //else
    //        {
    //            rotation += .1f * (target - rotation);
    //        }
    //    }
    //    transform.rotation = Quaternion.Euler(0, rotation, 0);
    //}
}
