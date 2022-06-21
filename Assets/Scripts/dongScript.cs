using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum priceType { coin, hamTicket, won,
    feverTicket, nameTicket,
    normalDirTicket, specialDirTicket, customDirTicket };

public class dongScript : MonoBehaviour
{
    public static dongScript instance;

    public static Sprite nullSprite;
    [SerializeField]
    private Sprite nulls;

    public Camera maincamera;
    Vector3 mousePos;

    [Header("Scripts")]
    [SerializeField] QuestManager questManager;
    [SerializeField] OutSideScript outSideScript;
    [SerializeField] DecorationManager decorationManager;
    [SerializeField] FeverScript feverScript;
    [SerializeField] EpisodeManager episodeManager;
    public DishScript[] dishscript;
    public WaterBowlScript[] waterBowlScript;
    public BathScript[] bathScript;
    public RollorScript[] rollorScript;

    [Header("CoinAmount")]
    [SerializeField] private int coin;
    [SerializeField] private int hamTicket;
    [SerializeField] private int feverTicket;
    [SerializeField] private int nameChangeTicket;
    [SerializeField] private int normalColorTicket;
    [SerializeField] private int specialColorTicket;
    [SerializeField] private int customColorTicket;

    [SerializeField] Text scoreText;
    [SerializeField] Text scoreTextInShop;
    [SerializeField] Text scoreTextInRandomBox;
    [SerializeField] Text ticketAmountText;
    [SerializeField] Text ticketAmountTextInRandomBox;
    [SerializeField] Text cageNumberText;

    [Header("Message")]
    [SerializeField] GameObject messageGroup;
    [SerializeField] GameObject messageOb;

    [Header("Cage")]
    public int cageAmount;
    public Transform[] cage_transform;
    public int cageNum;

    [Header("Wares")]
    public GameObject[] dishPoses;
    public GameObject[] wsterBowlPoses;
    public GameObject[] bathActivePoses;
    public GameObject[] bathingPoses;
    public GameObject[] rollorPoses;
    public GameObject[] rollingPoses;

    [Header("CatchHamster")]
    [SerializeField] bool catching;
    [SerializeField] float maxCatchTime;
    [SerializeField] float curCatchTime;
    [SerializeField] GameObject catchedHam;

    [SerializeField] Vector3[] catchBarrierMainPos;
    [SerializeField] Vector3[] catchBarrier;

    [Header("Panel")]
    [SerializeField] private GameObject warningPanel;
    [SerializeField] GameObject[] cageSellUi;

    [Header("Ui")]
    public GameObject myHamsterHeartOb;

    [Header("DdongAmount")]
    [SerializeField] int cage1Amount;
    [SerializeField] int cage2Amount;
    [SerializeField] int cage3Amount;

