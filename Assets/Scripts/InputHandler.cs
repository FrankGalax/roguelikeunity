using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class InputHandler
{
    public abstract void Update();
}

public class DungeonInputHandler : InputHandler
{
    private GameObject m_Player;
    private ActionQueue m_ActionQueue;

    public DungeonInputHandler()
    {
        m_ActionQueue = GameObject.FindObjectOfType<ActionQueue>();
        m_Player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    public override void Update()
    {
        if (m_Player == null || m_ActionQueue == null || m_ActionQueue.IsBusy())
        {
            return;
        }

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            m_ActionQueue.AddAction(new BumpAction { GameObject = m_Player, Direction = (0, 1) });
        }
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            m_ActionQueue.AddAction(new BumpAction { GameObject = m_Player, Direction = (-1, 0) });
        }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            m_ActionQueue.AddAction(new BumpAction { GameObject = m_Player, Direction = (0, -1) });
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            m_ActionQueue.AddAction(new BumpAction { GameObject = m_Player, Direction = (1, 0) });
        }
        else if (Input.GetKey(KeyCode.E))
        {
            m_ActionQueue.AddAction(new PickupAction { GameObject = m_Player });
        }
    }
}

public class TargetInputHandler : InputHandler
{
    public Tile MouseTile { get; private set; }
    public bool MouseDown { get; private set; }
    private GameMap m_GameMap;

    public TargetInputHandler()
    {
        m_GameMap = GameObject.FindObjectOfType<GameMap>();
    }

    public override void Update()
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        int x = (int)Math.Round(worldPosition[0]);
        int y = (int)Math.Round(worldPosition[1]);
        Tile actor = m_GameMap.GetActorAtLocation(x, y);
        if (actor != null && actor.IsVisible)
        {
            MouseTile = actor;
        }
        else
        {
            Tile tile = m_GameMap.GetTileAtLocation(x, y);
            if (tile != null && tile.IsVisible)
            {
                MouseTile = tile;
            }
        }

        MouseDown = Input.GetMouseButtonDown(0);
    }
}
