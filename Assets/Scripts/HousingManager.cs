using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Cages
{
    [SerializeField] string cageNum;
    public SpriteRenderer[] dishRenderer;
    public SpriteRenderer[] waterRenderer;
    public SpriteRenderer[] bathRenderer;
    public SpriteRenderer[] houseRenderer;
    public SpriteRenderer sandRenderer;
    public SpriteRenderer[] wallDecoRenderer;
    public SpriteRenderer[] beddingRenderer;
    public SpriteRenderer[] wallRenderer;
    public SpriteRenderer[] rollorRenderer;

    public GameObject[] allHamsterWares;
}

[System.Serializable]
public class WareOriginalPosition
{
    public bool isMoved;
    public Transform targetTransform;
    public Vector2 targetOriginalPos;
}
public class HousingManager : MonoBehaviour
{
    [SerializeField] dongScript gameManager;
    [SerializeField] ShopManager shopManager;
    [SerializeField] Ui_ChangeManager hamsterInfos;

    [SerializeField] GameObject hamsterRestPos;
    [SerializeField] List<Vector2> hamsterOriginalPos;

    [Header("CameraMove")]
    [SerializeField] Transform mainCamera;
    [SerializeField] Transform[] cageTransform;

    [SerializeField] Vector3 groundY;
    [SerializeField] Vector3 wallY;

    [SerializeField] Vector3 targetVector;
    [SerializeField] bool moveCamera;
    [SerializeField] float moveSpeed;

    [Header("Ui")]
    [SerializeField] Button[] changeBtns;
    [SerializeField] GameObject menuPanel;
    [SerializeField] GameObject decoChoicePanel;
    [SerializeField] Text cageText;
    [SerializeField] Button placeCompleteBtn;
    [SerializeField] Button placeWareFlipBtn;
    [SerializeField] GameObject mainUi;
    [SerializeField] GameObject moneyUi;
    [SerializeField] GameObject placeModeUi;
    [SerializeField] GameObject changeModeUi;

    [Header("HamsterWares")]
    public DishScript[] dish;
    public WaterBowlScript[] waterBowl;
    public BathScript[] bath;
    public HouseScript[] house;
    public WallDecoScript[] wallDeco;
    public RollorScript[] rollor;
    public BeddingScript[] bedding;
    public WallScript[] wall;
    public WindowScript[] window;

    [Header("ButtonUis")]
    [SerializeField] GameObject[] pageObs;
    [SerializeField] GameObject[] menuObs;
    [SerializeField] GameObject[] dishButtons;
    [SerializeField] GameObject[] waterButtons;
    [SerializeField] GameObject[] houseButtons;
    [SerializeField] GameObject[] bathButtons;
    [SerializeField] GameObject[] sandButtons;
    [SerializeField] GameObject[] rollorButtons;
    [SerializeField] GameObject[] wallDecoButtons;
    [SerializeField] GameObject[] beddingButtons;
    [SerializeField] GameObject[] wallButtons;
    [SerializeField] GameObject[] windowButtons;

    [Header("CurState")]
    int wareTypeCode;
    int curPage;
    public bool placeMode;
    public int placeOkayCount;
    [SerializeField] int wareCount;
    [SerializeField] bool isPlacing;

    [Header("PlaceMode")]
    [SerializeField] Vector3[] placeBarrierMainPos;//0 dish //1 water
    [SerializeField] Vector3[] placeBarrier;
    [SerializeField] GameObject movingWare;
    public List<WareOriginalPosition> wareOriginalPos;

    //[ArrayElementTitle("cageNum")]
    public List<Cages> hamsterWaresRenderers;

    private void Start()
    {
        //HamsterWaresUpdate();
    }

    public void HousingOpen()
    {
        curPage = -1;
        mainUi.SetActive(false);
        moneyUi.SetActive(false);
        gameManager.cageNum = 0;

        wareTypeCode = 0;

        CheckHamsterWares();
        PlaceMode(false);
        HousingItemUiUpdate();
        InputPageChangeBtn();
        HamsterWaresUpdate();
        MoveHamster(true);
        UpdateCageChangeBtns();

        cageText.text = "Cage" + (gameManager.cageNum + 1).ToString();

        decoChoicePanel.SetActive(false);
        menuPanel.SetActive(false);
    }
    public void HousingClose()
    {
        moveCamera = false;
        mainCamera.transform.position = cageTransform[gameManager.cageNum].position + new Vector3(0,0,-10);

        mainUi.SetActive(true);
        moneyUi.SetActive(true);

        MoveHamster(false);
    }

