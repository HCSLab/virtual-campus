//-----------------------------------------------------------------------
// Copyright 2016 Tobii AB (publ). All rights reserved.
//-----------------------------------------------------------------------

using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public abstract class Gun : MonoBehaviour
{
    //How fast the gun will rotate / align to the new aim direction
    public float GunAlignmentSpeed = 0.2f;
    public float TimeBetweenShots = 0.2F;
    public double StopShootingDelay = 0.5F;
    public int BulletsPerShot = 1;
    public float SpreadAtOneMeter = 0.02f;
    public GameObject BulletHolePrefab;
    public AudioClip FireSound;
    public AnimationCurve FireAnimationRotationCurve = AnimationCurve.EaseInOut(0.0f, 0.0f, 1.0f, 0.0f);
    //Don't shoot while selecting objects

    public ExtendedViewBase ExtendedView;
    private AudioSource _audio;
    protected WeaponController WeaponController;
    protected LaserSight OptionalLaserSight;

    private Transform _animationTransform;
    private Quaternion _baseRotation;
    private float _animationTime;
    private bool _lastLeftTrigger;

    private float _nextFire = 0.0F;
    private float _lastFire = 0.0F;

    protected void Start()
    {
        _audio = GetComponent<AudioSource>();
        OptionalLaserSight = GetComponentInChildren<LaserSight>();
        WeaponController = GetComponentInParent<WeaponController>();
        _animationTransform = transform.GetChild(0);
        _baseRotation = _animationTransform.localRotation;
    }

    protected void Update()
    {
        var leftTrigger = false;
        var rightTrigger = false;
        var leftTriggerDown = leftTrigger && !_lastLeftTrigger;
        var joystickButton1Down = Input.GetKeyDown(KeyCode.JoystickButton1);

        if ((WeaponController != null)
            && (Input.GetKeyDown(KeyCode.Mouse1)
            || leftTriggerDown))
        {
            WeaponController.StartAiming();
        }
        else if ((WeaponController != null)
            && (!Input.GetKey(KeyCode.Mouse1)
                && !leftTrigger))
        {
            WeaponController.StopAiming();
        }

        _lastLeftTrigger = leftTrigger;

        if ((WeaponController != null)
            && (Time.time > _lastFire + StopShootingDelay))
        {
            WeaponController.StopShooting();
        }

        if ((Input.GetKeyDown(KeyCode.Mouse0)
                || Input.GetKeyDown(KeyCode.E)
                || joystickButton1Down
                || rightTrigger)
            && Time.time > _nextFire)
        {
            _lastFire = Time.time;
            _nextFire = Time.time + TimeBetweenShots;

            var shootAtGaze = Input.GetKeyDown(KeyCode.E) || joystickButton1Down;

            Fire(shootAtGaze);
        }

        if (_animationTime < FireAnimationRotationCurve[FireAnimationRotationCurve.length - 1].time)
        {
            _animationTime += Time.deltaTime;
            _animationTransform.localRotation = _baseRotation * Quaternion.Euler(0.0f, FireAnimationRotationCurve.Evaluate(_animationTime), 0.0f);
        }
        else
        {
            _animationTransform.localRotation = _baseRotation;
        }

        if (WeaponController != null)
        {
            WeaponController.Calculate();
        }
        AlignGunToCrosshairDirection();
    }

    private void Fire(bool shootAtGaze)
    {
        if (WeaponController != null)
        {
            if (shootAtGaze)
            {
                WeaponController.StartShootingAtGaze();
            }
            else
            {
                WeaponController.StartShooting();
            }
            WeaponController.Calculate();
        }

        _animationTime = 0.0f;

        if (FireSound != null)
        {
            _audio.clip = FireSound;
            _audio.Play();
        }

        //Only interact with stuff if we actually have a target intersection point
        if ((BulletsPerShot > 0)
            && (WeaponController != null)
            && WeaponController.IsWeaponHitObject)
        {
            ShootBullet(WeaponController.WeaponHitData);
        }

        var origin = ExtendedView.CameraWithoutExtendedView.transform.position;
        var mainDirection = ExtendedView.CameraWithoutExtendedView.transform.forward;
        if (WeaponController != null)
        {
            origin = ExtendedView.CameraWithExtendedView.transform.position;
            if (WeaponController.OptionalWeaponFireOriginOverride != null)
            {
                origin = WeaponController.OptionalWeaponFireOriginOverride.position;
            }
            mainDirection = WeaponController.WeaponHitData.point - origin;
        }

        mainDirection.Normalize();

        for (var i = 1; i < BulletsPerShot; i++)
        {
            var rand = Random.insideUnitCircle * SpreadAtOneMeter;
            var left = Vector3.Cross(mainDirection, Vector3.Dot(mainDirection, Vector3.up) > 0.95 ? Vector3.right : Vector3.up);
            var up = Vector3.Cross(mainDirection, left);
            var direction = mainDirection + rand.x * left + rand.y * up;
            RaycastHit hitInfo;
            if (Physics.Raycast(origin, direction, out hitInfo, WeaponController.MaxProjectionDistance, WeaponController.RaycastLayerMask))
            {
                ShootBullet(hitInfo);
            }
        }
    }

    private void ShootBullet(RaycastHit hitInfo)
    {
        var hitObject = HitTarget(hitInfo.transform);
        SpawnBulletHole(hitInfo, hitObject);
        SpawnLaser(hitInfo);
    }

    private void SpawnLaser(RaycastHit hitInfo)
    {
        if (OptionalLaserSight == null)
            return;

        var go = new GameObject("LaserBeam");
        var line = go.AddComponent<LineRenderer>();
        line.materials = new[] { OptionalLaserSight.LaserSightMaterial };
        line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        line.receiveShadows = false;

        SetLinePositionCount(line, positionCount: 2);
        SetLineStartAndEndWidth(line, startWidth: 0.01f, endWidth: 0.01f);

        line.SetPosition(0, OptionalLaserSight.transform.position);
        line.SetPosition(1, hitInfo.point);
        Destroy(go, .1f);
    }

    private static void SetLinePositionCount(LineRenderer line, int positionCount)
    {
#if UNITY_5_6_OR_NEWER
        line.positionCount = positionCount;
#elif UNITY_5_5_OR_NEWER
        line.numPositions = positionCount;
#else
        line.SetVertexCount(positionCount);
#endif
    }

    private static void SetLineStartAndEndWidth(LineRenderer line, float startWidth, float endWidth)
    {
#if UNITY_5_5_OR_NEWER
        line.startWidth = startWidth;
        line.endWidth = endWidth;
#else
		line.SetWidth(startWidth, endWidth);
#endif
    }

    private void SpawnBulletHole(RaycastHit hitInfo, Transform hitObject)
    {
        if (BulletHolePrefab == null) return;

        var hitRotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
        var bulletHole = (GameObject)Instantiate(BulletHolePrefab, hitInfo.point + hitInfo.normal * 0.0001f, hitRotation);
        if (hitObject != null)
        {
            bulletHole.transform.SetParent(hitObject);
        }
    }

    private Transform HitTarget(Transform go)
    {
        var targetDummy = go.transform.GetComponent<TargetDummy>();
        if (targetDummy == null)
        {
            targetDummy = go.GetComponentInParent<TargetDummy>();
        }

        if (targetDummy != null)
        {
            targetDummy.Hit();
        }
        return go.transform;
    }

    protected void OnDisable()
    {
        if (OptionalLaserSight != null)
        {
            OptionalLaserSight.IsEnabled = false;
        }
    }

    protected abstract void AlignGunToCrosshairDirection();
}