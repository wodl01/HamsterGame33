using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxAnimatorScript : MonoBehaviour
{
    [SerializeField] RandomBoxScript box;

    public void BoxOpenComplete()
    {
        box.OpenBoxComplete();
    }

    public void PlaySound(string soundName)
    {
        AudioManager.Play(soundName);
    }

    public void StopSound(string soundName)
    {
        AudioManager.Stop(soundName);
    }
}
