using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

public class titleScreen : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Animator fadeScreen;
    [SerializeField] Animator titleFade;
    public static titleScreen instance;

    private void Awake()
    {
        instance = this;
    }

    public IEnumerator transitionScene()
    {
        titleFade.SetTrigger("TitleFade");
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
