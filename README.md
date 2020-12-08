# DS Pokemon Rom Editor

Nomura's C# and WinForm DS Pokemon ROM Editor forked with some QOL adjustments.

## Motivation

As DSPRE is currently undergoing an overhaul towards Java, I have forked and added some additions for myself in the meantime. These are all some simple QOL improvements.

## Changes to original

The current list of modified features is as follows:

### Finished additions
1. Line numbers in script editor.
2. A couple of patches here and there for certain unhandled exceptions.

### Almost finished
1. Full repair of script editor saving for HGSS.
2. Overworld sprite loading repair. Now that proper table format known should be relatively easy.

### Building blocks set
1. Internal camera preview and edit
2. BW2 compatability ([explained more in depth below](#bw2-support-plans))

### Planned
1. Script editor syntax highlight

## BW2 support plans

When I have time I will little by little be adding stuff for BW2, initial plan is Text files, then Maps, then Pokémon, and so on.

Do not expect any updates on this soon or with any consistency, this is purely done because I like to code and I wanted to see if I understand DS ROM hacking enough to be able to add BW2 to DSPRE.
