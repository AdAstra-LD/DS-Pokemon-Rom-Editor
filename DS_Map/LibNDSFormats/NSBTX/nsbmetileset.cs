/*
*   This file is part of NSMB Editor 5.
*
*   NSMB Editor 5 is free software: you can redistribute it and/or modify
*   it under the terms of the GNU General Public License as published by
*   the Free Software Foundation, either version 3 of the License, or
*   (at your option) any later version.
*
*   NSMB Editor 5 is distributed in the hope that it will be useful,
*   but WITHOUT ANY WARRANTY; without even the implied warranty of
*   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
*   GNU General Public License for more details.
*
*   You should have received a copy of the GNU General Public License
*   along with NSMB Editor 5.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

/**
 * 
 * TILESET FORMAT DOCS:
 * 
 * Graphics file:
 *  - LZ Compressed
 *  - 8bpp 8x8 tiles
 *  
 * Palette file:
 *  - LZ Compressed
 *  - 512 entries
 *  - 2-byte entries in RGB15 format
 *  - 0 and 256 transparent
 *  - 2 different palettes: 0-255 and 256-511
 *  
 * Map16 file:
 *  - Groups 8x8 tiles into 16x16 tiles
 *  - 8-byte per 16x16 tile
 *  - order: top-left, top-right, bottom-left, bottom-right
 *  - 2-byte per tile.
 *  
 * Object index file:
 *  - Defines offsets in the Object file
 *  - 4 bytes per object
 *    - Offset as ushort
 *    - Width and Height as 2 bytes. 
 *      These are unused by the game, and are inaccurate for slopes,
 *      so my implementation doesn't use them
 *  
 * Object file:
 *  - Data for each object.
 *  
 * 0xFF - End of object
 * 0xFE - New Line
 * 0x8x - Slope Control Byte
 * Else, its the beginning of a tile:
 *   Control Byte
 *   Map16 tile number as ushort
 *   
 * STANDARD OBJECTS:
 * 
 * Tile control byte:
 *  1 = horizontal repeat
 *  2 = vertical repeat
 *  
 * If no repeats, repeat all.
 * If there are repeats, divide everyting in 3:
 *    Before repeat (no repeat set)
 *    In repeat     (repeat set)
 *    After repeat  (no repeat set)
 *    
 * Then put the before repeat at the beginning.
 * The after repeat at the end
 * And then fill the space between them (if any) repeating the In repeat tiles.
 * 
 * SLOPED OBJECTS:
 * 
 * These objects are a pain to work with.
 * 
 * The first slope control byte defines the direction of slope:
 * 
 *  & with 1 -> Go left
 *  & with 2 -> Upside-down slope
 *   
 * 
 * The slope format is:
 * 
 * A slope control indicating it's a slope and its direction.
 * Then follows a rectangular block of tiles. These have to be placed
 * corner-by-corner, respecting their size, like this:
 * 
 *              _|_|
 *      _      | |             __
 *    _|_|    _|_|          __|__|
 *  _|_|     | |         __|__|
 * |_|       |_|        |__|
 * 
 * The first corner must be placed on a corner of the object, on the opposite
 * side of the direction (if slope goes right, start is at left), and at the 
 * bottom, or at the top if its an upside-down slope.
 * 
 * Optional: Then follows a 0x85 slope control, then another block of tiles 
 * that has to be placed under the previous blocks, or OVER if its an upside-down slope.
 * 
 * If the slope goes right the blocks have to be left-aligned:
 *  _
 * |_|__  main block
 * |____| sub (0x85) block
 * 
 * If the slope goes left the blocks have to be right-aligned:
 *     _
 *  __|_|
 * |____|
 * 
 * EXAMPLE: Slope going up right with 1x1 main block and 2x1 sub block in a 6x5 obj
 *        _ _
 *      _|_|_|
 *    _|_|___|
 *  _|_|___|
 * |_|___| 
 * 
 * NOTE: This info is not complete. This works for all the slopes used in-game, but
 * there are some unused bits that change their behavior:
 * 
 * -0x04 control byte in 0x85 slope block: All slopes that have the 0x85 block have
 * all its tiles with an 0x04 control byte. 
 * 
 * IF ITS NOT SET, then the 0x85 block is used to fill all the area below 
 * (or over if its upside down???) the slope. Not
 * sure how does it behave if the 0x85 block has multiple tiles.
 * Probably the Nintendo Guys thought it was best to have it like that, and then
 * realized that it caused the triangle below the slope to be unusable (filled) 
 * and then created a new mode.
 * NOTE: The editor doesn't implement this.
 * 
 * Not sure if there's more like this...
 **/


