using UnityEngine;
using Pathfinding;
using System.Collections;
using System.Linq;
using DG.Tweening;
public class MonsterAI : MonoBehaviour, ImLoadedByPlayer
{
    Animator animator;
    public AudioClip[] startChaseAudios;
    private AudioSource audioSource;
    Player player;
    public SpriteRenderer sprite;
    Rigidbody2D rb;
    Path path;
    Seeker seeker;
    RaycastHit2D ray;
    Vector3 direction;
    Vector3 lastPlayerSeenPosition;
    public Coroutine currentBehaviour;
    [SerializeField]
    float minDistanceToNextPath, speed, minTimePerRoom, maxTimePerRoom, sightDistanceWhileNotPursuing, currentSightDistance;
    float distanceToTarget;
    [SerializeField]
    int minSpotsPerRoom, maxSpotsPerRoom;
    bool seenPlayer;
    bool searchingForPlayer = false;
    bool onPursuit = false;
    private void Update()
    {
        sprite.sortingLayerID = SortingLayer.layers[Mathf.Clamp((int)(player.transform.position.y - transform.position.y),0,1)*3 + 3].id;
        ray = Physics2D.Raycast(transform.position + (player.transform.position - transform.position).normalized * 0.46f, (player.transform.position - transform.position).normalized, currentSightDistance, 1 << LayerMask.NameToLayer("Player") | 1<< LayerMask.NameToLayer("Obstacle"));
        Debug.DrawLine(transform.position, ray.point);
        animator.SetFloat("horizontal", direction.x);
        animator.SetFloat("vertical", direction.y);
        sprite.flipX = direction.x > 0;
        if (ray.collider != null)
            seenPlayer = ray.collider.gameObject == player.gameObject;
        else
            seenPlayer = false;
        if (seenPlayer)
        {
            lastPlayerSeenPosition = player.transform.position;
            if (!onPursuit)
            {
                if (currentBehaviour != null)
                    StopCoroutine(currentBehaviour);
                currentBehaviour = StartCoroutine(PursuitMode());
            }
            else if (searchingForPlayer)
            {
                StopCurrentBehaviour();
                currentBehaviour = StartCoroutine(PursuitMode());
            }
            animator.SetFloat("horizontal", direction.x);
            animator.SetFloat("vertical", direction.y);
            sprite.flipX = direction.x < 0;
        }
    }
    void ImLoadedByPlayer.Load(Player player)
    {
        this.player = player;
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
        seeker = GetComponent<Seeker>();
        Stairs.onPlayerUse += ChangeFloor;
        animator = GetComponentInChildren<Animator>();
    }
    private void FixedUpdate()
    {
        rb.MovePosition(transform.position + (direction.normalized * speed * Time.fixedDeltaTime));
    }
    public void StartMonster()
    {
        currentBehaviour = StartCoroutine(WanderMode());
    }
    private void ChangeFloor(Stairs stairs)
    {
        StopCurrentBehaviour();
        currentBehaviour = StartCoroutine(ChangingFloor());
        IEnumerator ChangingFloor()
        {
            if (seenPlayer)
            {
                yield return GuideMonsterToLocation(stairs.transform.position);
                direction = Vector3.zero;
                sprite.DOColor(Color.black, Stairs.transitionDuration / 2f);
                yield return new WaitForSeconds(Stairs.transitionDuration / 2f);
                transform.position = stairs.targetStairs.transform.position;
                sprite.DOColor(Color.white, Stairs.transitionDuration / 2f);
                yield return new WaitForSeconds(Stairs.transitionDuration / 2f);
                if (!onPursuit)
                { 
                    currentBehaviour = StartCoroutine(WanderMode());
                }
                else
                {
                    currentBehaviour = StartCoroutine(PursuitMode());
                }
            }
            else
            {
                direction = Vector2.zero;
                yield return new WaitForSeconds(1.5f);
                currentBehaviour = StartCoroutine(WanderMode());
            }
        }
    }
    private void StopCurrentBehaviour()
    {
        if (currentBehaviour != null)
        {
            StopCoroutine(currentBehaviour);
            currentBehaviour = null;
        }
    }
    private IEnumerator WanderMode()
    {
        currentSightDistance = sightDistanceWhileNotPursuing;
        Room[] rooms = Singleton<GameManager>.Instance.rooms.Where(room => room.zone == Singleton<GameManager>.Instance.currentRoom.zone && !room.safeZone).ToArray();
        Room chosenRoom = rooms[Random.Range(0, rooms.Length - 1)];
        transform.position = chosenRoom.roomSpawnPoint.position;
        while (true)
        {
            rooms = Singleton<GameManager>.Instance.rooms.Where(room => room.zone == Singleton<GameManager>.Instance.currentRoom.zone && !room.safeZone).ToArray();
            chosenRoom = rooms[Random.Range(0, rooms.Length - 1)];
            yield return StartCoroutine(WanderAroundRoom(chosenRoom, Random.Range(minSpotsPerRoom, maxSpotsPerRoom)));
        }
    }
    private IEnumerator PursuitMode()
    {
        audioSource.clip = startChaseAudios[Random.Range(0, startChaseAudios.Length - 1)];
        audioSource.Play();
        onPursuit = true;
        searchingForPlayer = false;
        currentSightDistance = Mathf.Infinity;
        while (true)
        {
            yield return StartCoroutine(GuideMonsterToLocation(lastPlayerSeenPosition, 0.5f));
            if (!seenPlayer)
            {
                searchingForPlayer = true;
                yield return StartCoroutine(GuideMonsterToLocation(lastPlayerSeenPosition));
                if (!seenPlayer)
                {
                    StopCurrentBehaviour();
                    currentBehaviour = StartCoroutine(WanderMode());
                    searchingForPlayer = false;
                    onPursuit = false;
                }
            }
        }
    }
    private IEnumerator GuideMonsterToLocation(Vector2 location, float time = 0)
    {
        path = seeker.StartPath(transform.position, location);
        while (!path.IsDone())
        {
            yield return null;
        }
        if (path.error)
        {
            yield break;
        }
        int currentWaypoint = 0;
        if (Mathf.Approximately(0, time))
        {
            while (currentWaypoint < path.vectorPath.Count)
            {
                direction = (path.vectorPath[currentWaypoint] - transform.position).normalized;
                distanceToTarget = Vector2.Distance(transform.position, path.vectorPath[currentWaypoint]);
                if (distanceToTarget < minDistanceToNextPath)
                {
                    currentWaypoint++;
                }
                yield return null;
            } 
        }
        else
        {
            for (float i = 0; i < time; i += Time.deltaTime)
            {
                direction = (path.vectorPath[currentWaypoint] - transform.position).normalized;
                distanceToTarget = Vector2.Distance(transform.position, path.vectorPath[currentWaypoint]);
                if (distanceToTarget < minDistanceToNextPath)
                {
                    currentWaypoint++;
                }
                if (currentWaypoint > path.vectorPath.Count - 1)
                {
                    break;
                }
                yield return null;
            }
        }
    }
    private IEnumerator WanderAroundRoom(Room room, int locationAmount)
    {
        Debug.Log($"Monster is looking around the {room.name} in {locationAmount} different spots");
        Vector2 location = GetRoomRandomLocation(room);
        yield return StartCoroutine(GuideMonsterToLocation(location));
        for (int i = 1; i < locationAmount; i++)
        {
            location = GetRoomRandomLocation(room);
            yield return StartCoroutine(GuideMonsterToLocation(location, Random.Range(minTimePerRoom , maxTimePerRoom)));
        }
    }
    private static Vector2 GetRoomRandomLocation(Room room)
    {
        Vector2 topLeftPosition = (Vector2)room.transform.position - room.area.size / 2;
        Vector2 bottomRightPosition = (Vector2)room.transform.position + room.area.size / 2;
        Vector2 location = new Vector2(Random.Range(topLeftPosition.x, bottomRightPosition.x), Random.Range(topLeftPosition.y, bottomRightPosition.y));
        return location;
    }
}
