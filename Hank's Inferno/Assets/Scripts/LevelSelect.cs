using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelect : MonoBehaviour
{
    public void GoToLevel(int sceneNum)
    {
        // Enklaste grejen n�gonsin.
        SceneManager.LoadScene(sceneNum);
    }
}
