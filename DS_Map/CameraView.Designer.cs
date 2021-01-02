namespace DSPRE
{
    partial class CameraView
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.cameraOpenGLControl = new Tao.Platform.Windows.SimpleOpenGlControl();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cameraOpenGLControl);
            this.panel1.Location = new System.Drawing.Point(387, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(401, 426);
            this.panel1.TabIndex = 27;
            // 
            // cameraOpenGLControl
            // 
            this.cameraOpenGLControl.AccumBits = ((byte)(0));
            this.cameraOpenGLControl.AutoCheckErrors = false;
            this.cameraOpenGLControl.AutoFinish = false;
            this.cameraOpenGLControl.AutoMakeCurrent = true;
            this.cameraOpenGLControl.AutoSwapBuffers = true;
            this.cameraOpenGLControl.BackColor = System.Drawing.Color.Black;
            this.cameraOpenGLControl.ColorBits = ((byte)(32));
            this.cameraOpenGLControl.DepthBits = ((byte)(24));
            this.cameraOpenGLControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cameraOpenGLControl.Location = new System.Drawing.Point(0, 0);
            this.cameraOpenGLControl.Name = "cameraOpenGLControl";
            this.cameraOpenGLControl.Size = new System.Drawing.Size(401, 426);
            this.cameraOpenGLControl.StencilBits = ((byte)(0));
            this.cameraOpenGLControl.TabIndex = 21;
            // 
            // CameraView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.panel1);
            this.Name = "CameraView";
            this.Text = "Camera View";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private Tao.Platform.Windows.SimpleOpenGlControl cameraOpenGLControl;
    }
}