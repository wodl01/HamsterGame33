using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HousingColliderChecker : MonoBehaviour
{
    [SerializeField] HousingManager housingManager;
    [SerializeField] BoxCollider2D col;

    [SerializeField] string[] tags;

    private bool Once;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        for (int i = 0; i < tags.Length; i++)
        {
            if(collision.tag == tags[i])
            {
                //housingManager.CompleteBtnActive(false);
                if (Once)
                {
                    Once = false;
                    housingManager.placeOkayCount--;
                }
                Debug.Log("AAAAAAAAAAAEW");
            }
        }
    }


    public void WarePick()
    {
        col.enabled = false;
        housingManager.CompleteBtnActive(false);
    }
    public void WareDown()
    {
        Once = true;
        col.enabled = false;
        col.enabled = true;
        //housingManager.CompleteBtnActive(true);

    }
}
