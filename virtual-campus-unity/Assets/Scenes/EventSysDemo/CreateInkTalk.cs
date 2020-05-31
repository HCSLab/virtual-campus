using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateInkTalk : MonoBehaviour
{
    public TextAsset inkFile;
    public string executeFunction;

    public GameObject talkPrefab;

    protected GameObject talk;

    public void Create()
    {
        talk = Instantiate(talkPrefab);

        var ink = talk.GetComponent<InkTalk>();
        ink.inkFile = inkFile;
        ink.talk = talk;
        ink.executeFunction = executeFunction;

        var eventOp = GetComponent<EventOperator>();
        if (eventOp)
        {
            var eventOpCopy = (EventOperator)CopyComponent(eventOp, talk);
            ink.afterStoryOperator = eventOpCopy;
        }

        EventCenter.Broadcast("create_" + gameObject.name + "_talk", talk);

        UIManager.Instance.OpenTalk(talk);
    }

    Component CopyComponent(Component original, GameObject destination)
    {
        System.Type type = original.GetType();
        Component copy = destination.AddComponent(type);
        // Copied fields can be restricted with BindingFlags
        System.Reflection.FieldInfo[] fields = type.GetFields();
        foreach (System.Reflection.FieldInfo field in fields)
        {
            field.SetValue(copy, field.GetValue(original));
        }
        return copy;
    }
}
