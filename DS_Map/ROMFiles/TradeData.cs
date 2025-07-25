using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DSPRE.RomInfo;

namespace DSPRE.ROMFiles
{
    internal class TradeData : RomFile
    {

        public int id;

        // TradeData structure in narc
        public int species;
        public int hpIV;
        public int atkIV;
        public int defIV;
        public int speedIV;
        public int spAtkIV;
        public int spDefIV;
        public int ability; // unused
        public int otID;
        public int cool;
        public int beauty;
        public int cute;
        public int smart;
        public int tough;
        public int pid;
        public int heldItem;
        public int otGender;
        public int sheen;
        public int language;
        public int requestedSpecies;
        public int unknown; // unused?

        public TradeData(int id, Stream stream)
        {
            this.id = id;
            using (BinaryReader br = new BinaryReader(stream))
            {
                species = br.ReadInt32();
                hpIV = br.ReadInt32();
                atkIV = br.ReadInt32();
                defIV = br.ReadInt32();
                speedIV = br.ReadInt32();
                spAtkIV = br.ReadInt32();
                spDefIV = br.ReadInt32();
                ability = br.ReadInt32(); // appears unused
                otID = br.ReadInt32();
                cool = br.ReadInt32();
                beauty = br.ReadInt32();
                cute = br.ReadInt32();
                smart = br.ReadInt32();
                tough = br.ReadInt32();
                pid = br.ReadInt32();
                heldItem = br.ReadInt32();
                otGender = br.ReadInt32();
                sheen = br.ReadInt32();
                language = br.ReadInt32();
                requestedSpecies = br.ReadInt32();

                if (RomInfo.gameFamily == GameFamilies.HGSS)
                {
                    unknown = br.ReadInt32(); // unused?
                }
                    
            }
        }

        public TradeData(int id) : this(id, GetStream(id)) { }

        private static Stream GetStream(int id)
        {

            if (!File.Exists(RomInfo.gameDirs[DirNames.tradeData].unpackedDir + "\\" + id.ToString("D4")))
            {
                // If the file does not exist, create it with default values
                FileStream fileStream = new FileStream(RomInfo.gameDirs[DirNames.tradeData].unpackedDir + "\\" + id.ToString("D4"), FileMode.Create);
                fileStream.Write(new byte[0x50], 0, 0x50); // create an empty file
                if (RomInfo.gameFamily == GameFamilies.HGSS)
                {
                    fileStream.Write(new byte[0x04], 0, 0x04); // HGSS only
                }
                fileStream.Seek(0, SeekOrigin.Begin); // Reset the position to the start of the file
                return fileStream;
            }
            
            return new FileStream(RomInfo.gameDirs[DirNames.tradeData].unpackedDir + "\\" + id.ToString("D4"), FileMode.Open);
            
        }

        public override byte[] ToByteArray()
        {
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                bw.Write(species);
                bw.Write(hpIV);
                bw.Write(atkIV);
                bw.Write(defIV);
                bw.Write(speedIV);
                bw.Write(spAtkIV);
                bw.Write(spDefIV);
                bw.Write(ability); // unused
                bw.Write(otID);
                bw.Write(cool);
                bw.Write(beauty);
                bw.Write(cute);
                bw.Write(smart);
                bw.Write(tough);
                bw.Write(pid);
                bw.Write(heldItem);
                bw.Write(otGender);
                bw.Write(sheen);
                bw.Write(language);
                bw.Write(requestedSpecies);

                if (RomInfo.gameFamily == GameFamilies.HGSS)
                {
                    bw.Write(unknown); // HGSS only 
                }
                    
                return ms.ToArray();
            }
        }

        public void SaveToFileDefaultDir(int IDtoReplace, bool showSuccessMessage = true)
        {
            SaveToFileDefaultDir(DirNames.tradeData, IDtoReplace, showSuccessMessage);
        }
        public void SaveToFileExplorePath(string suggestedFileName, bool showSuccessMessage = true)
        {
            SaveToFileExplorePath("Gen IV Trade data", "bin", suggestedFileName, showSuccessMessage);
        }

        public static int GetTradeCount()
        {
            return Directory.GetFiles(RomInfo.gameDirs[DirNames.tradeData].unpackedDir, "*.*").Length;
        }

    }
}
