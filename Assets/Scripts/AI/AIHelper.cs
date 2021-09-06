using UnityEngine;

public class AIHelper
{
    public static GameAction GetEffectGameplayAction(GameObject gameObject, GameObject player)
    {
        EffectComponent effectComponent = gameObject.GetComponent<EffectComponent>();
        if (effectComponent != null)
        {
            if (effectComponent.HasEffect(EffectType.Confusion))
            {
                int r = UnityEngine.Random.Range(0, 4);
                (int, int) direction = (0, 0);
                switch (r)
                {
                    case 0:
                        direction = (0, 1);
                        break;
                    case 1:
                        direction = (-1, 0);
                        break;
                    case 2:
                        direction = (0, -1);
                        break;
                    case 3:
                        direction = (1, 0);
                        break;
                }

                Tile tile = gameObject.GetComponent<Tile>();
                Tile playerTile = player.GetComponent<Tile>();

                int destX = tile.X + direction.Item1;
                int destY = tile.Y + direction.Item2;
                if (playerTile.X == destX && playerTile.Y == destY)
                {
                    return new MeleeAction { GameObject = gameObject, Target = player };
                }

                return new MoveAction { GameObject = gameObject, Direction = direction };
            }

            if (effectComponent.HasEffect(EffectType.Sleep))
            {
                return new PassAction { GameObject = gameObject };
            }
        }

        return null;
    }
}