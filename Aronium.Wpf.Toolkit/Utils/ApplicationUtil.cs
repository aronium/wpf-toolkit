using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Aronium.Wpf.Toolkit.Utils
{
    public static class ApplicationUtil
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern Boolean ShowWindow(IntPtr hWnd, Int32 nCmdShow);

        private const int SW_SHOWMAXIMIZED = 3;

        /// <summary>
        /// Ensures that single application process is running and if not, if specified, automatically activates already running process.
        /// </summary>
        /// <param name="activate">Indicates whether other process should be activated automatically.</param>
        /// <returns></returns>
        public static bool IsSingleInstance(bool activate = true)
        {
            Process currentProcess = Process.GetCurrentProcess();

            var runningProcess = (from process in Process.GetProcesses()
                                  where process.Id != currentProcess.Id &&
                                  process.ProcessName.EndsWith("vshost.exe") == false && //<-- In case there is more then one Printie.vshost.exe
                                  process.ProcessName.Equals(currentProcess.ProcessName, StringComparison.Ordinal)
                                  select process).FirstOrDefault();

            if (runningProcess != null)
            {
                if (activate)
                {
                    ShowWindow(runningProcess.MainWindowHandle, SW_SHOWMAXIMIZED);
                }

                return false;
            }

            return true;
        }
    }
}
