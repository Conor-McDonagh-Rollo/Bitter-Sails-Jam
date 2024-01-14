using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelFinish : MonoBehaviour
{
    public Animator animator;

    public TMP_Text txt;

    public int buildIndexNum;

    bool ending = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NPC") && !ending)
        {
            ending = true;
            animator.Play("finishScene");
            StartCoroutine(levelEnd());
        }
    }

    IEnumerator levelEnd()
    {
        Player.freeze = true;

        string s = "Level finished!";
        for (int i = 0; i < s.Length; i++)
        {
            yield return new WaitForSeconds(0.2f);
            txt.text = txt.text + s[i];
        }

        yield return new WaitForSeconds(1);

        for (int i = s.Length - 1; i >= 0; i--)
        {
            yield return new WaitForSeconds(0.2f);
            txt.text = txt.text.Remove(txt.text.Length - 1, 1);
        }

        yield return new WaitForSeconds(.5f);

        Player.freeze = false;

        SceneManager.LoadScene(buildIndexNum);
    }
}
