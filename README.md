# DS Pokemon Rom Editor

Nomura's C# and WinForm DS Pokemon ROM Editor forked with some QOL adjustments.

## Changes to Mixone's fork

1. New app Icon
2. Fixed header flag names.
3. Fixed "Text Editor" export button.
4. Fixed HGSS Overworld sprites not appearing in the event editor [overlay1 decompression command problem].
5. The "Open Matrix" button now loads the correct textures and buildings, even for interior maps.
6. Fixed a problem with Internal Names visualization, which occurred when a header's internal name was exactly 16 bytes long.
7. Fixed Exception upon loading Dragon's Den header (Music not found).
8. Fixed other Exceptions, which now show user-friendly messages
9. Correct list of HGSS cameras.
10. Fixed save error when a "Call Function_#" was in the script.
11. New palette match algorithm.
12. Updated editors' initial configurations.
13. Fixed some exceptions with more user-friendly messages.
14. Fixed some script command and movement names.
15. Enabled some of the quick script cmd buttons.