namespace NSMBe4
{

    public class NSMBTileset
    {
        //private bool editing = false;

        /*public File GFXFile;
        public File PalFile;
        public File Map16File;
        public File ObjFile;
        public File ObjIndexFile;
        public File TileBehaviorFile;*/

        public int TilesetNumber; // 0 for Jyotyu, 1 for Normal, 2 for SubUnit
        public int Map16TileOffset
        {
            get
            {
                if (TilesetNumber == 1)
                    return 192;
                else if (TilesetNumber == 2)
                    return 640;
                else
                    return 0;
            }
        }

        public int ObjectDefTileOffset
        {
            get
            {
                if (TilesetNumber == 1)
                    return 1;
                else if (TilesetNumber == 2)
                    return 4;
                else
                    return 0;
            }
        }

        public int Map16PaletteOffset
        {
            get
            {
                if (TilesetNumber == 0)
                    return 0;
                if (TilesetNumber == 1)
                    return 2;
                else
                    return 6;
            }
        }

        public Bitmap Map16Buffer;
        public bool UseOverrides;
        public Bitmap OverrideBitmap;

        public short[] Overrides;
        public short[] EditorOverrides;

        public bool UseNotes;
        public string[] ObjNotes;

        public ObjectDef[] Objects;

        //public Map16Tile[] Map16;

        public byte[][] TileBehaviors;

        public Bitmap TilesetBuffer;

        public Color[] Palette;
        public byte[] RawGFXData;

        //private Graphics Map16Graphics;

        /*public NSMBTileset(File GFXFile, File PalFile, File Map16File, File ObjFile, File ObjIndexFile, File TileBehaviorFile, bool OverrideFlag, int TilesetNumber)
        {
            this.GFXFile = GFXFile;
            this.PalFile = PalFile;
            this.Map16File = Map16File;
            this.ObjFile = ObjFile;
            this.ObjIndexFile = ObjIndexFile;
            this.TileBehaviorFile = TileBehaviorFile;

            //Console.Out.WriteLine(ROM.FileNames[TileBehaviorFile]);

            this.TilesetNumber = TilesetNumber;

            Console.Out.WriteLine("Load Tileset: " + GFXFile + ", " + PalFile + ", " + Map16File + ", " + ObjFile + ", " + ObjIndexFile);

            // First get the palette out
            byte[] ePalFile = ROM.LZ77_Decompress(PalFile.getContents());
            Palette = new Color[512];

            for (int PalIdx = 0; PalIdx < 512; PalIdx++)
            {
                Palette[PalIdx] = fromRGB15((ushort)(ePalFile[PalIdx * 2] + (ePalFile[(PalIdx * 2) + 1] << 8)));
            }

            //Palette[0] = Color.Fuchsia;
            //Palette[256] = Color.Fuchsia;
            Palette[0] = Color.LightSlateGray;
            Palette[256] = Color.LightSlateGray;

            // Load graphics
            SetGraphics(ROM.LZ77_Decompress(GFXFile.getContents()));

            loadMap16();
            loadTileBehaviors();

            // Finally the object file.
            loadObjects();

            // Finally, load overrides
            if (OverrideFlag)
            {
                UseOverrides = true;
                OverrideBitmap = Properties.Resources.tileoverrides;

                Overrides = new short[Map16.Length];
                EditorOverrides = new short[Map16.Length];

                for (int idx = 0; idx < Map16.Length; idx++)
                {
                    Overrides[idx] = -1;
                    EditorOverrides[idx] = -1;
                }
            }
        }*/

        /*public void close()
        {
            if (!editing)
                return;
            editing = false;

            GFXFile.endEdit(this);
            PalFile.endEdit(this);
            Map16File.endEdit(this);
            ObjFile.endEdit(this);
            ObjIndexFile.endEdit(this);
            if (TileBehaviorFile != null)
                TileBehaviorFile.endEdit(this);
        }*/

        /*~NSMBTileset()
        {
            if (editing)
                close();
        }*/

        /*public void enableWrite()
        {
            editing = true;

            GFXFile.beginEdit(this);
            PalFile.beginEdit(this);
            Map16File.beginEdit(this);
            ObjFile.beginEdit(this);
            ObjIndexFile.beginEdit(this);
            if (TileBehaviorFile != null)
                TileBehaviorFile.beginEdit(this);
        }*/

