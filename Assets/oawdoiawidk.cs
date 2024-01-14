using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class oawdoiawidk : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(adadawda());
    }

    IEnumerator adadawda()
    {
        yield return new WaitForSeconds(25f);
        SceneManager.LoadScene(0);
    }
}
