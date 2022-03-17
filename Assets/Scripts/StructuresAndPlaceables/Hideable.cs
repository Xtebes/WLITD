using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.InputSystem;
using DG.Tweening;
public class Hideable : Interactable
{
    [SerializeField]
    private GameObject hidingPosition;
    private Vector3 lastPosition;
    private Light2D fieldOfView;
    Tween lightTween;
    private void Awake()
    {
        Load(Singleton<Player>.Instance);
    }
    void EnterHideable(Player player)
    {
        lastPosition = player.transform.position;
        onInteract -= ()=> EnterHideable(player);
        onInteract += ()=> ExitHideable(player);
        if (fieldOfView != null)
        {
            if (lightTween != null)
                DOTween.Kill(lightTween);
            lightTween = DOTween.To(() => fieldOfView.intensity, x => fieldOfView.intensity = x, 1, 0.5f);
        }
        info.text = "Press " + player.input.worldInteraction.GetBindingDisplayString() + " to hide";
    }
    void ExitHideable(Player player)
    {
        player.transform.position = lastPosition;
        onInteract -= () => ExitHideable(player);
        onInteract += ()=> EnterHideable(player);
        if (fieldOfView != null)
        {
            if (lightTween != null)
                DOTween.Kill(lightTween);
            lightTween = DOTween.To(() => fieldOfView.intensity, x => fieldOfView.intensity = x, 0, 0.5f);
        }
        info.text = "Press " + player.input.worldInteraction.GetBindingDisplayString() + " to exit";
    }
    public void Load(Player player)
    {        
        fieldOfView = GetComponentInChildren<Light2D>();
        info.text = "Press " + player.input.worldInteraction.GetBindingDisplayString() + " to hide";
        onInteract += ()=> EnterHideable(player);
    }
}