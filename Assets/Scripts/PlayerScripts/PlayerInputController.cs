using System;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerInputController : MonoBehaviour, ImLoadedByPlayer
{
    public InputActionAsset inputActionAsset;
    [HideInInspector]
    public InputAction moveAction, toggleFlashlightAction, runAction, togglePhone, worldInteraction;
    public Action worldInteractionAction;
    public Vector3 mousePositionInWorld;
    public Vector2 directionalInput;
    public int toggle;
    void ImLoadedByPlayer.Load(Player player)
    {
        moveAction = inputActionAsset.FindAction("Move");
        toggleFlashlightAction = inputActionAsset.FindAction("ToggleFlashlight");
        runAction = inputActionAsset.FindAction("Run");
        togglePhone = inputActionAsset.FindAction("TogglePhone");
        worldInteraction = inputActionAsset.FindAction("ActionOnMap");
        worldInteraction.performed += delegate { worldInteractionAction?.Invoke(); };
    }
    private void Update()
    {
        mousePositionInWorld = Mouse.current.position.ReadValue();
        mousePositionInWorld.z = -20;
        mousePositionInWorld = Camera.main.ScreenToWorldPoint(mousePositionInWorld);     
        directionalInput = moveAction.ReadValue<Vector2>();
        toggle = (int)runAction.ReadValue<float>();
    }
}
