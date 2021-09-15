using UnityEngine;

public class TargetComponent : MonoBehaviour
{
    public Tile TargetTile { get; set; }
    public int Radius { get; set; }
    public (int, int)? Direction { get; set; }
}
