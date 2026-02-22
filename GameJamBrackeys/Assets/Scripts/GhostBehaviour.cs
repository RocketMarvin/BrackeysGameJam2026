using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Methods tussen //+ en //- horen bij elkaar, deze zijn voor de overzichtelijkheid in de code


public enum Rooms
{
    AlienRoom,
    DesertRoom,
    OceanRoom,
    HauntedRoom
}
public enum GhostState
{
    Idle,
    Moving,
    Annoying,
    Haunting
}
public class GhostBehaviour : MonoBehaviour
{
    public List<GameObject> ghostPrefabs = new List<GameObject>();
    public List<GameObject> interactables = new List<GameObject>();

    public float maxSpeed = 25f;
    public float slowDownDistance = 3f;

    private float angle;
    private float radius = 5f;
    private float rotationSpeed = 5f;

    private bool ghostHelped = false;
    [SerializeField] private bool annoyingPlayer = false;
    [SerializeField] private bool carryingItem;

    public GhostState currentState;
    private GhostState previousState;

    private Coroutine hauntCoroutine;
    private Coroutine annoyCoroutine;
    private Coroutine behaviourCoroutine;
    private Coroutine orbitItem;
    private Coroutine movementCoroutine;

    private Rigidbody2D rb;
    public Transform currentTarget;
    private Transform previousTarget;
    private GameObject currentInteractableTarget;

    public Rooms currentRoom;

    public  BoxCollider2D box;
    private Bounds bounds;

    public  Vector2 target;

    //+ Hieronder staan alle methodes die te maken hebben met het starten van de geest en het veranderen van zijn staat. Deze zijn allemaal met elkaar verbonden, omdat ze allemaal te maken hebben met het starten van de geest en het veranderen van zijn staat.

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        ChangeState(GhostState.Moving);

