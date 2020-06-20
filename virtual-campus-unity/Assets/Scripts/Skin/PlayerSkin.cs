using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkin : MonoBehaviour
{
    public Texture2D playerTexture;
    public Material playerMaterial;

    private void Start()
    {
        playerMaterial.mainTexture = playerTexture;
    }
}
