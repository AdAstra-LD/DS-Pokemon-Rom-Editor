# DS Pokemon Rom Editor

Nomura's C# and WinForm DS Pokemon ROM Editor forked with some QOL adjustments.

## Changes to Mixone's fork

1. Drastically sped up save time
2. New app Icon
3. Fixed app name
4. Added BDHCAM Support.
5. Fixed BGS signature of maps being overwritten upon saving.
6. Fixed header flag names.
7. Fixed "Text Editor" export button.
8. Fixed HGSS Overworld sprites not appearing in the event editor [overlay1 decompression command problem].
9. The "Open Matrix" button now loads the correct textures and buildings, even for interior maps.
10. Fixed a problem with Internal Names visualization, which occurred when a header's internal name was exactly 16 bytes long.
11. Fixed Exception upon loading Dragon's Den header (Music not found).
12. Fixed other Exceptions, which now show user-friendly messages
13. Correct list of HGSS cameras.
14. Fixed save error when a "Call Function_#" was in the script.
15. New palette match algorithm.
16. Updated editors' initial configurations.
17. Fixed some script command and movement names.
18. Enabled some of the quick script cmd buttons.
19. Fixed map screen going 3D with collision tab open
20. Added New Movement names and changed some of the old ones
21. Replaced flag search with "Search any command"
22. Changed row numbers to hex in Text Editor
23. Expanded collisions database
