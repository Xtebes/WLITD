using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class PlayerAnimationController : MonoBehaviour, ImLoadedByPlayer
{
    private Animator animator;
    [HideInInspector]
    public SpriteRenderer spriteRenderer;
    public int sortingLayerIndex;
    private PlayerInputController input;
    public Light2D flashLight;
    void ImLoadedByPlayer.Load(Player player)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        input = player.input;
    }
    void Update()
    {
        Vector3 playerToMouse = (transform.position - input.mousePositionInWorld).normalized;
        float directionDot = Vector3.Dot(input.directionalInput, playerToMouse);
        if (input.toggle == 0)
        {
            animator.SetFloat("Horizontal", playerToMouse.x);
            animator.SetFloat("Vertical", playerToMouse.y);
            spriteRenderer.flipX = playerToMouse.x < 0;
        }
        else
        {
            animator.SetFloat("Horizontal", input.directionalInput.x);
            animator.SetFloat("Vertical", input.directionalInput.y);
            spriteRenderer.flipX = input.directionalInput.x < 0;
        }
        animator.SetFloat("Speed", (input.directionalInput.magnitude + input.toggle) / 2f);
        animator.SetFloat("DirectionalSpeed", Mathf.Sign(directionDot));
        spriteRenderer.sortingLayerID = SortingLayer.layers[Mathf.CeilToInt(Vector3.Dot(gameObject.transform.up, flashLight.transform.up)) + 4].id;
    }
}
