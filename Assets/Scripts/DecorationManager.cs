using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ColorAskPanel
{
    public GameObject colorAskPanel;
    public Text infoText;
    public Image ticketImage;
    public Text costText;

    public string[] ticketKindString;
    public string[] partKindString;
    public string[] partKindStringEnd;
    public Sprite[] costTicketSprite;

    public int ticketKindIndex;
}

public enum colorTicketType { normal, special, custom, nameChange };

public class DecorationManager : MonoBehaviour
{
    [SerializeField] dongScript gameManager;
    [SerializeField] ShopManager shopManager;
    [SerializeField] Ui_ChangeManager hamsterInfoManager;

    [Header("Inventory")]
    [SerializeField] List<int> headItemList;
    [SerializeField] List<int> faceItemList;
    [SerializeField] List<int> dongItemList;
    [SerializeField] List<int> backItemList;

    [Header("Panels")]
    [SerializeField] GameObject menuPanel;
    [SerializeField] GameObject decoChoicePanel;

    [Header("DecorationPanels")]
    [SerializeField] GameObject[] decorationPanels;

    [Header("AskPanel")]
    [SerializeField] ColorAskPanel colorAsk;

    [Header("HamsterShape")]
    [SerializeField] Sprite nullSprite;
    [SerializeField] Image hamsterBodyImage;
    [SerializeField] Image hamsterSubImage;
    [SerializeField] Image hamsterLineImage;
    [SerializeField] Image hamsterOutlineImage;
    [SerializeField] Text hamsterText;
    [SerializeField] Image hamsterFaceImage;
    [SerializeField] Image hamsterHatImage;
    [SerializeField] Image hamsterAcImage;

    [Header("HamsterColorList")]
    [SerializeField] Color[] mainColorNormal;
    [SerializeField] Color[] mainColorSpecial;
    [SerializeField] Color[] lineColor;

    [Header("HSV_Color")]
    [SerializeField] private HSVPicker.ColorPicker mainCustomcolor;
    [SerializeField] private HSVPicker.ColorSlider mainCustomSlider_H;
    [SerializeField] private HSVPicker.ColorSlider mainCustomSlider_S;
    [SerializeField] private HSVPicker.ColorSlider mainCustomSlider_V;
    [SerializeField] private HSVPicker.ColorPicker subCustomcolor;
    [SerializeField] private HSVPicker.ColorSlider subCustomSlider_H;
    [SerializeField] private HSVPicker.ColorSlider subCustomSlider_S;
    [SerializeField] private HSVPicker.ColorSlider subCustomSlider_V;

    [Header("ColorPanel_Uis")]
    [SerializeField] private Button subOnOffBtn;
    [SerializeField] private Button lineOnOffBtn;
    [SerializeField] private Button[] colorApplyBtns;
    [SerializeField] private GameObject[] changeColorSign;
    [SerializeField] private Color[] colorPanelBtnColors;
    [SerializeField] Text[] ticketAmountText;

    [Header("ItemSelectBtns")]
    [SerializeField] GameObject[] headItemBtns;
    [SerializeField] GameObject[] faceItemBtns;
    [SerializeField] GameObject[] dongItemBtns;

    [SerializeField] GameObject[] colorPanels;
    [SerializeField] GameObject[] mainColorNormalBtns;
    [SerializeField] GameObject[] mainColorSpecialBtns;
    [SerializeField] GameObject mainColorCustomBtns;
    [SerializeField] GameObject subColorCustomBtns;
    [SerializeField] GameObject[] lineColorBtns;

    [Header("CurState")]
    public bool isCustomingMain;
    public bool isCustomingSub;

    HamsterAbility target;

    int hamsterPage;

    private void Start()
    {
    }
    public void OpenHamsterPanel()
    {
        hamsterPage = 0;

        InventoryListUpdate();
        ColorInventoryUpdate();
        HamsterShapeUpdate();

        InputDecorationPanel(0);

        decoChoicePanel.SetActive(false);
        menuPanel.SetActive(false);
    }

