using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Tools.CSharp.Extensions
{
    public static class DirectoryInfoExtension
    {
        #region private
        private static void _CopyAllDirectory(DirectoryInfo sourceDir, DirectoryInfo destDist, bool fullCopy)
        {
            //Если каталог не существует, то создаем
            if (!Directory.Exists(sourceDir.FullName))
            { Directory.CreateDirectory(sourceDir.FullName); }

            //копируем все файлы в каталог назначения
            foreach (var sourceFile in sourceDir.GetFiles())
            {
                var tempPath = Path.Combine(destDist.FullName, sourceFile.Name);
                if (!fullCopy && File.Exists(tempPath))
                {
                    var lastTimeSourceFile = File.GetLastWriteTime(sourceFile.FullName);
                    var lastTimeDestitionFile = File.GetLastWriteTime(tempPath);
                    if (lastTimeSourceFile < lastTimeDestitionFile)
                    { continue; }
                }
                sourceFile.CopyTo(tempPath, true);
            }

            //копируем все каталоги в каталог назначения
            foreach (var sourceSubDir in sourceDir.GetDirectories())
            {
                var tempPath = destDist.CreateSubdirectory(sourceSubDir.Name);
                _CopyAllDirectory(sourceSubDir, tempPath, fullCopy);
            }
        }
        //---------------------------------------------------------------------
        private static string _GetDemandDirectory(string fullPath, bool thisDirOnly)
        {
            string demandPath;

            if (thisDirOnly)
            {
                if (fullPath.EndsWith(Path.DirectorySeparatorChar) || fullPath.EndsWith(Path.AltDirectorySeparatorChar))
                {  demandPath = fullPath + '.'; }
                else
                {  demandPath = fullPath + Path.DirectorySeparatorChar + '.'; }
            }
            else
            {
                if (!(fullPath.EndsWith(Path.DirectorySeparatorChar) || fullPath.EndsWith(Path.AltDirectorySeparatorChar)))
                {  demandPath = fullPath + Path.DirectorySeparatorChar; }
                else
                {  demandPath = fullPath; }
            }
            return demandPath;
        }

        #endregion
        //---------------------------------------------------------------------
        public static string GetRootAndParentAndName(this DirectoryInfo directoryInfo)
        {
            if (directoryInfo == null)
            { return string.Empty; }

            var parent = directoryInfo.Parent;
            var root = directoryInfo.Root;

            if (parent == null)
            { return directoryInfo.Name; }

            if (root.Name.Equals(parent.Name, StringComparison.OrdinalIgnoreCase))
            { return string.Format(CultureInfo.CurrentCulture, "{0}{1}", root.Name, directoryInfo.Name); }

            //-----------------------------------------------------------------
            var result = new StringBuilder();
            result.Append(root.Name);

            if (!string.IsNullOrWhiteSpace(parent.Parent?.Name))
            { result.Append(string.Format("...{0}", Path.DirectorySeparatorChar)); }

            result.Append(parent.Name);
            result.Append(Path.DirectorySeparatorChar);
            result.Append(directoryInfo.Name);

            return result.ToString();
        }
        //---------------------------------------------------------------------
        public static void CopyTo(this DirectoryInfo sourceDirectory, DirectoryInfo destinationDirectory, bool fullCopy)
        {
            if (sourceDirectory == null)
            { throw new ArgumentNullException(nameof(sourceDirectory)); }
            if (!sourceDirectory.Exists)
            { throw new DirectoryNotFoundException(nameof(sourceDirectory)); }
            if (destinationDirectory == null)
            { throw new ArgumentNullException(nameof(destinationDirectory)); }

            var sourcePath = _GetDemandDirectory(sourceDirectory.FullName, false);
            var destinationPath = _GetDemandDirectory(destinationDirectory.FullName, false);

            if (string.Equals(sourcePath, destinationPath, StringComparison.OrdinalIgnoreCase))
            { throw new IOException(nameof(destinationDirectory)); }

            _CopyAllDirectory(sourceDirectory, destinationDirectory, fullCopy);
        }
        //---------------------------------------------------------------------
        public static bool IsEmpty(this DirectoryInfo directoryInfo)
        {
            if (directoryInfo == null)
            { throw new ArgumentNullException(nameof(directoryInfo)); }
            if (!directoryInfo.Exists)
            { throw new DirectoryNotFoundException(nameof(directoryInfo)); }

            if (directoryInfo.GetFiles().Any())
            { return false; }

            if (directoryInfo.GetDirectories().Any())
            { return false; }

            return true;
        }
        //---------------------------------------------------------------------
    }
}
