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

namespace NSMBe4.DSFileSystem
{
    public class Directory
    {
        private bool isSystemFolderP;
        public bool isSystemFolder { get { return isSystemFolderP; } }

        private string nameP;
        public string name { get { return nameP; } }

        private int idP;
        public int id { get { return idP; } }

        private Directory parentDirP;
        public Directory parentDir { get { return parentDirP; } }

        public List<File> childrenFiles = new List<File>();
        public List<Directory> childrenDirs = new List<Directory>();

        private Filesystem parent;

        public Directory(Filesystem parent, Directory parentDir, bool system, string name, int id)
        {
            this.parent = parent;
            this.parentDirP = parentDir;
            this.isSystemFolderP = system;
            this.nameP = name;
            this.idP = id;
        }

        public void dumpFiles()
        {
            dumpFiles(2);
        }

        public void dumpFiles(int ind)
        {
            for (int i = 0; i < ind; i++)
                Console.Out.Write(" ");
            Console.Out.WriteLine("[DIR" + id + "] " + name);
            foreach (Directory d in childrenDirs)
                d.dumpFiles(ind + 4);
            foreach (File f in childrenFiles)
                f.dumpFile(ind + 4);
        }

        public string getPath()
        {
            if (parentDir is null)
                return "FS";
            else
                return parentDir.getPath() + "/" + name;
        }
    }
}
