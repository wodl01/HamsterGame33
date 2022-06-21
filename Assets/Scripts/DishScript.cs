using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DishScript : MonoBehaviour
{
    public dongScript gameManager;

    public int cageNum;
    public int wareNum;

    public GameObject feed;
    public GameObject sign;


    public bool isFull;
    [SerializeField] private int maxFoodGuage;
    public int foodGuage;

    public int foodPrice;

    [SerializeField] GameObject dishImage;
    public bool isFlip;

    public void Start()
    {
        FlipImage(false);
        FoodImageUpdate();
    }

    public void FlipImage(bool change)
    {
        if (change)
        {
            isFlip = !isFlip;
        }
        if (isFlip) dishImage.transform.localScale = new Vector3(-1, 1, 1);
        else dishImage.transform.localScale = new Vector3(1, 1, 1);
    }

    public void MinusCoin()
    {
        gameManager.SummonMessage(gameObject.transform.position, false, foodPrice, 0);
    }

    public void EatFood()
    {
        foodGuage--;
        FoodImageUpdate();
    }

    public void FillFood()
    {
        foodGuage = maxFoodGuage;
        FoodImageUpdate();
    }

    public void FoodImageUpdate()
    {
        isFull = foodGuage > 0 ? true : false;
        if (foodGuage > 0)
        {
            feed.SetActive(true);
            sign.SetActive(false);
        }
        else
        {
            feed.SetActive(false);
            sign.SetActive(true);
        }
    }
}
