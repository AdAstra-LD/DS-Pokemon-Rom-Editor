using DSPRE.ROMFiles;
using Ekona.Images;
using Images;
using LibNDSFormats.NSBMD;
using LibNDSFormats.NSBTX;
using Microsoft.WindowsAPICodePack.Dialogs;
using NSMBe4.DSFileSystem;
using ScintillaNET;
using ScintillaNET.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Tao.OpenGl;
using Tao.Platform.Windows;
using Velopack;
using Velopack.Sources;
using static DSPRE.RomInfo;

namespace DSPRE {
    public static class Helpers {
        static MainProgram MainProgram;

        public static RomInfo romInfo;
        public static bool hideBuildings = new bool();

        public static NSBMDGlRenderer mapRenderer;

        public static ToolStripProgressBar toolStripProgressBar { get { return MainProgram.toolStripProgressBar; } }

        public static void Initialize(MainProgram mainProgram) {
            MainProgram = mainProgram;
            mapRenderer = new NSBMDGlRenderer();
        }

        public static void CheckForUpdates(bool silent = true)
        {
            AppLogger.Info("Checking for updates...");
            var mgr = new UpdateManager(new GithubSource("https://github.com/Mixone-FinallyHere/DS-Pokemon-Rom-Editor", "", prerelease: false));

            var newVersion = mgr.CheckForUpdates();
            if (newVersion == null)
            {
                AppLogger.Info("No updates available.");
                if (!silent)
                    MessageBox.Show("No update is available.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else
            {
                DialogResult update = MessageBox.Show($"A new DSPRE version is available: {newVersion.TargetFullRelease.Version}.\nDo you want to install it?", "New update", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (update == DialogResult.Yes)
                {
                    AppLogger.Info($"New version available: {newVersion.TargetFullRelease.Version} (Current: {mgr.CurrentVersion})");
                    mgr.DownloadUpdates(newVersion);

                    AppLogger.Info($"Installing update {newVersion.TargetFullRelease.Version} and restarting app...");
                    mgr.ApplyUpdatesAndRestart(newVersion);
                }
                else
                {
                    AppLogger.Info("User declined to update the application.");
                }
            }
        }

        static bool disableHandlersOld;
        static bool disableHandlers;

        public static bool HandlersDisabled { get { return disableHandlers == true; } }
        public static bool HandlersEnabled { get { return disableHandlers == false; } }

        public static void BackUpDisableHandler() {
            disableHandlersOld = disableHandlers;
        }

        public static void RestoreDisableHandler() {
            disableHandlers = disableHandlersOld;
        }

        public static void DisableHandlers() {
            disableHandlers = true;
        }

        public static void EnableHandlers() {
            disableHandlers = false;
        }

        public static string GetDSPREVersion()
        {
            return "" + Assembly.GetExecutingAssembly().GetName().Version.Major + "." + Assembly.GetExecutingAssembly().GetName().Version.Minor +
                "." + Assembly.GetExecutingAssembly().GetName().Version.Build;
        }

        public static void statusLabelMessage(string msg = "Ready") {
            ToolStripStatusLabel statusLabel = MainProgram.statusLabel;
            statusLabel.Text = msg;
            statusLabel.Font = new Font(statusLabel.Font, FontStyle.Regular);
            statusLabel.ForeColor = Color.Black;
            statusLabel.Invalidate();
        }

        public static void statusLabelError(string errorMsg, bool severe = true) {
            ToolStripStatusLabel statusLabel = MainProgram.statusLabel;
            statusLabel.Text = errorMsg;
            statusLabel.Font = new Font(statusLabel.Font, FontStyle.Bold);
            statusLabel.ForeColor = severe ? Color.Red : Color.DarkOrange;
            statusLabel.Invalidate();
        }

        //Locate File - buttons
        public static void ExplorerSelect(string path) {
            if (System.IO.File.Exists(path)) {
                Process.Start("explorer.exe", "/select" + "," + "\"" + path + "\"");
            }
        }

        public static string[] GetTrainerNames() {
            List<string> trainerList = new List<string>();

            /* Store all trainer names and classes */
            TextArchive trainerClasses = new TextArchive(RomInfo.trainerClassMessageNumber);
            TextArchive trainerNames = new TextArchive(RomInfo.trainerNamesMessageNumber);

            int trainerCount = Filesystem.GetTrainerPropertiesCount();
            for (int i = 0; i < trainerCount; i++) {
                string path = Filesystem.GetTrainerPropertiesPath(i);
                int classMessageID = BitConverter.ToUInt16(DSUtils.ReadFromFile(path, startOffset: 1, 2), 0);
                string currentTrainerName;

                if (i < trainerNames.messages.Count) {
                    currentTrainerName = trainerNames.messages[i];
                } else {
                    currentTrainerName = TrainerFile.NAME_NOT_FOUND;
                }

                trainerList.Add("[" + i.ToString("D2") + "] " + trainerClasses.messages[classMessageID] + " " + currentTrainerName);
            }

            return trainerList.ToArray();
        }

        public static void MW_LoadModelTextures(NSBMD model, string textureFolder, int fileID) {
            if (fileID < 0) {
                return;
            }

            string texturePath = Filesystem.GetPath(textureFolder, fileID);
            model.materials = NSBTXLoader.LoadNsbtx(new MemoryStream(System.IO.File.ReadAllBytes(texturePath)), out model.Textures, out model.Palettes);
            try {
                model.MatchTextures();
            } catch {
            }
        }

        public static void MW_LoadModelTextures(MapFile mapFile, int fileID) {
            MW_LoadModelTextures(mapFile.mapModel, Filesystem.mapTextures, fileID);
        }

        public static void MW_LoadModelTextures(Building building, int fileID) {
            MW_LoadModelTextures(building.NSBMDFile, Filesystem.buildingTextures, fileID);
        }

        public static void SetupRenderer(float ang, float dist, float elev, float perspective, int width, int height) {
            //TODO: improve this
            Gl.glEnable(Gl.GL_RESCALE_NORMAL);
            Gl.glEnable(Gl.GL_COLOR_MATERIAL);
            Gl.glEnable(Gl.GL_DEPTH_TEST);
            Gl.glEnable(Gl.GL_NORMALIZE);
            Gl.glDisable(Gl.GL_CULL_FACE);
            Gl.glFrontFace(Gl.GL_CCW);
            Gl.glClearDepth(1);
            Gl.glEnable(Gl.GL_ALPHA_TEST);
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
            Gl.glEnable(Gl.GL_BLEND);
            Gl.glAlphaFunc(Gl.GL_GREATER, 0f);
            Gl.glClearColor(51f / 255f, 51f / 255f, 51f / 255f, 1f);
            Gl.glViewport(0, 0, width, height);
            float aspect = width / height; //(vp[2] - vp[0]) / (vp[3] - vp[1]);
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            Glu.gluPerspective(perspective, aspect, 0.2f, 500.0f); //0.02f, 32.0f);
            Gl.glTranslatef(0, 0, -dist);
            Gl.glRotatef(elev, 1, 0, 0);
            Gl.glRotatef(ang, 0, 1, 0);
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();
            Gl.glTranslatef(0, 0, -dist);
            Gl.glRotatef(elev, 1, 0, 0);
            Gl.glRotatef(-ang, 0, 1, 0);
            Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_POSITION, new float[] { 1, 1, 1, 0 });
            Gl.glLightfv(Gl.GL_LIGHT1, Gl.GL_POSITION, new float[] { 1, 1, 1, 0 });
            Gl.glLightfv(Gl.GL_LIGHT2, Gl.GL_POSITION, new float[] { 1, 1, 1, 0 });
            Gl.glLightfv(Gl.GL_LIGHT3, Gl.GL_POSITION, new float[] { 1, 1, 1, 0 });
            Gl.glLoadIdentity();
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0);
            Gl.glColor3f(1.0f, 1.0f, 1.0f);
            Gl.glDepthMask(Gl.GL_TRUE);
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
        }

        public static void RenderMap(ref MapFile mapFile, int width, int height, float ang, float dist, float elev, float perspective, bool mapTexturesON = true, bool buildingTexturesON = true) {
            #region Useless variables that the rendering API still needs

            MKDS_Course_Editor.NSBTA.NSBTA.NSBTA_File ani = new MKDS_Course_Editor.NSBTA.NSBTA.NSBTA_File();
            MKDS_Course_Editor.NSBTP.NSBTP.NSBTP_File tp = new MKDS_Course_Editor.NSBTP.NSBTP.NSBTP_File();
            MKDS_Course_Editor.NSBCA.NSBCA.NSBCA_File ca = new MKDS_Course_Editor.NSBCA.NSBCA.NSBCA_File();
            int[] aniframeS = new int[0];

            #endregion

            /* Adjust rendering settings */
            SetupRenderer(ang, dist, elev, perspective, width, height);

            /* Render the map model */
            NSBMD model = mapFile.mapModel;
            mapRenderer.Model = model.models[0];

            // int scale = 64;
            float scale = 0.015625f;
            Gl.glScalef(mapRenderer.Model.modelScale * scale, mapRenderer.Model.modelScale * scale, mapRenderer.Model.modelScale * scale);

            /* Determine if map textures must be rendered */
            if (mapTexturesON) {
                Gl.glEnable(Gl.GL_TEXTURE_2D);
            } else {
                Gl.glDisable(Gl.GL_TEXTURE_2D);
            }

            // Render map model
            mapRenderer.RenderModel("", ani, aniframeS, aniframeS, aniframeS, aniframeS, aniframeS, ca, false, -1, 0.0f, 0.0f, dist, elev, ang, true, tp, model);

            if (hideBuildings) {
                return;
            }

            if (buildingTexturesON) {
                Gl.glEnable(Gl.GL_TEXTURE_2D);
            } else {
                Gl.glDisable(Gl.GL_TEXTURE_2D);
            }

            for (int i = 0; i < mapFile.buildings.Count; i++) {
                Building building = mapFile.buildings[i];
                model = building.NSBMDFile;
                if (model is null) {
                    Console.WriteLine("Null building can't be rendered");
                } else {
                    mapRenderer.Model = model.models[0];
                    ScaleTranslateRotateBuilding(building);
                    mapRenderer.RenderModel("", ani, aniframeS, aniframeS, aniframeS, aniframeS, aniframeS, ca, false, -1, 0.0f, 0.0f, dist, elev, ang, true, tp, model);
                }
            }
        }

        public static Bitmap GrabMapScreenshot(int width, int height) {
            Bitmap bmp = new Bitmap(width, height);
            System.Drawing.Imaging.BitmapData data = bmp.LockBits(new Rectangle(0, 0, width, height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Gl.glReadPixels(0, 0, width, height, Gl.GL_BGR, Gl.GL_UNSIGNED_BYTE, data.Scan0);
            bmp.UnlockBits(data);
            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
            return bmp;
        }

        private static void ScaleTranslateRotateBuilding(Building building) {
            float fullXcoord = building.xPosition + building.xFraction / 65536f;
            float fullYcoord = building.yPosition + building.yFraction / 65536f;
            float fullZcoord = building.zPosition + building.zFraction / 65536f;

            float scaleFactor = building.NSBMDFile.models[0].modelScale / 1024;
            float translateFactor = 256 / building.NSBMDFile.models[0].modelScale;

            Gl.glScalef(scaleFactor * building.width, scaleFactor * building.height, scaleFactor * building.length);
            Gl.glTranslatef(fullXcoord * translateFactor / building.width, fullYcoord * translateFactor / building.height, fullZcoord * translateFactor / building.length);
            Gl.glRotatef(Building.U16ToDeg(building.xRotation), 1, 0, 0);
            Gl.glRotatef(Building.U16ToDeg(building.yRotation), 0, 1, 0);
            Gl.glRotatef(Building.U16ToDeg(building.zRotation), 0, 0, 1);
        }

        public static Image GetPokePic(int species, int w, int h, PaletteBase paletteBase, ImageBase imageBase, SpriteBase spriteBase) {
            bool fiveDigits = false; // some extreme future proofing
            try {
                string path = Filesystem.GetMonIconPath(0);
                paletteBase = new NCLR(path, 0, Path.GetFileName(path));
            } catch (FileNotFoundException) {
                string path = Filesystem.GetMonIconPath(0, "D5");
                paletteBase = new NCLR(path, 0, Path.GetFileName(path));
                fiveDigits = true;
            }

            // read arm9 table to grab pal ID
            int paletteId = 0;
            byte[] iconPalTableBuf;

            switch (RomInfo.gameFamily) {
                case RomInfo.GameFamilies.DP:
                    iconPalTableBuf = ARM9.ReadBytes(0x6B838, 4);
                    break;
                case RomInfo.GameFamilies.Plat:
                    iconPalTableBuf = ARM9.ReadBytes(0x79F80, 4);
                    break;
                case RomInfo.GameFamilies.HGSS:
                default:
                    iconPalTableBuf = ARM9.ReadBytes(0x74408, 4);
                    break;
            }

            int iconPalTableAddress = (iconPalTableBuf[3] & 0xFF) << 24 | (iconPalTableBuf[2] & 0xFF) << 16 | (iconPalTableBuf[1] & 0xFF) << 8 | (iconPalTableBuf[0] & 0xFF) /* << 0 */;
            string iconTablePath;

            int iconPalTableOffsetFromFileStart;
            if (iconPalTableAddress >= RomInfo.synthOverlayLoadAddress) {
                // if the pointer shows the table was moved to the synthetic overlay
                iconPalTableOffsetFromFileStart = iconPalTableAddress - (int)RomInfo.synthOverlayLoadAddress;
                iconTablePath = Filesystem.expArmPath;
            } else {
                iconPalTableOffsetFromFileStart = iconPalTableAddress - 0x02000000;
                iconTablePath = RomInfo.arm9Path;
            }

            using (DSUtils.EasyReader idReader = new DSUtils.EasyReader(iconTablePath, iconPalTableOffsetFromFileStart + species)) {
                paletteId = idReader.ReadByte();
            }

            if (paletteId != 0) {
                paletteBase.Palette[0] = paletteBase.Palette[paletteId]; // update pal 0 to be the new pal
            }

            // grab tiles
            int spriteFileID = species + 7;
            if (fiveDigits) {
                string path = Filesystem.GetMonIconPath(spriteFileID, "D5");
                imageBase = new NCGR(path, spriteFileID, Path.GetFileName(path));
            } else {
                string path = Filesystem.GetMonIconPath(spriteFileID);
                imageBase = new NCGR(path, spriteFileID, Path.GetFileName(path));
            }

            // grab sprite
            const int ncerFileId = 2;
            if (fiveDigits) {
                string path = Filesystem.GetMonIconPath(ncerFileId, "D5");
                spriteBase = new NCER(path, ncerFileId, Path.GetFileName(path));
            } else {
                string path = Filesystem.GetMonIconPath(ncerFileId);
                spriteBase = new NCER(path, ncerFileId, Path.GetFileName(path));
            }

            // copy this from the trainer
            int bank0OAMcount = spriteBase.Banks[0].oams.Length;
            int[] OAMenabled = new int[bank0OAMcount];
            for (int i = 0; i < OAMenabled.Length; i++) {
                OAMenabled[i] = i;
            }

            // finally compose image
            try {
                return spriteBase.Get_Image(imageBase, paletteBase, 0, w, h, false, false, false, true, true, -1, OAMenabled);
            } catch (FormatException) {
                return Properties.Resources.IconPokeball;
            }
            // default:
            //partyPokemonPictureBoxList[partyPos].Image = cb.SelectedIndex > 0 ? (Image)Properties.PokePics.ResourceManager.GetObject(FixPokenameString(PokeDatabase.System.pokeNames[(ushort)cb.SelectedIndex])) : global::DSPRE.Properties.Resources.IconPokeball;
        }

        public static void GenerateKeystrokes(string keys, Scintilla textArea) {
            //Example
            //GenerateKeystrokes("+{TAB}");
            HotKeyManager.Enable = false;
            textArea.Focus();
            SendKeys.Send(keys);
            HotKeyManager.Enable = true;
        }

        public static void PictureBoxDisable(object sender, PaintEventArgs e) {
            if (sender is PictureBox pict && pict.Image != null && (!pict.Enabled)) {
                using (Bitmap img = new Bitmap(pict.Image, pict.ClientSize)) {
                    ControlPaint.DrawImageDisabled(e.Graphics, img, 0, 0, pict.BackColor);
                }
            }
        }

        public static List<string> getHeaderListBoxNames() {
            if (string.IsNullOrWhiteSpace(RomInfo.internalNamesPath)) {
                return null;
            }

            List<string> headerListBoxNames = new List<string>();

            using (DSUtils.EasyReader reader = new DSUtils.EasyReader(RomInfo.internalNamesPath)) {
                int headerCount = RomInfo.GetHeaderCount();
                for (int i = 0; i < headerCount; i++) {
                    byte[] row = reader.ReadBytes(RomInfo.internalNameLength);
                    string internalName = Encoding.ASCII.GetString(row); //.TrimEnd();
                    headerListBoxNames.Add(MapHeader.BuildName(i, internalName));
                }
            }

            return headerListBoxNames;
        }

        public static List<string> getInternalNames() {
            List<string> internalNames = new List<string>();

            using (DSUtils.EasyReader reader = new DSUtils.EasyReader(RomInfo.internalNamesPath)) {
                int headerCount = RomInfo.GetHeaderCount();
                for (int i = 0; i < headerCount; i++) {
                    byte[] row = reader.ReadBytes(RomInfo.internalNameLength);
                    string internalName = Encoding.ASCII.GetString(row); //.TrimEnd();
                    internalNames.Add(internalName.TrimEnd('\0'));
                }
            }

            return internalNames;
        }

        public static int CalculateTimeDifferenceInSeconds(int startHour, int startMinute, int startSecond, int endHour, int endMinute, int endSecond)
        {
            // Convert start time and end time to seconds since midnight
            int startTimeInSeconds = (startHour * 3600) + (startMinute * 60) + startSecond;
            int endTimeInSeconds = (endHour * 3600) + (endMinute * 60) + endSecond;

            // Calculate difference
            int timeDifference = endTimeInSeconds - startTimeInSeconds;

            // If time difference is negative (end time is past midnight), adjust
            if (timeDifference < 0)
            {
                timeDifference += 24 * 3600; // Add 24 hours in seconds
            }

            return timeDifference;
        }

        public static String formatTime(int time)
        {
            string stringTime = time.ToString();
            if (time < 10)
            {
                stringTime = "0" + stringTime;
            }

            return stringTime;
        }

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        extern static bool DestroyIcon(IntPtr handle);


        public static void PopOutEditor<T>(T control, string title, Image icon, Action<T> onClose = null) where T : Control
        {
            var originalParent = control.Parent;
            var originalIndex = originalParent?.Controls.IndexOf(control) ?? -1;

            originalParent?.Controls.Remove(control);
            Icon _icon = null;
            if(icon != null)
            {
                Bitmap bitmap = new Bitmap(icon);
                _icon = Icon.FromHandle(bitmap.GetHicon());
            }
        

            var form = new Form
            {
                Text = title,
                StartPosition = FormStartPosition.CenterScreen,
                FormBorderStyle = FormBorderStyle.FixedSingle,
                MaximizeBox = false,
                ClientSize = control.Size,
                ShowIcon = icon == null ? false : true,
                Icon = _icon
                
            };

            control.Dock = DockStyle.Fill;
            form.Controls.Add(control);

            form.FormClosing += (s, e) =>
            {
                form.Controls.Remove(control);
                if(_icon != null)
                {
                    DestroyIcon(_icon.Handle);
                }
                if (originalParent != null && !originalParent.IsDisposed)
                {
                    if (originalIndex >= 0 && originalIndex <= originalParent.Controls.Count)
                        originalParent.Controls.Add(control);
                    else
                        originalParent.Controls.Add(control);

                    originalParent.Controls.SetChildIndex(control, originalIndex);
                }

                onClose?.Invoke(control);
            };

            form.Show();
        }

        public static void ExclusiveCBInvert(CheckBox cb)
        {
            if (Helpers.HandlersDisabled)
            {
                return;
            }

            Helpers.DisableHandlers();

            if (cb.Checked)
            {
                cb.Checked = !cb.Checked;
            }

            Helpers.EnableHandlers();
        }

        public static void ContentBasedBatchRename(MainProgram parent, DirectoryInfo d = null)
        {
            (DirectoryInfo d, FileInfo[] files) dirData = OpenNonEmptyDir(d, title: "Content-Based Batch Rename Tool");
            d = dirData.d;
            FileInfo[] files = dirData.files;

            if (d == null || files == null)
            {
                return;
            }

            DialogResult dr = MessageBox.Show("About to rename " + files.Length + " file" + (files.Length > 1 ? "s" : "") +
                " from the input folder (taken in ascending order), according to their content.\n" +
                "If a destination file already exists, DSPRE will append a number to its name.\n\n" +
                "Do you want to proceed?", "Confirm operation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dr.Equals(DialogResult.Yes))
            {
                List<string> enumerationFile = new List<string> {
                    "#============================================================================",
                    "# File enumeration definition for folder " + "\"" + d.Name + "\"",
                    "#============================================================================"
                };
                int initialLength = enumerationFile.Count;

                const byte toRead = 16;
                foreach (FileInfo f in files)
                {
                    Console.WriteLine(f.Name);

                    string fileNameOnly = Path.GetFileNameWithoutExtension(f.FullName);
                    string dirNameOnly = Path.GetDirectoryName(f.FullName);

                    string destName = "";
                    byte[] b = DSUtils.ReadFromFile(f.FullName, 0, toRead);

                    if (b == null || b.Length < toRead)
                    {
                        continue;
                    }

                    string magic = "";

                    if (b[0] == 'B' && b[3] == '0')
                    { //B**0
                        ushort nameOffset;

                        destName = dirNameOnly + "\\"; //Full filename can be changed
                        nameOffset = (ushort)(52 + (4 * (BitConverter.ToUInt16(b, 0xE) - 1)));

                        if (b[1] == 'T' && b[2] == 'X')
                        { //BTX0
#if false
                            nameOffset += 0xEC;
#else
                            destName = fileNameOnly;
#endif
                        }

                        string nameRead = Encoding.UTF8.GetString(DSUtils.ReadFromFile(f.FullName, nameOffset, 16)).TrimEnd(new char[] { (char)0 });

                        if (nameRead.Length <= 0 || nameRead.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
                        {
                            destName = fileNameOnly; //Filename can't be changed, only extension
                        }
                        else
                        {
                            destName += nameRead;
                        }

                        destName += ".ns";
                        for (int i = 0; i < 3; i++)
                        {
                            magic += Char.ToLower((char)b[i]);
                        }
                    }
                    else
                    {
                        destName = fileNameOnly + ".";
                        byte offset = 0;

                        if (b[5] == 'R' && b[8] == 'N')
                        {
                            offset = 5;
                        }

                        for (int i = 0; i < 4; i++)
                        {
                            magic += Char.ToLower((char)b[offset + i]);
                        }
                    }

                    if (string.IsNullOrWhiteSpace(magic) || !magic.All(char.IsLetterOrDigit))
                    {
                        continue;
                    }

                    destName += magic;

                    if (string.IsNullOrWhiteSpace(destName))
                    {
                        continue;
                    }

                    destName = MakeUniqueName(destName, fileNameOnly = null, dirNameOnly);
                    System.IO.File.Move(f.FullName, Path.Combine(Path.GetDirectoryName(f.FullName), Path.GetFileName(destName)));

                    enumerationFile.Add(Path.GetFileName(destName));
                }

                if (enumerationFile.Count > initialLength)
                {
                    MessageBox.Show("Files inside folder \"" + d.FullName + "\" have been renamed according to their contents.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    DialogResult response = MessageBox.Show("Do you want to save a file enumeration list?", "Waiting for user", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (response.Equals(DialogResult.Yes))
                    {
                        MessageBox.Show("Choose where to save the output list file.", "Name your list file", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        SaveFileDialog sf = new SaveFileDialog
                        {
                            Filter = "List File (*.txt; *.list)|*.txt;*.list",
                            FileName = d.Name + ".list"
                        };
                        if (sf.ShowDialog(parent) != DialogResult.OK)
                        {
                            return;
                        }

                        System.IO.File.WriteAllLines(sf.FileName, enumerationFile);
                        MessageBox.Show("List file saved.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show("No file content could be recognized.", "Operation terminated", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        public static (DirectoryInfo, FileInfo[]) OpenNonEmptyDir(DirectoryInfo d = null, string title = "Waiting for user")
        {
            /*==================================================================*/
            if (d == null)
            {
                MessageBox.Show("Choose a source folder.", title, MessageBoxButtons.OK, MessageBoxIcon.Information);
                CommonOpenFileDialog sourceDirDialog = new CommonOpenFileDialog
                {
                    IsFolderPicker = true,
                    Multiselect = false
                };

                if (sourceDirDialog.ShowDialog() != CommonFileDialogResult.Ok)
                {
                    return (null, null);
                }

                d = new DirectoryInfo(sourceDirDialog.FileName);
            }

            FileInfo[] tempfiles = d.GetFiles();
            FileInfo[] files = tempfiles.OrderBy(n => System.Text.RegularExpressions.Regex.Replace(n.Name, @"\d+", e => e.Value.PadLeft(tempfiles.Length.ToString().Length, '0'))).ToArray();

            if (files.Length <= 0)
            {
                MessageBox.Show("Folder " + "\"" + d.FullName + "\"" + " is empty.\nCan't proceed.", "Invalid folder", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return (null, null);
            };

            return (d, files);
        }

        public static string MakeUniqueName(string fileName, string fileNameOnly = null, string dirNameOnly = null, string extension = null)
        {
            if (fileNameOnly == null)
            {
                fileNameOnly = Path.GetFileNameWithoutExtension(fileName);
            }
            if (dirNameOnly == null)
            {
                dirNameOnly = Path.GetDirectoryName(fileName);
            }
            if (extension == null)
            {
                extension = Path.GetExtension(fileName);
            }

            int append = 1;

            while (System.IO.File.Exists(Path.Combine(dirNameOnly, fileName)))
            {
                string tmp = fileNameOnly + "(" + (append++) + ")";
                fileName = Path.Combine(dirNameOnly, tmp + extension);
            }
            return fileName;
        }

        public static void ExportTrainerUsageToCSV(Dictionary<string, Dictionary<string, int>> trainerUsage, string csvFilePath)
        {
            // Create the StreamWriter to write data to the CSV file
            var sortedTrainerClasses = trainerUsage.Keys.OrderBy(className => className);

            using (StreamWriter sw = new StreamWriter(csvFilePath))
            {
                // Write the header row
                sw.WriteLine("Trainer Class;Pokemon Name;Occurrences");

                // Iterate over the sorted trainer class names
                foreach (string className in sortedTrainerClasses)
                {
                    Dictionary<string, int> innerDict = trainerUsage[className];

                    // Sort the Pokemon names alphabetically
                    var sortedPokemonNames = innerDict.Keys.OrderByDescending(pokeName => innerDict[pokeName]);

                    // Iterate over the sorted mon names
                    foreach (string pokeName in sortedPokemonNames)
                    {
                        int occurrences = innerDict[pokeName];

                        // Write the data row
                        sw.WriteLine($"{className};{pokeName};{occurrences}");
                    }
                    sw.WriteLine($"-;-;-");
                }
            }

            Console.WriteLine("CSV file exported successfully.");
        }

    }
}