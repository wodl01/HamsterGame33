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

    [Header("Episode")]
    public List<EpisodeInfo> episodeInfos;
    public List<EpisodeInfo> hamPisodeInfos;
    [SerializeField] Transform episodeUiTransform;


    private void Start()
    {
        //StoryEventManagement();
        //uiChangeManager.GetHamster(0);
        //uiChangeManager.GetHamster(1);
        //uiChangeManager.GetHamster(2);
    }

    public void StoryEventManagement()
    {

        EpisodeStart(true, 0, false);
        EpisodeStart(false, 2, false);
        EpisodeStart(false, 1, false);
        EpisodeStart(false, 0, false);


        HamsterLvEvent();

    }

    public void HamsterLvEvent()
    {
        if (hamsterUnion.GetTotalLv() >= 4)
        {
            EpisodeStart(true, 1, false);
        }
        if (hamsterUnion.GetTotalLv() >= 7)
        {
            EpisodeStart(true, 2, false);
        }
        if (hamsterUnion.GetTotalLv() >= 10)
        {
            EpisodeStart(true, 3, false);
        }
        if (hamsterUnion.GetTotalLv() >= 13)
        {
            EpisodeStart(true, 4, false);
        }
        if (hamsterUnion.GetTotalLv() >= 16)
        {
            EpisodeStart(true, 5, false);
        }
        if (hamsterUnion.GetTotalLv() >= 19)
        {
            EpisodeStart(true, 6, false);
        }
        if (hamsterUnion.GetTotalLv() >= 21)
        {
            EpisodeStart(true, 7, false);
        }
        if (hamsterUnion.GetTotalLv() >= 24)
        {
            EpisodeStart(true, 8, false);
        }
        if (hamsterUnion.GetTotalLv() >= 27)
        {
            EpisodeStart(true, 9, false);
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
        /*for (int i = 0; i < episodeInfos.Count; i++)
        {
            if (episodeInfos[i].episodePrefap.activeSelf)
            {
                exist = true;
                break;
            }
        }
        if(!exist)
            for (int i = 0; i < hamPisodeInfos.Count; i++)
            {
                if (hamPisodeInfos[i].episodePrefap.activeSelf)
                {
                    exist = true;
                    break;
                }
            }*/
        if (episodeUiTransform.childCount != 0)
            exist = true;
        gameManager.panelOn = exist;

    }

    public void EpisodeBtnOnClick(int index) => EpisodeStart(false, index, true);
    public void HamPisodeBtnOnClick(int index) => EpisodeStart(true, index, true);

    #region EpisodeActive
    public void EpisodeStart(bool isHamPisode, int index, bool isReplay)
    {
        EpisodeInfo curEpisode = isHamPisode ? hamPisodeInfos[index] : episodeInfos[index];
        if (!isReplay && curEpisode.isWatch) return;


        GameObject episodeOb = Instantiate(curEpisode.episodePrefap);

        //StoryScript storyScript = curEpisode.episodePrefap.transform.GetChild(0).GetComponent<StoryScript>();

        episodeOb.transform.parent = episodeUiTransform;
        episodeOb.transform.position = new Vector3(0, 0, 0);
        episodeOb.transform.localScale = new Vector3(1, 1, 1);

        StoryScript storyScript = episodeOb.GetComponent<StoryScript>();

        storyScript.storyIndex = index;
        storyScript.isAlreadyWatch = curEpisode.isWatch;
        if (curEpisode.hamsterAbility)
            storyScript.hamsterAbility = curEpisode.hamsterAbility;
        storyScript.episodeManager = this;

        storyScript.SettingInfos();

        AudioManager.Play("Touch1");
    }

    public void EpisodeEnd(int index, bool isHamPisode)
    {
        if (isHamPisode)
        {
            hamPisodeInfos[index].isWatch = true;
            uiChangeManager.GetHamster(index);


        }
        else
        {
            episodeInfos[index].isWatch = true;


        }


        Invoke("CheckEpisodeExist", 0.001f);
        AudioManager.Play("Touch1");
    }

    #endregion
}
