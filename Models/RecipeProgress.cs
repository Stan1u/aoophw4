public class RecipeProgress
{
    public string RecipeName { get; set; }
    public string CurrentStep { get; set; }
    public int StepIndex { get; set; }
    public int TotalSteps { get; set; }
    public double ProgressPercent => (double)StepIndex / TotalSteps * 100;
    public bool IsFinished => StepIndex >= TotalSteps;
}
