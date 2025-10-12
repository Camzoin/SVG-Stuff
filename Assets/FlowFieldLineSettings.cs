using UnityEngine;

public class FlowFieldLineSettings : MonoBehaviour
{
    public int flowFieldLineFillCount = 500;

    public bool flowFieldUseRandomDir = true;

    public bool useValueAsFlowFieldProb = false;

    public Vector2 flowFieldLineDir = new Vector2(0, 0);

    public Vector2 lineSegCountMinMax = new Vector2(30, 40);

    public bool endLineAtColorChange = false;

    public bool onlyUseFirstAndLastPoints = false;
}