    public void InputDecorationPanel(int index)
    {
        for (int i = 0; i < decorationPanels.Length; i++)
        {
            decorationPanels[i].transform.SetAsLastSibling();
        }

        decorationPanels[index].transform.SetAsLastSibling();

        AudioManager.Play("Touch1");
    }

    public void ChangeHamster(int index)
    {
        hamsterPage += index;
        if (hamsterPage == hamsterInfoManager.hamsterList.Count) hamsterPage = 0;
        else if (hamsterPage == -1) hamsterPage = hamsterInfoManager.hamsterList.Count - 1;

        HamsterShapeUpdate();
        InventoryListUpdate();
        ColorInventoryUpdate();
    }
    void HamsterShapeUpdate()
    {
        target = hamsterInfoManager.hamsterList[hamsterPage];
        HamsterInfo curHamInfo = target.hamsterInfo;

        hamsterBodyImage.sprite = target.profileMainSprite;
        hamsterBodyImage.color = curHamInfo.hamsterMainColor;
        hamsterText.text = curHamInfo.name_H;

        hamsterSubImage.color = curHamInfo.hamsterSubColor;
        hamsterLineImage.color = curHamInfo.hamsterLineColor;
        if (target.profileSubSprite && curHamInfo.isShowSub)
        {
            hamsterSubImage.sprite = target.profileSubSprite;

        }
        else
        {
            hamsterSubImage.sprite = nullSprite;
        }
        if (target.profileLineSprite && curHamInfo.isShowLine)
        {
            hamsterLineImage.sprite = target.profileLineSprite;

        }
        else
        {
            hamsterLineImage.sprite = nullSprite;
        }
        if (target.profileOutLineSprite)
            hamsterOutlineImage.sprite = target.profileOutLineSprite;
        else
            hamsterOutlineImage.sprite = nullSprite;

        hamsterFaceImage.sprite = target.profileFaceSprite;

        hamsterHatImage.GetComponent<RectTransform>().localPosition = target.hatVecter;
        hamsterAcImage.GetComponent<RectTransform>().localPosition = target.acVecter;

        if (curHamInfo.hamsterHatCode == 0) hamsterHatImage.sprite = nullSprite;
        else hamsterHatImage.sprite = shopManager.itemPageinfos[2].itemMenuInfos[0].itemInfos[curHamInfo.hamsterHatCode].itemImage[0];
        if (curHamInfo.hamsterAcCode == 0) hamsterAcImage.sprite = nullSprite;
        else hamsterAcImage.sprite = shopManager.itemPageinfos[2].itemMenuInfos[1].itemInfos[curHamInfo.hamsterAcCode].itemImage[0];
    }

