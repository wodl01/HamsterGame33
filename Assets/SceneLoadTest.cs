using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoadTest : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        SceneChange.LoadingScene(sceneName);
    }
}
