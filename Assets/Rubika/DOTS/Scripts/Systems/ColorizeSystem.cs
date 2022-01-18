using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;

[UpdateAfter(typeof(GridSpawnerSystem))]
public class ColorizeSystem : SystemBase
{
    protected override void OnUpdate() 
    { 
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var random = new Random((uint)System.DateTime.Now.Ticks);

        Entities.WithNone<HDRPMaterialPropertyBaseColor>().ForEach((Entity entity, in Colorize colorize) =>
        {
            ecb.AddComponent(entity, new HDRPMaterialPropertyBaseColor() { Value = random.NextFloat4() });
        }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();

        //Note: Speedest pattern is:
        // - add HDRPMaterialPropertyBaseColor on item that need coloration instead of relying on Colorize tag, in the spawner
        // - Here have a query that gather those component
        // - then remove those component HDRPMaterialPropertyBaseColor to ensure no one will be in the query anymore.
        //   To remove them, the speedes is to use: EntityManager.RemoveComponent<HDRPMaterialPropertyBaseColor>(GetEntityQuery(typeof(HDRPMaterialPropertyBaseColor))));
    }
}