    [Header("CurState")]
    public bool panelOn;
    public bool isOutSide;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        nullSprite = nulls;
        instance = this;

        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        float scaleHeight = ((float)Screen.width / Screen.height) / ((float)9 / 16);
        float scaleWidth = 1f / scaleHeight;
        Rect rect = maincamera.rect;
        if (scaleHeight < 1)
        {
            rect.height = scaleHeight;
            rect.y = (1f - scaleHeight) / 2f;
        }
        else
        {
            rect.width = scaleWidth;
            rect.x = (1f - scaleWidth) / 2f;
        }
        maincamera.rect = rect;
    }

    private void Start()
    {
        AddScoreValue(0, priceType.coin);


    }
    void Update()
    {
        if (!panelOn)
        {
            if (Input.GetMouseButton(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);


                if (hit.collider != null)
                {
                    if (hit.collider.tag == "CatchCol" && !catching)
                    {
                        Move hamScript = hit.collider.transform.parent.gameObject.GetComponent<Move>();
                        if (hamScript.isOutside) return;

                        if (curCatchTime < maxCatchTime && hamScript.canMove)
                        curCatchTime += Time.deltaTime;

                        if (curCatchTime > maxCatchTime)
                        {
                            catching = true;
                            curCatchTime = 0;
                            catchedHam = hamScript.gameObject;

                            hamScript.canMove = false;
                            hamScript.SetHamsterMovement(false);

                            AudioManager.Play("Touch1");
                        }

                    }
                    if (hit.collider.tag == "DD")
                    {
                        DdongEvent(hit);
                        AudioManager.Play("Coin");
                    }
                    if (hit.collider.tag == "Balloon")
                    {
                        hit.collider.GetComponent<BalloonScript>().BalloonTouch();
                        AudioManager.Play("BalloonPoop");
                    }
                }
                else curCatchTime = 0;
            }
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);


                if (hit.collider != null)
                {
                    if (hit.collider.tag == "Dish")
                    {
                        TouchDish(hit.transform.gameObject.GetComponent<DishScript>().cageNum);
                    }
                    if (hit.collider.tag == "WaterBowl")
                    {
                        TouchWaterBowl(hit.transform.gameObject.GetComponent<WaterBowlScript>().cageNum);

                    }
                    if (hit.collider.tag == "Window")
                    {
                        Debug.Log("fdaefefefefefef");
                        if(!panelOn)
                        outSideScript.GoOutside(true);
                    }
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                if (catching)
                {
                    catching = false;
                    catchedHam.GetComponent<Move>().canMove = true;

                    catchedHam = null;
                }
                curCatchTime = 0;
            }
            if (catching)
            {
                CheckHamsterArea();
            }
        }
    }

    public bool CountingDdong(int cageNum, bool isRest)
    {
        bool can = true;
        switch (cageNum)
        {
            case 1:
                if (cage1Amount < 90 && !isRest) cage1Amount++;
                else can = false;
                break;
            case 2:
                if (cage2Amount < 90 && !isRest) cage2Amount++;
                else can = false;
                break;
            case 3:
                if (cage3Amount < 90 && !isRest) cage3Amount++;
                else can = false;
                break;
        }
        
        return can;
    }

    void DdongEvent(RaycastHit2D hit)
    {
        DDMoneyScript dong = hit.collider.GetComponent<DDMoneyScript>();

        int bonus = dong.isGold == true ? 5 : 1;
        int finalMoney = dong.moneyAmount * bonus;

        switch (dong.cageNum)
        {
            case 1:
                cage1Amount--;
                break;
            case 2:
                cage2Amount--;
                break;
            case 3:
                cage3Amount--;
                break;
        }
        AddScoreValue(finalMoney, priceType.coin);
        Destroy(dong.gameObject);

        SummonMessage(dong.transform.position, true, finalMoney, 0);

        questManager.CheckQuestComplete(true);

        if (!episodeManager.episodeInfos[3].isWatch) episodeManager.EpisodeStart(false, 3, false);
    }

    public void CageUpdate()
    {
        switch (cageAmount)
        {
            case 1:
                cageSellUi[1].transform.GetChild(3).GetComponent<Button>().interactable = false;
                break;
            case 2:
                cageSellUi[0].SetActive(false);
                cageSellUi[1].transform.GetChild(3).GetComponent<Button>().interactable = true;
                break;
            case 3:
                cageSellUi[0].SetActive(false);
                cageSellUi[1].SetActive(false);
                break;
        }
    }
    public void BuyCage(int cost)
    {
        if (coin >= cost)
        {
            AddScoreValue(-cost, priceType.coin);

            cageAmount++;

            CageUpdate();
            AudioManager.Play("Touch1");
        }
        else
        {
            AudioManager.Play("Cancel2");
        }
    }
    public void SummonMessage(Vector2 pos ,bool isPlus, int amount, int statusCode)
    {
        StatusMessageScript message = Instantiate(messageOb, pos,Quaternion.identity).GetComponent<StatusMessageScript>();
        message.transform.parent = messageGroup.transform;
        message.transform.localScale = new Vector3(1, 1, 1);
        message.MessageUpdate(isPlus, amount, statusCode);
    }

    public void ChangeCameraView(int index)
    {
        if (catching) return;
        cageNum += index;
        if (cageNum == 3) cageNum = 0;
        else if (cageNum == -1) cageNum = 2;

        cageNumberText.text = "Cage" + (cageNum + 1).ToString();
        maincamera.transform.position = new Vector3(cage_transform[cageNum].position.x, cage_transform[cageNum].position.y, -10);
    }

    public void AddScoreValue(int amount, priceType typee)
    {
        switch (typee)
        {
            case priceType.coin:
                coin += amount;
                break;
            case priceType.hamTicket:
                hamTicket += amount;
                break;
            case priceType.feverTicket:
                feverTicket += amount;
                break;
            case priceType.nameTicket:
                nameChangeTicket += amount;
                break;
            case priceType.normalDirTicket:
                normalColorTicket += amount;
                break;
            case priceType.specialDirTicket:
                specialColorTicket += amount;
                break;
            case priceType.customDirTicket:
                customColorTicket += amount;
                break;
        }

        scoreText.text = coin.ToString() + "@";
        scoreTextInShop.text = "@ " + coin;
        scoreTextInRandomBox.text = "@ " + coin;

        ticketAmountText.text = "#" + hamTicket;
        ticketAmountTextInRandomBox.text = "#" + hamTicket;

        feverScript.FeverPanelUpdate();
    }
    public int GetScoreValue(priceType typee)
    {
        int value = 0;
        switch (typee)
        {
            case priceType.coin:
                value = coin;
                break;
            case priceType.hamTicket:
                value = hamTicket;
                break;
            case priceType.feverTicket:
                value = feverTicket;
                break;
            case priceType.nameTicket:
                value = nameChangeTicket;
                break;
            case priceType.normalDirTicket:
                value = normalColorTicket;
                break;
            case priceType.specialDirTicket:
                value = specialColorTicket;
                break;
            case priceType.customDirTicket:
                value = customColorTicket;
                break;
        }
        return value;
    }


    void TouchDish(int num)
    {
        DishScript curDish = dishscript[num];
        if (curDish.isFull == false)
        {
            Debug.Log("dwdw");
            if(coin >= dishscript[num].foodPrice)
            {
                AddScoreValue(-dishscript[0].foodPrice, priceType.coin);
                curDish.MinusCoin();
                curDish.isFull = true;
                curDish.FillFood();

                AudioManager.Play("Coin");
            }
            else
            {
                StartCoroutine(WarningPanelActive(curDish.foodPrice.ToString() + "코인이 부족합니다"));

                AudioManager.Play("Cancel2");
            }
        }
    }

    void TouchWaterBowl(int num)
    {
        Debug.Log("물통에 마우스가 닿음");

        waterBowlScript[num].RefillWaterBowl();
    }

    void CheckHamsterArea()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 hamsterPos = mousePos;
        float mainX = catchBarrierMainPos[cageNum].x + cage_transform[cageNum].position.x;
        float mainY = catchBarrierMainPos[cageNum].y + cage_transform[cageNum].position.y;

        if (mousePos.x < -(catchBarrier[cageNum].x - mainX))
            hamsterPos.x = -(catchBarrier[cageNum].x - mainX);
        else if (mousePos.x > catchBarrier[cageNum].x + mainX)
            hamsterPos.x = catchBarrier[cageNum].x + mainX;

        if(mousePos.y < -(catchBarrier[cageNum].y - mainY))
            hamsterPos.y = -(catchBarrier[cageNum].y - mainY);
        else if (mousePos.y > catchBarrier[cageNum].y + mainY)
            hamsterPos.y = catchBarrier[cageNum].y + mainY;

        catchedHam.transform.position = hamsterPos;
    }

    public IEnumerator WarningPanelActive(string talk)
    {
        warningPanel.transform.GetChild(2).GetComponent<Text>().text = talk;
        warningPanel.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        warningPanel.SetActive(false);
    }

    /*
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(cage_transform[0].position + catchBarrierMainPos[0], catchBarrier[0] * 2);
        Gizmos.DrawWireCube(cage_transform[1].position + catchBarrierMainPos[1], catchBarrier[1] * 2);
        Gizmos.DrawWireCube(cage_transform[2].position + catchBarrierMainPos[2], catchBarrier[2] * 2);
    }*/
}



