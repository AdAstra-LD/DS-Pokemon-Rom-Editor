// CTools library - Library functions for CTools
// Copyright (C) 2010 Chadderz

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System.Text;
using System.Runtime.InteropServices;

namespace System.IO {
    public sealed class EndianBinaryReader : IDisposable {
        private bool disposed;
        private byte[] buffer;

        public Stream BaseStream { get; private set; }
        public Endianness Endianness { get; set; }
        public Endianness SystemEndianness { get { return BitConverter.IsLittleEndian ? Endianness.LittleEndian : Endianness.BigEndian; } }

        private bool Reverse { get { return SystemEndianness != Endianness; } }

        public EndianBinaryReader(Stream baseStream)
            : this(baseStream, Endianness.BigEndian) { }

        public EndianBinaryReader(Stream baseStream, Endianness endianness) {
            if (baseStream is null) throw new ArgumentNullException("baseStream");
            if (!baseStream.CanRead) throw new ArgumentException("baseStream");

            BaseStream = baseStream;
            Endianness = endianness;
        }

        ~EndianBinaryReader() {
            Dispose(false);
        }

        private void FillBuffer(int bytes, int stride) {
            if (buffer is null || buffer.Length < bytes)
                buffer = new byte[bytes];

            BaseStream.Read(buffer, 0, bytes);

            if (Reverse)
                for (int i = 0; i < bytes; i += stride) {
                    Array.Reverse(buffer, i, stride);
                }
        }

        public byte ReadByte() {
            FillBuffer(1, 1);

            return buffer[0];
        }

        public byte[] ReadBytes(int count) {
            byte[] temp;

            FillBuffer(count, 1);
            temp = new byte[count];
            Array.Copy(buffer, 0, temp, 0, count);
            return temp;
        }

        public sbyte ReadSByte() {
            FillBuffer(1, 1);

            unchecked {
                return (sbyte)buffer[0];
            }
        }

        public sbyte[] ReadSBytes(int count) {
            sbyte[] temp;

            temp = new sbyte[count];
            FillBuffer(count, 1);

            unchecked {
                for (int i = 0; i < count; i++) {
                    temp[i] = (sbyte)buffer[i];
                }
            }

            return temp;
        }

        public char ReadChar(Encoding encoding) {
            int size;

            size = GetEncodingSize(encoding);
            FillBuffer(size, size);
            return encoding.GetChars(buffer, 0, size)[0];
        }

        public char[] ReadChars(Encoding encoding, int count) {
            int size;

            size = GetEncodingSize(encoding);
            FillBuffer(size * count, size);
            return encoding.GetChars(buffer, 0, size * count);
        }

        private static int GetEncodingSize(Encoding encoding) {
            if (encoding == Encoding.UTF8 || encoding == Encoding.ASCII)
                return 1;
            else if (encoding == Encoding.Unicode || encoding == Encoding.BigEndianUnicode)
                return 2;

            return 1;
        }

        public string ReadStringNT(Encoding encoding) {
            string text;

            text = "";

            do {
                text += ReadChar(encoding);
            } while (!text.EndsWith("\0", StringComparison.Ordinal));

            return text.Remove(text.Length - 1);
        }

        public string ReadString(Encoding encoding, int count) {
            return new string(ReadChars(encoding, count));
        }

        public double ReadDouble() {
            const int size = sizeof(double);
            FillBuffer(size, size);
            return BitConverter.ToDouble(buffer, 0);
        }

        public double[] ReadDoubles(int count) {
            const int size = sizeof(double);
            double[] temp;

            temp = new double[count];
            FillBuffer(size * count, size);

            for (int i = 0; i < count; i++) {
                temp[i] = BitConverter.ToDouble(buffer, size * i);
            }
            return temp;
        }

        public Single ReadSingle() {
            const int size = sizeof(Single);
            FillBuffer(size, size);
            return BitConverter.ToSingle(buffer, 0);
        }

        public Single[] ReadSingles(int count) {
            const int size = sizeof(Single);
            Single[] temp;

            temp = new Single[count];
            FillBuffer(size * count, size);

            for (int i = 0; i < count; i++) {
                temp[i] = BitConverter.ToSingle(buffer, size * i);
            }
            return temp;
        }

        public Int32 ReadInt32() {
            const int size = sizeof(Int32);
            FillBuffer(size, size);
            return BitConverter.ToInt32(buffer, 0);
        }

        public Int32[] ReadInt32s(int count) {
            const int size = sizeof(Int32);
            Int32[] temp;

            temp = new Int32[count];
            FillBuffer(size * count, size);

            for (int i = 0; i < count; i++) {
                temp[i] = BitConverter.ToInt32(buffer, size * i);
            }
            return temp;
        }

        public Int64 ReadInt64() {
            const int size = sizeof(Int64);
            FillBuffer(size, size);
            return BitConverter.ToInt64(buffer, 0);
        }

        public Int64[] ReadInt64s(int count) {
            const int size = sizeof(Int64);
            Int64[] temp;

            temp = new Int64[count];
            FillBuffer(size * count, size);

            for (int i = 0; i < count; i++) {
                temp[i] = BitConverter.ToInt64(buffer, size * i);
            }
            return temp;
        }

        public Int16 ReadInt16() {
            const int size = sizeof(Int16);
            FillBuffer(size, size);
            return BitConverter.ToInt16(buffer, 0);
        }

        public Int16[] ReadInt16s(int count) {
            const int size = sizeof(Int16);
            Int16[] temp;

            temp = new Int16[count];
            FillBuffer(size * count, size);

            for (int i = 0; i < count; i++) {
                temp[i] = BitConverter.ToInt16(buffer, size * i);
            }
            return temp;
        }

        public ushort ReadUInt16() {
            const int size = sizeof(ushort);
            FillBuffer(size, size);
            return BitConverter.ToUInt16(buffer, 0);
        }

        public ushort[] ReadUInt16s(int count) {
            const int size = sizeof(ushort);
            ushort[] temp;

            temp = new ushort[count];
            FillBuffer(size * count, size);

            for (int i = 0; i < count; i++) {
                temp[i] = BitConverter.ToUInt16(buffer, size * i);
            }
            return temp;
        }

        public UInt32 ReadUInt32() {
            const int size = sizeof(UInt32);
            FillBuffer(size, size);
            return BitConverter.ToUInt32(buffer, 0);
        }

        public UInt32[] ReadUInt32s(int count) {
            const int size = sizeof(UInt32);
            UInt32[] temp;

            temp = new UInt32[count];
            FillBuffer(size * count, size);

            for (int i = 0; i < count; i++) {
                temp[i] = BitConverter.ToUInt32(buffer, size * i);
            }
            return temp;
        }

        public UInt64 ReadUInt64() {
            const int size = sizeof(UInt64);
            FillBuffer(size, size);
            return BitConverter.ToUInt64(buffer, 0);
        }

        public UInt64[] ReadUInt64s(int count) {
            const int size = sizeof(UInt64);
            UInt64[] temp;

            temp = new UInt64[count];
            FillBuffer(size * count, size);

            for (int i = 0; i < count; i++) {
                temp[i] = BitConverter.ToUInt64(buffer, size * i);
            }
            return temp;
        }

        public void Close() {
            Dispose();
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing) {
            if (!disposed) {
                if (disposing) {
                    if (BaseStream != null) {
                        BaseStream.Close();
                    }
                }

                buffer = null;

                disposed = true;
            }
        }
    }

    public enum Endianness {
        BigEndian,
        LittleEndian,
    }
}
