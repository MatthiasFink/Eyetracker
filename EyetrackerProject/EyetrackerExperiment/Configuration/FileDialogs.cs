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

        public static String[] SelectFiles(String initialPath, String filter)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.InitialDirectory = initialPath;
            dlg.Filter = filter;
            dlg.CheckFileExists = true;
            dlg.Multiselect = true;
            if (dlg.ShowDialog() == DialogResult.OK)
                return dlg.FileNames;
            else
                return null;
        }
    }
}
