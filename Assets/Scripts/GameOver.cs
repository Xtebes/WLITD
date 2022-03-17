using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class GameOver : MonoBehaviour
{
    private bool isGameOver = false;
    public GameObject gameOverCanvas;
    [SerializeField]
    float timer = 0;
    public float timerMax;
    bool isCountingUp = false;
    Coroutine currentTimer;
    private void Update()
    {
        
    }
    private IEnumerator TimerUp(PlayerInputController player)
    {
        while (true)
        {
            timer += Time.deltaTime;
            if (timer > timerMax)
            {
                player.inputActionAsset.Disable();
                isGameOver = true;
                MainMenuUI menu = gameOverCanvas.GetComponent<MainMenuUI>();
                gameOverCanvas.SetActive(true);
                CanvasGroup canvasGroup = gameOverCanvas.GetComponent<CanvasGroup>();
                Tween tween = canvasGroup.DOFade(1, 0.5f);
                tween.onComplete += delegate { canvasGroup.interactable = true; };
                SmartPhoneController smartPhone = FindObjectOfType<SmartPhoneController>(true);
                if (smartPhone.gameObject.activeSelf)
                    smartPhone.ToggleSmartPhone(true);
                Singleton<EnvironmentManager>.Instance.StopAllCoroutines();
                Singleton<EnvironmentManager>.Instance.rainSource.Stop();
            }
            yield return null;
        }
    }
    private IEnumerator TimerDown()
    {
        while (true)
        {
            timer = Mathf.Max(timer - Time.deltaTime, 0);
            yield return null;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        PlayerInputController player;
        player = collision.gameObject.GetComponent<PlayerInputController>();
        if (player != null && !isGameOver && isCountingUp)
        {
            isCountingUp = false;
            if (currentTimer != null)
            StopCoroutine(currentTimer);
            currentTimer = StartCoroutine(TimerDown());
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerInputController player;
        player = collision.gameObject.GetComponent<PlayerInputController>();
        if (player != null && !isGameOver)
        {
            if (!isCountingUp)
            {
                if (currentTimer != null)
                StopCoroutine(currentTimer);
                currentTimer = StartCoroutine(TimerUp(player));
                isCountingUp = true;
            }
        }
    }
}