    public void MoveHamster(bool moveToRestRoom)
    {
        if(moveToRestRoom) hamsterOriginalPos = new List<Vector2>();

        for (int i = 0; i < hamsterInfos.hamsterList.Count; i++)
        {
            Move curHamster = hamsterInfos.hamsterList[i].GetComponent<Move>();
            if (moveToRestRoom)
            {
                Vector2 finalPos = new Vector2();
                if (curHamster.isRolling)
                    finalPos = gameManager.rollorPoses[curHamster.ability.hamsterInfo.cageNum - 1].transform.position;
                else
                    finalPos = curHamster.transform.position;

                hamsterOriginalPos.Add(finalPos);

                curHamster.isRestRoom = true;
                curHamster.SetHamsterMovement(false);

                hamsterInfos.hamsterList[i].transform.position = hamsterRestPos.transform.position;
            }
            else
            {
                curHamster.isRestRoom = false;
                if (!curHamster.isSleep || !curHamster.isEmoting)
                    curHamster.SetHamsterMovement(true);

                curHamster.ani.SetBool("Move", curHamster.canMove);
                curHamster.transform.position = hamsterOriginalPos[i];
            }
        }
    }

    #region ItemList
    void HousingItemUiUpdate()
    {
        for (int i = 0; i < shopManager.itemPageinfos.Count; i++)
        {
            for (int i2 = 0; i2 < shopManager.itemPageinfos[i].itemMenuInfos.Count; i2++)
            {
                for (int i3 = 0; i3 < shopManager.itemPageinfos[i].itemMenuInfos[i2].itemInfos.Count; i3++)
                {
                    itemInfo curItemInfo = shopManager.itemPageinfos[i].itemMenuInfos[i2].itemInfos[i3];
                    switch (i)
                    {
                        case 0://0 page
                            switch (i2)
                            {
                                case 0://dish 1
                                    if (curItemInfo.curBuyAmount > 0) dishButtons[i3].SetActive(true);
                                    else dishButtons[i3].SetActive(false);
                                    dishButtons[i3].transform.GetChild(0).GetComponent<Image>().sprite = curItemInfo.itemThumbNail;
                                    dishButtons[i3].transform.GetChild(0).GetComponent<Image>().SetNativeSize();
                                    break;
                                case 1://water 2
                                    if (curItemInfo.curBuyAmount > 0) waterButtons[i3].SetActive(true);
                                    else waterButtons[i3].SetActive(false);
                                    waterButtons[i3].transform.GetChild(0).GetComponent<Image>().sprite = curItemInfo.itemThumbNail;
                                    waterButtons[i3].transform.GetChild(0).GetComponent<Image>().SetNativeSize();
                                    break;
                                case 2://rollor 3
                                    if (curItemInfo.curBuyAmount > 0) rollorButtons[i3].SetActive(true);
                                    else rollorButtons[i3].SetActive(false);
                                    rollorButtons[i3].transform.GetChild(0).GetComponent<Image>().sprite = curItemInfo.itemThumbNail;
                                    rollorButtons[i3].transform.GetChild(0).GetComponent<Image>().SetNativeSize();
                                    break;
                                case 3://house 4
                                    if (curItemInfo.curBuyAmount > 0) houseButtons[i3].SetActive(true);
                                    else houseButtons[i3].SetActive(false);
                                    houseButtons[i3].transform.GetChild(0).GetComponent<Image>().sprite = curItemInfo.itemThumbNail;
                                    houseButtons[i3].transform.GetChild(0).GetComponent<Image>().SetNativeSize();
                                    break;
                                case 4://bath 5
                                    if (curItemInfo.curBuyAmount > 0) bathButtons[i3].SetActive(true);
                                    else bathButtons[i3].SetActive(false);
                                    bathButtons[i3].transform.GetChild(0).GetComponent<Image>().sprite = curItemInfo.itemThumbNail;
                                    bathButtons[i3].transform.GetChild(0).GetComponent<Image>().SetNativeSize();
                                    break;
                                case 5://sand 6
                                    if (curItemInfo.curBuyAmount > 0) sandButtons[i3].SetActive(true);
                                    else sandButtons[i3].SetActive(false);
                                    sandButtons[i3].transform.GetChild(0).GetComponent<Image>().sprite = curItemInfo.itemThumbNail;
                                    sandButtons[i3].transform.GetChild(0).GetComponent<Image>().SetNativeSize();
                                    break;
                            }
                            break;
                        case 1:
                            switch (i2)
                            {
                                case 0:
                                    if (curItemInfo.curBuyAmount > 0) beddingButtons[i3].SetActive(true);
                                    else beddingButtons[i3].SetActive(false);
                                    beddingButtons[i3].transform.GetChild(0).GetComponent<Image>().sprite = curItemInfo.itemThumbNail;
                                    beddingButtons[i3].transform.GetChild(0).GetComponent<Image>().SetNativeSize();
                                    break;
                                case 1:
                                    if (curItemInfo.curBuyAmount > 0) wallButtons[i3].SetActive(true);
                                    else wallButtons[i3].SetActive(false);
                                    wallButtons[i3].transform.GetChild(0).GetComponent<Image>().sprite = curItemInfo.itemThumbNail;
                                    wallButtons[i3].transform.GetChild(0).GetComponent<Image>().SetNativeSize();
                                    break;
                                case 2:
                                    if (curItemInfo.curBuyAmount > 0) wallDecoButtons[i3].SetActive(true);
                                    else wallDecoButtons[i3].SetActive(false);
                                    wallDecoButtons[i3].transform.GetChild(0).GetComponent<Image>().sprite = curItemInfo.itemThumbNail;
                                    wallDecoButtons[i3].transform.GetChild(0).GetComponent<Image>().SetNativeSize();
                                    break;
                                case 3:
                                    if (curItemInfo.curBuyAmount > 0) windowButtons[i3].SetActive(true);
                                    else windowButtons[i3].SetActive(false);
                                    windowButtons[i3].transform.GetChild(0).GetComponent<Image>().sprite = curItemInfo.itemThumbNail;
                                    windowButtons[i3].transform.GetChild(0).GetComponent<Image>().SetNativeSize();
                                    break;
                            }
                            break;
                    }
                }
            }
        }
    }
    public void InputPageChangeBtn()
    {
        curPage++;
        if (curPage == pageObs.Length) curPage = 0;

        for (int i = 0; i < pageObs.Length; i++)
            pageObs[i].SetActive(false);

        pageObs[curPage].SetActive(true);
        switch (curPage)
        {
            case 0:
                MenuOnClick("0 1");
                break;
            case 1:
                MenuOnClick("4 2");
                break;
        }
    }
    public void MenuOnClick(string code)//ex.(0 1) /1:Down, 2:Up //:CameraMove
    {
        string[] codes = code.Split(' ');//1 1
        wareTypeCode = int.Parse(codes[0]);
        for (int i = 0; i < menuObs.Length; i++)
        {
            menuObs[i].transform.SetAsLastSibling();
        }
        menuObs[int.Parse(codes[0])].transform.SetAsLastSibling();
        switch (int.Parse(codes[1]))
        {
            case 1:
                CameraMove_Ground();
                break;
            case 2:
                CameraMove_Wall();
                break;
        }
    }
    public void InputChangeCageBtn(int index)
    {

        gameManager.ChangeCameraView(index);
        cageText.text = "Cage" + (gameManager.cageNum + 1).ToString();
        CameraMove_Ground();
        UpdateCageChangeBtns();
    }
    #endregion
    public void UpdateCageChangeBtns()
    {
        switch (gameManager.cageAmount)
        {
            case 1:
                changeBtns[0].interactable = false;
                changeBtns[1].interactable = false;
                break;
            case 2:
                if (gameManager.cageNum == 1)
                {
                    changeBtns[0].interactable = true;
                    changeBtns[1].interactable = false;
                }
                else if(gameManager.cageNum == 0)
                {
                    changeBtns[0].interactable = false;
                    changeBtns[1].interactable = true;
                }
                break;
            case 3:
                changeBtns[0].interactable = true;
                changeBtns[1].interactable = true;
                break;
        }
    }

