using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace Синхронизация
{
    internal class Sync
    {
        public Sync(string pathSourse, string pathCopy, WriteText writeText, string notCopiedKeys)
        {
            PathSourse = pathSourse.Trim();
            PathCopy = pathCopy.Trim() + $"\\{PathSourse.Substring(PathSourse.LastIndexOf("\\") + 1)}";
            this.writeText = writeText;
            this.notCopiedKeys = notCopiedKeys;
        }

        public string PathSourse { get; private set; }
        public string PathCopy { get; private set; }

        public delegate void WriteText(string text);
        private WriteText writeText;
        private string notCopiedKeys;


        public void Copy()
        {
            writeText("-----------");
            writeText("КОПИРОВАНИЕ");
            writeText("-----------");
            CopyRec(PathSourse, PathCopy);
        }
        private void CopyRec(string pathSourse, string pathCopy)
        {
            var dir = new DirectoryInfo(pathSourse);

            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

            DirectoryInfo[] dirs = dir.GetDirectories();

            Directory.CreateDirectory(pathCopy);

            foreach (FileInfo file in dir.GetFiles())
            {
                if (notCopiedKeys.Contains(file.Name[0]))
                    continue;

                string targetFilePath = Path.Combine(pathCopy, file.Name);
                file.CopyTo(targetFilePath, true);
                writeText($"Копирование файла {file.FullName} в {pathCopy}");
            }

            foreach (DirectoryInfo subDir in dirs)
            {
                if (notCopiedKeys.Contains(subDir.Name[0]))
                    continue;

                string newDestinationDir = Path.Combine(pathCopy, subDir.Name);
                CopyRec(subDir.FullName, newDestinationDir);
            }
        }
        public void CheckFilesAndDirs()
        {
            writeText("-------------------");
            writeText("ПРОВЕРКА И УДАЛЕНИЕ");
            writeText("-------------------");
            CheckFilesAndDirsRec(PathCopy, PathSourse, "", "");
        }
        private void CheckFilesAndDirsRec(string pathS, string pathC, string pathSourse, string pathCheck) //Все пути, кроме PATH относительные
        {
            var dir = new DirectoryInfo(Path.Combine(pathS, pathSourse));

            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

            DirectoryInfo[] dirs = dir.GetDirectories();

            foreach (FileInfo file in dir.GetFiles())
            {
                if (notCopiedKeys.Contains(file.Name[0]))
                    continue;

                string fileName = Path.GetFileName(file.FullName);

                if (!File.Exists(Path.Combine(Path.Combine(pathC, pathCheck), fileName)))
                {
                    file.Delete();
                    writeText($"Удаление файл {file.FullName}");
                }
            }

            foreach (DirectoryInfo subDir in dirs)
            {
                var dirC = new DirectoryInfo(Path.Combine(Path.Combine(pathC, pathCheck), subDir.Name));

                if (notCopiedKeys.Contains(subDir.Name[0]))
                    continue;

                if (!dirC.Exists)
                {
                    subDir.Delete(true);
                    writeText($"Удаление папки {dirC}");
                    continue;
                }

                string newPathSourse = Path.Combine(pathSourse, subDir.Name);
                string newPathCheck = Path.Combine(pathCheck, subDir.Name);

                CheckFilesAndDirsRec(pathS, pathC, newPathSourse, newPathCheck);
            }
        }
    }
}