    #region Accessory
    void InventoryListUpdate()
    {
        headItemList = new List<int>();
        faceItemList = new List<int>();

        for (int i = 0; i < shopManager.itemPageinfos[2].itemMenuInfos.Count; i++)
        {
            menuInfo menu = shopManager.itemPageinfos[2].itemMenuInfos[i];
            switch (i)
            {
                case 0://head
                    for (int i2 = 0; i2 < menu.itemInfos.Count; i2++)
                    {
                        itemInfo curItemInfo = menu.itemInfos[i2];
                        int amount = curItemInfo.curBuyAmount;
                        for (int i3 = 0; i3 < hamsterInfoManager.hamsterList.Count; i3++)
                        {
                            if (hamsterInfoManager.hamsterList[i3].hamsterInfo.hamsterHatCode == i2) amount--;
                        }
                        headItemBtns[i2].transform.GetChild(2).GetComponent<Image>().sprite = curItemInfo.itemThumbNail;
                        headItemBtns[i2].transform.GetChild(3).gameObject.SetActive(false);
                        if (amount == 0) headItemBtns[i2].transform.GetChild(3).gameObject.SetActive(true);
                        headItemBtns[i2].transform.GetChild(5).GetComponent<Text>().text = amount.ToString();
                        if (curItemInfo.curBuyAmount > 0) headItemList.Add(curItemInfo.itemCode);
                    }
                    break;
                case 1://face
                    for (int i2 = 0; i2 < menu.itemInfos.Count; i2++)
                    {
                        itemInfo curItemInfo = menu.itemInfos[i2];
                        int amount = curItemInfo.curBuyAmount;
                        for (int i3 = 0; i3 < hamsterInfoManager.hamsterList.Count; i3++)
                        {
                            if (hamsterInfoManager.hamsterList[i3].hamsterInfo.hamsterAcCode == i2) amount--;
                        }
                        faceItemBtns[i2].transform.GetChild(2).GetComponent<Image>().sprite = curItemInfo.itemThumbNail;
                        faceItemBtns[i2].transform.GetChild(3).gameObject.SetActive(false);
                        if (amount == 0) faceItemBtns[i2].transform.GetChild(3).gameObject.SetActive(true);
                        faceItemBtns[i2].transform.GetChild(5).GetComponent<Text>().text = amount.ToString();
                        if (curItemInfo.curBuyAmount > 0) faceItemList.Add(curItemInfo.itemCode);
                    }
                    break;
                case 2://dong
                    for (int i2 = 0; i2 < menu.itemInfos.Count; i2++)
                    {
                        itemInfo curItemInfo = menu.itemInfos[i2];
                        int amount = curItemInfo.curBuyAmount;
                        for (int i3 = 0; i3 < hamsterInfoManager.hamsterList.Count; i3++)
                        {
                            if (hamsterInfoManager.hamsterList[i3].hamsterInfo.hamsterDongCode == i2) amount--;
                        }
                        dongItemBtns[i2].transform.GetChild(2).GetComponent<Image>().sprite = curItemInfo.itemThumbNail;
                        dongItemBtns[i2].transform.GetChild(3).gameObject.SetActive(false);
                        if (amount == 0) dongItemBtns[i2].transform.GetChild(3).gameObject.SetActive(true);
                        dongItemBtns[i2].transform.GetChild(5).GetComponent<Text>().text = amount.ToString();
                        if (curItemInfo.curBuyAmount > 0) dongItemList.Add(curItemInfo.itemCode);
                    }
                    break;
            }
        }
        target = hamsterInfoManager.hamsterList[hamsterPage];
        for (int i = 0; i < headItemBtns.Length; i++)//all false
        {
            headItemBtns[i].SetActive(false);
            headItemBtns[i].transform.GetChild(1).gameObject.SetActive(false);//shadow
        }
        for (int i = 0; i < faceItemBtns.Length; i++)//all false
        {
            faceItemBtns[i].SetActive(false);
            faceItemBtns[i].transform.GetChild(1).gameObject.SetActive(false);//shadow
        }
        for (int i = 0; i < dongItemBtns.Length; i++)//all false
        {
            dongItemBtns[i].SetActive(false);
            dongItemBtns[i].transform.GetChild(1).gameObject.SetActive(false);//shadow
        }


        for (int i = 0; i < headItemList.Count; i++)//on target
        {
            headItemBtns[headItemList[i]].SetActive(true);
            if (headItemList[i] == target.hamsterInfo.hamsterHatCode)
                headItemBtns[headItemList[i]].transform.GetChild(1).gameObject.SetActive(true);//shadow
        }
        for (int i = 0; i < faceItemList.Count; i++)//on target
        {
            faceItemBtns[faceItemList[i]].SetActive(true);
            if (faceItemList[i] == target.hamsterInfo.hamsterAcCode)
                faceItemBtns[faceItemList[i]].transform.GetChild(1).gameObject.SetActive(true);//shadow
        }
        for (int i = 0; i < dongItemList.Count; i++)//on target
        {
            dongItemBtns[dongItemList[i]].SetActive(true);
            if (dongItemList[i] == target.hamsterInfo.hamsterDongCode)
                dongItemBtns[dongItemList[i]].transform.GetChild(1).gameObject.SetActive(true);//shadow
        }
    }

