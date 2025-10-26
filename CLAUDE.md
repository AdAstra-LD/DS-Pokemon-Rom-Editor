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
- **ScriptFile**: Script commands and scripting data (supports both binary and plaintext formats)
- **LevelScriptFile**: Level scripts (trigger-based scripts for map events)
- **EncounterFile**: Wild Pokemon encounters
- **TrainerFile**: Trainer data with party and movesets
- **TradeData**: In-game trade Pokemon with IVs, natures, items
- **SafariZoneEncounterFile**: Safari Zone encounter data
- **HeadbuttEncounterFile**: Headbutt tree encounters (HGSS)
- **TextArchive**: In-game text strings
- **GameMatrix**: Area matrix layout
- **AreaData**: Area type and terrain information
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
- **NSBCA/NSBTA/NSBTP**: Animation formats (skeletal, texture, and texture pattern)
- **NSBUtils**: Utilities for merging models with textures, extracting textures
- **OBJWriter**: Export to Wavefront OBJ format
- **ModelUtils**: Export to DAE (via Apicula) and GLB formats

#### Script System (Major Feature - New Plaintext Support)

**Script Files and Formats** (`DS_Map\ROMFiles\ScriptFile.cs`):
- **Binary Format**: Original ROM format stored in NARC archives at `/fielddata/script/`
- **Plaintext Format**: NEW - Human-readable `.script` files exported to `expanded/scripts/` directory
- **Dual Representation**: Scripts maintain both binary (for ROM) and plaintext (for editing) versions
- **Automatic Sync**: Binary files automatically rebuilt from plaintext when plaintext is newer

**Plaintext Script Format**:
```
//===== SCRIPTS =====//
Script 1:
    Command1 param1 param2
    Command2 param1
Script 2:
    UseScript_#1

//===== FUNCTIONS =====//
Function 1:
    Command1 param1

//===== ACTIONS =====//
Action 1:
    Movement1
```

**Script File Structure**:
- Three sections: Scripts, Functions, Actions
- Each section can contain multiple numbered containers
- Commands within containers are indented
- UseScript references allow code reuse between scripts

**Script Database System** (`DS_Map\Resources\ScriptDatabase.cs`):
- **JSON-Based**: Script commands loaded from JSON database files
- **Version-Specific**: Separate command databases for Diamond/Pearl, Platinum, and HeartGold/SoulSilver
- **Custom Databases**: Users can load custom script command databases for ROM hacks
- **Database Hashing**: MD5 hash tracking detects database changes and triggers automatic re-export
- **Reference Data**: Built-in dictionaries for Pokemon, items, moves, sounds, trainers
- **Command Metadata**: Each command includes ID, name, parameter types, parameter names, descriptions

**Script Commands** (`DS_Map\ROMFiles\ScriptCommand.cs`, `DS_Map\Script\ScriptParameter.cs`):
- **ScriptCommand**: Represents individual script commands with ID and parameters
- **ScriptCommandContainer**: Groups related commands into scripts or functions
- **ScriptActionContainer**: Groups movement/action commands
- **Parameter Types**: 15+ types including Integer, Variable, Pokemon, Item, Move, Sound, Trainer, etc.
- **Smart Formatting**: Parameters displayed with friendly names (e.g., "Pikachu" instead of "25")

**Custom Database Management** (`DS_Map\Resources\CustomScrcmdManager.cs`):
- **CustomScrcmdManager**: Form for managing custom script databases
- **Auto-Detection**: Scans scripts on load and prompts user to load custom database if invalid commands found
- **Database Storage**: Custom databases stored in `edited_databases/` with naming: `{romname}_scrcmd_database.json`
- **Reparse Support**: Can reload database and reparse all scripts with progress tracking
- **Import/Export**: Share custom databases between users

**Database Hashing and Change Detection**:
- **Hash File**: `.database_hash` marker file in `expanded/scripts/` directory stores MD5 hash
- **Automatic Detection**: On editor load, compares current database hash against stored hash
- **Auto Re-export**: If database changed, automatically deletes and rebuilds all plaintext scripts
- **Prevents Corruption**: Ensures scripts and database are always in sync

**Plaintext Caching**:
- **Performance Optimization**: Dictionary cache stores parsed plaintext scripts with timestamps
- **Avoids Re-parsing**: During batch operations (like search), uses cache instead of re-reading files
- **Cache Invalidation**: Timestamps validate whether cached version is still current

