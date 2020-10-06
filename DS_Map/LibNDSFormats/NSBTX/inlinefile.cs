using System;
using System.Collections.Generic;
using System.Text;

namespace NSMBe4.DSFileSystem
{
    public class InlineFile : File
    {
        private int inlineOffs;
        private int inlineLen;
        private File parentFile;
        private CompressionType comp;

        public enum CompressionType : int
        {
            NoComp,
            LZComp,
            LZWithHeaderComp
        }

        public InlineFile(File parent, int offs, int len, string name, Directory parentDir) :
            this(parent, offs, len, name, parentDir, CompressionType.NoComp)
        {

        }

        public InlineFile(File parent, int offs, int len, string name, Directory parentDir, CompressionType comp)
            : base(parent.parent, parentDir, parent.name + " - " + name + ":" + offs.ToString("X") + ":" + len)
        {
            parentFile = parent;
            inlineOffs = offs;
            inlineLen = len;
            this.comp = comp;
            this.fixedFile = true;
            this.canChangeOffset = false;
            refreshOffsets();
        }

        public override byte[] getContents()
        {
            if (comp != CompressionType.NoComp)
            {
                byte[] data;
                if (comp == CompressionType.LZWithHeaderComp)
                    data = ROM.LZ77_DecompressWithHeader(parentFile.getContents());
                else
                    data = ROM.LZ77_Decompress(parentFile.getContents());
                byte[] thisdata = new byte[inlineLen];
                Array.Copy(data, inlineOffs, thisdata, 0, inlineLen);
                return thisdata;
            }
            else return base.getContents();
        }

        public override void replace(byte[] newFile, object editor)
        {
            //if (!isAGoodEditor(editor))
            //    throw new Exception("NOT CORRECT EDITOR " + name);

            if (comp != CompressionType.NoComp)
            {
                byte[] data;
                if (comp == CompressionType.LZWithHeaderComp)
                    data = ROM.LZ77_DecompressWithHeader(parentFile.getContents());
                else
                    data = ROM.LZ77_Decompress(parentFile.getContents());
                Array.Copy(newFile, 0, data, inlineOffs, inlineLen);
                parentFile.replace(ROM.LZ77_Compress(data, comp == CompressionType.LZWithHeaderComp), this);
            }
            else base.replace(newFile, editor);
        }

        public override void beginEdit(object editor)
        {
            parentFile.beginEditInline(this);
            base.beginEdit(editor);
        }

        public override void endEdit(object editor)
        {
            parentFile.endEditInline(this);
            base.endEdit(editor);
        }

        public override void refreshOffsets()
        {
            fileBegin = parentFile.fileBegin + inlineOffs;
            fileSize = inlineLen;
        }

        public override void saveOffsets()
        {
        }

        public override void enableEdition()
        {
            refreshOffsets(); // In case the parent file gets moved...
            base.enableEdition();
        }
    }
}
