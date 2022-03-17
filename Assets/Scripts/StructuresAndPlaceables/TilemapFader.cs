using UnityEngine;
using UnityEngine.Tilemaps;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.Experimental.Rendering.Universal;

public class TilemapFader : MonoBehaviour
{
    PlayerAnimationController player;
    [HideInInspector]
    public Tilemap tilemap;
    [HideInInspector]
    public TilemapRenderer tilemapRenderer;
    public List<Tween> tweens = new List<Tween>();
    [HideInInspector]
    public bool isPlayerOn;
    public SpriteRenderer[] fadingSpriteRenderers;
    public ShadowCaster2D[] shadowCasters;
    Bounds tilemapBounds;
    private void Start()
    {
        player = Singleton<Player>.Instance.animation;
        tilemap = GetComponent<Tilemap>();
        tilemapRenderer = GetComponent<TilemapRenderer>();
        tilemapBounds = tilemap.localBounds;
    }
    private void Update()
    {
        void CancelTweens()
        {
            for (int i = 0; i < tweens.Count; i++)
            {
                if (tweens[i] != null)
                {
                    tweens[i].Kill();
                    tweens.RemoveAt(i);
                    i--;
                }
            }
        }
        Vector3[] playerSpritePoints = new Vector3[]
        {
                    new Vector3(player.transform.parent.parent.localPosition.x - (player.spriteRenderer.size.x / 2f), player.transform.parent.parent.localPosition.y - (player.spriteRenderer.size.y / 2f), 0),
                    new Vector3(player.transform.parent.parent.localPosition.x + (player.spriteRenderer.size.x / 2f), player.transform.parent.parent.localPosition.y - (player.spriteRenderer.size.y / 2f), 0),
        };
        if (!isPlayerOn)
        {
            if (tilemapBounds.Contains(playerSpritePoints[0]) || tilemapBounds.Contains(playerSpritePoints[1]))
            {
                isPlayerOn = true;
                CancelTweens();
                tweens.Add(DOTween.To(() => tilemap.color, x => tilemap.color = x, new Color(1, 1, 1, 0.2f), 0.5f));
                foreach (SpriteRenderer spriteRenderer in fadingSpriteRenderers)
                {
                    tweens.Add(DOTween.To(() => spriteRenderer.color, x => spriteRenderer.color = x, new Color(1, 1, 1, 0.2f), 0.5f));
                }
                foreach (ShadowCaster2D shadowCaster in shadowCasters)
                {
                    shadowCaster.enabled = false;
                }
            }
        }
        else
        {
            if (!tilemapBounds.Contains(playerSpritePoints[0]) && !tilemapBounds.Contains(playerSpritePoints[1]))
            {
                isPlayerOn = false;
                CancelTweens();
                tweens.Add(DOTween.To(() => tilemap.color, x => tilemap.color = x, Color.white, 0.5f));
                foreach (SpriteRenderer spriteRenderer in fadingSpriteRenderers)
                {
                    tweens.Add(DOTween.To(() => spriteRenderer.color, x => spriteRenderer.color = x, Color.white, 0.5f));
                }
                foreach (ShadowCaster2D shadowCaster in shadowCasters)
                {
                    shadowCaster.enabled = true;
                }
            }
        }
    }
}