using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowScript : MonoBehaviour
{
    public dongScript gameManager;

    public int cageNum;
    public int wareNum;

    public SpriteMask spriteMask;
    [SerializeField] SpriteRenderer outlineRenderer;
    [SerializeField] SpriteRenderer outsideRenderer;

    public void SetWindow(Sprite windowOutside, Sprite windowOutline)
    {
        spriteMask.sprite = windowOutside;

        outlineRenderer.sprite = windowOutline;
        outsideRenderer.sprite = windowOutside;
    }
}
