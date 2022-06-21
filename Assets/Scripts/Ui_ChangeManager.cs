using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class Ui_ChangeManager : MonoBehaviour
{
    [SerializeField] dongScript gameManager;
    [SerializeField] HandlingManager handlingManager;
    [SerializeField] ShopManager shop;
    [SerializeField] OutSideScript outsideScript;
    [SerializeField] QuestManager questManager;
    [SerializeField] EpisodeManager episodeManager;

    [SerializeField] Sprite nullSprite;

    [Header("Hamster Datas")]
    public HamsterAbility[] hamsterOb;
    public List<HamsterAbility> hamsterList = new List<HamsterAbility>();
    public List<HamsterAbility> cage1HamsterList;
    public List<HamsterAbility> cage2HamsterList;
    public List<HamsterAbility> cage3HamsterList;
    public List<HamsterAbility> outSideHamsterList;
    public HamsterAbility target;

    [Header("Panels")]
    [SerializeField] GameObject levelUpPanel;
    [SerializeField] GameObject changeNamePanel;

    [Header("Ui")]
    [SerializeField] GameObject hamsterInfoPanel;
    [SerializeField] Text cageNumText;
    [SerializeField] Text lvText;

    [SerializeField] Text curExpText;
    [SerializeField] Text maxExpText;
    [SerializeField] Text expPercentText;
    [SerializeField] Slider expBar;
    [SerializeField] Button levelUpBtn;

    [SerializeField] Text nameText;
    [SerializeField] Text kindText;
    [SerializeField] Text moneyText;
    [SerializeField] Image stateImage;
    [SerializeField] Text tiredText;
    [SerializeField] Text cleanText;
    [SerializeField] Text hungryText;
    [SerializeField] Text thirstyText;

    [SerializeField] Text[] curLvText;
    [SerializeField] Text lvUpCostText;
    [SerializeField] Button finalLvUpBtn;

    [SerializeField] Image hamsterMainImage;
    [SerializeField] Image hamsterServeImage;
    [SerializeField] Image hamsterLineImage;
    [SerializeField] Image hamsterOutlineImage;
    [SerializeField] Image faceImage;
    [SerializeField] Image hatImage;
    [SerializeField] Image acImage;
    [SerializeField] Image bodyImage;

    [SerializeField] Button handlingBtn;

    [SerializeField] InputField nickNameInputField;
    [SerializeField] Text nickNameAmountText;
    [SerializeField] Button changeNameApplyBtn;
    public Text handlingTimerText;

    public int page;


    public void GetHamster(int hamsterCode)
    {
        HamsterInfo curHamInfo = hamsterOb[hamsterCode].hamsterInfo;

        if (curHamInfo.iHave) return;
        curHamInfo.iHave = true;
        if (cage1HamsterList.Count != 3) curHamInfo.cageNum = 1;
        else if (cage2HamsterList.Count != 3 && gameManager.cageAmount > 1) curHamInfo.cageNum = 2;
        else if (cage3HamsterList.Count != 3 && gameManager.cageAmount > 2) curHamInfo.cageNum = 3;
        else curHamInfo.cageNum = 0;

        HamsterListUpdate();
    }

    public void HamsterListUpdate()
    {
        hamsterList = new List<HamsterAbility>();

        cage1HamsterList = new List<HamsterAbility>();
        cage2HamsterList = new List<HamsterAbility>();
        cage3HamsterList = new List<HamsterAbility>();
        outSideHamsterList = new List<HamsterAbility>();
        for (int i = 0; i < hamsterOb.Length; i++)
        {
            HamsterAbility curHam = hamsterOb[i];
            if (curHam.hamsterInfo.iHave)
            {
                curHam.gameObject.SetActive(true);
                if (curHam.hamsterInfo.cageNum > 0)
                    hamsterList.Add(hamsterOb[i]);
                else
                    outSideHamsterList.Add(hamsterOb[i]);
                switch (hamsterOb[i].hamsterInfo.cageNum)
                {
                    case 1:
                        cage1HamsterList.Add(curHam);
                        break;
                    case 2:
                        cage2HamsterList.Add(curHam);
                        break;
                    case 3:
                        cage3HamsterList.Add(curHam);
                        break;
                }
            }
        }
        SetAllHamstersInCages();
    }

    Move hamsterScript;
    public void SetAllHamstersInCages()
    {
        for (int i = 0; i < hamsterList.Count; i++)
        {
            HamsterAbility curHam = hamsterList[i];
            curHam.gameObject.SetActive(curHam.hamsterInfo.iHave);
        }
        for (int i = 0; i < hamsterOb.Length; i++)
        {
            Move curHamsterScript = hamsterOb[i].GetComponent<Move>();
            if (curHamsterScript.ability.hamsterInfo.cageNum == 0)// GoOutside
            {
                curHamsterScript.GoToOutSide(true);
                curHamsterScript.transform.position = outsideScript.spawnPoint.position;
            }
            else if(curHamsterScript.ability.hamsterInfo.cageNum > 0)// GoInside
            {
                curHamsterScript.GoToOutSide(false);
            }
        }

        for (int i2 = 0; i2 < gameManager.cageAmount; i2++)
        {
            switch (i2)
            {
                case 0: //cage1
                    for (int i = 0; i < cage1HamsterList.Count; i++)
                    {
                        hamsterScript = cage1HamsterList[i].GetComponent<Move>();
                        //hamsterScript.SetHamsterMovement(false);
                        Vector2 midPos = gameManager.cage_transform[0].position;
                        hamsterScript.SetHansterScript();
                        hamsterScript.transform.position = new Vector2(midPos.x + Random.Range(-4, 4), midPos.y + Random.Range(-8, 2));
                    }
                    break;
                case 1:
                    for (int i = 0; i < cage2HamsterList.Count; i++)
                    {
                        hamsterScript = cage2HamsterList[i].GetComponent<Move>();
                        //hamsterScript.SetHamsterMovement(false);
                        Vector2 midPos = gameManager.cage_transform[1].position;
                        hamsterScript.SetHansterScript();
                        hamsterScript.transform.position = new Vector2(midPos.x + Random.Range(-4, 4), midPos.y + Random.Range(-8, 2));
                    }
                    break;
                case 2:
                    for (int i = 0; i < cage3HamsterList.Count; i++)
                    {
                        hamsterScript = cage3HamsterList[i].GetComponent<Move>();
                        //hamsterScript.SetHamsterMovement(false);
                        Vector2 midPos = gameManager.cage_transform[2].position;
                        hamsterScript.SetHansterScript();
                        hamsterScript.transform.position = new Vector2(midPos.x + Random.Range(-4, 4), midPos.y + Random.Range(-8, 2));
                    }
                    break;
            }
        }
        page = 0;
    }

    public void InfoUpdate()
    {
        if (target == null) return;
        HamsterInfo curHam = target.hamsterInfo;

        cageNumText.text = "Cage" + curHam.cageNum.ToString();
        lvText.text = "Lv." + curHam.lv.ToString();

        maxExpText.text = target.balance.maxExp[curHam.lv].ToString();
        curExpText.text = curHam.curExp.ToString();
        expPercentText.text = Mathf.FloorToInt(((float)curHam.curExp / (float)target.balance.maxExp[curHam.lv]) * 100).ToString() + "%";
        expBar.value = (float)curHam.curExp / (float)target.balance.maxExp[curHam.lv];

        nameText.text = curHam.name_H;
        kindText.text = curHam.kind;
        moneyText.text = target.balance.money[curHam.lv].ToString();

        faceImage.sprite = target.profileFaceSprite;

        hamsterMainImage.sprite = target.profileMainSprite;
        hamsterMainImage.color = curHam.hamsterMainColor;
        if (target.profileSubSprite && curHam.isShowSub)
        {
            hamsterServeImage.sprite = target.profileSubSprite;
            hamsterServeImage.color = curHam.hamsterSubColor;
        }
        else
        {
            hamsterServeImage.sprite = nullSprite;
        }
        if (target.profileLineSprite && curHam.isShowLine)
        {
            hamsterLineImage.sprite = target.profileLineSprite;
            hamsterLineImage.color = curHam.hamsterLineColor;
        }
        else
        {
            hamsterLineImage.sprite = nullSprite;
        }
        if (target.profileOutLineSprite)
            hamsterOutlineImage.sprite = target.profileOutLineSprite;
        else
            hamsterOutlineImage.sprite = nullSprite;

        hatImage.sprite = shop.GetItemInfo(2, 0, curHam.hamsterHatCode).itemImage[0];
        hatImage.GetComponent<RectTransform>().localPosition = target.hatVecter;
        acImage.sprite = shop.GetItemInfo(2, 1, curHam.hamsterAcCode).itemImage[0];
        acImage.GetComponent<RectTransform>().localPosition = target.acVecter;

        levelUpBtn.interactable = curHam.curExp == target.balance.maxExp[curHam.lv] ? true : false;
        if(curHam.curExp < target.balance.maxExp[curHam.lv]) levelUpPanel.SetActive(false);

        if (curHam.curExp == target.balance.maxExp[curHam.lv] &&
            gameManager.GetScoreValue(priceType.coin) >= target.balance.lvUpCost[curHam.lv]) finalLvUpBtn.interactable = true;
        else finalLvUpBtn.interactable = false;

        if(curHam.lv > 7 && handlingManager.handlingCoolOn)
        {
            handlingBtn.interactable = true;
        }
        else
        {
            handlingBtn.interactable = false;
        }
    }

    public bool CheckCanLvUpHamsterExist()
    {
        bool yes = false;

        for (int i = 0; i < hamsterList.Count; i++)
        {
            HamsterInfo curham = hamsterList[i].hamsterInfo;
            if (curham.curExp == hamsterList[i].balance.maxExp[curham.lv])
            {
                yes = true;
                break;
            }
        }

        return yes;
    }

    #region LvUp
    public void OpenLvUpPanel(bool active)
    {
        curLvText[0].text = "Lv." + target.hamsterInfo.lv;
        curLvText[1].text = "Lv." + (target.hamsterInfo.lv + 1);
        lvUpCostText.text = -target.balance.lvUpCost[target.hamsterInfo.lv] + "@";
        InfoUpdate();

        levelUpPanel.SetActive(active);

        if(active) AudioManager.Play("Touch1");
        else AudioManager.Play("Cancel1");
    }
    public void HamsterLevelUp()
    {
        gameManager.AddScoreValue(-target.balance.lvUpCost[target.hamsterInfo.lv], priceType.coin);
        target.hamsterInfo.lv++;
        target.hamsterInfo.curExp = 0;

        InfoUpdate();

        levelUpPanel.SetActive(false);

        questManager.CheckQuestComplete(false);
        episodeManager.HamsterLvEvent();

        gameManager.myHamsterHeartOb.SetActive(CheckCanLvUpHamsterExist());

        AudioManager.Play("LvUp");
    }
    #endregion

    #region ChangeName
    public void OpenNameChangePanel(bool active)
    {
        nickNameInputField.text = "";
        nickNameAmountText.text = "0" + "/" + nickNameInputField.characterLimit.ToString();
        changeNamePanel.SetActive(active);
    }
    private bool IsNickNameCurrectExact()//닉네임 확인
    {
        return Regex.IsMatch(nickNameInputField.text, @"[^0-9a-zA-Z가-힣]");
    }
    public void TypingHamsterName()
    {
        nickNameAmountText.text = nickNameInputField.text.Length.ToString() + "/" + nickNameInputField.characterLimit.ToString();
        
        if (!IsNickNameCurrectExact())
            changeNameApplyBtn.interactable = true;
        else
            changeNameApplyBtn.interactable = false;
    }

    public void ApplyNameChangeBtn()
    {
        target.hamsterInfo.name_H = nickNameInputField.text;
        changeNamePanel.SetActive(false);
        InfoUpdate();
    }

    #endregion

    public void OpenHamsterPanel()
    {
        target = hamsterList[page];
        InfoUpdate();
    }

    public void InputChangeHamsterPanelInfo(int index)
    {
        page += index;
        if (page == hamsterList.Count) page = 0;
        else if (page == -1) page = hamsterList.Count - 1;

        target = hamsterList[page];
        InfoUpdate();
    }


    private void Update()
    {
        if (!hamsterInfoPanel.activeSelf) return;

        tiredText.text = Mathf.Floor(target.hamsterInfo.tired).ToString() + "%";
        cleanText.text = Mathf.Floor(target.hamsterInfo.clean).ToString() + "%";
        hungryText.text = Mathf.Floor(target.hamsterInfo.hungry).ToString() + "%";
        thirstyText.text = Mathf.Floor( target.hamsterInfo.thirsty).ToString() + "%";


    }
}
