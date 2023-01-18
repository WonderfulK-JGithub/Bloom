using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class slowmotion : MonoBehaviour
{
    public void ChangeTimeScale(float ts)
    {
        Time.timeScale = ts;
    }
}
