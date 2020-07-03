//-----------------------------------------------------------------------
// Copyright 2016 Tobii AB (publ). All rights reserved.
//-----------------------------------------------------------------------

using UnityEngine;

public class GunThirdPerson : Gun
{
	public Transform GunHandArm;
	public Transform GunHandGrip;

	protected override void AlignGunToCrosshairDirection()
	{
		if (WeaponController.IsAiming || WeaponController.IsShooting)
		{
			if (OptionalLaserSight != null)
			{
				OptionalLaserSight.IsEnabled = true;
			}
			var gunOrbitRadius = 0.4f;
			var desiredHeading = WeaponController.WeaponHitData.point - GunHandArm.transform.position;

			desiredHeading.Normalize();

			transform.rotation = Quaternion.Lerp(transform.rotation,
				Quaternion.LookRotation(desiredHeading, Vector3.up), GunAlignmentSpeed);

			var desiredPosition = GunHandArm.transform.position + transform.forward * gunOrbitRadius;

			transform.position = Vector3.Lerp(transform.position, desiredPosition, GunAlignmentSpeed);
		}
		else
		{
			if (OptionalLaserSight != null)
			{
				OptionalLaserSight.IsEnabled = false;
			}
			transform.position = Vector3.Lerp(transform.position, GunHandGrip.position, GunAlignmentSpeed);
			transform.rotation = Quaternion.Lerp(transform.rotation, GunHandGrip.rotation, GunAlignmentSpeed);
		}
	}
}