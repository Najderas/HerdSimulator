using UnityEngine;

// Put this script on a Camera
public class DrawDebug : MonoBehaviour
{
    public bool DebugModeFlocks = true;
    private Shepherd _shepherd;
    private const float Margin = 0.2f;

    private void Start()
    {
        _shepherd = FindObjectOfType<Shepherd>();
    }

    private void DrawFlocks()
    {
        if (_shepherd == null || _shepherd.Flocks == null) return;
        foreach (var flock in _shepherd.Flocks)
        {
            var contour = flock.GetFlockContour();
            DrawLine(contour["left"], contour["top"], Color.yellow);
            DrawLine(contour["top"], contour["right"], Color.yellow);
            DrawLine(contour["right"], contour["bottom"], Color.yellow);
            DrawLine(contour["bottom"], contour["left"], Color.yellow);

            var borders = flock.GetFlockBorders();
            var topLeft = new Vector3(borders["left"] - Margin, borders["top"] + Margin);
            var topRight = new Vector3(borders["right"] + Margin, borders["top"] + Margin);
            var bottomLeft = new Vector3(borders["left"] - Margin, borders["bottom"] - Margin);
            var bottomRight = new Vector3(borders["right"] + Margin, borders["bottom"] - Margin);

            DrawLine(topLeft, topRight, Color.black);
            DrawLine(topRight, bottomRight, Color.black);
            DrawLine(bottomRight, bottomLeft, Color.black);
            DrawLine(bottomLeft, topLeft, Color.black);
        }
    }

    private void DrawLine(Vector3 start, Vector3 end, Color color)
    {
        GL.Begin(GL.LINES);
        GL.Color(color);
        GL.Vertex3(start.x, start.y, 1);
        GL.Vertex3(end.x, end.y, 1);
        GL.End();
    }

    // To show the lines in the game window whne it is running
    private void OnPostRender()
    {
        if (DebugModeFlocks) DrawFlocks();
    }
}
