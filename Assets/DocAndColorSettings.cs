using UnityEngine;
using System.Collections.Generic;

public class DocAndColorSettings : MonoBehaviour
{
    public Vector2 svgSize = new Vector2(816, 1056);

    public float renderScale = 4;

    public Vector2 clippingSize = new Vector2(0, 0);

    public bool useAnimationSVGOffset = false;

    public bool generateArtWithNewSeed = false;

    public List<Color> pensToUse = new List<Color>();


    [ContextMenu("Set Colors to Black and White")]
    public void SetColorsToBW()
    {
        pensToUse = new List<Color>();

        pensToUse.Add(Color.black);
        pensToUse.Add(Color.white);
    }


    [ContextMenu("Set Colors to Blue Cyan White Magenta")]
    public void SetColorsToBCWM()
    {
        pensToUse = new List<Color>();

        pensToUse.Add(Color.blue);
        pensToUse.Add(Color.cyan);
        pensToUse.Add(Color.white);
        pensToUse.Add(Color.magenta);
    }
}
