using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Globalization;
using System.IO;
using System.Text;

namespace Tools.CSharp.Extensions
{
    public static class StringExtensions
    {
        #region private
        private const string _ReplaceNewLine = " ";
        #endregion
        //---------------------------------------------------------------------
        public const char DefaultOfficialSymbol = '*';
        //---------------------------------------------------------------------
        public static readonly ReadOnlyCollection<string> NewLines = new List<string> { Environment.NewLine, "\n" }.AsReadOnly();
        //---------------------------------------------------------------------
        public static bool IsDigit(this string value)
        {
            return IsDigit(value, default(char));
        }
        public static bool IsDigit(this string value, char exceptSymbol)
        {
            if (string.IsNullOrEmpty(value))
            { return false; }

            return exceptSymbol == default(char) ? value.All(char.IsDigit) : value.All(x => exceptSymbol == x || char.IsDigit(x));
        }
        //---------------------------------------------------------------------
        public static bool AtLeastOneDigit(this string value)
        {
            return (value != null) && (value.FirstOrDefault(char.IsDigit) != default(char));
        }
        //---------------------------------------------------------------------
        public static string RemoveNewLines(this string value)
        {
            var result = new StringBuilder(value);

            foreach (var newLine in NewLines)
            { result.Replace(newLine, _ReplaceNewLine); }

            return result.ToString();
        }
        //---------------------------------------------------------------------
        public static string NormalizePath(this string path)
        {
            if (string.IsNullOrEmpty(path))
            { return path; }

            var newPath = Path.GetFullPath(new Uri(path).LocalPath).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            return newPath;
        }
        //---------------------------------------------------------------------
        public static bool Contains(this string str, string value, StringComparison comparisonType)
        {
            if (string.IsNullOrEmpty(str))
            { throw new ArgumentNullException(nameof(str)); }

            return str.IndexOf(value, comparisonType) >= 0;
        }
        //---------------------------------------------------------------------
        public static bool FoundByPattern(this string str, string pattern, char officialSymbol = DefaultOfficialSymbol, bool insensitiveRegister = true)
        {
            if (string.IsNullOrWhiteSpace(str) || string.IsNullOrEmpty(pattern))
            { return false; }

            var indexStr = 0;
            var indexPattern = 0;
            var saveIndexPattern = 0;
            var anyCharacter = false;

            for (; ; )
            {
                if (pattern.Length > indexPattern && pattern[indexPattern] == officialSymbol)
                {
                    while (pattern.Length > indexPattern && pattern[indexPattern] == officialSymbol)
                    { indexPattern++; }

                    if (pattern.Length == indexPattern)
                    { return true; }

                    saveIndexPattern = indexPattern;
                    anyCharacter = true;
                }

                var saveIndexStr = indexStr;

                for (; ; )
                {
                    var differenceCharacterCode = 0;
                    if (str.Length > indexStr && pattern.Length > indexPattern)
                    {
                        var strChar = str[indexStr];
                        var patternChar = pattern[indexPattern];

                        if (insensitiveRegister)
                        {
                            var textInfo = CultureInfo.CurrentCulture.TextInfo;
                            strChar = textInfo.ToUpper(strChar);
                            patternChar = textInfo.ToUpper(patternChar);
                        }
                        differenceCharacterCode = strChar - patternChar;
                    }
                    else if (str.Length <= indexStr && pattern.Length > indexPattern)
                    { differenceCharacterCode = pattern[indexPattern]; }
                    else if (str.Length > indexStr && pattern.Length <= indexPattern)
                    { differenceCharacterCode = str[indexStr]; }

                    if (str.Length <= indexStr)
                    { return differenceCharacterCode == 0; }

                    if (pattern.Length == indexPattern)
                    {
                        if (!anyCharacter)
                        { return false; }

                        indexPattern = saveIndexPattern;
                        indexStr = saveIndexStr;
                        saveIndexStr++;

                        continue;
                    }

                    if (differenceCharacterCode == 0)
                    {
                        indexPattern++;
                        indexStr++;

                        break;
                    }

                    if (!anyCharacter)
                    { return false; }

                    indexPattern = saveIndexPattern;
                    saveIndexStr++;
                    indexStr = saveIndexStr;
                }
            }
        }
        //---------------------------------------------------------------------
        public static string ToIgnoreCase(this string str)
        {
            return string.IsNullOrEmpty(str) ? str : str.ToLower();
        }
        //---------------------------------------------------------------------
        public static string GetFilePathUnderDirectory(this string filePath, int liftUnderDirectoryCount, char directorySeparatorChar)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            { return filePath; }

            var lastIndexDirectorySeparatorChar = -1;
            var directorySeparatorCharCount = 0;

            for (var i = filePath.Length - 1; i >= 0; --i)
            {
                if (filePath[i].Equals(directorySeparatorChar))
                { ++directorySeparatorCharCount; }

                if (directorySeparatorCharCount == liftUnderDirectoryCount)
                { 
                    lastIndexDirectorySeparatorChar = i; 
                    break; 
                }
            }

            return lastIndexDirectorySeparatorChar == -1 ? filePath : filePath.Substring(0, lastIndexDirectorySeparatorChar);
        }
        //---------------------------------------------------------------------
        public static bool EndsWith(this string str, char value)
        {
            if (str == null)
            { return false; }

            var strLength = str.Length;
            if (strLength != 0)
            {
                if (str[strLength - 1] == value)
                { return true; }
            }
            return false;
        }
        //---------------------------------------------------------------------
    }
}