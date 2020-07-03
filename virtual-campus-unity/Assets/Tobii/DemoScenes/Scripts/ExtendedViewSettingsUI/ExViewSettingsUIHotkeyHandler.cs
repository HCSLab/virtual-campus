using UnityEngine;

public class ExViewSettingsUIHotkeyHandler : MonoBehaviour {

    public GameObject ExtendedViewSettingsUi;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

	    if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
	        && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
	        && Input.GetKeyDown(KeyCode.X))
	    {
	        if (ExtendedViewSettingsUi != null)
	        {
	            ExtendedViewSettingsUi.SetActive(!ExtendedViewSettingsUi.activeSelf);
	        }
	    }
	}
}
