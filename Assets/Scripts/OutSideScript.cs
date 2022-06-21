using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OutSideScript : MonoBehaviour
{
    [SerializeField] dongScript gameManager;
    [SerializeField] TimeManager timeManager;

    [SerializeField] GameObject mainUi;

    public Vector2 centerPos;
    public Vector2 size;
    public Transform spawnPoint;

    private Vector3 Origin;
    private Vector3 Diference;
    private bool Drag = false;


    void LateUpdate()
    {
        if (gameManager.isOutSide)
        {
            if (Input.GetMouseButton(0))
            {
                Diference = (Camera.main.ScreenToWorldPoint(Input.mousePosition)) - Camera.main.transform.position;
                if (Drag == false)
                {
                    Drag = true;
                    Origin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                }
            }
            else
            {
                Drag = false;
            }

            if (Drag == true)
            {
                Vector3 cameraPos = Camera.main.transform.position;
                Vector3 finalVector = Origin - Diference;

                Camera.main.transform.position = finalVector;
                Camera.main.transform.position = new Vector3(
    Mathf.Clamp(Camera.main.transform.position.x, -size.x + centerPos.x, size.x + centerPos.x),
    Mathf.Clamp(Camera.main.transform.position.y, -size.y + centerPos.y, size.y + centerPos.y), cameraPos.z);
            }
        }
    }

    public void GoOutside(bool outing)
    {
        gameManager.isOutSide = outing;
        mainUi.SetActive(!outing);


        if (outing)
        {
            AudioManager.Play("Window1");
            Camera.main.transform.position = new Vector3(-size.x + centerPos.x, size.y + centerPos.y, -10);

            AudioManager.PlayBGM("Bgm1", 0.2f);
            if (timeManager.rainActive)
            {
                AudioManager.PlayBGM("Rain", 0.8f);
            }
            else
            {
                AudioManager.PlayBGM("OutSide", 0.8f);
            }
        }
        else
        {
            AudioManager.Play("Window2");
            gameManager.ChangeCameraView(0);

            AudioManager.PlayBGM("Bgm1", 0.5f);
            AudioManager.PlayBGM("Rain", 0);
            if (timeManager.rainActive)
            {
                AudioManager.PlayBGM("Rain", 0.5f);
            }
            else
            {
                AudioManager.PlayBGM("OutSide", 0.5f);
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawWireCube(centerPos, size * 2);
    }
}