        /*public void save()
        {
            saveObjects();
            saveMap16();
            saveTileBehaviors();

            byte[] CompGFXData = ROM.LZ77_Compress(RawGFXData);
            GFXFile.replace(CompGFXData, this);

            savePalette();
        }*/

        /*public void SetGraphics(byte[] GFXData)
        {
            RawGFXData = GFXData;
            int TileCount = GFXData.Length / 64;
            TilesetBuffer = new Bitmap(TileCount * 8, 16);

            int FilePos = 0;
            for (int TileIdx = 0; TileIdx < TileCount; TileIdx++)
            {
                int TileSrcX = TileIdx * 8;
                for (int TileY = 0; TileY < 8; TileY++)
                {
                    for (int TileX = 0; TileX < 8; TileX++)
                    {
                        TilesetBuffer.SetPixel(TileSrcX + TileX, TileY, Palette[GFXData[FilePos]]);
                        TilesetBuffer.SetPixel(TileSrcX + TileX, TileY + 8, Palette[GFXData[FilePos] + 256]);
                        FilePos++;
                    }
                }
            }
        }*/

        /*public void ResetGraphics(byte[] d)
        {
            SetGraphics(d);
            repaintAllMap16();
        }*/

        public static ushort toRGB15(Color c)
        {
            byte r = (byte)(c.R >> 3);
            byte g = (byte)(c.G >> 3);
            byte b = (byte)(c.B >> 3);

            ushort val = 0;

            val |= r;
            val |= (ushort)(g << 5);
            val |= (ushort)(b << 10);
            return val;
        }

        public static Color fromRGB15(ushort c)
        {
            int cR = (c & 31) * 8;
            int cG = ((c >> 5) & 31) * 8;
            int cB = ((c >> 10) & 31) * 8;
            return Color.FromArgb(cR, cG, cB);
        }

        public static byte[] paletteToRawData(Color[] pal)
        {
            ByteArrayOutputStream file = new ByteArrayOutputStream();
            for (int i = 0; i < pal.Length; i++)
            {
                file.writeUShort(toRGB15(pal[i]));
            }
            return file.getArray();
        }

        /*private void savePalette()
        {
            byte[] data = paletteToRawData(Palette);
            PalFile.replace(ROM.LZ77_Compress(data), this);
        }*/


        /*#region Tile Behaviors
        private void loadTileBehaviors()
        {
            byte[] tileBehaviorsFile = null;

            if (TilesetNumber == 0)
            {
                tileBehaviorsFile = ROM.GetInlineFile(ROM.Data.File_Jyotyu_CHK);
            }
            else if (TilesetNumber == 1 || TilesetNumber == 2)
            {
                tileBehaviorsFile = TileBehaviorFile.getContents();
            }

            if (tileBehaviorsFile != null)
            {
                TileBehaviors = new byte[Map16.Length][];
                for (int i = 0; i < Map16.Length; i++)
                {
                    TileBehaviors[i] = new byte[4];
                    Array.Copy(tileBehaviorsFile, i * 4, TileBehaviors[i], 0, 4);
                }
            }
        }*/

        /*private void saveTileBehaviors()
        {
            ByteArrayOutputStream file = new ByteArrayOutputStream();
            for (int i = 0; i < Map16.Length; i++)
                file.write(TileBehaviors[i]);

            if (TilesetNumber == 0)
            {
                ROM.ReplaceInlineFile(ROM.Data.File_Jyotyu_CHK, file.getArray());
            }
            else if (TilesetNumber == 1 || TilesetNumber == 2)
            {
                TileBehaviorFile.replace(file.getArray(), this);
            }
        }

        #endregion
         */
        #region Map16

        /*private void saveMap16()
        {
            ByteArrayOutputStream file = new ByteArrayOutputStream();
            foreach (Map16Tile t in Map16)
                t.save(file);

            Map16File.replace(file.getArray(), this);
        }*/

        /*private void loadMap16()
        {
            // Load Map16
            ByteArrayInputStream eMap16File = new ByteArrayInputStream(Map16File.getContents());
            int Map16Count = (int)eMap16File.available() / 8;
            Map16 = new Map16Tile[Map16Count];

            Map16Buffer = new Bitmap(Map16Count * 16, 16, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

            Map16Graphics = Graphics.FromImage(Map16Buffer);


            for (int Map16Idx = 0; Map16Idx < Map16Count; Map16Idx++)
            {
                Map16[Map16Idx] = new Map16Tile(eMap16File, this);
            }
            repaintAllMap16();
        }*/


