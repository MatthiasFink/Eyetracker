using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EyetrackerExperiment.Configuration
{
    class FileDialogs
    {
        public static String SelectFolder(String initialPath)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.SelectedPath = initialPath;
            if (dlg.ShowDialog() == DialogResult.OK)
                return dlg.SelectedPath;
            else
                return null;
        }
    }
}
