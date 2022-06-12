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

        if (!spellComponent.CanCastSpell(SpellIndex))
        {
            return;
        }

        spellComponent.Cast(SpellIndex, GameObject, gameMap);
    }

    public override string GetDebugString()
    {
        return "CastSpellAction with SpellIndex " + SpellIndex;
    }
}