using System.Collections;
using UnityEngine;

public class CallMenu : MonoBehaviour
{
    public GameObject menuUI;  
    public float slideDuration = 0.5f;  
    public Vector3 hiddenPosition = new Vector3(0, 450, 0);
    public Vector3 visiblePosition = new Vector3(0, 40, 0);

    private bool isMenuOpen = false;

    void Start()
    {
        if (menuUI != null)
        {
            menuUI.transform.localPosition = hiddenPosition; 
            menuUI.SetActive(false);  
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isMenuOpen)
            {
                OpenMenu();
            }
            else
            {
                CloseMenu();
            }
        }
    }

    void OpenMenu()
    {
        isMenuOpen = true;
        Time.timeScale = 0f;  
        menuUI.SetActive(true);  
        StartCoroutine(SlideMenu(hiddenPosition, visiblePosition, slideDuration));
    }

    public void CloseMenu()
    {
        isMenuOpen = false;
        StartCoroutine(SlideMenu(visiblePosition, hiddenPosition, slideDuration, () => {
            menuUI.SetActive(false); 
            Time.timeScale = 1f; 
        }));
    }

    IEnumerator SlideMenu(Vector3 start, Vector3 end, float duration, System.Action onComplete = null)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            menuUI.transform.localPosition = Vector3.Lerp(start, end, (elapsedTime / duration));
            elapsedTime += Time.unscaledDeltaTime;  
            yield return null;
        }
        menuUI.transform.localPosition = end;
        onComplete?.Invoke();  
    }
}
