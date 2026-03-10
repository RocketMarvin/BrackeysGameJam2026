using UnityEngine;
using System.Collections.Generic;

public class ItemPickUpManager : MonoBehaviour
{
    public List<GameObject> nearbyItems = new List<GameObject>();

    public GameObject closestItem;
    public GameObject currentItem;

    [SerializeField] Transform itemSlot;

    [SerializeField] private bool hasItem = false;
    public bool wrenchGrabbed = false;
    public bool equippedTeleporter = false;
    public bool equippedDiveSuit = false;
    public bool equippedHeatResist = false;
    public bool alienActive = false;
    public bool equippedTranslator = false;
    public bool cleanedDiveSuit = false;
    public bool inAlienDoor = false;
    public GameObject crowbar, uvViewer, heatResister, alien, codeTranslator, zeeSpons, codeUI, diveSuit;

    [SerializeField] private GameObject UVSplatsParent; // parent van de 4 sprites

    [SerializeField] InputReader input;
    [SerializeField] GameObject ghost;

    void Start()
    {
        currentItem = null;
    }

    void Update()
    {
        if (hasItem)
        {
            if (closestItem != null)
            {
                closestItem.transform.GetChild(0).gameObject.SetActive(false);
                closestItem = null;
            }

            if (currentItem != null)
                currentItem.transform.position = itemSlot.position;

            if (currentItem.name == "WrenchInteracable") wrenchGrabbed = true;
            else wrenchGrabbed = false;

                return;
        }

        GameObject newClosest = null;
        float shortestDistance = Mathf.Infinity;

        foreach (GameObject item in nearbyItems)
        {
            float distance = Vector3.Distance(transform.position, item.transform.position);

            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                newClosest = item;
            }
        }

        if (closestItem != newClosest)
        {
            if (closestItem != null && closestItem.transform.childCount > 0)
                closestItem.transform.GetChild(0).gameObject.SetActive(false);

            closestItem = newClosest;

            if (closestItem != null && closestItem.transform.childCount > 0)
                closestItem.transform.GetChild(0).gameObject.SetActive(true);
        }