    public void InputAcItemBtn(string code)
    {
        string[] page = code.Split(' ');//1 3
        switch (int.Parse(page[0]))
        {
            case 1: //head
                if (headItemBtns[int.Parse(page[1])].transform.GetChild(3).gameObject.activeSelf) return;
                target.hamsterInfo.hamsterHatCode = int.Parse(page[1]);
                break;
            case 2: //face
                if (faceItemBtns[int.Parse(page[1])].transform.GetChild(3).gameObject.activeSelf) return;
                target.hamsterInfo.hamsterAcCode = int.Parse(page[1]);
                break;
            case 3: //dong
                if (dongItemBtns[int.Parse(page[1])].transform.GetChild(3).gameObject.activeSelf) return;
                target.hamsterInfo.hamsterDongCode = int.Parse(page[1]);
                break;
        }
        HamsterShapeUpdate();
        InventoryListUpdate();
    }

    #endregion

    #region Color
    void ColorInventoryUpdate()
    {
        colorPanels[3].SetActive(target.profileSubSprite);
        colorPanels[4].SetActive(target.profileLineSprite);
        subOnOffBtn.gameObject.SetActive(target.profileSubSprite);
        lineOnOffBtn.gameObject.SetActive(target.profileLineSprite);
        if(target.profileSubSprite && target.hamsterInfo.isShowSub) subOnOffBtn.GetComponent<Image>().color = colorPanelBtnColors[0];
        else subOnOffBtn.GetComponent<Image>().color = colorPanelBtnColors[1];
        if (target.profileLineSprite && target.hamsterInfo.isShowLine) lineOnOffBtn.GetComponent<Image>().color = colorPanelBtnColors[0];
        else lineOnOffBtn.GetComponent<Image>().color = colorPanelBtnColors[1];

        float h_M, s_M, v_M;
        float h_S, s_S, v_S;

        Color.RGBToHSV(target.hamsterInfo.hamsterMainColor, out h_M, out s_M, out v_M);
        Color.RGBToHSV(target.hamsterInfo.hamsterSubColor, out h_S, out s_S, out v_S);
        Debug.Log("H: " + h_M + " S: " + s_M + " V: " + v_M);

        mainCustomSlider_H.slider.value = h_M;
        mainCustomSlider_H.SliderClicked(null);
        mainCustomSlider_S.slider.value = s_M;
        mainCustomSlider_S.SliderClicked(null);
        mainCustomSlider_V.slider.value = v_M;
        mainCustomSlider_V.SliderClicked(null);

        subCustomSlider_H.slider.value = h_S;
        subCustomSlider_H.SliderClicked(null);
        subCustomSlider_S.slider.value = s_S;
        subCustomSlider_S.SliderClicked(null);
        subCustomSlider_V.slider.value = v_S;
        subCustomSlider_V.SliderClicked(null);

        isCustomingMain = false;
        isCustomingSub = false;

        mainColorCustomBtns.transform.GetChild(1).gameObject.SetActive(false);
        subColorCustomBtns.transform.GetChild(1).gameObject.SetActive(false);

        ticketAmountText[0].text = "일반 염색" + "X" + gameManager.GetScoreValue(priceType.normalDirTicket);
        ticketAmountText[1].text = "고급 염색" + "X" + gameManager.GetScoreValue(priceType.specialDirTicket);
        ticketAmountText[2].text = "커스텀 염색" + "X" + gameManager.GetScoreValue(priceType.customDirTicket);
        ticketAmountText[3].text = "커스텀 무늬 염색" + "X" + gameManager.GetScoreValue(priceType.customDirTicket);
        ticketAmountText[4].text = "고급 띠 염색" + "X" + gameManager.GetScoreValue(priceType.specialDirTicket);

        for (int i = 0; i < changeColorSign.Length; i++)
        {
            changeColorSign[i].SetActive(false);
        }

        for (int i = 0; i < mainColorNormal.Length; i++)// 일반염색
        {
            mainColorNormalBtns[i].transform.GetChild(0).GetComponent<Image>().color = mainColorNormal[i];
            mainColorNormalBtns[i].transform.GetChild(1).gameObject.SetActive(false);

            if (gameManager.GetScoreValue(priceType.normalDirTicket) == 0)
                mainColorNormalBtns[i].transform.GetChild(2).gameObject.SetActive(true);
            else
                mainColorNormalBtns[i].transform.GetChild(2).gameObject.SetActive(false);
        }
        for (int i = 0; i < mainColorSpecial.Length; i++)//고급 염색
        {
            mainColorSpecialBtns[i].transform.GetChild(0).GetComponent<Image>().color = mainColorSpecial[i];
            mainColorSpecialBtns[i].transform.GetChild(1).gameObject.SetActive(false);

            if (gameManager.GetScoreValue(priceType.specialDirTicket) == 0)
                mainColorSpecialBtns[i].transform.GetChild(2).gameObject.SetActive(true);
            else
                mainColorSpecialBtns[i].transform.GetChild(2).gameObject.SetActive(false);
        }
        for (int i = 0; i < lineColor.Length; i++)//띠 염색
        {
            lineColorBtns[i].transform.GetChild(0).GetComponent<Image>().color = lineColor[i];
            lineColorBtns[i].transform.GetChild(1).gameObject.SetActive(false);

            if (gameManager.GetScoreValue(priceType.specialDirTicket) == 0)
                lineColorBtns[i].transform.GetChild(2).gameObject.SetActive(true);
            else
                lineColorBtns[i].transform.GetChild(2).gameObject.SetActive(false);
        }

        if (gameManager.GetScoreValue(priceType.customDirTicket) == 0)// 커스텀 염색
            mainColorCustomBtns.transform.GetChild(2).gameObject.SetActive(true);
        else
            mainColorCustomBtns.transform.GetChild(2).gameObject.SetActive(false);
        if (gameManager.GetScoreValue(priceType.customDirTicket) == 0)// 커스텀 무늬 염색
            subColorCustomBtns.transform.GetChild(2).gameObject.SetActive(true);
        else
            subColorCustomBtns.transform.GetChild(2).gameObject.SetActive(false);

        for (int i = 0; i < colorApplyBtns.Length; i++)
        {
            colorApplyBtns[i].interactable = false;
        }
    }

