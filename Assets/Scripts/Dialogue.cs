using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Windows;

public class Dialogue : MonoBehaviour
{
    public string dialogue;
    public TMP_Text txt;
    public Animator anim;
    public AudioSource talking_as;
    public List<AudioClip> talking_clips = new List<AudioClip>();

    bool dialogue_displayed = false;
    bool still_talking = true;

    private void Start()
    {
        talking_as.clip = talking_clips[Random.Range(0, talking_clips.Count)];
    }

    public void StartDialogue()
    {
        if (dialogue_displayed)
            return;

        StartCoroutine(DisplayDialogue());
    }

    private void Update()
    {
        if (dialogue_displayed && still_talking)
        {
            if (!talking_as.isPlaying)
            {
                if (talking_as.volume == 0f)
                {
                    talking_as.volume = 1f;
                    still_talking = false;
                    return;
                }
                int newclip = Random.Range(0, talking_clips.Count - 1);
                if (talking_as.clip == talking_clips[newclip])
                {
                    if (newclip >= talking_clips.Count)
                    {
                        newclip -= 1;
                    }
                    else
                    {
                        newclip += 1;
                    }
                }
                talking_as.clip = talking_clips[newclip];
                talking_as.Play();
                
            }
        }
    }

    IEnumerator DisplayDialogue()
    {
        anim.SetTrigger("Talking");

        talking_as.Play();

        dialogue_displayed = true;

        string[] list = dialogue.Split('#');

        yield return new WaitForSeconds(.5f);

        foreach (string str in list)
        {
            string[] seperated = str.Split(':');
            txt.text = seperated[0] + ": ";
            for (int i = 0; i < seperated[1].Length; i++)
            {
                yield return new WaitForSeconds(0.03f);
                txt.text += seperated[1][i];
            }
            yield return new WaitForSeconds(3f);
        }

        txt.text = "";
        talking_as.volume = 0f;

        anim.SetTrigger("Shrink");
        GetComponent<NPC>().BeginFollow();
        Player.freeze = false;
    }
}
