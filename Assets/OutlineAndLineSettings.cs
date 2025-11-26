using UnityEngine;

public class OutlineAndLineSettings : MonoBehaviour
{
    public bool drawColorBoundsOutlines = false;

    public int addedOutlineCount = 0;

    public int addedOutlineDist = 10;
    
    public float combineLineDist = 3f;

    //This is jank and not needed once I find a better way of assigning lines to colors
    public bool ignoreFirstLinePointForWiggle = true;
}
