using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Profiling;
using Unity.Burst;
#if UNITY_EDITOR
using UnityEditor;
#endif

//Job
[BurstCompile]
public struct Mean100Job : IJobParallelFor
{
    [ReadOnly]
    public NativeArray<int> inputs;
    [WriteOnly]
    public NativeArray<float> results;

    public void Execute(int index)
    {
        int sum = 0;
        for (int i = index * 100; i < (index + 1) * 100; ++i)
            sum += inputs[i];
        results[index] = sum / 100;
    }
}

//Component
class Mean : MonoBehaviour
{ }

//Authoring
#if UNITY_EDITOR
[CustomEditor(typeof(Mean))]
class MeanEditor : Editor
{
    [MenuItem("CONTEXT/Mean/Compute mean of 1000 int")]
    static void Compute(MenuCommand menuCommand)
    { 
        //Create inputs
        NativeArray<int> inputs = new NativeArray<int>(1000, Allocator.TempJob);
        for (int i = 0; i < 1000; ++i)
            inputs[i] = i;//Random.Range(0, 100);

        //create outputs
        NativeArray<float> results = new NativeArray<float>(10, Allocator.TempJob);

        //Init jobs
        var job = new Mean100Job();
        job.inputs = inputs;
        job.results = results;

        //Launch jobs
        Profiler.BeginSample($"Mean 1000 int parallelized jobs");
        JobHandle handle = job.Schedule(10, 1); // 1 by batch only

        //Wait completion
        handle.Complete();
        Profiler.EndSample();

        //Compute last remnant of the sum
        float mean = 0;
        for (int i = 0; i < 10; ++i)
            mean += results[i];
        mean /= 10;

        //Display result per thread to ensure everything was alright
        string debug = "";
        for (int i = 0; i < 10; ++i)
            debug += $"Job_{i}: {results[i]}\n";
        Debug.Log(debug);

        //Disposal of Native Collections
        inputs.Dispose();
        results.Dispose();

        //Display result
        Debug.Log(mean);
    }
}
#endif
