public class CastSpellAction : GameAction
{
    public int SpellIndex { get; set; }

    public override void Apply(GameMap gameMap)
    {
        base.Apply(gameMap);

        IsSuccess = false;
        IsDone = true;

        SpellComponent spellComponent = GameObject.GetComponent<SpellComponent>();
        if (spellComponent == null)
        {
            return;
        }

        SpellInstance spellInstance = spellComponent.GetSpellInstance(SpellIndex);
        if (spellInstance == null)
        {
            return;
        }

        spellInstance.Cast(GameObject, gameMap);
    }

    public override string GetDebugString()
    {
        return "CastSpellAction with SpellIndex " + SpellIndex;
    }
}