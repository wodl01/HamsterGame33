using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class HamsterHandling
{
    public Sprite happySpriteLv1_Main;
    public Sprite happySpriteLv1_Sub;
    public Sprite happySpriteLv1_Line;
    public Sprite happySpriteLv1_OutLine;
    public Sprite happySpriteLv2_Main;
    public Sprite happySpriteLv2_Sub;
    public Sprite happySpriteLv2_Line;
    public Sprite happySpriteLv2_OutLine;

    public Sprite jumpMain;
    public Sprite jumpSub;
    public Sprite jumpLine;
    public Sprite jumpOutLine;

    public Sprite sitMain;
    public Sprite sitSub;
    public Sprite sitLine;
    public Sprite sitOutLine;
}

[System.Serializable]
public class HamsterInfo
{
    public HamsterAbility curhamAbility;

    public bool iHave;
    public int hamsterCode;

    public int cageNum;
    public string name_H;
    public string message;
    public int lv;
    public int curExp;

    public string kind;

    public float tired;
    public float hungry;
    public float clean;
    public float thirsty;
    public float rollingCool;

    public Color hamsterMainColor;
    public Color hamsterSubColor;
    public Color hamsterLineColor;
    public int hamsterHatCode;
    public int hamsterAcCode;
    public int hamsterDongCode;
    public int hamsterBackCode;

    public bool isShowSub;
    public bool isShowLine;
}

public class HamsterAbility : MonoBehaviour
{
    [Header("Scripts")]
    [SerializeField] QuestManager questManager;
    [SerializeField] Ui_ChangeManager uiManager;
    public HamsterBalance balance;

    public bool isSpawn;
    public float rollingCoolTime;

    [Header("HamsterLook")]

    public Vector2 hatVecter;

    public Vector2 acVecter;

    public Vector2 backVecter;
    public Sprite profileMainSprite;
    public Sprite profileLineSprite;
    public Sprite profileSubSprite;
    public Sprite profileOutLineSprite;
    public Sprite profileFaceSprite;


    [Header("HamsterDecoImages")]
    public SpriteRenderer hat;
    public SpriteRenderer face;

    public HamsterHandling hamsterHandling;
    public HamsterInfo hamsterInfo;


    public void AddExp(int expAmount)
    {
        hamsterInfo.curExp += expAmount;
        if (hamsterInfo.curExp > balance.maxExp[hamsterInfo.lv]) hamsterInfo.curExp = balance.maxExp[hamsterInfo.lv];
        else if (hamsterInfo.curExp < 0) hamsterInfo.curExp = 0;

        if (uiManager.target == this)
            uiManager.InfoUpdate();

        questManager.CheckQuestComplete(false);
    }
    public void AddTired(int tiredValue)
    {
        hamsterInfo.tired += tiredValue;
        if (hamsterInfo.tired < 0) hamsterInfo.tired = 0;

        if (uiManager.target == this)
            uiManager.InfoUpdate();

        questManager.CheckQuestComplete(false);
    }
}
