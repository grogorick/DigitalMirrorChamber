using Oculus.Avatar2;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteControl : MonoBehaviour
{
    private void Start()
    {
        initAvatarHandPoses();
        initAvatarEditor();
        initZoom();

        initAnims();

        // start with unmounted tablet
        customHandPoseLeft.enabled = false;
        customHandPoseRight.enabled = false;
        anim_zoomTablet(animZoom.value);

        // cache y-pos of mirror cameras in scene
        foreach (var cam in webcamObjects)
        {
            cam.maxPosY = cam.cameraPosHandle.transform.localPosition.y;

            cam.maxRot = cam.cameraAngleHandle.transform.localRotation;

            Vector3 eulers = cam.maxRot.eulerAngles;
            eulers.x = cam.minAngleX;
            cam.minRot = Quaternion.Euler(eulers);
        }
    }

    private void FixedUpdate()
    {
        leaveTabletAfterDelay();

        // run all animations
        anims.update();

        zoomUnmountWhenLeftHandDown();
    }


    [Header("Avatars")]
    public Avatar mainAvatar;
    public Avatar mirrorAvatar;
    public AvatarChangeHandler avatarChangeHandler;
    private OvrAvatarCustomHandPose customHandPoseLeft, customHandPoseRight;

    private void initAvatarHandPoses()
    {
        Debug.Log("### MY | Remote Control | Init hand poses");
        OvrAvatarCustomHandPose[] customHandPoses = mainAvatar.GetComponents<OvrAvatarCustomHandPose>();
        customHandPoseLeft = customHandPoses[0];
        customHandPoseRight = customHandPoses[1];
    }


    private DateTime waitUntil = DateTime.Now;

    public void action_approachTablet()
    {
        if (customHandPoseRight != null && !customHandPoseRight.enabled)
            customHandPoseRight.enabled = true;
        waitUntil = DateTime.Now.AddSeconds(.5);
    }

    private void leaveTabletAfterDelay()
    {
        if (customHandPoseRight != null && customHandPoseRight.enabled && waitUntil < DateTime.Now)
            customHandPoseRight.enabled = false;
    }


    [Header("Spinning Platform")]
    public float rotationSpeed = .01f;
    public GameObject platformObject;

    public void action_rotateToDisplayWall()
    {
        moveCamerasUpAndRotatePlatform(0);
    }

    public void action_rotateToMirrorAvatar()
    {
        moveCamerasUpAndRotatePlatform(180);
    }

    public void anim_rotatePlatform(float value)
    {
        platformObject.transform.rotation = Quaternion.Euler(0, value, 0);
    }

    public void moveCamerasUpAndRotatePlatform(float rotation)
    {
        float tmpCameraValue = animMoveCameras.value;
        animMoveCameras.animate(1).then(() =>
        {
            animRotatePlatform.animate(rotation).then(() =>
            {
                animMoveCameras.animate(tmpCameraValue);
            });
        });
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

    public void anim_moveCameras(float value)
    {
        foreach (var cam in webcamObjects)
        {
            Vector3 pos = cam.cameraPosHandle.transform.localPosition;
            pos.y = Mathf.Lerp(cam.minPosY, cam.maxPosY, value);
            cam.cameraPosHandle.transform.localPosition = pos;

            cam.cameraAngleHandle.transform.localRotation = Quaternion.Slerp(cam.minRot, cam.maxRot, value);
        }
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
    private TabletButton[] buttons;

    private void initZoom()
    {
        Debug.Log("### MY | Remote Control | Init tablet zoom");
        maxZoomTriggerFingertip = zoomTriggerFingertip.transform.localScale.x;
        maxZoomRemoteControl = zoomOriginRemoteControl.transform.localScale.x;

        buttons = GetComponentsInChildren<TabletButton>();
    }

    public void action_zoomMount()
    {
        zoomOriginRemoteControl.SetActive(true);
        customHandPoseLeft.enabled = true;
        Vibrate.now(false, true);
        animZoom.animate(1).then(() => StartCoroutine(_enableTabletButtonsAfterDelay()));
    }
    private IEnumerator _enableTabletButtonsAfterDelay(float delaySeconds = 2)
    {
        yield return new WaitForSeconds(delaySeconds);
        foreach (TabletButton button in buttons)
        {
            button.enabled = true;
        }
    }

    public void zoomUnmountWhenLeftHandDown()
    {
        // left hand pointing down
        if (animZoom.value == 1 && (transform.rotation * Vector3.up).y < -.2f)
        {
            Debug.Log("### MY | Remote Control | Hand down detected -> tablet zoom unmount");
            zoomTriggerRing.SetActive(true);
            customHandPoseLeft.enabled = false;
            customHandPoseRight.enabled = false;
            Vibrate.now(false, true);
            animZoom.animate(0);

            foreach (TabletButton button in buttons)
                button.enabled = false;
        }
    }

    public void anim_zoomTablet(float value)
    {
        float zTriggerRing = Mathf.Lerp(minZoomTrigger, maxZoomTriggerRing, 1 - value);
        float zTriggerFingertip = Mathf.Lerp(minZoomTrigger, maxZoomTriggerFingertip, 1 - value);
        float zRemoteControl = Mathf.Lerp(minZoomRemoteControl, maxZoomRemoteControl, value);
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


    void initAvatarEditor()
    {
        Debug.Log("### MY | Remote Control | Init avatar editor");
        OVRManager.InputFocusAcquired += inputFocusAcquired;
        avatarChangeHandler.onAvatarReplaceEvent.AddListener(onAvatarReplace);
    }

    public void action_startAvatarEditor()
    {
        Debug.Log("### MY | Remote Control | Open avatar editor");
#if UNITY_EDITOR
        avatarChangeHandler.onAvatarChangeDetected(mainAvatar);
#else
        AvatarEditorDeeplink.LaunchAvatarEditor();
#endif
    }
    public bool loadAvatarEditor = false;
    public bool showTablet = false;
    private void Update()
    {
        if (loadAvatarEditor)
        {
            loadAvatarEditor = false;
            action_startAvatarEditor();
        }
        if (showTablet)
        {
            showTablet = false;
            action_zoomMount();
        }
    }

    private void inputFocusAcquired() // return from avatar editor
    {
        Debug.Log("### MY | Remote Control | Focus acquired -> check for avatar changes");
        mainAvatar.checkForChanges();
    }
    //private void OnApplicationPause(bool pause)
    //{
    //    if (!pause) // User returned to app, may have edited their avatar
    //    {
    //        Debug.Log("### MY | Remote Control | Resume");
    //        //avatarEntity.checkForChanges();
    //    }
    //}

    private void onAvatarReplace(Avatar oldAvatar, Avatar newAvatar)
    {
        mainAvatar = newAvatar;
        initAvatarHandPoses();
    }


    public void action_quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }


    private Anim animRotatePlatform;
    private Anim animMoveCameras;
    private Anim animZoom;

    private AnimationPool anims = new();

    private void initAnims()
    {
        Debug.Log("### MY | Remote Control | Init animations");
        anims.Add(animRotatePlatform = new(0, rotationSpeed, anim_rotatePlatform));
        anims.Add(animMoveCameras = new(1, moveCamerasSpeed, anim_moveCameras));
        anims.Add(animZoom = new(0, zoomSpeed, anim_zoomTablet));
    }
}
