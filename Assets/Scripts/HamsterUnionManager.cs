using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class UnionCageHam
{
    public GameObject parent;

    public Image hamsterMainImage;
    public Image hamsterServeImage;
    public Image hamsterLineImage;
    public Image hamsterOutlineImage;

    public Image hamsterFaceImage;
    public Image hamsterHatImage;
    public Image hamsterAcImage;
}

public class HamsterUnionManager : MonoBehaviour
{
    [SerializeField] Ui_ChangeManager hamsterList;
    [SerializeField] ShopManager shop;
    [SerializeField] dongScript gameManager;

    [Header("CurState")]
    [SerializeField] int curCageNum;
    bool isDragging;
    int totalLvValue;

    [Header("Ui")]
    [SerializeField] GameObject tutorialPanel;
    [SerializeField] Text totalLvText;

    [Header("Panels")]
    [SerializeField] GameObject menuPanel;

    [Header("CageInfo")]
    [SerializeField] WallScript[] wallScripts;
    [SerializeField] BeddingScript[] beddingScripts;
    [SerializeField] Image wallImage;
    [SerializeField] Image beddingImage;

    [SerializeField] Sprite nullSprite;

    [Header("Curser")]
    [SerializeField] GameObject mouseOb;
    [SerializeField] Image curserMainImage;
    [SerializeField] Image curserServeImage;
    [SerializeField] Image curserLineImage;
    [SerializeField] Image curserOutlineImage;
    [SerializeField] Image curserFaceImage;
    [SerializeField] Image curserHatImage;
    [SerializeField] Image curserAcImage;

    [SerializeField] Color[] cageColors;

    [Header("SelectedHam")]
    [SerializeField] Image selectedMainImage;
    [SerializeField] Image selectedServeImage;
    [SerializeField] Image selectedLineImage;
    [SerializeField] Image selectedOutlineImage;
    [SerializeField] Image selectedfaceImage;
    [SerializeField] Image selectedHatImage;
    [SerializeField] Image selectedAcImage;
    [SerializeField] Image selectedBodyImage;
    [SerializeField] Text hamsterMessageText;
    [SerializeField] Text hamsterKindText;
    [SerializeField] Text hamsterLvText;
    [SerializeField] Text hamsterNameText;

    [SerializeField] List<UnionCageHam> cageHamsUi;
    List<HamsterAbility> cageHamsterList;

    [SerializeField] GameObject[] changeCagePanels;
    [SerializeField] GameObject[] hamsterInfoBtns;

    [SerializeField] HamsterAbility target;

    private void Start()
    {
        //HamsterListUpdate();
        //hamsterList.SetAllHamstersInCages();
    }

    public void OpenUnionPanel()
    {
        HamsterListUpdate();
        InputHamsterProfile(0);
        InputCagePanel(0);
        menuPanel.SetActive(false);
        mouseOb.SetActive(false);
    }

    public void CloseUnionPanel()
    {
        hamsterList.SetAllHamstersInCages();
        for (int i = 0; i < gameManager.rollorScript.Length; i++)
        {
            gameManager.rollorScript[i].rolling = false;
        }
    }

    public void TutorialPanelActive(bool active) => tutorialPanel.SetActive(active);


