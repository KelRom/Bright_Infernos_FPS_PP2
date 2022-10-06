using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    #region title menu button functions
    public void newGame()
    {
        StartCoroutine(titleScreen.instance.transitionScene());
    }

    public void continueGame()
    {
        //This is just a placeHolder till we get everything finished and implemented with the save system
        StartCoroutine(titleScreen.instance.transitionScene());
    }

    public void goToNextMenu()
    {
        titleScreen.instance.cirleTransition.SetActive(false);
        StartCoroutine(titleScreen.instance.showNextMenu(this.name));
    }
    #endregion
}