**VS Code Integration**:
- **External Editing**: "Open in VSCode" button launches Visual Studio Code
- **Command**: `code "{scriptsFolder}" "{txtPath}"` opens both folder and specific file
- **Timestamp-based Sync**: On script load and ROM save, DSPRE checks if plaintext files are newer than binary and rebuilds if needed
- **Bidirectional Sync**: Changes in VSCode reflected in DSPRE on next load/save, changes in DSPRE reflected in plaintext files on save

**Script Export/Import Workflow**:
1. **Initial Load**: On first ROM open, all binary scripts exported to plaintext in `expanded/scripts/`
2. **Selective Export**: Existing plaintext files preserved (not overwritten) to maintain user edits
3. **External Editing**: User can edit `.script` files in VSCode or any text editor
4. **Auto-Rebuild**: On ROM save, DSPRE scans for plaintext files newer than binary and rebuilds them
5. **Binary Update**: Rebuilt binary scripts packed back into ROM NARC archive

**Progress Tracking** (`DS_Map\Editors\Utils\LoadingForm.cs`):
- **LoadingForm**: Progress bar dialog for long-running script operations
- **Pokemon Facts**: Displays random Pokemon facts during loading to entertain users
- **Thread-Safe Updates**: Real-time progress updates via Invoke pattern
- **Used For**: Initial script export, database reparsing, batch operations

#### Editor Framework (`DS_Map\Editors\`)
Editors follow two patterns:
- **UserControl editors**: Embedded in main MDI window (MapEditor, HeaderEditor, ScriptEditor, etc.)
- **Form editors**: Standalone windows (BuildingEditor, ItemEditor, PokemonEditor, etc.)

Key editors:
- **MapEditor**: 3D map visualization with OpenGL, collision editing, building placement
- **ScriptEditor**: Syntax-highlighted script editing using ScintillaNET with plaintext export, VSCode integration, custom database support
- **LevelScriptEditor**: Level script trigger management (separate from regular scripts)
- **EventEditor**: Event placement with mouse support, navigation, and sprite rendering
- **HeaderEditor**: Map header properties with copy/paste support
- **MatrixEditor**: Area matrix editing with visual grid
- **EncountersEditor**: Wild Pokemon encounter editing
- **TrainerEditor**: Complete trainer data editing with party, movesets, and AI flags
- **TextEditor**: In-game text editing with search/replace
- **SafariZoneEditor**: Safari Zone encounter management
- **HeadbuttEncounterEditor**: Headbutt tree encounter editing (HGSS)
- **TradeEditor**: In-game trade Pokemon editor with IV/nature/item support
- **EvolutionsEditor**: Evolution method and trigger editing
- **LearnsetEditor**: Move learning method editor
- **BtxEditor**: Dedicated NSBTX texture/palette editor with confirmation dialogs

#### Main Application (`DS_Map\Main Window.cs`)
- **MainProgram**: MDI (Multiple Document Interface) main window
- Manages ROM project loading, editor lifecycle, and user preferences
- Uses Velopack for automatic updates
- Settings persisted via `SettingsManager` and `App.config`
- Recent project reopening with confirmation dialog
- Version label display showing game version and ROM ID

### Architectural Patterns

1. **Abstract Base Class Pattern**: `RomFile` base class for all ROM data types
2. **Static Helpers**: `Helpers.cs` (rendering, UI, ROM operations), `Filesystem.cs` (ROM I/O)
3. **Stream-Based I/O**: Heavy use of `MemoryStream`, `BinaryReader`/`BinaryWriter`, `EndianBinaryReader`
4. **Plugin Architecture**: `IPlugin`, `IGamePlugin` interfaces in Ekona library
5. **OpenGL Rendering**: 3D visualization via Tao.OpenGl and HelixToolkit
6. **Dual File Format**: Binary (ROM) + Plaintext (editing) for script files
7. **Caching with Validation**: Timestamp-based cache invalidation for performance

### External Dependencies

Key NuGet packages:
- **ScintillaNET**: Syntax-highlighted code editor for scripts
- **Velopack**: Application update framework
- **OpenTK, Tao.OpenGl, HelixToolkit**: 3D graphics rendering
- **Microsoft.WindowsAPICodePack**: Windows integration
- **LibGit2Sharp**: Git integration
- **System.Text.Json**: JSON serialization for settings and databases

### File Locations

