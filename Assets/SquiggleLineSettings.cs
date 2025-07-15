using UnityEngine;

public class SquiggleLineSettings : MonoBehaviour
{
    public int squiggleLineFillCount = 500;

    public bool squiggleUseRandomDir = true;

    public Vector2 squiggleLineDir = new Vector2(1, 1);

    public Vector2 squiggleSinDir = new Vector2(0, 1);

    public bool squiggleLinesUseSine = false;

    public bool useValueAsSquiggleProb = false;

    public Vector2 lineSegCountMinMax = new Vector2(30, 40);
}
