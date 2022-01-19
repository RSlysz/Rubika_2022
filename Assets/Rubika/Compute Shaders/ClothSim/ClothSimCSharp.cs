using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class ClothSimCSharp
{
    public struct Point
    {
        public float3 position, prevPosition, normal;
        public int locked;
        public int connectionsCounts;
        public int neighborUp;
        public int neighborDown;
        public int neighborLeft;
        public int neighborRight;
    }

    public struct Stick
    {
        public int pointA, pointB;
        public float length;
    }

    public struct SimuationSettings
    {
        public int iterations;
        public float gravity;
        public float dampening;
    }

    public struct ForceField
    {
        public float3 position;
        public float size;
        public float intensity;
    }

    public static void Simulate (ref Point[] points, ref Stick[] sticks, SimuationSettings settings)
    {
        int i = 0;
        for (i=0; i<points.Length; i++)
        {
            var p = points[i];
            if (p.locked != 1)
            {
                var positionBefore = p.position;
                p.position += ( p.position - p.prevPosition ) *(1f- settings.dampening);
                p.position.y -= settings.gravity * Time.deltaTime * Time.deltaTime;
                p.prevPosition = positionBefore;
                points[i] = p;
            }
        }

        for (int j=0; j< settings.iterations; j++)
        {
            for (i=0; i<sticks.Length; i++)
            {
                var s = sticks[i];
                var center = (points[s.pointA].position + points[s.pointB].position) / 2f;
                var dir = math.normalize(points[s.pointA].position - points[s.pointB].position);
                if (points[s.pointA].locked!=1)
                    points[s.pointA].position = center + dir * s.length / 2f;
                if (points[s.pointB].locked!=1)
                    points[s.pointB].position = center - dir * s.length / 2f;

                sticks[i] = s;
            }
        }
    }
}
