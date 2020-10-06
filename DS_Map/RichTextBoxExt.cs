namespace System.Windows.Forms
{
    /// <summary>
    /// Extension methods for System.IO.Stream.
    /// </summary>
    using System.Drawing;

    public static class RichTextBoxExtensions
    {
        public static void AppendText(this RichTextBox box, string text, Color color)
        {
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;

            box.SelectionColor = color;
            box.AppendText(text);
            box.SelectionColor = Color.Black;
        }
    }
}