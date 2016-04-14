using System;
using System.IO;

namespace Tools.CSharp.Extensions
{
    public static class FileInfoExtensions
    {
        //---------------------------------------------------------------------
        public static string GetNameWithoutExtension(this FileInfo file)
        {
            if (file == null)
            { throw new ArgumentNullException(nameof(file)); }

            var fileName = file.Name;
            var nameWithoutExtension = string.IsNullOrWhiteSpace(fileName) ? fileName : fileName.Replace(file.Extension, "");
            return nameWithoutExtension;
        }
        //---------------------------------------------------------------------
        public static string GetRootAndParentAndNameDirectoryName(this FileInfo file, bool isCheckFileExists = true, bool fullPath = false)
        {
            if (file == null)
            { throw new ArgumentNullException(nameof(file)); }

            if (isCheckFileExists && !file.Exists)
            { return string.Empty; }

            string directoryName;

            if (fullPath)
            { directoryName = file.FullName; }
            else
            {
                var rootAndParentAndName = file.Directory.GetRootAndParentAndName();
                directoryName = string.IsNullOrWhiteSpace(rootAndParentAndName) ? rootAndParentAndName : Path.Combine(rootAndParentAndName, file.Name);
            }

            return directoryName;
        }
        //---------------------------------------------------------------------
        public static string GetFullFileName(this FileInfo file)
        {
            return GetRootAndParentAndNameDirectoryName(file, false, true);
        }
        //---------------------------------------------------------------------
        public static string GetTempFilePathWithExtension(string extension = ".tmp")
        {
            var path = Path.GetTempPath();
            var fileName = Guid.NewGuid().ToString() + extension;
            return Path.Combine(path, fileName);
        }
        //---------------------------------------------------------------------
    }
}