        print($"First Target: {currentTarget.name}");
    }

    void FixedUpdate()
    {
        if (currentState == GhostState.Idle)
            rb.linearVelocity = Vector2.zero;
    }

    GhostState GetRandomState()
    {
        GhostState newState;

        do
        {
            newState = (GhostState)Random.Range(0, 4);
        }
        while (newState == currentState);

        return newState;
    }

    void ChangeState(GhostState newState)
    {
        StopAllCoroutines();

        rb.linearVelocity = Vector2.zero;

        currentState = newState;

        switch (currentState)
        {
            case GhostState.Idle:
                StartCoroutine(IdleState());
                break;

            case GhostState.Moving:
                StartCoroutine(GhostMoving());
                break;

            case GhostState.Annoying:
                StartCoroutine(AnnoyPlayer());
                break;

            case GhostState.Haunting:
                StartCoroutine(HauntRoom());
                break;
        }
    }
    
    //- Einde van de methodes die te maken hebben met het starten van de geest en het veranderen van zijn staat.

    //+ Hieronder staan alle methodes die te maken hebben met het kiezen van een nieuw doel en er naartoe bewegen.
    
    void ChooseNewTarget()
    {
        // Moving -> altijd teleportMenu
        if (currentState == GhostState.Moving)
        {
            currentTarget = ghostPrefabs[4].transform;
            box = currentTarget.GetComponent<BoxCollider2D>();
            bounds = box.bounds;
            target = box.bounds.center;
            return;
        }

        currentRoom = (Rooms)Random.Range(0, (int)Rooms.HauntedRoom + 1);

        switch (currentRoom)
        {
            case Rooms.AlienRoom: currentTarget = ghostPrefabs[0].transform; break;
            case Rooms.HauntedRoom: currentTarget = ghostPrefabs[1].transform; break;
            case Rooms.OceanRoom: currentTarget = ghostPrefabs[2].transform; break;
            case Rooms.DesertRoom: currentTarget = ghostPrefabs[3].transform; break;
        }

        box = currentTarget.GetComponent<BoxCollider2D>();
        bounds = box.bounds;
        target = GetRandomPointInBiome();
    }

    IEnumerator IdleState()
    {
        yield return new WaitForSeconds(5f);
        ChangeState(GhostState.Moving);
    }

    IEnumerator GhostMoving()
    {
        ChooseNewTarget();

        movementCoroutine = StartCoroutine(Movement());

        yield return new WaitForSeconds(10f);

        StopCoroutine(movementCoroutine);
        movementCoroutine = null;

        ChangeState(GetRandomState());
    }

    void MoveTowardsTarget()
    {
        if (currentTarget == null) return;

        Vector2 direction = currentTarget.position - transform.position;
        float distance = direction.magnitude;

        direction.Normalize();

        float speed = maxSpeed;

        if (distance < slowDownDistance)
        {
            speed = maxSpeed * (distance / slowDownDistance);
        }

        rb.linearVelocity = direction * speed;
    }

    Vector2 GetRandomPointInBiome()
    {
        float randomX = Random.Range(bounds.min.x, bounds.max.x);
        float randomY = Random.Range(bounds.min.y, bounds.max.y);

        return new Vector2(randomX, randomY);
    }

    IEnumerator Movement()
    {
        if (currentTarget == null)
            ChooseNewTarget();

        target = GetRandomPointInBiome();

        while (true)
        {
            rb.MovePosition(Vector2.MoveTowards(rb.position, target, maxSpeed * Time.fixedDeltaTime));

            if (Vector2.Distance(transform.position, target) < 0.3f)
            {
                yield return new WaitForSeconds(1f);
                target = GetRandomPointInBiome();
            }

            yield return new WaitForFixedUpdate();
        }
    }

    //- Einde van de methodes die te maken hebben met het kiezen van een nieuw doel en er naartoe bewegen.

    //+ Hieronder staan alle methodes die te maken hebben met het oppakken en laten vallen van een item.

    IEnumerator AnnoyPlayer()
    {
        box = null;

        if (currentTarget == null) yield break;

        if (!annoyingPlayer)
        {
            annoyingPlayer = true;
            carryingItem = false;

            if (interactables.Count > 0)
            {
                currentInteractableTarget = interactables[Random.Range(0, interactables.Count)];
                currentTarget = currentInteractableTarget.transform;
            }
            else
            {
                annoyingPlayer = false;
                ChangeState(GetRandomState());
                yield break;
            }

            print($"Targeting item: {currentTarget.name}");
        }

        while (annoyingPlayer)
        {
            MoveTowardsTarget();

            if (Vector2.Distance(transform.position, currentTarget.position) < 0.3f)
            {
                rb.linearVelocity = Vector2.zero;

                if (!carryingItem)
                {
                    PickUpItem(currentInteractableTarget);

                    carryingItem = true;
                    previousTarget = currentTarget;

                    ChooseNewTarget();
                }
                else
                {
                    yield return new WaitForSeconds(2f);

                    DropItem(currentInteractableTarget);

                    ChangeState(GhostState.Moving);

                    annoyCoroutine = null;
                    carryingItem = false;
                    yield break;
                }
            }

            yield return null;
        }
    }

    void PickUpItem(GameObject targetedInteractable)
    {
        targetedInteractable.GetComponent<Rigidbody2D>().simulated = false;
        targetedInteractable.transform.SetParent(transform);
        targetedInteractable.transform.position = transform.position;

        if (orbitItem == null)
            orbitItem = StartCoroutine(OrbitItem(targetedInteractable));
    }

    void DropItem(GameObject targetedInteractable)
    {
        StopCoroutine(orbitItem);
        orbitItem = null;

        targetedInteractable.transform.SetParent(null);
        targetedInteractable.GetComponent<Rigidbody2D>().simulated = true;
        print($"Dropping item: {targetedInteractable.name}");

        annoyingPlayer = false;
        previousTarget = null;
    }

    IEnumerator OrbitItem(GameObject targetedItem)
    {
        while (true)
        {
            print(carryingItem);

            angle += rotationSpeed * Time.deltaTime;

            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;

            targetedItem.transform.position = transform.position + new Vector3(x, y, 0);
            print($"Orbiting item: {targetedItem.name}");

            yield return null;
        }
    }

    //- Einde van de methodes die te maken hebben met het oppakken en laten vallen van een item.

    //+ Hieronder staan alle methodes die te maken hebben met het spoken in een kamer.

    IEnumerator HauntRoom()
    {
        ChooseNewTarget();

        movementCoroutine = StartCoroutine(Movement());

        yield return new WaitForSeconds(30f);

        if (movementCoroutine != null)
        {
            StopCoroutine(movementCoroutine);
            movementCoroutine = null;
        }

        ChangeState(GetRandomState());
    }

    //- Einde van de methodes die te maken hebben met het spoken in een kamer.
}
