using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class StaminaBarUI : MonoBehaviour, ImLoadedByPlayer
{
    private Slider slider;
    private CanvasGroup canvasGroup;
    private Tween tween;
    [SerializeField][Range(0f,1f)]
    private float minOpacity, maxOpacity;
    private IEnumerator BarBehaviour(PlayerMovementController player)
    {
        while (true)
        {
            slider.value = player.currentStamina;
            yield return null;
        }
    }
    void ImLoadedByPlayer.Load(Player player)
    {
        slider = GetComponentInChildren<Slider>();
        canvasGroup = GetComponent<CanvasGroup>();
        player.input.runAction.started += delegate 
        {
            if (player.movement.currentStamina > player.movement.staminaThreshold)
            {
                if (tween != null) 
                    DOTween.Kill(tween);
                tween = canvasGroup.DOFade(maxOpacity, 0.3f);
            } 
        };
        player.input.runAction.performed += delegate 
        { 
            if (tween != null) 
                DOTween.Kill(tween); 
            tween = canvasGroup.DOFade(minOpacity, 0.3f); 
        };
        slider.minValue = 0;
        slider.maxValue = player.movement.maxStamina;
        StartCoroutine(BarBehaviour(player.movement));
    }
}
