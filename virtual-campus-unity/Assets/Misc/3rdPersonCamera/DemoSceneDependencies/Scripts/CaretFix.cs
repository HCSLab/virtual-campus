using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
     
public class CaretFix : UIBehaviour
{     
    public InputField inputField = null;

    new IEnumerator Start()
    {
        yield return null;
        if (inputField == null)
            inputField = GetComponent<InputField>();

        if (inputField != null)
        {
            // Find the child by name. This usually isnt good but is the easiest way for the time being.
            Transform caretGO = inputField.transform.Find(inputField.transform.name + " Input Caret");

            if (caretGO != null)
                caretGO.GetComponent<CanvasRenderer>().SetMaterial(Graphic.defaultGraphicMaterial, Texture2D.whiteTexture);
        }
    }
}
