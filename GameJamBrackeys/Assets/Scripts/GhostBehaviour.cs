using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class GhostBehaviour : MonoBehaviour
{
    public List<GameObject> ghostPrefabs = new List<GameObject>();

    public float maxSpeed = 25f;
    public float slowDownDistance = 3f;

    private GhostState currentState = GhostState.Moving;
    private GhostState previousState;

    private Rigidbody2D rb;
    private Transform currentTarget;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ChooseNewTarget();
        StartCoroutine(BehaviourSelection());
    }

    void FixedUpdate()
    {
        switch (currentState)
        {
            case GhostState.Idle:
                rb.linearVelocity = Vector2.zero;
                break;
            case GhostState.Moving:
                GhostMoving();
                break;
            case GhostState.Annoying:
                // Implement annoying behavior here
                break;
            case GhostState.Haunting:
                // Implement haunting behavior here
                break;
        }
    }
    enum GhostState
    {
        Idle,
        Moving,
        Annoying, 
        Haunting,
        num_states
    }

    IEnumerator BehaviourSelection()
    {
        yield return new WaitForSeconds(10f);
        previousState = currentState;
    again:
        currentState = (GhostState)Random.Range(0, (int)GhostState.num_states);
        print($"New Ghost State: {currentState}");
        if (currentState == previousState)
        {
            goto again;
        }
        StartCoroutine(BehaviourSelection());
    }

    void GhostMoving()
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

        if (distance < 0.3f)
        {
            rb.linearVelocity = Vector2.zero;
            StartCoroutine(NextDestination());
        }
    }

    void ChooseNewTarget()
    {
        int randomIndex = Random.Range(0, ghostPrefabs.Count);
        currentTarget = ghostPrefabs[randomIndex].transform;
    }

    IEnumerator NextDestination()
    {
        currentTarget = null;
        yield return new WaitForSeconds(5f);
        ChooseNewTarget();
    }
}
