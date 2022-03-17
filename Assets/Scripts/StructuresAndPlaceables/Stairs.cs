using UnityEngine;
using DG.Tweening;
using System;

public class Stairs : Interactable
{
    public Stairs targetStairs;
    public static readonly int transitionDuration = 2;
    [HideInInspector]
    public BoxCollider2D boxCollider2D;
    public static Action<Stairs> onPlayerUse;
    Vector3 stairDirection;
    public enum AxisDirection
    {
        None = 0,
        Positive = 1,
        Negative = -1,
    }
    [SerializeField]
    public AxisDirection UpAxis, RightAxis;
    private void Awake()
    {
        onPlayerUse = null;
    }
    private void Start()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
        stairDirection = new Vector3((int)RightAxis, (int)UpAxis);
    }
    public override void OnPlayerEnter(Player player)
    {
        UseStairs(player);
    }
    private void UseStairs(Player player)
    {
        FadeInOut.FadeInAndOut(transitionDuration/2,0.1f,transitionDuration/2);
        BoxCollider2D targetTrigger = targetStairs.GetComponent<BoxCollider2D>();
        BoxCollider2D thisTrigger = GetComponent<BoxCollider2D>();
        onPlayerUse?.Invoke(this);
        player.input.inputActionAsset.Disable();
        targetTrigger.enabled = false;
        thisTrigger.enabled = false;       
        Tween tween = player.transform.DOMove(player.gameObject.transform.position + stairDirection, transitionDuration/2).SetEase(Ease.Linear);
        tween.onComplete += delegate 
        {
            player.transform.position = targetStairs.transform.position - (Vector3)player.GetComponent<CapsuleCollider2D>().offset - new Vector3(targetStairs.transform.localScale.x/2 * targetStairs.stairDirection.x,targetStairs.transform.localScale.y/2 * targetStairs.stairDirection.y);
            Tween tween2 = player.transform.DOMove(player.gameObject.transform.position - targetStairs.stairDirection, transitionDuration/2).SetEase(Ease.Linear);
            player.transform.parent = targetTrigger.transform.parent;
            tween2.onComplete += delegate
            {
                player.input.inputActionAsset.Enable();
                targetTrigger.enabled = true;
                thisTrigger.enabled = true;
            };
        };
    }
}
