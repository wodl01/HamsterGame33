using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ShopItemCheck
{
    public string checkingItemRoot;
    public int checkingBuyAmount;
}
[System.Serializable]
public class WareCodeCheck
{
    public WareType wareType;
    public int cageNum;
}

[System.Serializable]
public class HamsterConditionCheck
{
    public HamsterConditionType conditionType;
    public HamsterAbility ability;
    public int checkIndex;
}

[System.Serializable]
public class DongEventCheck
{
    public int nothing;
}

public enum WareType { dish, water, bath, rollor }
public enum HamsterConditionType { happy, lv, hungry, thirsty }
[System.Serializable]
public class Quest
{
    public bool isComplete;
    public string title;
    public string information;

    public List<ShopItemCheck> shopItemCheck;
    public List<WareCodeCheck> hamsterWareCheck;
    public List<DongEventCheck> hamsterDongClick;
    public List<HamsterConditionCheck> hamsterConditionCheck;
}

[System.Serializable]
public class QuestGroup
{
    public List<Quest> questInfos;

    public bool isGiftPrice;
    public string giftName;
    public string giftRoot;
    public string giftTipInfo;
}

public class QuestManager : MonoBehaviour
{
    [Header("Manager")]
    [SerializeField] private dongScript gameManager;
    [SerializeField] private ShopManager shopManager;

    public List<QuestGroup> questInfos;

    [Header("Panels")]
    [SerializeField] private GameObject questPanel;
    [SerializeField] private GameObject giftPanel;
    [SerializeField] private GameObject questInfoPanel;
    [SerializeField] private GameObject[] questPanels;
    [SerializeField] private GameObject giftGetButtonOb;

    [Header("Ui")]
    [SerializeField] private Text[] titleText;
    [SerializeField] private Text informationText;
    [SerializeField] private Image[] IconImage;
    [SerializeField] private Image giftThumbnailImage;
    [SerializeField] private Text giftNameText;
    [SerializeField] private Text giftTipText;

    [SerializeField] private Sprite[] iconSprites;

    [Header("Animation")]
    [SerializeField] Animator[] ani;

    [Header("Statue")]
    [SerializeField] private int questIndex;
    [SerializeField] private bool[] isOpenQuest;


    public void QuestPanelUpdate()
    {
        questInfoPanel.SetActive(false);
        if (questIndex != questInfos.Count)
        {
            giftGetButtonOb.SetActive(false);

            for (int i = 0; i < questInfos[questIndex].questInfos.Count; i++)
            {
                Quest curQuest = questInfos[questIndex].questInfos[i];

                titleText[i].text = curQuest.title;
            }
            CheckQuestComplete(false);
            questPanel.SetActive(true);
        }
        else
            questPanel.SetActive(false);

    }
    public void ClickQuestPanel(int index)
    {
        isOpenQuest[index] = !isOpenQuest[index];
        ani[index].SetBool("Open", isOpenQuest[index]);

        informationText.text = questInfos[questIndex].questInfos[index].information;

        if (!isOpenQuest[0] && !isOpenQuest[1]) questInfoPanel.SetActive(false);
        else questInfoPanel.SetActive(true);
    }


