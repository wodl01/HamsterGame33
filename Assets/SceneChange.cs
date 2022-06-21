using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    static string nextScene;

    [SerializeField]
    Slider progressBar;
    [SerializeField]
    GameObject loadingPanel;


    private void Start()
    {
        Application.targetFrameRate = 60;

        if (Application.isMobilePlatform)
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            float scaleHeight = ((float)Screen.width / Screen.height) / ((float)9 / 16);
            float scaleWidth = 1f / scaleHeight;
            Rect rect = Camera.main.rect;
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
            Camera.main.rect = rect;
        }
        else
        {
            SetResolution(1080, 1920);
        }
    }
    public void SetResolution(int width, int height)
    {
        int setWidth = width; // 사용자 설정 너비
        int setHeight = height; // 사용자 설정 높이

        int deviceWidth = Screen.width; // 기기 너비 저장
        int deviceHeight = Screen.height; // 기기 높이 저장

        Screen.SetResolution(setWidth, (int)(((float)deviceHeight / deviceWidth) * setWidth), true); // SetResolution 함수 제대로 사용하기

        if ((float)setWidth / setHeight < (float)deviceWidth / deviceHeight) // 기기의 해상도 비가 더 큰 경우
        {
            float newWidth = ((float)setWidth / setHeight) / ((float)deviceWidth / deviceHeight); // 새로운 너비
            Camera.main.rect = new Rect((1f - newWidth) / 2f, 0f, newWidth, 1f); // 새로운 Rect 적용
        }
        else // 게임의 해상도 비가 더 큰 경우
        {
            float newHeight = ((float)deviceWidth / deviceHeight) / ((float)setWidth / setHeight); // 새로운 높이
            Camera.main.rect = new Rect(0f, (1f - newHeight) / 2f, 1f, newHeight); // 새로운 Rect 적용
        }
    }
    public void StartBtn()
    {
        loadingPanel.SetActive(true);
        LoadingScene("SampleScene");
        StartCoroutine(LoadSceneProcess());
    }
    public static void LoadingScene(string sceneName)
    {
        nextScene = sceneName;

        //SceneManager.LoadScene("LoadingScene");
    }
    IEnumerator LoadSceneProcess()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;

        float timer = 0f;
        while (!op.isDone)
        {
            yield return null;

            if(op.progress < 0.9f)
            {
                progressBar.value = op.progress;
            }
            else
            {
                timer += Time.deltaTime;
                progressBar.value = Mathf.Lerp(0.9f, 1f, timer);
                if(progressBar.value >= 1f)
                {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }
    }
}
