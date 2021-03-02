using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSPRE.Resources.ROMToolboxDB {
    public class ToolboxDB {
        public static Dictionary<string, uint> syntheticOverlayFileNumbersDB = new Dictionary<string, uint>() {
            ["D"] = 9,
            ["P"] = 9,
            ["Plat"] = 9,
            ["HG"] = 0,
            ["SS"] = 0
        };

        public static Dictionary<string, string> arm9ExpansionCodeDB = new Dictionary<string, string>() {
            ["branchString_D_ENG"] = "05 F1 34 FC",
            ["branchString_D_ESP"] = "05 F1 04 FD",
            ["branchString_Plat_ENG"] = "00 F1 B4 F8",
            ["branchString_Plat_ESP"] = "00 F1 B2 F9",
            ["branchString_HG_ENG"] = "0F F1 30 FB",
            ["branchString_HG_ESP"] = "0F F1 40 FB",

            ["initString_D"] = "FC B5 05 48 C0 46 41 21 09 22 02 4D A8 47 00 20 03 21 FC BD F1 64 00 02 00 80 3C 02",  //Valid for ENG and ESP, also for P            
            ["initString_Plat_ENG"] = "FC B5 05 48 C0 46 41 21 09 22 02 4D A8 47 00 20 03 21 FC BD A5 6A 00 02 00 80 3C 02",
            ["initString_Plat_ESP"] = "FC B5 05 48 C0 46 41 21 09 22 02 4D A8 47 00 20 03 21 FC BD B9 6A 00 02 00 80 3C 02",
            ["initString_HG"] = "FC B5 05 48 C0 46 1C 21 00 22 02 4D A8 47 00 20 03 21 FC BD 09 75 00 02 00 80 3C 02" //Valid for ENG and ESP, also for SS
        };
        public static Dictionary<string, uint> arm9ExpansionOffsetsDB = new Dictionary<string, uint>() {
            ["branchOffset_D"] = 0x02000C80, //Valid also for P
            ["branchOffset_Plat"] = 0x02000CB4,
            ["branchOffset_HG"] = 0x02000CD0, //Valid also for SS

            ["initOffset_D_ENG"] = 0x021064EC,
            ["initOffset_D_ESP"] = 0x0210668C,
            ["initOffset_Plat_ENG"] = 0x02100E20,
            ["initOffset_Plat_ESP"] = 0x0210101C,
            ["initOffset_HG_ENG"] = 0x02110334,
            ["initOffset_HG_ESP"] = 0x02110354
        };

        public static Dictionary<string, string> bdhcamCodeDB = new Dictionary<string, string>() {
            ["branchString_Plat_ENG"] = "B9 F3 E2 F8",
            ["branchString_Plat_ESP"] = "B9 F3 AA F8",
            ["branchString_HG"] = "B6 F3 2E FA", //Also valid for SS, both ESP and ENG

            ["overlayString1"] = "00 4B 18 47 41 9C 3D 02",
            ["overlayString2"] = "00 4B 18 47 01 9C 3D 02",
        };
        public static Dictionary<string, uint> bdhcamOffsetsDB = new Dictionary<string, uint>() {
            ["branchOffset_Plat_ENG"] = 0x0202040C,
            ["branchOffset_Plat_ESP"] = 0x0202047C,
            ["branchOffset_HG"] = 0x02023174, //Also valid for SS, both ESP and ENG

            ["overlayOffset1_Plat_ENG"] = 0x0001E1B4,
            ["overlayOffset1_Plat_ESP"] = 0x0001E1BC,
            ["overlayOffset1_HG"] = 0x0001574C,

            ["overlayOffset2_Plat_ENG"] = 0x0001E2CC,
            ["overlayOffset2_Plat_ESP"] = 0x0001E2D4,
            ["overlayOffset2_HG"] = 0x00015864,
        };
        public static uint bdhcamSubroutineOffset = 0x000115B0;
    }
}
