using Unity.Entities;

[GenerateAuthoringComponent]
public struct Elevate : IComponentData
{
    public float minY;
    public float maxY;
}
