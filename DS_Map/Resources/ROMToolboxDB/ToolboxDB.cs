using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public static Dictionary<uint[], string> matrixExpansionDB = new Dictionary<uint[], string>() {
            [new uint[] { 0x0203AEBE }] = "FF 01",
            [new uint[] { 0x0203AEC0 }] = "76 01",
            [new uint[] { 0x0203AF58 }] = "C9 01",
            [new uint[] { 0x0203AF72 }] = "49 01",
            [new uint[] { 0x0203AF8C }] = "3E 06 00 00",
            [new uint[] { 0x0203AF90 }] = "3C 1F 00 00",
            [new uint[] { 0x0203AFA8 }] = "50 1F 00 00",
            [new uint[] {   0x0203AFF8, 
                            0x0203B108, 
                            0x0203B1F0, 
                            0x0203B25C  }] = "C4 12 00 00",
            [new uint[] { 0x0203B088 }] = "84 0C 00 00",
            [new uint[] { 0x0203B0BC }] = "7C 0C 00 00",
        };
    }
}