DSPRE uses specific directory structures within ROM files:
- Map data: NARC files in `/fielddata/land_data/`
- Scripts (binary): `/fielddata/script/` (NARC archive)
- Scripts (plaintext): `expanded/scripts/` (working directory, `.script` files)
- Events: `/fielddata/eventdata/`
- Encounters: `/fielddata/encountdata/`
- Text: `/msgdata/`
- Graphics: `/data/` (NSBMD, NSBTX files)

DSPRE user data directories:
- Database path: `Program.DatabasePath` (typically `~/.dspre/databases/`)
- Custom databases: `edited_databases/` subdirectory
- Database hash marker: `expanded/scripts/.database_hash`

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

### Script Editing Pattern (NEW)
When working with scripts:
1. **Loading**: ScriptFile automatically checks for plaintext version via `TryReadPlaintextIfNewer()`
2. **Editing**: User can edit in ScriptEditor (Scintilla) or external editor (VSCode)
3. **Plaintext Export**: First load exports all scripts to `expanded/scripts/{ID:D4}.script`
4. **External Changes**: DSPRE detects when plaintext files are newer than binary
5. **Rebuilding**: `RebuildBinaryScriptsFromPlaintext()` converts plaintext back to binary on save
6. **Database Changes**: Hash comparison triggers automatic re-export when database changes

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
- **Primary Database**: `Resources\ScriptDatabase.cs` with JSON loader
- **Custom Databases**: User-provided JSON files in `edited_databases/` directory
- **Version-Specific**: Different command sets for DP, Platinum, HGSS
- **Parameters**: Parsed using `ScriptParameter` class with 15+ parameter types
- **Smart Display**: Friendly names for Pokemon, items, moves, etc. from reference dictionaries
- **Variable Length**: Commands can have variable-length parameters

### Error Handling
- Use structured exception handling with user-friendly messages
- `CrashReporter.cs` logs errors
- `AppLogger.cs` for application logging
- `correctnessFlag` in data structures tracks integrity
- **Script Validation**: Invalid commands detected on load with detailed error messages
- **Database Prompts**: User prompted to load custom database when invalid commands found

## ROM Toolbox Patches

DSPRE includes a ROM Toolbox with patches:
- **ARM9 Expansion**: Expand ARM9 usable memory
- **Dynamic Cameras**: BDH camera patch for dynamic positioning
- **Overlay Management**: Set Overlay1 as uncompressed
- **Pokemon Names**: Convert Pokemon names to Sentence Case
- **Item Standardization**: Standardize item numbers across games
- **Matrix Expansion**: Expand matrix 0 for larger areas
- **Dynamic Headers**: Extended header functionality
- **Script Command Repointing**: Support for custom script databases
- **Trainer Name Expansion**: Extended trainer name length
- **Texture Animation Killswitch**: Disable texture animation patches