        if (nearbyItems.Count == 0 && closestItem != null)
        {
            closestItem.transform.GetChild(0).gameObject.SetActive(false);
            closestItem = null;
        }
    }

    void PickupAndDrop()
    {
        // ITEM DROPPEN
        if (hasItem && currentItem != null)
        {
            hasItem = false;

            Rigidbody2D rb = currentItem.GetComponent<Rigidbody2D>();
            rb.gravityScale = 1;
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.linearVelocity = Vector2.zero;

            currentItem = null;
            return;
        }

        if (closestItem == null) return;

        // EQUIP ITEMS
        if (closestItem.CompareTag("DiveSuit"))
        {
            ActivateDiveSuit();
            closestItem.SetActive(false);
            if (cleanedDiveSuit)
            {
                ReturnDiveSuit();
                Destroy(closestItem);
                return;
            }
            return;
        }

        if (closestItem.CompareTag("Teleporter"))
        {
            ActivateTeleporter();
            Destroy(closestItem);
            return;
        }

        if (closestItem.CompareTag("UVViewer"))
        {
            ActivateUVViewer();
            Destroy(closestItem);
            return;
        }

        if (closestItem.CompareTag("HeatResister"))
        {
            ActivateHeatResister();
            Destroy(closestItem);
            alienActive = true;
            alien.SetActive(true);
            return;
        }

        if (closestItem.CompareTag("CodeTranslator"))
        {
            Destroy(closestItem);
            equippedTranslator = true;
            zeeSpons.SetActive(true);
            return;
        }

        if (closestItem.CompareTag("ZeeSpons"))
        {
            Destroy(closestItem);
            diveSuit.SetActive(true);
            cleanedDiveSuit = true;
            return;
        }

        // CROWBAR SPAWNEN MET WRENCH
        if (closestItem.CompareTag("DoorMechanism"))
        {
            if (currentItem != null && currentItem.CompareTag("Wrench"))
            {
                Destroy(closestItem);
                crowbar.SetActive(true);
                Debug.Log("Crowbar spawned!");
            }

            return;
        }

        // ITEM OPPAKKEN
        if (closestItem.CompareTag("NormalItem") || closestItem.CompareTag("Wrench") || closestItem.CompareTag("Crowbar"))
        {
            currentItem = closestItem;

            Rigidbody2D rb = currentItem.GetComponent<Rigidbody2D>();
            rb.gravityScale = 0;
            rb.bodyType = RigidbodyType2D.Kinematic;

            hasItem = true;
        }
    }

    void ActivateDiveSuit()
    {
        Debug.Log("Dive suit equipped");
        ghost.SetActive(true);
        equippedDiveSuit = true;
    }

    void ReturnDiveSuit()
    {
        Debug.Log("Dive suit Cleaned");
        ghost.GetComponent<GhostBehaviour>().ghostHelped = true;
        alien.SetActive(false);
        alienActive = false;
    }

    void ActivateTeleporter()
    {
        Debug.Log("Teleporter equipped");
        equippedTeleporter = true;
    }

    void ActivateUVViewer()
    {
        Debug.Log("UV Viewer equipped");
        UVSplatsParent.SetActive(true);
        codeTranslator.SetActive(true);
    }

    void ActivateHeatResister()
    {
        Debug.Log("Heat Resister equipped");
        equippedHeatResist = true;
    } 

    private void OnEnable()
    {
        input.InteractEvent += PickupAndDrop;
    }

    private void OnDisable()
    {
        input.InteractEvent -= PickupAndDrop;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("NormalItem") ||
            collision.CompareTag("DiveSuit") ||
            collision.CompareTag("Teleporter") ||
            collision.CompareTag("UVViewer") ||
            collision.CompareTag("Wrench") ||
            collision.CompareTag("HeatResister") ||
            collision.CompareTag("Crowbar") ||
            collision.CompareTag("CodeTranslator") ||
            collision.CompareTag("ZeeSpons"))
        {
            nearbyItems.Add(collision.gameObject);
        }

        // CAPSULE REPAREREN MET WRENCH
        if (collision.CompareTag("AlienCapsule"))
        {
            if (currentItem != null && currentItem.CompareTag("Wrench"))
            {
                AlienCapsule cap = collision.GetComponent<AlienCapsule>();
                if (cap != null)
                {
                    Debug.Log("Capsule repaired");
                    cap.RepairCapsule();
                    uvViewer.SetActive(true);
                }
            }
        }

        if (collision.CompareTag("DoorMechanism"))
        {
            if (currentItem != null && currentItem.CompareTag("Wrench"))
            {
                collision.gameObject.SetActive(false);
                crowbar.SetActive(true);
            }
        }

        if (collision.CompareTag("WoodenBox"))
        {
            if (currentItem != null && currentItem.CompareTag("Crowbar"))
            {
                collision.gameObject.SetActive(false);
                heatResister.SetActive(true);
            }
        }

        if (collision.CompareTag("AlienDoor") && equippedTranslator)
        {
            codeUI.SetActive(true);
            inAlienDoor = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("NormalItem") ||
            collision.CompareTag("DiveSuit") ||
            collision.CompareTag("Teleporter") ||
            collision.CompareTag("UVViewer") ||
            collision.CompareTag("Wrench") ||
            collision.CompareTag("HeatResister") ||
            collision.CompareTag("Crowbar") ||
            collision.CompareTag("CodeTranslator") ||
            collision.CompareTag("ZeeSpons"))
        {
            nearbyItems.Remove(collision.gameObject);
        }

        if (collision.CompareTag("AlienDoor"))
        {
            codeUI.SetActive(false);
            inAlienDoor = false;
        }
    }
}