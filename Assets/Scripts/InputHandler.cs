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
        if (Player == null || m_ActionQueue.IsBusy())
        {
            return;
        }

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            m_ActionQueue.AddAction(new BumpAction { GameObject = Player, Speed = MoveSpeed, Direction = (0, 1) });
        }
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            m_ActionQueue.AddAction(new BumpAction { GameObject = Player, Speed = MoveSpeed, Direction = (-1, 0) });
        }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            m_ActionQueue.AddAction(new BumpAction { GameObject = Player, Speed = MoveSpeed, Direction = (0, -1) });
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            m_ActionQueue.AddAction(new BumpAction { GameObject = Player, Speed = MoveSpeed, Direction = (1, 0) });
        }
    }
}
