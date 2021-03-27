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

namespace NSMBe4 {
    public class ByteArrayOutputStream {
        //implements an unbonded array to store unlimited data.
        //writes in amortized constant time.

        private byte[] buf = new byte[16];
        private int pos = 0;

        public ByteArrayOutputStream() {
        }

        public int getPos() {
            return pos;
        }

        public byte[] getArray() {
            byte[] ret = new byte[pos];
            Array.Copy(buf, ret, pos);
            return ret;
        }

        public void writeByte(byte b) {
            if (buf.Length <= pos)
                grow();

            buf[pos] = b;
            pos++;
        }

        public void writeUShort(ushort u) {
            writeByte((byte)u);
            writeByte((byte)(u >> 8));
        }
        public void writeUInt(uint u) {
            writeByte((byte)u);
            writeByte((byte)(u >> 8));
            writeByte((byte)(u >> 16));
            writeByte((byte)(u >> 24));
        }

        public void writeInt(int i) {
            writeUInt((uint)i);

        }
        public void writeLong(long u) {
            writeByte((byte)u);
            writeByte((byte)(u >> 8));
            writeByte((byte)(u >> 16));
            writeByte((byte)(u >> 24));
            writeByte((byte)(u >> 32));
            writeByte((byte)(u >> 40));
            writeByte((byte)(u >> 48));
            writeByte((byte)(u >> 56));
        }

        public void align(int m) {
            while (pos % m != 0)
                writeByte(0);
        }

        private void grow() {
            byte[] nbuf = new byte[buf.Length * 2];
            Array.Copy(buf, nbuf, buf.Length);
            buf = nbuf;
        }

        public void write(byte[] ar) {
            for (int i = 0; i < ar.Length; i++)
                writeByte(ar[i]);
        }
    }
}
