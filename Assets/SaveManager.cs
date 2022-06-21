using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    [SerializeField] private string url;
    private float passingTimeInGame;
    [SerializeField] int passedTime;
    [SerializeField] int loginedTime;

    [SerializeField]
    private TimeManager timeManager;
    [SerializeField]
    private TreeScript treeManager;
    [SerializeField]
    private dongScript gameManager;
    [SerializeField]
    private Ui_ChangeManager uiChangeManager;
    [SerializeField]
    private EpisodeManager episodeManager;
    [SerializeField]
    private QuestManager questManager;
    [SerializeField]
    private HousingManager housingManager;
    [SerializeField]
    private ShopManager shopManager;

    TimeSpan timeStamp;

    private void Start()
    {
        instance = this;
        GetAllPlayerPrefs();
        StartCoroutine(GetLogined(0));
    }
    public IEnumerator GetLogined(int eventNum)
    {
        UnityWebRequest request = new UnityWebRequest();
        using (request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.isNetworkError)
            {
                Debug.Log(request.error);
            }
            else
            {
                string date = request.GetResponseHeader("date");

                DateTime dateTime = DateTime.Parse(date);
                timeStamp = dateTime - new DateTime(2022, 1, 1, 0, 0, 0);
                switch (eventNum)
                {
                    case 0:
                        loginedTime = (int)timeStamp.TotalSeconds;
                        passedTime = (int)timeStamp.TotalSeconds - PlayerPrefs.GetInt("time");
                        treeManager.GetLogined(passedTime);

                        break;
                }

            }
        }
    }





    void SetAllPlayerPrefs()
    {
        PlayerPrefs.SetInt("cageAmount", gameManager.cageAmount);
        PlayerPrefs.SetInt("fruitAmount", treeManager.GetFruitAmount());

        #region Story
        for (int i = 0; i < episodeManager.episodeInfos.Count; i++)
        {
            EpisodeInfo curEpisode = episodeManager.episodeInfos[i];
            string playerPrefsCode = "Episode" + i.ToString();

            PlayerPrefs.SetInt(playerPrefsCode, curEpisode.isWatch ? 1 : 0);
        }
        for (int i = 0; i < episodeManager.hamPisodeInfos.Count; i++)
        {
            EpisodeInfo curEpisode = episodeManager.hamPisodeInfos[i];
            string playerPrefsCode = "Hampisode" + i.ToString();

            PlayerPrefs.SetInt(playerPrefsCode, curEpisode.isWatch ? 1 : 0);
        }
        #endregion

        PlayerPrefs.SetInt("QuestIndex", questManager.GetQuestIndex());

        #region Goods
        PlayerPrefs.SetInt("GoldAmount", gameManager.GetScoreValue(priceType.coin));
        PlayerPrefs.SetInt("HamTicketAmount", gameManager.GetScoreValue(priceType.hamTicket));
        PlayerPrefs.SetInt("NormalDirTicketAmount", gameManager.GetScoreValue(priceType.normalDirTicket));
        PlayerPrefs.SetInt("SpecialDirTicketAmount", gameManager.GetScoreValue(priceType.specialDirTicket));
        PlayerPrefs.SetInt("CustomDirTicketAmount", gameManager.GetScoreValue(priceType.customDirTicket));
        PlayerPrefs.SetInt("FeverTicketAmount", gameManager.GetScoreValue(priceType.feverTicket));
        #endregion

        #region ShopItem
        for (int i = 0; i < shopManager.itemPageinfos.Count; i++)
        {
            for (int i2 = 0; i2 < shopManager.itemPageinfos[i].itemMenuInfos.Count; i2++)
            {
                menuInfo curMenuInfo = shopManager.itemPageinfos[i].itemMenuInfos[i2];
                for (int i3 = 0; i3 < shopManager.itemPageinfos[i].itemMenuInfos[i2].itemInfos.Count; i3++)
                {
                    itemInfo curItemInfo = shopManager.itemPageinfos[i].itemMenuInfos[i2].itemInfos[i3];
                    string itemCode = "ShopItem" + i.ToString() + i2.ToString() + i3.ToString();

                    PlayerPrefs.SetInt(itemCode, curItemInfo.curBuyAmount);
                }
            }
        }
        #endregion

        for (int i = 0; i < uiChangeManager.hamsterOb.Length; i++)
        {
            HamsterInfo curHamster = uiChangeManager.hamsterOb[i].hamsterInfo;
            string playerPrefsCode = "HamsterInfo" + i.ToString();
            string curHamsterInfo = JsonUtility.ToJson(curHamster);

            PlayerPrefs.SetString(playerPrefsCode, curHamsterInfo);
        }

        #region HamsterWare

        for (int i = 0; i < housingManager.dish.Length; i++)
        {
            DishScript curWare = housingManager.dish[i];
            string playerPrefsCode = "Dish" + i.ToString();
            string isFlip = curWare.isFlip ? "1" : "0";

            Vector3 warePosition = curWare.transform.position;
            WareOriginalPosition movingWare = housingManager.wareOriginalPos[0];
            if (housingManager.placeMode && movingWare.isMoved && movingWare.targetTransform.gameObject == curWare.gameObject)
                warePosition = movingWare.targetOriginalPos;

            string curHamsterInfo = JsonUtility.ToJson(warePosition) + "/"
                + curWare.wareNum + "/"
                + curWare.foodGuage + "/"
                + isFlip;
            PlayerPrefs.SetString(playerPrefsCode, curHamsterInfo);
        }
        for (int i = 0; i < housingManager.waterBowl.Length; i++)
        {
            WaterBowlScript curWare = housingManager.waterBowl[i];
            string playerPrefsCode = "WaterBowl" + i.ToString();
            string isFlip = curWare.isFlip ? "1" : "0";

            Vector3 warePosition = curWare.transform.position;
            WareOriginalPosition movingWare = housingManager.wareOriginalPos[1];
            if (housingManager.placeMode && movingWare.isMoved && movingWare.targetTransform.gameObject == curWare.gameObject)
                warePosition = movingWare.targetOriginalPos;

            string curHamsterInfo = JsonUtility.ToJson(warePosition) + "/"
                + curWare.wareNum + "/"
                + isFlip;
            PlayerPrefs.SetString(playerPrefsCode, curHamsterInfo);
        }
        for (int i = 0; i < housingManager.rollor.Length; i++)
        {
            RollorScript curWare = housingManager.rollor[i];
            string playerPrefsCode = "Rollor" + i.ToString();

            Vector3 warePosition = curWare.transform.position;
            WareOriginalPosition movingWare = housingManager.wareOriginalPos[2];
            if (housingManager.placeMode && movingWare.isMoved && movingWare.targetTransform.gameObject == curWare.gameObject)
                warePosition = movingWare.targetOriginalPos;

            string curHamsterInfo = JsonUtility.ToJson(warePosition) + "/"
                + curWare.wareNum;
            PlayerPrefs.SetString(playerPrefsCode, curHamsterInfo);
        }
        for (int i = 0; i < housingManager.bath.Length; i++)
        {
            BathScript curWare = housingManager.bath[i];
            string playerPrefsCode1 = "Bath" + i.ToString();
            string playerPrefsCode2 = "Sand" + i.ToString();

            Vector3 warePosition = curWare.transform.position;
            WareOriginalPosition movingWare = housingManager.wareOriginalPos[3];
            if (housingManager.placeMode && movingWare.isMoved && movingWare.targetTransform.gameObject == curWare.gameObject)
                warePosition = movingWare.targetOriginalPos;

            string curHamsterInfo1 = JsonUtility.ToJson(warePosition) + "/"
                + curWare.bathWareNum;
            string curHamsterInfo2 = JsonUtility.ToJson(warePosition) + "/"
+ curWare.bathWareNum;
            PlayerPrefs.SetString(playerPrefsCode1, curHamsterInfo1);
            PlayerPrefs.SetString(playerPrefsCode2, curHamsterInfo2);
        }
        for (int i = 0; i < housingManager.house.Length; i++)
        {
            HouseScript curWare = housingManager.house[i];
            string playerPrefsCode = "House" + i.ToString();

            Vector3 warePosition = curWare.transform.position;
            WareOriginalPosition movingWare = housingManager.wareOriginalPos[4];
            if (housingManager.placeMode && movingWare.isMoved && movingWare.targetTransform.gameObject == curWare.gameObject)
                warePosition = movingWare.targetOriginalPos;

            string curHamsterInfo = JsonUtility.ToJson(warePosition) + "/"
                + curWare.wareNum;
            PlayerPrefs.SetString(playerPrefsCode, curHamsterInfo);
        }
        for (int i = 0; i < housingManager.wallDeco.Length; i++)
        {
            WallDecoScript curWare = housingManager.wallDeco[i];
            string playerPrefsCode = "WallDeco" + i.ToString();

            Vector3 warePosition = curWare.transform.position;
            WareOriginalPosition movingWare = housingManager.wareOriginalPos[5];
            if (housingManager.placeMode && movingWare.isMoved && movingWare.targetTransform.gameObject == curWare.gameObject)
                warePosition = movingWare.targetOriginalPos;

            string curHamsterInfo = JsonUtility.ToJson(warePosition) + "/"
                + curWare.wareNum;
            PlayerPrefs.SetString(playerPrefsCode, curHamsterInfo);
        }
        for (int i = 0; i < housingManager.wall.Length; i++)
        {
            WallScript curWare = housingManager.wall[i];
            string playerPrefsCode = "Wall" + i.ToString();

            string curHamsterInfo = JsonUtility.ToJson(curWare.transform.position) + "/"
                + curWare.wareNum;
            PlayerPrefs.SetString(playerPrefsCode, curHamsterInfo);
        }
        for (int i = 0; i < housingManager.bedding.Length; i++)
        {
            BeddingScript curWare = housingManager.bedding[i];
            string playerPrefsCode = "Bedding" + i.ToString();

            string curHamsterInfo = JsonUtility.ToJson(curWare.transform.position) + "/"
                + curWare.wareNum;
            PlayerPrefs.SetString(playerPrefsCode, curHamsterInfo);
        }
        for (int i = 0; i < housingManager.window.Length; i++)
        {
            WindowScript curWare = housingManager.window[i];
            string playerPrefsCode = "Window" + i.ToString();

            Vector3 warePosition = curWare.transform.position;
            WareOriginalPosition movingWare = housingManager.wareOriginalPos[5];
            if (housingManager.placeMode && movingWare.isMoved && movingWare.targetTransform.gameObject == curWare.gameObject)
                warePosition = movingWare.targetOriginalPos;

            string curHamsterInfo = JsonUtility.ToJson(warePosition) + "/"
                + curWare.wareNum;
            PlayerPrefs.SetString(playerPrefsCode, curHamsterInfo);
        }


        #endregion
    }

    void GetAllPlayerPrefs()
    {
        if (PlayerPrefs.HasKey("cageAmount"))
            gameManager.cageAmount = PlayerPrefs.GetInt("cageAmount");
        gameManager.CageUpdate();
        #region Story
        for (int i = 0; i < episodeManager.episodeInfos.Count; i++)
        {
            EpisodeInfo curEpisode = episodeManager.episodeInfos[i];
            string playerPrefsCode = "Episode" + i.ToString();

            if (PlayerPrefs.HasKey(playerPrefsCode))
            {
                int curHamsterInfo = PlayerPrefs.GetInt(playerPrefsCode);
                episodeManager.episodeInfos[i].isWatch = curHamsterInfo == 1 ? true : false;
            }
        }
        for (int i = 0; i < episodeManager.hamPisodeInfos.Count; i++)
        {
            EpisodeInfo curEpisode = episodeManager.hamPisodeInfos[i];
            string playerPrefsCode = "Hampisode" + i.ToString();

            if (PlayerPrefs.HasKey(playerPrefsCode))
            {
                int curHamsterInfo = PlayerPrefs.GetInt(playerPrefsCode);
                episodeManager.hamPisodeInfos[i].isWatch = curHamsterInfo == 1 ? true : false;
            }
        }

        #endregion



        #region Goods

        if (PlayerPrefs.HasKey("GoldAmount"))
            gameManager.AddScoreValue(PlayerPrefs.GetInt("GoldAmount"), priceType.coin);
        if (PlayerPrefs.HasKey("HamTicketAmount"))
            gameManager.AddScoreValue(PlayerPrefs.GetInt("HamTicketAmount"), priceType.hamTicket);
        if (PlayerPrefs.HasKey("NormalDirTicketAmount"))
            gameManager.AddScoreValue(PlayerPrefs.GetInt("NormalDirTicketAmount"), priceType.normalDirTicket);
        if (PlayerPrefs.HasKey("SpecialDirTicketAmount"))
            gameManager.AddScoreValue(PlayerPrefs.GetInt("SpecialDirTicketAmount"), priceType.specialDirTicket);
        if (PlayerPrefs.HasKey("CustomDirTicketAmount"))
            gameManager.AddScoreValue(PlayerPrefs.GetInt("CustomDirTicketAmount"), priceType.customDirTicket);
        if (PlayerPrefs.HasKey("FeverTicketAmount"))
            gameManager.AddScoreValue(PlayerPrefs.GetInt("FeverTicketAmount"), priceType.feverTicket);
        #endregion

        #region ShopItem
        for (int i = 0; i < shopManager.itemPageinfos.Count; i++)
        {
            for (int i2 = 0; i2 < shopManager.itemPageinfos[i].itemMenuInfos.Count; i2++)
            {
                menuInfo curMenuInfo = shopManager.itemPageinfos[i].itemMenuInfos[i2];
                for (int i3 = 0; i3 < shopManager.itemPageinfos[i].itemMenuInfos[i2].itemInfos.Count; i3++)
                {
                    itemInfo curItemInfo = shopManager.itemPageinfos[i].itemMenuInfos[i2].itemInfos[i3];
                    string itemCode = "ShopItem" + i.ToString() + i2.ToString() + i3.ToString();
                    if (PlayerPrefs.HasKey(itemCode))
                    {
                        int itemInfo = PlayerPrefs.GetInt(itemCode);
                        curItemInfo.curBuyAmount = itemInfo;
                    }
                }
            }
        }
        #endregion

        #region HamsterInfo
        for (int i = 0; i < uiChangeManager.hamsterOb.Length; i++)
        {
            HamsterInfo curHamster = uiChangeManager.hamsterOb[i].hamsterInfo;
            string playerPrefsCode = "HamsterInfo" + i.ToString();
            if (PlayerPrefs.HasKey(playerPrefsCode))
            {
                string curHamsterInfo = PlayerPrefs.GetString(playerPrefsCode);
                uiChangeManager.hamsterOb[i].hamsterInfo = JsonUtility.FromJson<HamsterInfo>(curHamsterInfo);
            }
        }
        uiChangeManager.HamsterListUpdate();

        episodeManager.StoryEventManagement();
        #endregion


        for (int i = 0; i < housingManager.dish.Length; i++)
        {
            DishScript curWare = housingManager.dish[i];
            string playerPrefsCode = "Dish" + i.ToString();
            if (PlayerPrefs.HasKey(playerPrefsCode))
            {
                string curWareInfo = PlayerPrefs.GetString(playerPrefsCode);
                string[] wareInfos = curWareInfo.Split('/');
                curWare.transform.position = JsonUtility.FromJson<Vector3>(wareInfos[0]);
                curWare.wareNum = int.Parse(wareInfos[1]);
                curWare.foodGuage = int.Parse(wareInfos[2]);
                curWare.isFlip = wareInfos[3] == "1" ? true : false;
            }
        }
        for (int i = 0; i < housingManager.waterBowl.Length; i++)
            {
                WaterBowlScript curWare = housingManager.waterBowl[i];
                string playerPrefsCode = "WaterBowl" + i.ToString();
                if (PlayerPrefs.HasKey(playerPrefsCode))
                {
                    string curWareInfo = PlayerPrefs.GetString(playerPrefsCode);
                    string[] wareInfos = curWareInfo.Split('/');
                    curWare.transform.position = JsonUtility.FromJson<Vector3>(wareInfos[0]);
                    curWare.wareNum = int.Parse(wareInfos[1]);
                    curWare.isFlip = wareInfos[2] == "1" ? true : false;
                }
            }
            for (int i = 0; i < housingManager.rollor.Length; i++)
            {
                RollorScript curWare = housingManager.rollor[i];
                string playerPrefsCode = "Rollor" + i.ToString();
                if (PlayerPrefs.HasKey(playerPrefsCode))
                {
                    string curWareInfo = PlayerPrefs.GetString(playerPrefsCode);
                    string[] wareInfos = curWareInfo.Split('/');
                    curWare.transform.position = JsonUtility.FromJson<Vector3>(wareInfos[0]);
                    curWare.wareNum = int.Parse(wareInfos[1]);
                }
            }
            for (int i = 0; i < housingManager.bath.Length; i++)
            {
                BathScript curWare = housingManager.bath[i];
                string playerPrefsCode1 = "Bath" + i.ToString();
                string playerPrefsCode2 = "Sand" + i.ToString();
                if (PlayerPrefs.HasKey(playerPrefsCode1))
                {
                    string curWareInfo = PlayerPrefs.GetString(playerPrefsCode1);
                    string[] wareInfos = curWareInfo.Split('/');
                    curWare.transform.position = JsonUtility.FromJson<Vector3>(wareInfos[0]);
                    curWare.bathWareNum = int.Parse(wareInfos[1]);
                }
                if (PlayerPrefs.HasKey(playerPrefsCode2))
                {
                    string curWareInfo = PlayerPrefs.GetString(playerPrefsCode2);
                    string[] wareInfos = curWareInfo.Split('/');
                    curWare.transform.position = JsonUtility.FromJson<Vector3>(wareInfos[0]);
                    curWare.sandWareNum = int.Parse(wareInfos[1]);
                }
            }
        for (int i = 0; i < housingManager.house.Length; i++)
        {
            HouseScript curWare = housingManager.house[i];
            string playerPrefsCode = "House" + i.ToString();
            if (PlayerPrefs.HasKey(playerPrefsCode))
            {
                string curWareInfo = PlayerPrefs.GetString(playerPrefsCode);
                string[] wareInfos = curWareInfo.Split('/');
                curWare.transform.position = JsonUtility.FromJson<Vector3>(wareInfos[0]);
                curWare.wareNum = int.Parse(wareInfos[1]);
            }
        }
        for (int i = 0; i < housingManager.wallDeco.Length; i++)
            {
                WallDecoScript curWare = housingManager.wallDeco[i];
                string playerPrefsCode = "WallDeco" + i.ToString();
                if (PlayerPrefs.HasKey(playerPrefsCode))
                {
                    string curWareInfo = PlayerPrefs.GetString(playerPrefsCode);
                    string[] wareInfos = curWareInfo.Split('/');
                    curWare.transform.position = JsonUtility.FromJson<Vector3>(wareInfos[0]);
                    curWare.wareNum = int.Parse(wareInfos[1]);
                }
            }
            for (int i = 0; i < housingManager.wall.Length; i++)
            {
                WallScript curWare = housingManager.wall[i];
                string playerPrefsCode = "Wall" + i.ToString();
                if (PlayerPrefs.HasKey(playerPrefsCode))
                {
                    string curWareInfo = PlayerPrefs.GetString(playerPrefsCode);
                    string[] wareInfos = curWareInfo.Split('/');
                    curWare.transform.position = JsonUtility.FromJson<Vector3>(wareInfos[0]);
                    curWare.wareNum = int.Parse(wareInfos[1]);
                }
            }
            for (int i = 0; i < housingManager.bedding.Length; i++)
            {
                BeddingScript curWare = housingManager.bedding[i];
                string playerPrefsCode = "Bedding" + i.ToString();
                if (PlayerPrefs.HasKey(playerPrefsCode))
                {
                    string curWareInfo = PlayerPrefs.GetString(playerPrefsCode);
                    string[] wareInfos = curWareInfo.Split('/');
                    curWare.transform.position = JsonUtility.FromJson<Vector3>(wareInfos[0]);
                    curWare.wareNum = int.Parse(wareInfos[1]);
                }
            }
            for (int i = 0; i < housingManager.window.Length; i++)
            {
                WindowScript curWare = housingManager.window[i];
                string playerPrefsCode = "Window" + i.ToString();
                if (PlayerPrefs.HasKey(playerPrefsCode))
                {
                    string curWareInfo = PlayerPrefs.GetString(playerPrefsCode);
                    string[] wareInfos = curWareInfo.Split('/');
                    curWare.transform.position = JsonUtility.FromJson<Vector3>(wareInfos[0]);
                    curWare.wareNum = int.Parse(wareInfos[1]);
                }
            }
        
        housingManager.HamsterWaresUpdate();

        if (PlayerPrefs.HasKey("QuestIndex"))
        {
            int questIndex = PlayerPrefs.GetInt("QuestIndex");
            questManager.SetQuestIndex(questIndex);
        }
        questManager.QuestPanelUpdate();
        episodeManager.CheckEpisodeExist();

        gameManager.myHamsterHeartOb.SetActive(uiChangeManager.CheckCanLvUpHamsterExist());

    }

    bool resetActive = false;


    private void OnApplicationQuit()
    {
        if (!resetActive)
        {
            SetAllPlayerPrefs();


        }
        LogOut();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SetAllPlayerPrefs();


            LogOut();
        }
        else
        {

        }
    }

    public void LogOut()
    {
        int passedTimeInGame = loginedTime + Mathf.RoundToInt(passingTimeInGame) - treeManager.GetTimeValue(0) - (treeManager.GetTimeValue(1) * 60);

        if (!resetActive)
            PlayerPrefs.SetInt("time", passedTimeInGame);
    }


    public void AllDatasReset()
    {
        PlayerPrefs.DeleteAll();
        resetActive = true;

        Application.Quit();
    }
    private void FixedUpdate()
    {
        passingTimeInGame += Time.deltaTime;
    }
}
