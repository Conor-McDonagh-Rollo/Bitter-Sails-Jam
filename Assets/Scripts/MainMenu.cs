using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public BridgeCreator bc;
    public GameObject settingsObj;
    public Slider slider;
    public AudioSource music;

    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        bc.StartBridge();
    }

    public void Settings()
    {
        settingsObj.SetActive(true);
    }

    public void ChangeVolume()
    {
        music.volume = slider.value;
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        StartCoroutine(startGame());
    }

    IEnumerator startGame()
    {
        animator.Play("playgame");
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(1);
    }
}
