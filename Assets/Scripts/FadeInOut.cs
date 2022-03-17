using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class FadeInOut : MonoBehaviour
{
    public static CanvasGroup canvasGroup;
    public static VideoPlayer player;
    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        player = GetComponent<VideoPlayer>();
    }
    public static IEnumerator FadeInAndOut(float FadeInTime ,float duration, float FadeOutTime)
    {
        canvasGroup.DOFade(1, FadeInTime);
        yield return new WaitForSeconds(duration);
        canvasGroup.DOFade(0, FadeOutTime);
    }
    public static IEnumerator FadeInAndOutCutscene(float FadeInTime, VideoClip video, float fadeOutTime)
    {
        Singleton<GameManager>.Instance.isOnCutscene = true;
        canvasGroup.DOFade(1,FadeInTime);
        yield return new WaitForSeconds(FadeInTime);
        player.clip = video;
        player.Play();
        yield return new WaitForSeconds((float)video.length);
        canvasGroup.DOFade(0, fadeOutTime);
        Singleton<GameManager>.Instance.isOnCutscene = false;
    }
}
