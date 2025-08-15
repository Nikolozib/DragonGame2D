using UnityEngine;

public class SceneManager : MonoBehaviour
{
    void Update()
    {

    }

    public void FirstLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("1Level");
    }

    public void SecondLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("2Level");
    }

    public void ThirdLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("3Level");
    }

    public void FourthLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("4Level");
    }

    public void FifthLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("5Level");
    }

    public void SixLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("6Level");
    }

    public void BackToMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
