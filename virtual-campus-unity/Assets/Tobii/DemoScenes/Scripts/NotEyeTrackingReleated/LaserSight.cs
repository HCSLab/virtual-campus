//-----------------------------------------------------------------------
// Copyright 2016 Tobii AB (publ). All rights reserved.
//-----------------------------------------------------------------------

using UnityEngine;

public class LaserSight : MonoBehaviour
{
	public GameObject LaserSightImpactPointPrefab;
	public Material LaserSightMaterial;
	public float Width = 0.0005f;
	public bool IsEnabled = true;

	private GameObject _laserSightImpactPoint;
	private WeaponController _weaponController;
	private LineRenderer _laserSight;

	protected void Start()
	{
		_weaponController = transform.root.gameObject.GetComponentInChildren<WeaponController>();

		_laserSight = gameObject.AddComponent<LineRenderer>();
		_laserSight.materials = new[] { LaserSightMaterial };
		_laserSight.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		_laserSight.receiveShadows = false;

		_laserSightImpactPoint = Instantiate(LaserSightImpactPointPrefab);
	}

	protected void LateUpdate()
	{
		var laserSightPositions = new[] { transform.position, transform.position + transform.forward * 0.5f };
		_laserSight.SetPosition(0, laserSightPositions[0]);
		_laserSight.SetPosition(1, laserSightPositions[1]);
	    
        SetLaserSightStartAndEndWidth(startWidth: 0.002f, endWidth: 0.0f);

        _laserSight.material.mainTextureOffset = new Vector2((-Time.time * 2.0f) % 1.0f, 0.0f);

		if (IsEnabled && _weaponController.IsWeaponHitObject)
		{
			var hitInfo = _weaponController.WeaponHitData;
			_laserSightImpactPoint.SetActive(true);
			_laserSightImpactPoint.transform.position = hitInfo.point + hitInfo.normal * 0.0005f;
			_laserSightImpactPoint.transform.rotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
		}
		else
		{
			_laserSightImpactPoint.SetActive(false);
		}
	}
	protected void OnDisable()
	{
		if (_laserSightImpactPoint != null)
		{
			_laserSightImpactPoint.SetActive(false);
		}
	}

    private void SetLaserSightStartAndEndWidth(float startWidth, float endWidth)
    {
#if UNITY_5_5_OR_NEWER
        _laserSight.startWidth = startWidth;
        _laserSight.endWidth = endWidth;
#else
        _laserSight.SetWidth(startWidth, endWidth);
#endif
    }
}