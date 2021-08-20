public class UseItemAction : GameAction
{
    public Item Item;

    public override void Apply(GameMap gameMap)
    {
        base.Apply(gameMap);

        Item.Apply(GameObject, gameMap);

        IsSuccess = !Item.IsCastingSpell;
        IsDone = true;
    }

    public override string GetDebugString()
    {
        return "UseItemAction with Item " + Item.name;
    }
}