    public void InputOnOffBtn(string kind)
    {
        if (kind == "Sub")
        {
            target.hamsterInfo.isShowSub = !target.hamsterInfo.isShowSub;
            if(target.hamsterInfo.isShowSub)
            {
                subOnOffBtn.GetComponent<Image>().color = colorPanelBtnColors[0];

                hamsterSubImage.sprite = target.profileSubSprite;
            }
            else
            {
                subOnOffBtn.GetComponent<Image>().color = colorPanelBtnColors[1];

                hamsterSubImage.sprite = nullSprite;
            }
        }
        else if(kind == "Line")
        {
            target.hamsterInfo.isShowLine = !target.hamsterInfo.isShowLine;
            if (target.hamsterInfo.isShowLine)
            {
                lineOnOffBtn.GetComponent<Image>().color = colorPanelBtnColors[0];

                hamsterLineImage.sprite = target.profileLineSprite;
            }
            else
            {
                lineOnOffBtn.GetComponent<Image>().color = colorPanelBtnColors[1];

                hamsterLineImage.sprite = nullSprite;
            }
        }

        AudioManager.Play("Touch1");
    }

    public void ClickColor(string code)
    {
        string[] a = code.Split(' ');//1 3
        int kindNum = int.Parse(a[0]);
        int index = int.Parse(a[1]);

        if (a[0] == "0" && gameManager.GetScoreValue(priceType.normalDirTicket) == 0) return;
        else if (a[0] == "1" && gameManager.GetScoreValue(priceType.specialDirTicket) == 0) return;
        else if (a[0] == "2" && gameManager.GetScoreValue(priceType.customDirTicket) == 0) return;
        else if (a[0] == "3" && gameManager.GetScoreValue(priceType.customDirTicket) == 0) return;
        else if (a[0] == "4" && gameManager.GetScoreValue(priceType.specialDirTicket) == 0) return;

        for (int i = 0; i < mainColorNormalBtns.Length; i++)
            mainColorNormalBtns[i].transform.GetChild(1).gameObject.SetActive(false);
        for (int i = 0; i < mainColorSpecialBtns.Length; i++)
            mainColorSpecialBtns[i].transform.GetChild(1).gameObject.SetActive(false);
        for (int i = 0; i < lineColorBtns.Length; i++)
            lineColorBtns[i].transform.GetChild(1).gameObject.SetActive(false);

        mainColorCustomBtns.transform.GetChild(1).gameObject.SetActive(false);
        subColorCustomBtns.transform.GetChild(1).gameObject.SetActive(false);

        isCustomingMain = false;
        isCustomingSub = false;

        switch (kindNum)
        {
            case 0:
                mainColorNormalBtns[int.Parse(a[1])].transform.GetChild(1).gameObject.SetActive(true);
                hamsterBodyImage.color = mainColorNormal[index];
                break;
            case 1:
                mainColorSpecialBtns[int.Parse(a[1])].transform.GetChild(1).gameObject.SetActive(true);
                hamsterBodyImage.color = mainColorSpecial[index];
                break;
            case 2:
                mainColorCustomBtns.transform.GetChild(1).gameObject.SetActive(true);
                hamsterBodyImage.color = mainCustomcolor._color;
                isCustomingMain = true;
                break;
            case 3:
                subColorCustomBtns.transform.GetChild(1).gameObject.SetActive(true);
                hamsterSubImage.color = subCustomcolor._color;
                isCustomingSub = true;
                break;
            case 4:
                lineColorBtns[index].transform.GetChild(1).gameObject.SetActive(true);
                hamsterLineImage.color = lineColor[index];
                break;
        }

        if(kindNum < 3)// 몸통 염색
        {
            for (int i = 0; i < 3; i++)
            {
                colorApplyBtns[i].interactable = false;
                changeColorSign[i].SetActive(false);
            }

            if (target.hamsterInfo.hamsterMainColor != hamsterBodyImage.color)
            {
                colorApplyBtns[kindNum].interactable = true;

                changeColorSign[kindNum].SetActive(true);
            }
        }
        else
        {
            switch (kindNum)
            {
                case 3:// 무늬 염색
                    if (target.hamsterInfo.hamsterSubColor != hamsterSubImage.color)
                    {
                        changeColorSign[kindNum].SetActive(true);
                        colorApplyBtns[kindNum].interactable = true;
                    }
                    else
                    {
                        changeColorSign[kindNum].SetActive(false);
                        colorApplyBtns[kindNum].interactable = false;
                    }
                    break;
                case 4:// 무늬 염색
                    if (target.hamsterInfo.hamsterLineColor != hamsterLineImage.color)
                    {
                        changeColorSign[kindNum].SetActive(true);
                        colorApplyBtns[kindNum].interactable = true;
                    }
                    else
                    {
                        changeColorSign[kindNum].SetActive(false);
                        colorApplyBtns[kindNum].interactable = false;
                    }
                    break;
            }
        }

        AudioManager.Play("Touch1");
    }


