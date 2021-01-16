# DS Pokemon Rom Editor

Nomura's C# and WinForm DS Pokemon ROM Editor forked with some QOL adjustments.

## Major Changes to Mixone's fork
- Drastically sped up load and save time
- New search location feature
- Added mouse support and warps navigation feature to the Event Editor
- Encounter editor now allows adding and removing encounter files
- Fixed OW search algorithm for most (if not all) supported ROMs
- Added ARM9 memory expander to the new ROM Toolbox
- New palette match algorithm.
- Replaced flag search with "Search any command"
- Fixed map screen randomly going 3D with collision tab open
- Fixed "Open Matrix" not loading the correct textures and buildings, especially for interior maps.


## Minor Changes to Mixone's fork

- New app Icon
- Fixed app name
- Added BDHCAM Support.
- Fixed header flag names.
- Corrected list of HGSS cameras.
- Added new ALT key shortcuts (Hold alt to see which shortcuts are available)
- Wild Poke editor now detects selected header
- Fixed Text message search being case sensitive no matter what
- Fixed BGS signature of maps being overwritten upon saving.
- Fixed "Text Editor" export button.
- Fixed a problem with Internal Names visualization (String termination problem for 16-byte long names).
- Fixed Exception upon loading Dragon's Den header (Music not found).
- Fixed many Exceptions, which now show user-friendly messages
- Fixed save error when a "Call Function_#" was in the script.
- Updated editors' initial configurations.
- Fixed some script command and movement names.
- Enabled some of the quick script cmd buttons.
- Added New Movement names and changed some of the old ones
- Changed row numbers to hex in Text Editor
- Expanded collisions database
- Added placeholder BDHCAM button to toolbox
- Added placeholder "Pokemon names case converter" button to toolbox
- Fixed Encounters editor not opening
- Fixed wrong numbers when resizing matrices
- Added a warning when attempting to resize matrix 0
- Fixed Matrix editor "add" and "remove" buttons GUI numbering mismatch
- the "Open Wild Pokémon" button in Header Editor is disabled when a NULL encounter file is detected