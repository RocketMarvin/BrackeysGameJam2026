using UnityEngine;

public class AlienCapsule : MonoBehaviour
{
    [SerializeField] private Sprite damagedSprite, repairedSprite;

    private void Start()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = damagedSprite;
    }

    public void RepairCapsule()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = repairedSprite;
    }

}
