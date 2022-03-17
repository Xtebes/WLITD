using System.Collections;
using TMPro;
using UnityEngine;
using System;
public abstract class Interactable : MonoBehaviour
{
    [SerializeField]
    private GameObject ui;
    [Range(-1,1)]
    public float dotMin;
    [SerializeField]
    float dot;
    public Action onInteract;
    public TextMeshProUGUI title, info;
    private void Start()
    {
        if (ui != null)
            ui.SetActive(false);
    }
    public virtual void OnPlayerEnter(Player player) 
    { 
        StartCoroutine(UIshow(player)); 
    }
    public virtual void OnPlayerExit(Player player) 
    {
        StopAllCoroutines();
        if (ui != null)
        {
            ui.SetActive(false);
        }
        if (onInteract != null)
        {
            player.input.worldInteractionAction = null;
            player.input.worldInteraction.Disable();
        }
    }
    private IEnumerator UIshow(Player player)
    {
        while (true)
        { 
            dot = Vector3.Dot((player.animation.flashLight.transform.up).normalized, (transform.position - player.transform.position).normalized);
            if (dot > dotMin)
            {
                if (ui != null)
                {
                    ui.SetActive(true);
                }
                if (onInteract != null)
                {
                    player.input.worldInteractionAction = onInteract.Invoke;
                    player.input.worldInteraction.Enable();
                }
            }
            else
            {
                if (ui != null)
                {
                    ui.SetActive(false);
                }
                if (onInteract != null)
                {
                    player.input.worldInteractionAction = null;
                    player.input.worldInteraction.Disable();
                }
            }
            yield return null;
        }
    }
    public virtual void OnMonsterEnter(MonsterAI monster) { }
    public virtual void OnMonsterExit(MonsterAI monster) { }
   
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            OnPlayerEnter(collision.GetComponentInParent<Player>());
        }
        else if (collision.gameObject.CompareTag("Monster"))
        {
            OnMonsterEnter(collision.GetComponentInChildren<MonsterAI>());
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            OnPlayerExit(collision.GetComponentInParent<Player>());
        }
        else if (collision.gameObject.CompareTag("Monster"))
        {
            OnMonsterExit(collision.GetComponentInChildren<MonsterAI>());
        }
    }
}