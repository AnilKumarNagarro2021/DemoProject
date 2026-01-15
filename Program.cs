using System.Text.Json;
using ConsoleApp.Agents;
using ConsoleApp.Models;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

Console.WriteLine("Running code reviewers...");

var reviewers = new ICodeReviewer[]
{
    new StyleReviewer(),
    new TodoReviewer()
};

var csFiles = System.IO.Directory.GetFiles(System.AppContext.BaseDirectory, "*.cs", System.IO.SearchOption.AllDirectories);
var results = new System.Collections.Generic.List<ReviewResult>();

foreach (var file in csFiles)
{
    // skip compiled generated files in bin/obj
    if (file.Contains(System.IO.Path.DirectorySeparatorChar + "bin" + System.IO.Path.DirectorySeparatorChar) ||
        file.Contains(System.IO.Path.DirectorySeparatorChar + "obj" + System.IO.Path.DirectorySeparatorChar))
        continue;

    foreach (var r in reviewers)
    {
        var res = await r.ReviewAsync(file);
        results.Add(res);
        Console.WriteLine($"{res.FilePath} - {res.AgentName} - {(res.Passed ? "OK" : "Issue")}: {res.Message}");
    }
}

var outPath = System.IO.Path.Combine(System.AppContext.BaseDirectory, "code-review-report.json");
await System.IO.File.WriteAllTextAsync(outPath, JsonSerializer.Serialize(results, new JsonSerializerOptions { WriteIndented = true }));

Console.WriteLine($"Review complete. Report written to: {outPath}");
