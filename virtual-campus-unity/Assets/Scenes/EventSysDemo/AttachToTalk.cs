using UnityEngine;

public class AttachToTalk : MonoBehaviour
{
    [HideInInspector] public GameObject talk;

    public string talkerName;

    private void Start()
    {
        EventCenter.AddListener("create_" + talkerName + "_talk", OnTalkCreate);
    }

    private void OnTalkCreate(object data)
    {
        talk = (GameObject)data;

        var buttons = talk.transform.Find("Panel/Buttons");
        var btn = Instantiate(gameObject);
        btn.transform.parent = buttons;
        Destroy(btn.GetComponent<AttachToTalk>());

        var eventOp = GetComponent<EventOperator>();
        if (eventOp)
        {
            btn.SetActive(eventOp.CheckConditions());
        }
        else
        {
            btn.SetActive(true);
        }
    }

    private void OnDestroy()
    {
        EventCenter.RemoveListener("create_" + talkerName + "_talk", OnTalkCreate);
    }
}
