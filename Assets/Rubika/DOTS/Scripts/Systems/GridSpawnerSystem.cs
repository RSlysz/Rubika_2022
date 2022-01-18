using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class GridSpawnerSystem : SystemBase
{
    protected override void OnUpdate() 
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var random = new Random((uint)System.DateTime.Now.Ticks);

        Entities.ForEach((Entity entity, in GridSpawner spawner, in LocalToWorld ltw) =>
        {
            ecb.DestroyEntity(entity);

            for (int x = 0; x < spawner.countX; ++x)
                for (int z = 0; z < spawner.countZ; ++z)
                {
                    var posX = spawner.spacingX * (x - (spawner.countX - 1) / 2);
                    var posZ = spawner.spacingZ * (z - (spawner.countZ - 1) / 2);
                    var translation = new Translation {Value = ltw.Position + new float3(posX, 0, posZ)};
                    var instance = ecb.Instantiate(spawner.prefab);
                    ecb.SetComponent(instance, translation);

                    if (random.NextBool())
                        ecb.AddComponent(instance, new Colorize());
                }
        }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();

        // Note: Run is in main thread. Schedule is on a parallel thread. ScheduleParallel is to run it on several parallel thread.
    }
}