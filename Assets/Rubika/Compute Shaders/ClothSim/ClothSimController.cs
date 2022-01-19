using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ClothSimController : MonoBehaviour
{
    public int gridCount = 10;
    public int anchorEveryN = 10;

    public int iterations = 5;
    public float gravity = 9.81f;
    public float dampening = 0.1f;

    public bool useComputeShader = false;
    public ComputeShader computeShader;

    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private Mesh mesh;
    private MaterialPropertyBlock mpb;
    private ClothSimCSharp.ForceField[] forces;

    private ClothSimCSharp.Point[] points;
    private ClothSimCSharp.Stick[] sticks;
    private Vector3[] vertices;

    private List<ControllObject> controllObjects;

    private ComputeBuffer pointsBuffer, sticksBuffer, pointsTempPositionsBuffer, sticksPointsMappingBuffer, forcesBuffer;
    private int pointsKernel, sticksKernel, sanitizeKernel, calculateNormalsKernel;
    private int[] sticksPointsMapping;

    public bool simulate = false;
    public bool debugUpdate = false;

    public WindZone[] windZones;

    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();

        GenerateMesh();

        pointsBuffer = new ComputeBuffer(points.Length, Unity.Collections.LowLevel.Unsafe.UnsafeUtility.SizeOf<ClothSimCSharp.Point>());
        sticksBuffer = new ComputeBuffer(sticks.Length, Unity.Collections.LowLevel.Unsafe.UnsafeUtility.SizeOf<ClothSimCSharp.Stick>());
        pointsTempPositionsBuffer = new ComputeBuffer(points.Length * 4, sizeof(float) * 3);
        sticksPointsMappingBuffer = new ComputeBuffer(sticks.Length * 2, sizeof(int));

        forces = new ClothSimCSharp.ForceField[3];
        forces[0].position = new Vector3(0, 0, 1f);
        forcesBuffer = new ComputeBuffer(3, Unity.Collections.LowLevel.Unsafe.UnsafeUtility.SizeOf <ClothSimCSharp.ForceField>());

        pointsBuffer.SetData(points.ToList());
        sticksBuffer.SetData(sticks.ToList());
        pointsTempPositionsBuffer.SetData(new Vector3[points.Length * 4]);
        sticksPointsMappingBuffer.SetData(sticksPointsMapping);

        pointsKernel = computeShader.FindKernel("CSPoints");
        sticksKernel = computeShader.FindKernel("CSSticks");
        sanitizeKernel = computeShader.FindKernel("CSSanitize");
        calculateNormalsKernel = computeShader.FindKernel("CSCalculateNormals");

        mpb = new MaterialPropertyBlock();
        mpb.SetBuffer("points", pointsBuffer);
        mpb.SetInt("gridCount", gridCount);

        meshRenderer.SetPropertyBlock(mpb);
    }

    private void OnDisable()
    {
        pointsBuffer.Dispose();
        sticksBuffer.Dispose();
        pointsTempPositionsBuffer.Dispose();
        sticksPointsMappingBuffer.Dispose();
    }

    private void Update()
    {
        if (debugUpdate || simulate)
        {
            debugUpdate = false;
            Simulate();
        }
    }

    void Simulate()
    {
        forces = new ClothSimCSharp.ForceField[windZones.Length];
        for (int i = 0; i < windZones.Length; i++)
        {
            if (windZones[i].mode == WindZoneMode.Spherical)
            {
                forces[i].position = windZones[i].transform.position;
                forces[i].intensity = windZones[i].windMain + Random.Range(-0.5f, 0.5f) * windZones[i].windTurbulence;
                forces[i].intensity += windZones[i].windPulseMagnitude * Mathf.Clamp01(Mathf.Sin(Mathf.PI * 2f * Time.time * windZones[i].windPulseFrequency));
                forces[i].size = windZones[i].radius;
            }
            else
            {
                forces[i].intensity = 0;
            }
        }

        foreach(ControllObject c in controllObjects)
        {
            points[c.pointIndex].position = points[c.pointIndex].prevPosition = c.transform.localPosition;
            pointsBuffer.SetData(new ClothSimCSharp.Point[] { points[c.pointIndex] }, 0, c.pointIndex, 1) ;
        }

        if (useComputeShader)
        {
            computeShader.SetFloat("dampening", dampening);
            computeShader.SetFloat("gravity", gravity * Time.deltaTime * Time.deltaTime);

            forcesBuffer.SetData(forces);
            computeShader.SetBuffer(pointsKernel, "forces", forcesBuffer);
            computeShader.SetInt("forcesCount", forces.Length);

            computeShader.SetBuffer(pointsKernel, "points", pointsBuffer);
            computeShader.Dispatch(pointsKernel, 64, 1, 1);

            computeShader.SetBuffer(sticksKernel, "points", pointsBuffer);
            computeShader.SetBuffer(sticksKernel, "sticks", sticksBuffer);
            computeShader.SetBuffer(sticksKernel, "pointsTempPositions", pointsTempPositionsBuffer);
            computeShader.SetBuffer(sticksKernel, "sticksPointsMapping", sticksPointsMappingBuffer);

            computeShader.SetBuffer(sanitizeKernel, "points", pointsBuffer);
            computeShader.SetBuffer(sanitizeKernel, "pointsTempPositions", pointsTempPositionsBuffer);
            for (int i=0; i<iterations; i++)
            {
                computeShader.Dispatch(sticksKernel, 64, 1, 1);
                computeShader.Dispatch(sanitizeKernel, 64, 1, 1);
            }

            computeShader.SetBuffer(calculateNormalsKernel, "points", pointsBuffer);
            computeShader.Dispatch(calculateNormalsKernel, 64, 1, 1);

            

            /*
            pointsBuffer.GetData(points);
            mesh.vertices = points.Select(p => (Vector3)p.position).ToArray();
            mesh.normals = points.Select(p => (Vector3)p.normal).ToArray();
            */
        }
        else
        {
            ClothSimCSharp.Simulate(ref points, ref sticks, new ClothSimCSharp.SimuationSettings()
            {
                iterations = iterations,
                gravity = gravity,
                dampening = dampening
            });
            mesh.vertices = points.Select(p => (Vector3)p.position).ToArray();
            mesh.RecalculateNormals();
        }
    }

    void GenerateMesh()
    {
        int vertexCount = (gridCount + 1) * (gridCount + 1);
        vertices = new Vector3[vertexCount];
        var uvs = new Vector2[vertexCount];
        int[] triangles = new int[gridCount * gridCount * 6];
        points = new ClothSimCSharp.Point[vertexCount];
        sticks = new ClothSimCSharp.Stick[gridCount * (gridCount+1) * 2];
        sticksPointsMapping = new int[sticks.Length * 2];

        int s = 0;
        float stickLength = 1f / gridCount;

        controllObjects = new List<ControllObject>();

        for (int x = 0; x<=gridCount; x++)
        {
            for (int y = 0; y<=gridCount; y++)
            {
                int i = x + y * (gridCount + 1);
                var pos = new Vector3(0.5f - x * 1f / gridCount, 0.5f - y * 1f / gridCount, 0);
                vertices[i] = pos;
                uvs[i] = new Vector2(x, y);
                var isControllPoint = (y == 0) & (x%anchorEveryN == 0);
                points[i] = new ClothSimCSharp.Point()
                {
                    position = pos,
                    prevPosition = pos,
                    locked = isControllPoint?1:0,
                    connectionsCounts = 0,
                    neighborUp = (y>0)?i-gridCount-1:-1,
                    neighborDown = (y<gridCount)?i+gridCount+1:-1,
                    neighborRight = (x>0)?i-1:-1,
                    neighborLeft = (x<gridCount)?i+1:-1
                };
                if (isControllPoint)
                {
                    var controller = new ControllObject()
                    {
                        pointIndex = i,
                        transform = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform
                    };
                    controller.transform.parent = transform;
                    controller.transform.localPosition = pos;
                    controller.transform.localScale = Vector3.one * 0.1f;
                    controllObjects.Add(controller);
                }

                if (x < gridCount)
                {
                    sticks[s] = new ClothSimCSharp.Stick()
                    {
                        pointA = i,
                        pointB = i+1,
                        length = stickLength
                    };
                    s++;

                    if (y<gridCount)
                    {
                        var t = ( x + y * gridCount ) * 6;
                        triangles[t  ] = i;
                        triangles[t+1] = i + gridCount + 1; 
                        triangles[t+2] = i + 1;

                        triangles[t+3] = i+1;
                        triangles[t+4] = i+gridCount+1;
                        triangles[t+5] = i+gridCount+2;
                    }
                }

                if (y<gridCount)
                {
                    sticks[s] = new ClothSimCSharp.Stick()
                    {
                        pointA = i,
                        pointB = i + gridCount+1,
                        length = stickLength
                    };
                    s++;
                }
            }
        }

        for (int i =0; i<sticks.Length; i++)
        {
            var stick = sticks[i];
            sticksPointsMapping[i*2] = stick.pointA * 4 + points[stick.pointA].connectionsCounts;
            points[stick.pointA].connectionsCounts++;
            sticksPointsMapping[i*2+1] = stick.pointB * 4 + points[stick.pointB].connectionsCounts;
            points[stick.pointB].connectionsCounts++;
        }

        mesh = new Mesh();
        mesh.name = "Cloth";
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.SetUVs(0,uvs);
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();

        meshFilter.sharedMesh = mesh;
    }

    struct ControllObject
    {
        public Transform transform;
        public int pointIndex;
    }
}
