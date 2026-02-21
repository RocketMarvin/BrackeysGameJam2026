using UnityEngine;
using System.Collections.Generic;

public class ItemPickUpManager : MonoBehaviour
{
    public List <GameObject> items = new List <GameObject>();
    public List <GameObject> nearbyItems = new List <GameObject>();

    public GameObject closestItem;
    public GameObject currentItem;
    [SerializeField] Transform itemSlot;

    [SerializeField]private bool hasItem = false;

    [SerializeField] InputReader input;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentItem = null;

        items.AddRange(GameObject.FindGameObjectsWithTag("Interactable"));
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
        if (!hasItem && closestItem != null)
        {
            currentItem = closestItem;

            currentItem.transform.GetChild(0).gameObject.SetActive(false);
            closestItem = null;

            currentItem.GetComponent<Rigidbody2D>().gravityScale = 0;
            hasItem = true;
        }
        else if (hasItem)
        {
            hasItem = false;
            currentItem.GetComponent<Rigidbody2D>().gravityScale = 1;
            currentItem = null;
        }
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
        if (collision.CompareTag("Interactable"))
        {
            nearbyItems.Add(collision.gameObject);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Interactable"))
        {
            nearbyItems.Remove(collision.gameObject);
        }
    }
}
