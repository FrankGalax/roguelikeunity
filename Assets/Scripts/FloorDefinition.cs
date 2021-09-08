using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "NewFloorDefinition", menuName = "FloorDefinitions/FloorDefinition")]
public class FloorDefinition : ScriptableObject
{
    public int MaxMobsPerRoom;
    public int MinMobsPerRoom;
    public int MaxItemsPerRoom;
    public int MinItemsPerRoom;
    public List<Spawnable> Mobs;
    public List<Spawnable> Items;
    public GameObject Boss;
    public string TemplateFileName;

    public GameObject GetMob()
    {
        return GetSpawnable(Mobs);
    }

    public GameObject GetItem()
    {
        return GetSpawnable(Items);
    }

    private GameObject GetSpawnable(List<Spawnable> spawnables)
    {
        int weightSum = 0;
        foreach (Spawnable spawnable in spawnables)
        {
            weightSum += spawnable.weight;
        }

        int r = UnityEngine.Random.Range(0, weightSum);
        foreach (Spawnable spawnable in spawnables)
        {
            if (r < spawnable.weight)
            {
                return spawnable.GameObject;
            }

            r -= spawnable.weight;
        }

        return null;
    }
}

[Serializable]
public class Spawnable
{
    public GameObject GameObject;
    public int weight;
}