    public void CheckQuestComplete(bool isDongEvent)
    {
        if (questIndex == questInfos.Count)
            return;
        for (int i = 0; i < questPanels.Length; i++)
            questPanels[i].SetActive(false);

        int completeQuestAmount = 0;

        for (int i = 0; i < questInfos[questIndex].questInfos.Count; i++)
        {
            List<bool> checkingQuestCompletion = new List<bool>();
            Quest curQuest = questInfos[questIndex].questInfos[i];
            questPanels[i].SetActive(true);

            if (!curQuest.isComplete)
            {
                if (curQuest.shopItemCheck.Count > 0)
                {
                    for (int i2 = 0; i2 < curQuest.shopItemCheck.Count; i2++)
                    {
                        string[] page = curQuest.shopItemCheck[i2].checkingItemRoot.Split(' ');//1 3 4
                        itemInfo curitemInfo = shopManager.GetItemInfo(int.Parse(page[0]), int.Parse(page[1]), int.Parse(page[2]));

                        if (curitemInfo.curBuyAmount >= curQuest.shopItemCheck[i2].checkingBuyAmount) checkingQuestCompletion.Add(true);
                    }
                }
                if (curQuest.hamsterWareCheck.Count > 0)
                {
                    for (int i2 = 0; i2 < curQuest.hamsterWareCheck.Count; i2++)
                    {
                        WareCodeCheck curWareCheck = curQuest.hamsterWareCheck[i2];
                        switch (curWareCheck.wareType)
                        {
                            case WareType.dish:
                                if (gameManager.dishscript[curWareCheck.cageNum].wareNum > -1)
                                    checkingQuestCompletion.Add(true);
                                break;
                            case WareType.water:
                                if (gameManager.waterBowlScript[curWareCheck.cageNum].wareNum > -1)
                                    checkingQuestCompletion.Add(true);
                                break;
                            case WareType.bath:
                                if (gameManager.bathScript[curWareCheck.cageNum].bathWareNum > -1)
                                    checkingQuestCompletion.Add(true);
                                break;
                            case WareType.rollor:
                                if (gameManager.rollorScript[curWareCheck.cageNum].wareNum > -1)
                                    checkingQuestCompletion.Add(true);
                                break;
                        }
                    }
                }
                if (curQuest.hamsterDongClick.Count > 0)
                {
                    if (isDongEvent)
                    {
                        checkingQuestCompletion.Add(true);
                        Debug.Log("DongEventActive");
                    }

                }
                if (curQuest.hamsterConditionCheck.Count > 0)
                {
                    for (int i2 = 0; i2 < curQuest.hamsterConditionCheck.Count; i2++)
                    {
                        HamsterConditionCheck curConditionCheck = curQuest.hamsterConditionCheck[i2];
                        
                        switch (curConditionCheck.conditionType)
                        {
                            case HamsterConditionType.happy:
                                if (curConditionCheck.ability.hamsterInfo.curExp > curConditionCheck.checkIndex)
                                    checkingQuestCompletion.Add(true);
                                break;
                            case HamsterConditionType.lv:
                                if (curConditionCheck.ability.hamsterInfo.lv > curConditionCheck.checkIndex)
                                    checkingQuestCompletion.Add(true);
                                break;
                            case HamsterConditionType.hungry:
                                if (curConditionCheck.ability.hamsterInfo.hungry > curConditionCheck.checkIndex)
                                    checkingQuestCompletion.Add(true);
                                break;
                            case HamsterConditionType.thirsty:
                                if (curConditionCheck.ability.hamsterInfo.thirsty > curConditionCheck.checkIndex)
                                    checkingQuestCompletion.Add(true);
                                break;
                        }
                    }
                }
                

                int maxCheckingIndex = curQuest.shopItemCheck.Count + curQuest.hamsterWareCheck.Count + curQuest.hamsterConditionCheck.Count + curQuest.hamsterDongClick.Count;

                if (maxCheckingIndex == checkingQuestCompletion.Count)
                {
                    IconImage[i].sprite = iconSprites[1];
                    curQuest.isComplete = true;
                }
                else IconImage[i].sprite = iconSprites[0];

            }
        }
        for (int i = 0; i < questInfos[questIndex].questInfos.Count; i++)
            if (questInfos[questIndex].questInfos[i].isComplete) completeQuestAmount++;

        if (completeQuestAmount == questInfos[questIndex].questInfos.Count)
            giftGetButtonOb.SetActive(true);
    }

    public void SettingGiftPanel()
    {
        string[] page = questInfos[questIndex].giftRoot.Split(' '); //1 3 4
        QuestGroup curQuest = questInfos[questIndex];
        itemInfo curItemInfo = null;
        switch (curQuest.isGiftPrice)
        {
            case true:
                curItemInfo = shopManager.GetGoodsInfo(int.Parse(page[0]));
                break;
            case false:
                curItemInfo = shopManager.GetItemInfo(int.Parse(page[0]), int.Parse(page[1]), int.Parse(page[2]));
                break;
        }
        giftThumbnailImage.sprite = curItemInfo.itemThumbNail;
        giftNameText.text = curQuest.giftName;
        giftTipText.text = curQuest.giftTipInfo;

        giftPanel.SetActive(true);

        AudioManager.Play("QuestComplete");
    }
    public void GetCompensation()
    {
        string[] page = questInfos[questIndex].giftRoot.Split(' '); //1 3 4
        QuestGroup curQuest = questInfos[questIndex];
        switch (curQuest.isGiftPrice)
        {
            case true:
                itemInfo curItemInfo = shopManager.GetGoodsInfo(int.Parse(page[0]));
                gameManager.AddScoreValue(curItemInfo.maxBuyAmount, curItemInfo.priceType);
                break;
        }
        giftPanel.SetActive(false);
        questIndex++;

        QuestPanelUpdate();

        AudioManager.Play("Cancel1");
    }

    public void SetQuestIndex(int index)
    {
        questIndex = index;
    }
    public int GetQuestIndex()
    {
        return questIndex;
    }
}
