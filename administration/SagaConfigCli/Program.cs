using System.CommandLine;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using EventDriven.Sagas.Configuration.Abstractions;
using EventDriven.Sagas.Configuration.Abstractions.DTO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SagaConfigCli;

// Program arguments:
// -n SagaConfigDefinitions -p ../../../../../reference-architecture/SagaConfigDefinitions/bin/Debug/net6.0 -id d89ffb1e-7481-4111-a4dd-ac5123217293 -j ../../../../../reference-architecture/SagaConfigDefinitions/json -uri http://localhost:5256/api/sagaconfig/

// Tool installation:
// dotnet tool install -g --add-source <project_root_path>/bin/Debug EventDriven.Sagas.SagaConfig.CLI --version 1.0.0-beta1

// Usage:
// cd <path>/SagaConfigDefinitions
// sagaconfig -id d89ffb1e-7481-4111-a4dd-ac5123217293 -p bin/Debug/net6.0 -j json
// sagaconfig -id d89ffb1e-7481-4111-a4dd-ac5123217293 -p bin/Debug/net6.0 -j json -uri http://localhost:5256/api/sagaconfig/

var host = Host
    .CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHttpClient();
    })
    .ConfigureLogging(logging => logging.SetMinimumLevel(LogLevel.Error))
    .Build();

// Options
var sagaConfigDefNameOption = new Option<string?>(new[] { "-n", "--assembly-name" }, 
    () => "SagaConfigDefinitions", "Name of the assembly containing saga config definitions");
var sagaConfigDefPathOption = new Option<string?>(new[] { "-p", "--assembly-path" },
    () => ".", "Path to folder in which saga config assembly is located");
var sagaConfigIdOption = new Option<string?>(new[] { "-i", "-id", "--id" },
    () => Guid.NewGuid().ToString(), "Saga config identifier");
var sagaConfigJsonPathOption = new Option<string?>(new[] { "-j", "--json-path" },
    "Path to folder in which to place saga config JSON");
var sagaConfigServiceUriOption = new Option<string?>(new[] { "-u", "-uri", "--service-uri" },
    "Saga config service URI");

sagaConfigIdOption.AddValidator(result =>
{
    if (!Guid.TryParse(result.GetValueForOption(sagaConfigIdOption), out _))
        result.ErrorMessage = $"Cannot convert option '{sagaConfigIdOption.Name}' to Guid.";
});
sagaConfigServiceUriOption.AddValidator(result =>
{
    if (!Uri.TryCreate(result.GetValueForOption(sagaConfigServiceUriOption), UriKind.Absolute, out _))
        result.ErrorMessage = $"Cannot convert option '{sagaConfigServiceUriOption.Name}' to Uri.";
});

var rootCommand = new RootCommand
{
    sagaConfigDefPathOption,
    sagaConfigDefNameOption,
    sagaConfigIdOption,
    sagaConfigJsonPathOption,
    sagaConfigServiceUriOption
};

rootCommand.Name = "sagaconfig";
rootCommand.Description = "Saga Configuration CLI";

rootCommand.SetHandler((string? sagaConfigDefPath, string? sagaConfigDefName, string? sagaConfigIdString,
    string? sagaConfigJsonPath, string? sagaConfigServiceUriString) =>
{
    // Get saga config definitions path
    if (sagaConfigDefPath == null && sagaConfigDefName == null
        || !Guid.TryParse(sagaConfigIdString, out var sagaConfigId)) return;
    var sagaConfigAssemblyPath = GetSagaConfigAssemblyPath(sagaConfigDefPath, sagaConfigDefName);
    if (sagaConfigAssemblyPath == null) return;
    
    // Get saga config
    var sagaConfig = GetSagaConfiguration(sagaConfigAssemblyPath,
        sagaConfigId, out var sagaConfigDefTypeName);
    if (sagaConfigDefTypeName == null) return;
    
    // Save json
    if (sagaConfigJsonPath != null)
    {
        if (!Directory.Exists(sagaConfigJsonPath))
        {
            Console.WriteLine($"Directory does not exist: '{sagaConfigJsonPath}'");
            return;
        }
        var sagaConfigJson = GetSagaConfigurationJson(sagaConfig);
        var sagaConfigJsonFullPath = Path.Join(sagaConfigJsonPath, $"{sagaConfigDefTypeName}.json");
        SaveSagaConfigurationJson(sagaConfigJson, sagaConfigJsonFullPath);
        Console.WriteLine("Saved saga configuration.");
    }

    // Invoke saga config service
    if (sagaConfigServiceUriString != null && Uri.TryCreate(sagaConfigServiceUriString, UriKind.Absolute,
            out var sagaConfigServiceUri))
    {
        var httpClientFactory = host.Services.GetRequiredService<IHttpClientFactory>();
        var sagaConfigService = new SagaConfigService(httpClientFactory, sagaConfigServiceUri);
        _ = sagaConfigService.UpsertSagaConfiguration(sagaConfig).Result;
        Console.WriteLine("Posted saga configuration.");
    }
}, sagaConfigDefPathOption, sagaConfigDefNameOption, sagaConfigIdOption,
    sagaConfigJsonPathOption, sagaConfigServiceUriOption);
return rootCommand.Invoke(args);

string? GetSagaConfigAssemblyPath(string? sagaConfigDefPath, string? sagaConfigDefName)
{
    if (!Directory.Exists(sagaConfigDefPath))
    {
        Console.WriteLine($"Directory does not exist: '{sagaConfigDefPath}'");
        return null;
    }
    var relativePath = Directory.GetFiles(sagaConfigDefPath, "*.dll", SearchOption.AllDirectories)
        .FirstOrDefault(f => string.Compare(Path.GetFileName(f), $"{sagaConfigDefName}.dll", StringComparison.OrdinalIgnoreCase) == 0);
    if (relativePath != null) return Path.GetFullPath(relativePath);
    Console.WriteLine($"'{sagaConfigDefName}.dll' not found in the directory '{sagaConfigDefPath}'");
    return null;
}

SagaConfigurationDto? GetSagaConfiguration(string file, Guid configId, out string? sagaConfigDefTypeName)
{
    Assembly asm;
    try
    {
        asm = Assembly.LoadFrom(file);
    }
    catch (FileNotFoundException)
    {
        Console.WriteLine($"Could not locate the file '{file}'");
        sagaConfigDefTypeName = null;
        return null;
    }
    
    var sagaConfigDefType = asm
        .GetExportedTypes()
        .FirstOrDefault(t => typeof(ISagaConfigDefinition).IsAssignableFrom(t)
            && !t.IsInterface && !t.IsAbstract);
    sagaConfigDefTypeName = sagaConfigDefType?.Name;
    if (sagaConfigDefType == null) return null;
    var sagaConfigDef = (ISagaConfigDefinition?)Activator.CreateInstance(sagaConfigDefType);
    return sagaConfigDef?.CreateSagaConfig(configId);
}

string? GetSagaConfigurationJson(SagaConfigurationDto? sagaConfig)
{
    if (sagaConfig == null) return null;
    var options = new JsonSerializerOptions
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };
    return JsonSerializer.Serialize(sagaConfig, options);
}

void SaveSagaConfigurationJson(string? sagaConfigJson, string sagaConfigJsonPath)
{
    if (sagaConfigJson == null) return;
    File.WriteAllText(sagaConfigJsonPath, sagaConfigJson);
}

