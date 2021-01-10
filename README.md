# DS Pokemon Rom Editor

Nomura's C# and WinForm DS Pokemon ROM Editor forked with some QOL adjustments.

## Major Changes to Mixone's fork
- Drastically sped up save time
- New search location feature
- Fixed HGSS Overworld sprites not appearing in the event editor [overlay1 decompression command problem].
- Added ARM9 memory expander to the new ROM Toolbox
- New palette match algorithm.
- Replaced flag search with "Search any command"
- Fixed map screen randomly going 3D with collision tab open
- Fixed "Open Matrix" not loading the correct textures and buildings, especially for interior maps.



## Minor Changes to Mixone's fork

- New app Icon
- Fixed app name
- Added BDHCAM Support.
- Corrected list of HGSS cameras.
- Fixed Text message search being case sensitive no matter what
- Fixed BGS signature of maps being overwritten upon saving.
- Fixed header flag names.
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
- Added new ALT key shortcuts (Hold alt to see which shortcuts are available)
- Added placeholder BDHCAM button to toolbox
- Added placeholder "Pokemon names case converter" button to toolbox
- Fixed Encounters editor not opening
