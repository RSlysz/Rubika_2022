using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class RotateSystem : SystemBase
{
    public float baseSpeed = 0.1f;

    protected override void OnUpdate()
    {
        var delta = baseSpeed * math.radians(Time.DeltaTime * 360);
        
        Entities.ForEach((ref Rotation rotation, in Rotate rotate) => {
            rotation.Value = math.normalize(math.mul(rotation.Value, quaternion.RotateY(delta * rotate.speedMultiplier)));
        }).ScheduleParallel();
    }
}