    #region PlaceMode
    public void InputPlaceBtn()
    {
        PlaceMode(true);

        for (int i = 0; i < wareOriginalPos.Count; i++)
            wareOriginalPos[i].isMoved = false;
    }
    public void InputPlaceYesOrNoBtn(bool yes)
    {
        if (isPlacing) return;
        if(yes)
        {

        }
        else
        {
            for (int i = 0; i < wareOriginalPos.Count; i++)
            {
                if(wareOriginalPos[i].isMoved)
                    wareOriginalPos[i].targetTransform.position = wareOriginalPos[i].targetOriginalPos;

            }
        }
        PlaceMode(false);
    }
    public void InputReverseBtn()
    {
        switch (wareTypeCode)
        {
            case 0:
                dish[gameManager.cageNum].FlipImage(true);
                break;
            case 1:
                waterBowl[gameManager.cageNum].FlipImage(true);
                break;
        }
    }
    void PlaceMode(bool active)
    {
        placeMode = active;
        placeModeUi.SetActive(active);
        changeModeUi.SetActive(!active);
        if (active)
        {
            placeWareFlipBtn.interactable = false;

            moveCamera = false;
            mainCamera.transform.position = cageTransform[gameManager.cageNum].position + new Vector3(0, 0, -10);
        }
        else
        {
            moveCamera = true;
        }
    }
    void CameraMove_Ground()
    {
        targetVector = cageTransform[gameManager.cageNum].position + groundY;
    }
    void CameraMove_Wall()
    {
        targetVector = cageTransform[gameManager.cageNum].position + wallY;
    }
#endregion

