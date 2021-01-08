# DS Pokemon Rom Editor

Nomura's C# and WinForm DS Pokemon ROM Editor forked with some QOL adjustments.

## Changes to Mixone's fork

1. New app Icon
2. Fixed app name
3. Added BDHCAM Support.
4. Fixed BGS signature of maps being overwritten upon saving.
5. Fixed header flag names.
6. Fixed "Text Editor" export button.
7. Fixed HGSS Overworld sprites not appearing in the event editor [overlay1 decompression command problem].
8. The "Open Matrix" button now loads the correct textures and buildings, even for interior maps.
9. Fixed a problem with Internal Names visualization, which occurred when a header's internal name was exactly 16 bytes long.
10. Fixed Exception upon loading Dragon's Den header (Music not found).
11. Fixed other Exceptions, which now show user-friendly messages
12. Correct list of HGSS cameras.
13. Fixed save error when a "Call Function_#" was in the script.
14. New palette match algorithm.
15. Updated editors' initial configurations.
16. Fixed some script command and movement names.
17. Enabled some of the quick script cmd buttons.
18. Fixed map screen going 3D with collision tab open
19. Added New Movement names and changed some of the old ones
20. Replaced flag search with "Search any command"
21. Changed row numbers to hex in Text Editor
