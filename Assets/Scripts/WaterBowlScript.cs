using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBowlScript : MonoBehaviour
{
    public int cageNum;
    public int wareNum;

    [SerializeField] GameObject[] waterOb;

    public GameObject sign;

    [SerializeField] int waterGuage;

    public bool isWater;

    [SerializeField] GameObject waterBowlImage;
    public bool isFlip;

    private void Start()
    {
        FlipImage(false);
        WaterBowlShapeUpdate();
    }

    public void FlipImage(bool change)
    {
        if (change)
        {
            isFlip = !isFlip;
        }
        if (isFlip) waterBowlImage.transform.localScale = new Vector3(-1, 1, 1);
        else waterBowlImage.transform.localScale = new Vector3(1, 1, 1);
    }

    public void DrinkWater()
    {
        waterGuage--;

        WaterBowlShapeUpdate();
    }

    public void WaterBowlShapeUpdate()
    {
        for (int i = 0; i < waterOb.Length; i++)
        {
            waterOb[i].SetActive(false);
        }
        if (waterGuage == 0)
        {
            isWater = false;
            sign.SetActive(true);
        }
        else
            sign.SetActive(false);
        waterOb[waterGuage].SetActive(true);
    }

    public void RefillWaterBowl()
    {
        if (waterGuage == 4) return;
        isWater = true;
        waterGuage = 4;
        WaterBowlShapeUpdate();
        sign.SetActive(false);

        AudioManager.Play("WaterRefill");
    }
}
