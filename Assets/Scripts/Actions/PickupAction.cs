using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupAction : GameAction
{
    public float WaitTime = 0.5f;

    private float m_Timer;
    private GameObject m_Target;

    public override void Apply(GameMap gameMap)
    {
        base.Apply(gameMap);

        Tile tile = GameObject.GetComponent<Tile>();
        Tile actorTile = gameMap.GetActorAtLocation(tile.X, tile.Y);

        if (actorTile == null)
        {
            IsDone = true;
            IsSuccess = false;
            return;
        }

        ItemComponent itemComponent = actorTile.GetComponent<ItemComponent>();
        if (itemComponent == null)
        {
            IsDone = true;
            IsSuccess = false;
            return;
        }

        InventoryComponent inventoryComponent = GameObject.GetComponent<InventoryComponent>();
        if (inventoryComponent == null)
        {
            IsDone = true;
            IsSuccess = false;
            return;
        }

        if (inventoryComponent.Items.Count > inventoryComponent.MaxItems)
        {
            IsDone = true;
            IsSuccess = false;
            return;
        }

        m_Target = actorTile.gameObject;
        inventoryComponent.AddItem(itemComponent.Item);
        gameMap.RemoveActor(actorTile);
    }

    public override void Update(GameMap gameMap)
    {
        base.Update(gameMap);

        m_Timer -= Time.deltaTime;
        if (m_Timer <= 0)
        {
            IsDone = true;
        }
    }

    public override string GetDebugString()
    {
        return "PickupAction with Target " + (m_Target != null ? m_Target.name : "NULL");
    }
}
