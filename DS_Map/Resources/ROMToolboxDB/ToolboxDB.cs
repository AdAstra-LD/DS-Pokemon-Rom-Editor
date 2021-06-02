using System;
using System.Collections.Generic;

namespace DSPRE.Resources.ROMToolboxDB {
    public class ToolboxDB {
        public static Dictionary<string, uint> syntheticOverlayFileNumbersDB = new Dictionary<string, uint>() {
            ["DP"] = 9,
            ["Plat"] = 9,
            ["HGSS"] = 0,
        };

        public static Dictionary<string, string> arm9ExpansionCodeDB = new Dictionary<string, string>() {
            ["branchString_DP_ENG"] = "05 F1 34 FC",
            ["branchString_DP_ESP"] = "05 F1 04 FD",
            ["branchString_Plat_ENG"] = "00 F1 B4 F8",
            ["branchString_Plat_ESP"] = "00 F1 B2 F9",
            ["branchString_HGSS_ENG"] = "0F F1 30 FB",
            ["branchString_HGSS_ESP"] = "0F F1 40 FB",

            ["initString_DP"] = "FC B5 05 48 C0 46 41 21 09 22 02 4D A8 47 00 20 03 21 FC BD F1 64 00 02 00 80 3C 02",  //Valid for ENG and ESP, also for P            
            ["initString_Plat_ENG"] = "FC B5 05 48 C0 46 41 21 09 22 02 4D A8 47 00 20 03 21 FC BD A5 6A 00 02 00 80 3C 02",
            ["initString_Plat_ESP"] = "FC B5 05 48 C0 46 41 21 09 22 02 4D A8 47 00 20 03 21 FC BD B9 6A 00 02 00 80 3C 02",
            ["initString_HGSS"] = "FC B5 05 48 C0 46 1C 21 00 22 02 4D A8 47 00 20 03 21 FC BD 09 75 00 02 00 80 3C 02" //Valid for ENG and ESP, also for SS
        };
        public static Dictionary<string, uint> arm9ExpansionOffsetsDB = new Dictionary<string, uint>() {
            ["branchOffset_DP"] = 0x02000C80, //Valid also for P
            ["branchOffset_Plat"] = 0x02000CB4,
            ["branchOffset_HGSS"] = 0x02000CD0, //Valid also for SS

            ["initOffset_DP_ENG"] = 0x021064EC,
            ["initOffset_DP_ESP"] = 0x0210668C,
            ["initOffset_Plat_ENG"] = 0x02100E20,
            ["initOffset_Plat_ESP"] = 0x0210101C,
            ["initOffset_HGSS_ENG"] = 0x02110334,
            ["initOffset_HGSS_ESP"] = 0x02110354
        };

        public static Dictionary<string, string> BDHCamCodeDB = new Dictionary<string, string>() {
            ["branchString_Plat_ENG"] = "B9 F3 E2 F8",
            ["branchString_Plat_ESP"] = "B9 F3 AA F8",
            ["branchString_HGSS"] = "B6 F3 2E FA", //Also valid for SS, both ESP and ENG

            ["overlayString1"] = "00 4B 18 47 41 9C 3D 02",
            ["overlayString2"] = "00 4B 18 47 01 9C 3D 02",
        };
        public static Dictionary<string, uint> BDHCamOffsetsDB = new Dictionary<string, uint>() {
            ["branchOffset_Plat_ENG"] = 0x0202040C,
            ["branchOffset_Plat_ESP"] = 0x0202047C,
            ["branchOffset_HGSS"] = 0x02023174, //Also valid for SS, both ESP and ENG

            ["overlayOffset1_Plat_ENG"] = 0x0001E1B4,
            ["overlayOffset1_Plat_ESP"] = 0x0001E1BC,
            ["overlayOffset1_HGSS"] = 0x0001574C,

            ["overlayOffset2_Plat_ENG"] = 0x0001E2CC,
            ["overlayOffset2_Plat_ESP"] = 0x0001E2D4,
            ["overlayOffset2_HGSS"] = 0x00015864,
        };
        public static uint BDHCamSubroutineOffset = 0x000115B0;

