using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteChanger : MonoBehaviour
{
    [SerializeField] private Image changedImage;

    [SerializeField] private List<Sprite> imageSamples;
    [SerializeField] private float changingDelay;
    private float curChangeDelay;
    private int curImageCount;
    private void FixedUpdate()
    {
        curChangeDelay -= Time.deltaTime;
        if(curChangeDelay < 0)
        {
            curImageCount++;
            if (curImageCount == imageSamples.Count) curImageCount = 0;

            changedImage.sprite = imageSamples[curImageCount];

            curChangeDelay = changingDelay;
        }
    }

}
