using UnityEngine;

public class RestartGame : MonoBehaviour
{
    public void LoadCurrentScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Level1");
        Time.timeScale = 1;
    }
}