    void HamsterListUpdate()
    {
        for (int i = 0; i < hamsterList.hamsterOb.Length; i++)
        {
            hamsterInfoBtns[i].SetActive(false);
            HamsterAbility hamInfo = hamsterList.hamsterOb[i];
            HamsterInfo curHamInfo = hamInfo.hamsterInfo;

            if (curHamInfo.iHave)
            {
                GameObject curBtn = hamsterInfoBtns[i];
                GameObject mainImage = curBtn.transform.GetChild(0).transform.GetChild(0).gameObject;
                GameObject decoInfo = curBtn.transform.GetChild(0).transform.GetChild(1).gameObject;

                curBtn.SetActive(true);

                mainImage.transform.GetChild(0).GetComponent<Image>().sprite = hamInfo.profileMainSprite;
                mainImage.transform.GetChild(0).GetComponent<Image>().color = curHamInfo.hamsterMainColor;
                if (hamInfo.profileSubSprite && curHamInfo.isShowSub)
                {
                    mainImage.transform.GetChild(1).GetComponent<Image>().sprite = hamInfo.profileSubSprite;
                    mainImage.transform.GetChild(1).GetComponent<Image>().color = curHamInfo.hamsterSubColor;
                }
                else
                {
                    mainImage.transform.GetChild(1).GetComponent<Image>().sprite = nullSprite;
                }
                if (hamInfo.profileLineSprite && curHamInfo.isShowLine)
                {
                    mainImage.transform.GetChild(2).GetComponent<Image>().sprite = hamInfo.profileLineSprite;
                    mainImage.transform.GetChild(2).GetComponent<Image>().color = curHamInfo.hamsterLineColor;
                }
                else
                {
                    mainImage.transform.GetChild(2).GetComponent<Image>().sprite = nullSprite;
                }
                if (hamInfo.profileOutLineSprite)
                    mainImage.transform.GetChild(3).GetComponent<Image>().sprite = hamInfo.profileOutLineSprite;
                else
                    mainImage.transform.GetChild(3).GetComponent<Image>().sprite = nullSprite;

                curBtn.transform.GetChild(2).GetComponent<Text>().text = curHamInfo.name_H;
                curBtn.transform.GetChild(3).GetComponent<Text>().text = "Lv." + curHamInfo.lv.ToString();

                decoInfo.transform.GetChild(0).GetComponent<Image>().sprite = hamInfo.profileFaceSprite;
                decoInfo.transform.GetChild(1).GetComponent<Image>().sprite = shop.GetItemInfo(2, 1, curHamInfo.hamsterAcCode).itemImage[0];
                decoInfo.transform.GetChild(1).GetComponent<RectTransform>().localPosition = hamInfo.acVecter;
                decoInfo.transform.GetChild(2).GetComponent<Image>().sprite = shop.GetItemInfo(2, 0, curHamInfo.hamsterHatCode).itemImage[0];
                decoInfo.transform.GetChild(2).GetComponent<RectTransform>().localPosition = hamInfo.hatVecter;

            }
        }
        for (int i = 0; i < hamsterList.hamsterOb.Length; i++)
        {
            HamsterAbility hamInfo = hamsterList.hamsterOb[i];
            HamsterInfo curHamInfo = hamInfo.hamsterInfo;
            GameObject curBtn = hamsterInfoBtns[i];
            if (curHamInfo.cageNum > 0)
            {
                curBtn.transform.GetChild(1).GetComponent<Image>().color = cageColors[curHamInfo.cageNum - 1];
                curBtn.transform.GetChild(1).transform.GetChild(0).GetComponent<Text>().text = curHamInfo.cageNum.ToString();
                curBtn.transform.GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                curBtn.transform.GetChild(1).gameObject.SetActive(false);
            }
        }
        totalLvText.text = "Total:Lv." + GetTotalLv();
    }
    public int GetTotalLv()
    {
        int finalIndex = 0;
        for (int i = 0; i < hamsterList.hamsterOb.Length; i++)
        {
            if (hamsterList.hamsterOb[i].hamsterInfo.iHave)
            {
                HamsterAbility hamInfo = hamsterList.hamsterOb[i];
                finalIndex += hamInfo.hamsterInfo.lv;
            }
        }

        return finalIndex;
    }

    public void InputHamsterProfile(int hamsterCode)
    {
        target = hamsterList.hamsterOb[hamsterCode];
        HamsterInfo curHamInfo = target.hamsterInfo;

        selectedMainImage.sprite = target.profileMainSprite;
        selectedMainImage.color = curHamInfo.hamsterMainColor;
        if (target.profileSubSprite && curHamInfo.isShowSub)
        {
            selectedServeImage.sprite = target.profileSubSprite;
            selectedServeImage.color = curHamInfo.hamsterSubColor;
        }
        else
        {
            selectedServeImage.sprite = nullSprite;
        }
        if (target.profileLineSprite && curHamInfo.isShowLine)
        {
            selectedLineImage.sprite = target.profileLineSprite;
            selectedLineImage.color = curHamInfo.hamsterLineColor;
        }
        else
        {
            selectedLineImage.sprite = nullSprite;
        }
        if (target.profileOutLineSprite)
            selectedOutlineImage.sprite = target.profileOutLineSprite;
        else
            selectedOutlineImage.sprite = nullSprite;

        selectedHatImage.sprite = shop.GetItemInfo(2, 0, curHamInfo.hamsterHatCode).itemImage[0];
        selectedHatImage.GetComponent<RectTransform>().localPosition = target.hatVecter;
        selectedAcImage.sprite = shop.GetItemInfo(2, 1, curHamInfo.hamsterAcCode).itemImage[0];
        selectedAcImage.GetComponent<RectTransform>().localPosition = target.acVecter;

        selectedfaceImage.sprite = target.profileFaceSprite;

        hamsterNameText.text = curHamInfo.name_H;
        hamsterLvText.text = "Lv." + curHamInfo.lv.ToString();
        hamsterKindText.text = curHamInfo.kind;
        hamsterMessageText.text = curHamInfo.message;
    }

