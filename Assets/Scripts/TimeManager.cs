using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System;

#region Weader
[System.Serializable]
public class OWM_Coord
{
    public float lon;
    public float lat;
}

[System.Serializable]
public class OWM_Weather
{
    public int id;
    public string main;
    public string description;
    public string icon;
}

[System.Serializable]
public class OWM_Main
{
    public int temp;
    public float feels_like;
    public int temp_min;
    public int temp_max;
    public int pressure;
    public int humidity;
}

[System.Serializable]
public class OWM_Wind
{
    public float speed;
    public int deg;
}

[System.Serializable]
public class OWM_Clouds
{
    public int all;
}

[System.Serializable]
public class OWM_Sys
{
    public int type;
    public int id;
    public string country;
    public int sunrise;
    public int sunset;
}

[System.Serializable]
public class WeatherData
{
    public OWM_Coord coord;
    public OWM_Weather[] weather;
    public string basem;
    public OWM_Main main;
    public int visibility;
    public OWM_Wind wind;
    public OWM_Clouds clouds;
    public int dt;
    public OWM_Sys sys;
    public int timezone;
    public int id;
    public string name;
    public int cod;

}
#endregion
public class Country
{

    public string ip;
    public string city;
    public string region;
    public string country;
    public string loc;
    public string org;
    public string postal;
    public string timezone;
   
}



public class TimeManager : MonoBehaviour
{
    [SerializeField] TreeScript treeScript;

    [SerializeField] string url;

    [SerializeField] int curTime;


    [Header("Colors")]
    [SerializeField] Color[] outSideColors;
    [SerializeField] Color[] outSideColors_Rain;
    [SerializeField] Color[] starColors;
    Color changedSkyColor;
    [SerializeField] float changingSpeed;
    [SerializeField] SpriteRenderer skySpriteRenderer;
    [SerializeField] SpriteRenderer[] windowRenderer;
    [SerializeField] SpriteRenderer starSpriteRenderer;
    [SerializeField] bool changeColor;

    [Header("Weather")]
    [SerializeField] Animator[] weatherAnimator;
    [SerializeField] bool changingWeather;
    float changingWeatherTime;
    [SerializeField] GameObject cloudPrefap;
    private bool cloudActive;
    [SerializeField] int cloudCool;
    private float curCloudCool;

    public bool rainActive;
    private float rainChangeCool;
    [SerializeField] GameObject rainOb;
    [SerializeField] SpriteRenderer rainRenderer;
    [SerializeField] SpriteRenderer[] rainRenderer_window;
    [SerializeField] Sprite[] rainSprite;
    [SerializeField] Sprite[] rainSprite_window;

    public bool snowActive;
    private float snowChangeCool;
    [SerializeField] GameObject snowOb;
    [SerializeField] SpriteRenderer snowRenderer;
    [SerializeField] SpriteRenderer[] snowRenderer_window;
    [SerializeField] Sprite[] snowSprite;
    [SerializeField] Sprite[] snowSprite_window;


    [SerializeField] GameObject rainCloud;


