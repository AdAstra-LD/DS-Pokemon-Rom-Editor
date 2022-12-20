using DSPRE;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NarcAPI {
    public class Narc { //Nintendo Archive
        public String Name { get; set; }
        private MemoryStream[] Elements;
        private int FntbOffset, FimgOffset;             //fntb = File Name TaBle
                                                        //fimg = File image
        
        private const int NARCFILEMAGICNUM = 0x4352414E; //"NARC" in ascii/unicode
        private const int FATBOFFSET = 0x10;             //fatb = File Allocation TaBle
        private const int FATBNUMELEMENTSPOS = 0x18;
        private const int FATBHEADERSIZE = 12;
        private const int FATBELEMENTSIZE = 0x8;           //size of startOffset + size of endOffset (4 bytes each)
        private const int FIMGHEADERSIZE = 0x8;

        private Narc(String name) {
            this.Name = name;
        }

        public static Narc NewEmpty(String name = "NewNarc") {
            Narc narc = new Narc(name);
            return narc;
        }

        public static Narc Open(String filePath) {
            Narc narc = new Narc(Path.GetFileNameWithoutExtension(filePath));
            BinaryReader br = new BinaryReader(File.OpenRead(filePath));

            if (br.ReadUInt32() != NARCFILEMAGICNUM) {
                return null;
            }

            narc.ReadOffsets(br);
            narc.ReadElements(br);
            br.Close();
            return narc;
        }

        public static Narc FromFolder(String dirPath) {
            Narc narc = new Narc(Path.GetDirectoryName(dirPath));
            String[] fileNames = Directory.GetFiles(dirPath, "*.*", SearchOption.AllDirectories);
            uint numberOfElements = (uint)fileNames.Length;
            narc.Elements = new MemoryStream[numberOfElements];

            Parallel.For(0, numberOfElements, i => {
                FileStream fs = File.OpenRead(fileNames[i]);
                MemoryStream ms = new MemoryStream();
                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, (int)fs.Length);
                ms.Write(buffer, 0, (int)fs.Length);
                narc.Elements[i] = ms;
                fs.Close();
            });
            return narc;
        }

        public void Save(String filePath) {
            uint fileSizeOffset, fimgSizeOffset, curOffset;

            BinaryWriter bw = new BinaryWriter(File.Create(filePath));
            // Write NARC Section
            bw.Write(NARCFILEMAGICNUM);
            bw.Write(0x0100FFFE);                   //the end of the file signature for all nintendo files
            fileSizeOffset = (uint)bw.BaseStream.Position;
            bw.Write((UInt32)0x0);
            bw.Write((ushort)16);                   //full size of header section
            bw.Write((ushort)3);                    //the number of sections in the header
            // Write FATB Section
            bw.Write(0x46415442);                   // "BTAF"
            bw.Write((UInt32)(FATBHEADERSIZE + Elements.Length * FATBELEMENTSIZE));
            bw.Write((UInt32)Elements.Length);      // Number of elements
            curOffset = 0;
            for (int i = 0; i < Elements.Length; i++) {
                while (curOffset % 4 != 0) {
                    curOffset++;     // Force offsets to be a multiple of 4
                }

                bw.Write(curOffset);
                curOffset += (uint)Elements[i].Length;
                bw.Write(curOffset);
            }
            // Write FNTB Section (No names, sorry =( )
            bw.Write(0x464E5442);       //"BTNF"
            bw.Write(0x10);             //FNTB Size
            bw.Write(0x4);              //the offset of the first name directory
            bw.Write(0x10000);          //filler data describing a file at position 0 with 1 directory in the archive
            // Write FIMG Section
            bw.Write(0x46494D47);       // "GMIF"
            fimgSizeOffset = (uint)bw.BaseStream.Position;
            bw.Write((UInt32)0x0);      
            curOffset = 0;
            byte[] buffer;
            for (int i = 0; i < Elements.Length; i++) {
                while (curOffset % 4 != 0) { // Force offsets to be a multiple of 4
                    bw.Write((Byte)0xFF); curOffset++; 
                }     
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
            bw.Seek((int)fimgSizeOffset, SeekOrigin.Begin);         // seeks back to FIMG size
            bw.Write((UInt32)curOffset + FIMGHEADERSIZE);                      // FIMG size == Last end offset + File image header size
            bw.Close();
        }

        public void ExtractToFolder(String dirPath, string extension = null) {
            if ( string.IsNullOrWhiteSpace(dirPath) ) {
                MessageBox.Show("Dir path + \"" + dirPath + "\" is invalid.", "Can't create directory", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (Directory.Exists(dirPath)) {
                if (Directory.GetFiles(dirPath).Length > 0) {
                    try {
                        if (dirPath.IndexOf(RomInfo.folderSuffix, StringComparison.CurrentCultureIgnoreCase) >= 0) {
                            Directory.Delete(dirPath, true);
                            Console.WriteLine("Deleted DSPRE-related folder \"" + dirPath + "\" without user confirmation.");
                        } else {
                            DialogResult d = MessageBox.Show("Directory \"" + dirPath + "\" already exists and is not empty.\n" +
                                "Do you want to delete its contents?", "Directory not empty", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                            if (d.Equals(DialogResult.Yes)) {
                                Directory.Delete(dirPath, true);
                                Console.WriteLine("Deleted non-DSPRE-related folder \"" + dirPath + "\" after user confirmation.");
                            }
                        }
                    } catch (IOException) {
                        MessageBox.Show("NARC has not been extracted.\nCan't delete directory: \n" + dirPath + "\nThis might be a temporary issue.\nMake sure no other process is using it and try again.", "Delete Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }

            if (!Directory.Exists(dirPath)) {
                try {
                    Directory.CreateDirectory(dirPath);
                    Console.WriteLine("Created NARC folder \"" + dirPath + "\".");
                } catch (IOException) {
                    MessageBox.Show("NARC has not been extracted.\nCan't create directory: \n" + dirPath + "\nThis might be a temporary issue.\nMake sure no other process is using it and try again.", "Creation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            Parallel.For(0, Elements.Length, i => {
                string path = Path.Combine(dirPath, i.ToString("D4") + (string.IsNullOrWhiteSpace(extension) ? "" : extension) );
                using (BinaryWriter wr = new BinaryWriter(File.Create(path))) {
                    long len = Elements[i].Length;
                    byte[] buffer = new byte[len];
                    Elements[i].Seek(0, SeekOrigin.Begin);
                    Elements[i].Read(buffer, 0, (int)len);
                    wr.Write(buffer);
                }
            });
        }

        public void Free() { // Libera todos los recursos de memoria asociados (cierra los streams)
            Parallel.For(0, Elements.Length, i => {
                Elements[i].Close();
            });
        }

        public MemoryStream this[int elemIndex] {
            get {
                return Elements[elemIndex];
            }
            set {
                Elements[elemIndex] = value;
            }
        }

        public int GetElementsLength() {
            return Elements.Length;
        }

        private void ReadOffsets(BinaryReader br) {
            br.BaseStream.Position = FATBNUMELEMENTSPOS;             
            FntbOffset = (int)br.ReadUInt32() * FATBELEMENTSIZE + FATBOFFSET + FATBHEADERSIZE;
            br.BaseStream.Position = FntbOffset + 0x4;               // FNTB Size, skip forward 4 bytes for Fntb file signature
            FimgOffset = (int)br.ReadUInt32() + FntbOffset;
        }

        private void ReadElements(BinaryReader br) {
            uint numberOfElements;
            uint[] startOffsets, endOffsets;
            // Create array of elements
            br.BaseStream.Position = FATBNUMELEMENTSPOS;
            Elements = new MemoryStream[numberOfElements = br.ReadUInt32()];

            // Read offsets of each element
            startOffsets = new uint[numberOfElements]; 
            endOffsets = new uint[numberOfElements];
            br.BaseStream.Position = FATBOFFSET + FATBHEADERSIZE;
            for (int i = 0; i < numberOfElements; i++) { 
                startOffsets[i] = br.ReadUInt32(); 
                endOffsets[i] = br.ReadUInt32(); 
            }
            // Read elements
            for(int i = 0; i < numberOfElements; i++) {
                br.BaseStream.Position = FimgOffset + startOffsets[i] + FIMGHEADERSIZE;
                byte[] buffer = new byte[endOffsets[i] - startOffsets[i]];
                br.Read(buffer, 0, (int)(endOffsets[i] - startOffsets[i]));
                Elements[i] = new MemoryStream(buffer);
            }
        }
    }
}
