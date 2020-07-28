using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemPanel : MonoBehaviour
{
	// 物品三类：item、skin、real world photo
	// item只能看预览，skin可以预览+装备，real world photo可以预览+大图预览

	public enum ItemType
	{
		item,
		skin,
		realWorldPhoto
	};

	public static ItemPanel Instance;

	public Transform elementContainer;

	[Header("Item")]
	public GameObject itemDisplayPrefab;
	public GameObject itemRight;
	public TextMeshProUGUI itemNameText, itemDescriptionText;
	public Image itemIcon;

	[Header("Skin")]
	public GameObject skinDisplayPrefab;
	public GameObject skinRight;
	public TextMeshProUGUI skinNameText;
	public PlayerSkin player, previewPlayer;
	public float rotateSpeed;
	public SkinScriptableObject[] skins;

	[Header("Real World Photo")]
	public GameObject realWorldPhotoDisplayPrefab;
	public GameObject realWorldPhotoRight;
	public TextMeshProUGUI realWorldPhotoNameText, realWorldPhotoDescriptionText;
	public Image realWorldPhotoPreview;

	Dictionary<string, GameObject> nameToItemDisplay = new Dictionary<string, GameObject>();
	SkinScriptableObject currentSelectedSkin;
	RealWorldPhotoScriptableObject currentSelectedPhoto;

	[Header("Item Skin Photo ScriptableObject List")]
	public List<ItemScriptableObject> itemList = new List<ItemScriptableObject>();
	public List<SkinScriptableObject> skinList = new List<SkinScriptableObject>();
	public List<RealWorldPhotoScriptableObject> photoList = new List<RealWorldPhotoScriptableObject>();

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		foreach (var skin in skins)
			AddSkin(skin);
	}

	private void OnEnable()
	{
		itemRight.SetActive(false);
		skinRight.SetActive(false);
		realWorldPhotoRight.SetActive(false);
	}

	GameObject InstantiateDisplayAndAddToContainer(GameObject prefab)
	{
		var instance = Instantiate(prefab);
		instance.transform.SetParent(elementContainer);
		instance.transform.localScale = Vector3.one;
		return instance;
	}


	#region Item
	public void AddItem(ItemScriptableObject item)
	{
		var itemDisplay = InstantiateDisplayAndAddToContainer(itemDisplayPrefab);
		itemDisplay.GetComponent<ItemDisplay>().Initialize(item);
		nameToItemDisplay[item.name] = itemDisplay;
	}

	public void AddItem(string itemName)
	{
		foreach (var item in itemList)
		{
			if (item.name == itemName)
			{
				AddItem(item);
				break;
			}
		}
	}

	public void ShowItem(ItemScriptableObject item)
	{
		itemRight.SetActive(true);
		skinRight.SetActive(false);
		realWorldPhotoRight.SetActive(false);
		itemIcon.sprite = item.icon;
		itemNameText.text = item.name;
		itemDescriptionText.text = item.description;
	}

	public void RemoveItem(ItemScriptableObject item)
	{
		RemoveItem(item.name);
	}

	public void RemoveItem(string itemName)
	{
		if (nameToItemDisplay.ContainsKey(itemName))
		{
			var itemToRemove = nameToItemDisplay[itemName];
			nameToItemDisplay.Remove(itemName);
			Destroy(itemToRemove);
		}
	}
	#endregion

	#region Skin
	public void AddSkin(SkinScriptableObject skin)
	{
		var skinDisplay = InstantiateDisplayAndAddToContainer(skinDisplayPrefab);
		skinDisplay.GetComponent<SkinDisplay>().Initialize(skin);
	}

	public void AddSkin(string skinName)
	{
		foreach (var skin in skinList)
		{
			if (skin.name == skinName)
			{
				AddSkin(skin);
				break;
			}
		}
	}

	public void ShowSkin(SkinScriptableObject skin)
	{
		itemRight.SetActive(false);
		skinRight.SetActive(true);
		realWorldPhotoRight.SetActive(false);
		skinNameText.text = skin.name;
		currentSelectedSkin = skin;
		previewPlayer.ChangeSkinTexture(skin.skinTexture);
	}

	public void UseSkin()
	{
		player.ChangeSkinTexture(currentSelectedSkin.skinTexture);
	}
	#endregion

	#region Real World Photo
	public void AddPhoto(RealWorldPhotoScriptableObject photo)
	{
		var photoDisplay = InstantiateDisplayAndAddToContainer(realWorldPhotoDisplayPrefab);
		photoDisplay.GetComponent<RealWorldPhotoDisplay>().Initialize(photo);
	}

	public void AddPhoto(string photoName)
	{
		foreach (var photo in photoList)
		{
			if (photo.name == photoName)
			{
				AddPhoto(photo);
				break;
			}
		}
	}

	public void ShowPhoto(RealWorldPhotoScriptableObject photo)
	{
		itemRight.SetActive(false);
		skinRight.SetActive(false);
		realWorldPhotoRight.SetActive(true);
		realWorldPhotoNameText.text = photo.name;
		realWorldPhotoDescriptionText.text = photo.description;
		realWorldPhotoPreview.sprite = photo.photo;

		// Reset the aspect of the preview image container
		// to fit the aspect ratio of the photo.
		var aspect = (float)photo.photo.rect.width / photo.photo.rect.height;
		realWorldPhotoPreview.GetComponent<AspectRatioFitter>().aspectRatio = aspect;

		currentSelectedPhoto = photo;
	}

	public void ViewPhoto()
	{
		PhotoView.Instance.ShowPhoto(currentSelectedPhoto.photo);
	}
	#endregion

	#region Skin Preview Rotation
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
			previewPlayer.transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime, Space.World);
		}
		else if (rotateRight)
		{
			previewPlayer.transform.Rotate(Vector3.down * rotateSpeed * Time.deltaTime, Space.World);
		}
	}
	#endregion
}
