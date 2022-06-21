using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] GameObject bgmGroupOb;
    public static AudioSource[] audioSource;
    public static AudioSource[] bgmSource;
    public static Dictionary<string, AudioSource> sounds;
    public static Dictionary<string, AudioSource> bgms;

    private void Awake()
    {
        audioSource = GetComponents<AudioSource>();
        bgmSource = bgmGroupOb.GetComponents<AudioSource>();
        sounds = new Dictionary<string, AudioSource>();
        bgms = new Dictionary<string, AudioSource>();

        bgms.Add("Bgm1", bgmSource[0]);
        bgms.Add("OutSide", bgmSource[1]);
        bgms.Add("Rain", bgmSource[2]);

        sounds.Add("Touch1", audioSource[0]);

        sounds.Add("Cancel1", audioSource[1]);

        sounds.Add("Eating", audioSource[2]);
        sounds.Add("WaterRefill", audioSource[3]);
        sounds.Add("DrinkWater", audioSource[4]);

        sounds.Add("Coin", audioSource[5]);

        sounds.Add("BalloonPoop", audioSource[6]);

        sounds.Add("Window1", audioSource[7]);
        sounds.Add("Window2", audioSource[8]);

        sounds.Add("OpenChest", audioSource[9]);
        sounds.Add("Car", audioSource[10]);
        sounds.Add("Holy", audioSource[11]);

        sounds.Add("BuyItem", audioSource[12]);
        sounds.Add("QuestComplete", audioSource[13]);

        sounds.Add("LvUp", audioSource[14]);
    }
    //  SoundManager.Play("Effect_Getcombine");

    public static void PlayBGM(string BGMName, float bgmValue)
    {
        if (!bgms[BGMName].isPlaying)
            bgms[BGMName].Play();

        if (bgmValue != -1)
            bgms[BGMName].volume = bgmValue;
    }
    public static void StopBGM(string BGMName)
    {
        bgms[BGMName].Stop();
    }

    public static void ChangeWeather(string WeatherType)
    {
        if (bgms[WeatherType].isPlaying) return;

        bgms["OutSide"].Stop();
        bgms["Rain"].Stop();

        bgms[WeatherType].Play();
    }

    public static void Play(string soundName)
    {
        if (sounds.ContainsKey(soundName))
        {
            sounds[soundName].Play();
        }
    }

    public static void Stop(string soundName)
    {
        if (sounds.ContainsKey(soundName))
        {
            sounds[soundName].Stop();
        }
    }
}