    [SerializeField] private WeatherData weatherInfo;
    [SerializeField] private Country countryInfo;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DetectCountry());//날씨
    }

    public void CheckCityWeather(string city)
    {
        StartCoroutine(GetWeather(city));
    }

    IEnumerator DetectCountry()
    {
        string ip = new WebClient().DownloadString("http://icanhazip.com");
        string url = "https://ipinfo.io/";
        url += ip;
        url += "?token=4cc058b9c9dca1";

        UnityWebRequest request = UnityWebRequest.Get(url);
        request.chunkedTransfer = false;
        yield return request.Send();


        if (request.isNetworkError)
        {

        }
        else
        {
            if (request.isDone)
            {
                countryInfo = JsonUtility.FromJson<Country>(request.downloadHandler.text);

                StartCoroutine(GetWeather(countryInfo.city));
            }
        }
    }
    IEnumerator GetWeather(string city)
    {
        city = UnityWebRequest.EscapeURL(city);
        string url = "http://api.openweathermap.org/data/2.5/weather?q=";
        url += city;
        url += "&units=metric&appid=612eac35193e448f2213170cd110bdc1";
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        string json = www.downloadHandler.text;
        json = json.Replace("\"base\":", "\"basem\":");
        weatherInfo = JsonUtility.FromJson<WeatherData>(json);

        if (weatherInfo.weather.Length > 0)
        {

            StartCoroutine(WebChk());//시간
        }

    }


    IEnumerator Timer()
    {
        yield return new WaitForSeconds(600);
        StartCoroutine(DetectCountry());//날씨
    }

    IEnumerator WebChk()
    {
        UnityWebRequest request = new UnityWebRequest();
        using (request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.isNetworkError)
            {

            }
            else
            {
                string date = request.GetResponseHeader("date");

                DateTime dateTime = DateTime.Parse(date);
                curTime = dateTime.Hour;
            }

            starSpriteRenderer.color = starColors[curTime];
            WeatherEvent();
            StartCoroutine(Timer());
        }
    }




    void WeatherEvent()
    {
        cloudActive = false;
        rainActive = false;
        snowActive = false;
        switch (weatherInfo.weather[0].main)
        {
            case "Clear":
                changedSkyColor = outSideColors[curTime];
                for (int i = 0; i < weatherAnimator.Length; i++)
                    weatherAnimator[i].SetInteger("WeatherCode", 0);

                AudioManager.ChangeWeather("OutSide");
                break;
            case "Clouds":
                changedSkyColor = outSideColors[curTime];
                cloudCool = 15;
                cloudActive = true;
                for (int i = 0; i < weatherAnimator.Length; i++)
                    weatherAnimator[i].SetInteger("WeatherCode", 0);

                AudioManager.ChangeWeather("OutSide");
                break;
            case "Rain":
                changedSkyColor = outSideColors_Rain[curTime];
                rainActive = true;
                for (int i = 0; i < weatherAnimator.Length; i++)
                    weatherAnimator[i].SetInteger("WeatherCode", 1);

                AudioManager.ChangeWeather("Rain");
                break;
            case "Snow":
                changedSkyColor = outSideColors_Rain[curTime];
                snowActive = true;
                for (int i = 0; i < weatherAnimator.Length; i++)
                    weatherAnimator[i].SetInteger("WeatherCode", 2);

                AudioManager.ChangeWeather("OutSide");
                break;
        }
        changingWeatherTime = 5;
        changingWeather = true;

        changeColor = true;
    }

    int rainChangeNum;
    int snowChangeNum;
    private void FixedUpdate()
    {
        if (changeColor)
        {
            skySpriteRenderer.color = Color.Lerp(skySpriteRenderer.color, changedSkyColor, changingSpeed);
            for (int i = 0; i < windowRenderer.Length; i++)
                windowRenderer[i].color = Color.Lerp(windowRenderer[i].color, changedSkyColor, changingSpeed);

            if (skySpriteRenderer.color == changedSkyColor)
            {
                changeColor = false ;
            }
        }
        if (cloudActive)
        {
            curCloudCool -= Time.deltaTime;
            if(curCloudCool < 0)
            {
                //28 32
                Instantiate(cloudPrefap, new Vector3(26.5f, UnityEngine.Random.Range(28f, 32f), 0), Quaternion.identity);
                curCloudCool = UnityEngine.Random.Range(cloudCool - 5, cloudCool) ;
            }
        }
        if (changingWeather)
        {
            changingWeatherTime -= Time.deltaTime;
            if (changingWeatherTime < 0) changingWeather = false;

            rainChangeCool -= Time.deltaTime;
            if (rainChangeCool < 0)
            {
                rainChangeNum++;
                if (rainChangeNum == rainSprite.Length) rainChangeNum = 0;

                for (int i = 0; i < rainRenderer_window.Length; i++)
                    rainRenderer_window[i].sprite = rainSprite_window[rainChangeNum];

                rainRenderer.sprite = rainSprite[rainChangeNum];


                rainChangeCool = 0.5f;
            }
            snowChangeCool -= Time.deltaTime;
            if (snowChangeCool < 0)
            {
                snowChangeNum++;
                if (snowChangeNum == snowSprite.Length) snowChangeNum = 0;

                for (int i = 0; i < snowRenderer_window.Length; i++)
                    snowRenderer_window[i].sprite = snowSprite_window[snowChangeNum];

                snowRenderer.sprite = snowSprite[snowChangeNum];
                snowChangeCool = 0.75f;
            }
        }
        else
        {
            if (rainActive)
            {
                rainChangeCool -= Time.deltaTime;
                if (rainChangeCool < 0)
                {
                    rainChangeNum++;
                    if (rainChangeNum == rainSprite.Length) rainChangeNum = 0;

                    for (int i = 0; i < rainRenderer_window.Length; i++)
                        rainRenderer_window[i].sprite = rainSprite_window[rainChangeNum];

                    rainRenderer.sprite = rainSprite[rainChangeNum];
                    rainChangeCool = 0.5f;
                }
            }
            if (snowActive)
            {
                snowChangeCool -= Time.deltaTime;
                if (snowChangeCool < 0)
                {
                    snowChangeNum++;
                    if (snowChangeNum == snowSprite.Length) snowChangeNum = 0;

                    for (int i = 0; i < snowRenderer_window.Length; i++)
                        snowRenderer_window[i].sprite = snowSprite_window[snowChangeNum];

                    snowRenderer.sprite = snowSprite[snowChangeNum];
                    snowChangeCool = 0.75f;
                }
            }
        }
        
    }

    private void OnApplicationQuit()
    {


        //LogOut();
    }
}