    public void ItemOnClick(string code)
    {
        string[] codes = code.Split(' ');//1 1 1
        int cagePage = gameManager.cageNum;
        itemInfo target = shopManager.itemPageinfos[int.Parse(codes[0])].itemMenuInfos[int.Parse(codes[1])].itemInfos[int.Parse(codes[2])];
        int itemCode = target.itemCode;
        switch (int.Parse(codes[0]))
        {
            case 0:
                switch (int.Parse(codes[1]))
                {
                    case 0:
                        dish[cagePage].wareNum = itemCode;
                        break;
                    case 1:
                        waterBowl[cagePage].wareNum = itemCode;
                        break;
                    case 2:
                        rollor[cagePage].wareNum = itemCode;
                        break;
                    case 3:
                        house[cagePage].wareNum = itemCode;
                        break;
                    case 4:
                        bath[cagePage].bathWareNum = itemCode;
                        break;
                    case 5:
                        bath[cagePage].sandWareNum = itemCode;
                        break;
                }
                break;
            case 1:
                switch (int.Parse(codes[1]))
                {
                    case 0:
                        bedding[cagePage].wareNum = itemCode;
                        break;
                    case 1:
                        wall[cagePage].wareNum = itemCode;
                        break;
                    case 2:
                        wallDeco[cagePage].wareNum = itemCode;
                        break;
                    case 3:
                        window[cagePage].wareNum = itemCode;
                        break;
                }
                break;
        }
        HamsterWaresUpdate();
    }

