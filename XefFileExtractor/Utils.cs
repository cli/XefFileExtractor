using System;
using System.IO;
using Microsoft.Win32;

namespace XefFileExtractor {
    public static class Utils {
        public static byte ClipToByte(int p_ValueToClip) {
            return
                Convert.ToByte((p_ValueToClip < byte.MinValue)
                    ? byte.MinValue
                    : ((p_ValueToClip > byte.MaxValue) ? byte.MaxValue : p_ValueToClip));
        }

        public static void WriteTimingToFile(string filepath, double[] timing)
        {
            using (StreamWriter file = new StreamWriter(filepath))
            {
                foreach (double time in timing)
                {
                    file.WriteLine(time.ToString());
                }
            }
        }

        /// <summary>
        /// Checks if the given directory exists. If not it is tried to create it.
        /// </summary>
        /// <param name="path"></param>
        public static void ExistOrCreateDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public static string OpenXefFile() {
            string fileName = string.Empty;

            OpenFileDialog dlg = new OpenFileDialog();
            //dlg.FileName = _currentFilePath;
            dlg.DefaultExt = Properties.Resources.XefExtension; // Default file extension
            dlg.Filter = Properties.Resources.EventFileDescription + " " + Properties.Resources.EventFileFilter;
            // Filter files by extension 
            bool? result = dlg.ShowDialog();

            if (result == true) {
                fileName = dlg.FileName;
            }

            return fileName;
        }
    }
}