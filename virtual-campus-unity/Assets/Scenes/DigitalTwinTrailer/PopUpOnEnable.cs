using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpOnEnable : MonoBehaviour
{
	private void OnEnable()
	{
		transform.localScale *= 0.1f;

		transform.LeanScale(transform.localScale * 10f, 0.2f)
			.setEaseOutQuad();
	}

}
