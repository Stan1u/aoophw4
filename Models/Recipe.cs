namespace MyKitchenSim.Models;
using System.Collections.Generic;

public class Recipe


{
    public string Name { get; set; }
    public string Difficulty { get; set; }
    public List<string> Equipment { get; set; }
    public List<RecipeStep> Steps { get; set; }
}
