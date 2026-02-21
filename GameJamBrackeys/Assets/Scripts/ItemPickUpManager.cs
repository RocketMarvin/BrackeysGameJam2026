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
    void FixedUpdate()
    {
        if (hasItem) currentItem.transform.position = itemSlot.position;
        
        
        if (nearbyItems.Count > 0)
        {
            closestItem = nearbyItems[0];
            foreach (GameObject item in nearbyItems)
            {
                if (Vector3.Distance(transform.position, item.transform.position) < Vector3.Distance(transform.position, closestItem.transform.position))
                {
                    closestItem = item;
                }
            }
        }
    }

    void PickupAndDrop()
    {
        if (!hasItem)
        {
            currentItem = closestItem;
            closestItem.GetComponent<Rigidbody2D>().gravityScale = 0;
            hasItem = true;
        }
        else
        {
            hasItem = false;
            closestItem.GetComponent<Rigidbody2D>().gravityScale = 1;
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
