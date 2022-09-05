using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour
{
    public void resume()
    {
        if (gameManager.instance.isPaused)
        {
            gameManager.instance.togglePauseMenu();
        }
    }

    public void restart()
    {
        gameManager.instance.togglePauseMenu();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void quit()
    {
        Application.Quit();
    }
}