        /*public void repaintAllMap16()
        {
            if (Map16Graphics != null)
                Map16Graphics.Clear(Color.LightSlateGray);
            for (int Map16Idx = 0; Map16Idx < Map16.Length; Map16Idx++)
                RenderMap16Tile(Map16Idx);
        }*/

        /*private void RenderMap16Quarter(Map16Quarter q, int x, int y)
        {
            int TileNum = q.TileNum;

            Rectangle SrcRect = new Rectangle(TileNum * 8, q.secondPalette ? 8 : 0, 8, 8);
            Rectangle DestRect = new Rectangle(x, y, 8, 8);
            Map16Graphics.FillRectangle(Brushes.LightSlateGray, DestRect);

            if (q.TileByte != 0 || q.ControlByte != 0)
            {
                Bitmap tile = new Bitmap(8, 8);
                Graphics g = Graphics.FromImage(tile);
                g.DrawImage(TilesetBuffer, new Rectangle(0, 0, 8, 8), SrcRect, GraphicsUnit.Pixel);
                if ((q.ControlByte & 4) != 0)
                    tile.RotateFlip(RotateFlipType.RotateNoneFlipX);
                if ((q.ControlByte & 8) != 0)
                    tile.RotateFlip(RotateFlipType.RotateNoneFlipY);
                Map16Graphics.DrawImage(tile, DestRect, new Rectangle(0, 0, 8, 8), GraphicsUnit.Pixel);
            }
        }*/

        /*public void RenderMap16Tile(int Map16Idx)
        {
            Map16Tile t = Map16[Map16Idx];
            int x = Map16Idx * 16;
            RenderMap16Quarter(t.topLeft, x, 0);
            RenderMap16Quarter(t.topRight, x + 8, 0);
            RenderMap16Quarter(t.bottomLeft, x, 8);
            RenderMap16Quarter(t.bottomRight, x + 8, 8);

            if (UseOverrides && Overrides[Map16Idx] > -1)
            {
                Map16Graphics.DrawImage(OverrideBitmap, new Rectangle(Map16Idx << 4, 0, 16, 16), new Rectangle(Overrides[Map16Idx] << 4, 0, 16, 16), GraphicsUnit.Pixel);
            }

        }*/

        /*public class Map16Tile
        {
            public Map16Quarter topLeft, topRight, bottomLeft, bottomRight;
            NSMBTileset t;
            public Map16Tile(NSMBTileset t)
            {
                this.t = t;
            }
            public Map16Tile(ByteArrayInputStream inp, NSMBTileset t)
            {
                this.t = t;
                topLeft = new Map16Quarter(inp, t);
                topRight = new Map16Quarter(inp, t);
                bottomLeft = new Map16Quarter(inp, t);
                bottomRight = new Map16Quarter(inp, t);
            }

            public void save(ByteArrayOutputStream outp)
            {
                topLeft.save(outp);
                topRight.save(outp);
                bottomLeft.save(outp);
                bottomRight.save(outp);
            }

            public void makeEmpty()
            {
                topLeft.ControlByte = 0;
                topLeft.TileByte = 0;
                topRight.ControlByte = 0;
                topRight.TileByte = 0;
                bottomLeft.ControlByte = 0;
                bottomLeft.TileByte = 0;
                bottomRight.ControlByte = 0;
                bottomRight.TileByte = 0;
            }
        }*/

        /*public class Map16Quarter
        {
            private NSMBTileset t;

            private byte ControlByteF;

            public byte ControlByte
            {
                get { return ControlByteF; }
                set { ControlByteF = value; }
            }
            private byte TileByteF;
            public byte TileByte
            {
                get { return TileByteF; }
                set { TileByteF = value; }
            }

            public int TileNum
            {
                get
                {
                    if (TileByte == 0 && ControlByte == 0)
                        return -1;

                    return (TileByteF | ((ControlByte & 3) << 8)) - t.Map16TileOffset;
                }
                set
                {
                    if (value == -1)
                    {
                        ControlByte = 0;
                        TileByte = 0;
                        return;
                    }

                    value += t.Map16TileOffset;

                    TileByteF = (byte)(value % 256);
                    ControlByte &= 0xFF ^ 3;
                    ControlByte |= (byte)((value >> 8) & 3);
                }
            }*/

            /*public bool secondPalette
            {
                get
                {
                    return (ControlByte >> 4) % 2 == 1;
                }
                set
                {
                    ControlByte &= 0xF;
                    int num = t.Map16PaletteOffset;
                    if (value) num++;
                    ControlByte |= (byte)(num << 4);
                }
            }*/

