using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FeverScript : MonoBehaviour
{
    [SerializeField] dongScript gameManager;
    [SerializeField] Ui_ChangeManager UiChangeMamager;

    [SerializeField] Text ticketText;

    [SerializeField] GameObject mainScreenUi;
    [SerializeField] GameObject feverPanel;

    [SerializeField] GameObject[] feverBalloonPrefaps;
    [SerializeField] GameObject balloonPrefap;

    [SerializeField] float curDuringTime;
    [SerializeField] float maxDuringTime;

    [SerializeField] float curSpawnCool;
    [SerializeField] float maxSpawnCool;

    Vector3 feverPos;

    int finalValue;

    private void Start()
    {
        FeverPanelUpdate();
    }

    public void FeverPanelUpdate()
    {
        ticketText.text = gameManager.GetScoreValue(priceType.feverTicket).ToString();
    }

    public void FeverBtnInput()
    {
        if (gameManager.GetScoreValue(priceType.feverTicket) == 0 || curDuringTime > 0) return;

        mainScreenUi.SetActive(false);
        feverPanel.SetActive(true);

        gameManager.AddScoreValue(-1, priceType.feverTicket);
        FeverPanelUpdate();
        SetFinalValue();

        feverPos = gameManager.cage_transform[gameManager.cageNum].position;

        BalloonScript balloon1 = Instantiate(feverBalloonPrefaps[0], new Vector3(feverPos.x - 4, feverPos.y - 12, 0), Quaternion.identity).GetComponent<BalloonScript>();
        balloon1.gameManager = gameManager;
        balloon1.scoreValue = finalValue;
        BalloonScript balloon2 = Instantiate(feverBalloonPrefaps[1], new Vector3(feverPos.x - 2, feverPos.y - 12.5f, 0), Quaternion.identity).GetComponent<BalloonScript>();
        balloon2.gameManager = gameManager;
        balloon2.scoreValue = finalValue;
        BalloonScript balloon3 = Instantiate(feverBalloonPrefaps[2], new Vector3(feverPos.x - 0, feverPos.y - 12, 0), Quaternion.identity).GetComponent<BalloonScript>();
        balloon3.gameManager = gameManager;
        balloon3.scoreValue = finalValue;
        BalloonScript balloon4 = Instantiate(feverBalloonPrefaps[3], new Vector3(feverPos.x + 2, feverPos.y - 12.5f, 0), Quaternion.identity).GetComponent<BalloonScript>();
        balloon4.gameManager = gameManager;
        balloon4.scoreValue = finalValue;
        BalloonScript balloon5 = Instantiate(feverBalloonPrefaps[4], new Vector3(feverPos.x + 4, feverPos.y - 12, 0), Quaternion.identity).GetComponent<BalloonScript>();
        balloon5.gameManager = gameManager;
        balloon5.scoreValue = finalValue;

        curDuringTime = maxDuringTime;
        curSpawnCool = 1;

        switch (gameManager.cageNum)
        {
            case 0:
                for (int i = 0; i < UiChangeMamager.cage1HamsterList.Count; i++)
                    UiChangeMamager.cage1HamsterList[i].GetComponent<Move>().ani.SetTrigger("Interaction1Start");
                break;
            case 1:
                for (int i = 0; i < UiChangeMamager.cage2HamsterList.Count; i++)
                    UiChangeMamager.cage2HamsterList[i].GetComponent<Move>().ani.SetTrigger("Interaction1Start");
                break;
            case 2:
                for (int i = 0; i < UiChangeMamager.cage3HamsterList.Count; i++)
                    UiChangeMamager.cage3HamsterList[i].GetComponent<Move>().ani.SetTrigger("Interaction1Start");
                break;
        }
    }

    void SetFinalValue()
    {
        float finalIndex = 0;
        switch (gameManager.cageNum)
        {
            case 0:
                for (int i = 0; i < UiChangeMamager.cage1HamsterList.Count; i++)
                    finalIndex += UiChangeMamager.cage1HamsterList[i].balance.money[UiChangeMamager.cage1HamsterList[i].hamsterInfo.lv];
                finalIndex = finalIndex / UiChangeMamager.cage1HamsterList.Count + 1;
                break;
            case 1:
                for (int i = 0; i < UiChangeMamager.cage2HamsterList.Count; i++)
                    finalIndex += UiChangeMamager.cage2HamsterList[i].balance.money[UiChangeMamager.cage2HamsterList[i].hamsterInfo.lv];
                finalIndex = finalIndex / UiChangeMamager.cage2HamsterList.Count + 1;
                break;
            case 2:
                for (int i = 0; i < UiChangeMamager.cage3HamsterList.Count; i++)
                    finalIndex += UiChangeMamager.cage3HamsterList[i].balance.money[UiChangeMamager.cage3HamsterList[i].hamsterInfo.lv];
                finalIndex = finalIndex / UiChangeMamager.cage3HamsterList.Count + 1;
                break;
        }
        Mathf.RoundToInt(finalIndex);
        finalIndex = finalIndex * 2;

        finalValue = (int)finalIndex;
    }

    private void FixedUpdate()
    {
        if(curDuringTime > 0)
        {
            curDuringTime -= Time.deltaTime;

            curSpawnCool -= Time.deltaTime;
            if(curSpawnCool < 0)
            {
                curSpawnCool = maxSpawnCool;
                float randomPosition = Random.Range(-4, 4);
                BalloonScript balloon = Instantiate(balloonPrefap, new Vector3(randomPosition, feverPos.y - 12, 0), Quaternion.identity).GetComponent<BalloonScript>();
                balloon.gameManager = gameManager;
                balloon.scoreValue = finalValue;


            }
            if(curDuringTime < 0)
            {
                feverPanel.SetActive(false);
                mainScreenUi.SetActive(true);
            }

        }
    }
}