    public void InputApplyBtn(int num)
    {
        colorAsk.ticketKindIndex = num;
        colorAsk.colorAskPanel.SetActive(true);

        colorAsk.infoText.text =
            "<color=#D98888>" + colorAsk.ticketKindString[num] + "</color>염색권 " +
            "<color=#D98888>1</color>장을 사용하여 " +
            "<color=#D98888>" + colorAsk.partKindString[num] + 
            "</color>"+ colorAsk.partKindStringEnd[num]+" 염색하시겠습니까?";

        colorAsk.ticketImage.sprite = colorAsk.costTicketSprite[num];
        switch (num)
        {
            case 0:
                colorAsk.costText.text = "보유 염색권:         x" + gameManager.GetScoreValue(priceType.normalDirTicket);
                break;
            case 1:
                colorAsk.costText.text = "보유 염색권:         x" + gameManager.GetScoreValue(priceType.specialDirTicket);
                break;
            case 2:
                colorAsk.costText.text = "보유 염색권:         x" + gameManager.GetScoreValue(priceType.customDirTicket);
                break;
            case 3:
                colorAsk.costText.text = "보유 염색권:         x" + gameManager.GetScoreValue(priceType.customDirTicket);
                break;
            case 4:
                colorAsk.costText.text = "보유 염색권:         x" + gameManager.GetScoreValue(priceType.specialDirTicket);
                break;
        }

        AudioManager.Play("Touch1");
    }
    public void InputColorAskApplyBtn(bool isApply)
    {
        if (isApply)
        {
            switch (colorAsk.ticketKindIndex)
            {
                case 0:
                    gameManager.AddScoreValue(-1 ,priceType.normalDirTicket);
                    target.hamsterInfo.hamsterMainColor = hamsterBodyImage.color;
                    break;
                case 1:
                    gameManager.AddScoreValue(-1, priceType.specialDirTicket);
                    target.hamsterInfo.hamsterMainColor = hamsterBodyImage.color;
                    break;
                case 2:
                    gameManager.AddScoreValue(-1, priceType.customDirTicket);
                    target.hamsterInfo.hamsterMainColor = hamsterBodyImage.color;
                    break;
                case 3:
                    gameManager.AddScoreValue(-1, priceType.customDirTicket);
                    target.hamsterInfo.hamsterSubColor = hamsterSubImage.color;
                    break;
                case 4:
                    gameManager.AddScoreValue(-1, priceType.specialDirTicket);
                    target.hamsterInfo.hamsterLineColor = hamsterLineImage.color;
                    break;
            }
            colorAsk.colorAskPanel.SetActive(false);
            changeColorSign[colorAsk.ticketKindIndex].SetActive(false);
            colorApplyBtns[colorAsk.ticketKindIndex].interactable = false;

            ColorInventoryUpdate();
        }
        else
        {
            colorAsk.colorAskPanel.SetActive(false);
        }

        AudioManager.Play("Touch2");
    }

    public void CustomingColor(bool isMain)
    {
        if (isMain && isCustomingMain)
        {
            if(target.hamsterInfo.hamsterMainColor != hamsterBodyImage.color)
            {
                changeColorSign[2].SetActive(true);
                colorApplyBtns[2].interactable = true;
            }
            else
            {
                changeColorSign[2].SetActive(false);
                colorApplyBtns[2].interactable = false;
            }

        }
        else if(!isMain && isCustomingSub)
        {
            if (target.hamsterInfo.hamsterSubColor != hamsterSubImage.color)
            {
                changeColorSign[3].SetActive(true);
                colorApplyBtns[3].interactable = true;
            }
            else
            {
                changeColorSign[3].SetActive(false);
                colorApplyBtns[3].interactable = false;
            }
        }
    }

    #endregion

    public void HamsterShapeUpdateInGame()
    {
        for (int i = 0; i < hamsterInfoManager.hamsterList.Count; i++)
        {
            hamsterInfoManager.hamsterList[i].gameObject.GetComponent<Move>().HamsterShapeUpdate();
        }
    }
}
