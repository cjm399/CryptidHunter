using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashScreen : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Image logo;
    [SerializeField] private float fadeInTime;
    [SerializeField] private float fadeOutTime;
    [SerializeField] private float holdTime;
    [SerializeField] private int levelToLoad;


    void Start()
    {
        logo.color = new Color(1, 1, 1, 0);
        StartCoroutine(LerpLogo());
    }

    private IEnumerator LerpLogo()
    {
        yield return new WaitForSeconds(1);
        float percent = 0;
        float startTime = Time.time;
        Color startColor = new Color(1, 1, 1, 0);
        Color endColor = new Color(1, 1, 1, 1);

        while(percent < 1)
        {
            percent = (Time.time - startTime) / fadeInTime;

            logo.color = Color.Lerp(startColor, endColor, percent);
            yield return null;
        }

        yield return new WaitForSeconds(holdTime);

        percent = 0;
        startTime = Time.time;

        while(percent  < 1)
        {
            percent = (Time.time - startTime) / fadeOutTime;

            logo.color = Color.Lerp(endColor, startColor, percent);
            yield return null;
        }

        yield return new WaitForSeconds(1);

        UnityEngine.SceneManagement.SceneManager.LoadScene(levelToLoad);
    }
}
