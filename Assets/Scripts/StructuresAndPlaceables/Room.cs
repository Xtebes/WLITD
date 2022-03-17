using UnityEngine;
[RequireComponent(typeof(BoxCollider2D))]
public class Room : MonoBehaviour
{
    public enum Zones
    {
        firstFloor = 0,
        secondFloor = 1,
    }
    [SerializeField]
    public Zones zone;
    public BoxCollider2D area;
    public string roomName;
    public bool safeZone;
    public Transform roomSpawnPoint;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponentInParent<PlayerMovementController>() != null)
        {
            Singleton<GameManager>.Instance.currentRoom = this;
        }
    }
}
