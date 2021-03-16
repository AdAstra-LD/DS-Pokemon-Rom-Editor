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
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            this.SuspendLayout();
            // 
            // textureComboBox
            // 
            this.textureComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.textureComboBox.FormattingEnabled = true;
            this.textureComboBox.Location = new System.Drawing.Point(120, 465);
            this.textureComboBox.Name = "textureComboBox";
            this.textureComboBox.Size = new System.Drawing.Size(140, 21);
            this.textureComboBox.TabIndex = 25;
            this.textureComboBox.SelectedIndexChanged += new System.EventHandler(this.textureComboBox_SelectedIndexChanged);
            // 
            // importButton
            // 
            this.importButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.importButton.Location = new System.Drawing.Point(12, 492);
            this.importButton.Name = "importButton";
            this.importButton.Size = new System.Drawing.Size(93, 23);
            this.importButton.TabIndex = 23;
            this.importButton.Text = "Import NSBMD";
            this.importButton.UseVisualStyleBackColor = true;
            this.importButton.Click += new System.EventHandler(this.importButton_Click);
            // 
            // exportButton
            // 
            this.exportButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.exportButton.Location = new System.Drawing.Point(12, 464);
            this.exportButton.Name = "exportButton";
            this.exportButton.Size = new System.Drawing.Size(93, 23);
            this.exportButton.TabIndex = 22;
            this.exportButton.Text = "Export NSBMD";
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
            this.buildingOpenGLControl.Location = new System.Drawing.Point(3, 8);
            this.buildingOpenGLControl.Name = "buildingOpenGLControl";
            this.buildingOpenGLControl.Size = new System.Drawing.Size(500, 500);
            this.buildingOpenGLControl.StencilBits = ((byte)(0));
            this.buildingOpenGLControl.TabIndex = 21;
            this.buildingOpenGLControl.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.buildingOpenGLControl_PreviewKeyDown);
            // 
            // interiorCheckBox
            // 
            this.interiorCheckBox.AutoSize = true;
            this.interiorCheckBox.Enabled = false;
            this.interiorCheckBox.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.interiorCheckBox.Location = new System.Drawing.Point(120, 499);
            this.interiorCheckBox.Name = "interiorCheckBox";
            this.interiorCheckBox.Size = new System.Drawing.Size(139, 17);
            this.interiorCheckBox.TabIndex = 20;
            this.interiorCheckBox.Text = "Interior Models (HG/SS)";
            this.interiorCheckBox.UseVisualStyleBackColor = true;
            this.interiorCheckBox.CheckedChanged += new System.EventHandler(this.interiorCheckBox_CheckedChanged);
            // 
            // buildingEditorBldListBox
            // 
            this.buildingEditorBldListBox.FormattingEnabled = true;
            this.buildingEditorBldListBox.Location = new System.Drawing.Point(12, 12);
            this.buildingEditorBldListBox.Name = "buildingEditorBldListBox";
            this.buildingEditorBldListBox.Size = new System.Drawing.Size(248, 446);
            this.buildingEditorBldListBox.TabIndex = 19;
            this.buildingEditorBldListBox.SelectedIndexChanged += new System.EventHandler(this.buildingEditorListBox_SelectedIndexChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.buildingOpenGLControl);
            this.panel1.Location = new System.Drawing.Point(266, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(507, 512);
            this.panel1.TabIndex = 26;
            // 
            // BuildingEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 525);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.textureComboBox);
            this.Controls.Add(this.importButton);
            this.Controls.Add(this.exportButton);
            this.Controls.Add(this.interiorCheckBox);
            this.Controls.Add(this.buildingEditorBldListBox);
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
    }
}