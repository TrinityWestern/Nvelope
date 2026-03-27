# Copilot Coding Agent Instructions for Nvelope

## Project Overview

**Nvelope** is a C# utility library ("Batman utility belt") published as a NuGet package by Trinity Western University. It provides a large collection of extension methods and utility classes to make C# development faster and more ergonomic. The current published version is **1.1.0.2**.

The library targets **two .NET Framework versions** in parallel:
- `.NET Framework 4.6.1` — primary build (`src/Nvelope/`)
- `.NET Framework 4.0` — legacy compatibility build (`src/Nvelope-net4/`)

**There is no .NET Core or modern .NET support.** This is a classic .NET Framework library.

---

## Repository Structure

```
Nvelope/
├── .github/                        # GitHub configuration
│   └── copilot-instructions.md     # This file
├── src/
│   ├── Nvelope/                    # Primary source project (.NET 4.6.1)
│   │   ├── Nvelope.csproj
│   │   ├── *.cs                    # Core extension classes
│   │   ├── Collections/            # Custom collection types
│   │   ├── Combinatorics/          # Combination/permutation utilities
│   │   ├── Configuration/          # Config helpers
│   │   ├── Data/                   # DataReader/DataProxy extensions
│   │   ├── Exceptions/             # Custom exception types
│   │   ├── IO/                     # File and process utilities
│   │   ├── Reading/                # Type-conversion / parsing
│   │   ├── Reflection/             # Reflection helpers
│   │   ├── Security/               # Encryption utilities
│   │   ├── Tabular/                # Table/cell utilities
│   │   └── Web/                    # HTTP/HTML/URI extensions
│   └── Nvelope-net4/               # .NET 4.0 build (shares same source via linked files)
├── test/
│   └── Nvelope.Tests/              # NUnit 2.6.3 test suite (mirrors src structure)
├── packages/                       # NuGet packages (checked-in)
├── .nuget/                         # NuGet configuration
├── Nvelope.sln                     # Visual Studio solution
├── Nvelope.nunit                   # NUnit project file
├── Nvelope.nuspec                  # NuGet package specification
├── rakefile.rb                     # Rake-based build automation
├── README.md
├── Changelog.md
└── LICENSE.txt
```

---

## Build System

### ⚠️ Windows-Only Build Environment

