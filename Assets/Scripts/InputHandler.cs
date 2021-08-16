using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public GameObject Player;
    public float MoveSpeed = 4.0f;
    private ActionQueue m_ActionQueue;

    private void Awake()
    {
        m_ActionQueue = GetComponent<ActionQueue>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            m_ActionQueue.AddAction(new BumpAction { GameObject = Player, Speed = MoveSpeed, Direction = (0, 1) });
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            m_ActionQueue.AddAction(new BumpAction { GameObject = Player, Speed = MoveSpeed, Direction = (-1, 0) });
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            m_ActionQueue.AddAction(new BumpAction { GameObject = Player, Speed = MoveSpeed, Direction = (0, -1) });
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            m_ActionQueue.AddAction(new BumpAction { GameObject = Player, Speed = MoveSpeed, Direction = (1, 0) });
        }
    }
}
