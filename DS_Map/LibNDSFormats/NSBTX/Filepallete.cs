using System;
using System.Collections.Generic;
using System.Text;
using NSMBe4.DSFileSystem;
using System.Drawing;

namespace NSMBe4
{
    public class FilePalette : Palette
    {
        File f;
        string name;

        public FilePalette(File f)
            : this(f, f.name)
        {
        }

        public FilePalette(File f, string name)
        {
            this.f = f;
            f.beginEdit(this);
            this.name = name;

            pal = arrayToPalette(f.getContents());
            if (pal.Length != 0) 
            { 
                //pal[0] = Color.Transparent; 
            }
                
        }
        public static Color[] arrayToPalette(byte[] data)
        {
            ByteArrayInputStream ii = new ByteArrayInputStream(data);
            Color[] pal = new Color[data.Length / 2];
            for (int i = 0; i < pal.Length; i++)
            {
                pal[i] = NSMBTileset.fromRGB15(ii.readUShort());
            }
            return pal;
        }

        public override void save()
        {
            ByteArrayOutputStream oo = new ByteArrayOutputStream();
            for (int i = 0; i < pal.Length; i++)
                oo.writeUShort(NSMBTileset.toRGB15(pal[i]));

            f.replace(oo.getArray(), this);

        }
        public override byte[] getRawData()
        {
            ByteArrayOutputStream oo = new ByteArrayOutputStream();
            for (int i = 0; i < pal.Length; i++)
                oo.writeUShort(NSMBTileset.toRGB15(pal[i]));

            return oo.getArray();

        }

        public override void close()
        {
            f.endEdit(this);
        }

        public override string ToString()
        {
            return name;
        }
    }
}
