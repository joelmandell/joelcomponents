using Microsoft.AspNetCore.Razor.Language;

namespace JoelComponents.Services;

using Microsoft.AspNetCore.Razor.Language; // Add this using
using System.Text;
using System.Reflection;
using Microsoft.AspNetCore.Components;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

// Helper class to represent a Razor file that exists only in memory
public class VirtualRazorProjectItem : RazorProjectItem
{
    private readonly byte[] _content;

    public VirtualRazorProjectItem(string filePath, string content)
    {
        // The engine needs a file path to identify the item
        FilePath = filePath;
        PhysicalPath = filePath; // Can be the same for in-memory items
        _content = Encoding.UTF8.GetBytes(content);
    }

    public override string? BasePath { get; }
    public override string FilePath { get; }
    public override string PhysicalPath { get; }
    public override bool Exists => true;
    public override Stream Read() => new MemoryStream(_content);
}

public class CompilerService
{
  public (Type? ComponentType, string[] Errors) CompileDeclarativeComponent(
        string razorContent)
  {
       // razorContent = $"@using static Microsoft.AspNetCore.Components.Web.RenderMode{Environment.NewLine}@rendermode Microsoft.AspNetCore.Components.Web.InteractiveWebAssembly{Environment.NewLine}{razorContent}";
        // 1. LOAD ALL REFERENCES
        // Start with the application's base assemblies
        var baseAssemblyPaths = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
            .Select(a => a.Location);

        // Combine base assemblies with the new external DLL paths
        var allAssemblyPaths = baseAssemblyPaths.Distinct();

        var references = allAssemblyPaths
            .Select(path => MetadataReference.CreateFromFile(path))
            .ToList();

        // 2. CREATE A "DESIGN-TIME" COMPILATION
        // This compilation's only purpose is to help the Razor engine discover Tag Helpers.
        var designTimeCompilation = CSharpCompilation.Create("DesignTimeAssembly")
            .AddReferences(references);

        // // 3. DISCOVER TAG HELPERS (COMPONENTS) FROM THE COMPILATION
        // var tagHelperProvider = new DefaultTagHelperDescriptorProvider();
        // var context = TagHelperDescriptorProviderContext.Create(designTimeCompilation);
        // tagHelperProvider.Execute(context);
        // var tagHelpers = context.Results;

        // 4. CONFIGURE THE RAZOR PROJECT ENGINE
        var fileSystem = RazorProjectFileSystem.Create(".");
        var projectEngine = RazorProjectEngine.Create(RazorConfiguration.Default, fileSystem, builder =>
        {
            // IMPORTANT: Add the discovered Tag Helpers to the engine
            builder.SetRootNamespace("JoelComponents.UI.Components.Menu"); // Set a predictable namespace

           // builder.AddTagHelpers(tagHelpers);
        });

        // 5. GENERATE C# CODE FROM THE RAZOR FILE (same as before)
        var virtualFile = new VirtualRazorProjectItem("/DynamicComponent.razor", razorContent);
        RazorCodeDocument codeDocument = projectEngine.Process(virtualFile);
        RazorCSharpDocument csharpDocument = codeDocument.GetCSharpDocument();

        var razorDiagnostics = csharpDocument.Diagnostics;
        if (razorDiagnostics.Any(d => d.Severity == RazorDiagnosticSeverity.Error))
        {
            return (null, razorDiagnostics.Select(d => d.GetMessage()).ToArray());
        }
        string generatedCsCode = csharpDocument.GeneratedCode;

        // 6. COMPILE THE GENERATED C# CODE WITH ROSLYN (using all references)
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(generatedCsCode);

        // The final compilation must also have all the references
        var finalCompilation = CSharpCompilation.Create(
            $"DynamicComponentAssembly_{Guid.NewGuid()}",
            syntaxTrees: new[] { syntaxTree },
            references: references, // Use the same full list of references
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        using var ms = new MemoryStream();
        EmitResult result = finalCompilation.Emit(ms);

        if (!result.Success)
        {
            var errors = result.Diagnostics
                .Where(d => d.Severity == DiagnosticSeverity.Error)
                .Select(d => $"{d.Id}: {d.GetMessage()}")
                .ToArray();
            return (null, errors);
        }

        // 7. LOAD THE ASSEMBLY (same as before)
        ms.Seek(0, SeekOrigin.Begin);
        Assembly assembly = Assembly.Load(ms.ToArray());
        Type? componentType = assembly.GetExportedTypes()
            .FirstOrDefault(t => t.IsSubclassOf(typeof(ComponentBase)));

        if (componentType == null)
        {
            return (null, new[] { "Error: No public class inheriting from 'ComponentBase' was found." });
        }

        return (componentType, Array.Empty<string>());
    }
}