        public static uint getDynamicHeadersInitOffset(string romID) {
            switch (romID) {
                case "CPUE":
                    return 0x3A024;
                case "CPUS":
                case "CPUI":
                case "CPUF":
                case "CPUD":
                    return 0x3A0C8;
                case "CPUJ":
                    return 0x39BE0;
                case "IPKS":
                    return 0x3B260;
                case "IPKJ":
                case "IPGJ":
                    return 0x3AE20;
                default:
                    return 0x3B268;
            }
        }
        public static string getDynamicHeadersInitString(string romID) {
            switch (romID) {
                case "CPUE":
                    return "00 B5 01 1C 94 20 00 22 CC F7 48 FD 03 1C DE F7 C7 F8 00 BD";
                case "CPUS":
                case "CPUI":
                case "CPUF":
                case "CPUD":
                    return "00 B5 01 1C 94 20 00 22 CC F7 00 FD 03 1C CC F7 74 FC 00 BD";
                case "CPUJ":
                    return "00 B5 01 1C 94 20 00 22 CC F7 0A FF 03 1C DE F7 3D F9 00 BD";
                case "IPKS":
                    return "00 B5 01 1C 32 20 00 22 CC F7 5C F9 03 1C DF F7 4D FC 00 BD";
                case "IPKJ":
                case "IPGJ":
                    return "00 B5 01 1C 32 20 00 22 CC F7 08 FB 03 1C DF F7 C7 FC 00 BD";
                default:
                    return "00 B5 01 1C 32 20 00 22 CC F7 58 F9 03 1C DF F7 49 FC 00 BD";
            }
        }
        public static Dictionary<string, Tuple<uint, uint>[]> dynamicHeadersPointersDB = new Dictionary<string, Tuple<uint, uint>[]>() {
            // format: headerID*18 offset, (ARM9_HEADER_TABLE_OFFSET + n) offset

            ["Plat"] = new Tuple<uint, uint>[] {
            new Tuple<uint, uint>(0x3A03E, 0x3A048),
            new Tuple<uint, uint>(0x3A052, 0x3A05C),
            new Tuple<uint, uint>(0x3A066, 0x3A080),
            new Tuple<uint, uint>(0x3A08E, 0x3A098),
            new Tuple<uint, uint>(0x3A0A2, 0x3A0AC),
            new Tuple<uint, uint>(0x3A0B6, 0x3A0C0),
            new Tuple<uint, uint>(0x3A0CA, 0x3A0D4),
            new Tuple<uint, uint>(0x3A0DE, 0x3A0E8),
            new Tuple<uint, uint>(0x3A0F2, 0x3A108),
            new Tuple<uint, uint>(0x3A116, 0x3A120),
            new Tuple<uint, uint>(0x3A12A, 0x3A134),
            new Tuple<uint, uint>(0x3A13E, 0x3A150),
            new Tuple<uint, uint>(0x3A15A, 0x3A170),
            new Tuple<uint, uint>(0x3A17A, 0x3A184),
            new Tuple<uint, uint>(0x3A18E, 0x3A198),
            new Tuple<uint, uint>(0x3A1A2, 0x3A1B4),
            new Tuple<uint, uint>(0x3A1BE, 0x3A1D0),
            new Tuple<uint, uint>(0x3A1DA, 0x3A1EC),
            new Tuple<uint, uint>(0x3A1F6, 0x3A208),
            new Tuple<uint, uint>(0x3A212, 0x3A224),
            },

            ["HGSS"] = new Tuple<uint, uint>[] {
            new Tuple<uint, uint>(0x3B282, 0x3B28C),
            new Tuple<uint, uint>(0x3B296, 0x3B2A8),
            new Tuple<uint, uint>(0x3B2B2, 0x3B2BC),
            new Tuple<uint, uint>(0x3B2C6, 0x3B2D0),
            new Tuple<uint, uint>(0x3B2DA, 0x3B2E4),
            new Tuple<uint, uint>(0x3B2EE, 0x3B2F8),
            new Tuple<uint, uint>(0x3B302, 0x3B30C),
            new Tuple<uint, uint>(0x3B316, 0x3B320),
            new Tuple<uint, uint>(0x3B32A, 0x3B340),
            new Tuple<uint, uint>(0x3B34A, 0x3B354),
            new Tuple<uint, uint>(0x3B35E, 0x3B368),
            new Tuple<uint, uint>(0x3B372, 0x3B384),
            new Tuple<uint, uint>(0x3B38E, 0x3B3A4),
            new Tuple<uint, uint>(0x3B3AE, 0x3B3C4),
            new Tuple<uint, uint>(0x3B3CE, 0x3B3E0),
            new Tuple<uint, uint>(0x3B3EA, 0x3B3FC),
            new Tuple<uint, uint>(0x3B406, 0x3B418),
            new Tuple<uint, uint>(0x3B422, 0x3B434),
            new Tuple<uint, uint>(0x3B43E, 0x3B450),
            new Tuple<uint, uint>(0x3B45A, 0x3B46C),
            new Tuple<uint, uint>(0x3B476, 0x3B488),
            new Tuple<uint, uint>(0x3B492, 0x3B4A4),
            new Tuple<uint, uint>(0x3B4AE, 0x3B4C0),
            new Tuple<uint, uint>(0x3B4CA, 0x3B4D8),
            new Tuple<uint, uint>(0x3B4E2, 0x3B4F4),
            new Tuple<uint, uint>(0x3B4FE, 0x3B514),
            },

        };

        public static Dictionary<uint[], string> matrixExpansionDB = new Dictionary<uint[], string>() {
            [new uint[] { 0x0203AEBE }] = "FF 01",
            [new uint[] { 0x0203AEC0 }] = "76 01",
            [new uint[] { 0x0203AF58 }] = "C9 01",
            [new uint[] { 0x0203AF72 }] = "49 01",
            [new uint[] { 0x0203AF8C }] = "3E 06 00 00",
            [new uint[] { 0x0203AF90 }] = "3C 1F 00 00",
            [new uint[] { 0x0203AFA8 }] = "50 1F 00 00",
            [new uint[] { 0x0203AFF8,
                          0x0203B108,
                          0x0203B1F0,
                          0x0203B25C }] = "C4 12 00 00",
            [new uint[] { 0x0203B088 }] = "84 0C 00 00",
            [new uint[] { 0x0203B0BC }] = "7C 0C 00 00",
        };
    }
}
