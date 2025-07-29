using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSPRE.Editors.Utils
{
    public class NarcReader
    {
        public int Entrys;

        public FileEntry[] fe;

        public FileStream fs;

        string m_sFileName;

        public long size;

        public NarcReader(string strFileName)
        {
            m_sFileName = strFileName;
            fs = new FileStream(strFileName, FileMode.Open, FileAccess.ReadWrite);
            BinaryReader binaryReader = new BinaryReader(fs);
            byte[] array = new byte[16];
            binaryReader.Read(array, 0, 16);
            size = BitConverter.ToUInt32(array, 8);
            int num = BitConverter.ToInt16(array, 12);
            fs.Seek(num, SeekOrigin.Begin);
            array = new byte[12];
            binaryReader.Read(array, 0, 12);
            int num2 = BitConverter.ToInt32(array, 4);
            Entrys = BitConverter.ToInt32(array, 8);
            fe = new FileEntry[Entrys];
            for (int i = 0; i < Entrys; i++)
            {
                fe[i].Ofs = binaryReader.ReadInt32();
                fe[i].Size = binaryReader.ReadInt32() - fe[i].Ofs;
            }
            fs.Seek(num + num2, SeekOrigin.Begin);
            array = new byte[16];
            binaryReader.Read(array, 0, 16);
            int num3 = BitConverter.ToInt32(array, 4);
            num3 = num + num3 + num2 + 8;
            for (int j = 0; j < Entrys; j++)
            {
                fe[j].Ofs += num3;
            }
            fs.Close();
        }

        public void Close()
        {
            fs.Close();
        }

        public int OpenEntry(int id)
        {
            fs = new FileStream(m_sFileName, FileMode.Open, FileAccess.ReadWrite);
            fs.Seek(fe[id].Ofs, SeekOrigin.Begin);
            return 0;
        }
    }
}
