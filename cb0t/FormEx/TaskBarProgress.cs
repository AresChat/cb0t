using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FormEx.TaskBarProgressBarEx
{
    public class TaskBarProgress
    {
        private DwmForm Owner { get; set; }
        private bool Visible { get; set; }

        private TaskBarProgressColor _color { get; set; }
        public TaskBarProgressColor Color
        {
            get { return this._color; }
            set
            {
                this._color = value;

                if (this.Visible)
                    switch (value)
                    {
                        case TaskBarProgressColor.Green:
                            Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager.Instance.SetProgressState(Microsoft.WindowsAPICodePack.Taskbar.TaskbarProgressBarState.Normal, this.Owner.Handle);
                            break;

                        case TaskBarProgressColor.Red:
                            Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager.Instance.SetProgressState(Microsoft.WindowsAPICodePack.Taskbar.TaskbarProgressBarState.Error, this.Owner.Handle);
                            break;

                        case TaskBarProgressColor.Yellow:
                            Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager.Instance.SetProgressState(Microsoft.WindowsAPICodePack.Taskbar.TaskbarProgressBarState.Paused, this.Owner.Handle);
                            break;
                    }
            }
        }

        public TaskBarProgress(DwmForm f)
        {
            this.Owner = f;
            this.Color = TaskBarProgressColor.Green;
        }

        public void SetValue(int current, int total)
        {
            Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager.Instance.SetProgressValue(current, total, this.Owner.Handle);

            switch (this._color)
            {
                case TaskBarProgressColor.Green:
                    Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager.Instance.SetProgressState(Microsoft.WindowsAPICodePack.Taskbar.TaskbarProgressBarState.Normal, this.Owner.Handle);
                    break;

                case TaskBarProgressColor.Red:
                    Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager.Instance.SetProgressState(Microsoft.WindowsAPICodePack.Taskbar.TaskbarProgressBarState.Error, this.Owner.Handle);
                    break;

                case TaskBarProgressColor.Yellow:
                    Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager.Instance.SetProgressState(Microsoft.WindowsAPICodePack.Taskbar.TaskbarProgressBarState.Paused, this.Owner.Handle);
                    break;
            }

            this.Visible = true;
        }

        public void Reset()
        {
            if (this.Visible)
            {
                Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager.Instance.SetProgressState(Microsoft.WindowsAPICodePack.Taskbar.TaskbarProgressBarState.NoProgress, this.Owner.Handle);
                Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager.Instance.SetProgressValue(0, 1, this.Owner.Handle);
                this.Visible = false;
            }
        }
    }
}
