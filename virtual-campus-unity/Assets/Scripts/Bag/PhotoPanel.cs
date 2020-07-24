using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotoPanel : MonoBehaviour
{
    public static PhotoPanel Instance;

	public Transform elementContainer;
	public GameObject elementDisplayPrefab;

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
}