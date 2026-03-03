using UnityEngine;
using System.Collections.Generic;

public class ItemPickUpManager : MonoBehaviour
{
    public List <GameObject> items = new List <GameObject>();
    public List <GameObject> nearbyItems = new List <GameObject>();

    public GameObject equippedDiveSuit;
    public GameObject equippedTeleporter;
    public GameObject equippedUVViewer;

    public GameObject closestItem;
    public GameObject currentItem;
    [SerializeField] Transform itemSlot;

    [SerializeField]private bool hasItem = false;

    [SerializeField] InputReader input;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentItem = null;

        items.AddRange(GameObject.FindGameObjectsWithTag("NormalItem"));
    }

    // Update is called once per frame
    void Update()
    {
        // Als we iets vasthouden > forceer alles uit en stop
        if (hasItem)
        {
            if (closestItem != null)
            {
                closestItem.transform.GetChild(0).gameObject.SetActive(false);
                closestItem = null;
            }

            if (currentItem != null)
                currentItem.transform.position = itemSlot.position;

            return; // Voorkomt dat nieuwe canvassen aan gaan
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

        // Als closest verandert > oude uit, nieuwe aan
        if (closestItem != newClosest)
        {
            if (closestItem != null)
                closestItem.transform.GetChild(0).gameObject.SetActive(false);

            closestItem = newClosest;

            if (closestItem != null)
                closestItem.transform.GetChild(0).gameObject.SetActive(true);
        }

        // Als niets meer in range
        if (nearbyItems.Count == 0 && closestItem != null)
        {
            closestItem.transform.GetChild(0).gameObject.SetActive(false);
            closestItem = null;
        }
    }

    void PickupAndDrop()
    {
        // DROPPEN heeft voorrang
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

        // Als we niets vasthouden maar ook geen closest hebben
        if (closestItem == null) return;

        // EQUIP ITEMS
        if (closestItem.CompareTag("DiveSuit"))
        {
            ActivateDiveSuit();
            Destroy(closestItem);
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

        // NORMAAL ITEM OPPakken
        if (closestItem.CompareTag("NormalItem"))
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
        Debug.Log("Dive suit equipped - underwater breathing enabled");
        // Hier kan je bijvoorbeeld:
        // player.canBreatheUnderwater = true;
    }

    void ActivateTeleporter()
    {
        Debug.Log("Teleporter equipped");
        // input.TeleportEvent += Teleport;
    }

    void ActivateUVViewer()
    {
        Debug.Log("UV Viewer equipped - UV spots visible");
        // Bijvoorbeeld:
        // uvLayer.SetActive(true);
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
            collision.CompareTag("UVViewer"))
        {
            nearbyItems.Add(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("NormalItem") ||
            collision.CompareTag("DiveSuit") ||
            collision.CompareTag("Teleporter") ||
            collision.CompareTag("UVViewer"))
        {
            nearbyItems.Remove(collision.gameObject);
        }
    }
}
