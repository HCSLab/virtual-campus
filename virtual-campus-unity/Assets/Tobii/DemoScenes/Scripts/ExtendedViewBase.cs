//-----------------------------------------------------------------------
// Copyright 2016 Tobii AB (publ). All rights reserved.
//-----------------------------------------------------------------------

using UnityEngine;
using Tobii.Gaming;
using System.Collections;
using Tobii.GameIntegration.Net;

/*
 * Extended View
 * 
 * Extended view decouples your look direction from your movement direction. 
 * This lets you both look around a bit more without conciously have to move 
 * the mouse, but also lets you inspect objects that are locked to the camera's 
 * reference frame.
 * 
 * This implementation works in an absolute reference frame, essentially 
 * mapping each possible gaze point directly to a camera target orientation and
 * then interpolating the camera to that orientation. It does this using two 
 * levels of indirection, in this order:  Gaze point -> View target -> Camera 
 * rotation. The main reason is to smooth out the action when the camera 
 * rotation is close to the gaze point, but having the intermediate view target
 * is also useful when we want to pause the system, letting it smoothly come to
 * a stop instead of instantly halting it.
 */
public abstract class ExtendedViewBase : MonoBehaviour
{
    public float ResetResponsiveness = 5;

    public bool IsEnabled = true;

    public bool IsAiming { get; set; }

    public bool IsPaused { get { return Time.deltaTime < 1E-08f; } }

    [HideInInspector]
    public bool IsLocked = false;

    [HideInInspector]
    public float GazeViewResponsiveness = 0.5f;
    [HideInInspector]
    public float GazeViewExtensionAngle = 5f;
    [HideInInspector]
    public float HeadViewSensitivityScale = 0.65f;

    [HideInInspector]
    public Vector2 MaximumRotationLocalAngles = new Vector3(90f, 180f);
    [HideInInspector]
    public Vector2 MinimumRotationLocalAngles = new Vector3(-90f, -180f);

    [HideInInspector]
    public float MaximumRotationWorldAnglesX = 90f;
    [HideInInspector]
    public float MinimumRotationWorldAnglesX = -90f;

    [HideInInspector]
    public float HeadRotationScalar = 1;


    /* The extra yaw produced by the system */
    public float Yaw { get; protected set; }

    /* The extra pitch produced by the system */
    public float Pitch { get; protected set; }

    //-------------------------------------------------------------------------
    // Protected members
    //-------------------------------------------------------------------------

    protected Vector2 AimTargetScreen;
    protected Ray AimTargetRay;


    //-------------------------------------------------------------------------
    // Private members
    //-------------------------------------------------------------------------

    private Camera _cameraWithoutExtendedView;
    private Camera _cameraWithExtendedView;

    private bool _lastIsAiming;

    private const float MaxTimeWithoutData = 2f;

    private float _aimAtGazeResponsiveness;

    private const float AimAtGazeMaxReleaseResponsiveness = 3;
    private const float AimAtGazeMaxReleaseLerpSpeed = 0.5f;

    private float _clampedPitch;
    private float _clampedYaw;

    private Transformation _extendedViewTransformation;


    //-------------------------------------------------------------------------
    // Public properties
    //-------------------------------------------------------------------------

    public virtual Camera CameraWithoutExtendedView
    {
        get
        {
            if (_cameraWithoutExtendedView != null && _cameraWithoutExtendedView.gameObject != null) return _cameraWithoutExtendedView;

            var cameraGo = new GameObject("CameraTransformWithoutExtendedView");

            cameraGo.transform.parent = null;

            _cameraWithoutExtendedView = cameraGo.AddComponent<Camera>();
            _cameraWithoutExtendedView.enabled = false;
            return _cameraWithoutExtendedView;
        }
    }


    public virtual Camera CameraWithExtendedView
    {
        get
        {
            if (_cameraWithExtendedView != null && _cameraWithExtendedView.gameObject != null) return _cameraWithExtendedView;

            var cameraGo = new GameObject("CameraTransformWithExtendedView");

            cameraGo.transform.parent = null;

            _cameraWithExtendedView = cameraGo.AddComponent<Camera>();
            _cameraWithExtendedView.enabled = false;
            TobiiAPI.SetCurrentUserViewPointCamera(CameraWithExtendedView);
            return _cameraWithExtendedView;
        }
    }

    protected virtual void UpdateSettings()
    {
    }

    protected virtual void SendUpdatedSettingsToTgi()
    {
    }

    protected virtual void UpdateTransform()
    {
    }

    protected virtual void UpdateAllChangedExtendedViewSettings()
    {
    }
    //--------------------------------------------------------------------
    // MonoBehaviour event functions (messages)
    //--------------------------------------------------------------------

    protected virtual void Start()
    {
    }

