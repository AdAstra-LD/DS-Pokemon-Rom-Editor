using System;
using System.IO;
using System.Windows.Forms;

<<<<<<< HEAD
namespace NarcAPI {
    public class Narc {
=======
namespace NarcAPI
{
    public class Narc
    {
>>>>>>> 3431ae2... First commit
        public String Name { get; set; }
        private MemoryStream[] Elements;
        private int FatbOffset, FntbOffset, FimgOffset;

<<<<<<< HEAD
        private Narc(String name) {
            this.Name = name;
        }

        public static Narc NewEmpty(String name = "NewNarc") {
=======
        private Narc(String name)
        {
            this.Name = name;
        }

        public static Narc NewEmpty(String name = "NewNarc")
        {
>>>>>>> 3431ae2... First commit
            Narc narc = new Narc(name);
            return narc;
        }

<<<<<<< HEAD
        public static Narc Open(String filePath) {
=======
        public static Narc Open(String filePath)
        {
>>>>>>> 3431ae2... First commit
            FileStream file = File.OpenRead(filePath);
            Narc narc = new Narc(Path.GetFileNameWithoutExtension(filePath));
            BinaryReader br = new BinaryReader(file);

            uint magicNumber = br.ReadUInt32();
            if (magicNumber != 0x4352414E)
                return null;

            narc.ReadOffsets(br);
            narc.ReadElements(br);
            br.Close();
            return narc;
        }

<<<<<<< HEAD
        public static Narc FromFolder(String dirPath) {
=======
        public static Narc FromFolder(String dirPath)
        {
>>>>>>> 3431ae2... First commit
            Narc narc = new Narc(Path.GetDirectoryName(dirPath));
            String[] fileNames = Directory.GetFiles(dirPath, "*.*", SearchOption.AllDirectories);
            uint numberOfElements = (uint)fileNames.Length;
            narc.Elements = new MemoryStream[numberOfElements];
<<<<<<< HEAD
            for (int i = 0; i < numberOfElements; i++) {
=======
            for (int i = 0; i < numberOfElements; i++)
            {
>>>>>>> 3431ae2... First commit
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

<<<<<<< HEAD
        public void Save(String filePath) {
=======
        public void Save(String filePath)
        {
>>>>>>> 3431ae2... First commit
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
<<<<<<< HEAD
            for (int i = 0; i < Elements.Length; i++) {
=======
            for (int i = 0; i < Elements.Length; i++)
            {
>>>>>>> 3431ae2... First commit
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
<<<<<<< HEAD
            for (int i = 0; i < Elements.Length; i++) {
=======
            for (int i = 0; i < Elements.Length; i++)
            {
>>>>>>> 3431ae2... First commit
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

<<<<<<< HEAD
        public void ExtractToFolder(String dirPath) {
=======
        public void ExtractToFolder(String dirPath)
        {
>>>>>>> 3431ae2... First commit
            Console.WriteLine(dirPath);
            if (Directory.Exists(dirPath)) {
                try {
                    Directory.Delete(dirPath, true);
                } catch (IOException) {
<<<<<<< HEAD
                    MessageBox.Show("Can't access temp directory: \n" + dirPath + "\nThis might be a temporary issue.\nMake sure no other process is using it and try again.", "Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
=======
                    MessageBox.Show("Can't access temp directory: \n" + dirPath +"\nThis might be a temporary issue.\nMake sure no other process is using it and try again.", "Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
>>>>>>> 3431ae2... First commit
                    return;
                }
            }
            try {
                Directory.CreateDirectory(dirPath);
            } catch (ArgumentNullException) {
                MessageBox.Show("Dir path is null.", "Can't create directory", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            FileStream file;
            byte[] buffer;
<<<<<<< HEAD
            for (int i = 0; i < Elements.Length; i++) {
=======
            for (int i = 0; i < Elements.Length; i++)
            {
>>>>>>> 3431ae2... First commit
                file = File.Create(Path.Combine(dirPath, i.ToString("D4")));
                buffer = new byte[Elements[i].Length];
                Elements[i].Seek(0, SeekOrigin.Begin);
                Elements[i].Read(buffer, 0, (int)Elements[i].Length);
                file.Write(buffer, 0, (int)Elements[i].Length);
                file.Close();
            }
        }

        public void Free()              // Libera todos los recursos de memoria asociados (cierra los streams)
        {
<<<<<<< HEAD
            for (int i = 0; i < Elements.Length; i++) {
=======
            for (int i = 0; i < Elements.Length; i++)
            {
>>>>>>> 3431ae2... First commit
                Elements[i].Close();
            }
        }

<<<<<<< HEAD
        public MemoryStream this[int elemIndex] {
            get {
                return Elements[elemIndex];
            }
            set {
=======
        public MemoryStream this[int elemIndex]
        {
            get
            {
                return Elements[elemIndex];
            }
            set
            {
>>>>>>> 3431ae2... First commit
                Elements[elemIndex] = value;
            }
        }

<<<<<<< HEAD
        public int GetElementsLength() {
            return Elements.Length;
        }

        private void ReadOffsets(BinaryReader br) {
=======
        public int GetElementsLength()
        {
            return Elements.Length;
        }

        private void ReadOffsets(BinaryReader br)
        {
>>>>>>> 3431ae2... First commit
            FatbOffset = 0x10;
            br.BaseStream.Position = 0x18;                  // Number of elements
            FntbOffset = (int)br.ReadUInt32() * 8 + FatbOffset + 12;
            br.BaseStream.Position = FntbOffset + 0x4;      // FNTB Size
            FimgOffset = (int)br.ReadUInt32() + FntbOffset;
        }

<<<<<<< HEAD
        private void ReadElements(BinaryReader br) {
=======
        private void ReadElements(BinaryReader br)
        {
>>>>>>> 3431ae2... First commit
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
<<<<<<< HEAD
            for (int i = 0; i < numberOfElements; i++) {
=======
            for (int i = 0; i < numberOfElements; i++)
            {
>>>>>>> 3431ae2... First commit
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
