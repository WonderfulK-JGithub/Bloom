using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition current;
    Animator anim;

    int sceneToEnter;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        current = this;
    }

    public void EnterScene(int _sceneIndex)
    {
        sceneToEnter = _sceneIndex;
        anim.Play("Transition_Exit");
    }

    void LoadScene()
    {
        SceneManager.LoadScene(sceneToEnter);
    }
}
