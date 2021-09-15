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
    public (int, int) Distance { get; set; }
    public int Radius { get; set; }

    public void SetupSprite()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        (int, int) direction = (0, 0);
        direction.Item1 = Distance.Item1 > 0 ? 1 : (Distance.Item1 < 0 ? -1 : 0);
        direction.Item2 = Distance.Item2 > 0 ? 1 : (Distance.Item2 < 0 ? -1 : 0);

        Action<int, bool> setSprite = (distanceLength, diagonal) =>
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
        };

        if (direction == (0, 1))
        {
            transform.rotation = Quaternion.identity;
            setSprite(Distance.Item2, false);
        }
        else if (direction == (-1, 1))
        {
            transform.rotation = Quaternion.identity;
            setSprite(Distance.Item2, true);
        }
        else if (direction == (-1, 0))
        {
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, 90.0f);
            setSprite(-Distance.Item1, false);
        }
        else if (direction == (-1, -1))
        {
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, 90.0f);
            setSprite(-Distance.Item1, true);
        }
        else if (direction == (0, -1))
        {
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, 180.0f);
            setSprite(-Distance.Item2, false);
        }
        else if (direction == (1, -1))
        {
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, 180.0f);
            setSprite(-Distance.Item2, true);
        }
        else if (direction == (1, 0))
        {
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, 270.0f);
            setSprite(Distance.Item1, false);
        }
        else if (direction == (1, 1))
        {
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, 270.0f);
            setSprite(Distance.Item1, true);
        }
    }
}
