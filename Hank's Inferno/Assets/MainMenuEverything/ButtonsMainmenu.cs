using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonsMainmenu : MonoBehaviour
{
    public CallMenu callMenuScript; 

    void Start()
    {

    }

    public void Resume()
    {
        if (callMenuScript != null)
        {
            callMenuScript.CloseMenu();  
        }
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;  
        SceneManager.LoadScene(0);  
    }
}
