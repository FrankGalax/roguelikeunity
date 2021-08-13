using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSprite : MonoBehaviour
{
    public List<Sprite> Sprites;

    private void Awake()
    {
        int random = UnityEngine.Random.Range(0, Sprites.Count);
        GetComponent<SpriteRenderer>().sprite = Sprites[random];
    }
}
