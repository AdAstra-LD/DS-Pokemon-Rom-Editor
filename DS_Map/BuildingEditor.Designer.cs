namespace DSPRE
{
    partial class BuildingEditor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.textureComboBox = new System.Windows.Forms.ComboBox();
            this.importButton = new System.Windows.Forms.Button();
            this.exportButton = new System.Windows.Forms.Button();
            this.buildingOpenGLControl = new Tao.Platform.Windows.SimpleOpenGlControl();
            this.interiorCheckBox = new System.Windows.Forms.CheckBox();
            this.buildingEditorBldListBox = new System.Windows.Forms.ListBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            this.SuspendLayout();
            // 
            // textureComboBox
            // 
            this.textureComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.textureComboBox.FormattingEnabled = true;
            this.textureComboBox.Location = new System.Drawing.Point(1314, 1006);
            this.textureComboBox.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.textureComboBox.Name = "textureComboBox";
            this.textureComboBox.Size = new System.Drawing.Size(312, 33);
            this.textureComboBox.TabIndex = 25;
            this.textureComboBox.SelectedIndexChanged += new System.EventHandler(this.textureComboBox_SelectedIndexChanged);
            // 
            // importButton
            // 
            this.importButton.Image = global::DSPRE.Properties.Resources.importArrow;
            this.importButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.importButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.importButton.Location = new System.Drawing.Point(24, 971);
            this.importButton.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.importButton.Name = "importButton";
            this.importButton.Size = new System.Drawing.Size(222, 69);
            this.importButton.TabIndex = 23;
            this.importButton.Text = "Import NSBMD";
            this.importButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.importButton.UseVisualStyleBackColor = true;
            this.importButton.Click += new System.EventHandler(this.importButton_Click);
            // 
            // exportButton
            // 
            this.exportButton.Image = global::DSPRE.Properties.Resources.exportArrow;
            this.exportButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.exportButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.exportButton.Location = new System.Drawing.Point(248, 971);
            this.exportButton.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.exportButton.Name = "exportButton";
            this.exportButton.Size = new System.Drawing.Size(222, 69);
            this.exportButton.TabIndex = 22;
            this.exportButton.Text = "Export NSBMD";
            this.exportButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.exportButton.UseVisualStyleBackColor = true;
            this.exportButton.Click += new System.EventHandler(this.exportButton_Click);
            // 
            // buildingOpenGLControl
            // 
            this.buildingOpenGLControl.AccumBits = ((byte)(0));
            this.buildingOpenGLControl.AutoCheckErrors = false;
            this.buildingOpenGLControl.AutoFinish = false;
            this.buildingOpenGLControl.AutoMakeCurrent = true;
            this.buildingOpenGLControl.AutoSwapBuffers = true;
            this.buildingOpenGLControl.BackColor = System.Drawing.Color.Black;
            this.buildingOpenGLControl.ColorBits = ((byte)(32));
            this.buildingOpenGLControl.DepthBits = ((byte)(24));
            this.buildingOpenGLControl.Location = new System.Drawing.Point(6, 13);
            this.buildingOpenGLControl.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.buildingOpenGLControl.Name = "buildingOpenGLControl";
            this.buildingOpenGLControl.Size = new System.Drawing.Size(1000, 962);
            this.buildingOpenGLControl.StencilBits = ((byte)(0));
            this.buildingOpenGLControl.TabIndex = 21;
            this.buildingOpenGLControl.KeyUp += new System.Windows.Forms.KeyEventHandler(this.buildingOpenGLControl_KeyUp);
            this.buildingOpenGLControl.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.buildingOpenGLControl_PreviewKeyDown);
            // 
            // interiorCheckBox
            // 
            this.interiorCheckBox.AutoSize = true;
            this.interiorCheckBox.Enabled = false;
            this.interiorCheckBox.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.interiorCheckBox.Location = new System.Drawing.Point(624, 1010);
            this.interiorCheckBox.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.interiorCheckBox.Name = "interiorCheckBox";
            this.interiorCheckBox.Size = new System.Drawing.Size(226, 29);
            this.interiorCheckBox.TabIndex = 20;
            this.interiorCheckBox.Text = "Interior Models List";
            this.interiorCheckBox.UseVisualStyleBackColor = true;
            this.interiorCheckBox.CheckedChanged += new System.EventHandler(this.interiorCheckBox_CheckedChanged);
            // 
            // buildingEditorBldListBox
            // 
            this.buildingEditorBldListBox.FormattingEnabled = true;
            this.buildingEditorBldListBox.ItemHeight = 25;
            this.buildingEditorBldListBox.Location = new System.Drawing.Point(24, 23);
            this.buildingEditorBldListBox.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.buildingEditorBldListBox.Name = "buildingEditorBldListBox";
            this.buildingEditorBldListBox.Size = new System.Drawing.Size(578, 929);
            this.buildingEditorBldListBox.TabIndex = 19;
            this.buildingEditorBldListBox.SelectedIndexChanged += new System.EventHandler(this.buildingEditorListBox_SelectedIndexChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.buildingOpenGLControl);
            this.panel1.Location = new System.Drawing.Point(618, 8);
            this.panel1.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1014, 985);
            this.panel1.TabIndex = 26;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1076, 1013);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(224, 25);
            this.label1.TabIndex = 27;
            this.label1.Text = "Texture Pack Selector";
            // 
            // button1
            // 
            this.button1.Image = global::DSPRE.Properties.Resources.exportArrow;
            this.button1.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.button1.Location = new System.Drawing.Point(472, 971);
            this.button1.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(134, 69);
            this.button1.TabIndex = 28;
            this.button1.Text = "DAE";
            this.button1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.bldExportDAEbutton_Click);
            // 
            // BuildingEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1642, 1054);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.textureComboBox);
            this.Controls.Add(this.importButton);
            this.Controls.Add(this.exportButton);
            this.Controls.Add(this.interiorCheckBox);
            this.Controls.Add(this.buildingEditorBldListBox);
            this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.Name = "BuildingEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Building Editor";
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox textureComboBox;
        private System.Windows.Forms.Button importButton;
        private System.Windows.Forms.Button exportButton;
        private Tao.Platform.Windows.SimpleOpenGlControl buildingOpenGLControl;
        private System.Windows.Forms.CheckBox interiorCheckBox;
        private System.Windows.Forms.ListBox buildingEditorBldListBox;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.BindingSource bindingSource1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
    }
}