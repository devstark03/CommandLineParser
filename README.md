# StarKNET CommandLine

A lightweight, simple command line argument parser for .NET applications that supports string arguments, boolean switches, and bracketed items.

## Features

- **String arguments** with long and short form support (`--key value` or `-k value`)
- **Boolean switches** for flag-based options (`--verbose`, `--help`)
- **Bracketed items** parsing for special parameters (`[item1] [item2]`)
- **Simple API** with minimal configuration required
- **Lightweight** - single class implementation with no external dependencies

## Quick Start

### Basic Usage

```csharp
using StarKNET.CommandLine;

static void Main(string[] args)
{
    var parser = new CommandLineParser(args);
    
    // Parse string arguments (supports both long and short forms)
    string inputFile = parser.GetStringArgument("input", 'i');
    string outputFile = parser.GetStringArgument("output", 'o');
    
    // Parse boolean switches
    bool verbose = parser.GetSwitchArgument("verbose");
    bool help = parser.GetSwitchArgument("help");
    
    // Access bracketed items
    foreach (string item in parser.Items)
    {
        Console.WriteLine($"Processing item: {item}");
    }
}
```

### Example Command Line

```bash
myapp.exe --input data.txt -o results.txt --verbose [config1] [config2]
```

This would parse as:
- `inputFile` = "data.txt"
- `outputFile` = "results.txt" 
- `verbose` = true
- `Items` = ["config1", "config2"]

## Command Line Formats Supported

### String Arguments

**Long form:**
```bash
--key value
--input myfile.txt
--output /path/to/file
```

**Short form:**
```bash
-k value
-i myfile.txt
-o /path/to/file
```

### Boolean Switches

```bash
--verbose
--help
--debug
--force
```

### Bracketed Items

```bash
[item1]
[configuration]
[profile-name]
```

## API Reference

### CommandLineParser Class

#### Constructor

```csharp
CommandLineParser(string[] args)
```

Creates a new parser instance with the provided command line arguments.

#### Properties

```csharp
List<string> Items { get; }
```

Contains all bracketed items found in the command line arguments (brackets removed).

#### Methods

```csharp
string GetStringArgument(string key, char shortKey)
```

Retrieves a string value for the specified argument key. Supports both long form (`--key`) and short form (`-k`).

**Parameters:**
- `key` - The long form key name (without dashes)
- `shortKey` - The short form character (without dash)

**Returns:** The argument value as string, or `null` if not found.

```csharp
bool GetSwitchArgument(string value)
```

Checks if a boolean switch is present in the command line arguments.

**Parameters:**
- `value` - The switch name (without dashes)

**Returns:** `true` if the switch is present, `false` otherwise.

## Usage Examples

### File Processing Application

```csharp
static void Main(string[] args)
{
    var parser = new CommandLineParser(args);
    
    string input = parser.GetStringArgument("input", 'i');
    string output = parser.GetStringArgument("output", 'o');
    bool recursive = parser.GetSwitchArgument("recursive");
    bool verbose = parser.GetSwitchArgument("verbose");
    
    if (string.IsNullOrEmpty(input))
    {
        Console.WriteLine("Error: Input file required. Use --input or -i");
        return;
    }
    
    if (verbose)
    {
        Console.WriteLine($"Processing: {input}");
        Console.WriteLine($"Output to: {output ?? "console"}");
        Console.WriteLine($"Recursive: {recursive}");
    }
    
    // Process files...
}
```

**Command line:**
```bash
fileprocessor.exe --input data.csv -o results.json --recursive --verbose
```

### Configuration Manager

```csharp
static void Main(string[] args)
{
    var parser = new CommandLineParser(args);
    
    string environment = parser.GetStringArgument("environment", 'e');
    bool dryRun = parser.GetSwitchArgument("dry-run");
    
    // Process each configuration profile
    foreach (string profile in parser.Items)
    {
        Console.WriteLine($"Applying profile: {profile}");
        if (!dryRun)
        {
            ApplyConfiguration(profile, environment);
        }
    }
}
```

**Command line:**
```bash
configmgr.exe -e production --dry-run [web-config] [database-config] [cache-config]
```

### Build Tool Example

```csharp
static void Main(string[] args)
{
    var parser = new CommandLineParser(args);
    
    string configuration = parser.GetStringArgument("configuration", 'c') ?? "Debug";
    string platform = parser.GetStringArgument("platform", 'p') ?? "AnyCPU";
    bool clean = parser.GetSwitchArgument("clean");
    bool rebuild = parser.GetSwitchArgument("rebuild");
    
    if (clean)
    {
        Console.WriteLine("Cleaning previous build...");
    }
    
    Console.WriteLine($"Building in {configuration} mode for {platform}");
    
    // Process project files from bracketed items
    var projects = parser.Items.Any() ? parser.Items : new List<string> { "default.sln" };
    
    foreach (string project in projects)
    {
        Console.WriteLine($"Building project: {project}");
    }
}
```

**Command line:**
```bash
builder.exe --configuration Release -p x64 --clean [ProjectA.csproj] [ProjectB.csproj]
```

## Argument Parsing Rules

### String Arguments
- Must be followed by a value
- Long form: `--key value`
- Short form: `-k value`
- If the same key appears multiple times, the last value wins

### Boolean Switches
- Presence indicates `true`, absence indicates `false`
- Only long form supported: `--switch`
- No value required or expected

### Bracketed Items
- Must be enclosed in square brackets: `[item]`
- Brackets are automatically removed from the parsed items
- Multiple bracketed items are supported
- Order is preserved in the `Items` collection

## Best Practices

### Argument Validation

```csharp
var parser = new CommandLineParser(args);

string input = parser.GetStringArgument("input", 'i');
if (string.IsNullOrEmpty(input))
{
    Console.WriteLine("Error: --input argument is required");
    ShowHelp();
    return 1;
}
```

### Help Implementation

```csharp
static void Main(string[] args)
{
    var parser = new CommandLineParser(args);
    
    if (parser.GetSwitchArgument("help") || args.Length == 0)
    {
        ShowHelp();
        return;
    }
    
    // Continue with normal processing...
}

static void ShowHelp()
{
    Console.WriteLine("Usage: myapp.exe [options] [items...]");
    Console.WriteLine();
    Console.WriteLine("Options:");
    Console.WriteLine("  -i, --input <file>    Input file path");
    Console.WriteLine("  -o, --output <file>   Output file path");
    Console.WriteLine("  --verbose             Enable verbose output");
    Console.WriteLine("  --help                Show this help");
    Console.WriteLine();
    Console.WriteLine("Items:");
    Console.WriteLine("  [item]                Bracketed configuration items");
}
```

### Default Values

```csharp
string logLevel = parser.GetStringArgument("log-level", 'l') ?? "Info";
string output = parser.GetStringArgument("output", 'o') ?? "output.txt";
bool force = parser.GetSwitchArgument("force"); // defaults to false
```

## Limitations

- **No argument validation** - the parser doesn't validate argument formats or required arguments
- **No type conversion** - all string arguments are returned as strings
- **No value validation** - doesn't check if string arguments have associated values
- **Simple parsing logic** - doesn't handle complex scenarios like quoted strings with spaces
- **No help generation** - doesn't automatically generate help text

## Requirements

- .NET Framework 4.0+ or .NET Core 1.0+
- No external dependencies

## Thread Safety

The parser is read-only after construction and is thread-safe for reading operations.

## License

This project is part of the StarKNET framework.
