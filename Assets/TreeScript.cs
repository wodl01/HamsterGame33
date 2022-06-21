using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TreeScript : MonoBehaviour
{
    [SerializeField] ShopManager shopManager;
    [SerializeField] dongScript gameManager;

    [SerializeField] private int fruitAmount;
    [SerializeField] int maxFruitAmount;
    [SerializeField] int fruitPerMinite;

    [SerializeField] private int curTime_m;
    [SerializeField] private float curTime_s;

    private int passedTime;
    bool timerActive = false;

    [SerializeField] Sprite[] treeSprites;

    [Header("Panels")]
    [SerializeField] GameObject shopPanel;
    [SerializeField] GameObject askPanel;

    [Header("Ui")]
    [SerializeField] SpriteRenderer treeImage;
    [SerializeField] Text leftTimeText;
    [SerializeField] Text fruitAmountText;
    [SerializeField] Text fruitAmountTextInShop;

    [SerializeField] Text askInfomationText;

    [SerializeField] List<itemInfo> itemList;
    [SerializeField] itemInfo selectedItemInfo;
    public void GetLogined(int passedTime_A)
    {
        fruitAmount = PlayerPrefs.GetInt("fruitAmount");
        passedTime = passedTime_A;
        while (passedTime > fruitPerMinite * 60 && fruitAmount != maxFruitAmount)
        {
            passedTime -= fruitPerMinite * 60;
            fruitAmount++;
        }
        curTime_s = passedTime;
        if (fruitAmount == maxFruitAmount)
        {
            curTime_s = 0;
            curTime_m = 0;
            leftTimeText.text = "0:0";
        }
        timerActive = true;
        fruitAmountText.text = fruitAmount.ToString() + "/" + maxFruitAmount.ToString();
        TreeShapeUpdate();
    }

    private void FixedUpdate()
    {
        if(timerActive)
            if (fruitAmount != maxFruitAmount)
            {
                curTime_s += Time.deltaTime;
                leftTimeText.text = (fruitPerMinite - curTime_m - 1).ToString() + ":" + Mathf.Floor(60 - curTime_s).ToString();
                if (curTime_s > 60)
                {
                    curTime_m++;
                    curTime_s -= 60;

                    if (curTime_m == fruitPerMinite)
                    {
                        fruitAmount++;
                        curTime_m = 0;
                        fruitAmountText.text = fruitAmount.ToString() + "/" + maxFruitAmount.ToString();
                        leftTimeText.text = "0:0";
                        ShopPanelUpdate();
                        TreeShapeUpdate();
                    }
                }
            }
    }

    

    public int GetFruitAmount()
    {
        return fruitAmount;
    }
    public int GetTimeValue(int index)
    {
        int finalValue = 0;
        switch (index)
        {
            case 0:
                finalValue = Mathf.RoundToInt(curTime_s);
                break;
            case 1:
                finalValue = curTime_m;
                break;
        }
        return finalValue;
    }

    void TreeShapeUpdate()
    {
        float fruitRatio = (float)fruitAmount / (float)maxFruitAmount;

        if (fruitRatio < 0.1f) treeImage.sprite = treeSprites[0];
        else if (fruitRatio < 0.4f) treeImage.sprite = treeSprites[1];
        else if (fruitRatio < 0.8f) treeImage.sprite = treeSprites[2];
        else treeImage.sprite = treeSprites[3];
    }

    public void ShopPanelUpdate()
    {
        fruitAmountTextInShop.text = "X" + fruitAmount.ToString();
        for (int i = 0; i < itemList.Count; i++)
        {
            itemInfo curItemInfo = itemList[i];
            curItemInfo.itemPanelOb.transform.GetChild(0).GetComponent<Image>().sprite = curItemInfo.itemThumbNail;
            curItemInfo.itemPanelOb.transform.GetChild(1).GetComponent<Text>().text = "X" + curItemInfo.itemPrice;
            curItemInfo.itemPanelOb.transform.GetChild(2).GetComponent<Button>().interactable = fruitAmount >= curItemInfo.itemPrice;
        }
    }

    public void ShopItemOnClick(int itemCode)
    {
        selectedItemInfo = itemList[itemCode];

        askInfomationText.text = "<color=#755C79>앵이나무열매</color>"+ 
            selectedItemInfo.itemPrice +
            "개를 <color=#D98888>" + selectedItemInfo.itemName + "</color>으로 교환 하시겠습니까?";

        askPanel.SetActive(true);
    }


    public void AskPanelClick(bool True)
    {
        if (True)
        {
            fruitAmount -= selectedItemInfo.itemPrice;
            int amount = selectedItemInfo.curBuyAmount;
            switch (selectedItemInfo.priceType)
            {
                case priceType.coin:
                    gameManager.AddScoreValue(amount, priceType.coin);
                    break;
                case priceType.hamTicket:
                    gameManager.AddScoreValue(amount, priceType.hamTicket);
                    break;
                case priceType.feverTicket:
                    gameManager.AddScoreValue(amount, priceType.feverTicket);
                    break;
                    /*
                case priceType.nameTicket:
                    break;
                case priceType.normalDirTicket:
                    break;
                case priceType.specialDirTicket:
                    break;
                case priceType.customDirTicket:
                    break;*/
            }
            fruitAmountText.text = fruitAmount.ToString() + "/" + maxFruitAmount.ToString();
            ShopPanelUpdate();
        }
        askPanel.SetActive(false);
    }
}
