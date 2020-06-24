using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mapping : MonoBehaviour
{
    private GameObject model;
    public GameObject skinBag;

    /*
    private List<Sprite> sprites;
    private int index;
    */


    void Start()
    {
        /*
        List<GameObject> temp = GameObject.FindGameObjectWithTag("SkinBag").GetComponent<SkinBag>().testItems;
        foreach (GameObject g in temp)
        {
            sprites.Add(g.GetComponent<Item>().image);
        }
        */
        /*
        int index = skinBag.GetComponent<SkinBag>().index;
        GameObject temp = skinBag.GetComponent<SkinBag>().testItems[index];
        model = gameObject;
        if (model != null)
        {
            model.GetComponent<SpriteRenderer>().sprite = temp.GetComponent<SpriteItem>().image;
            //model.GetComponent<SkinnedMeshRenderer>().material.SetTextureScale("_MainTex", new Vector2(5, 5));

        }
        */
    }

    public void ChangeMapping(SkinItem item)
    {
        if (model != null)
        {
            model.GetComponent<SpriteRenderer>().sprite = item.image;
            model.transform.localScale = new Vector3(150/item.image.rect.width, 250/item.image.rect.height, 1);
            //Debug.Log(item.image.rect.width);
        }
    }
}
