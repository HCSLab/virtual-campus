using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTarget: CameraClass {

    [SerializeField] private float speed;
	void Start(){
       // _position =  target.InverseTransformPoint(transform.localPosition);
    }
	public override void SetPosition () {
		transform.localPosition = _position;
		transform.LookAt(target.position);
	}

	void FixedUpdate() {
        var targetPos = target.TransformPoint(_position);
        cameraRotate = transform;
        cameraRotate.LookAt(target.position);
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * speed);
        transform.rotation = Quaternion.Lerp(transform.rotation, cameraRotate.rotation, Time.deltaTime * speed);
    }
}