    protected virtual void LateUpdate()
    {
        _extendedViewTransformation = TobiiGameIntegrationApi.GetExtendedViewTransformation();

        UpdateSettings();
        SendUpdatedSettingsToTgi();

        UpdateExtendedViewAngles();
        UpdateTransform();

#if UNITY_EDITOR
        UpdateAllChangedExtendedViewSettings();
#endif
    }

    //-------------------------------------------------------------------------
    // Protected/public virtual functions
    //-------------------------------------------------------------------------

    public virtual void AimAtWorldPosition(Vector3 worldPosition)
    {
        /* empty default implementation */
    }

    protected virtual bool ShouldPauseExtendedViewOnCleanUi()
    {
        return false;
    }

    //-------------------------------------------------------------------------
    // Protected functions
    //-------------------------------------------------------------------------

    protected void ProcessAimAtGaze(Camera mainCamera)
    {
        if (!_lastIsAiming && IsAiming && HasRecentGazePointData())
        {
            AimTargetScreen = TobiiAPI.GetGazePoint().Screen;
            var aimTargetWorld = mainCamera.ScreenToWorldPoint(new Vector3(AimTargetScreen.x, AimTargetScreen.y, 10));
            AimTargetRay = mainCamera.ScreenPointToRay(new Vector3(AimTargetScreen.x, AimTargetScreen.y, 10));

            AimAtWorldPosition(aimTargetWorld);
        }

        _lastIsAiming = IsAiming;
    }

    public void UpdateCameraWithoutExtendedView(Camera mainCamera)
    {
        UpdateCamera(CameraWithoutExtendedView, mainCamera);
    }

    public void UpdateCameraWithExtendedView(Camera mainCamera)
    {
        UpdateCamera(CameraWithExtendedView, mainCamera);
    }

    protected void UpdateCamera(Camera cameraToUpdate, Camera mainCamera)
    {
        cameraToUpdate.transform.position = mainCamera.transform.position;
        cameraToUpdate.transform.rotation = mainCamera.transform.rotation;
        cameraToUpdate.fieldOfView = mainCamera.fieldOfView;
    }

    public void Rotate(Component componentToRotate, float fovScalar = 1f, Vector3 up = new Vector3(), bool calculateClampedValues = true)
    {
        var componenetAsTransform = componentToRotate as Transform;
        var transformToRotate = componenetAsTransform != null ? componenetAsTransform : componentToRotate.transform;

        if (calculateClampedValues)
        {
            _clampedPitch = Pitch;
            _clampedYaw = Yaw;

            _clampedPitch = _clampedPitch < MaximumRotationLocalAngles.x ? _clampedPitch : MaximumRotationLocalAngles.x;
            _clampedPitch = _clampedPitch > MinimumRotationLocalAngles.x ? _clampedPitch : MinimumRotationLocalAngles.x;

            _clampedYaw = _clampedYaw < MaximumRotationLocalAngles.y ? _clampedYaw : MaximumRotationLocalAngles.y;
            _clampedYaw = _clampedYaw > MinimumRotationLocalAngles.y ? _clampedYaw : MinimumRotationLocalAngles.y;

            _clampedPitch = Mathf.DeltaAngle(0, _clampedPitch + transformToRotate.rotation.eulerAngles.x) < MaximumRotationWorldAnglesX ? _clampedPitch : MaximumRotationWorldAnglesX - transformToRotate.rotation.eulerAngles.x;
            _clampedPitch = Mathf.DeltaAngle(0, _clampedPitch + transformToRotate.rotation.eulerAngles.x) > MinimumRotationWorldAnglesX ? _clampedPitch : MinimumRotationWorldAnglesX - transformToRotate.rotation.eulerAngles.x;
        }

        transformToRotate.Rotate(_clampedPitch * fovScalar, 0.0f, 0.0f, Space.Self);

        if (up == new Vector3())
        {
            transformToRotate.Rotate(0.0f, _clampedYaw * fovScalar, 0.0f, Space.World);
        }
        else
        {
            transformToRotate.Rotate(up, _clampedYaw * fovScalar, Space.World);
        }
    }

    public Quaternion GetRotation(Component componentToRotate, float fovScalar = 1f, Vector3 up = new Vector3(), bool calculateClampedValues = true)
    {
        var componentAsTransform = componentToRotate as Transform;
        var transformToRotate = componentAsTransform != null ? componentAsTransform : componentToRotate.transform;
        var oldRotation = transformToRotate.rotation;

        Rotate(transformToRotate, fovScalar, up, calculateClampedValues);

        var rotation = transformToRotate.rotation;

        transformToRotate.rotation = oldRotation;

        return rotation;
    }

