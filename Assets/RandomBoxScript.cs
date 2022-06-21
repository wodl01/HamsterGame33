using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class RandomBoxInfos
{
    public BoxType boxType;
    //[ArrayElementTitle("itemType")]
    public List<GoodsRoot> goodsType;
}
[System.Serializable]
public class GoodsRoot
{
    [SerializeField] private string itemType;
    public int weight;
    public bool isShopItem;
    public string[] goodsRoot;
}
public enum BoxType { 일반, 고급, 프리미엄 };

public class RandomBoxScript : MonoBehaviour
{
    [Header("Scripts")]
    [SerializeField] private ShopManager shop;
    [SerializeField] private dongScript gameManager;
    [SerializeField] private DecorationManager decoration;

    [Header("Ui")]
    [SerializeField] private GameObject mainUi;
    [SerializeField] private Button xBtnOb;
    [SerializeField] private Image itemThumbNailImage;
    [SerializeField] private Text itemNameText;
    [SerializeField] private Text askInfoText;
    [SerializeField] private Text askPriceText;

    [SerializeField] private Image boxIdleImage;

    [Header("Panels")]
    [SerializeField] private GameObject outingItemPanel;
    [SerializeField] private GameObject askPanel;

    [SerializeField] private GameObject skipBtn;

    [SerializeField] private Animator boxAnimator;

    [Header("Price")]
    [SerializeField] private int normalBoxPrice;
    [SerializeField] private int specialBoxPrice;
    [SerializeField] private int premiumBoxPrice;

    [Header("IdleBoxShape")]
    private Sprite[] curBoxShape;
    [SerializeField] private Sprite[] normalBoxShape;
    [SerializeField] private Sprite[] specialBoxShape;
    [SerializeField] private Sprite[] premiumBoxShape;

    [Header("CurState")]
    [SerializeField] private bool isIdle;

    //[ArrayElementTitle("boxType")]
    [SerializeField] private List<RandomBoxInfos> randomBoxInfos;
    [SerializeField] private List<itemInfo> goodsInfos;

    private int curBoxCode;

    public void OpenRandomBoxPanel()
    {
        gameManager.AddScoreValue(0,priceType.coin);
    }

    public void InputBoxBtn1(int boxCode)
    {
        curBoxCode = boxCode;

        

        string boxName = "";
        string boxPrice = "";
        switch (boxCode)
        {
            case 0:
                if(gameManager.GetScoreValue(priceType.coin) < normalBoxPrice)
                {
                    StartCoroutine(dongScript.instance.WarningPanelActive("코인이 부족합니다"));
                    AudioManager.Play("Cancel1");
                    return;
                }
                boxName = "일반 랜덤박스";
                boxPrice = normalBoxPrice + "@";
                break;
            case 1:
                if (gameManager.GetScoreValue(priceType.hamTicket) < specialBoxPrice)
                {
                    StartCoroutine(dongScript.instance.WarningPanelActive("햄티켓이 부족합니다"));
                    AudioManager.Play("Cancel1");
                    return;
                }
                boxName = "고급 랜덤박스";
                boxPrice = specialBoxPrice + "#";
                break;
            case 2:
                if (gameManager.GetScoreValue(priceType.hamTicket) < premiumBoxPrice)
                {
                    StartCoroutine(dongScript.instance.WarningPanelActive("햄티켓이 부족합니다"));
                    AudioManager.Play("Cancel1");
                    return;
                }
                boxName = "프리미엄 랜덤박스";
                boxPrice = premiumBoxPrice + "#";
                break;
        }
        askInfoText.text = "<color=#D98888>" + boxName + "</color>를 개봉하시겠습니까?";
        askPriceText.text = "-" + boxPrice;

        askPanel.SetActive(true);

        AudioManager.Play("Touch1");
    }

    public void InputApplyBtn(bool isApply)//Start Ani
    {
        if (isApply)
        {
            mainUi.SetActive(false);
            xBtnOb.gameObject.SetActive(false);
            xBtnOb.interactable = false;
            askPanel.SetActive(false);

            skipBtn.SetActive(false);
            switch (curBoxCode)
            {
                case 0:
                    curBoxShape = normalBoxShape;
                    break;
                case 1:
                    curBoxShape = specialBoxShape;
                    break;
                case 2:
                    curBoxShape = premiumBoxShape;
                    break;
            }
            isIdle = true;
            boxAnimator.SetBool(curBoxCode.ToString(),true);

            AudioManager.Play("Touch1");
        }
        else
        {
            askPanel.SetActive(false);

            AudioManager.Play("Cancel1");
        }
    }

