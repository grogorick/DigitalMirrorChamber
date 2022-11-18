using Oculus.Avatar2;
using System;
using System.Collections.Generic;
using UnityEngine;

public class RemoteControl : MonoBehaviour
{
    [Header("Avatar")]
    public GameObject avatarObject;

    private OvrAvatarCustomHandPose customHandPoseLeft, customHandPoseRight;


    private void Start()
    {
        initAnims();
        anims[ANIM.rotatePlatform].speed = rotationSpeed;
        anims[ANIM.moveCameras].speed = moveCameraSpeed;

        // cache y-pos of mirror cameras in scene
        foreach (var cam in webcamObjects)
            cam.maxY = cam.cameraHandle.transform.localPosition.y;

        OvrAvatarCustomHandPose[] customHandPoses = avatarObject.GetComponents<OvrAvatarCustomHandPose>();
        customHandPoseLeft = customHandPoses[0];
        customHandPoseRight = customHandPoses[1];
    }

    public void action_approachTablet()
    {
        customHandPoseRight.enabled = true;
    }
    public void action_leaveTablet()
    {
        customHandPoseRight.enabled = false;
    }

    private void FixedUpdate()
    {
        // run all animations
        updateAnims();

        // platform rotation
        var rot = anims[ANIM.rotatePlatform];
        if (rot.changed)
            platformObject.transform.rotation = Quaternion.Euler(0, rot.result, 0);

        // webcam movement
        var mc = anims[ANIM.moveCameras];
        if (mc.changed)
        {
            foreach (var cam in webcamObjects)
            {
                var pos = cam.cameraHandle.transform.localPosition;
                pos.y = Mathf.Lerp(cam.maxY, cam.minY, mc.result);
                cam.cameraHandle.transform.localPosition = pos;
            }
        }
    }


    [Header("Spinning Platform")]
    public float rotationSpeed = .01f;
    public GameObject platformObject;

    public void action_rotateToDisplayWall()
    {
        anims[ANIM.rotatePlatform].animate(0);
    }

    public void action_rotateToMirrorAvatar()
    {
        anims[ANIM.rotatePlatform].animate(180);
    }


    [Header("Mirror Webcams")]
    public float moveCameraSpeed = .01f;

    [Serializable]
    public class WebCam
    {
        public GameObject cameraHandle;
        public float minY;

        [System.NonSerialized]
        public float maxY;
    }
    public List<WebCam> webcamObjects;

    public void action_moveCamerasUp()
    {
        anims[ANIM.moveCameras].animateDiff(-.25f);
    }

    public void action_moveCamerasDown()
    {
        anims[ANIM.moveCameras].animateDiff(.25f);
    }


    enum ANIM { rotatePlatform, moveCameras, COUNT }
    Dictionary<ANIM, Anim> anims = new();

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

    private void initAnims()
    {
        for (int i = 0; i < (int)ANIM.COUNT; i++)
            anims.Add((ANIM)i, new());
    }

    private void updateAnims()
    {
        foreach (var anim in anims.Values)
            anim.step();
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
