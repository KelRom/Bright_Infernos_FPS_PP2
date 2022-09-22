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
            gameManager.instance.isPaused = !gameManager.instance.isPaused;
            gameManager.instance.cursorUnlockUnpause();
        }
    }

    public void restart()
    {
        gameManager.instance.cursorUnlockUnpause();
        gameManager.instance.playerScript.Reset();
        SceneManager.LoadScene(1);

    }

    public void restartLevel() 
    {
        gameManager.instance.cursorUnlockUnpause();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void quit()
    {
        Application.Quit();
    }

    public void respawn()
    {
        gameManager.instance.isPaused = !gameManager.instance.isPaused;
        gameManager.instance.playerScript.playerRespawn();
        gameManager.instance.cursorUnlockUnpause();
    }
}