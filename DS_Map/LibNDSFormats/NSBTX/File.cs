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
using System.IO;

namespace NSMBe4.DSFileSystem
{
    public class File : IComparable
    {
        public bool isSystemFile;
        byte[] file2;
        protected string nameP;
        public string name { get { return nameP; } }

        protected int idP;
        public int id { get { return idP; } }

        protected Directory parentDirP;
        public Directory parentDir { get { return parentDirP; } }

        //if allocationFile is not null,
        //these are the offsets within the alloc file where the offsets
        //of this file are found.

        protected File beginFile;
        protected int beginOffset;
        protected File endFile;
        protected int endOffset;
        protected bool endIsSize; //means that the end offset is the size of the file
        protected bool fixedFile; //means that the file cant be moved nor changed size

        public int fileBegin;
        public int fileSize;

        public int alignment = 4; // word align by default
        public bool canChangeOffset = true; //false for arm9 and 7 bins

        public Filesystem parent;

        protected Object editedBy = null;
        public Boolean beingEdited
        {
            get { return editedBy != null; }
        }

        public File(Filesystem parent, Directory parentDir, string name)
        {
            this.parent = parent;
            this.parentDirP = parentDir;
            this.nameP = name;
            this.fileBegin = 0;
            this.fileSize = (int)parent.s.Length;
            this.isSystemFile = true;
        }

        public File(Filesystem parent, Directory parentDir, bool systemFile, int id, string name, File alFile, int alBeg, int alEnd)
        {
            this.parent = parent;
            this.parentDirP = parentDir;
            this.isSystemFile = systemFile;
            this.idP = id;
            this.nameP = name;
            this.beginFile = alFile;
            this.endFile = alFile;
            this.beginOffset = alBeg;
            this.endOffset = alEnd;
            refreshOffsets();
        }

        public File(Filesystem parent, Directory parentDir, bool systemFile, int id, string name, File alFile, int alBeg, int alEnd, bool endsize)
        {
            this.parent = parent;
            this.parentDirP = parentDir;
            this.isSystemFile = systemFile;
            this.idP = id;
            this.nameP = name;
            this.beginFile = alFile;
            this.endFile = alFile;
            this.beginOffset = alBeg;
            this.endOffset = alEnd;
            this.endIsSize = endsize;
            refreshOffsets();
        }

        public File(Filesystem parent, Directory parentDir, bool systemFile, int id, string name, int alBeg, int alSize)
        {
            this.parent = parent;
            this.parentDirP = parentDir;
            this.isSystemFile = systemFile;
            this.idP = id;
            this.nameP = name;
            this.fileBegin = alBeg;
            this.fileSize = alSize;
            refreshOffsets();
        }

        public virtual byte[] getContents()
        {
            //            enableEdition();
            byte[] file = new byte[fileSize];
            parent.s.Seek(fileBegin, SeekOrigin.Begin);
            parent.s.Read(file, 0, file.Length);
            return file;
        }

        public void dumpFile(int ind)
        {
            for (int i = 0; i < ind; i++)
                Console.Out.Write(" ");
            Console.Out.WriteLine("[" + id + "] " + name + ", at " + fileBegin.ToString("X") + ", size: " + fileSize);
        }


        public virtual void refreshOffsets()
        {
            if (beginFile != null)
                fileBegin = (int)beginFile.getUintAt(beginOffset) + parent.fileDataOffset;

            if (endFile != null)
            {
                int end = (int)endFile.getUintAt(endOffset);
                if (endIsSize)
                    fileSize = (int)end;
                else
                    fileSize = (int)end + parent.fileDataOffset - fileBegin;
            }
        }

        public virtual void saveOffsets()
        {
            if (beginFile != null)
                beginFile.setUintAt(beginOffset, (uint)(fileBegin - parent.fileDataOffset));

            if (endFile != null)
                if (endIsSize)
                    endFile.setUintAt(endOffset, (uint)fileSize);
                else
                    endFile.setUintAt(endOffset, (uint)(fileBegin + fileSize - parent.fileDataOffset));
        }

        public uint getUintAt(int offset)
        {
            enableEdition();
            long pos = parent.s.Position;
            parent.s.Seek(fileBegin + offset, SeekOrigin.Begin);

            uint res = 0;
            for (int i = 0; i < 4; i++)
            {
                res |= (uint)parent.s.ReadByte() << 8 * i;
            }
            parent.s.Seek(pos, SeekOrigin.Begin);
            return res;
        }

        public void setUintAt(int offset, uint val)
        {
            enableEdition();
            long pos = parent.s.Position;
            parent.s.Seek(fileBegin + offset, SeekOrigin.Begin);
            for (int i = 0; i < 4; i++)
            {
                parent.s.WriteByte((byte)(val & 0xFF));
                val >>= 8;
            }
            parent.s.Seek(pos, SeekOrigin.Begin);
        }

        public ushort getUshortAt(int offset)
        {
            enableEdition();
            long pos = parent.s.Position;
            parent.s.Seek(fileBegin + offset, SeekOrigin.Begin);

            ushort res = 0;
            for (int i = 0; i < 2; i++)
            {
                res |= (ushort)(parent.s.ReadByte() << 8 * i);
            }
            parent.s.Seek(pos, SeekOrigin.Begin);
            return res;
        }


