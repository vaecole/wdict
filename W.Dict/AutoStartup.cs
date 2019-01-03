using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;

namespace W.Dict
{
    internal class AutoStartup
    {
        const string KeyName = "W.Dict伟哥词典";
        internal static void SetStartup(bool startup)
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey
                ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (startup)
                rk.SetValue(KeyName, Application.ExecutablePath);
            else
                rk.DeleteValue(KeyName, false);
        }


        internal static bool GetIfStartUp()
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey
    ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            return rk.GetValue(KeyName)?.Equals(Application.ExecutablePath) == true;
        }
    }
}