The build system uses **Ruby Rake + MSBuild** and is designed for Windows. The `rakefile.rb` relies on MSBuild, Windows paths (`\`), and optional tools like NUnit console runner and NCover.

**Building with Rake (full build pipeline):**
```bash
rake build       # Compiles and places artifacts in workspace\artifacts\
rake test        # Runs NUnit tests; output to workspace\reports\tests.xml
rake coverage    # Runs NCover coverage; output to workspace\reports\ncover\
```

**Building with MSBuild directly:**
```bash
msbuild /p:Configuration=Release;DebugSymbols=false;OutDir=bin\ src\Nvelope\Nvelope.csproj
msbuild /p:Configuration=Release;DebugSymbols=false;OutDir=bin\ src\Nvelope-net4\Nvelope-net4.csproj
```

**Building with Visual Studio:**
- Open `Nvelope.sln` (requires Visual Studio 2012 or later, ToolsVersion 12.0)

Build outputs:
- `src/Nvelope/bin/Release/Nvelope.dll`
- `src/Nvelope/bin/Release/Nvelope.xml` (XML documentation)
- `src/Nvelope-net4/bin/Release/Nvelope.dll`

### NuGet Package
```bash
nuget pack Nvelope.nuspec
```
The package targets `lib\net451\` and `lib\net40\`.

---

## Testing

**Framework:** NUnit 2.6.3 (very old but still functional)

**Running tests:**
```bash
# Via Rake (recommended)
rake test

# Via NUnit console runner directly
nunit-console.exe test\Nvelope.Tests\bin\Release\Nvelope.Tests.dll

# Output goes to workspace\reports\tests.xml
```

**Test structure:**
- Tests mirror the source structure under `test/Nvelope.Tests/`
- All test classes use standard NUnit attributes: `[TestFixture]`, `[Test]`, `[SetUp]`, `[TearDown]`
- Assertions use `NUnit.Framework.Assert.*`

**Writing new tests:**
- Add test files in `test/Nvelope.Tests/` mirroring the source path
- Use `[TestFixture]` on the class, `[Test]` on each test method
- Follow the naming pattern `<ClassName>Tests.cs`
- No mocking framework is used; tests exercise the extension methods directly

---

## Code Style and Conventions

### C# Conventions
- **Namespaces**: `Nvelope` (root) and `Nvelope.<SubModule>` for specialized types
- **Extension classes**: Named `<Type>Extensions` (e.g., `StringExtensions`, `ListExtensions`)
- **PascalCase** for all public types and members
- **camelCase** for parameters and local variables
- **4-space indentation** (no tabs)

### Code Analysis
- **StyleCop** is enabled in Debug builds (via `CODE_ANALYSIS` define)
- **FxCop / Code Analysis** is enabled in Release builds
- Intentional violations are suppressed with `[SuppressMessage("...", "...", Justification="...")]`
- XML documentation comments (`/// <summary>`) are required in Release builds

### Extension Method Pattern
All extension methods follow this pattern:
```csharp
using System.Diagnostics.CodeAnalysis;

namespace Nvelope
{
    public static class SomeTypeExtensions
    {
        /// <summary>Description of method</summary>
        /// <param name="source">The source object</param>
        /// <returns>Description of return value</returns>
        public static ReturnType MethodName(this SomeType source, ...)
        {
            // implementation
        }
    }
}
```

### Assembly Attributes
The assembly uses strong naming (`keypair.snk`) and is CLS compliant:
```csharp
[assembly: CLSCompliant(true)]
[assembly: ComVisible(false)]
```

---

## Key Modules and Classes

### Root namespace (`Nvelope`)
| File | Description |
|------|-------------|
| `StringExtensions.cs` | 50+ string methods (splitting, casing, formatting, sentence manipulation) |
| `ListExtensions.cs` | LINQ-like list operations (Push, Pop, Shift, None, Prepend, etc.) |
| `DictionaryExtensions.cs` | Merge, inversion, printing, defaulting |
| `NumberExtensions.cs` | IsEven/IsOdd, LesserOf/GreaterOf, range operations |
| `DateTimeExtensions.cs` | Month parsing, interval calculations |
| `ObjectExtensions.cs` | Generic object utilities |
| `ConversionExtensions.cs` | Type conversion helpers |
| `Fn.cs` | Functional programming: `Thread<T>()`, `Curry<>()` |
| `EnumExtras.cs` | Enum parsing and utilities |
| `PhoneNumber.cs` | Phone number parsing and formatting |
| `Interval.cs` / `IntervalExtensions.cs` | Date/sequence range handling |
| `English.cs` | English language utilities (pluralization, articles) |
| `RegexExtensions.cs` | Regex helper extensions |
| `SerializationExtensions.cs` | Serialization helpers |
| `TypeExtensions.cs` | Type reflection utilities |

### Collections (`Nvelope.Collections`)
| Class | Description |
|-------|-------------|
| `Range<T>` | Ordered sequence range |
| `FixedSizeQueue<T>` | Bounded queue |
| `DictionaryMirror` | Bidirectional dictionary |
| `DirectedAcyclicGraph` | DAG implementation |
| `SDictionary` | Specialized dictionary |
| `OrderedSequential<T>` | Base class for comparable sequences |

### IO (`Nvelope.IO`)
| Class | Description |
|-------|-------------|
| `TextFile` | File read/write helpers |
| `FileUtils` | File operation utilities |
| `Folder` | Directory helpers |
| `Processes` | External process execution |
| `CommandParser` / `CommandInterpreter` | Command-line parsing |

### Reading (`Nvelope.Reading`)
| Class | Description |
|-------|-------------|
| `Read` | Parse dictionaries, lists, and enums from strings |
| `TypeConversion` | Type-safe conversion utilities |

### Security (`Nvelope.Security`)
| Class | Description |
|-------|-------------|
| `PasswordCryptor` | Password hashing/verification |
| `CertificateCryptor` | X.509 certificate-based encryption |
| `PassthroughCryptor` | No-op cryptor (for testing) |

### Data (`Nvelope.Data`)
| Class | Description |
|-------|-------------|
| `DataProxy` | Dynamic data access proxy |
| `IDataReaderExtensions` | DataReader extension methods |

---

## Known Issues and Quirks

### Breaking Changes (v1.1)
1. **DateTime parsing** now defaults to en-US locale (m/d/y) when ambiguous. Previously used local settings (defaulted to en-CA).
2. **Phone number parsing** behavior changed in edge cases.
3. **Enum parsing** is stricter — converting an int to an int-based Enum throws if the value is not defined.

### Environment Quirks
- The build system is **Windows-only** (paths use `\`, requires MSBuild and NUnit console runner on PATH)
- The `rakefile.rb` references Cygwin-style paths for optional `rsync` deployment
- The `WORKSPACE` environment variable controls where build outputs go (defaults to `./workspace/`)
- **No modern `dotnet` CLI support** — cannot use `dotnet build` or `dotnet test`
- **NUnit 2.6.3** is very old; some modern NUnit patterns will not work
- The `packages/` directory is checked into source control (old NuGet workflow)
- The assembly is strongly signed — changes require access to `keypair.snk`

### StringExtensions Quirk
`StringExtensions.Print()` has special handling to avoid accidentally dispatching to the `IEnumerable<char>` version of `Print()`. Be aware of this when adding overloads to string-related methods.

### Dual Project Builds
`src/Nvelope-net4/` links the same source files as `src/Nvelope/` but targets .NET 4.0. Any source changes automatically apply to both targets. Do not add files only to one project unless there's a specific framework-version reason.

---

## Dependencies

### Runtime Dependencies
The library has **no external runtime NuGet dependencies**. It only references standard .NET Framework assemblies:
- `System`, `System.Core`, `System.Configuration`
- `System.Data`, `System.Data.DataSetExtensions`
- `System.Drawing`, `System.Web`
- `System.Xml`, `System.Xml.Linq`

### Test Dependencies
- **NUnit 2.6.3** (in `packages/NUnit.2.6.3/`)

### Build Tools (must be on PATH)
- MSBuild (comes with Visual Studio)
- `nunit-console.exe` (for running tests)
- `ncover.console.exe` (optional, for coverage)
- Ruby + Rake (for the rake build system)

---

## Development Workflow

1. **Explore** the relevant source file in `src/Nvelope/` (or its subdirectory)
2. **Find the corresponding test file** in `test/Nvelope.Tests/` (same subdirectory structure)
3. **Make changes** following the extension method and XML documentation conventions
4. **Build**: `msbuild /p:Configuration=Release src\Nvelope\Nvelope.csproj` (or via Rake)
5. **Test**: `nunit-console.exe test\Nvelope.Tests\bin\Release\Nvelope.Tests.dll` (or `rake test`)
6. **Both projects**: Remember `src/Nvelope-net4/` links the same source files — verify compatibility if using .NET 4.6.1-specific APIs

### Adding a New Extension Method
1. Find or create the appropriate `*Extensions.cs` file in `src/Nvelope/`
2. Add the static method with full XML documentation comment
3. Add corresponding test in `test/Nvelope.Tests/`
4. Do NOT add external NuGet dependencies

### Suppressing Code Analysis Warnings
When a FxCop/StyleCop warning is intentional:
```csharp
[SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix",
    Justification = "Reason why this is intentional")]
```
