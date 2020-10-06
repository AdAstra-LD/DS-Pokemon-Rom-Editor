using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NarcAPI
{
    public class Narc
    {
        public String Name { get; set; }
        private MemoryStream[] Elements;
        private int FatbOffset, FntbOffset, FimgOffset;

        private Narc(String name)
        {
            this.Name = name;
        }

        public static Narc NewEmpty(String name = "NewNarc")
        {
            Narc narc = new Narc(name);
            return narc;
        }

        public static Narc Open(String filePath)
        {
            FileStream file = File.OpenRead(filePath);
            Narc narc = new Narc(Path.GetFileNameWithoutExtension(filePath));
            BinaryReader br = new BinaryReader(file);
            narc.ReadOffsets(br);
            narc.ReadElements(br);
            br.Close();
            return narc;
        }

        public static Narc FromFolder(String folderPath)
        {
            Narc narc = new Narc(Path.GetDirectoryName(folderPath));
            String[] fileNames = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories);
            uint numberOfElements = (uint)fileNames.Length;
            narc.Elements = new MemoryStream[numberOfElements];
            for (int i = 0; i < numberOfElements; i++)
            {
                FileStream fs = File.OpenRead(fileNames[i]);
                MemoryStream ms = new MemoryStream();
                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, (int)fs.Length);
                ms.Write(buffer, 0, (int)fs.Length);
                narc.Elements[i] = ms;
                fs.Close();
            }
            return narc;
        }

        public void Save(String filePath)
        {
            uint fileSizeOffset, fimgSizeOffset, curOffset;

            FileStream file = File.Create(filePath);
            BinaryWriter bw = new BinaryWriter(file);
            // Write NARC Section
            bw.Write(0x4352414E);      // "NARC"
            bw.Write(0x0100FFFE);      // Constant
            fileSizeOffset = (uint)bw.BaseStream.Position;
            bw.Write((UInt32)0x0);
            bw.Write((UInt16)16);
            bw.Write((UInt16)3);
            // Write FATB Section
            bw.Write(0x46415442);      // "BTAF"
            bw.Write((UInt32)(0xC + Elements.Length * 8));  // FATB Size
            bw.Write((UInt32)Elements.Length);              // Number of elements
            curOffset = 0;
            for (int i = 0; i < Elements.Length; i++)
            {
                while (curOffset % 4 != 0) curOffset++;     // Force offsets to be a multiple of 4
                bw.Write(curOffset);
                curOffset += (uint)Elements[i].Length;
                bw.Write(curOffset);
            }
            // Write FNTB Section (No names, sorry =( )
            bw.Write(0x464E5442);       // "BTNF"
            bw.Write(0x10);             // FNTB Size
            bw.Write(0x4);              // <-
            bw.Write(0x10000);          //  |-- Pointless data
            // Write FIMG Section
            bw.Write(0x46494D47);       // "GMIF"
            fimgSizeOffset = (uint)bw.BaseStream.Position;
            bw.Write((UInt32)0x0);
            curOffset = 0;
            byte[] buffer;
            for (int i = 0; i < Elements.Length; i++)
            {
                while (curOffset % 4 != 0) { bw.Write((Byte)0xFF); curOffset++; }     // Force offsets to be a multiple of 4
                // Data writin'
                buffer = new byte[Elements[i].Length];
                Elements[i].Seek(0, SeekOrigin.Begin);
                Elements[i].Read(buffer, 0, (int)Elements[i].Length);
                bw.Write(buffer, 0, (int)Elements[i].Length);
                curOffset += (uint)Elements[i].Length;
            }
            // Writes sizes
            int fileSize = (int)bw.BaseStream.Position;
            bw.Seek((int)fileSizeOffset, SeekOrigin.Begin);         // File size
            bw.Write((UInt32)fileSize);
            bw.Seek((int)fimgSizeOffset, SeekOrigin.Begin);         // FIMG size
            bw.Write((UInt32)curOffset + 0x8);                      // FIMG size == Last end offset + 0x8
            bw.Close();
        }

        public void ExtractToFolder(String folderPath)
        {
            if (Directory.Exists(folderPath))
            {
                Directory.Delete(folderPath, true);
            }
            Directory.CreateDirectory(folderPath);
            FileStream file;
            byte[] buffer;
            for (int i = 0; i < Elements.Length; i++)
            {
                file = File.Create(Path.Combine(folderPath, i.ToString("D4")));
                buffer = new byte[Elements[i].Length];
                Elements[i].Seek(0, SeekOrigin.Begin);
                Elements[i].Read(buffer, 0, (int)Elements[i].Length);
                file.Write(buffer, 0, (int)Elements[i].Length);
                file.Close();
            }
        }

        public void Free()              // Libera todos los recursos de memoria asociados (cierra los streams)
        {
            for (int i = 0; i < Elements.Length; i++)
            {
                Elements[i].Close();
            }
        }

        public MemoryStream this[int elemIndex]
        {
            get
            {
                return Elements[elemIndex];
            }
            set
            {
                Elements[elemIndex] = value;
            }
        }

        public int GetElementsLength()
        {
            return Elements.Length;
        }

        private void ReadOffsets(BinaryReader br)
        {
            FatbOffset = 0x10;
            br.BaseStream.Position = 0x18;                  // Number of elements
            FntbOffset = (int)br.ReadUInt32() * 8 + FatbOffset + 12;
            br.BaseStream.Position = FntbOffset + 0x4;      // FNTB Size
            FimgOffset = (int)br.ReadUInt32() + FntbOffset;
        }

        private void ReadElements(BinaryReader br)
        {
            uint numberOfElements;
            uint[] startOffsets, endOffsets;
            // Create array of elements
            br.BaseStream.Position = 0x18;
            Elements = new MemoryStream[numberOfElements = br.ReadUInt32()];
            // Read offsets of each element
            startOffsets = new uint[numberOfElements]; endOffsets = new uint[numberOfElements];
            br.BaseStream.Position = FatbOffset + 0xC;
            for (int i = 0; i < numberOfElements; i++) { startOffsets[i] = br.ReadUInt32(); endOffsets[i] = br.ReadUInt32(); }
            // Read elements
            for (int i = 0; i < numberOfElements; i++)
            {
                MemoryStream ms;
                byte[] buffer;
                br.BaseStream.Position = FimgOffset + startOffsets[i] + 0x8;
                buffer = new byte[endOffsets[i] - startOffsets[i]];
                br.Read(buffer, 0, (int)(endOffsets[i] - startOffsets[i]));
                ms = new MemoryStream(buffer);
                Elements[i] = ms;
            }
        }
    }
}