            /*public bool xFlip
            {
                get { return (ControlByte & 4) != 0; }
                set { if (((ControlByte & 4) != 0) != value) ControlByte ^= 4; }
            }
            public bool yFlip
            {
                get { return (ControlByte & 8) != 0; }
                set { if (((ControlByte & 8) != 0) != value) ControlByte ^= 8; }
            }

            public Map16Quarter(NSMBTileset t)
            {
                this.t = t;
            }
            public Map16Quarter(ByteArrayInputStream inp, NSMBTileset t)
            {
                this.t = t;
                TileByteF = inp.readByte();
                ControlByteF = inp.readByte();
            }

            public void save(ByteArrayOutputStream outp)
            {
                outp.writeByte(TileByteF);
                outp.writeByte(ControlByteF);
            }*/
        }

        /*public void removeUnusedMap16()
        {
            for (int i = 0; i < Map16.Length; i++)
            {
                if (!isMap16Used(i))
                    emptyMap16Tile(Map16[i]);
            }
        }

        private bool isMap16Used(int tile)
        {
            foreach (ObjectDef o in Objects)
                if (o != null)
                    foreach (List<ObjectDefTile> row in o.tiles)
                        foreach (ObjectDefTile t in row)
                            if (t.tileID == tile)
                                return true;
            return false;
        }

        private void emptyMap16Tile(Map16Tile t)
        {
            t.topLeft = new Map16Quarter(this);
            t.topRight = new Map16Quarter(this);
            t.bottomLeft = new Map16Quarter(this);
            t.bottomRight = new Map16Quarter(this);
        } */

        #endregion
        
        #region Objects
        public class ObjectDef
        {
            public List<List<ObjectDefTile>> tiles;
            public int width, height; //these are useless, but I keep them
            //in case the game uses them.
            private NSMBTileset t;

            public ObjectDef(NSMBTileset t)
            {
                this.t = t;
                tiles = new List<List<ObjectDefTile>>();
                List<ObjectDefTile> row = new List<ObjectDefTile>();
                tiles.Add(row);
            }

            /*public ObjectDef(byte[] data, NSMBTileset t)
            {
                this.t = t;
                load(new ByteArrayInputStream(data));
            }*/

            /*public void load(ByteArrayInputStream inp)
            {
                tiles = new List<List<ObjectDefTile>>();
                List<ObjectDefTile> row = new List<ObjectDefTile>();

                while (true)
                {
                    ObjectDefTile t = new ObjectDefTile(inp, this.t);
                    if (t.lineBreak)
                    {
                        tiles.Add(row);
                        row = new List<ObjectDefTile>();
                    }
                    else if (t.objectEnd)
                        break;
                    else
                        row.Add(t);
                }
            }*/

            public void save(ByteArrayOutputStream outp)
            {
                foreach (List<ObjectDefTile> row in tiles)
                {
                    foreach (ObjectDefTile t in row)
                        t.write(outp);
                    outp.writeByte(0xFE); //new line
                }
                outp.writeByte(0xFF); //end object
            }
        }

        public class ObjectDefTile
        {
            public int tileID;
            public byte controlByte;
            private NSMBTileset t;

            public bool emptyTile
            {
                get { return tileID == -1; }
            }

            public bool lineBreak
            {
                get { return controlByte == 0xFE; }
            }

            public bool objectEnd
            {
                get { return controlByte == 0xFF; }
            }

            public bool slopeControl
            {
                get { return (controlByte & 0x80) != 0; }
            }

            public bool controlTile
            {
                get { return lineBreak || objectEnd || slopeControl; }
            }

            public ObjectDefTile(NSMBTileset t) { this.t = t; }
            /*public ObjectDefTile(ByteArrayInputStream inp, NSMBTileset t)
            {
                this.t = t;
                controlByte = inp.readByte();

                if (!controlTile)
                {
                    byte a, b;
                    a = inp.readByte();
                    b = inp.readByte();

                    tileID = a + ((b - t.ObjectDefTileOffset) << 8);

                    if ((controlByte & 64) != 0) //OVERRIDES
                        tileID += 768;
                    if (a == 0 && b == 0)
                        tileID = -1;
                }
            }*/

