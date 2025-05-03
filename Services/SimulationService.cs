using MyKitchenSim;
using MyKitchenSim.Models;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using Avalonia.Threading;

public class SimulationService
{
    private readonly ObservableCollection<RecipeProgress> _progressList;
    private readonly SemaphoreSlim _stationLimiter;
    private readonly CancellationTokenSource _cts = new();

    public SimulationService(ObservableCollection<RecipeProgress> progressList, int kitchenStations = 3)
    {
        _progressList = progressList;
        _stationLimiter = new SemaphoreSlim(kitchenStations);
    }

    public async Task StartSimulationAsync(List<Recipe> recipes)
    {
        List<Task> tasks = new();

        foreach (var recipe in recipes)
        {
            tasks.Add(Task.Run(() => SimulateRecipeAsync(recipe, _cts.Token)));
        }

        await Task.WhenAll(tasks);
    }

    public void StopSimulation()
    {
        _cts.Cancel();
    }

    private async Task SimulateRecipeAsync(Recipe recipe, CancellationToken token)
    {
        await _stationLimiter.WaitAsync(token);

        try
        {
            var progress = new RecipeProgress
            {
                RecipeName = recipe.Name,
                StepIndex = 0,
                TotalSteps = recipe.Steps.Count,
                CurrentStep = "Starting..."
            };

            lock (_progressList)
            {
                _progressList.Add(progress);
            }

            for (int i = 0; i < recipe.Steps.Count; i++)
            {
                if (token.IsCancellationRequested) break;

                var step = recipe.Steps[i];
                progress.CurrentStep = step.Step;
                progress.StepIndex = i + 1;

                // Notify UI thread (since ObservableCollection isn't thread-safe)
                Dispatcher.UIThread.Post(() =>
                {
                    var index = _progressList.IndexOf(progress);
                    if (index >= 0)
                    {
                        _progressList[index] = new RecipeProgress
                        {
                            RecipeName = progress.RecipeName,
                            StepIndex = progress.StepIndex,
                            TotalSteps = progress.TotalSteps,
                            CurrentStep = progress.CurrentStep
                        };
                    }
                });

                await Task.Delay(step.Duration * 1000, token); // simulate step duration
            }
        }
        catch (OperationCanceledException)
        {
            // optional: log cancellation
        }
        finally
        {
            _stationLimiter.Release();
        }
    }
}
