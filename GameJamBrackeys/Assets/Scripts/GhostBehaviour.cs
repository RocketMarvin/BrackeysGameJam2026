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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        box = null;
        currentState = GhostState.Moving;
        ChooseNewTarget();
        print($"First Target: {currentTarget.name}");
        behaviourCoroutine = StartCoroutine(BehaviourSelection());
    }

    void FixedUpdate()
    {
        switch (currentState)
        {
            case GhostState.Idle:
                rb.linearVelocity = Vector2.zero;
                break;
            case GhostState.Moving:
                if (movementCoroutine == null) movementCoroutine = StartCoroutine(GhostMoving());
                break;
            case GhostState.Annoying:
                if (annoyCoroutine == null) annoyCoroutine = StartCoroutine(AnnoyPlayer());
                break;
            case GhostState.Haunting:
                if (hauntCoroutine == null) hauntCoroutine = StartCoroutine(HauntRoom());
                break;
        }
    }

    IEnumerator BehaviourSelection()
    {
        while (true)
        {
            if (currentState != GhostState.Annoying)
                yield return new WaitForSeconds(10f);

            previousState = currentState;

            GhostState newState;

            do
            {
                newState = (GhostState)Random.Range(0, (int)GhostState.Haunting + 1);
            }
            while (newState == previousState);

            if (currentRoom == Rooms.AlienRoom && ghostHelped)
            {
                newState = GhostState.Haunting;
            }

            ChangeState(newState);
            print($"New Ghost State: {currentState}");

            if (currentState != GhostState.Idle)
            {
                behaviourCoroutine = null;
                yield break;
            }

            yield return null;
        }
    }
    void ChangeState(GhostState newState)
    {
        // Stop alle actieve state coroutines
        if (movementCoroutine != null)
        {
            StopCoroutine(movementCoroutine);
            movementCoroutine = null;
        }

        if (hauntCoroutine != null)
        {
            StopCoroutine(hauntCoroutine);
            hauntCoroutine = null;
        }

        if (annoyCoroutine != null)
        {
            StopCoroutine(annoyCoroutine);
            annoyCoroutine = null;
        }

        rb.linearVelocity = Vector2.zero;

        currentState = newState;
    }

    //+ Hieronder staan alle methodes die te maken hebben met het kiezen van een nieuw doel en er naartoe bewegen. Deze zijn allemaal met elkaar verbonden, omdat ze allemaal te maken hebben met het bewegen van de geest naar een nieuw doel.
    void ChooseNewTarget()
    {
        if (currentState == GhostState.Moving)
        {
            currentTarget = ghostPrefabs[4].transform;
            currentRoom = Rooms.HauntedRoom;
        }
        else
        {
            currentRoom = (Rooms)Random.Range(0, (int)Rooms.HauntedRoom + 1);

            switch (currentRoom)
            {
                case Rooms.AlienRoom:
                    currentTarget = ghostPrefabs[0].transform;
                    break;
                case Rooms.HauntedRoom:
                    currentTarget = ghostPrefabs[1].transform;
                    break;
                case Rooms.OceanRoom:
                    currentTarget = ghostPrefabs[2].transform;
                    break;
                case Rooms.DesertRoom:
                    currentTarget = ghostPrefabs[3].transform;
                    break;
            }
        }

        if (currentState != GhostState.Moving)
        {
            box = currentTarget.GetComponent<BoxCollider2D>();
            bounds = box.bounds;

            target = GetRandomPointInBiome();
        }
        else
        {
            target = currentTarget.position; // direct naartoe bewegen
        }
    }

    IEnumerator GhostMoving()
    {
        print($"New target: {currentTarget.name}");

        movementCoroutine = StartCoroutine(Movement());

        yield return new WaitForSeconds(30f);

        StopCoroutine(movementCoroutine);
        movementCoroutine = null;
        behaviourCoroutine = StartCoroutine(BehaviourSelection());
    }

    IEnumerator NextDestination()
    {
        currentTarget = null;
        yield return new WaitForSeconds(3f);
        ChooseNewTarget();
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

    //- Einde van de methodes die te maken hebben met het kiezen van een nieuw doel en er naartoe bewegen.

    //+ Hieronder staan alle methodes die te maken hebben met het oppakken en laten vallen van een item. Deze zijn allemaal met elkaar verbonden, omdat ze allemaal te maken hebben met het oppakken en laten vallen van een item.

    IEnumerator AnnoyPlayer()
    {
        if (currentTarget == null) yield break;

        if (!annoyingPlayer)
        {
            annoyingPlayer = true;
            carryingItem = false;

            currentInteractableTarget = interactables[Random.Range(0, interactables.Count)];
            currentTarget = currentInteractableTarget.transform;

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

                    if (behaviourCoroutine == null) behaviourCoroutine = StartCoroutine(BehaviourSelection());

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

        Coroutine localMovement = StartCoroutine(Movement());

        yield return new WaitForSeconds(30f);

        StopCoroutine(localMovement);

        behaviourCoroutine = StartCoroutine(BehaviourSelection());
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
                yield return new WaitForSeconds(1f); // kleine pauze tussen punten
                target = GetRandomPointInBiome();
            }

            yield return new WaitForFixedUpdate();
        }
    }

    //- Einde van de methodes die te maken hebben met het spoken in een kamer.
}
