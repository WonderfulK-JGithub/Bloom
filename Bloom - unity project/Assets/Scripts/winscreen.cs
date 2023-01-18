using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class winscreen : MonoBehaviour
{
    public Image black;
    void Start()
    {
        Invoke("ChangeSceneAnimation", 1);
    }

    void ChangeSceneAnimation()
    {
        GetComponentInChildren<Animator>().SetTrigger("fade");
    }

    void ChangeScene()
    {
        SceneManager.LoadScene(0);
    }
}
