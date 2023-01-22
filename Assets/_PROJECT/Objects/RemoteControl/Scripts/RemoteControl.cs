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
        initLights();

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
        Debug.Log("### MY | Remote Control | Tablet zoom mount");
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
            Debug.Log("### MY | Remote Control | Left hand down detected");
            zoomUnmount();

            customHandPoseLeft.enabled = false;
            customHandPoseRight.enabled = false;
        }
    }
    private void zoomUnmount(Anim.CallbackVoid callbackWhenUnmounted = null)
    {
        Debug.Log("### MY | Remote Control | Tablet zoom unmount");

        zoomTriggerRing.SetActive(true);
        Vibrate.now(false, true);
        Anim a = animZoom.animate(0);
        if (callbackWhenUnmounted != null)
        {
            a.then(() =>
            {
                customHandPoseLeft.enabled = false;
                customHandPoseRight.enabled = false;

                callbackWhenUnmounted();
            });
        }

        foreach (TabletButton button in buttons)
            button.enabled = false;
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


    [Header("Light Switch")]
    public Light sunLight;
    public float sunLightIntensityOff;
    private float sunLightIntensityOn;

    public List<Light> lights;
    public Material lightBulbMaterial;
    public Color lightBulbColorOff;
    private Color lightBulbColorOn;
    public Material lampShadeMaterial;

    private void initLights()
    {
        sunLightIntensityOn = sunLight.intensity;
        lightBulbColorOn = lightBulbMaterial.color;
    }

    public void action_switchLights(bool lightsOn)
    {
        foreach (Light light in lights)
        {
            light.gameObject.SetActive(lightsOn);
        }
        if (lightsOn)
        {
            sunLight.intensity = sunLightIntensityOn;
            lightBulbMaterial.color = lightBulbColorOn;
            lightBulbMaterial.EnableKeyword("_EMISSION");
            lampShadeMaterial.EnableKeyword("_EMISSION");
        }
        else
        {
            sunLight.intensity = sunLightIntensityOff;
            lightBulbMaterial.color = lightBulbColorOff;
            lightBulbMaterial.DisableKeyword("_EMISSION");
            lampShadeMaterial.DisableKeyword("_EMISSION");
        }
    }


    [Header("Avatar Editor")]
    public TabletButton openAvatarEditorButton;
    private bool avatarEditorStarted = false;

    void initAvatarEditor()
    {
        Debug.Log("### MY | Remote Control | Init avatar editor");
        OVRManager.InputFocusAcquired += inputFocusAcquired;
        avatarChangeHandler.onAvatarReplaceEvent.AddListener(onAvatarReplace);
    }

#if UNITY_EDITOR
    [Header("Debug (in play mode only)")]
    public bool _reloadAvatarNow = false;
    public bool _showTablet = false;
    private void Update()
    {
        if (_reloadAvatarNow)
        {
            _reloadAvatarNow = false;
            action_startAvatarEditor();
        }
        if (_showTablet)
        {
            _showTablet = false;
            action_zoomMount();
        }
    }
#endif

    public void action_startAvatarEditor()
    {
        Debug.Log("### MY | Remote Control | Open avatar editor");

        zoomUnmount(_startAvatarEditor);
    }
    private void _startAvatarEditor()
    {
        openAvatarEditorButton.setPressed(false);

        avatarEditorStarted = true;
#if UNITY_EDITOR
        inputFocusAcquired();
#else
        AvatarEditorDeeplink.LaunchAvatarEditor();
#endif
    }

    private void inputFocusAcquired() // return from avatar editor
    {
        Debug.Log("### MY | Remote Control | Focus acquired");
        if (avatarEditorStarted)
        {
            avatarEditorStarted = false;

            Debug.Log("### MY | Remote Control | Resumed from avatar editor -> Reload avatar");
            avatarChangeHandler.reloadAvatar();
        }
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
        StartCoroutine(_quit());
    }
    private IEnumerator _quit()
    {
        yield return new WaitForSeconds(1);
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
