using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using System.Linq;

public class Burster : MonoBehaviour
{
    public List<Vector2> HandleJob(Color[] pixelArray, Color layerColor, int renderTexWidth)
    {
        // Step 1: Get pixel data
        //Color32[] pixelArray = texture.GetPixels32();
        NativeArray<Color> pixels = new NativeArray<Color>(pixelArray, Allocator.TempJob);


        // Step 2: Create a thread-safe queue for results
        NativeQueue<Vector2> matchingPositions = new NativeQueue<Vector2>(Allocator.TempJob);
        NativeQueue<Vector2>.ParallelWriter queueWriter = matchingPositions.AsParallelWriter();


        // Step 2: Create and schedule the job
        var job = new TextureProcessJob
        {
            pixels = pixels,
            thisColorLayer = layerColor,
            imageWidth = renderTexWidth,
            matchingPositions = queueWriter
            //multiplier = brightnessMultiplier
        };
        JobHandle handle = job.Schedule(pixels.Length, 128); // 64 is the batch size

        // Step 3: Wait for the job to complete
        handle.Complete();


        // Step 5: Collect results from the queue
        NativeArray<Vector2> results = matchingPositions.ToArray(Allocator.Temp);
        //Debug.Log($"Found {results.Length} matching pixels.");


        List<Vector2> listlistlist = results.ToList();


        // Step 5: Clean up
        pixels.Dispose();
        matchingPositions.Dispose();
        results.Dispose();


        return listlistlist;
    }



    [BurstCompile] // Enable Burst compilation for this job
    public struct TextureProcessJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<Color> pixels; // Input/output pixel data
        [ReadOnly] public Color thisColorLayer;
        [ReadOnly] public int imageWidth;


        public NativeQueue<Vector2>.ParallelWriter matchingPositions; // Thread-safe queue for results


        public void Execute(int index)
        {
            if (thisColorLayer == pixels[index])
            {
                // Calculate the pixel position (x, y) from the index
                int x = index % imageWidth;
                int y = index / imageWidth;
                matchingPositions.Enqueue(new Vector2(x, y));
            }
        }
    }















































    public List<Color> HandleColorListJob(Color[] pixelArray)
    {
        // Step 1: Get pixel data
        NativeArray<Color> pixels = new NativeArray<Color>(pixelArray, Allocator.TempJob);

        // Step 2: Create a set for unique colors
        NativeHashSet<Color> uniqueColors = new NativeHashSet<Color>(pixels.Length, Allocator.TempJob);

        // Step 3: Create and schedule the job
        var job = new ColorListProcessJob
        {
            pixels = pixels,
            uniqueColors = uniqueColors
        };
        JobHandle handle = job.Schedule();
        handle.Complete();

        // Step 4: Extract unique colors from the set
        NativeArray<Color> uniqueColorArray = uniqueColors.ToNativeArray(Allocator.Temp);
        Debug.Log($"Found {uniqueColorArray.Length} unique colors.");

        // Step 5: Process or use the unique colors (e.g., print them)
        for (int i = 0; i < uniqueColorArray.Length; i++)
        {
            //Debug.Log($"Unique color: {uniqueColorArray[i]}");
        }

        List<Color> uniqueColorList = uniqueColorArray.ToList();

        // Step 6: Clean up
        pixels.Dispose();
        uniqueColors.Dispose();
        uniqueColorArray.Dispose();

        return uniqueColorList;
    }



    [BurstCompile] // Enable Burst compilation for this job
    public struct ColorListProcessJob : IJob
    {
        [ReadOnly] public NativeArray<Color> pixels;
        public NativeHashSet<Color> uniqueColors;

        public void Execute()
        {
            for (int i = 0; i < pixels.Length; i++)
            {
                uniqueColors.Add(pixels[i]);
            }
        }
    }
}
