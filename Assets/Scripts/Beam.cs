using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Beam : MonoBehaviour
{
    public Sprite Base;
    public Sprite DiagonalBase;
    public Sprite Looping;
    public Sprite DiagonalLooping;
    public Sprite End;
    public Sprite DiagonalEnd;
    public GameObject DiagonalJunction;
    public (int, int) Distance { get; set; }
    public int Radius { get; set; }

    public void SetupSprite()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        (int, int) direction = (0, 0);
        direction.Item1 = Distance.Item1 > 0 ? 1 : (Distance.Item1 < 0 ? -1 : 0);
        direction.Item2 = Distance.Item2 > 0 ? 1 : (Distance.Item2 < 0 ? -1 : 0);

        if (direction == (0, 1))
        {
            transform.rotation = Quaternion.identity;
            int distance = Distance.Item2;
            SetSprite(spriteRenderer, direction, distance, false);
        }
        else if (direction == (-1, 1))
        {
            transform.rotation = Quaternion.identity;
            int distance = Distance.Item2;
            SetSprite(spriteRenderer, direction, distance, true);

            if (distance > 1)
            {
                SpawnJunctions();
            }
        }
        else if (direction == (-1, 0))
        {
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, 90.0f);
            int distance = -Distance.Item1;
            SetSprite(spriteRenderer, direction, distance, false);
        }
        else if (direction == (-1, -1))
        {
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, 90.0f);
            int distance = -Distance.Item1;
            SetSprite(spriteRenderer, direction, distance, true);

            if (distance > 1)
            {
                SpawnJunctions();
            }
        }
        else if (direction == (0, -1))
        {
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, 180.0f);
            int distance = -Distance.Item2;
            SetSprite(spriteRenderer, direction, distance, false);
        }
        else if (direction == (1, -1))
        {
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, 180.0f);
            int distance = -Distance.Item2;
            SetSprite(spriteRenderer, direction, distance, true);

            if (distance > 1)
            {
                SpawnJunctions();
            }
        }
        else if (direction == (1, 0))
        {
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, 270.0f);
            int distance = Distance.Item1;
            SetSprite(spriteRenderer, direction, distance, false);
        }
        else if (direction == (1, 1))
        {
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, 270.0f);
            int distance = Distance.Item1;
            SetSprite(spriteRenderer, direction, distance, true);

            if (distance > 1)
            {
                SpawnJunctions();
            }
        }
    }

    private void SetSprite(SpriteRenderer spriteRenderer, (int, int) direction, int distanceLength, bool diagonal)
    {
        if (direction == Distance)
        {
            spriteRenderer.sprite = diagonal ? DiagonalBase : Base;
        }
        else if (distanceLength == Radius)
        {
            spriteRenderer.sprite = diagonal ? DiagonalEnd : End;
        }
        else
        {
            spriteRenderer.sprite = diagonal ? DiagonalLooping : Looping;
        }
    }

    private void SpawnJunctions()
    {
        GameObject junction1 = Instantiate(DiagonalJunction, Vector3.zero, Quaternion.identity, transform);
        junction1.transform.localPosition = new Vector3(1.0f, 0.0f, 0.0f);
        junction1.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);

        GameObject junction2 = Instantiate(DiagonalJunction, Vector3.zero, Quaternion.identity, transform);
        junction2.transform.localPosition = new Vector3(0.0f, -1.0f, 0.0f);
        junction2.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 180.0f);
    }
}
