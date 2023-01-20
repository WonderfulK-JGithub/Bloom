using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ESC : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && Input.GetKey(KeyCode.LeftControl))
        {
            SceneManager.LoadScene(0);
        }
    }
}
