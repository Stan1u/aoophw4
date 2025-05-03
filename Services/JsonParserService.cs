
using System.Text.Json;
using System;

using System.Collections.Generic;
using System.IO;


using MyKitchenSim.Models;
public static class JsonParserService
{
    public static KitchenData LoadFromFile(string filePath)
    {
        try
        {
            var json = File.ReadAllText(filePath);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Deserialize<KitchenData>(json, options)
                   ?? throw new Exception("JSON parsed as null");
        }
        catch (Exception ex)
        {
            // Log or handle error
            throw new ApplicationException($"Failed to parse JSON: {ex.Message}", ex);
        }
    }
}