    public void InputHamsterImageInCage(int hamsterNum)
    {
        switch (curCageNum)
        {
            case 0:
                if(hamsterList.cage1HamsterList.Count >= hamsterNum + 1)
                    hamsterList.cage1HamsterList[hamsterNum].hamsterInfo.cageNum = 0;
                break;
            case 1:
                if (hamsterList.cage2HamsterList.Count >= hamsterNum + 1)
                    hamsterList.cage2HamsterList[hamsterNum].hamsterInfo.cageNum = 0;
                break;
            case 2:
                if (hamsterList.cage3HamsterList.Count >= hamsterNum + 1)
                    hamsterList.cage3HamsterList[hamsterNum].hamsterInfo.cageNum = 0;
                break;
        }
        hamsterList.HamsterListUpdate();
        InputCagePanel(curCageNum);
        HamsterListUpdate();

        AudioManager.Play("Cancel1");
    }
    public void AddingHamsterInCage()
    {
        if (target.hamsterInfo.cageNum == 0)
            switch (curCageNum)
            {
                case 0:
                    if (hamsterList.cage1HamsterList.Count != 3)
                        target.hamsterInfo.cageNum = curCageNum + 1;
                    break;
                case 1:
                    if (hamsterList.cage2HamsterList.Count != 3)
                        target.hamsterInfo.cageNum = curCageNum + 1;
                    break;
                case 2:
                    if (hamsterList.cage3HamsterList.Count != 3)
                        target.hamsterInfo.cageNum = curCageNum + 1;
                    break;
            }
        hamsterList.HamsterListUpdate();
        InputCagePanel(curCageNum);
        HamsterListUpdate();

        AudioManager.Play("Touch1");
    }

