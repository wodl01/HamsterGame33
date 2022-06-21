using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class itemInfo
{
    public GameObject itemPanelOb;

    public int itemCode;
    public string itemName;

    public priceType priceType;
    public int itemPrice;
    public Sprite itemThumbNail;

    public int curBuyAmount;
    public int maxBuyAmount;

    public Sprite[] itemImage;
    public float[] itemVariable;
    public List<Text> itemVariableTextInShop;
}
public enum amountType { unVisible , expendable, notExpendable}
[System.Serializable]
public class menuInfo
{
    public amountType amountTextType;
    public string itemType;

    [ArrayElementTitle("itemName")]
    public List<itemInfo> itemInfos = new List<itemInfo>();
}
[System.Serializable]
public class pageInfo
{
    [ArrayElementTitle("itemType")]
    public List<menuInfo> itemMenuInfos = new List<menuInfo>();
    public GameObject[] menuObjects;
}

public class ShopManager : MonoBehaviour
{
    [Header("Scripts")]
    [SerializeField] dongScript manager;
    [SerializeField] SnackManager snackManager;
    [SerializeField] QuestManager questManager;

    [SerializeField] string[] shopMainTitle;
    [SerializeField] int curPage;

    [Header("Objects")]
    [SerializeField] GameObject[] pageObjects;
    [SerializeField] GameObject warningPanel;
    [SerializeField] GameObject askPanel;
    [SerializeField] GameObject selectPanel;
    [SerializeField] GameObject successPanel;

    [Header("Sprites")]
    [SerializeField] Sprite[] selectBtnSprites;

    [Header("Ui")]
    [SerializeField] Image[] selectBtnImage;

    [Header("HamsterWare")]
    [SerializeField] DishScript[] dish;

    public List<pageInfo> itemPageinfos = new List<pageInfo>();
    public List<itemInfo> goodsInfos;


    [SerializeField] itemInfo selectedItemInfo;
    [SerializeField] menuInfo selectedMenuInfo;
    [SerializeField] string selectedItemWare;

    int itemProcedure;

    public void InputShopBtn()
    {
        manager.AddScoreValue(0,priceType.coin);

        for (int i = 0; i < pageObjects.Length; i++)
            pageObjects[i].SetActive(i == curPage ? true : false);
        for (int i = 0; i < itemPageinfos.Count; i++)
        {
            for (int i2 = 0; i2 < itemPageinfos[i].menuObjects.Length; i2++)
                itemPageinfos[i].menuObjects[i2].transform.SetAsFirstSibling();
        }
        

        ItemInfosAllUpdate();
    }

    public void InputPageChangeBtn(int next)
    {
        curPage += next;
        for (int i = 0; i < pageObjects.Length; i++)
        {
            pageObjects[i].SetActive(false);
        }
        if (curPage == -1) curPage = pageObjects.Length - 1;
        else if (curPage == pageObjects.Length) curPage = 0;
        pageObjects[curPage].SetActive(true);

        AudioManager.Play("Touch1");
    }

    public void ResetNextMenuInfos(GameObject menuGroup)
    {
        for (int i = 0; i < menuGroup.transform.childCount; i++)
        {
            menuGroup.transform.GetChild(menuGroup.transform.childCount - 1).gameObject.SetActive(false);
        }
        menuGroup.transform.GetChild(0).gameObject.SetActive(true);
    }
    public void InputNextMenuBtn(GameObject menuGroup)
    {
        List<GameObject> menuChild = new List<GameObject>();
        int onObjectIndex = 0;
        for (int i = 0; i < menuGroup.transform.childCount; i++)
        {
            menuChild.Add(menuGroup.transform.GetChild(i).gameObject);
            if (menuChild[i].activeSelf) onObjectIndex = i;
            menuChild[i].SetActive(false);
        }

        if (onObjectIndex == menuChild.Count - 1) menuChild[0].SetActive(true);
        else menuChild[onObjectIndex + 1].SetActive(true);

        AudioManager.Play("Touch1");
    }
    public void InputItemMenuBtn(string code)
    {
        string[] page = code.Split(' ');//1 3
        itemPageinfos[int.Parse(page[0]) - 1].menuObjects[int.Parse(page[1]) - 1].transform.SetAsLastSibling();

        Debug.Log(int.Parse(page[0]) + "," + int.Parse(page[1]));

        AudioManager.Play("Touch1");
    }

