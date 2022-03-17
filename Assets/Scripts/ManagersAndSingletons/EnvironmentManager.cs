using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
public class EnvironmentManager : Singleton<EnvironmentManager>
{
    [SerializeField]
    private int flashMaxVariations;
    [SerializeField]
    private float flashMinGapTime, flashMaxGapTime, thunderMinGapTime, thunderMaxGapTime;
    [SerializeField]
    private AudioClip[] thunderAudios;
    [SerializeField]
    private AudioClip rainAudio;
    public AudioSource thunderSource, rainSource;
    [SerializeField]
    private Light2D environmentLight;
    private void Start()
    {
        rainSource.clip = rainAudio;
        rainSource.Play();
        DOTween.To(() => rainSource.volume, x => rainSource.volume = x, 1, 2);
        StartCoroutine(ThunderLoop());
    }
    private IEnumerator ThunderLoop()
    {
        while (true)
        {
            float secondsBeforeNextFlash = Random.Range(flashMinGapTime, flashMaxGapTime);
            yield return new WaitForSeconds(secondsBeforeNextFlash);
            float variations = 0;
            void StartThunderLightning()
            {
                variations++;
                if (variations >= flashMaxVariations)
                {
                    return;
                }
                Debug.Log(variations);
                DOTween.To(() => environmentLight.intensity, x => environmentLight.intensity = x, Random.Range(0.3f, 0.9f), Random.Range(0.1f, 0.3f))
                    .onComplete += StartThunderLightning;
            }
            StartThunderLightning();
            while (variations < flashMaxVariations)
            {
                yield return null;
            }
            DOTween.To(() => environmentLight.intensity, x => environmentLight.intensity = x, 0, Random.Range(0.1f, 0.3f));
            variations = 0;
            float secondsBeforeNextThunder = Random.Range(thunderMinGapTime, thunderMaxGapTime);
            yield return new WaitForSeconds(secondsBeforeNextThunder);
            thunderSource.clip = thunderAudios[Random.Range(0, thunderAudios.Length - 1)];
            thunderSource.Play();
        }
    }
}