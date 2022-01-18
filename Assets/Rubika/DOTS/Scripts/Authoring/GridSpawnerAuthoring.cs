using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
public class GridSpawnerAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GameObject prefab;
    [Range(1, 1000)] public int countX;
    [Range(1, 1000)] public int countZ;
    [Range(0, 5)] public float spacingX;
    [Range(0, 5)] public float spacingZ;
    
    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(prefab);
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new GridSpawner
        {
            prefab = conversionSystem.GetPrimaryEntity(prefab),
            countX = countX,
            countZ = countZ,
            spacingX = spacingX,
            spacingZ = spacingZ,
        });
    }
}
