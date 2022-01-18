using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
public class RotateAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    [Range(0, 100)] public int speedMultiplier;
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Rotate
        {
            speedMultiplier = speedMultiplier
        });
    }
}