    public void HamsterWaresUpdate()
    {
        for (int i3 = 0; i3 < 3; i3++)
        {
            if (dish[i3].wareNum == -1) dish[i3].gameObject.SetActive(false);
            else
            {
                dish[i3].gameObject.SetActive(true);
                for (int i = 0; i < hamsterWaresRenderers[i3].dishRenderer.Length; i++)
                {
                    hamsterWaresRenderers[i3].dishRenderer[i].sprite = shopManager.GetItemInfo(0, 0, dish[i3].wareNum).itemImage[i];
                }
            }
            dish[i3].FoodImageUpdate();

            if (waterBowl[i3].wareNum == -1) waterBowl[i3].gameObject.SetActive(false);
            else
            {
                waterBowl[i3].gameObject.SetActive(true);
                for (int i = 0; i < hamsterWaresRenderers[i3].waterRenderer.Length; i++)
                {
                    hamsterWaresRenderers[i3].waterRenderer[i].sprite = shopManager.GetItemInfo(0, 1, waterBowl[i3].wareNum).itemImage[i];
                    if (i > 1) hamsterWaresRenderers[i3].waterRenderer[i].sortingOrder = (int)shopManager.GetItemInfo(0, 1, waterBowl[i3].wareNum).itemVariable[0];
                }
            }

            if (rollor[i3].wareNum == -1) rollor[i3].gameObject.SetActive(false);
            else
            {
                rollor[i3].gameObject.SetActive(true);
                itemInfo rollorItemInfo = shopManager.GetItemInfo(0, 2, rollor[i3].wareNum);
                for (int i = 0; i < hamsterWaresRenderers[i3].rollorRenderer.Length; i++)
                {
                    hamsterWaresRenderers[i3].rollorRenderer[i].sprite = rollorItemInfo.itemImage[i];
                }
                rollor[i3].rollingPos.transform.localPosition = new Vector2(0, rollorItemInfo.itemVariable[0]);
            }

            if (house[i3].wareNum == -1) house[i3].gameObject.SetActive(false);
            else
            {
                house[i3].gameObject.SetActive(true);
                itemInfo houseItemInfo = shopManager.GetItemInfo(0, 3, house[i3].wareNum);

                hamsterWaresRenderers[i3].houseRenderer[0].sprite = houseItemInfo.itemImage[0];
            }

            if (bath[i3].bathWareNum == -1)
                bath[i3].gameObject.SetActive(false);
            else
            {
                bath[i3].gameObject.SetActive(true);
                for (int i = 0; i < hamsterWaresRenderers[i3].bathRenderer.Length; i++)
                {
                    hamsterWaresRenderers[i3].bathRenderer[i].sprite = shopManager.GetItemInfo(0, 4, bath[i3].bathWareNum).itemImage[i];
                }
                if (bath[i3].sandWareNum == -1)
                    hamsterWaresRenderers[i3].sandRenderer.gameObject.SetActive(false);
                else
                    hamsterWaresRenderers[i3].sandRenderer.sprite = shopManager.GetItemInfo(0, 4, bath[i3].sandWareNum).itemImage[bath[i3].sandWareNum + 2];
            }

            if (wallDeco[i3].wareNum == -1) wallDeco[i3].gameObject.SetActive(false);
            else
            {
                wallDeco[i3].gameObject.SetActive(true);
                for (int i = 0; i < hamsterWaresRenderers[i3].wallDecoRenderer.Length; i++)
                {
                    hamsterWaresRenderers[i3].wallDecoRenderer[i].sprite = shopManager.GetItemInfo(1, 2, wallDeco[i3].wareNum).itemImage[i];
                }
            }

            for (int i = 0; i < hamsterWaresRenderers[i3].beddingRenderer.Length; i++)
            {
                hamsterWaresRenderers[i3].beddingRenderer[i].sprite = shopManager.GetItemInfo(1, 0, bedding[i3].wareNum).itemImage[i];
            }

            for (int i = 0; i < hamsterWaresRenderers[i3].wallRenderer.Length; i++)
            {
                hamsterWaresRenderers[i3].wallRenderer[i].sprite = shopManager.GetItemInfo(1, 1, wall[i3].wareNum).itemImage[i];
            }

            //window
            Sprite outside = shopManager.GetItemInfo(1, 3, window[i3].wareNum).itemImage[0];
            Sprite outline = shopManager.GetItemInfo(1, 3, window[i3].wareNum).itemImage[1];
            window[i3].SetWindow(outside, outline);
            //window
        }
    }

    void CheckHamsterWares()
    {
        for (int i = 0; i < rollor.Length; i++)
        {
            rollor[i].rolling = false;
        }
    }


    public void CompleteBtnActive(bool active)
    {
        placeCompleteBtn.interactable = active;
    }

    private void Update()
    {
        if (!placeMode) return;

        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);