Patch data stored in `Resources\ROMToolboxDB\`.

## Important Considerations

### Game Version Detection
Always check `RomInfo.gameFamily` or `RomInfo.gameVersion` as different Pokemon games have:
- Different file offsets and structures
- Different header formats
- Different script command sets (DP vs Pt vs HGSS)
- Different encounter table layouts
- Different event structures

### Performance
- Original DSPRE had slow load/save times - optimizations focused on streaming I/O
- Parallel processing used for ROM unpacking
- In-memory caching for frequently accessed data
- **Script Caching**: Plaintext scripts cached with timestamps to avoid re-parsing
- **Selective Export**: Only re-export scripts when database hash changes
- **Lazy Loading**: Plaintext only read if newer than binary

### Unsafe Code
The project uses `AllowUnsafeBlocks=true` for performance-critical binary operations.

### Tools Directory
External tools in `DS_Map\Tools\`:
- **apicula.exe**: DAE export support
- **blz.exe**: Compression utilities
- **ndstool.exe**: ROM manipulation
- **charmap.xml**: Character encoding map
- **pokefacts.txt**: Pokemon facts for loading screens (optional)

## Application Configuration

Key settings in `DS_Map\App.config` and `SettingsManager`:
- `menuLayout`: UI layout preference
- `lastColorTablePath`: User's palette path
- `textEditorPreferHex`: Text format preference
- `scriptEditorFormatPreference`: Script display format (binary or plaintext)
- `useDecompNames`: Option to use decompilation project names
- `automaticallyUpdateDBs`: Auto-sync online databases
- `renderSpawnables`, `renderOverworlds`, `renderWarps`, `renderTriggers`: Event rendering toggles
- `exportPath`, `mapImportStarterPoint`: Import/export paths
- `openDefaultRom`: ROM opening behavior
- `databasesPulled`: Online database sync status

## Key File Paths

### Core Application Files
- Entry point: `DS_Map\Program.cs`
- Main window: `DS_Map\Main Window.cs`
- Settings: `DS_Map\SettingsManager.cs`
- ROM file base: `DS_Map\ROMFiles\RomFile.cs`
- File system: `DS_Map\Filesystem.cs`
- NARC handler: `DS_Map\Narc.cs`
- ROM info: `DS_Map\RomInfo.cs`
- Helpers: `DS_Map\Helpers.cs`

### Script System Files (Important)
- Script file I/O: `DS_Map\ROMFiles\ScriptFile.cs`
- Script commands: `DS_Map\ROMFiles\ScriptCommand.cs`
- Script containers: `DS_Map\ROMFiles\ScriptCommandContainer.cs`
- Script parameters: `DS_Map\Script\ScriptParameter.cs`
- Script database: `DS_Map\Resources\ScriptDatabase.cs`
- Custom DB manager: `DS_Map\Resources\CustomScrcmdManager.cs`
- Script editor: `DS_Map\Editors\ScriptEditor.cs`
- Level scripts: `DS_Map\ROMFiles\LevelScriptFile.cs`
- Level script editor: `DS_Map\Editors\LevelScriptEditor.cs`

### Utility Files
- Loading form: `DS_Map\Editors\Utils\LoadingForm.cs`
- ARM9 tools: `DS_Map\DSUtils\ARM9.cs`
- Text converter: `DS_Map\DSUtils\TextConverter.cs`
- Overlay utils: `DS_Map\DSUtils\OverlayUtils.cs`

## Script System Architecture (Detailed)

### File Structure
```
DS_Map/
├── Editors/
│   ├── ScriptEditor.cs              # Main script editor UI with ScintillaNET
│   ├── LevelScriptEditor.cs         # Level script trigger editor
│   └── Utils/
│       └── LoadingForm.cs           # Progress bar with Pokemon facts
├── ROMFiles/
│   ├── ScriptFile.cs                # Binary/plaintext I/O, caching, hashing
│   ├── ScriptCommand.cs             # Command representation
│   ├── ScriptCommandContainer.cs    # Script/function containers
│   ├── ScriptAction.cs              # Action/movement commands
│   ├── ScriptActionContainer.cs     # Action containers
│   └── LevelScriptFile.cs           # Level script handling
├── Resources/
│   ├── ScriptDatabase.cs            # JSON database loader, reference data
│   ├── ScriptCommandInfo.cs         # Command metadata structure
│   └── CustomScrcmdManager.cs       # Custom database management UI
├── Script/
│   ├── ScriptParameter.cs           # Parameter type and formatting
│   ├── ScriptCommandPosition.cs     # Position tracking for navigation
│   └── ScriptLabeledSection.cs      # Section labels and organization
└── ScintillaUtils/
    └── ScriptTooltip.cs             # Syntax-highlighted tooltips
```

### Script Parameter Types
```csharp
enum ParameterType {
    Integer,              // Raw integer value
    Variable,             // Game variable reference (0x4000+)
    Flex,                 // Flexible size parameter
    Overworld,            // Overworld/NPC ID
    OwMovementType,       // Overworld movement type
    OwMovementDirection,  // Movement direction (Up, Down, Left, Right)
    ComparisonOperator,   // Comparison operator (==, !=, <, >, <=, >=)
    Function,             // Function reference (#1, #2, etc.)
    Action,               // Action/movement reference (#1, #2, etc.)
    CMDNumber,            // Script command number
    Pokemon,              // Pokemon species ID (friendly name: "Pikachu")
    Item,                 // Item ID (friendly name: "Potion")
    Move,                 // Move ID (friendly name: "Thunderbolt")
    Sound,                // Sound ID
    Trainer               // Trainer ID
}
```

### Script Workflow Diagram
```
ROM Load → Unpack NARC → Binary Scripts
                              ↓
                    Check Database Hash
                              ↓
                    ┌─────────┴─────────┐
                    ↓                   ↓
            Hash Matches         Hash Changed
                    ↓                   ↓
          Skip Re-export       Delete & Re-export All
                    ↓                   ↓
                    └─────────┬─────────┘
                              ↓
                  Export to Plaintext (.script)
                              ↓
                    Store Database Hash
                              ↓
        ┌───────────────┬─────┴─────┬────────────────┐
        ↓               ↓           ↓                ↓
   Edit in DSPRE   Edit in VSCode  Search       View Only
   (ScintillaNET)  (external)      Scripts      Read Cache
        ↓               ↓           ↓                ↓
   Auto-save to    Detect newer    Parse all    No re-parse
   plaintext       plaintext       (with cache)  (use cache)
        ↓               ↓           ↓                ↓
        └───────────────┴───────────┴────────────────┘
                              ↓
                       ROM Save Event
                              ↓
              Scan for Newer Plaintext Files
                              ↓
              Rebuild Binary from Plaintext
                              ↓
                      Pack into NARC
                              ↓
                    Save ROM Project
