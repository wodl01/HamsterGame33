using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SnackManager : MonoBehaviour
{
    [SerializeField] dongScript manager;
    [SerializeField] Ui_ChangeManager uiChangeManager;

    [SerializeField] GameObject[] snackBtns;
    [SerializeField] List<itemInfo> snackItemList;
    int maxPage;
    int multiple;
    [SerializeField] int curPage;

    [SerializeField] Animator btnAni;


    [SerializeField] Button nextBtn;
    [SerializeField] Button backBtn;

    [SerializeField] Sprite nullImage;

    [SerializeField] ShopManager shop;

    [SerializeField] RectTransform snackObject;
    [SerializeField] bool isHoldSnack;
    itemInfo holdedSnackInfo;

    private void Start()
    {
        snackObject.pivot = Vector2.up;
    }
    bool panelActive;
    public void InputSnackPanelBtn()
    {
        panelActive = !panelActive;
        isHoldSnack = false;
        if (panelActive)
        {
            curPage = 1;
            SnackListUpdate();
            InputNextPageBtns(0);
        }
        btnAni.SetBool("Active", panelActive);
    }
    public void PanelClose() => btnAni.SetBool("Active", false);

    void SnackListUpdate()
    {
        snackItemList = new List<itemInfo>();
        for (int i2 = 0; i2 < shop.itemPageinfos[3].itemMenuInfos.Count; i2++)
        {
            for (int i = 0; i < shop.itemPageinfos[3].itemMenuInfos[i2].itemInfos.Count; i++)
            {
                itemInfo curItemInfo = shop.itemPageinfos[3].itemMenuInfos[i2].itemInfos[i];
                if (curItemInfo.curBuyAmount > 0) snackItemList.Add(curItemInfo);
            }
        }
    }

    void SnackBtnsUpdate()
    {
        multiple = (curPage - 1) * snackBtns.Length;
        for (int i = 0; i < snackBtns.Length; i++)
        {
            snackBtns[i].transform.GetChild(0).GetComponent<Image>().sprite = (multiple + i < snackItemList.Count) ? snackItemList[multiple + i].itemThumbNail : nullImage;
            snackBtns[i].transform.GetChild(1).GetComponent<Text>().text = (multiple + i < snackItemList.Count) ? snackItemList[multiple + i].curBuyAmount.ToString() : "";
            snackBtns[i].SetActive(multiple + i < snackItemList.Count);
        }
    }

    public void InputNextPageBtns(int index)
    {
        curPage += index;
        maxPage = (snackItemList.Count % snackBtns.Length == 0) ? snackItemList.Count / snackBtns.Length : (snackItemList.Count / snackBtns.Length) + 1;

        SnackBtnsUpdate();

        backBtn.interactable = (curPage <= 1) ? false : true;
        nextBtn.interactable = (curPage >= maxPage) ? false : true;
    }

    private void Update()
    {
        if (isHoldSnack)
        {
            if (Input.GetMouseButton(0))
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                snackObject.position = new Vector3(mousePos.x, mousePos.y, 0);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                isHoldSnack = false;
                snackObject.gameObject.SetActive(false);

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);
                if (hit.collider != null)
                    if (hit.collider.tag == "CatchCol")
                    {
                        GameObject hamster = hit.collider.transform.parent.gameObject;
                        switch (holdedSnackInfo.itemVariable.Length)
                        {
                            case 1:
                                if (hamster.GetComponent<Move>().isSleep) return;
                                int exp = (int)holdedSnackInfo.itemVariable[0];
                                hamster.GetComponent<HamsterAbility>().AddExp(exp);
                                manager.SummonMessage(hamster.GetComponent<Move>().messagePos.position, true, exp, 1);
                                break;
                            case 2:
                                if (!hamster.GetComponent<Move>().isSleep) return;
                                int time = (int)holdedSnackInfo.itemVariable[0];
                                hamster.GetComponent<HamsterAbility>().AddTired(-time);
                                manager.SummonMessage(hamster.GetComponent<Move>().messagePos.position, false, time, 2);
                                break;
                            case 3:
                                break;
                        }
                        holdedSnackInfo.curBuyAmount--;
                        SnackListUpdate();
                        InputNextPageBtns(0);

                        manager.myHamsterHeartOb.SetActive(uiChangeManager.CheckCanLvUpHamsterExist());

                        AudioManager.Play("Eating");
                    }
            }
        }    
    }
    public void GetSnack(int index)
    {
        if (isHoldSnack) return;

        isHoldSnack = true;
        holdedSnackInfo = snackItemList[multiple + index];
        snackObject.gameObject.SetActive(true);
        snackObject.GetChild(0).GetComponent<Image>().sprite = holdedSnackInfo.itemThumbNail;
    }
}
