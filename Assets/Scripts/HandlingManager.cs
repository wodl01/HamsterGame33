using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandlingManager : MonoBehaviour
{
    [SerializeField] Ui_ChangeManager hamsterList;
    [SerializeField] dongScript gameManager;
    [SerializeField] HousingManager housingManager;
    [SerializeField] ShopManager shopManager;

    [Header("Panels")]
    [SerializeField] GameObject handlingPanel;
    [SerializeField] GameObject mainUiPanel;
    [SerializeField] GameObject myHamsterPanel;

    [Header("Ui")]
    [SerializeField] Slider timerBar;
    [SerializeField] Image jumpingHamsterImage;
    [SerializeField] Image jumpingHamsterSub;
    [SerializeField] Image jumpingHamsterLine;
    [SerializeField] Image jumpingHamsterOutLine;
    [SerializeField] Image sitHamsterImage;
    [SerializeField] Image sitHamsterSub;
    [SerializeField] Image sitHamsterLine;
    [SerializeField] Image sitHamsterOutLine;
    [SerializeField] Transform messageTextPoint;
    [SerializeField] Text expAmountText;

    [SerializeField] Animator animator;

    [Header("CurState")]
    bool canHandling;
    public bool handlingCoolOn;

    [Header("Exp")]
    int curPlusExp;
    int finalPlusExp;
    [SerializeField] int plusStack;

    [Header("Time")]
    float curDraggingTime;
    [SerializeField] float maxDraggingTime;
    bool timerActive;
    float curTimerTime;
    public float maxTimerTime;

    [SerializeField] private float curCoolTimeSec;
    [SerializeField] private int curCoolTimeMin;

    [Header("Heart")]
    [SerializeField] Image heartImage;
    [SerializeField] Sprite[] heartSprites;
    int curHeartIndex;
    float curHeartChangeTime;
    [SerializeField] float maxHeartChangeTime;

    [Header("Delay")]
    [SerializeField] float startDelay;
    [SerializeField] float outDelay;

    public void OpenHandlingPanel()
    {
        mainUiPanel.SetActive(false);
        myHamsterPanel.SetActive(false);
        handlingPanel.SetActive(true);
        HamsterInfo curHamInfo = hamsterList.target.hamsterInfo;

        jumpingHamsterImage.sprite = hamsterList.target.hamsterHandling.jumpMain;
        jumpingHamsterImage.color = curHamInfo.hamsterMainColor;
        if (curHamInfo.isShowSub)
        {
            jumpingHamsterSub.sprite = hamsterList.target.hamsterHandling.jumpSub;
            jumpingHamsterSub.color = curHamInfo.hamsterSubColor;
        }
        else jumpingHamsterSub.sprite = dongScript.nullSprite;
        if (curHamInfo.isShowLine)
        {
            jumpingHamsterLine.sprite = hamsterList.target.hamsterHandling.jumpLine;
            jumpingHamsterLine.color = curHamInfo.hamsterLineColor;
        }
        else jumpingHamsterLine.sprite = dongScript.nullSprite;
        if (hamsterList.target.hamsterHandling.jumpOutLine)
            jumpingHamsterOutLine.sprite = hamsterList.target.hamsterHandling.jumpOutLine;
        else jumpingHamsterOutLine.sprite = dongScript.nullSprite;


        sitHamsterImage.sprite = hamsterList.target.hamsterHandling.sitMain;
        sitHamsterImage.color = curHamInfo.hamsterMainColor;
        if (curHamInfo.isShowSub)
        {
            sitHamsterSub.sprite = hamsterList.target.hamsterHandling.sitSub;
            sitHamsterSub.color = curHamInfo.hamsterSubColor;
        }
        else sitHamsterSub.sprite = dongScript.nullSprite;
        if (curHamInfo.isShowLine)
        {
            sitHamsterLine.sprite = hamsterList.target.hamsterHandling.sitLine;
            sitHamsterLine.color = curHamInfo.hamsterLineColor;
        }
        else sitHamsterLine.sprite = dongScript.nullSprite;
        if (hamsterList.target.hamsterHandling.sitOutLine)
            sitHamsterOutLine.sprite = hamsterList.target.hamsterHandling.sitOutLine;
        else
            sitHamsterOutLine.sprite = dongScript.nullSprite;


        heartImage.sprite = heartSprites[0];
        
        expAmountText.gameObject.SetActive(false);

        timerActive = false;
        canHandling = true;

        curPlusExp = 1;
        finalPlusExp = 0;
        curHeartIndex = 0;

        animator.SetBool("End", false);

        housingManager.MoveHamster(true);

        StartCoroutine(StartDelay());
    }
    IEnumerator StartDelay()
    {
        yield return new WaitForSeconds(startDelay);
        timerActive = true;
        curTimerTime = maxTimerTime;
    }

    public void DragHamster()
    {
        if (!canHandling) return;
        HamsterInfo curHamInfo = hamsterList.target.hamsterInfo;

        bool lv2 = curHamInfo.lv > 35;

        if (lv2)
            sitHamsterImage.sprite = hamsterList.target.hamsterHandling.happySpriteLv1_Main;
        else
            sitHamsterImage.sprite = hamsterList.target.hamsterHandling.happySpriteLv2_Main;

        if (curHamInfo.isShowSub)
        {
            if(lv2) sitHamsterSub.sprite = hamsterList.target.hamsterHandling.happySpriteLv1_Sub;
            else sitHamsterSub.sprite = hamsterList.target.hamsterHandling.happySpriteLv2_Sub;
        } 
        else
            sitHamsterSub.sprite = dongScript.nullSprite;

        if (curHamInfo.isShowLine)
        {
            if (lv2) sitHamsterLine.sprite = hamsterList.target.hamsterHandling.happySpriteLv1_Line;
            else sitHamsterLine.sprite = hamsterList.target.hamsterHandling.happySpriteLv2_Line;
        }
        else
            sitHamsterLine.sprite = dongScript.nullSprite;

        if (hamsterList.target.hamsterHandling.happySpriteLv1_OutLine)
        {
            if(lv2) sitHamsterOutLine.sprite = hamsterList.target.hamsterHandling.happySpriteLv1_OutLine;
            else sitHamsterOutLine.sprite = hamsterList.target.hamsterHandling.happySpriteLv2_OutLine;
        }
        else
            sitHamsterOutLine.sprite = dongScript.nullSprite;


        curDraggingTime += Time.deltaTime;
        if(curDraggingTime > maxDraggingTime)
        {
            curDraggingTime = 0;
            plusStack++;
            if(plusStack > 200)
            {
                plusStack = 0;
                curPlusExp++;
            }
            finalPlusExp += curPlusExp;
            expAmountText.text = finalPlusExp.ToString();
            expAmountText.gameObject.SetActive(true);
            //hamsterList.target.AddExp(curPlusExp);
            //gameManager.SummonMessage(messageTextPoint.position, true, curPlusExp, 1);
        }

        curHeartChangeTime -= Time.deltaTime;
        if (curHeartChangeTime < 0)
        {
            curHeartChangeTime = maxHeartChangeTime;
            curHeartIndex++;
            if (curHeartIndex == 5) curHeartIndex = 1;
        }

        heartImage.sprite = heartSprites[curHeartIndex];
    }
    public void DragHamsterEnd()
    {
        HamsterInfo curHamInfo = hamsterList.target.hamsterInfo;

        sitHamsterImage.sprite = hamsterList.target.hamsterHandling.sitMain;
        sitHamsterImage.color = curHamInfo.hamsterMainColor;
        if (curHamInfo.isShowSub)
        {
            sitHamsterSub.sprite = hamsterList.target.hamsterHandling.sitSub;
            sitHamsterSub.color = curHamInfo.hamsterSubColor;
        }
        else sitHamsterSub.sprite = dongScript.nullSprite;
        if (curHamInfo.isShowLine)
        {
            sitHamsterLine.sprite = hamsterList.target.hamsterHandling.sitLine;
            sitHamsterLine.color = curHamInfo.hamsterLineColor;
        }
        else sitHamsterLine.sprite = dongScript.nullSprite;
        if (hamsterList.target.hamsterHandling.sitOutLine)
            sitHamsterOutLine.sprite = hamsterList.target.hamsterHandling.sitOutLine;
        else
            sitHamsterOutLine.sprite = dongScript.nullSprite;

        heartImage.sprite = heartSprites[0];
    }

    public void CloseHandlingPanel()
    {
        handlingPanel.SetActive(false);
        mainUiPanel.SetActive(true);
        shopManager.ClosePanel(myHamsterPanel);
        timerActive = false;

        housingManager.MoveHamster(false);
    }

    private void FixedUpdate()
    {
        if(curTimerTime > 0 && timerActive)
        {
            curTimerTime -= Time.deltaTime;
            timerBar.value = curTimerTime / maxTimerTime;
            if (curTimerTime < 0)
            {
                animator.SetBool("End",true);
                canHandling = false;
                heartImage.sprite = heartSprites[0];

                handlingCoolOn = false;
                curCoolTimeMin = 3;

                hamsterList.target.AddExp(finalPlusExp);

                StartCoroutine(OutDelay());
            }
        }

        if(curCoolTimeSec > 0|| curCoolTimeMin > 1)
        {
            curCoolTimeSec -= Time.deltaTime;
            if(curCoolTimeSec < 0)
            {
                if(curCoolTimeMin > 0)
                {
                    curCoolTimeMin--;
                    curCoolTimeSec = 60;
                }
                else
                {
                    handlingCoolOn = true;
                    hamsterList.InfoUpdate();
                }
                
            }
        }
        hamsterList.handlingTimerText.text = curCoolTimeMin.ToString() + ":" + Mathf.Ceil(curCoolTimeSec).ToString();
    }

    IEnumerator OutDelay()
    {
        yield return new WaitForSeconds(outDelay);
        CloseHandlingPanel();
    }
}
