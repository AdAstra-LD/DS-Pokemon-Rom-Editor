using System;
using System.Drawing;
using System.Windows.Forms;

namespace DSPRE {
    public partial class OffsetPictureBox : PictureBox {
        public float offsX { get; private set; } = 0;
        public float offsY { get; private set; } = 0;
        public bool invertDrag { get; set; } = false;

        bool dragging;
        private Point dragStart = new Point(0, 0);

        protected override void OnPaint(PaintEventArgs pe) {
            if (this.Image != null) {
                pe.Graphics.TranslateTransform(offsX, offsY);
            }
            base.OnPaint(pe);
        }
        protected override void OnMouseDown(MouseEventArgs e) {
            if (e.Button != MouseButtons.Left) {
                return;
            }
            dragStart.X = e.X;
            dragStart.Y = e.Y;

            dragging = true;
            this.OnMouseMove(e);

            base.OnMouseDown(e);
        }
        protected override void OnMouseMove(MouseEventArgs e) {
            if (!dragging || this.Image is null) {
                return;
            }

            if (e.Button == MouseButtons.Left) {

                this.DrawTranslate(dragStart.X - e.X, dragStart.Y - e.Y);

                dragStart.X = e.X;
                dragStart.Y = e.Y;

                this.Invalidate();
            } else {
                AppLogger.Debug(e.Delta.ToString());
            }

            base.OnMouseMove(e);
        }
        protected override void OnMouseUp(MouseEventArgs e) {
            dragging = false;
        }

        public void DrawAt(float offsX, float offsY) {
            this.offsX = offsX;
            this.offsY = offsY;
            this.Invalidate();
        }
        public void DrawTranslate(float incrementX, float incrementY) {
            if (invertDrag) {
                DrawAt(this.offsX - incrementX, this.offsY - incrementY);
            } else {
                DrawAt(this.offsX + incrementX, this.offsY + incrementY);
            }
        }
        public void RedrawCentered() => DrawAt(0, 0);
    }
}
