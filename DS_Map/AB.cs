using System;
using System.IO;

namespace AB_API
{
    public class AB
    {
        public static void Extract(string filePath, string folderPath)
        {
            BinaryReader read = new BinaryReader(File.OpenRead(filePath));
            if (read.ReadUInt16() != 0x4241)
            {
                read.Close();
                return;
            }
            int fileCount = read.ReadUInt16();
            int[] offsets = new int[fileCount];
            for (int i = 0; i < fileCount; i++)
            {
                offsets[i] = read.ReadInt32();
            }
            FileStream file;
            byte[] buffer;
            if (Directory.Exists(folderPath + "\\" + "header"))
            {
                Directory.Delete(folderPath + "\\" + "header", true);
            }
            if (Directory.Exists(folderPath + "\\" + "model"))
            {
                Directory.Delete(folderPath + "\\" + "model", true);
            }
            Directory.CreateDirectory(folderPath + "\\" + "header");
            Directory.CreateDirectory(folderPath + "\\" + "model");
            for (int i = 0; i < fileCount / 2; i++)
            {
                read.BaseStream.Position = offsets[i] + 19;
                int count = read.ReadByte();
                if (count == 0)
                {
                    file = File.Create(folderPath + "\\" + "header" + "\\" + i.ToString("D4"));
                    read.BaseStream.Position = offsets[i];
                    buffer = new byte[0x24];
                    read.Read(buffer, 0, 0x24);
                    file.Write(buffer, 0, 0x24);
                    file.Close();
                }
                else
                {
                    BinaryWriter write = new BinaryWriter(File.Create(folderPath + "\\" + "header" + "\\" + i.ToString("D4")));
                    if (count == 2) read.BaseStream.Position += 4;
                    if (count == 3) read.BaseStream.Position += 8;
                    if (count == 4) read.BaseStream.Position += 12;
                    int offset = read.ReadInt32();
                    read.BaseStream.Position = offsets[i] + 0x18 + offset;
                    int size = read.ReadInt32();
                    read.BaseStream.Position = offsets[i];
                    for (int j = 0; j < 0x10 + offset + size; j++)
                    {
                        write.Write((byte)read.ReadByte());
                    }
                    write.Close();
                }
            }
            for (int i = fileCount / 2; i < fileCount; i++)
            {
                read.BaseStream.Position = offsets[i] + 8;
                BinaryWriter write = new BinaryWriter(File.Create(folderPath + "\\" + "model" + "\\" + (i - fileCount / 2).ToString("D4")));
                int size = read.ReadInt32();
                read.BaseStream.Position = offsets[i];
                for (int j = 0; j < size; j++)
                {
                    write.Write((byte)read.ReadByte());
                }
                write.Close();
            }
            read.Close();
        }

        public static void Pack(string folderPath, string filePath)
        {
            BinaryWriter write = new BinaryWriter(File.Create(filePath));
            int count = Directory.GetFiles(folderPath + "\\" + "header").Length;
            write.Write((Int16)0x4241);
            write.Write((Int16)(count * 2));
            int offset = 0x4 + (count * 2) * 4;
            for (int i = 0; i < count; i++)
            {
                write.Write((UInt32)offset);
                offset += (int)new FileInfo(folderPath + "\\" + "header" + "\\" + i.ToString("D4")).Length;
            }
            for (int i = 0; i < count; i++)
            {
                write.Write((UInt32)offset);
                offset += (int)new FileInfo(folderPath + "\\" + "model" + "\\" + i.ToString("D4")).Length;
            }
            for (int i = 0; i < count; i++)
            {
                BinaryReader read = new BinaryReader(File.OpenRead(folderPath + "\\" + "header" + "\\" + i.ToString("D4")));
                write.Write(read.ReadBytes((int)new FileInfo(folderPath + "\\" + "header" + "\\" + i.ToString("D4")).Length));
                read.Close();
            }
            for (int i = 0; i < count; i++)
            {
                BinaryReader read = new BinaryReader(File.OpenRead(folderPath + "\\" + "model" + "\\" + i.ToString("D4")));
                write.Write(read.ReadBytes((int)new FileInfo(folderPath + "\\" + "model" + "\\" + i.ToString("D4")).Length));
                read.Close();
            }
            write.Close();
        }
    }
}
