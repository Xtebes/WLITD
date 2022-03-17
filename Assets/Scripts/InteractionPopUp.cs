using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class InteractionPopUp : MonoBehaviour
{
    TextMeshProUGUI text;
    CanvasGroup canvasGroup;
    Button button;
    Transform box;
    private void Start()
    {
        gameObject.SetActive(false);
        canvasGroup = GetComponent<CanvasGroup>();
        button = GetComponent<Button>();
        box = transform.GetChild(0);
        text = box.GetComponentInChildren<TextMeshProUGUI>();
        box.localScale = new Vector2(0.7f, 0.7f);
        canvasGroup.interactable = false;
        canvasGroup.alpha = 0;
        button.onClick.AddListener(PopDown);
    }
    public void PopUp(string text)
    {
        canvasGroup.interactable = false;
        this.text.text = text;
        box.DOScale(1, 0.3f);
        gameObject.SetActive(true);
        Tween tween = canvasGroup.DOFade(1, 0.3f);
        tween.onComplete += delegate { canvasGroup.interactable = true; };
    }
    public void PopDown()
    {
        canvasGroup.interactable = false;
        box.DOScale(0.7f, 0.3f);
        Tween tween = canvasGroup.DOFade(0, 0.3f);
        tween.onComplete += delegate { gameObject.SetActive(false); };
    }
}