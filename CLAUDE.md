# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

DS Pokemon ROM Editor (DSPRE) Reloaded is a C# Windows Forms application for editing Nintendo DS Pokemon ROM files. This is a major overhaul of the original DSPRE by Nomura with significant new features, performance improvements, and bug fixes. The editor supports multiple Pokemon games: Diamond/Pearl/Platinum (DPPt), and HeartGold/SoulSilver (HGSS).

## Build Commands

This is a .NET Framework 4.8 Windows Forms application.

### Build the application:
```bash
# From Visual Studio: Build > Build Solution (Ctrl+Shift+B)
# Or from command line:
msbuild DS_Map.sln /t:Build /p:Configuration=Release
```

### Build configurations:
- **Debug**: `bin\Debug\` - Full debug symbols, no optimization
- **Release**: `bin\Release\` - Optimized, no debug info

### Run the application:
```bash
# From Visual Studio: Debug > Start Debugging (F5)
# Or directly:
.\DS_Map\bin\Debug\DSPRE.exe
```

## Solution Structure

The solution consists of three main projects:

1. **DSPRE.csproj** (`DS_Map\`) - Main Windows Forms application
2. **Ekona.csproj** (`Ekona\`) - Image/sprite processing library with plugin architecture
3. **Images.csproj** (`Images\Images\`) - Nintendo DS image format handlers (NCGR, NCER, NCLR, etc.)

## Architecture

### Core Components

#### ROM File System (`DS_Map\Filesystem.cs`, `DS_Map\Narc.cs`)
- **Filesystem**: Static utility class for ROM file operations, NARC packing/unpacking
- **Narc**: Handles Nitro Archive (NARC) files - the standard Nintendo DS archive format
- All ROM data is extracted to/from NARC archives for editing

#### ROM Data Model (`DS_Map\ROMFiles\`)
All ROM data structures inherit from the abstract `RomFile` base class with serialization methods:

- **MapFile**: Complete map data (collisions, permissions, buildings, terrain, BGS)
- **MapHeader**: Map metadata and properties
- **EventFile**: Map events (spawns, warps, triggers, overworlds)
- **ScriptFile**: Level scripts and scripting commands
- **EncounterFile**: Wild Pokemon encounters
- **TrainerFile**: Trainer data with party and movesets
- **TextArchive**: In-game text strings
- **GameMatrix**: Area matrix layout
- **Building**: 3D building objects with position/rotation/scale

Each ROM data type implements `ToByteArray()` for binary serialization back to ROM format.

#### ROM Version Management (`DS_Map\RomInfo.cs`)
- **GameVersions**: Enum of individual game versions (DP, Pt, HGSS, etc.)
- **GameFamilies**: Groups of related games
- **DirNames**: Directory mapping for different ROM sections
- **gameDirs**: Static dictionary mapping sections to file paths per game

#### 3D Graphics System (`DS_Map\LibNDSFormats\`)
Handles Nintendo DS 3D formats:

- **NSBMD** (`NSBMD\`): Nitro Polygon Model format
  - `NSBMDLoader`: Parsing and loading
  - `NSBMDGlRenderer`: OpenGL-based rendering
  - `MTX44`: 4x4 matrix transformations (column-major)
- **NSBTX** (`NSBTX\`): Nitro Texture format with palette management
- **NSBTA/NSBTP**: Animation formats (skeletal and texture pattern)
- **OBJWriter**: Export to Wavefront OBJ format

#### Editor Framework (`DS_Map\Editors\`)
Editors follow two patterns:
- **UserControl editors**: Embedded in main MDI window (MapEditor, HeaderEditor, ScriptEditor, etc.)
- **Form editors**: Standalone windows (BuildingEditor, ItemEditor, PokemonEditor, etc.)

Key editors:
- **MapEditor**: 3D map visualization with OpenGL, collision editing, building placement
- **ScriptEditor**: Syntax-highlighted script editing using ScintillaNET
- **EventEditor**: Event placement with mouse support and navigation
- **HeaderEditor**: Map header properties with copy/paste support
- **MatrixEditor**: Area matrix editing with visual grid
- **EncountersEditor**: Wild Pokemon encounter editing
- **TrainerEditor**: Complete trainer data editing
- **TextEditor**: In-game text editing with search/replace

#### Main Application (`DS_Map\Main Window.cs`)
- **MainProgram**: MDI (Multiple Document Interface) main window
- Manages ROM project loading, editor lifecycle, and user preferences
- Uses Velopack for automatic updates
- Settings persisted via `App.config`

### Architectural Patterns

1. **Abstract Base Class Pattern**: `RomFile` base class for all ROM data types
2. **Static Helpers**: `Helpers.cs` (rendering, UI, ROM operations), `Filesystem.cs` (ROM I/O)
3. **Stream-Based I/O**: Heavy use of `MemoryStream`, `BinaryReader`/`BinaryWriter`, `EndianBinaryReader`
4. **Plugin Architecture**: `IPlugin`, `IGamePlugin` interfaces in Ekona library
5. **OpenGL Rendering**: 3D visualization via Tao.OpenGl and HelixToolkit

### External Dependencies

Key NuGet packages:
- **ScintillaNET**: Syntax-highlighted code editor for scripts
- **Velopack**: Application update framework
- **OpenTK, Tao.OpenGl, HelixToolkit**: 3D graphics rendering
- **Microsoft.WindowsAPICodePack**: Windows integration
- **LibGit2Sharp**: Git integration

### File Locations

DSPRE uses specific directory structures within ROM files:
- Map data: NARC files in `/fielddata/land_data/`
- Scripts: `/fielddata/script/`
- Events: `/fielddata/eventdata/`
- Encounters: `/fielddata/encountdata/`
- Text: `/msgdata/`
- Graphics: `/data/` (NSBMD, NSBTX files)

Game-specific paths are defined in `RomInfo.gameDirs`.

## Development Guidelines

### Code Style and Formatting

**IMPORTANT: Avoid Useless Comments**
- Do NOT write comments that simply restate what the code does
- Comments should only explain "why" the code exists, not "what" it does
- Only add comments when there's a non-obvious reason, tricky logic, or important context

### ROM File Editing Pattern
When editing ROM data:
1. Load ROM project (unpacks to working directory)
2. Open NARC archives using `Narc.Open()`
3. Parse binary data into data structures (e.g., `MapFile.FromByteArray()`)
4. Modify data in memory
5. Serialize back using `ToByteArray()`
6. Save NARC using `narc.Save()`
7. Save entire ROM project to repack into ROM file

### 3D Rendering Conventions
- Uses column-major matrices (`MTX44`)
- OpenGL coordinate system (right-handed)
- Separate rendering paths for textured vs untextured models
- Camera position managed by `GameCamera` class

### Binary Format Handling
- Nintendo DS uses little-endian architecture (use `EndianBinaryReader` for big-endian sections)
- NARC format: BTAF (File Allocation Table), BTNF (Name Table), GMIF (File Image)
- Many formats use magic numbers for identification (e.g., "NSBMD", "NSBTX", "NARC")

### Editor State Management
- Editors use event handlers to disable/enable during bulk operations
- State saved via `SettingsManager` to `App.config`
- User preferences include UI layout, rendering toggles, export paths

### Script Command System
- Script commands defined in `Resources\ScriptDatabase.cs`
- Version-specific commands in `Resources\ScriptsV\`
- Parameters parsed using `ScriptParameter` class
- Commands can have variable-length parameters

### Error Handling
- Use structured exception handling with user-friendly messages
- `CrashReporter.cs` logs errors
- `AppLogger.cs` for application logging
- `correctnessFlag` in data structures tracks integrity

## ROM Toolbox Patches

DSPRE includes a ROM Toolbox with patches:
- Expand ARM9 usable memory
- Dynamic Cameras
- Set Overlay1 as uncompressed
- Convert Pokemon names to Sentence Case
- Standardize item numbers
- Expand matrix 0
- Dynamic Headers

Patch data stored in `Resources\ROMToolboxDB\`.

## Important Considerations

### Game Version Detection
Always check `RomInfo.gameFamily` or `RomInfo.gameVersion` as different Pokemon games have:
- Different file offsets and structures
- Different header formats
- Different script command sets
- Different encounter table layouts

### Performance
- Original DSPRE had slow load/save times - optimizations focused on streaming I/O
- Parallel processing used for ROM unpacking
- In-memory caching for frequently accessed data

### Unsafe Code
The project uses `AllowUnsafeBlocks=true` for performance-critical binary operations.

### Tools Directory
External tools in `DS_Map\Tools\`:
- **apicula.exe**: DAE export support
- **blz.exe**: Compression utilities
- **ndstool.exe**: ROM manipulation
- **charmap.xml**: Character encoding map

## Application Configuration

Key settings in `DS_Map\App.config`:
- `menuLayout`: UI layout preference
- `lastColorTablePath`: User's palette path
- `textEditorPreferHex`: Text format preference
- `scriptEditorFormatPreference`: Script display format
- `renderSpawnables`, `renderOverworlds`, `renderWarps`, `renderTriggers`: Event rendering toggles
- `exportPath`, `mapImportStarterPoint`: Import/export paths
- `openDefaultRom`: ROM opening behavior
- `databasesPulled`: Online database sync status

## Key File Paths

- Entry point: `DS_Map\Program.cs`
- Main window: `DS_Map\Main Window.cs`
- ROM file base: `DS_Map\ROMFiles\RomFile.cs`
- File system: `DS_Map\Filesystem.cs`
- NARC handler: `DS_Map\Narc.cs`
- ROM info: `DS_Map\RomInfo.cs`
- Helpers: `DS_Map\Helpers.cs`

## Notes on Codebase Evolution

This is a major overhaul with significant improvements:
- Faster load/save times
- New Script Editor with syntax highlighting
- Fixed Japanese DP ROM support
- Configurable UI layouts
- User preference memorization
- Advanced header search
- New NSBMD/NSBTX utilities
- NARC packer/unpacker
- Batch rename utilities
- ARM9 mismatch warnings
- ALT key shortcuts
- ROM Toolbox with patches
