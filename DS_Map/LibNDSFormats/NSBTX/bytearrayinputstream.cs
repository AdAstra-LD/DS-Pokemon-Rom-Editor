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

namespace NSMBe4
{
    public class ByteArrayInputStream
    {
        private byte[] array;
        private uint pos = 0, origin = 0;
        private Stack<uint> savedPositions = new Stack<uint>();

        public ByteArrayInputStream(byte[] array)
        {
            this.array = array;
            pos = 0;
        }

        public void setOrigin(uint o)
        {
            this.origin = o;
        }

        public void savePos()
        {
            savedPositions.Push(pos);
        }

        public void loadPos()
        {
            pos = savedPositions.Pop();
        }

        public int available
        {
            get
            {
                return (int)(array.Length - pos - origin);
            }
        }

        public bool lengthAvailable(int len)
        {
            return available >= len;
        }

        public byte readByte()
        {
            return array[origin + pos++];
        }

        public void dumpAsciiData()
        {
            for (int i = 0; i < array.Length; i++)
            {
                Console.Out.Write((char)array[i]);
                if ((i % 60) == 0)
                    Console.Out.WriteLine();
            }
        }

        public void write(byte[] data)
        {
            Array.Copy(data, 0, array, pos + origin, data.Length);
            pos += (uint)data.Length;
        }

        public void writeByte(byte b)
        {
            array[origin + pos++] = b;
        }

        public void seek(uint pos)
        {
            this.pos = pos;
        }
        public void seek(int pos)
        {
            this.pos = (uint)pos;
        }

        public void skip(uint bytes)
        {
            pos += bytes;
        }

        public byte[] getData()
        {
            return array;
        }
        public void skipback(uint bytes)
        {
            pos -= bytes;
        }


        public uint getPos()
        {
            return pos;
        }

        public ushort ReadUInt16()
        {
            pos += 2;
            return (ushort)(array[pos - 2 + origin] | array[pos - 1 + origin] << 8);
        }

        public uint readUInt()
        {
            uint res = 0;
            for (int i = 0; i < 4; i++)
            {
                res |= (uint)readByte() << 8 * i;
            }
            return res;
        }
        public int readInt()
        {
            uint res = 0;
            for (int i = 0; i < 4; i++)
            {
                res |= (uint)readByte() << 8 * i;
            }
            return (int)res;
        }

        public long readLong()
        {
            long res = 0;
            for (int i = 0; i < 8; i++)
            {
                res |= (long)readByte() << 8 * i;
            }
            return res;
        }

        public void read(byte[] dest)
        {
            Array.Copy(array, pos + origin, dest, 0, dest.Length);
            pos += (uint)(dest.Length);
        }

        public bool end()
        {
            return pos + origin >= array.Length;
        }

        public string ReadString(int l)
        {
            if (l == 0) return ""; // simple error checking

            byte[] arr = new byte[l];
            read(arr);

            StringBuilder NewStr = new StringBuilder(l);
            for (int i = 0; i < l; i++)
                if (arr[i] != 0)
                    NewStr.Append((char)arr[i]);

            return NewStr.ToString().Trim();
        }
    }
}
