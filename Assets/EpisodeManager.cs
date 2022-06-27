using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable] 
public class EpisodeInfo
{
    public GameObject episodePrefap;
    public HamsterAbility hamsterAbility;
    public Sprite[] hamsterSprite;
    public bool isWatch;
    public bool isPlayList;
    public bool isSummoned;
}

public class EpisodeManager : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] dongScript gameManager;
    [SerializeField] HamsterUnionManager hamsterUnion;
    [SerializeField] Ui_ChangeManager uiChangeManager;


    [Header("Objects")]
    [SerializeField] GameObject[] menuObjects;
    [SerializeField] GameObject[] episodeGroup;
    [SerializeField] GameObject[] hampisodeGroup;


    [Header("Uis")]
    [SerializeField] GameObject[] episodeBtns;
    [SerializeField] GameObject[] hamPisodeBtns;
    [SerializeField] GameObject episodeOpenBtn;

    [Header("Episode")]
    public List<EpisodeInfo> episodeInfos;
    public List<EpisodeInfo> hamPisodeInfos;
    [SerializeField] Transform episodeUiTransform;

    [SerializeField] private List<EpisodeInfo> episodePlayList;


    public void StoryEventManagement()
    {
        RightSummonEpisode(true, 0, false);
        RightSummonEpisode(false, 2, false);
        RightSummonEpisode(false, 1, false);
        RightSummonEpisode(false, 0, false);


        HamsterLvEvent();

    }

    public void HamsterLvEvent()
    {
        if (hamsterUnion.GetTotalLv() >= 4)
        {
            AddEpisodeList(true, 1);
        }
        if (hamsterUnion.GetTotalLv() >= 7)
        {
            AddEpisodeList(true, 2);
        }
        if (hamsterUnion.GetTotalLv() >= 10)
        {
            AddEpisodeList(true, 3);
        }
        if (hamsterUnion.GetTotalLv() >= 13)
        {
            AddEpisodeList(true, 4);
        }
        if (hamsterUnion.GetTotalLv() >= 16)
        {
            AddEpisodeList(true, 5);
        }
        if (hamsterUnion.GetTotalLv() >= 19)
        {
            AddEpisodeList(true, 6);
        }
        if (hamsterUnion.GetTotalLv() >= 21)
        {
            AddEpisodeList(true, 7);
        }
        if (hamsterUnion.GetTotalLv() >= 24)
        {
            AddEpisodeList(true, 8);
        }
        if (hamsterUnion.GetTotalLv() >= 27)
        {
            AddEpisodeList(true, 9);
        }
    }


    public void OpenHamsterStoryPanel()
    {
        EpisodePanelUpdate();
        HamPisodePanelUpdate();
    }
    public void MenuOnClick(int index)
    {
        menuObjects[index].transform.SetAsLastSibling();
    }
    void EpisodePanelUpdate()
    {
        for (int i = 0; i < episodeInfos.Count; i++)
        {
            episodeBtns[i].transform.GetChild(1).GetComponent<Text>().text = (i + 1).ToString();
            episodeBtns[i].transform.GetChild(2).gameObject.SetActive(!episodeInfos[i].isWatch);
        }
    }
    void HamPisodePanelUpdate()
    {
        for (int i = 0; i < hamPisodeInfos.Count; i++)
        {
            EpisodeInfo curEpi = hamPisodeInfos[i];
            hamPisodeBtns[i].transform.GetChild(1).GetComponent<Image>().sprite = curEpi.hamsterSprite[curEpi.isWatch ? 1 : 0];
            hamPisodeBtns[i].transform.GetChild(1).GetComponent<Image>().color = curEpi.isWatch ? new Color(1, 1, 1) : new Color(0.3f, 0.3f, 0.3f);
            hamPisodeBtns[i].transform.GetChild(2).gameObject.SetActive(!curEpi.isWatch);
        }
    }



    public void CheckEpisodeExist()
    {
        bool exist = false;

        if (episodeUiTransform.childCount != 0)
            exist = true;
        gameManager.panelOn = exist;

        if (episodePlayList.Count == 0) episodeOpenBtn.SetActive(false);
    }
    bool IsEpisodeExist()
    {
        return episodeUiTransform.childCount != 0;
    }

    public void EpisodeBtnOnClick(int index) => RightSummonEpisode(false, index, true);
    public void HamPisodeBtnOnClick(int index) => RightSummonEpisode(true, index, true);

    #region EpisodeActive

    public void AddEpisodeList(bool isHampisode, int index)
    {
        EpisodeInfo curEpi = isHampisode ? hamPisodeInfos[index] : episodeInfos[index];
        if (!curEpi.isWatch)
            episodePlayList.Add(curEpi);
    }
    public void RightSummonEpisode(bool isHampisode, int index, bool isReplay)
    {
        EpisodeInfo curEpi = isHampisode ? hamPisodeInfos[index] : episodeInfos[index];
        if (curEpi.isWatch && !isReplay) return;
        GameObject episodeOb = Instantiate(curEpi.episodePrefap);
        Debug.Log("!!!");

        episodeOb.transform.parent = episodeUiTransform;
        episodeOb.transform.position = new Vector3(0, 0, 0);
        episodeOb.transform.localScale = new Vector3(1, 1, 1);

        StoryScript storyScript = episodeOb.GetComponent<StoryScript>();

        storyScript.isAlreadyWatch = curEpi.isWatch;
        if (curEpi.hamsterAbility)
            storyScript.hamsterAbility = curEpi.hamsterAbility;
        storyScript.episodeManager = this;

        storyScript.SettingInfos();

        AudioManager.Play("Touch1");
    }
    public void SummonEpisodeList()
    {
        GameObject episodeOb = Instantiate(episodePlayList[0].episodePrefap);
        Debug.Log("!!!");

        episodeOb.transform.parent = episodeUiTransform;
        episodeOb.transform.position = new Vector3(0, 0, 0);
        episodeOb.transform.localScale = new Vector3(1, 1, 1);

        StoryScript storyScript = episodeOb.GetComponent<StoryScript>();

        storyScript.isAlreadyWatch = episodePlayList[0].isWatch;
        if (episodePlayList[0].hamsterAbility)
            storyScript.hamsterAbility = episodePlayList[0].hamsterAbility;
        storyScript.episodeManager = this;

        storyScript.SettingInfos();
        episodePlayList[0].isPlayList = false;
        episodePlayList[0].isSummoned = true;
        episodePlayList.RemoveAt(0);


        AudioManager.Play("Touch1");
    }

    public void EpisodeEnd(int index, bool isHamPisode)
    {
        if (isHamPisode)
        {
            hamPisodeInfos[index].isWatch = true;
            uiChangeManager.GetHamster(index);

            hamPisodeInfos[index].isSummoned = false;
        }
        else
        {
            episodeInfos[index].isWatch = true;

            episodeInfos[index].isSummoned = false;
        }


        Invoke("CheckEpisodeExist", 0.001f);
        AudioManager.Play("Touch1");
    }

    #endregion
}