            public void write(ByteArrayOutputStream outp)
            {
                if (controlTile)
                    outp.writeByte(controlByte);
                else if (emptyTile)
                {
                    outp.writeByte(0);
                    outp.writeByte(0);
                    outp.writeByte(0);
                }
                else
                {
                    outp.writeByte(controlByte);
                    outp.writeByte((byte)(tileID % 256));
                    outp.writeByte((byte)(tileID / 256 + t.ObjectDefTileOffset));
                }
            }
        }

        /*public void loadObjects()
        {
            ByteArrayInputStream eObjIndexFile = new ByteArrayInputStream(ObjIndexFile.getContents());
            ByteArrayInputStream eObjFile = new ByteArrayInputStream(ObjFile.getContents());

            Objects = new ObjectDef[256];

            //read object index
            int obj = 0;
            while (eObjIndexFile.available(4) && obj < Objects.Length)
            {
                Objects[obj] = new ObjectDef(this);
                int offset = eObjIndexFile.ReadUInt16();
                Objects[obj].width = eObjIndexFile.readByte();
                Objects[obj].height = eObjIndexFile.readByte();

                eObjFile.seek(offset);
                Objects[obj].load(eObjFile);
                obj++;
            }
        }

        public void saveObjects()
        {
            ByteArrayOutputStream eObjIndexFile = new ByteArrayOutputStream();
            ByteArrayOutputStream eObjFile = new ByteArrayOutputStream();

            for (int i = 0; i < Objects.Length; i++)
            {
                if (Objects[i] is null)
                    break;

                eObjIndexFile.writeUShort((ushort)eObjFile.getPos());
                eObjIndexFile.writeByte((byte)Objects[i].width);
                eObjIndexFile.writeByte((byte)Objects[i].height);
                Objects[i].save(eObjFile);
            }

            ObjFile.replace(eObjFile.getArray(), this);
            ObjIndexFile.replace(eObjIndexFile.getArray(), this);
        }

        public int[,] RenderObject(int ObjNum, int Width, int Height)
        {
            // First allocate an array
            int[,] Dest = new int[Width, Height];

            // Non-existent objects can just be made out of 0s
            if (ObjNum >= Objects.Length || ObjNum < 0 || Objects[ObjNum] is null)
                return Dest;

            ObjectDef obj = Objects[ObjNum];

            if (Objects[ObjNum].tiles.Count == 0)
                return Dest;

            // Diagonal objects are rendered differently
            if ((Objects[ObjNum].tiles[0][0].controlByte & 0x80) != 0)
            {
                RenderDiagonalObject(Dest, obj, Width, Height);
            }
            else
            {
                bool repeatFound = false;
                List<List<ObjectDefTile>> beforeRepeat = new List<List<ObjectDefTile>>();
                List<List<ObjectDefTile>> inRepeat = new List<List<ObjectDefTile>>();
                List<List<ObjectDefTile>> afterRepeat = new List<List<ObjectDefTile>>();

                foreach (List<ObjectDefTile> row in obj.tiles)
                {
                    if (row.Count == 0)
                        continue;

                    if ((row[0].controlByte & 2) != 0)
                    {
                        repeatFound = true;
                        inRepeat.Add(row);
                    }
                    else
                    {
                        if (repeatFound)
                            afterRepeat.Add(row);
                        else
                            beforeRepeat.Add(row);
                    }
                }

                for (int y = 0; y < Height; y++)
                {
                    if (inRepeat.Count == 0) //if no repeat data, just repeat all
                        renderStandardRow(Dest, beforeRepeat[y % beforeRepeat.Count], y, Width);
                    else if (y < beforeRepeat.Count) //if repeat data
                        renderStandardRow(Dest, beforeRepeat[y], y, Width);
                    else if (y > Height - afterRepeat.Count - 1)
                        renderStandardRow(Dest, afterRepeat[y - Height + afterRepeat.Count], y, Width);
                    else
                        renderStandardRow(Dest, inRepeat[(y - beforeRepeat.Count) % inRepeat.Count], y, Width);
                }

            }
            return Dest;
        }*/