    public void InputCagePanel(int cageNum)
    {
        curCageNum = 0;
        switch (cageNum)
        {
            case 0:
                curCageNum = 0;
                cageHamsterList = hamsterList.cage1HamsterList;

                changeCagePanels[0].transform.SetSiblingIndex(2);
                changeCagePanels[1].transform.SetSiblingIndex(1);
                changeCagePanels[2].transform.SetSiblingIndex(0);

                AudioManager.Play("Touch1");
                break;
            case 1:
                if(gameManager.cageAmount > 1)
                {
                    curCageNum = 1;
                    cageHamsterList = hamsterList.cage2HamsterList;

                    changeCagePanels[0].transform.SetSiblingIndex(0);
                    changeCagePanels[1].transform.SetSiblingIndex(2);
                    changeCagePanels[2].transform.SetSiblingIndex(1);

                    AudioManager.Play("Touch1");
                }
                else AudioManager.Play("Cancel2");
                break;
            case 2:
                if(gameManager.cageAmount > 2)
                {
                    curCageNum = 2;
                    cageHamsterList = hamsterList.cage3HamsterList;

                    changeCagePanels[0].transform.SetSiblingIndex(0);
                    changeCagePanels[1].transform.SetSiblingIndex(1);
                    changeCagePanels[2].transform.SetSiblingIndex(2);

                    AudioManager.Play("Touch1");
                }
                else AudioManager.Play("Cancel2");
                break;
        }

        wallImage.sprite = shop.GetItemInfo(1, 1, wallScripts[curCageNum].wareNum).itemImage[0];
        beddingImage.sprite = shop.GetItemInfo(1, 0, beddingScripts[curCageNum].wareNum).itemImage[0];

        for (int i = 0; i < 3; i++)
        {
            cageHamsUi[i].parent.gameObject.SetActive(false);
        }
        for (int i = 0; i < cageHamsterList.Count; i++)
        {
            HamsterAbility target = cageHamsterList[i];
            HamsterInfo curHamInfo = target.hamsterInfo;

            if (target.profileOutLineSprite)
                cageHamsUi[i].hamsterOutlineImage.sprite = target.profileOutLineSprite;
            else
                cageHamsUi[i].hamsterOutlineImage.sprite = nullSprite;

            cageHamsUi[i].hamsterMainImage.sprite = target.profileMainSprite;
            cageHamsUi[i].hamsterMainImage.color = curHamInfo.hamsterMainColor;
            if (target.profileSubSprite && curHamInfo.isShowSub)
            {
                cageHamsUi[i].hamsterServeImage.sprite = target.profileSubSprite;
                cageHamsUi[i].hamsterServeImage.color = curHamInfo.hamsterSubColor;
            }
            else
            {
                cageHamsUi[i].hamsterServeImage.sprite = nullSprite;
            }
            if (target.profileLineSprite && curHamInfo.isShowLine)
            {
                cageHamsUi[i].hamsterLineImage.sprite = target.profileLineSprite;
                cageHamsUi[i].hamsterLineImage.color = curHamInfo.hamsterLineColor;
            }
            else
            {
                cageHamsUi[i].hamsterLineImage.sprite = nullSprite;
            }

            cageHamsUi[i].hamsterFaceImage.sprite = target.profileFaceSprite;

            cageHamsUi[i].hamsterHatImage.sprite = shop.GetItemInfo(2, 0, curHamInfo.hamsterHatCode).itemImage[0];
            cageHamsUi[i].hamsterHatImage.GetComponent<RectTransform>().localPosition = target.hatVecter;
            cageHamsUi[i].hamsterAcImage.sprite = shop.GetItemInfo(2, 1, curHamInfo.hamsterAcCode).itemImage[0];
            cageHamsUi[i].hamsterAcImage.GetComponent<RectTransform>().localPosition = target.acVecter;

            cageHamsUi[i].parent.gameObject.SetActive(true);
        }

    }

    public void DraggingHamsterStart()
    {
        HamsterInfo curHamInfo = target.hamsterInfo;
        if (target.profileOutLineSprite)
            curserOutlineImage.sprite = target.profileOutLineSprite;
        else
            curserOutlineImage.sprite = nullSprite;

        curserMainImage.sprite = target.profileMainSprite;
        curserMainImage.color = curHamInfo.hamsterMainColor;
        if (target.profileSubSprite && curHamInfo.isShowSub)
        {
            curserServeImage.sprite = target.profileSubSprite;
            curserServeImage.color = curHamInfo.hamsterSubColor;
        }
        else
        {
            curserServeImage.sprite = nullSprite;
        }
        if (target.profileLineSprite && curHamInfo.isShowLine)
        {
            curserLineImage.sprite = target.profileLineSprite;
            curserLineImage.color = curHamInfo.hamsterLineColor;
        }
        else
        {
            curserLineImage.sprite = nullSprite;
        }

        curserFaceImage.sprite = target.profileFaceSprite;

        curserHatImage.sprite = shop.GetItemInfo(2, 0, curHamInfo.hamsterHatCode).itemImage[0];
        curserHatImage.GetComponent<RectTransform>().localPosition = target.hatVecter;
        curserAcImage.sprite = shop.GetItemInfo(2, 1, curHamInfo.hamsterAcCode).itemImage[0];
        curserAcImage.GetComponent<RectTransform>().localPosition = target.acVecter;

        isDragging = true;
        mouseOb.SetActive(true);
    }

    public void DraggingHamsterEnd()
    {
        isDragging = false;
        mouseOb.SetActive(false);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D[] hit = Physics2D.GetRayIntersectionAll(ray, Mathf.Infinity);
        for (int i = 0; i < hit.Length; i++)
        {
            if (hit[i].collider.tag == "Manager")
            {
                AddingHamsterInCage();
                break;
            }
        }
    }

    private void Update()
    {
        if (isDragging)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseOb.GetComponent<RectTransform>().position = mousePos;
        }
    }
}
