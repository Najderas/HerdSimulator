using Assets;

public class OutlierSheepAgent : SheepAgent
{
    public OutlierSheepAgent() :base()
    {
        FkParams = new Parameters()
        {
            CohesionCoefficient = 0.75f,
            AlignmentCoefficient = 0.05f,
            SeparationCoefficient = 0.2f
        };

        MaxCooldown = 600;
        FlockLeaveProbability = 0.999f;
    }
}
