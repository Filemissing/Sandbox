using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    [SerializeField] CanvasGroup winScreen;
    [SerializeField] CanvasGroup loseScreen;

    public void ShowLoseScreen()
    {
        loseScreen.alpha = 1f;
        loseScreen.blocksRaycasts = true;
    }
    public void ShowWinScreen()
    {
        winScreen.alpha = 1f;
        winScreen.blocksRaycasts = true;
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
