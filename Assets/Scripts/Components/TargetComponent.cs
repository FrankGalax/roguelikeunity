using UnityEngine;

public class TargetComponent : MonoBehaviour
{
    public Tile TargetTile { get; set; }
    public int Radius { get; set; }
    public (int, int)? Direction { get; set; }
    public bool IsCanceled { get; set; }

    public void Reset()
    {
        TargetTile = null;
        Radius = 0;
        Direction = null;
        IsCanceled = false;
    }
}