        public void setUshortAt(int offset, ushort val)
        {
            enableEdition();
            long pos = parent.s.Position;
            parent.s.Seek(fileBegin + offset, SeekOrigin.Begin);
            for (int i = 0; i < 2; i++)
            {
                parent.s.WriteByte((byte)(val & 0xFF));
                val >>= 8;
            }
            parent.s.Seek(pos, SeekOrigin.Begin);
        }

        public byte getByteAt(int offs)
        {
            enableEdition();
            long pos = parent.s.Position;
            parent.s.Seek(fileBegin + offs, SeekOrigin.Begin);
            byte res = (byte)parent.s.ReadByte();
            parent.s.Seek(pos, SeekOrigin.Begin);
            return res;
        }

        public void setByteAt(int offs, byte val)
        {
            enableEdition();
            long pos = parent.s.Position;
            parent.s.Seek(fileBegin + offs, SeekOrigin.Begin);
            parent.s.WriteByte(val);
            parent.s.Seek(pos, SeekOrigin.Begin);
        }

        public bool isAGoodEditor(object editor)
        {
            if (!beingEdited)
                return false;

            if (editor == editedBy)
                return true;

            if (editor is InlineFile && inlineEditors.Contains(editor as InlineFile))
                return true;

            return false;
        }
        public virtual void replace(byte[] newFile, object editor)
        {
            //if (!isAGoodEditor(editor))
             //   throw new Exception("NOT CORRECT EDITOR " + name);

            if (newFile.Length != fileSize && fixedFile)
                throw new Exception("TRYING TO RESIZE FIXED FILE: " + name);

            //            enableEdition();

            //            Console.Out.WriteLine("Replacing: [" + id + "] " + name);
            int newStart = fileBegin;
            if (newFile.Length > fileSize) //if we insert a bigger file
            {                         //it might not fit in the current place
                if (canChangeOffset)
                {
                    newStart = parent.findFreeSpace(newFile.Length, alignment);
                    if (newStart % alignment != 0)
                        newStart += alignment - newStart % alignment;
                }
                else
                {
                    parent.allFiles.Sort();
                    if (!(parent.allFiles.IndexOf(this) == parent.allFiles.Count - 1))
                    {
                        File nextFile = parent.allFiles[parent.allFiles.IndexOf(this) + 1];
                        parent.moveAllFiles(nextFile, fileBegin + newFile.Length);
                    }
                }
            }
            /*else if (parent is NarcFilesystem)
            {
                parent.allFiles.Sort();
                if (!(parent.allFiles.IndexOf(this) == parent.allFiles.Count - 1))
                {
                    File nextFile = parent.allFiles[parent.allFiles.IndexOf(this) + 1];
                    parent.moveAllFiles(nextFile, fileBegin + newFile.Length);
                }
            }*/
            if (newStart % alignment != 0)
                Console.Out.Write("Warning: File is not being aligned: " + nameP + ", at " + newStart.ToString("X"));
            //write the file
            parent.s.Seek(newStart, SeekOrigin.Begin);
            parent.s.Write(newFile, 0, newFile.Length);
            //if (parent is NarcFilesystem)
            //    parent.s.SetLength(parent.allFiles[parent.allFiles.Count - 1].fileBegin + parent.allFiles[parent.allFiles.Count - 1].fileSize + 10);
            //update ending pos
            fileBegin = newStart;
            fileSize = newFile.Length;
            saveOffsets();
            parent.fileMoved(this); //NEEDED FOR UPDATING TOTAL USED ROM SIZE IN HEADER
        }

        public void moveTo(int newOffs)
        {
            if (newOffs % alignment != 0)
                Console.Out.Write("Warning: File is not being aligned: " + nameP + ", at " + newOffs.ToString("X"));
            byte[] data = getContents();
            parent.s.Seek(newOffs, SeekOrigin.Begin);
            parent.s.Write(data, 0, data.Length);
            fileBegin = newOffs;
            saveOffsets();
        }

        public int CompareTo(object obj)
        {
            File f = obj as File;
            if (fileBegin == f.fileBegin)
                return fileSize.CompareTo(f.fileSize);
            return fileBegin.CompareTo(f.fileBegin);
        }

        public virtual void beginEdit(Object editor)
        {
            if (beingEdited)
                throw new Exception();
            else
                editedBy = editor;
        }

        public virtual void endEdit(Object editor)
        {
            //if (!beingEdited)
            //    throw new Exception("NOT EDITING FILE " + name);
            //if (editor != editedBy)
            //    throw new Exception("NOT CORRECT EDITOR" + name);

            editedBy = null;
        }

        public bool beingEditedBy(Object ed)
        {
            return ed == editedBy;
        }


        public bool isAddrInFile(int addr)
        {
            return addr >= fileBegin && addr < fileBegin + fileSize;
        }

        private List<InlineFile> inlineEditors = new List<InlineFile>();

        public void beginEditInline(InlineFile f)
        {
            if (inlineEditors.Count == 0)
                beginEdit(this);

            inlineEditors.Add(f);
        }

        public void endEditInline(InlineFile f)
        {
            if (!inlineEditors.Contains(f))
                throw new Exception("ERROR: INLINE FILE");
            inlineEditors.Remove(f);
            if (inlineEditors.Count == 0)
                endEdit(this);
        }

        //Intended for compressed files like overlays.
        //Must decompress the file so it's editable and still playable.
        public virtual void enableEdition() { }


        public string getPath()
        {
            return parentDir.getPath() + "/" + name;
        }
    }
}
