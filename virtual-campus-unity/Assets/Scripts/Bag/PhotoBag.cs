using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotoBag : Bag
{
    public static PhotoBag Instance;

    private void Awake()
    {
        Instance = this;
    }

	public void Add(Texture2D photo)
	{
		var display = Instantiate(elementDisplayPrefab);
		display.GetComponentInChildren<RawImage>().texture = photo;
		display.transform.SetParent(elementContainer.transform);
		display.transform.localScale = Vector3.one;

		var asepct = ((float)photo.width) / photo.height;
		var gridLayoutGroup = elementContainer.GetComponent<GridLayoutGroup>();
		var newCellSize = gridLayoutGroup.cellSize;
		newCellSize.x = newCellSize.y * asepct;
		gridLayoutGroup.cellSize = newCellSize;
	}

	public override void Select(Item item, ItemBox itemBox)
    {

    }
}