    public void InputBoxBtn2()
    {
        itemInfo finalItemInfo = new itemInfo();
        string[] finalString = new string[0];
        bool isShopItem = true;

        RandomBoxInfos curBoxInfo = randomBoxInfos[curBoxCode];
        int randomItemTypeNum = RandomBox(curBoxInfo);
        int randomItemNum = Random.Range(0, curBoxInfo.goodsType[randomItemTypeNum].goodsRoot.Length);
        finalString = curBoxInfo.goodsType[randomItemTypeNum].goodsRoot;
        isShopItem = curBoxInfo.goodsType[randomItemTypeNum].isShopItem;


        if (isShopItem)
        {
            int randomNumN = Random.Range(0, finalString.Length);
            string[] page = finalString[randomNumN].Split(' ');//1 3 4
            finalItemInfo = shop.GetItemInfo(int.Parse(page[0]), int.Parse(page[1]), int.Parse(page[2]));
            finalItemInfo.curBuyAmount++;
        }
        else
        {
            int randomNumN = Random.Range(0, finalString.Length);
            string[] page = finalString[randomNumN].Split(' ');//0 커스텀염색티켓
            finalItemInfo = goodsInfos[int.Parse(page[0])];

            gameManager.AddScoreValue(finalItemInfo.maxBuyAmount, finalItemInfo.priceType);
        }

        itemThumbNailImage.sprite = finalItemInfo.itemThumbNail;
        itemThumbNailImage.SetNativeSize();
        itemNameText.text = finalItemInfo.itemName;

        boxAnimator.SetBool("Open", true);

        AudioManager.Play("OpenChest");
    }

    int RandomBox(RandomBoxInfos curBoxInfo)
    {
        int total = 0;
        int weight = 0;
        int selectNum = 0;

        for (int i = 0; i < curBoxInfo.goodsType.Count; i++)
        {
            total += curBoxInfo.goodsType[i].weight;
        }
        selectNum = Mathf.RoundToInt(total * Random.Range(0.0f, 1.0f));

        for (int i = 0; i < curBoxInfo.goodsType.Count; i++)
        {
            weight += curBoxInfo.goodsType[i].weight;
            if (selectNum <= weight)
            {
                return i;
            }
        }
        return 0;
    }


    public void OpenBoxComplete()
    {
        mainUi.SetActive(true);
        outingItemPanel.SetActive(true);
        xBtnOb.gameObject.SetActive(true);
        xBtnOb.interactable = true;

        boxAnimator.SetBool(curBoxCode.ToString(), false);
        boxAnimator.SetBool("Open", false);

        isIdle = false;

        switch (curBoxCode)
        {
            case 0:
                gameManager.AddScoreValue(-normalBoxPrice, priceType.coin);
                break;
            case 1:
                gameManager.AddScoreValue(-specialBoxPrice, priceType.hamTicket);
                break;
            case 2:
                gameManager.AddScoreValue(-premiumBoxPrice, priceType.hamTicket);
                break;
        }
        gameManager.AddScoreValue(0, priceType.coin);
    }

    public void InputItemPanelXBtn()
    {
        outingItemPanel.SetActive(false);
    }

    public void TouchScreen()
    {
        skipBtn.SetActive(!skipBtn.activeSelf);
    }
    public void SkipBtnInput()
    {
        boxAnimator.SetBool("Open", true);
        InputBoxBtn2();

        AudioManager.Play("Touch1");
    }

    float cycleTime;
    int cycleNum;
    private void FixedUpdate()
    {
        if (isIdle)
        {
            cycleTime += Time.deltaTime;
            if(cycleTime > 0.5f)
            {
                cycleNum++;
                if (cycleNum >= curBoxShape.Length) cycleNum = 0;

                cycleTime = 0;

                boxIdleImage.sprite = curBoxShape[cycleNum];
            }
        }
    }


}
