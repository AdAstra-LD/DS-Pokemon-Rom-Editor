using System.Drawing;

namespace NSMBe4
{
    public abstract class PalettedImage
    {
        public string name;

        public abstract Bitmap render(Palette p);
        public abstract void replaceWithPal(Bitmap b, Palette p);
        public abstract void replaceImgAndPal(Bitmap b, Palette p);
        public bool supportsReplaceWithPal() { return true; }
        public abstract void save();
        public abstract int getWidth();
        public abstract int getHeight();
        public abstract void close();
        public int format;
        public long offset;
        public long offset5;
        public bool color0;

        //These two must return raw data representing the whole image
        //Used for undo/redo in the GraphicsEditor
        public abstract byte[] getRawData();
        public abstract byte[] getRawData5();
        public abstract void setRawData(byte[] data);

    }
}