    public void SetItemInfo(string code)
    {
        string[] page = code.Split(' ');//1 3 4
        Debug.Log(page[0] + "." + page[1] + "." + page[2]);
        selectedItemInfo = itemPageinfos[int.Parse(page[0]) - 1].itemMenuInfos[int.Parse(page[1]) - 1].itemInfos[int.Parse(page[2]) - 1];
        selectedMenuInfo = itemPageinfos[int.Parse(page[0]) - 1].itemMenuInfos[int.Parse(page[1]) - 1];
        itemProcedure = int.Parse(page[2]);
    }
    public void CheckCanBuy()
    {
        int priceAmount = selectedItemInfo.priceType == priceType.coin ? manager.GetScoreValue(priceType.coin) : manager.GetScoreValue(priceType.hamTicket);
        Debug.Log(priceAmount);
        if (priceAmount >= selectedItemInfo.itemPrice)
        {
            if (selectedMenuInfo.amountTextType == amountType.expendable)
            {
                InputItemBuyBtn();
            }
            else
            {
                askPanel.transform.GetChild(2).GetComponent<Text>().text = "\"" + selectedItemInfo.itemName + "\"";
                askPanel.SetActive(true);
            }
            AudioManager.Play("Cancel1");
        }
        else
        {
            StartCoroutine(PanelActive(warningPanel));
            AudioManager.Play("Cancel2");
        }


    }
    string finalItemSelect;
    public void InputItemBuyBtn()
    {
        manager.AddScoreValue(-selectedItemInfo.itemPrice, selectedItemInfo.priceType);
        if (selectedItemInfo.maxBuyAmount == 99991)
            manager.AddScoreValue(1, priceType.normalDirTicket);
        else if (selectedItemInfo.maxBuyAmount == 99992)
            manager.AddScoreValue(1, priceType.specialDirTicket);
        else
            selectedItemInfo.curBuyAmount++;


        switch (selectedMenuInfo.amountTextType)
        {
            case amountType.unVisible:
                finalItemSelect = selectedItemInfo.itemName;
                break;
            case amountType.expendable:
                finalItemSelect = selectedItemInfo.itemName + ":" + selectedItemInfo.curBuyAmount;
                break;
            case amountType.notExpendable:
                finalItemSelect = selectedItemInfo.itemName + " (" + selectedItemInfo.curBuyAmount + "/" + selectedItemInfo.maxBuyAmount + ")";
                break;
            default:
                break;
        }

        selectedItemInfo.itemPanelOb.transform.GetChild(0).GetComponent<Text>().text = finalItemSelect;


        if (selectedItemInfo.curBuyAmount == selectedItemInfo.maxBuyAmount)
            selectedItemInfo.itemPanelOb.transform.GetChild(3).gameObject.SetActive(true);
        else
            selectedItemInfo.itemPanelOb.transform.GetChild(3).gameObject.SetActive(false);

        askPanel.SetActive(false);
        successPanel.transform.GetChild(2).GetComponent<Text>().text = selectedItemInfo.itemName;

        if (selectedMenuInfo.amountTextType != amountType.expendable)
            StartCoroutine(PanelActive(successPanel));

        AudioManager.Play("BuyItem");
    }


    public void OpenPanel(GameObject target)
    {
        target.SetActive(true);
        manager.panelOn = true;

        snackManager.PanelClose();
        AudioManager.Play("Touch1");
    }
    public void ClosePanel(GameObject target)
    {
        target.SetActive(false);
        manager.panelOn = false;

        questManager.QuestPanelUpdate();
        AudioManager.Play("Cancel1");
    }
    public void ClosePanelNotTouchPanelOn(GameObject target)
    {
        target.SetActive(false);

        questManager.QuestPanelUpdate();
        AudioManager.Play("Cancel1");
    }

    void ItemInfosAllUpdate()
    {
        for (int i = 0; i < itemPageinfos.Count; i++)
        {
            for (int i2 = 0; i2 < itemPageinfos[i].itemMenuInfos.Count; i2++)
            {
                menuInfo curMenuInfo = itemPageinfos[i].itemMenuInfos[i2];
                for (int i3 = 0; i3 < itemPageinfos[i].itemMenuInfos[i2].itemInfos.Count; i3++)
                {
                    itemInfo curItemInfo = itemPageinfos[i].itemMenuInfos[i2].itemInfos[i3];
                    if (!curItemInfo.itemPanelOb) break;

                    curItemInfo.itemPanelOb.GetComponent<Image>().sprite = curItemInfo.itemThumbNail;

                    switch (curMenuInfo.amountTextType)
                    {
                        case amountType.unVisible:
                            finalItemSelect = curItemInfo.itemName;
                            break;
                        case amountType.expendable:
                            finalItemSelect = curItemInfo.itemName + ":" + curItemInfo.curBuyAmount;
                            break;
                        case amountType.notExpendable:
                            finalItemSelect = curItemInfo.itemName + " (" + curItemInfo.curBuyAmount + "/" + curItemInfo.maxBuyAmount + ")";
                            break;
                        default:
                            break;
                    }
                    curItemInfo.itemPanelOb.transform.GetChild(0).GetComponent<Text>().text = finalItemSelect;

                    if (curItemInfo.itemVariableTextInShop.Count != 0)
                        for (int index = 0; index < curItemInfo.itemVariableTextInShop.Count; index++)
                            curItemInfo.itemVariableTextInShop[index].text = curItemInfo.itemVariable[index].ToString();

                    switch (curItemInfo.priceType)
                    {
                        case priceType.coin: curItemInfo.itemPanelOb.transform.GetChild(1).GetComponent<Text>().text = curItemInfo.itemPrice + " @"; break;
                        case priceType.hamTicket: curItemInfo.itemPanelOb.transform.GetChild(1).GetComponent<Text>().text = curItemInfo.itemPrice + " #"; break;
                        case priceType.won: curItemInfo.itemPanelOb.transform.GetChild(1).GetComponent<Text>().text = curItemInfo.itemPrice + " !"; break;
                    }
                    

                    curItemInfo.itemPanelOb.transform.GetChild(3).gameObject.SetActive(curItemInfo.curBuyAmount == curItemInfo.maxBuyAmount);
                }
            }
        }
    }

    public itemInfo GetItemInfo(int page, int menu, int itemCode)
    {
        itemInfo item = itemPageinfos[page].itemMenuInfos[menu].itemInfos[itemCode];

        return item;
    }
    public itemInfo GetGoodsInfo(int GoodsIndex)
    {
        itemInfo item = goodsInfos[GoodsIndex];

        return item;
    }


    IEnumerator PanelActive(GameObject target)
    {
        target.SetActive(true);
        yield return new WaitForSeconds(1.2f);
        target.SetActive(false);
    }
}
