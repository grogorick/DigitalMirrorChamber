using Oculus.Avatar2;
using System;
using System.Collections.Generic;
using UnityEngine;

public class RemoteControl : MonoBehaviour
{
    private void Start()
    {
        initAnims();

        // cache y-pos of mirror cameras in scene
        foreach (var cam in webcamObjects)
            cam.maxY = cam.cameraHandle.transform.localPosition.y;

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
                var pos = cam.cameraHandle.transform.localPosition;
                pos.y = Mathf.Lerp(cam.minY, cam.maxY, animMoveCameras.value);
                cam.cameraHandle.transform.localPosition = pos;
            }
        }
    }


    [Header("Avatar")]
    public SampleAvatarEntity avatarEntity;

    private OvrAvatarCustomHandPose customHandPoseLeft, customHandPoseRight;

    private void initAvatar()
    {
        OvrAvatarCustomHandPose[] customHandPoses = avatarEntity.GetComponents<OvrAvatarCustomHandPose>();
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
        public GameObject cameraHandle;
        public float minY;

        [System.NonSerialized]
        public float maxY;
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

    public void action_soundPlay()
    {
        soundSource.Play();
    }

    public void action_soundPause()
    {
        soundSource.Pause();
    }


    private Anim animRotatePlatform;
    private Anim animMoveCameras;

    private AnimationPool anims = new();

    private void initAnims()
    {
        anims.Add(animRotatePlatform = new(0, rotationSpeed));
        anims.Add(animMoveCameras = new(1, moveCamerasSpeed));
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
