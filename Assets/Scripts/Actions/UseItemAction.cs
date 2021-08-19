public class UseItemAction : GameAction
{
    public Item Item;

    public override void Apply(GameMap gameMap)
    {
        base.Apply(gameMap);

        foreach (ItemEffect itemEffect in Item.ItemEffects)
        {
            itemEffect.Apply(GameObject);
        }

        IsDone = true;
    }
}