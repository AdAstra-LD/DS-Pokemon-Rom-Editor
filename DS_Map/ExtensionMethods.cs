using System;
using System.Windows.Forms;

namespace DSPRE {
  public static class ExtensionMethods {
    /// <summary>
    /// Increment the progress bar value, without using Windows Aero animation
    /// </summary>
    public static void IncrementNoAnimation(this ProgressBar pb, int amount = 1) {
      pb.Value += amount;
      if (pb.Value != pb.Maximum) pb.Value++;
      pb.Value--;
    }

    //https://stackoverflow.com/a/10939890
    /// <summary>
    /// Sets the progress bar value, without using Windows Aero animation
    /// </summary>
    public static void SetProgressNoAnimation(this ProgressBar pb, int value) {
      // Don't redraw if nothing is changing.
      if (value == pb.Value)
        return;

      // To get around this animation, we need to move the progress bar backwards.
      if (value == pb.Maximum) {
        // Special case (can't set value > Maximum).
        pb.Value = value; // Set the value
        pb.Value = value - 1; // Move it backwards
      }
      else {
        pb.Value = value + 1; // Move past
      }

      pb.Value = value; // Move to correct value
    }

    /*
        searchInScriptsButton.BeginInvoke(new Action(() => {
        }));

        BackgroundWorker bw = new BackgroundWorker();
        bw.DoWork += (_sender, args) => {
          this.UIThread(() => {
          });
        };

        //easier to set up
        searchInScriptsButton.BeginInvoke(new Action(() => {
          searchProgressBar.Value += 1;
        }));

        //smoother if you put the control updates in the UIThread
        //same as BeginInvoke if you put the entire logic in UIThread
        BackgroundWorker bw = new BackgroundWorker();
        bw.DoWork += (_sender, args) => {
          this.UIThread(() => {
            searchProgressBar.Value += 1;
          });
        };
        bw.RunWorkerAsync();
    */
    public static void UIThread(this Control control, Action code) {
      if (control.InvokeRequired) {
        control.BeginInvoke(code);
        return;
      }

      code.Invoke();
    }
  }
}
