using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuAnimations : MonoBehaviour
{
    public float waitBeforeIntro = 1;
    public float logoFadeSpeed = 1;
    public float waitBeforeInput = 1;
    public float inputTextFadeInSpeed = 2;
    public float inputTextMoveDownSpeed = 1;
    public float inputTextFadeOutSpeed = 2;
    public float logoMoveSpeed = 1;
    public float buttonFadeSpeed = 2;
    public float buttonFadeDelay = 0.1f;
    bool awaitInput = false;
    [Space]
    public Image logo;
    public RectTransform logoTargetPos;
    public Image bg;
    public TextMeshProUGUI anyKeyText;
    private void Start()
    {
        logoTargetPos.GetComponent<Image>().color = new Color(1, 1, 1, 0);

        bg.color = new Color(0, 0, 0, 1);
        anyKeyText.color = new Color(anyKeyText.color.r, anyKeyText.color.b, anyKeyText.color.g, 0);
        logo.color = new Color(1, 1, 1, 0);

        StartCoroutine(FadeLogo());
    }

    IEnumerator FadeLogo()
    {
        yield return new WaitForSeconds(waitBeforeIntro);
        while (logo.color.a < 1)
        {
            bg.color += new Color(Time.deltaTime * logoFadeSpeed, Time.deltaTime * logoFadeSpeed, Time.deltaTime * logoFadeSpeed, 1);
            logo.color += new Color(0, 0, 0, Time.deltaTime * logoFadeSpeed);
            yield return 0;
        }
        yield return new WaitForSeconds(waitBeforeInput);
        while (anyKeyText.color.a < 1)
        {
            anyKeyText.color += new Color(anyKeyText.color.r, anyKeyText.color.b, anyKeyText.color.g, Time.deltaTime * inputTextFadeInSpeed);
            yield return 0;
        }
        awaitInput = true;
    }

    private void Update()
    {
        if (awaitInput)
        {
            if (Input.anyKeyDown)
            {
                awaitInput = false;
                StartCoroutine(LoadMenu());
            }
        }
    }

    IEnumerator LoadMenu()
    {
        while (anyKeyText.color.a > 0)
        {
            anyKeyText.GetComponent<RectTransform>().localPosition -= new Vector3(0, Time.deltaTime * inputTextMoveDownSpeed * 100, 0);
            anyKeyText.color -= new Color(anyKeyText.color.r, anyKeyText.color.b, anyKeyText.color.g, Time.deltaTime * inputTextFadeOutSpeed);
            yield return 0;
        }

        float t = 0;
        Vector3 a = logo.GetComponent<RectTransform>().localPosition;
        while (t < 1)
        {
            logo.GetComponent<RectTransform>().localPosition = Vector3.Lerp(a, logoTargetPos.localPosition, t);
            t += Time.deltaTime * logoMoveSpeed;
            yield return 0;
        }


    }

    IEnumerator FadeButton()
    {
        yield return 0;
    }
}
