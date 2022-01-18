using Unity.Entities;

public struct GridSpawner : IComponentData
{
    public Entity prefab;
    public int countX;
    public int countZ;
    public float spacingX;
    public float spacingZ;
}