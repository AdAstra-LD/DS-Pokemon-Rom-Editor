using DSPRE.Properties;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;

namespace DSPRE
{
    partial class DVCalcNatureViewerForm
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
            this.natureGridView = new System.Windows.Forms.DataGridView();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DVCalcNatureViewerForm));
            ((System.ComponentModel.ISupportInitialize)(this.natureGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // natureGridView
            // 
            this.natureGridView.AllowUserToAddRows = false;
            this.natureGridView.AllowUserToDeleteRows = false;
            this.natureGridView.AllowUserToOrderColumns = true;
            this.natureGridView.AllowUserToResizeRows = false;
            this.natureGridView.BackgroundColor = System.Drawing.SystemColors.Menu;
            this.natureGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.natureGridView.ColumnHeadersHeight = 29;
            this.natureGridView.Location = new System.Drawing.Point(16, 10);
            this.natureGridView.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.natureGridView.Name = "natureGridView";
            this.natureGridView.ReadOnly = true;
            this.natureGridView.RowHeadersWidth = 51;
            this.natureGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.natureGridView.Size = new System.Drawing.Size(387, 333);
            this.natureGridView.TabIndex = 0;
            this.natureGridView.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.natureGridView_CellDoubleClick);
            this.natureGridView.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.natureGridView_ColumnHeaderMouseClick);
            // 
            // DVCalcNatureViewerForm
            //
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(415, 352);
            this.Controls.Add(this.natureGridView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MaximizeBox = false;
            this.Name = "DVCalcNatureViewerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Nature List";
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            ((System.ComponentModel.ISupportInitialize)(this.natureGridView)).EndInit();
            this.ResumeLayout(false);
            

        }

        #endregion

        private System.Windows.Forms.DataGridView natureGridView;
    }
}