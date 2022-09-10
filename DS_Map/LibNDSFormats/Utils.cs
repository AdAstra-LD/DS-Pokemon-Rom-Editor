// PG4Map - a 4th Gen Pokemon Map Viewer.
// Utility class.
//
// Copyright (C) 2010 SentryAlphaOmega
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

using System;
using System.IO;

namespace LibNDSFormats {
    /// <summary>
    /// Utility class.
    /// </summary>
    public static class Utils {
        #region Methods (1) 

        // Public Methods (1) 

        /// <summary>
        /// Read string in file.
        /// </summary>
        /// <param name="reader">Binary reader to use.</param>
        /// <returns>Trimmed string.</returns>
        public static string ReadNSBMDString(EndianBinaryReader reader) {
            var str = new String(reader.ReadChars(System.Text.Encoding.ASCII, 16));
            str = str.Replace("\0", "");

            return str;
        }

        /// <summary>
        /// Read 2 Bytes as ushort
        /// </summary>
        /// <param name="bytes">Byte array.</param>
        /// <param name="s">Offset in array.</param>
        /// <returns>2 Bytes as ushort.</returns>
        public static ushort Read2BytesAsushort(byte[] bytes, int offset) {
            int result = 0;
            for (int i = 0; i < 2; ++i)
                result |= bytes[offset + i] << (8 * (i));
            return (ushort)result;
        }

        /// <summary>
        /// Read 2 Bytes as Int16
        /// </summary>
        /// <param name="bytes">Byte array.</param>
        /// <param name="s">Offset in array.</param>
        /// <returns>2 Bytes as Int16.</returns>
        public static Int16 Read2BytesAsInt16(byte[] bytes, int offset) {
            int result = 0;
            for (int i = 0; i < 2; ++i)
                result |= bytes[offset + i] << (8 * (i));
            return (short)result;
        }

        /// <summary>
        /// Read 3 Bytes as Int24
        /// </summary>
        /// <param name="bytes">Byte array.</param>
        /// <param name="s">Offset in array.</param>
        /// <returns>3 Bytes as Int32.</returns>
        public static int Read3BytesAsInt24(byte[] bytes, int offset) {
            int result = 0;
            for (int i = 0; i < 3; ++i)
                result |= bytes[offset + i] << (8 * i);
            return result;
        }

        /// <summary>
        /// Read 4 Bytes as Int32
        /// </summary>
        /// <param name="bytes">Byte array.</param>
        /// <param name="s">Offset in array.</param>
        /// <returns>4 Bytes as Int32.</returns>
        public static int Read4BytesAsInt32(byte[] bytes, int offset) {
            int result = 0;
            for (int i = 0; i < 4; ++i)
                result |= bytes[offset + i] << (8 * i);
            return result;
        }
        /// <summary>
        /// Read 4 Bytes as Float
        /// </summary>
        /// <param name="bytes">Byte array.</param>
        /// <param name="s">Offset in array.</param>
        /// <returns>4 Bytes as Int32.</returns>
        public static float Read4BytesAsFloat(byte[] bytes, int offset) {
            int result = 0;
            for (int i = 0; i < 4; ++i)
                result |= bytes[offset + i] << (8 * i);
            return result;
        }

        /// <summary>
        /// Read 4 Bytes as UInt32
        /// </summary>
        /// <param name="bytes">Byte array.</param>
        /// <param name="s">Offset in array.</param>
        /// <returns>4 Bytes as Int32.</returns>
        public static UInt32 Read4BytesAsUInt32(byte[] bytes, int offset) {
            UInt32 result = 0;
            for (int i = 0; i < 4; ++i) {
                result |= (uint)(bytes[offset + i] << (8 * i));
            }

            return result;
        }

        #endregion Methods 
    }
}