using System.Collections.Generic;
using UnityEngine;

public class UVManager : MonoBehaviour
{
    [System.Serializable]
    public class SpriteOption
    {
        public Sprite sprite;
        public int number;
    }

    public SpriteOption[] possibleSprites;

    private SpriteRenderer spriteRenderer;

    public int SelectedNumber { get; private set; }

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        Generate();
    }

    public void Generate()
    {
        int randomIndex = Random.Range(0, possibleSprites.Length);

        spriteRenderer.sprite = possibleSprites[randomIndex].sprite;
        SelectedNumber = possibleSprites[randomIndex].number;
    }
}