        /*private void renderStandardRow(int[,] Dest, List<ObjectDefTile> row, int y, int width)
        {
            bool repeatFound = false;
            List<ObjectDefTile> beforeRepeat = new List<ObjectDefTile>();
            List<ObjectDefTile> inRepeat = new List<ObjectDefTile>();
            List<ObjectDefTile> afterRepeat = new List<ObjectDefTile>();

            foreach (ObjectDefTile tile in row)
            {
                if ((tile.controlByte & 1) != 0)
                {
                    repeatFound = true;
                    inRepeat.Add(tile);
                }
                else
                {
                    if (repeatFound)
                        afterRepeat.Add(tile);
                    else
                        beforeRepeat.Add(tile);
                }
            }

            for (int x = 0; x < width; x++)
            {
                if (inRepeat.Count == 0) //if no repeat data, just repeat all
                    Dest[x, y] = beforeRepeat[x % beforeRepeat.Count].tileID;
                else if (x < beforeRepeat.Count) //if repeat data
                    Dest[x, y] = beforeRepeat[x].tileID;
                else if (x > width - afterRepeat.Count - 1)
                    Dest[x, y] = afterRepeat[x - width + afterRepeat.Count].tileID;
                else
                    Dest[x, y] = inRepeat[(x - beforeRepeat.Count) % inRepeat.Count].tileID;
            }
        }

        private void RenderDiagonalObject(int[,] Dest, ObjectDef obj, int width, int height)
        {
            //empty tiles fill
            for (int xp = 0; xp < width; xp++)
                for (int yp = 0; yp < height; yp++)
                    Dest[xp, yp] = -1;

            //get sections
            List<ObjectDefTile[,]> sections = getSlopeSections(obj);
            ObjectDefTile[,] mainBlock = sections[0];
            ObjectDefTile[,] subBlock = null;
            if (sections.Count > 1)
                subBlock = sections[1];

            byte controlByte = obj.tiles[0][0].controlByte;

            //get direction
            bool goLeft = (controlByte & 1) != 0;
            bool goDown = (controlByte & 2) != 0;

            //get starting point

            int x = 0;
            int y = 0;
            if (!goDown)
                y = height - mainBlock.GetLength(1);
            if (goLeft)
                x = width - mainBlock.GetLength(0);

            //get increments
            int xi = mainBlock.GetLength(0);
            if (goLeft)
                xi = -xi;

            int yi = mainBlock.GetLength(1);
            if (!goDown)
                yi = -yi;

            //this is a strange stop condition.
            //Put tells if we have put a tile in the destination
            //When we don't put any tile in destination we are completely
            //out of bounds, so stop.
            bool put = true;
            while (put)
            {
                put = false;
                putArray(Dest, x, y, mainBlock, width, height, ref put);
                if (subBlock != null)
                {
                    int xb = x;
                    if (goLeft) // right align
                        xb = x + mainBlock.GetLength(0) - subBlock.GetLength(0);
                    if (goDown)
                        putArray(Dest, xb, y - subBlock.GetLength(1), subBlock, width, height, ref put);
                    else
                        putArray(Dest, xb, y + mainBlock.GetLength(1), subBlock, width, height, ref put);
                }
                x += xi;
                y += yi;
            }
        }

        private void putArray(int[,] Dest, int xo, int yo, ObjectDefTile[,] block, int width, int height, ref bool put)
        {
            for (int x = 0; x < block.GetLength(0); x++)
                for (int y = 0; y < block.GetLength(1); y++)
                    putTile(Dest, x + xo, y + yo, width, height, block[x, y], ref put);
        }

        private List<ObjectDefTile[,]> getSlopeSections(ObjectDef d)
        {
            List<ObjectDefTile[,]> sections = new List<ObjectDefTile[,]>();
            List<List<ObjectDefTile>> currentSection = null;

            foreach (List<ObjectDefTile> row in d.tiles)
            {
                if (row.Count > 0 && row[0].slopeControl) // begin new section
                {
                    if (currentSection != null)
                        sections.Add(createSection(currentSection));
                    currentSection = new List<List<ObjectDefTile>>();
                }
                currentSection.Add(row);
            }
            if (currentSection != null) //end last section
                sections.Add(createSection(currentSection));

            return sections;
        }

        private ObjectDefTile[,] createSection(List<List<ObjectDefTile>> tiles)
        {
            //calculate width
            int width = 0;
            foreach (List<ObjectDefTile> row in tiles)
            {
                int thiswidth = countTiles(row);
                if (width < thiswidth)
                    width = thiswidth;
            }

            //allocate array
            ObjectDefTile[,] section = new ObjectDefTile[width, tiles.Count];
            for (int y = 0; y < tiles.Count; y++)
            {
                int x = 0;
                foreach (ObjectDefTile t in tiles[y])
                    if (!t.controlTile)
                    {
                        section[x, y] = t;
                        x++;
                    }
            }

            return section;
        }

        private int countTiles(List<ObjectDefTile> l)
        {
            int res = 0;
            foreach (ObjectDefTile t in l)
                if (!t.controlTile)
                    res++;
            return res;
        }

        private void putTile(int[,] Dest, int x, int y, int width, int height, ObjectDefTile t, ref bool put)
        {
            if (x >= 0 && x < width)
                if (y >= 0 && y < height)
                {
                    put = true;
                    if (t != null)
                        Dest[x, y] = t.tileID;
                }
        }


        public bool objectExists(int objNum)
        {
            if (objNum < 0) return false;
            if (objNum >= Objects.Length) return false;
            if (Objects[objNum] is null) return false;
            return true;
        }
        #endregion
        #region Import / Export GFX
        public void ExportGFX(string filename)
        {
            int tileCount = TilesetBuffer.Width / 8;
            int[] tilesUsed = new int[tileCount];

            foreach (Map16Tile t in Map16)
                checkMap16UsedTiles(t, tilesUsed);

            Bitmap b = new Bitmap(256, tileCount / (256 / 8) * 8);
            for (int i = 0; i < tileCount; i++)
            {
                if (tilesUsed[i] == 0) continue;

                int tx = (i % 32) * 8;
                int ty = (int)(i / 32) * 8;
                int palOffs = 0;
                if (tilesUsed[i] == 2) palOffs = 256;

                for (int x = 0; x < 8; x++)
                    for (int y = 0; y < 8; y++)
                    {
                        byte color = RawGFXData[i * 64 + y * 8 + x];
                        Color col = Palette[palOffs + color];
                        if (color == 0)
                            col = Color.Transparent;
                        b.SetPixel(tx + x, ty + y, col);
                    }
            }
            b.Save(filename, ImageFormat.Png);
            b.Dispose();
        }

        public void ImportGFX(string filename, bool newInSecondPal)
        {
            int tileCount = TilesetBuffer.Width / 8;
            int[] tilesUsed = new int[tileCount];

            foreach (Map16Tile t in Map16)
                checkMap16UsedTiles(t, tilesUsed);

            Bitmap img = new Bitmap(filename, true);
            //            new ImagePreviewer(img).Show();

            Bitmap a = new Bitmap(tileCount * 8, 8);
            Graphics ga = Graphics.FromImage(a);
            ga.Clear(Color.Transparent);

            Bitmap b = new Bitmap(tileCount * 8, 8);
            Graphics gb = Graphics.FromImage(b);
            gb.Clear(Color.Transparent);

            for (int i = 0; i < tileCount; i++)
            {
                int tx = (i % 32) * 8;
                int ty = (int)(i / 32) * 8;

                Graphics dest = ga;
                if (newInSecondPal) dest = gb;
                if (tilesUsed[i] == 1) dest = ga;
                if (tilesUsed[i] == 2) dest = gb;

                dest.DrawImage(img, new Rectangle(i * 8, 0, 8, 8), tx, ty, 8, 8, GraphicsUnit.Pixel);
            }

            ImageIndexer ia = new ImageIndexer(a);
            ImageIndexer ib = new ImageIndexer(b);
            Array.Copy(ia.palette, 1, Palette, 1, 255);
            Array.Copy(ib.palette, 1, Palette, 257, 255);
            //            Array.Copy(ia.palettedImage, RawGFXData, RawGFXData.Length);

            for (int i = 0; i < tileCount; i++)
            {
                int tx = (i % 32) * 8;
                int ty = (int)(i / 32) * 8;

                ImageIndexer src = ia;
                if (newInSecondPal) src = ib;
                if (tilesUsed[i] == 1) src = ia;
                if (tilesUsed[i] == 2) src = ib;

                Array.Copy(src.palettedImage, i * 64, RawGFXData, i * 64, 64);
            }

            img.Dispose();
            ResetGraphics(RawGFXData);
        }*/

        /*private void checkMap16UsedTiles(Map16Tile t, int[] tilesUsed)
        {
            checkMap16UsedQuarter(t.topLeft, tilesUsed);
            checkMap16UsedQuarter(t.topRight, tilesUsed);
            checkMap16UsedQuarter(t.bottomLeft, tilesUsed);
            checkMap16UsedQuarter(t.bottomRight, tilesUsed);
        }

        private void checkMap16UsedQuarter(Map16Quarter q, int[] tilesUsed)
        {
            if (q.TileNum < 0) return;
            if (q.TileNum >= tilesUsed.Length) return;

            if (q.secondPalette)
                tilesUsed[q.TileNum] |= 2;
            else
                tilesUsed[q.TileNum] |= 1;
        }
*/
        #endregion
    }
