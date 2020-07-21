using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkinBag : Bag
{
	public static SkinBag Instance;

	[Header("Skin Bag")]
	public GameObject skinPreviewPlayer;
	public Space rotateSpace;
	public float rotateSpeed = 20f;
	public GameObject painterHub;
	public GameObject skinBagRenameCanvas;
	public Button deleteButton;
	public GameObject renamePanel;
	public TMP_InputField nameInput;

	#region Preview Rotation
	private bool rotateLeft = false;
	private bool rotateRight = false;

	public void RotateLeftDown()
	{
		rotateLeft = true;
	}

	public void RotateLeftUp()
	{
		rotateLeft = false;
	}

	public void RotateRightDown()
	{
		rotateRight = true;
	}

	public void RotateRightUp()
	{
		rotateRight = false;
	}

	private void Update()
	{
		if (rotateLeft)
		{
			skinPreviewPlayer.transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime, rotateSpace);
		}
		else if (rotateRight)
		{
			skinPreviewPlayer.transform.Rotate(Vector3.down * rotateSpeed * Time.deltaTime, rotateSpace);
		}
	}
	#endregion

	private void Awake()
	{
		Instance = this;

		foreach (GameObject testSkin in testItems)
		{
			Texture2D tex = testSkin.GetComponent<SkinItem>().texture;
			Sprite sprite = Sprite.Create(
				TextureToTexture2D(tex),
				new Rect(0, 0, tex.width, tex.height),
				new Vector2(0.5f, 0.5f), 1000
				);
			testSkin.GetComponent<SkinItem>().image = sprite;
		}
	}

	Texture2D TextureToTexture2D(Texture texture)
	{
		Texture2D texture2D = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
		RenderTexture currentRT = RenderTexture.active;
		RenderTexture renderTexture = RenderTexture.GetTemporary(texture.width, texture.height, 32);
		Graphics.Blit(texture, renderTexture);

		RenderTexture.active = renderTexture;
		texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
		texture2D.Apply();

		RenderTexture.active = currentRT;
		RenderTexture.ReleaseTemporary(renderTexture);

		return texture2D;
	}

	public override void Add(GameObject newSkin)
	{
		var item = Instantiate(newSkin).GetComponent<Item>();
		item.transform.SetParent(transform);

		var display = Instantiate(elementDisplayPrefab);
		var itemBox = display.GetComponent<SkinBox>();
		itemBox.Init(item);
		itemBoxs.Add(itemBox);
		display.transform.SetParent(elementContainer.transform);
		display.transform.localScale = Vector3.one;
	}


	public override void Select(Item item, ItemBox itemBox)
	{
        base.Select(item, itemBox);
		detailName.text = item.itemName;
		detailDescription.text = item.description;
		currentItem = item;
		currentItemBox = itemBox;
		SkinItem skinItem = (SkinItem)currentItem;
		skinPreviewPlayer.GetComponent<PlayerSkin>().ChangeSkinTexture(skinItem.texture);
		if (!skinItem.customized)
		{
			deleteButton.enabled = false;
			deleteButton.interactable = false;
		}
		else
		{
			deleteButton.enabled = true;
			deleteButton.interactable = true;
		}
	}

	public void ReloadSprites()
	{
		foreach (ItemBox itemBox in itemBoxs)
			itemBox.image.sprite = itemBox.item.image;
	}

	public override void Reload()
	{
		foreach (var box in itemBoxs)
			Destroy(box.gameObject);
		itemBoxs.Clear();
		foreach (var item in testItems)
			Add(item);
	}

	public void UseCurrentlySelectedSkin()
	{
		if (currentItem != null)
		{
			SkinItem skinItem = (SkinItem)currentItem;
			GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerSkin>().ChangeSkinTexture(skinItem.texture);
		}
	}

	public void Paint()
	{
		if (painterHub.activeSelf)
		{
			painterHub.SetActive(false);
		}
		else
		{
			painterHub.SetActive(true);
			gameObject.SetActive(false);
		}
	}

	public void Delete()
	{
		if (currentItemBox != null)
		{
			int index = itemBoxs.IndexOf(currentItemBox);
			Remove(currentItem);
			if (itemBoxs.Count > 0)
			{
				Select(itemBoxs[index - 1].item, itemBoxs[index - 1]);
			}
		}
	}

	public void Rename()
	{
		renamePanel.gameObject.SetActive(true);
	}

	public void RenameSave()
	{
		string name = nameInput.text;
		currentItem.itemName = name;
		currentItem.description = string.Empty;
		currentItemBox.text.text = name;
		nameInput.text = "";
		renamePanel.SetActive(false);
		Select(currentItem, currentItemBox);
	}

	public void RenameCancel()
	{
		nameInput.text = "";
		renamePanel.gameObject.SetActive(false);
	}
}