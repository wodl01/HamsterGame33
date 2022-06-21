using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class StoryScript : MonoBehaviour
{
    public Ui_ChangeManager uiChangeManager;
    public EpisodeManager episodeManager;

    public int storyIndex;
    [SerializeField] private bool isHamPisode;

    public itemInfo rewardItem;

    [Header("Pages")]
    [SerializeField] List<GameObject> storyPages;
    int curStoryPage;
    [SerializeField] int nickNameCheckPage;

    [Header("Uis")]
    [SerializeField] GameObject rewardPanel;
    [SerializeField] Image helpingSign;
    [SerializeField] GameObject episodeEndObject;
    [SerializeField] InputField hamsterNameInputField;
    [SerializeField] Text[] hamsterNameTexts;

    [Header("CurState")]
    public bool canPaging;
    public bool isAlreadyWatch;

    public HamsterAbility hamsterAbility;

    public void SettingInfos()
    {
        dongScript.instance.panelOn = true;
        if (rewardPanel != null)
            rewardPanel.SetActive(false);
        curStoryPage = 0;
        helpingSign.color = new Color(1, 1, 1, 1);
        ChangePage(0);

        if (isAlreadyWatch)
        {
            if(hamsterNameInputField)
            hamsterNameInputField.text = hamsterAbility.hamsterInfo.name_H;

            for (int i = 0; i < hamsterNameTexts.Length; i++)
                hamsterNameTexts[i].text = hamsterAbility.hamsterInfo.name_H;
        }
        if (hamsterNameInputField)
            hamsterNameInputField.GetComponent<Image>().raycastTarget = !isAlreadyWatch;
    }

    public void SetActiveAllFalse()
    {
        for (int i = 0; i < storyPages.Count; i++)
        {
            storyPages[i].SetActive(false);
        }
        episodeEndObject.SetActive(false);
    }

    public void ChangePage(int index)
    {
        if (!canPaging && index == 1) return;
        SetActiveAllFalse();

        curStoryPage += index;

        if (curStoryPage < 0) curStoryPage = 0;
        else if (curStoryPage == storyPages.Count) curStoryPage = storyPages.Count - 1;

        if (hamsterNameInputField)
            if (curStoryPage == nickNameCheckPage - 1 && !IsNickNameCurrectExact() && !isAlreadyWatch) canPaging = false;
            else canPaging = true;

        if (curStoryPage >= storyPages.Count - 1) episodeEndObject.SetActive(true);

        storyPages[curStoryPage].SetActive(true);
    }

    private void FixedUpdate()
    {
        if(helpingSign.color.a != 0)
            helpingSign.color = Color.Lerp(helpingSign.color, new Color(1, 1, 1, 0), Time.deltaTime * 2);
    }

    private bool IsNickNameCurrectExact()//닉네임 확인
    {
        if(!Regex.IsMatch(hamsterNameInputField.text, "^[0-9a-zA-Z가-힣]*$") || hamsterNameInputField.text == "")
            return false;
        else
            return true;
    }

    public void SettingHamsterName()
    {
        if(!IsNickNameCurrectExact())
        {
            canPaging = false;
        }
        else
        {
            canPaging = true;
            for (int i = 0; i < hamsterNameTexts.Length; i++)
                hamsterNameTexts[i].text = hamsterNameInputField.text;
        }
    }

    public void EndStory()
    {
        if (hamsterNameInputField)
            hamsterAbility.hamsterInfo.name_H = hamsterNameInputField.text;

        if (rewardItem.itemPrice != 0 && !isAlreadyWatch)
        {
            rewardPanel.transform.GetChild(2).GetComponent<Text>().text = rewardItem.itemPrice.ToString();
            rewardPanel.transform.GetChild(2).transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().sprite = rewardItem.itemThumbNail;
            rewardPanel.SetActive(true);


        }
        else
        {
            episodeManager.EpisodeEnd(storyIndex, isHamPisode);

            Destroy(gameObject);
        }
            
    }

    public void InputRewardBtn()
    {
        dongScript.instance.AddScoreValue(rewardItem.itemPrice, rewardItem.priceType);

        episodeManager.EpisodeEnd(storyIndex, isHamPisode);

        Destroy(gameObject);
    }
}
