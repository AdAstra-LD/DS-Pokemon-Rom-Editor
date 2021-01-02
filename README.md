# DS Pokemon Rom Editor

Nomura's C# and WinForm DS Pokemon ROM Editor forked with some QOL adjustments.

## Changes to Mixone's fork

1. New app Icon
2. Added BDHCAM Support.
3. Fixed BGS signature of maps being overwritten upon saving.
4. Fixed header flag names.
5. Fixed "Text Editor" export button.
6. Fixed HGSS Overworld sprites not appearing in the event editor [overlay1 decompression command problem].
7. The "Open Matrix" button now loads the correct textures and buildings, even for interior maps.
8. Fixed a problem with Internal Names visualization, which occurred when a header's internal name was exactly 16 bytes long.
9. Fixed Exception upon loading Dragon's Den header (Music not found).
10. Fixed other Exceptions, which now show user-friendly messages
11. Correct list of HGSS cameras.
12. Fixed save error when a "Call Function_#" was in the script.
13. New palette match algorithm.
14. Updated editors' initial configurations.
15. Fixed some exceptions with more user-friendly messages.
16. Fixed some script command and movement names.
17. Enabled some of the quick script cmd buttons.
18. Fixed map screen going 3D with collision tab open