```

### Script Database Structure (JSON)
```json
{
  "commands": [
    {
      "id": 123,
      "name": "GiveItem",
      "parameters": [
        {
          "type": "Item",
          "name": "item",
          "size": 2,
          "description": "Item to give"
        },
        {
          "type": "Integer",
          "name": "quantity",
          "size": 2,
          "description": "Number of items"
        }
      ],
      "decompName": "ScriptCmd_GiveItem"
    }
  ],
  "movements": [...],
  "comparisons": [...],
  "specialOverworlds": [...],
  "overworldDirections": [...]
}
```

## Notes on Codebase Evolution

This is a major overhaul with significant improvements:

### Original Features
- Faster load/save times
- Fixed Japanese DP ROM support
- Configurable UI layouts
- User preference memorization
- Advanced header search
- New NSBMD/NSBTX utilities
- NARC packer/unpacker
- Batch rename utilities
- ARM9 mismatch warnings
- ALT key shortcuts

### Recent Major Additions (Script System Overhaul)
- **Plaintext Script Support**: Human-readable `.script` files with timestamp-based sync
- **VS Code Integration**: External editor support with timestamp checking on load/save
- **Custom Script Databases**: Load and manage custom command databases for ROM hacks
- **Database Hashing**: Automatic change detection and re-export
- **Script Caching**: Performance optimization for large ROM hacks
- **Progress Tracking**: Loading screens with Pokemon facts for user engagement
- **Syntax Highlighting**: ScintillaNET-based editor with color-coded commands
- **Smart Parameter Display**: Friendly names for Pokemon, items, moves, etc.
- **Error Detection**: Automatic detection of invalid commands with user prompts
- **Bidirectional Sync**: Binary ↔ plaintext synchronization via timestamp comparison

### Other Recent Additions
- **Safari Zone Editor**: Complete Safari Zone encounter management
- **Headbutt Editor**: HGSS headbutt tree encounter editing
- **Trade Editor**: In-game trade Pokemon with IV/nature/item support
- **Enhanced Editors**: Evolution, learnset, and BTX texture editors
- **Model Export**: DAE and GLB format support via Apicula integration
- **ROM Patches**: Script repointing, trainer name expansion, texture animation control
- **UI Improvements**: Version labels, recent project reopening, better settings management

## Common Development Tasks

### Adding a New Script Command
1. Update the JSON database file for the appropriate game version
2. Add command metadata (ID, name, parameters, types)
3. ScriptDatabase.cs will auto-load on next run
4. No code changes needed - database-driven architecture

### Adding a New Parameter Type
1. Add enum value to `ParameterType` in `ScriptParameter.cs`
2. Implement formatting logic in `ScriptParameter.GetFormattedValue()`
3. Add reference dictionary to `ScriptDatabase.cs` if needed
4. Update JSON database to use new type

### Supporting a New ROM Hack
1. User provides custom script database JSON file
2. Load via CustomScrcmdManager form
3. DSPRE stores in `edited_databases/{romname}_scrcmd_database.json`
4. Auto-reparsing rebuilds all scripts with new database
5. Hash system prevents future mismatches

### Debugging Script Issues
1. Check `.database_hash` file in `expanded/scripts/`
2. Verify plaintext script syntax in `.script` files
3. Look for error messages in script editor about invalid commands
4. Check if custom database is loaded (CustomScrcmdManager)
5. Examine `ScriptFile.TryReadPlainTextFileCore()` for parsing errors

### Performance Optimization
- Use plaintext caching for batch operations (already implemented)
- Avoid unnecessary re-exports by checking database hash
- Implement progress callbacks for long-running operations
- Use `suppressErrors: true` flag for bulk script operations to avoid popup spam
