using Oculus.Avatar2;
using OVR;
using System;
using System.Collections.Generic;
using UnityEngine;

public class RemoteControl : MonoBehaviour
{
    private void Start()
    {
        initZoom();
        initAnims();

        // cache y-pos of mirror cameras in scene
        foreach (var cam in webcamObjects)
        {
            cam.maxPosY = cam.cameraPosHandle.transform.localPosition.y;

            cam.maxRot = cam.cameraAngleHandle.transform.localRotation;

            Vector3 eulers = cam.maxRot.eulerAngles;
            eulers.x = cam.minAngleX;
            cam.minRot = Quaternion.Euler(eulers);
        }

        initAvatar();
    }

    private void FixedUpdate()
    {
        // run all animations
        anims.update();

        // platform rotation
        if (animRotatePlatform.changed)
            platformObject.transform.rotation = Quaternion.Euler(0, animRotatePlatform.value, 0);

        // webcam movement
        if (animMoveCameras.changed)
        {
            foreach (var cam in webcamObjects)
            {
                Vector3 pos = cam.cameraPosHandle.transform.localPosition;
                pos.y = Mathf.Lerp(cam.minPosY, cam.maxPosY, animMoveCameras.value);
                cam.cameraPosHandle.transform.localPosition = pos;

                cam.cameraAngleHandle.transform.localRotation = Quaternion.Slerp(cam.minRot, cam.maxRot, animMoveCameras.value);
            }
        }

        // un-/mount zoom
        if (animZoom.changed)
        {
            float zTriggerRing = Mathf.Lerp(minZoomTrigger, maxZoomTriggerRing, 1 - animZoom.value);
            float zTriggerFingertip = Mathf.Lerp(minZoomTrigger, maxZoomTriggerFingertip, 1 - animZoom.value);
            float zRemoteControl = Mathf.Lerp(minZoomRemoteControl, maxZoomRemoteControl, animZoom.value);
            zoomTriggerRing.transform.localScale = new Vector3(zTriggerRing, zTriggerRing, zTriggerRing);
            zoomTriggerFingertip.transform.localScale = new Vector3(zTriggerFingertip, zTriggerFingertip, zTriggerFingertip);
            zoomOriginRemoteControl.transform.localScale = new Vector3(zRemoteControl, zRemoteControl, zRemoteControl);

            if (animZoom.value == 0)
            {
                zoomOriginRemoteControl.SetActive(false);
            }
            if (animZoom.value == 1)
            {
                zoomTriggerRing.SetActive(false);
            }
        }
        // unmount event
        else if (animZoom.value == 1)
        {
            if ((transform.rotation * Vector3.up).y < -.2f)
            {
                action_zoomUnmount();
            }
        }
    }


    [Header("Avatar")]
    public SampleAvatarEntity avatarEntity;
    private OvrAvatarCustomHandPose customHandPoseLeft, customHandPoseRight;
    private int approachTabletCount = 0;

    private void initAvatar()
    {
        OvrAvatarCustomHandPose[] customHandPoses = avatarEntity.GetComponents<OvrAvatarCustomHandPose>();
        customHandPoseLeft = customHandPoses[0];
        customHandPoseRight = customHandPoses[1];
    }

    public void action_approachTablet()
    {
        if (approachTabletCount++ == 0)
            customHandPoseRight.enabled = true;
    }
    public void action_leaveTablet()
    {
        if (--approachTabletCount == 0)
            customHandPoseRight.enabled = false;
    }


    [Header("Spinning Platform")]
    public float rotationSpeed = .01f;
    public GameObject platformObject;

    public void action_rotateToDisplayWall()
    {
        animRotatePlatform.animate(0);
    }

    public void action_rotateToMirrorAvatar()
    {
        animRotatePlatform.animate(180);
    }


    [Header("Mirror Webcams")]
    public float moveCamerasSpeed = .01f;

    [Serializable]
    public class WebCam
    {
        public GameObject cameraPosHandle;
        public float minPosY;
        [System.NonSerialized]
        public float maxPosY;

        public GameObject cameraAngleHandle;
        public float minAngleX;
        [System.NonSerialized]
        public Quaternion minRot, maxRot;
    }
    public List<WebCam> webcamObjects;

    public void action_moveCamerasUp()
    {
        animMoveCameras.animateDiff(.25f);
    }

    public void action_moveCamerasDown()
    {
        animMoveCameras.animateDiff(-.25f);
    }


    [Header("Sound System")]
    public AudioSource soundSource;
    public GameObject soundSystemPowerOn;

    public void action_soundPlay()
    {
        soundSource.Play();
        soundSystemPowerOn.SetActive(true);
    }

    public void action_soundPause()
    {
        soundSource.Pause();
        soundSystemPowerOn.SetActive(false);
    }


    [Header("Un-/Mount Zoom")]
    public GameObject zoomTriggerRing;
    public GameObject zoomTriggerFingertip;
    public GameObject zoomOriginRemoteControl;
    public float minZoomTrigger = 0;
    public float maxZoomTriggerRing = 1;
    public float maxZoomTriggerFingertip;
    public float minZoomRemoteControl = .01f;
    private float maxZoomRemoteControl;
    public float zoomSpeed = .01f;

    private void initZoom()
    {
        maxZoomTriggerFingertip = zoomTriggerFingertip.transform.localScale.x;
        maxZoomRemoteControl = zoomOriginRemoteControl.transform.localScale.x;
    }

    public void action_zoomMount()
    {
        animZoom.animate(1);
        zoomOriginRemoteControl.SetActive(true);
        customHandPoseLeft.enabled = true;
        Vibrate.now(false, true);
    }
    public void action_zoomUnmount()
    {
        animZoom.animate(0);
        zoomTriggerRing.SetActive(true);
        customHandPoseLeft.enabled = false;
        Vibrate.now(false, true);
    }


    private Anim animRotatePlatform;
    private Anim animMoveCameras;
    private Anim animZoom;

    private AnimationPool anims = new();

    private void initAnims()
    {
        anims.Add(animRotatePlatform = new(0, rotationSpeed));
        anims.Add(animMoveCameras = new(1, moveCamerasSpeed));
        anims.Add(animZoom = new(1, zoomSpeed));
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


class AnimationPool : List<Anim>
{
    public void update()
    {
        foreach (var anim in this)
            anim.step();
    }
}

class Anim
{
    public float speed;
    public bool changed { get; private set; } = false;
    public float value { get; private set; }

    private float alpha = 1, start = 0, end = 0;

    public Anim(float startValue, float speed, float? endValue = null)
    {
        this.speed = speed;
        value = startValue;

        if (endValue != null)
            animate(endValue.Value);
    }

    public void animate(float endValue)
    {
        start = value;
        end = endValue;
        alpha = 0;
    }

    public void animateDiff(float diffValue, float clampMin = 0, float clampMax = 1)
    {
        animate(Mathf.Clamp(value + diffValue, clampMin, clampMax));
    }

    public void step()
    {
        if (changed = (alpha != 1))
        {
            alpha = Mathf.Min(1, alpha + speed);
            value = Mathf.SmoothStep(start, end, alpha);
        }
    }
}
