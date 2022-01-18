using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace LegoDOTS
{
    public class ElevateSystem : SystemBase
    {
        Transform playerPosition;
        const float minY = -5.99f;
        const float maxY = 0f;

        protected override void OnUpdate()
        {
            if (playerPosition == null || playerPosition.Equals(null))
                playerPosition = GameObject.FindGameObjectWithTag("Player")?.transform;

            Vector2 position;
            float elevateMax;

            if (playerPosition == null)
            {
                position = default; // (0,0) is far away
                elevateMax = minY;
            }
            else
            {
                position = new Vector2(playerPosition.position.x, playerPosition.position.z);

                //this is to have a nice lerp effect when player fall down from the bridge
                elevateMax = math.clamp(playerPosition.position.y - 6f, minY, maxY);
            }

            Entities.ForEach((ref Translation entityPosition, in Elevate elevate) =>
            {
                var dist = math.distancesq(new float2(entityPosition.Value.x, entityPosition.Value.z), position);
                var coef = dist > 31.416 ? 0f : math.min(1f, 0.525f + .525f * math.cos(.1f * dist)); 
                entityPosition.Value.y = minY + (elevateMax - minY) * coef;
            }).ScheduleParallel();
        }
    }
}
