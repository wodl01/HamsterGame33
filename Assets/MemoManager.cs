using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoInfo
{

}

public class MemoManager : MonoBehaviour
{
    [SerializeField] Ui_ChangeManager uiChangeManager;
    [SerializeField] HousingManager housing;

    [SerializeField] int maxMemoAmount;
    [SerializeField] int curMemoAmount;

    [SerializeField] GameObject memoObject;

    float memoCool;
    private void FixedUpdate()
    {
        if (!housing.placeMode)
            memoCool -= Time.deltaTime;
        if(memoCool < 0)
        {
            memoCool = 60;

        }
    }

    void SummonMemo()
    {
        //햄스터의 위치에 생성
        //Vector2 randomPos = new Vector2(midPos.x + Random.Range(-4, 4), midPos.y + Random.Range(-8, 2));
        //Instantiate(memoObject,)
    }
}