    protected void RotateAndGetCrosshairScreenPosition(Camera cameraToRotate, out Vector2 crosshairScreenPosition, float fovScalar = 1f, Vector3 up = new Vector3())
    {
        Vector3 vector;
        RotateAndGetCrosshairScreenPosition(cameraToRotate, out vector, fovScalar, up);
        crosshairScreenPosition = vector;
    }

    protected void RotateAndGetCrosshairScreenPosition(Camera cameraToRotate, out Vector3 crosshairScreenPosition, float fovScalar = 1f, Vector3 up = new Vector3())
    {
        var crosshairWorldPosition = cameraToRotate.transform.position + cameraToRotate.transform.forward;
        Rotate(cameraToRotate.transform, fovScalar, up);
        crosshairScreenPosition = cameraToRotate.WorldToScreenPoint(crosshairWorldPosition);
    }

    protected void RotateAndGetCrosshairViewportPosition(Camera cameraToRotate, out Vector2 crosshairScreenPosition, float fovScalar = 1f, Vector3 up = new Vector3())
    {
        Vector3 vector;
        RotateAndGetCrosshairViewportPosition(cameraToRotate, out vector, fovScalar, up);
        crosshairScreenPosition = vector;
    }

    protected void RotateAndGetCrosshairViewportPosition(Camera cameraToRotate, out Vector3 crosshairScreenPosition, float fovScalar = 1f, Vector3 up = new Vector3())
    {
        var crosshairWorldPosition = cameraToRotate.transform.position + cameraToRotate.transform.forward;
        Rotate(cameraToRotate.transform, fovScalar, up);
        crosshairScreenPosition = cameraToRotate.WorldToViewportPoint(crosshairWorldPosition);
    }

    protected IEnumerator ResetCameraWorld(Quaternion rotation, Transform cameraTransform = null)
    {
        cameraTransform = cameraTransform ? cameraTransform : transform;

        yield return new WaitForEndOfFrame();
        cameraTransform.rotation = rotation;
        PostResetCamera();
    }

    protected IEnumerator ResetCameraLocal(Quaternion? rotation = null, Transform camTransform = null)
    {
        camTransform = camTransform ? camTransform : transform;
        rotation = rotation.HasValue ? rotation : Quaternion.identity;

        yield return new WaitForEndOfFrame();
        camTransform.localRotation = rotation.Value;
        PostResetCamera();
    }

    protected IEnumerator ResetTransformPosition(Transform aTransform, Vector3 position)
    {
        yield return new WaitForEndOfFrame();
        aTransform.position = position;
    }

    protected IEnumerator ResetCameraPosition(Vector3 position)
    {
        yield return ResetTransformPosition(transform, position);
    }

    protected virtual void PostResetCamera()
    {

    }

    //private void OnGUI()
    //{
    //	GUI.backgroundColor = Color.blue;
    //	GUI.Box(new Rect((_gazeViewTarget.x + 1) * 0.5f * Screen.width - 5, Screen.height - ((_gazeViewTarget.y + 1) * 0.5f * Screen.height) - 5, 10, 10), " ");
    //}

    /// <summary>
    /// Translates the current view target to target orientation angles and lerp 
    /// the camera orientation towards it.
    /// </summary>
    private void UpdateExtendedViewAngles()
    {
        float targetYaw;
        float targetPitch;

        if (IsAiming || !IsEnabled)
        {
            targetYaw = 0;
            targetPitch = 0;
            _aimAtGazeResponsiveness = 0.5f;
            Yaw = Mathf.LerpAngle(Yaw, targetYaw, ResetResponsiveness * Time.unscaledDeltaTime);
            Pitch = Mathf.LerpAngle(Pitch, targetPitch, ResetResponsiveness * Time.unscaledDeltaTime);
        }
        else if (!IsLocked && !ShouldPauseExtendedViewOnCleanUi())
        {
            _aimAtGazeResponsiveness = Mathf.Lerp(_aimAtGazeResponsiveness, AimAtGazeMaxReleaseResponsiveness, AimAtGazeMaxReleaseLerpSpeed * Time.unscaledDeltaTime);
            targetYaw = _extendedViewTransformation.Rotation.Yaw * Mathf.Rad2Deg;
            targetPitch = -1 * _extendedViewTransformation.Rotation.Pitch * Mathf.Rad2Deg;

            Yaw = Mathf.LerpAngle(Yaw, targetYaw, _aimAtGazeResponsiveness * Time.unscaledDeltaTime);
            Pitch = Mathf.LerpAngle(Pitch, targetPitch, _aimAtGazeResponsiveness * Time.unscaledDeltaTime);
        }
    }

    private bool HasRecentGazePointData()
    {
        var gazePoint = TobiiAPI.GetGazePoint();
        return gazePoint.IsRecent(MaxTimeWithoutData);
    }
}