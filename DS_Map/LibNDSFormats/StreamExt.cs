// PG4Map - a 4th Gen Pokemon Map Viewer.
// Miscellaneous stream extensions.
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

namespace System.IO {
    /// <summary>
    /// Extension methods for System.IO.Stream.
    /// </summary>
    static class StreamExt {
        #region Methods (3) 

        // Public Methods (3) 

        /// <summary>
        /// Get remaining length from stream position.
        /// </summary>
        public static long GetRemainingLength(this Stream stream) {
            return stream.Length - stream.Position;
        }

        /// <summary>
        /// Skip a number of bytes.
        /// </summary>
        /// <param name="skipCount">Number of bytes to skip.</param>
        public static void Skip(this Stream stream, long skipCount) {
            stream.Seek(skipCount, SeekOrigin.Current);
        }

        /// <summary>
        /// Skip a number of bytes.
        /// </summary>
        /// <param name="skipCount">Number of bytes to skip.</param>
        public static void Skip(this Stream stream, uint skipCount) {
            Skip(stream, (long)skipCount);
        }

        #endregion Methods 
    }
}