            if (hit.collider != null)
            {
                if (!isPlacing)
                {
                    if (hit.collider.tag == "Dish")
                    {
                        movingWare = hit.collider.gameObject;
                        wareTypeCode = 0;
                        //movingWare.GetComponent<HousingColliderChecker>().WarePick();
                        isPlacing = true;
                    }
                    else if (hit.collider.tag == "WaterBowl")
                    {
                        movingWare = hit.collider.gameObject;
                        wareTypeCode = 1;
                        //movingWare.GetComponent<HousingColliderChecker>().WarePick();
                        isPlacing = true;
                    }
                    else if (hit.collider.tag == "Bath")
                    {
                        movingWare = hit.collider.gameObject;
                        wareTypeCode = 2;
                        //movingWare.GetComponent<HousingColliderChecker>().WarePick();
                        isPlacing = true;
                    }
                    else if (hit.collider.tag == "Rollor")
                    {
                        movingWare = hit.collider.gameObject;
                        wareTypeCode = 3;

                        isPlacing = true;
                    }
                    else if (hit.collider.tag == "House")
                    {
                        movingWare = hit.collider.gameObject;
                        wareTypeCode = 4;

                        isPlacing = true;
                    }
                    else if (hit.collider.tag == "WallDeco")
                    {
                        movingWare = hit.collider.gameObject;
                        wareTypeCode = 5;
                        //movingWare.GetComponent<HousingColliderChecker>().WarePick();
                        isPlacing = true;
                    }
                    else if (hit.collider.tag == "Window")
                    {
                        movingWare = hit.collider.gameObject;
                        wareTypeCode = 6;
                        //movingWare.GetComponent<HousingColliderChecker>().WarePick();
                        isPlacing = true;
                    }
                    else wareTypeCode = -1;

                    if (wareTypeCode != -1)
                        if (!wareOriginalPos[wareTypeCode].isMoved)
                        {
                            wareOriginalPos[wareTypeCode].isMoved = true;
                            wareOriginalPos[wareTypeCode].targetOriginalPos = movingWare.transform.position;
                            wareOriginalPos[wareTypeCode].targetTransform = movingWare.transform;
                        }

                    placeWareFlipBtn.interactable = true;
                }
            }
            if (isPlacing) CheckWareBarrier();
        }
        if (Input.GetMouseButtonUp(0) && movingWare)
        {
            isPlacing = false;

            placeOkayCount = 0;
            for (int i = 0; i < hamsterWaresRenderers[curPage].allHamsterWares.Length; i++)
            {
                if (hamsterWaresRenderers[curPage].allHamsterWares[i].activeSelf)
                {
                    hamsterWaresRenderers[curPage].allHamsterWares[i].GetComponent<HousingColliderChecker>().WareDown();
                    placeOkayCount++;
                }
            }
            wareCount = placeOkayCount;
            if (placeOkayCount == wareCount)
                CompleteBtnActive(true);
            else
                CompleteBtnActive(false);
        }
    }

    private void FixedUpdate()
    {
        if (moveCamera)
            mainCamera.transform.position = Vector3.Lerp(mainCamera.position, targetVector, Time.deltaTime * moveSpeed);
    }
    void CheckWareBarrier()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 hamsterPos = mousePos;
        int cageNum = gameManager.cageNum;
        float mainX = placeBarrierMainPos[wareTypeCode].x + gameManager.cage_transform[cageNum].position.x;
        float mainY = placeBarrierMainPos[wareTypeCode].y + gameManager.cage_transform[cageNum].position.y;

        if (mousePos.x < -(placeBarrier[wareTypeCode].x - mainX))
            hamsterPos.x = -(placeBarrier[wareTypeCode].x - mainX);
        else if (mousePos.x > placeBarrier[wareTypeCode].x + mainX)
            hamsterPos.x = placeBarrier[wareTypeCode].x + mainX;

        if (mousePos.y < -(placeBarrier[wareTypeCode].y - mainY))
            hamsterPos.y = -(placeBarrier[wareTypeCode].y - mainY);
        else if (mousePos.y > placeBarrier[wareTypeCode].y + mainY)
            hamsterPos.y = placeBarrier[wareTypeCode].y + mainY;

        movingWare.transform.position = hamsterPos;
    }

    /*
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(placeBarrierMainPos[0] + gameManager.cage_transform[gameManager.cageNum].position, placeBarrier[0] * 2);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(placeBarrierMainPos[1] + gameManager.cage_transform[gameManager.cageNum].position, placeBarrier[1] * 2);
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(placeBarrierMainPos[2] + gameManager.cage_transform[gameManager.cageNum].position, placeBarrier[2] * 2);
        Gizmos.color = Color.black;
        Gizmos.DrawWireCube(placeBarrierMainPos[3] + gameManager.cage_transform[gameManager.cageNum].position, placeBarrier[3] * 2);
        Gizmos.color = Color.gray;
        Gizmos.DrawWireCube(placeBarrierMainPos[4] + gameManager.cage_transform[gameManager.cageNum].position, placeBarrier[4] * 2);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(placeBarrierMainPos[5] + gameManager.cage_transform[gameManager.cageNum].position, placeBarrier[5] * 2);
    }*/
}
