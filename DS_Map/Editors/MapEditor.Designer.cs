namespace DSPRE.Editors
{
    partial class MapEditor
    {
        /// <summary> 
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur de composants

        /// <summary> 
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MapEditor));
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.mapScreenshotButton = new System.Windows.Forms.Button();
            this.wireframeCheckBox = new System.Windows.Forms.CheckBox();
            this.radio3D = new System.Windows.Forms.RadioButton();
            this.radio2D = new System.Windows.Forms.RadioButton();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.addMapFileButton = new System.Windows.Forms.Button();
            this.locateCurrentMapBin = new System.Windows.Forms.Button();
            this.removeMapFileButton = new System.Windows.Forms.Button();
            this.replaceMapBinButton = new System.Windows.Forms.Button();
            this.exportCurrentMapBinButton = new System.Windows.Forms.Button();
            this.saveMapButton = new System.Windows.Forms.Button();
            this.mapRenderPanel = new System.Windows.Forms.Panel();
            this.mapOpenGlControl = new Tao.Platform.Windows.SimpleOpenGlControl();
            this.movPictureBox = new System.Windows.Forms.PictureBox();
            this.label26 = new System.Windows.Forms.Label();
            this.buildTextureComboBox = new System.Windows.Forms.ComboBox();
            this.mapFileLabel = new System.Windows.Forms.Label();
            this.mapTextureComboBox = new System.Windows.Forms.ComboBox();
            this.mapTextureLabel = new System.Windows.Forms.Label();
            this.selectMapComboBox = new System.Windows.Forms.ComboBox();
            this.mapPartsTabControl = new System.Windows.Forms.TabControl();
            this.buildingsTabPage = new System.Windows.Forms.TabPage();
            this.groupBox33 = new System.Windows.Forms.GroupBox();
            this.yRotDegBldUpDown = new System.Windows.Forms.NumericUpDown();
            this.xRotDegBldUpDown = new System.Windows.Forms.NumericUpDown();
            this.zRotDegBldUpDown = new System.Windows.Forms.NumericUpDown();
            this.yRotBuildUpDown = new System.Windows.Forms.NumericUpDown();
            this.xRotBuildUpDown = new System.Windows.Forms.NumericUpDown();
            this.zRotBuildUpDown = new System.Windows.Forms.NumericUpDown();
            this.yLabel = new System.Windows.Forms.Label();
            this.lockXZgroupbox = new System.Windows.Forms.GroupBox();
            this.bldPlaceLockXcheckbox = new System.Windows.Forms.CheckBox();
            this.bldPlaceLockZcheckbox = new System.Windows.Forms.CheckBox();
            this.zLabel = new System.Windows.Forms.Label();
            this.xLabel = new System.Windows.Forms.Label();
            this.bldRoundGroupbox = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.bldRoundDecmil = new System.Windows.Forms.RadioButton();
            this.bldRoundCentMil = new System.Windows.Forms.RadioButton();
            this.bldRoundWhole = new System.Windows.Forms.RadioButton();
            this.bldRoundDec = new System.Windows.Forms.RadioButton();
            this.bldRoundCent = new System.Windows.Forms.RadioButton();
            this.bldRoundMil = new System.Windows.Forms.RadioButton();
            this.bldPlaceWithMouseCheckbox = new System.Windows.Forms.CheckBox();
            this.importBuildingsButton = new System.Windows.Forms.Button();
            this.groupBox20 = new System.Windows.Forms.GroupBox();
            this.interiorbldRadioButton = new System.Windows.Forms.RadioButton();
            this.exteriorbldRadioButton = new System.Windows.Forms.RadioButton();
            this.buildIndexComboBox = new System.Windows.Forms.ComboBox();
            this.groupBox19 = new System.Windows.Forms.GroupBox();
            this.buildingHeightUpDown = new System.Windows.Forms.NumericUpDown();
            this.buildingWidthUpDown = new System.Windows.Forms.NumericUpDown();
            this.buildingLengthUpDown = new System.Windows.Forms.NumericUpDown();
            this.duplicateBuildingButton = new System.Windows.Forms.Button();
            this.exportBuildingsButton = new System.Windows.Forms.Button();
            this.removeBuildingButton = new System.Windows.Forms.Button();
            this.addBuildingButton = new System.Windows.Forms.Button();
            this.buildPositionGroupBox = new System.Windows.Forms.GroupBox();
            this.yBuildUpDown = new System.Windows.Forms.NumericUpDown();
            this.xBuildUpDown = new System.Windows.Forms.NumericUpDown();
            this.zBuildUpDown = new System.Windows.Forms.NumericUpDown();
            this.buildingsListBox = new System.Windows.Forms.ListBox();
            this.permissionsTabPage = new System.Windows.Forms.TabPage();
            this.transparencyBar = new System.Windows.Forms.TrackBar();
            this.scanUnusedCollisionTypesButton = new System.Windows.Forms.Button();
            this.clearCurrentButton = new System.Windows.Forms.Button();
            this.typeLabel = new System.Windows.Forms.Label();
            this.collisionLabel = new System.Windows.Forms.Label();
            this.typeGroupBox = new System.Windows.Forms.GroupBox();
            this.knownTypesRadioButton = new System.Windows.Forms.RadioButton();
            this.valueTypeRadioButton = new System.Windows.Forms.RadioButton();
            this.typePainterUpDown = new System.Windows.Forms.NumericUpDown();
            this.collisionTypePainterComboBox = new System.Windows.Forms.ComboBox();
            this.typePainterPictureBox = new System.Windows.Forms.PictureBox();
            this.collisionGroupBox = new System.Windows.Forms.GroupBox();
            this.collisionPainterComboBox = new System.Windows.Forms.ComboBox();
            this.collisionPainterPictureBox = new System.Windows.Forms.PictureBox();
            this.selectCollisionPanel = new System.Windows.Forms.Panel();
            this.collisionPictureBox = new System.Windows.Forms.PictureBox();
            this.selectTypePanel = new System.Windows.Forms.Panel();
            this.typePictureBox = new System.Windows.Forms.PictureBox();
            this.ImportMovButton = new System.Windows.Forms.Button();
            this.exportMovButton = new System.Windows.Forms.Button();
            this.modelTabPage = new System.Windows.Forms.TabPage();
            this.glbExportButton = new System.Windows.Forms.Button();
            this.daeExportButton = new System.Windows.Forms.Button();
            this.embedTexturesInMapModelCheckBox = new System.Windows.Forms.CheckBox();
            this.modelSizeLBL = new System.Windows.Forms.Label();
            this.nsbmdSizeLabel = new System.Windows.Forms.Label();
            this.unsupported3DModelEditLBL = new System.Windows.Forms.Label();
            this.importMapButton = new System.Windows.Forms.Button();
            this.exportMapButton = new System.Windows.Forms.Button();
            this.terrainTabPage = new System.Windows.Forms.TabPage();
            this.terrainSizeLBL = new System.Windows.Forms.Label();
            this.terrainDataLBL = new System.Windows.Forms.Label();
            this.unsupportedBDHCEditLBL = new System.Windows.Forms.Label();
            this.bdhcImportButton = new System.Windows.Forms.Button();
            this.bdhcExportButton = new System.Windows.Forms.Button();
            this.bgsTabPage = new System.Windows.Forms.TabPage();
            this.blankBGSButton = new System.Windows.Forms.Button();
            this.BGSSizeLBL = new System.Windows.Forms.Label();
            this.bgsDataLBL = new System.Windows.Forms.Label();
            this.unsupportedBGSEditLBL = new System.Windows.Forms.Label();
            this.soundPlatesImportButton = new System.Windows.Forms.Button();
            this.soundPlatesExportButton = new System.Windows.Forms.Button();
            this.flowLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.mapRenderPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.movPictureBox)).BeginInit();
            this.mapPartsTabControl.SuspendLayout();
            this.buildingsTabPage.SuspendLayout();
            this.groupBox33.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.yRotDegBldUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xRotDegBldUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.zRotDegBldUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.yRotBuildUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xRotBuildUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.zRotBuildUpDown)).BeginInit();
            this.lockXZgroupbox.SuspendLayout();
            this.bldRoundGroupbox.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.groupBox20.SuspendLayout();
            this.groupBox19.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.buildingHeightUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.buildingWidthUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.buildingLengthUpDown)).BeginInit();
            this.buildPositionGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.yBuildUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xBuildUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.zBuildUpDown)).BeginInit();
            this.permissionsTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.transparencyBar)).BeginInit();
            this.typeGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.typePainterUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.typePainterPictureBox)).BeginInit();
            this.collisionGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.collisionPainterPictureBox)).BeginInit();
            this.selectCollisionPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.collisionPictureBox)).BeginInit();
            this.selectTypePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.typePictureBox)).BeginInit();
            this.modelTabPage.SuspendLayout();
            this.terrainTabPage.SuspendLayout();
            this.bgsTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.mapScreenshotButton);
            this.flowLayoutPanel1.Controls.Add(this.wireframeCheckBox);
            this.flowLayoutPanel1.Controls.Add(this.radio3D);
            this.flowLayoutPanel1.Controls.Add(this.radio2D);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.BottomUp;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(1145, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(48, 646);
            this.flowLayoutPanel1.TabIndex = 42;
            // 
            // mapScreenshotButton
            // 
            this.mapScreenshotButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.mapScreenshotButton.Image = global::DSPRE.Properties.Resources.cameraIcon;
            this.mapScreenshotButton.Location = new System.Drawing.Point(3, 603);
            this.mapScreenshotButton.Name = "mapScreenshotButton";
            this.mapScreenshotButton.Size = new System.Drawing.Size(41, 40);
            this.mapScreenshotButton.TabIndex = 39;
            this.mapScreenshotButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.mapScreenshotButton.UseVisualStyleBackColor = true;
            this.mapScreenshotButton.Click += new System.EventHandler(this.mapScreenshotButton_Click);
            // 
            // wireframeCheckBox
            // 
            this.wireframeCheckBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.wireframeCheckBox.Appearance = System.Windows.Forms.Appearance.Button;
            this.wireframeCheckBox.AutoSize = true;
            this.wireframeCheckBox.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.wireframeCheckBox.Location = new System.Drawing.Point(8, 574);
            this.wireframeCheckBox.Name = "wireframeCheckBox";
            this.wireframeCheckBox.Size = new System.Drawing.Size(31, 23);
            this.wireframeCheckBox.TabIndex = 27;
            this.wireframeCheckBox.Text = " W";
            this.wireframeCheckBox.UseVisualStyleBackColor = true;
            this.wireframeCheckBox.CheckedChanged += new System.EventHandler(this.wireframeCheckBox_CheckedChanged);
            // 
            // radio3D
            // 
            this.radio3D.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.radio3D.Appearance = System.Windows.Forms.Appearance.Button;
            this.radio3D.AutoSize = true;
            this.radio3D.Checked = true;
            this.radio3D.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.radio3D.Location = new System.Drawing.Point(8, 545);
            this.radio3D.Name = "radio3D";
            this.radio3D.Size = new System.Drawing.Size(31, 23);
            this.radio3D.TabIndex = 26;
            this.radio3D.TabStop = true;
            this.radio3D.Text = "3D";
            this.radio3D.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.radio3D.UseVisualStyleBackColor = true;
            // 
            // radio2D
            // 
            this.radio2D.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.radio2D.Appearance = System.Windows.Forms.Appearance.Button;
            this.radio2D.AutoSize = true;
            this.radio2D.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.radio2D.Location = new System.Drawing.Point(8, 516);
            this.radio2D.Name = "radio2D";
            this.radio2D.Size = new System.Drawing.Size(31, 23);
            this.radio2D.TabIndex = 25;
            this.radio2D.Text = "2D";
            this.radio2D.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.radio2D.UseVisualStyleBackColor = true;
            this.radio2D.CheckedChanged += new System.EventHandler(this.radio2D_CheckedChanged);
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 4;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel3.Controls.Add(this.addMapFileButton, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.locateCurrentMapBin, 3, 2);
            this.tableLayoutPanel3.Controls.Add(this.removeMapFileButton, 2, 0);
            this.tableLayoutPanel3.Controls.Add(this.replaceMapBinButton, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.exportCurrentMapBinButton, 2, 1);
            this.tableLayoutPanel3.Controls.Add(this.saveMapButton, 0, 2);
            this.tableLayoutPanel3.Location = new System.Drawing.Point(271, 486);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 3;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(199, 129);
            this.tableLayoutPanel3.TabIndex = 41;
            // 
            // addMapFileButton
            // 
            this.tableLayoutPanel3.SetColumnSpan(this.addMapFileButton, 2);
            this.addMapFileButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.addMapFileButton.Image = ((System.Drawing.Image)(resources.GetObject("addMapFileButton.Image")));
            this.addMapFileButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.addMapFileButton.Location = new System.Drawing.Point(3, 3);
            this.addMapFileButton.Name = "addMapFileButton";
            this.addMapFileButton.Size = new System.Drawing.Size(92, 37);
            this.addMapFileButton.TabIndex = 36;
            this.addMapFileButton.Text = "Add \r\nMap File";
            this.addMapFileButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.addMapFileButton.UseVisualStyleBackColor = true;
            this.addMapFileButton.Click += new System.EventHandler(this.addMapFileButton_Click);
            // 
            // locateCurrentMapBin
            // 
            this.locateCurrentMapBin.Dock = System.Windows.Forms.DockStyle.Fill;
            this.locateCurrentMapBin.Image = global::DSPRE.Properties.Resources.open_file;
            this.locateCurrentMapBin.Location = new System.Drawing.Point(150, 89);
            this.locateCurrentMapBin.Name = "locateCurrentMapBin";
            this.locateCurrentMapBin.Size = new System.Drawing.Size(46, 37);
            this.locateCurrentMapBin.TabIndex = 40;
            this.locateCurrentMapBin.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.locateCurrentMapBin.UseVisualStyleBackColor = true;
            this.locateCurrentMapBin.Click += new System.EventHandler(this.locateCurrentMapBin_Click);
            // 
            // removeMapFileButton
            // 
            this.tableLayoutPanel3.SetColumnSpan(this.removeMapFileButton, 2);
            this.removeMapFileButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.removeMapFileButton.Image = global::DSPRE.Properties.Resources.deleteIcon;
            this.removeMapFileButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.removeMapFileButton.Location = new System.Drawing.Point(101, 3);
            this.removeMapFileButton.Name = "removeMapFileButton";
            this.removeMapFileButton.Size = new System.Drawing.Size(95, 37);
            this.removeMapFileButton.TabIndex = 35;
            this.removeMapFileButton.Text = "Remove \r\nLast Map";
            this.removeMapFileButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.removeMapFileButton.UseVisualStyleBackColor = true;
            this.removeMapFileButton.Click += new System.EventHandler(this.removeLastMapFileButton_Click);
            // 
            // replaceMapBinButton
            // 
            this.tableLayoutPanel3.SetColumnSpan(this.replaceMapBinButton, 2);
            this.replaceMapBinButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.replaceMapBinButton.Image = ((System.Drawing.Image)(resources.GetObject("replaceMapBinButton.Image")));
            this.replaceMapBinButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.replaceMapBinButton.Location = new System.Drawing.Point(3, 46);
            this.replaceMapBinButton.Name = "replaceMapBinButton";
            this.replaceMapBinButton.Size = new System.Drawing.Size(92, 37);
            this.replaceMapBinButton.TabIndex = 37;
            this.replaceMapBinButton.Text = "Replace \r\nMap BIN";
            this.replaceMapBinButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.replaceMapBinButton.UseVisualStyleBackColor = true;
            this.replaceMapBinButton.Click += new System.EventHandler(this.replaceMapBinButton_Click);
            // 
            // exportCurrentMapBinButton
            // 
            this.tableLayoutPanel3.SetColumnSpan(this.exportCurrentMapBinButton, 2);
            this.exportCurrentMapBinButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.exportCurrentMapBinButton.Image = ((System.Drawing.Image)(resources.GetObject("exportCurrentMapBinButton.Image")));
            this.exportCurrentMapBinButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.exportCurrentMapBinButton.Location = new System.Drawing.Point(101, 46);
            this.exportCurrentMapBinButton.Name = "exportCurrentMapBinButton";
            this.exportCurrentMapBinButton.Size = new System.Drawing.Size(95, 37);
            this.exportCurrentMapBinButton.TabIndex = 38;
            this.exportCurrentMapBinButton.Text = "Export \r\nMap BIN";
            this.exportCurrentMapBinButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.exportCurrentMapBinButton.UseVisualStyleBackColor = true;
            this.exportCurrentMapBinButton.Click += new System.EventHandler(this.exportCurrentMapBinButton_Click);
            // 
            // saveMapButton
            // 
            this.tableLayoutPanel3.SetColumnSpan(this.saveMapButton, 3);
            this.saveMapButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.saveMapButton.Image = global::DSPRE.Properties.Resources.save_rom;
            this.saveMapButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.saveMapButton.Location = new System.Drawing.Point(3, 89);
            this.saveMapButton.Name = "saveMapButton";
            this.saveMapButton.Size = new System.Drawing.Size(141, 37);
            this.saveMapButton.TabIndex = 34;
            this.saveMapButton.Text = "Save This\r\nMap BIN\r\n";
            this.saveMapButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.saveMapButton.UseVisualStyleBackColor = true;
            this.saveMapButton.Click += new System.EventHandler(this.saveMapButton_Click);
            // 
            // mapRenderPanel
            // 
            this.mapRenderPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mapRenderPanel.Controls.Add(this.mapOpenGlControl);
            this.mapRenderPanel.Controls.Add(this.movPictureBox);
            this.mapRenderPanel.Location = new System.Drawing.Point(519, 3);
            this.mapRenderPanel.Name = "mapRenderPanel";
            this.mapRenderPanel.Size = new System.Drawing.Size(610, 610);
            this.mapRenderPanel.TabIndex = 23;
            // 
            // mapOpenGlControl
            // 
            this.mapOpenGlControl.AccumBits = ((byte)(0));
            this.mapOpenGlControl.AutoCheckErrors = false;
            this.mapOpenGlControl.AutoFinish = false;
            this.mapOpenGlControl.AutoMakeCurrent = true;
            this.mapOpenGlControl.AutoSwapBuffers = true;
            this.mapOpenGlControl.BackColor = System.Drawing.Color.Black;
            this.mapOpenGlControl.ColorBits = ((byte)(32));
            this.mapOpenGlControl.DepthBits = ((byte)(64));
            this.mapOpenGlControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mapOpenGlControl.ForeColor = System.Drawing.Color.Black;
            this.mapOpenGlControl.Location = new System.Drawing.Point(0, 0);
            this.mapOpenGlControl.Name = "mapOpenGlControl";
            this.mapOpenGlControl.Size = new System.Drawing.Size(608, 608);
            this.mapOpenGlControl.StencilBits = ((byte)(0));
            this.mapOpenGlControl.TabIndex = 2;
            this.mapOpenGlControl.Load += new System.EventHandler(this.mapOpenGlControl_Load);
            this.mapOpenGlControl.Click += new System.EventHandler(this.mapOpenGlControl_Click);
            this.mapOpenGlControl.KeyUp += new System.Windows.Forms.KeyEventHandler(this.mapOpenGlControl_KeyUp);
            this.mapOpenGlControl.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.mapOpenGlControl_PreviewKeyDown);
            // 
            // movPictureBox
            // 
            this.movPictureBox.BackColor = System.Drawing.Color.White;
            this.movPictureBox.Location = new System.Drawing.Point(0, 0);
            this.movPictureBox.Name = "movPictureBox";
            this.movPictureBox.Size = new System.Drawing.Size(608, 608);
            this.movPictureBox.TabIndex = 3;
            this.movPictureBox.TabStop = false;
            this.movPictureBox.Click += new System.EventHandler(this.movPictureBox_Click);
            this.movPictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.movPictureBox_MouseMove);
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(16, 574);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(84, 13);
            this.label26.TabIndex = 33;
            this.label26.Text = "Buildings texture";
            // 
            // buildTextureComboBox
            // 
            this.buildTextureComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.buildTextureComboBox.FormattingEnabled = true;
            this.buildTextureComboBox.Location = new System.Drawing.Point(19, 588);
            this.buildTextureComboBox.Name = "buildTextureComboBox";
            this.buildTextureComboBox.Size = new System.Drawing.Size(233, 21);
            this.buildTextureComboBox.TabIndex = 32;
            this.buildTextureComboBox.SelectedIndexChanged += new System.EventHandler(this.buildTextureComboBox_SelectedIndexChanged);
            // 
            // mapFileLabel
            // 
            this.mapFileLabel.AutoSize = true;
            this.mapFileLabel.Location = new System.Drawing.Point(16, 486);
            this.mapFileLabel.Name = "mapFileLabel";
            this.mapFileLabel.Size = new System.Drawing.Size(44, 13);
            this.mapFileLabel.TabIndex = 31;
            this.mapFileLabel.Text = "Map file";
            // 
            // mapTextureComboBox
            // 
            this.mapTextureComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mapTextureComboBox.FormattingEnabled = true;
            this.mapTextureComboBox.Location = new System.Drawing.Point(19, 544);
            this.mapTextureComboBox.Name = "mapTextureComboBox";
            this.mapTextureComboBox.Size = new System.Drawing.Size(233, 21);
            this.mapTextureComboBox.TabIndex = 30;
            this.mapTextureComboBox.SelectedIndexChanged += new System.EventHandler(this.mapTextureComboBox_SelectedIndexChanged);
            // 
            // mapTextureLabel
            // 
            this.mapTextureLabel.AutoSize = true;
            this.mapTextureLabel.Location = new System.Drawing.Point(16, 530);
            this.mapTextureLabel.Name = "mapTextureLabel";
            this.mapTextureLabel.Size = new System.Drawing.Size(63, 13);
            this.mapTextureLabel.TabIndex = 29;
            this.mapTextureLabel.Text = "Map texture";
            // 
            // selectMapComboBox
            // 
            this.selectMapComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.selectMapComboBox.FormattingEnabled = true;
            this.selectMapComboBox.Location = new System.Drawing.Point(19, 500);
            this.selectMapComboBox.Name = "selectMapComboBox";
            this.selectMapComboBox.Size = new System.Drawing.Size(233, 21);
            this.selectMapComboBox.TabIndex = 28;
            this.selectMapComboBox.SelectedIndexChanged += new System.EventHandler(this.selectMapComboBox_SelectedIndexChanged);
            // 
            // mapPartsTabControl
            // 
            this.mapPartsTabControl.Controls.Add(this.buildingsTabPage);
            this.mapPartsTabControl.Controls.Add(this.permissionsTabPage);
            this.mapPartsTabControl.Controls.Add(this.modelTabPage);
            this.mapPartsTabControl.Controls.Add(this.terrainTabPage);
            this.mapPartsTabControl.Controls.Add(this.bgsTabPage);
            this.mapPartsTabControl.Location = new System.Drawing.Point(15, 6);
            this.mapPartsTabControl.Name = "mapPartsTabControl";
            this.mapPartsTabControl.SelectedIndex = 0;
            this.mapPartsTabControl.Size = new System.Drawing.Size(489, 476);
            this.mapPartsTabControl.TabIndex = 24;
            this.mapPartsTabControl.SelectedIndexChanged += new System.EventHandler(this.mapPartsTabControl_SelectedIndexChanged);
            // 
            // buildingsTabPage
            // 
            this.buildingsTabPage.Controls.Add(this.groupBox33);
            this.buildingsTabPage.Controls.Add(this.yLabel);
            this.buildingsTabPage.Controls.Add(this.lockXZgroupbox);
            this.buildingsTabPage.Controls.Add(this.zLabel);
            this.buildingsTabPage.Controls.Add(this.xLabel);
            this.buildingsTabPage.Controls.Add(this.bldRoundGroupbox);
            this.buildingsTabPage.Controls.Add(this.bldPlaceWithMouseCheckbox);
            this.buildingsTabPage.Controls.Add(this.importBuildingsButton);
            this.buildingsTabPage.Controls.Add(this.groupBox20);
            this.buildingsTabPage.Controls.Add(this.groupBox19);
            this.buildingsTabPage.Controls.Add(this.duplicateBuildingButton);
            this.buildingsTabPage.Controls.Add(this.exportBuildingsButton);
            this.buildingsTabPage.Controls.Add(this.removeBuildingButton);
            this.buildingsTabPage.Controls.Add(this.addBuildingButton);
            this.buildingsTabPage.Controls.Add(this.buildPositionGroupBox);
            this.buildingsTabPage.Controls.Add(this.buildingsListBox);
            this.buildingsTabPage.Location = new System.Drawing.Point(4, 22);
            this.buildingsTabPage.Name = "buildingsTabPage";
            this.buildingsTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.buildingsTabPage.Size = new System.Drawing.Size(481, 450);
            this.buildingsTabPage.TabIndex = 0;
            this.buildingsTabPage.Text = "Buildings";
            this.buildingsTabPage.UseVisualStyleBackColor = true;
            // 
            // groupBox33
            // 
            this.groupBox33.Controls.Add(this.yRotDegBldUpDown);
            this.groupBox33.Controls.Add(this.xRotDegBldUpDown);
            this.groupBox33.Controls.Add(this.zRotDegBldUpDown);
            this.groupBox33.Controls.Add(this.yRotBuildUpDown);
            this.groupBox33.Controls.Add(this.xRotBuildUpDown);
            this.groupBox33.Controls.Add(this.zRotBuildUpDown);
            this.groupBox33.Enabled = false;
            this.groupBox33.Location = new System.Drawing.Point(339, 141);
            this.groupBox33.Name = "groupBox33";
            this.groupBox33.Size = new System.Drawing.Size(131, 123);
            this.groupBox33.TabIndex = 44;
            this.groupBox33.TabStop = false;
            this.groupBox33.Text = "Rotation";
            // 
            // yRotDegBldUpDown
            // 
            this.yRotDegBldUpDown.DecimalPlaces = 2;
            this.yRotDegBldUpDown.Location = new System.Drawing.Point(66, 56);
            this.yRotDegBldUpDown.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.yRotDegBldUpDown.Name = "yRotDegBldUpDown";
            this.yRotDegBldUpDown.Size = new System.Drawing.Size(58, 20);
            this.yRotDegBldUpDown.TabIndex = 27;
            this.yRotDegBldUpDown.ValueChanged += new System.EventHandler(this.yRotDegBldUpDown_ValueChanged);
            // 
            // xRotDegBldUpDown
            // 
            this.xRotDegBldUpDown.DecimalPlaces = 2;
            this.xRotDegBldUpDown.Location = new System.Drawing.Point(66, 20);
            this.xRotDegBldUpDown.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.xRotDegBldUpDown.Name = "xRotDegBldUpDown";
            this.xRotDegBldUpDown.Size = new System.Drawing.Size(58, 20);
            this.xRotDegBldUpDown.TabIndex = 25;
            this.xRotDegBldUpDown.ValueChanged += new System.EventHandler(this.xRotDegBldUpDown_ValueChanged);
            // 
            // zRotDegBldUpDown
            // 
            this.zRotDegBldUpDown.DecimalPlaces = 2;
            this.zRotDegBldUpDown.Location = new System.Drawing.Point(66, 93);
            this.zRotDegBldUpDown.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.zRotDegBldUpDown.Name = "zRotDegBldUpDown";
            this.zRotDegBldUpDown.Size = new System.Drawing.Size(58, 20);
            this.zRotDegBldUpDown.TabIndex = 26;
            this.zRotDegBldUpDown.ValueChanged += new System.EventHandler(this.zRotDegBldUpDown_ValueChanged);
            // 
            // yRotBuildUpDown
            // 
            this.yRotBuildUpDown.Location = new System.Drawing.Point(6, 56);
            this.yRotBuildUpDown.Maximum = new decimal(new int[] {
            65536,
            0,
            0,
            0});
            this.yRotBuildUpDown.Name = "yRotBuildUpDown";
            this.yRotBuildUpDown.Size = new System.Drawing.Size(56, 20);
            this.yRotBuildUpDown.TabIndex = 24;
            this.yRotBuildUpDown.ValueChanged += new System.EventHandler(this.yRotBuildUpDown_ValueChanged);
            // 
            // xRotBuildUpDown
            // 
            this.xRotBuildUpDown.Location = new System.Drawing.Point(6, 20);
            this.xRotBuildUpDown.Maximum = new decimal(new int[] {
            65536,
            0,
            0,
            0});
            this.xRotBuildUpDown.Name = "xRotBuildUpDown";
            this.xRotBuildUpDown.Size = new System.Drawing.Size(56, 20);
            this.xRotBuildUpDown.TabIndex = 22;
            this.xRotBuildUpDown.ValueChanged += new System.EventHandler(this.xRotBuildUpDown_ValueChanged);
            // 
            // zRotBuildUpDown
            // 
            this.zRotBuildUpDown.Location = new System.Drawing.Point(6, 93);
            this.zRotBuildUpDown.Maximum = new decimal(new int[] {
            65536,
            0,
            0,
            0});
            this.zRotBuildUpDown.Name = "zRotBuildUpDown";
            this.zRotBuildUpDown.Size = new System.Drawing.Size(56, 20);
            this.zRotBuildUpDown.TabIndex = 23;
            this.zRotBuildUpDown.ValueChanged += new System.EventHandler(this.zRotBuildUpDown_ValueChanged);
            // 
            // yLabel
            // 
            this.yLabel.AutoSize = true;
            this.yLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.yLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.yLabel.Location = new System.Drawing.Point(183, 198);
            this.yLabel.Name = "yLabel";
            this.yLabel.Size = new System.Drawing.Size(15, 15);
            this.yLabel.TabIndex = 10;
            this.yLabel.Text = "Y";
            // 
            // lockXZgroupbox
            // 
            this.lockXZgroupbox.Controls.Add(this.bldPlaceLockXcheckbox);
            this.lockXZgroupbox.Controls.Add(this.bldPlaceLockZcheckbox);
            this.lockXZgroupbox.Enabled = false;
            this.lockXZgroupbox.Location = new System.Drawing.Point(272, 271);
            this.lockXZgroupbox.Name = "lockXZgroupbox";
            this.lockXZgroupbox.Size = new System.Drawing.Size(154, 36);
            this.lockXZgroupbox.TabIndex = 43;
            this.lockXZgroupbox.TabStop = false;
            // 
            // bldPlaceLockXcheckbox
            // 
            this.bldPlaceLockXcheckbox.Appearance = System.Windows.Forms.Appearance.Button;
            this.bldPlaceLockXcheckbox.AutoSize = true;
            this.bldPlaceLockXcheckbox.Enabled = false;
            this.bldPlaceLockXcheckbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bldPlaceLockXcheckbox.ForeColor = System.Drawing.Color.Red;
            this.bldPlaceLockXcheckbox.Location = new System.Drawing.Point(10, 9);
            this.bldPlaceLockXcheckbox.Name = "bldPlaceLockXcheckbox";
            this.bldPlaceLockXcheckbox.Size = new System.Drawing.Size(57, 23);
            this.bldPlaceLockXcheckbox.TabIndex = 41;
            this.bldPlaceLockXcheckbox.Text = "Lock X";
            this.bldPlaceLockXcheckbox.UseVisualStyleBackColor = true;
            this.bldPlaceLockXcheckbox.CheckedChanged += new System.EventHandler(this.bldPlaceLockXcheckbox_CheckedChanged);
            // 
            // bldPlaceLockZcheckbox
            // 
            this.bldPlaceLockZcheckbox.Appearance = System.Windows.Forms.Appearance.Button;
            this.bldPlaceLockZcheckbox.AutoSize = true;
            this.bldPlaceLockZcheckbox.Enabled = false;
            this.bldPlaceLockZcheckbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bldPlaceLockZcheckbox.ForeColor = System.Drawing.Color.Blue;
            this.bldPlaceLockZcheckbox.Location = new System.Drawing.Point(85, 9);
            this.bldPlaceLockZcheckbox.Name = "bldPlaceLockZcheckbox";
            this.bldPlaceLockZcheckbox.Size = new System.Drawing.Size(57, 23);
            this.bldPlaceLockZcheckbox.TabIndex = 42;
            this.bldPlaceLockZcheckbox.Text = "Lock Z";
            this.bldPlaceLockZcheckbox.UseVisualStyleBackColor = true;
            this.bldPlaceLockZcheckbox.CheckedChanged += new System.EventHandler(this.bldPlaceLockZcheckbox_CheckedChanged);
            // 
            // zLabel
            // 
            this.zLabel.AutoSize = true;
            this.zLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.zLabel.ForeColor = System.Drawing.Color.Blue;
            this.zLabel.Location = new System.Drawing.Point(183, 233);
            this.zLabel.Name = "zLabel";
            this.zLabel.Size = new System.Drawing.Size(15, 15);
            this.zLabel.TabIndex = 9;
            this.zLabel.Text = "Z";
            // 
            // xLabel
            // 
            this.xLabel.AutoSize = true;
            this.xLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xLabel.ForeColor = System.Drawing.Color.Red;
            this.xLabel.Location = new System.Drawing.Point(182, 162);
            this.xLabel.Name = "xLabel";
            this.xLabel.Size = new System.Drawing.Size(16, 15);
            this.xLabel.TabIndex = 8;
            this.xLabel.Text = "X";
            // 
            // bldRoundGroupbox
            // 
            this.bldRoundGroupbox.Controls.Add(this.tableLayoutPanel2);
            this.bldRoundGroupbox.Enabled = false;
            this.bldRoundGroupbox.Location = new System.Drawing.Point(195, 316);
            this.bldRoundGroupbox.Name = "bldRoundGroupbox";
            this.bldRoundGroupbox.Size = new System.Drawing.Size(269, 83);
            this.bldRoundGroupbox.TabIndex = 40;
            this.bldRoundGroupbox.TabStop = false;
            this.bldRoundGroupbox.Text = "Round";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel2.Controls.Add(this.bldRoundDecmil, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.bldRoundCentMil, 2, 1);
            this.tableLayoutPanel2.Controls.Add(this.bldRoundWhole, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.bldRoundDec, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.bldRoundCent, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.bldRoundMil, 0, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(263, 64);
            this.tableLayoutPanel2.TabIndex = 45;
            // 
            // bldRoundDecmil
            // 
            this.bldRoundDecmil.Appearance = System.Windows.Forms.Appearance.Button;
            this.bldRoundDecmil.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bldRoundDecmil.Location = new System.Drawing.Point(90, 35);
            this.bldRoundDecmil.Name = "bldRoundDecmil";
            this.bldRoundDecmil.Size = new System.Drawing.Size(81, 26);
            this.bldRoundDecmil.TabIndex = 4;
            this.bldRoundDecmil.Text = ".0001";
            this.bldRoundDecmil.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.bldRoundDecmil.UseVisualStyleBackColor = true;
            this.bldRoundDecmil.CheckedChanged += new System.EventHandler(this.bldRoundDecmil_CheckedChanged);
            // 
            // bldRoundCentMil
            // 
            this.bldRoundCentMil.Appearance = System.Windows.Forms.Appearance.Button;
            this.bldRoundCentMil.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bldRoundCentMil.Location = new System.Drawing.Point(177, 35);
            this.bldRoundCentMil.Name = "bldRoundCentMil";
            this.bldRoundCentMil.Size = new System.Drawing.Size(83, 26);
            this.bldRoundCentMil.TabIndex = 5;
            this.bldRoundCentMil.Text = ".00001";
            this.bldRoundCentMil.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.bldRoundCentMil.UseVisualStyleBackColor = true;
            this.bldRoundCentMil.CheckedChanged += new System.EventHandler(this.bldRoundCentMil_CheckedChanged);
            // 
            // bldRoundWhole
            // 
            this.bldRoundWhole.Appearance = System.Windows.Forms.Appearance.Button;
            this.bldRoundWhole.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bldRoundWhole.Location = new System.Drawing.Point(3, 3);
            this.bldRoundWhole.Name = "bldRoundWhole";
            this.bldRoundWhole.Size = new System.Drawing.Size(81, 26);
            this.bldRoundWhole.TabIndex = 0;
            this.bldRoundWhole.Text = "Whole";
            this.bldRoundWhole.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.bldRoundWhole.UseVisualStyleBackColor = true;
            this.bldRoundWhole.CheckedChanged += new System.EventHandler(this.bldRoundWhole_CheckedChanged);
            // 
            // bldRoundDec
            // 
            this.bldRoundDec.Appearance = System.Windows.Forms.Appearance.Button;
            this.bldRoundDec.Checked = true;
            this.bldRoundDec.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bldRoundDec.Location = new System.Drawing.Point(90, 3);
            this.bldRoundDec.Name = "bldRoundDec";
            this.bldRoundDec.Size = new System.Drawing.Size(81, 26);
            this.bldRoundDec.TabIndex = 1;
            this.bldRoundDec.TabStop = true;
            this.bldRoundDec.Text = ".1";
            this.bldRoundDec.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.bldRoundDec.UseVisualStyleBackColor = true;
            this.bldRoundDec.CheckedChanged += new System.EventHandler(this.bldRoundDec_CheckedChanged);
            // 
            // bldRoundCent
            // 
            this.bldRoundCent.Appearance = System.Windows.Forms.Appearance.Button;
            this.bldRoundCent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bldRoundCent.Location = new System.Drawing.Point(177, 3);
            this.bldRoundCent.Name = "bldRoundCent";
            this.bldRoundCent.Size = new System.Drawing.Size(83, 26);
            this.bldRoundCent.TabIndex = 2;
            this.bldRoundCent.Text = ".01";
            this.bldRoundCent.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.bldRoundCent.UseVisualStyleBackColor = true;
            this.bldRoundCent.CheckedChanged += new System.EventHandler(this.bldRoundCent_CheckedChanged);
            // 
            // bldRoundMil
            // 
            this.bldRoundMil.Appearance = System.Windows.Forms.Appearance.Button;
            this.bldRoundMil.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bldRoundMil.Location = new System.Drawing.Point(3, 35);
            this.bldRoundMil.Name = "bldRoundMil";
            this.bldRoundMil.Size = new System.Drawing.Size(81, 26);
            this.bldRoundMil.TabIndex = 3;
            this.bldRoundMil.Text = ".001";
            this.bldRoundMil.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.bldRoundMil.UseVisualStyleBackColor = true;
            this.bldRoundMil.CheckedChanged += new System.EventHandler(this.bldRoundMil_CheckedChanged);
            // 
            // bldPlaceWithMouseCheckbox
            // 
            this.bldPlaceWithMouseCheckbox.Appearance = System.Windows.Forms.Appearance.Button;
            this.bldPlaceWithMouseCheckbox.Enabled = false;
            this.bldPlaceWithMouseCheckbox.Image = global::DSPRE.Properties.Resources.selectBldWithMouse;
            this.bldPlaceWithMouseCheckbox.Location = new System.Drawing.Point(227, 271);
            this.bldPlaceWithMouseCheckbox.Name = "bldPlaceWithMouseCheckbox";
            this.bldPlaceWithMouseCheckbox.Size = new System.Drawing.Size(39, 40);
            this.bldPlaceWithMouseCheckbox.TabIndex = 40;
            this.bldPlaceWithMouseCheckbox.UseVisualStyleBackColor = true;
            this.bldPlaceWithMouseCheckbox.CheckedChanged += new System.EventHandler(this.bldPlaceWithMouseCheckbox_CheckedChanged);
            // 
            // importBuildingsButton
            // 
            this.importBuildingsButton.Image = ((System.Drawing.Image)(resources.GetObject("importBuildingsButton.Image")));
            this.importBuildingsButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.importBuildingsButton.Location = new System.Drawing.Point(193, 11);
            this.importBuildingsButton.Name = "importBuildingsButton";
            this.importBuildingsButton.Size = new System.Drawing.Size(102, 40);
            this.importBuildingsButton.TabIndex = 21;
            this.importBuildingsButton.Text = "Import\r\nBuildings";
            this.importBuildingsButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.importBuildingsButton.UseVisualStyleBackColor = true;
            this.importBuildingsButton.Click += new System.EventHandler(this.importBuildingsButton_Click);
            // 
            // groupBox20
            // 
            this.groupBox20.Controls.Add(this.interiorbldRadioButton);
            this.groupBox20.Controls.Add(this.exteriorbldRadioButton);
            this.groupBox20.Controls.Add(this.buildIndexComboBox);
            this.groupBox20.Location = new System.Drawing.Point(189, 57);
            this.groupBox20.Name = "groupBox20";
            this.groupBox20.Size = new System.Drawing.Size(284, 81);
            this.groupBox20.TabIndex = 26;
            this.groupBox20.TabStop = false;
            this.groupBox20.Text = "Building Selector";
            // 
            // interiorbldRadioButton
            // 
            this.interiorbldRadioButton.Appearance = System.Windows.Forms.Appearance.Button;
            this.interiorbldRadioButton.AutoSize = true;
            this.interiorbldRadioButton.Enabled = false;
            this.interiorbldRadioButton.Location = new System.Drawing.Point(9, 18);
            this.interiorbldRadioButton.Name = "interiorbldRadioButton";
            this.interiorbldRadioButton.Size = new System.Drawing.Size(68, 23);
            this.interiorbldRadioButton.TabIndex = 3;
            this.interiorbldRadioButton.Text = "Interior List";
            this.interiorbldRadioButton.UseVisualStyleBackColor = true;
            this.interiorbldRadioButton.CheckedChanged += new System.EventHandler(this.interiorRadioButton_CheckedChanged);
            // 
            // exteriorbldRadioButton
            // 
            this.exteriorbldRadioButton.Appearance = System.Windows.Forms.Appearance.Button;
            this.exteriorbldRadioButton.AutoSize = true;
            this.exteriorbldRadioButton.Checked = true;
            this.exteriorbldRadioButton.Enabled = false;
            this.exteriorbldRadioButton.Location = new System.Drawing.Point(83, 18);
            this.exteriorbldRadioButton.Name = "exteriorbldRadioButton";
            this.exteriorbldRadioButton.Size = new System.Drawing.Size(71, 23);
            this.exteriorbldRadioButton.TabIndex = 4;
            this.exteriorbldRadioButton.TabStop = true;
            this.exteriorbldRadioButton.Text = "Exterior List";
            this.exteriorbldRadioButton.UseVisualStyleBackColor = true;
            // 
            // buildIndexComboBox
            // 
            this.buildIndexComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.buildIndexComboBox.FormattingEnabled = true;
            this.buildIndexComboBox.Location = new System.Drawing.Point(9, 48);
            this.buildIndexComboBox.Name = "buildIndexComboBox";
            this.buildIndexComboBox.Size = new System.Drawing.Size(264, 21);
            this.buildIndexComboBox.TabIndex = 1;
            this.buildIndexComboBox.SelectedIndexChanged += new System.EventHandler(this.buildIndexComboBox_SelectedIndexChanged);
            // 
            // groupBox19
            // 
            this.groupBox19.Controls.Add(this.buildingHeightUpDown);
            this.groupBox19.Controls.Add(this.buildingWidthUpDown);
            this.groupBox19.Controls.Add(this.buildingLengthUpDown);
            this.groupBox19.Location = new System.Drawing.Point(283, 141);
            this.groupBox19.Name = "groupBox19";
            this.groupBox19.Size = new System.Drawing.Size(53, 123);
            this.groupBox19.TabIndex = 12;
            this.groupBox19.TabStop = false;
            this.groupBox19.Text = "Scale";
            // 
            // buildingHeightUpDown
            // 
            this.buildingHeightUpDown.Location = new System.Drawing.Point(4, 56);
            this.buildingHeightUpDown.Name = "buildingHeightUpDown";
            this.buildingHeightUpDown.Size = new System.Drawing.Size(43, 20);
            this.buildingHeightUpDown.TabIndex = 24;
            this.buildingHeightUpDown.ValueChanged += new System.EventHandler(this.buildingHeightUpDown_ValueChanged);
            // 
            // buildingWidthUpDown
            // 
            this.buildingWidthUpDown.Location = new System.Drawing.Point(4, 20);
            this.buildingWidthUpDown.Name = "buildingWidthUpDown";
            this.buildingWidthUpDown.Size = new System.Drawing.Size(43, 20);
            this.buildingWidthUpDown.TabIndex = 22;
            this.buildingWidthUpDown.ValueChanged += new System.EventHandler(this.buildingWidthUpDown_ValueChanged);
            // 
            // buildingLengthUpDown
            // 
            this.buildingLengthUpDown.Location = new System.Drawing.Point(4, 93);
            this.buildingLengthUpDown.Name = "buildingLengthUpDown";
            this.buildingLengthUpDown.Size = new System.Drawing.Size(43, 20);
            this.buildingLengthUpDown.TabIndex = 23;
            this.buildingLengthUpDown.ValueChanged += new System.EventHandler(this.buildingLengthUpDown_ValueChanged);
            // 
            // duplicateBuildingButton
            // 
            this.duplicateBuildingButton.Image = global::DSPRE.Properties.Resources.copyIcon_small;
            this.duplicateBuildingButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.duplicateBuildingButton.Location = new System.Drawing.Point(352, 405);
            this.duplicateBuildingButton.Name = "duplicateBuildingButton";
            this.duplicateBuildingButton.Size = new System.Drawing.Size(80, 32);
            this.duplicateBuildingButton.TabIndex = 25;
            this.duplicateBuildingButton.Text = "Duplicate";
            this.duplicateBuildingButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.duplicateBuildingButton.UseVisualStyleBackColor = true;
            this.duplicateBuildingButton.Click += new System.EventHandler(this.duplicateBuildingButton_Click);
            // 
            // exportBuildingsButton
            // 
            this.exportBuildingsButton.Image = ((System.Drawing.Image)(resources.GetObject("exportBuildingsButton.Image")));
            this.exportBuildingsButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.exportBuildingsButton.Location = new System.Drawing.Point(298, 11);
            this.exportBuildingsButton.Name = "exportBuildingsButton";
            this.exportBuildingsButton.Size = new System.Drawing.Size(102, 40);
            this.exportBuildingsButton.TabIndex = 20;
            this.exportBuildingsButton.Text = "Export\r\nBuildings";
            this.exportBuildingsButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.exportBuildingsButton.UseVisualStyleBackColor = true;
            this.exportBuildingsButton.Click += new System.EventHandler(this.exportBuildingsButton_Click);
            // 
            // removeBuildingButton
            // 
            this.removeBuildingButton.Image = global::DSPRE.Properties.Resources.deleteIcon;
            this.removeBuildingButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.removeBuildingButton.Location = new System.Drawing.Point(281, 405);
            this.removeBuildingButton.Name = "removeBuildingButton";
            this.removeBuildingButton.Size = new System.Drawing.Size(70, 32);
            this.removeBuildingButton.TabIndex = 13;
            this.removeBuildingButton.Text = "Delete";
            this.removeBuildingButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.removeBuildingButton.UseVisualStyleBackColor = true;
            this.removeBuildingButton.Click += new System.EventHandler(this.removeBuildingButton_Click);
            // 
            // addBuildingButton
            // 
            this.addBuildingButton.Image = ((System.Drawing.Image)(resources.GetObject("addBuildingButton.Image")));
            this.addBuildingButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.addBuildingButton.Location = new System.Drawing.Point(224, 405);
            this.addBuildingButton.Name = "addBuildingButton";
            this.addBuildingButton.Size = new System.Drawing.Size(56, 32);
            this.addBuildingButton.TabIndex = 12;
            this.addBuildingButton.Text = "Add";
            this.addBuildingButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.addBuildingButton.UseVisualStyleBackColor = true;
            this.addBuildingButton.Click += new System.EventHandler(this.addBuildingButton_Click);
            // 
            // buildPositionGroupBox
            // 
            this.buildPositionGroupBox.Controls.Add(this.yBuildUpDown);
            this.buildPositionGroupBox.Controls.Add(this.xBuildUpDown);
            this.buildPositionGroupBox.Controls.Add(this.zBuildUpDown);
            this.buildPositionGroupBox.Location = new System.Drawing.Point(198, 141);
            this.buildPositionGroupBox.Name = "buildPositionGroupBox";
            this.buildPositionGroupBox.Size = new System.Drawing.Size(81, 123);
            this.buildPositionGroupBox.TabIndex = 11;
            this.buildPositionGroupBox.TabStop = false;
            this.buildPositionGroupBox.Text = "Position";
            // 
            // yBuildUpDown
            // 
            this.yBuildUpDown.DecimalPlaces = 5;
            this.yBuildUpDown.Location = new System.Drawing.Point(6, 56);
            this.yBuildUpDown.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.yBuildUpDown.Minimum = new decimal(new int[] {
            255,
            0,
            0,
            -2147483648});
            this.yBuildUpDown.Name = "yBuildUpDown";
            this.yBuildUpDown.Size = new System.Drawing.Size(69, 20);
            this.yBuildUpDown.TabIndex = 7;
            this.yBuildUpDown.ValueChanged += new System.EventHandler(this.yBuildUpDown_ValueChanged);
            // 
            // xBuildUpDown
            // 
            this.xBuildUpDown.DecimalPlaces = 5;
            this.xBuildUpDown.Location = new System.Drawing.Point(6, 20);
            this.xBuildUpDown.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.xBuildUpDown.Minimum = new decimal(new int[] {
            255,
            0,
            0,
            -2147483648});
            this.xBuildUpDown.Name = "xBuildUpDown";
            this.xBuildUpDown.Size = new System.Drawing.Size(69, 20);
            this.xBuildUpDown.TabIndex = 5;
            this.xBuildUpDown.ValueChanged += new System.EventHandler(this.xBuildUpDown_ValueChanged);
            // 
            // zBuildUpDown
            // 
            this.zBuildUpDown.DecimalPlaces = 5;
            this.zBuildUpDown.Location = new System.Drawing.Point(6, 93);
            this.zBuildUpDown.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.zBuildUpDown.Minimum = new decimal(new int[] {
            255,
            0,
            0,
            -2147483648});
            this.zBuildUpDown.Name = "zBuildUpDown";
            this.zBuildUpDown.Size = new System.Drawing.Size(69, 20);
            this.zBuildUpDown.TabIndex = 6;
            this.zBuildUpDown.ValueChanged += new System.EventHandler(this.zBuildUpDown_ValueChanged);
            // 
            // buildingsListBox
            // 
            this.buildingsListBox.FormattingEnabled = true;
            this.buildingsListBox.Location = new System.Drawing.Point(9, 7);
            this.buildingsListBox.Name = "buildingsListBox";
            this.buildingsListBox.Size = new System.Drawing.Size(168, 433);
            this.buildingsListBox.TabIndex = 0;
            this.buildingsListBox.SelectedIndexChanged += new System.EventHandler(this.buildingsListBox_SelectedIndexChanged);
            // 
            // permissionsTabPage
            // 
            this.permissionsTabPage.Controls.Add(this.transparencyBar);
            this.permissionsTabPage.Controls.Add(this.scanUnusedCollisionTypesButton);
            this.permissionsTabPage.Controls.Add(this.clearCurrentButton);
            this.permissionsTabPage.Controls.Add(this.typeLabel);
            this.permissionsTabPage.Controls.Add(this.collisionLabel);
            this.permissionsTabPage.Controls.Add(this.typeGroupBox);
            this.permissionsTabPage.Controls.Add(this.collisionGroupBox);
            this.permissionsTabPage.Controls.Add(this.selectCollisionPanel);
            this.permissionsTabPage.Controls.Add(this.selectTypePanel);
            this.permissionsTabPage.Controls.Add(this.ImportMovButton);
            this.permissionsTabPage.Controls.Add(this.exportMovButton);
            this.permissionsTabPage.Location = new System.Drawing.Point(4, 22);
            this.permissionsTabPage.Name = "permissionsTabPage";
            this.permissionsTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.permissionsTabPage.Size = new System.Drawing.Size(481, 450);
            this.permissionsTabPage.TabIndex = 1;
            this.permissionsTabPage.Text = "Move Permissions";
            this.permissionsTabPage.UseVisualStyleBackColor = true;
            // 
            // transparencyBar
            // 
            this.transparencyBar.BackColor = System.Drawing.SystemColors.Menu;
            this.transparencyBar.Location = new System.Drawing.Point(12, 391);
            this.transparencyBar.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.transparencyBar.Maximum = 255;
            this.transparencyBar.Name = "transparencyBar";
            this.transparencyBar.Size = new System.Drawing.Size(445, 45);
            this.transparencyBar.TabIndex = 42;
            this.transparencyBar.TickFrequency = 255;
            this.transparencyBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.transparencyBar.Scroll += new System.EventHandler(this.transparencyBar_Scroll);
            // 
            // scanUnusedCollisionTypesButton
            // 
            this.scanUnusedCollisionTypesButton.Image = global::DSPRE.Properties.Resources.SearchMiniIcon;
            this.scanUnusedCollisionTypesButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.scanUnusedCollisionTypesButton.Location = new System.Drawing.Point(345, 108);
            this.scanUnusedCollisionTypesButton.Name = "scanUnusedCollisionTypesButton";
            this.scanUnusedCollisionTypesButton.Size = new System.Drawing.Size(111, 27);
            this.scanUnusedCollisionTypesButton.TabIndex = 33;
            this.scanUnusedCollisionTypesButton.Text = "Scan used types";
            this.scanUnusedCollisionTypesButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.scanUnusedCollisionTypesButton.UseVisualStyleBackColor = true;
            this.scanUnusedCollisionTypesButton.Click += new System.EventHandler(this.scanUsedCollisionTypesButton_Click);
            // 
            // clearCurrentButton
            // 
            this.clearCurrentButton.Location = new System.Drawing.Point(11, 134);
            this.clearCurrentButton.Name = "clearCurrentButton";
            this.clearCurrentButton.Size = new System.Drawing.Size(212, 23);
            this.clearCurrentButton.TabIndex = 32;
            this.clearCurrentButton.Text = "Clear current";
            this.clearCurrentButton.UseVisualStyleBackColor = true;
            this.clearCurrentButton.Click += new System.EventHandler(this.clearCurrentButton_Click);
            // 
            // typeLabel
            // 
            this.typeLabel.AutoSize = true;
            this.typeLabel.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.typeLabel.Location = new System.Drawing.Point(155, 9);
            this.typeLabel.Name = "typeLabel";
            this.typeLabel.Size = new System.Drawing.Size(31, 13);
            this.typeLabel.TabIndex = 29;
            this.typeLabel.Text = "Type";
            // 
            // collisionLabel
            // 
            this.collisionLabel.AutoSize = true;
            this.collisionLabel.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.collisionLabel.Location = new System.Drawing.Point(40, 9);
            this.collisionLabel.Name = "collisionLabel";
            this.collisionLabel.Size = new System.Drawing.Size(45, 13);
            this.collisionLabel.TabIndex = 28;
            this.collisionLabel.Text = "Collision";
            // 
            // typeGroupBox
            // 
            this.typeGroupBox.Controls.Add(this.knownTypesRadioButton);
            this.typeGroupBox.Controls.Add(this.valueTypeRadioButton);
            this.typeGroupBox.Controls.Add(this.typePainterUpDown);
            this.typeGroupBox.Controls.Add(this.collisionTypePainterComboBox);
            this.typeGroupBox.Controls.Add(this.typePainterPictureBox);
            this.typeGroupBox.Enabled = false;
            this.typeGroupBox.Location = new System.Drawing.Point(13, 272);
            this.typeGroupBox.Name = "typeGroupBox";
            this.typeGroupBox.Size = new System.Drawing.Size(444, 113);
            this.typeGroupBox.TabIndex = 25;
            this.typeGroupBox.TabStop = false;
            this.typeGroupBox.Text = "Type Painter";
            // 
            // knownTypesRadioButton
            // 
            this.knownTypesRadioButton.AutoSize = true;
            this.knownTypesRadioButton.Checked = true;
            this.knownTypesRadioButton.Location = new System.Drawing.Point(135, 21);
            this.knownTypesRadioButton.Name = "knownTypesRadioButton";
            this.knownTypesRadioButton.Size = new System.Drawing.Size(90, 17);
            this.knownTypesRadioButton.TabIndex = 5;
            this.knownTypesRadioButton.TabStop = true;
            this.knownTypesRadioButton.Text = "Known Types";
            this.knownTypesRadioButton.UseVisualStyleBackColor = true;
            this.knownTypesRadioButton.CheckedChanged += new System.EventHandler(this.typesRadioButton_CheckedChanged);
            // 
            // valueTypeRadioButton
            // 
            this.valueTypeRadioButton.AutoSize = true;
            this.valueTypeRadioButton.Location = new System.Drawing.Point(341, 19);
            this.valueTypeRadioButton.Name = "valueTypeRadioButton";
            this.valueTypeRadioButton.Size = new System.Drawing.Size(52, 17);
            this.valueTypeRadioButton.TabIndex = 4;
            this.valueTypeRadioButton.Text = "Value";
            this.valueTypeRadioButton.UseVisualStyleBackColor = true;
            this.valueTypeRadioButton.CheckedChanged += new System.EventHandler(this.valueTypeRadioButton_CheckedChanged);
            // 
            // typePainterUpDown
            // 
            this.typePainterUpDown.Enabled = false;
            this.typePainterUpDown.Hexadecimal = true;
            this.typePainterUpDown.Location = new System.Drawing.Point(341, 45);
            this.typePainterUpDown.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.typePainterUpDown.Name = "typePainterUpDown";
            this.typePainterUpDown.Size = new System.Drawing.Size(78, 20);
            this.typePainterUpDown.TabIndex = 3;
            this.typePainterUpDown.ValueChanged += new System.EventHandler(this.typePainterUpDown_ValueChanged);
            // 
            // collisionTypePainterComboBox
            // 
            this.collisionTypePainterComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.collisionTypePainterComboBox.FormattingEnabled = true;
            this.collisionTypePainterComboBox.IntegralHeight = false;
            this.collisionTypePainterComboBox.Location = new System.Drawing.Point(135, 44);
            this.collisionTypePainterComboBox.MaxDropDownItems = 10;
            this.collisionTypePainterComboBox.Name = "collisionTypePainterComboBox";
            this.collisionTypePainterComboBox.Size = new System.Drawing.Size(200, 21);
            this.collisionTypePainterComboBox.TabIndex = 2;
            this.collisionTypePainterComboBox.SelectedIndexChanged += new System.EventHandler(this.typePainterComboBox_SelectedIndexChanged);
            // 
            // typePainterPictureBox
            // 
            this.typePainterPictureBox.BackColor = System.Drawing.Color.White;
            this.typePainterPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.typePainterPictureBox.Location = new System.Drawing.Point(18, 31);
            this.typePainterPictureBox.Name = "typePainterPictureBox";
            this.typePainterPictureBox.Size = new System.Drawing.Size(66, 64);
            this.typePainterPictureBox.TabIndex = 1;
            this.typePainterPictureBox.TabStop = false;
            // 
            // collisionGroupBox
            // 
            this.collisionGroupBox.Controls.Add(this.collisionPainterComboBox);
            this.collisionGroupBox.Controls.Add(this.collisionPainterPictureBox);
            this.collisionGroupBox.Location = new System.Drawing.Point(12, 167);
            this.collisionGroupBox.Name = "collisionGroupBox";
            this.collisionGroupBox.Size = new System.Drawing.Size(444, 99);
            this.collisionGroupBox.TabIndex = 24;
            this.collisionGroupBox.TabStop = false;
            this.collisionGroupBox.Text = "Collision Painter";
            // 
            // collisionPainterComboBox
            // 
            this.collisionPainterComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.collisionPainterComboBox.FormattingEnabled = true;
            this.collisionPainterComboBox.Location = new System.Drawing.Point(134, 23);
            this.collisionPainterComboBox.Name = "collisionPainterComboBox";
            this.collisionPainterComboBox.Size = new System.Drawing.Size(284, 21);
            this.collisionPainterComboBox.TabIndex = 1;
            this.collisionPainterComboBox.SelectedIndexChanged += new System.EventHandler(this.collisionPainterComboBox_SelectedIndexChange);
            // 
            // collisionPainterPictureBox
            // 
            this.collisionPainterPictureBox.BackColor = System.Drawing.Color.White;
            this.collisionPainterPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.collisionPainterPictureBox.Location = new System.Drawing.Point(18, 23);
            this.collisionPainterPictureBox.Name = "collisionPainterPictureBox";
            this.collisionPainterPictureBox.Size = new System.Drawing.Size(66, 64);
            this.collisionPainterPictureBox.TabIndex = 0;
            this.collisionPainterPictureBox.TabStop = false;
            // 
            // selectCollisionPanel
            // 
            this.selectCollisionPanel.Controls.Add(this.collisionPictureBox);
            this.selectCollisionPanel.Location = new System.Drawing.Point(10, 23);
            this.selectCollisionPanel.Name = "selectCollisionPanel";
            this.selectCollisionPanel.Size = new System.Drawing.Size(102, 102);
            this.selectCollisionPanel.TabIndex = 30;
            // 
            // collisionPictureBox
            // 
            this.collisionPictureBox.BackColor = System.Drawing.Color.White;
            this.collisionPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.collisionPictureBox.Location = new System.Drawing.Point(3, 3);
            this.collisionPictureBox.Name = "collisionPictureBox";
            this.collisionPictureBox.Size = new System.Drawing.Size(96, 96);
            this.collisionPictureBox.TabIndex = 26;
            this.collisionPictureBox.TabStop = false;
            this.collisionPictureBox.Click += new System.EventHandler(this.collisionPictureBox_Click);
            // 
            // selectTypePanel
            // 
            this.selectTypePanel.Controls.Add(this.typePictureBox);
            this.selectTypePanel.Location = new System.Drawing.Point(122, 23);
            this.selectTypePanel.Name = "selectTypePanel";
            this.selectTypePanel.Size = new System.Drawing.Size(102, 102);
            this.selectTypePanel.TabIndex = 31;
            // 
            // typePictureBox
            // 
            this.typePictureBox.BackColor = System.Drawing.Color.White;
            this.typePictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.typePictureBox.Location = new System.Drawing.Point(3, 3);
            this.typePictureBox.Name = "typePictureBox";
            this.typePictureBox.Size = new System.Drawing.Size(96, 96);
            this.typePictureBox.TabIndex = 27;
            this.typePictureBox.TabStop = false;
            this.typePictureBox.Click += new System.EventHandler(this.typePictureBox_Click);
            // 
            // ImportMovButton
            // 
            this.ImportMovButton.Image = global::DSPRE.Properties.Resources.importArrow;
            this.ImportMovButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ImportMovButton.Location = new System.Drawing.Point(318, 22);
            this.ImportMovButton.Name = "ImportMovButton";
            this.ImportMovButton.Size = new System.Drawing.Size(138, 38);
            this.ImportMovButton.TabIndex = 23;
            this.ImportMovButton.Text = "Import Permissions";
            this.ImportMovButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ImportMovButton.UseVisualStyleBackColor = true;
            this.ImportMovButton.Click += new System.EventHandler(this.importMovButton_Click);
            // 
            // exportMovButton
            // 
            this.exportMovButton.Image = global::DSPRE.Properties.Resources.exportArrow;
            this.exportMovButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.exportMovButton.Location = new System.Drawing.Point(318, 64);
            this.exportMovButton.Name = "exportMovButton";
            this.exportMovButton.Size = new System.Drawing.Size(138, 38);
            this.exportMovButton.TabIndex = 22;
            this.exportMovButton.Text = "Export Permissions";
            this.exportMovButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.exportMovButton.UseVisualStyleBackColor = true;
            this.exportMovButton.Click += new System.EventHandler(this.exportMovButton_Click);
            // 
            // modelTabPage
            // 
            this.modelTabPage.Controls.Add(this.glbExportButton);
            this.modelTabPage.Controls.Add(this.daeExportButton);
            this.modelTabPage.Controls.Add(this.embedTexturesInMapModelCheckBox);
            this.modelTabPage.Controls.Add(this.modelSizeLBL);
            this.modelTabPage.Controls.Add(this.nsbmdSizeLabel);
            this.modelTabPage.Controls.Add(this.unsupported3DModelEditLBL);
            this.modelTabPage.Controls.Add(this.importMapButton);
            this.modelTabPage.Controls.Add(this.exportMapButton);
            this.modelTabPage.Location = new System.Drawing.Point(4, 22);
            this.modelTabPage.Name = "modelTabPage";
            this.modelTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.modelTabPage.Size = new System.Drawing.Size(481, 450);
            this.modelTabPage.TabIndex = 2;
            this.modelTabPage.Text = "3D Model";
            this.modelTabPage.UseVisualStyleBackColor = true;
            // 
            // glbExportButton
            // 
            this.glbExportButton.Image = global::DSPRE.Properties.Resources.exportArrow;
            this.glbExportButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.glbExportButton.Location = new System.Drawing.Point(351, 156);
            this.glbExportButton.Margin = new System.Windows.Forms.Padding(4);
            this.glbExportButton.Name = "glbExportButton";
            this.glbExportButton.Size = new System.Drawing.Size(120, 38);
            this.glbExportButton.TabIndex = 31;
            this.glbExportButton.Text = "Export GLB";
            this.glbExportButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.glbExportButton.UseVisualStyleBackColor = true;
            this.glbExportButton.Click += new System.EventHandler(this.glbExportButton_Click);
            // 
            // daeExportButton
            // 
            this.daeExportButton.Image = global::DSPRE.Properties.Resources.exportArrow;
            this.daeExportButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.daeExportButton.Location = new System.Drawing.Point(351, 111);
            this.daeExportButton.Name = "daeExportButton";
            this.daeExportButton.Size = new System.Drawing.Size(120, 38);
            this.daeExportButton.TabIndex = 30;
            this.daeExportButton.Text = "Export DAE";
            this.daeExportButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.daeExportButton.UseVisualStyleBackColor = true;
            this.daeExportButton.Click += new System.EventHandler(this.daeExportButton_Click);
            // 
            // embedTexturesInMapModelCheckBox
            // 
            this.embedTexturesInMapModelCheckBox.AutoSize = true;
            this.embedTexturesInMapModelCheckBox.Checked = true;
            this.embedTexturesInMapModelCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.embedTexturesInMapModelCheckBox.Location = new System.Drawing.Point(284, 66);
            this.embedTexturesInMapModelCheckBox.Name = "embedTexturesInMapModelCheckBox";
            this.embedTexturesInMapModelCheckBox.Size = new System.Drawing.Size(68, 17);
            this.embedTexturesInMapModelCheckBox.TabIndex = 29;
            this.embedTexturesInMapModelCheckBox.Text = "Textured";
            this.embedTexturesInMapModelCheckBox.UseVisualStyleBackColor = true;
            // 
            // modelSizeLBL
            // 
            this.modelSizeLBL.AutoSize = true;
            this.modelSizeLBL.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.modelSizeLBL.ForeColor = System.Drawing.SystemColors.ControlText;
            this.modelSizeLBL.Location = new System.Drawing.Point(104, 9);
            this.modelSizeLBL.Name = "modelSizeLBL";
            this.modelSizeLBL.Size = new System.Drawing.Size(97, 16);
            this.modelSizeLBL.TabIndex = 28;
            this.modelSizeLBL.Text = "ModelSizeTXT";
            // 
            // nsbmdSizeLabel
            // 
            this.nsbmdSizeLabel.AutoSize = true;
            this.nsbmdSizeLabel.BackColor = System.Drawing.Color.Transparent;
            this.nsbmdSizeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nsbmdSizeLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.nsbmdSizeLabel.Location = new System.Drawing.Point(6, 9);
            this.nsbmdSizeLabel.Name = "nsbmdSizeLabel";
            this.nsbmdSizeLabel.Size = new System.Drawing.Size(100, 16);
            this.nsbmdSizeLabel.TabIndex = 27;
            this.nsbmdSizeLabel.Text = "3D Model Size: ";
            // 
            // unsupported3DModelEditLBL
            // 
            this.unsupported3DModelEditLBL.AutoSize = true;
            this.unsupported3DModelEditLBL.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.unsupported3DModelEditLBL.ForeColor = System.Drawing.SystemColors.ControlText;
            this.unsupported3DModelEditLBL.Location = new System.Drawing.Point(132, 248);
            this.unsupported3DModelEditLBL.Name = "unsupported3DModelEditLBL";
            this.unsupported3DModelEditLBL.Size = new System.Drawing.Size(256, 48);
            this.unsupported3DModelEditLBL.TabIndex = 26;
            this.unsupported3DModelEditLBL.Text = "DSPRE cannot edit nor create 3D models.\r\nPlease use Blender, Sketchup, or \r\nTrifi" +
    "ndo\'s Pokemon DS Map Studio.";
            this.unsupported3DModelEditLBL.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // importMapButton
            // 
            this.importMapButton.Image = global::DSPRE.Properties.Resources.importArrow;
            this.importMapButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.importMapButton.Location = new System.Drawing.Point(351, 9);
            this.importMapButton.Name = "importMapButton";
            this.importMapButton.Size = new System.Drawing.Size(120, 38);
            this.importMapButton.TabIndex = 25;
            this.importMapButton.Text = "Import NSBMD";
            this.importMapButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.importMapButton.UseVisualStyleBackColor = true;
            this.importMapButton.Click += new System.EventHandler(this.importMapButton_Click);
            // 
            // exportMapButton
            // 
            this.exportMapButton.Image = global::DSPRE.Properties.Resources.exportArrow;
            this.exportMapButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.exportMapButton.Location = new System.Drawing.Point(351, 54);
            this.exportMapButton.Name = "exportMapButton";
            this.exportMapButton.Size = new System.Drawing.Size(120, 38);
            this.exportMapButton.TabIndex = 24;
            this.exportMapButton.Text = "Export NSBMD";
            this.exportMapButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.exportMapButton.UseVisualStyleBackColor = true;
            this.exportMapButton.Click += new System.EventHandler(this.exportMapButton_Click);
            // 
            // terrainTabPage
            // 
            this.terrainTabPage.Controls.Add(this.terrainSizeLBL);
            this.terrainTabPage.Controls.Add(this.terrainDataLBL);
            this.terrainTabPage.Controls.Add(this.unsupportedBDHCEditLBL);
            this.terrainTabPage.Controls.Add(this.bdhcImportButton);
            this.terrainTabPage.Controls.Add(this.bdhcExportButton);
            this.terrainTabPage.Location = new System.Drawing.Point(4, 22);
            this.terrainTabPage.Name = "terrainTabPage";
            this.terrainTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.terrainTabPage.Size = new System.Drawing.Size(481, 450);
            this.terrainTabPage.TabIndex = 3;
            this.terrainTabPage.Text = "Terrain Data";
            this.terrainTabPage.UseVisualStyleBackColor = true;
            // 
            // terrainSizeLBL
            // 
            this.terrainSizeLBL.AutoSize = true;
            this.terrainSizeLBL.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.terrainSizeLBL.Location = new System.Drawing.Point(124, 9);
            this.terrainSizeLBL.Name = "terrainSizeLBL";
            this.terrainSizeLBL.Size = new System.Drawing.Size(102, 16);
            this.terrainSizeLBL.TabIndex = 30;
            this.terrainSizeLBL.Text = "TerrainSizeTXT";
            // 
            // terrainDataLBL
            // 
            this.terrainDataLBL.AutoSize = true;
            this.terrainDataLBL.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.terrainDataLBL.Location = new System.Drawing.Point(6, 9);
            this.terrainDataLBL.Name = "terrainDataLBL";
            this.terrainDataLBL.Size = new System.Drawing.Size(117, 16);
            this.terrainDataLBL.TabIndex = 29;
            this.terrainDataLBL.Text = "Terrain Data Size: ";
            // 
            // unsupportedBDHCEditLBL
            // 
            this.unsupportedBDHCEditLBL.AutoSize = true;
            this.unsupportedBDHCEditLBL.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.unsupportedBDHCEditLBL.Location = new System.Drawing.Point(113, 257);
            this.unsupportedBDHCEditLBL.Name = "unsupportedBDHCEditLBL";
            this.unsupportedBDHCEditLBL.Size = new System.Drawing.Size(290, 32);
            this.unsupportedBDHCEditLBL.TabIndex = 28;
            this.unsupportedBDHCEditLBL.Text = "DSPRE cannot edit nor create BDHC data.\r\nPlease use Trifindo\'s Pokemon DS Map Stu" +
    "dio.";
            this.unsupportedBDHCEditLBL.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // bdhcImportButton
            // 
            this.bdhcImportButton.Image = global::DSPRE.Properties.Resources.importArrow;
            this.bdhcImportButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bdhcImportButton.Location = new System.Drawing.Point(351, 9);
            this.bdhcImportButton.Name = "bdhcImportButton";
            this.bdhcImportButton.Size = new System.Drawing.Size(120, 38);
            this.bdhcImportButton.TabIndex = 27;
            this.bdhcImportButton.Text = "Import BDHC";
            this.bdhcImportButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bdhcImportButton.UseVisualStyleBackColor = true;
            this.bdhcImportButton.Click += new System.EventHandler(this.bdhcImportButton_Click);
            // 
            // bdhcExportButton
            // 
            this.bdhcExportButton.Image = global::DSPRE.Properties.Resources.exportArrow;
            this.bdhcExportButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bdhcExportButton.Location = new System.Drawing.Point(351, 54);
            this.bdhcExportButton.Name = "bdhcExportButton";
            this.bdhcExportButton.Size = new System.Drawing.Size(120, 38);
            this.bdhcExportButton.TabIndex = 26;
            this.bdhcExportButton.Text = "Export BDHC";
            this.bdhcExportButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bdhcExportButton.UseVisualStyleBackColor = true;
            this.bdhcExportButton.Click += new System.EventHandler(this.bdhcExportButton_Click);
            // 
            // bgsTabPage
            // 
            this.bgsTabPage.Controls.Add(this.blankBGSButton);
            this.bgsTabPage.Controls.Add(this.BGSSizeLBL);
            this.bgsTabPage.Controls.Add(this.bgsDataLBL);
            this.bgsTabPage.Controls.Add(this.unsupportedBGSEditLBL);
            this.bgsTabPage.Controls.Add(this.soundPlatesImportButton);
            this.bgsTabPage.Controls.Add(this.soundPlatesExportButton);
            this.bgsTabPage.Location = new System.Drawing.Point(4, 22);
            this.bgsTabPage.Name = "bgsTabPage";
            this.bgsTabPage.Size = new System.Drawing.Size(481, 450);
            this.bgsTabPage.TabIndex = 4;
            this.bgsTabPage.Text = "Sound Plates";
            this.bgsTabPage.UseVisualStyleBackColor = true;
            // 
            // blankBGSButton
            // 
            this.blankBGSButton.Image = global::DSPRE.Properties.Resources.muteIcon;
            this.blankBGSButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.blankBGSButton.Location = new System.Drawing.Point(344, 112);
            this.blankBGSButton.Name = "blankBGSButton";
            this.blankBGSButton.Size = new System.Drawing.Size(120, 38);
            this.blankBGSButton.TabIndex = 34;
            this.blankBGSButton.Text = "Blank BGS";
            this.blankBGSButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.blankBGSButton.UseVisualStyleBackColor = true;
            this.blankBGSButton.Click += new System.EventHandler(this.soundPlatesBlankButton_Click);
            // 
            // BGSSizeLBL
            // 
            this.BGSSizeLBL.AutoSize = true;
            this.BGSSizeLBL.BackColor = System.Drawing.Color.Transparent;
            this.BGSSizeLBL.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BGSSizeLBL.ForeColor = System.Drawing.SystemColors.ControlText;
            this.BGSSizeLBL.Location = new System.Drawing.Point(73, 9);
            this.BGSSizeLBL.Name = "BGSSizeLBL";
            this.BGSSizeLBL.Size = new System.Drawing.Size(84, 16);
            this.BGSSizeLBL.TabIndex = 33;
            this.BGSSizeLBL.Text = "BGSSizeLBL";
            // 
            // bgsDataLBL
            // 
            this.bgsDataLBL.AutoSize = true;
            this.bgsDataLBL.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bgsDataLBL.Location = new System.Drawing.Point(7, 9);
            this.bgsDataLBL.Name = "bgsDataLBL";
            this.bgsDataLBL.Size = new System.Drawing.Size(73, 16);
            this.bgsDataLBL.TabIndex = 32;
            this.bgsDataLBL.Text = "BGS Data: ";
            // 
            // unsupportedBGSEditLBL
            // 
            this.unsupportedBGSEditLBL.AutoSize = true;
            this.unsupportedBGSEditLBL.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.unsupportedBGSEditLBL.Location = new System.Drawing.Point(73, 266);
            this.unsupportedBGSEditLBL.Name = "unsupportedBGSEditLBL";
            this.unsupportedBGSEditLBL.Size = new System.Drawing.Size(338, 32);
            this.unsupportedBGSEditLBL.TabIndex = 31;
            this.unsupportedBGSEditLBL.Text = "DSPRE cannot edit nor create Background Sound Files.\r\nPlease use Trifindo\'s Pokem" +
    "on DS Map Studio.\r\n";
            this.unsupportedBGSEditLBL.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // soundPlatesImportButton
            // 
            this.soundPlatesImportButton.Image = global::DSPRE.Properties.Resources.importArrow;
            this.soundPlatesImportButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.soundPlatesImportButton.Location = new System.Drawing.Point(344, 10);
            this.soundPlatesImportButton.Name = "soundPlatesImportButton";
            this.soundPlatesImportButton.Size = new System.Drawing.Size(120, 38);
            this.soundPlatesImportButton.TabIndex = 30;
            this.soundPlatesImportButton.Text = "Import BGS";
            this.soundPlatesImportButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.soundPlatesImportButton.UseVisualStyleBackColor = true;
            this.soundPlatesImportButton.Click += new System.EventHandler(this.soundPlatesImportButton_Click);
            // 
            // soundPlatesExportButton
            // 
            this.soundPlatesExportButton.Image = global::DSPRE.Properties.Resources.exportArrow;
            this.soundPlatesExportButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.soundPlatesExportButton.Location = new System.Drawing.Point(344, 55);
            this.soundPlatesExportButton.Name = "soundPlatesExportButton";
            this.soundPlatesExportButton.Size = new System.Drawing.Size(120, 38);
            this.soundPlatesExportButton.TabIndex = 29;
            this.soundPlatesExportButton.Text = "Export BGS";
            this.soundPlatesExportButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.soundPlatesExportButton.UseVisualStyleBackColor = true;
            this.soundPlatesExportButton.Click += new System.EventHandler(this.soundPlatesExportButton_Click);
            // 
            // MapEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.tableLayoutPanel3);
            this.Controls.Add(this.mapRenderPanel);
            this.Controls.Add(this.label26);
            this.Controls.Add(this.buildTextureComboBox);
            this.Controls.Add(this.mapFileLabel);
            this.Controls.Add(this.mapTextureComboBox);
            this.Controls.Add(this.mapTextureLabel);
            this.Controls.Add(this.selectMapComboBox);
            this.Controls.Add(this.mapPartsTabControl);
            this.Name = "MapEditor";
            this.Size = new System.Drawing.Size(1193, 646);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.mapRenderPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.movPictureBox)).EndInit();
            this.mapPartsTabControl.ResumeLayout(false);
            this.buildingsTabPage.ResumeLayout(false);
            this.buildingsTabPage.PerformLayout();
            this.groupBox33.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.yRotDegBldUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xRotDegBldUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.zRotDegBldUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.yRotBuildUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xRotBuildUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.zRotBuildUpDown)).EndInit();
            this.lockXZgroupbox.ResumeLayout(false);
            this.lockXZgroupbox.PerformLayout();
            this.bldRoundGroupbox.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.groupBox20.ResumeLayout(false);
            this.groupBox20.PerformLayout();
            this.groupBox19.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.buildingHeightUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.buildingWidthUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.buildingLengthUpDown)).EndInit();
            this.buildPositionGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.yBuildUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xBuildUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.zBuildUpDown)).EndInit();
            this.permissionsTabPage.ResumeLayout(false);
            this.permissionsTabPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.transparencyBar)).EndInit();
            this.typeGroupBox.ResumeLayout(false);
            this.typeGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.typePainterUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.typePainterPictureBox)).EndInit();
            this.collisionGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.collisionPainterPictureBox)).EndInit();
            this.selectCollisionPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.collisionPictureBox)).EndInit();
            this.selectTypePanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.typePictureBox)).EndInit();
            this.modelTabPage.ResumeLayout(false);
            this.modelTabPage.PerformLayout();
            this.terrainTabPage.ResumeLayout(false);
            this.terrainTabPage.PerformLayout();
            this.bgsTabPage.ResumeLayout(false);
            this.bgsTabPage.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button mapScreenshotButton;
        private System.Windows.Forms.CheckBox wireframeCheckBox;
        private System.Windows.Forms.RadioButton radio3D;
        private System.Windows.Forms.RadioButton radio2D;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Button addMapFileButton;
        private System.Windows.Forms.Button locateCurrentMapBin;
        private System.Windows.Forms.Button removeMapFileButton;
        private System.Windows.Forms.Button replaceMapBinButton;
        private System.Windows.Forms.Button exportCurrentMapBinButton;
        private System.Windows.Forms.Button saveMapButton;
        private System.Windows.Forms.Panel mapRenderPanel;
        public Tao.Platform.Windows.SimpleOpenGlControl mapOpenGlControl;
        private System.Windows.Forms.PictureBox movPictureBox;
        private System.Windows.Forms.Label label26;
        public System.Windows.Forms.ComboBox buildTextureComboBox;
        private System.Windows.Forms.Label mapFileLabel;
        public System.Windows.Forms.ComboBox mapTextureComboBox;
        private System.Windows.Forms.Label mapTextureLabel;
        public System.Windows.Forms.ComboBox selectMapComboBox;
        private System.Windows.Forms.TabControl mapPartsTabControl;
        private System.Windows.Forms.TabPage buildingsTabPage;
        private System.Windows.Forms.GroupBox groupBox33;
        private System.Windows.Forms.NumericUpDown yRotDegBldUpDown;
        private System.Windows.Forms.NumericUpDown xRotDegBldUpDown;
        private System.Windows.Forms.NumericUpDown zRotDegBldUpDown;
        private System.Windows.Forms.NumericUpDown yRotBuildUpDown;
        private System.Windows.Forms.NumericUpDown xRotBuildUpDown;
        private System.Windows.Forms.NumericUpDown zRotBuildUpDown;
        private System.Windows.Forms.Label yLabel;
        private System.Windows.Forms.GroupBox lockXZgroupbox;
        private System.Windows.Forms.CheckBox bldPlaceLockXcheckbox;
        private System.Windows.Forms.CheckBox bldPlaceLockZcheckbox;
        private System.Windows.Forms.Label zLabel;
        private System.Windows.Forms.Label xLabel;
        private System.Windows.Forms.GroupBox bldRoundGroupbox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.RadioButton bldRoundDecmil;
        private System.Windows.Forms.RadioButton bldRoundCentMil;
        private System.Windows.Forms.RadioButton bldRoundWhole;
        private System.Windows.Forms.RadioButton bldRoundDec;
        private System.Windows.Forms.RadioButton bldRoundCent;
        private System.Windows.Forms.RadioButton bldRoundMil;
        private System.Windows.Forms.CheckBox bldPlaceWithMouseCheckbox;
        private System.Windows.Forms.Button importBuildingsButton;
        private System.Windows.Forms.GroupBox groupBox20;
        public System.Windows.Forms.RadioButton interiorbldRadioButton;
        public System.Windows.Forms.RadioButton exteriorbldRadioButton;
        private System.Windows.Forms.ComboBox buildIndexComboBox;
        private System.Windows.Forms.GroupBox groupBox19;
        private System.Windows.Forms.NumericUpDown buildingHeightUpDown;
        private System.Windows.Forms.NumericUpDown buildingWidthUpDown;
        private System.Windows.Forms.NumericUpDown buildingLengthUpDown;
        private System.Windows.Forms.Button duplicateBuildingButton;
        private System.Windows.Forms.Button exportBuildingsButton;
        private System.Windows.Forms.Button removeBuildingButton;
        private System.Windows.Forms.Button addBuildingButton;
        private System.Windows.Forms.GroupBox buildPositionGroupBox;
        private System.Windows.Forms.NumericUpDown yBuildUpDown;
        private System.Windows.Forms.NumericUpDown xBuildUpDown;
        private System.Windows.Forms.NumericUpDown zBuildUpDown;
        private System.Windows.Forms.ListBox buildingsListBox;
        private System.Windows.Forms.TabPage permissionsTabPage;
        private System.Windows.Forms.TrackBar transparencyBar;
        private System.Windows.Forms.Button scanUnusedCollisionTypesButton;
        private System.Windows.Forms.Button clearCurrentButton;
        private System.Windows.Forms.Label typeLabel;
        private System.Windows.Forms.Label collisionLabel;
        private System.Windows.Forms.GroupBox typeGroupBox;
        private System.Windows.Forms.RadioButton knownTypesRadioButton;
        private System.Windows.Forms.RadioButton valueTypeRadioButton;
        private System.Windows.Forms.NumericUpDown typePainterUpDown;
        private System.Windows.Forms.ComboBox collisionTypePainterComboBox;
        private System.Windows.Forms.PictureBox typePainterPictureBox;
        private System.Windows.Forms.GroupBox collisionGroupBox;
        private System.Windows.Forms.ComboBox collisionPainterComboBox;
        private System.Windows.Forms.PictureBox collisionPainterPictureBox;
        private System.Windows.Forms.Panel selectCollisionPanel;
        private System.Windows.Forms.PictureBox collisionPictureBox;
        private System.Windows.Forms.Panel selectTypePanel;
        private System.Windows.Forms.PictureBox typePictureBox;
        private System.Windows.Forms.Button ImportMovButton;
        private System.Windows.Forms.Button exportMovButton;
        private System.Windows.Forms.TabPage modelTabPage;
        private System.Windows.Forms.Button glbExportButton;
        private System.Windows.Forms.Button daeExportButton;
        private System.Windows.Forms.CheckBox embedTexturesInMapModelCheckBox;
        private System.Windows.Forms.Label modelSizeLBL;
        private System.Windows.Forms.Label nsbmdSizeLabel;
        private System.Windows.Forms.Label unsupported3DModelEditLBL;
        private System.Windows.Forms.Button importMapButton;
        private System.Windows.Forms.Button exportMapButton;
        private System.Windows.Forms.TabPage terrainTabPage;
        private System.Windows.Forms.Label terrainSizeLBL;
        private System.Windows.Forms.Label terrainDataLBL;
        private System.Windows.Forms.Label unsupportedBDHCEditLBL;
        private System.Windows.Forms.Button bdhcImportButton;
        private System.Windows.Forms.Button bdhcExportButton;
        private System.Windows.Forms.TabPage bgsTabPage;
        private System.Windows.Forms.Button blankBGSButton;
        private System.Windows.Forms.Label BGSSizeLBL;
        private System.Windows.Forms.Label bgsDataLBL;
        private System.Windows.Forms.Label unsupportedBGSEditLBL;
        private System.Windows.Forms.Button soundPlatesImportButton;
        private System.Windows.Forms.Button soundPlatesExportButton;

    }
}
