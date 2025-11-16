using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Синхронизация
{
    internal class FileHandler
    {
        public string Path { get; private set; }

        public FileHandler(string path)
        {
            Path = path;
        }

        public void Update(string value)
        {
            using (FileStream fs = new FileStream(Path, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    writer.Write(value);
                }
            }
        }

        public string Read()
        {
            using (StreamReader reader = new StreamReader(Path))
            {
                string content = reader.ReadToEnd();
                return content;
            }
        }

        public bool Exist()
        {
            return File.Exists(Path);
        }

        public static void createLinkFile(string path, string name, string value, string arguments)
        {
            string shortcutPath = System.IO.Path.Combine(path, name + ".lnk");

            IWshRuntimeLibrary.WshShell shell = new IWshRuntimeLibrary.WshShell();
            IWshRuntimeLibrary.IWshShortcut shortcut = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(shortcutPath);

            shortcut.TargetPath = value; 
            shortcut.Description = name;
            shortcut.Arguments = arguments;
            shortcut.WorkingDirectory = System.IO.Path.GetDirectoryName(value);

            shortcut.Save();
        }
    }
}
