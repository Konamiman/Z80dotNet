using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace Konamiman.NestorMSX.Misc
{
    public static class StringExtensions
    {
        public static string AsAbsolutePath(this string path)
        {
            if(Path.IsPathRooted(path))
                return path;

            var specialFolderNames = Enum.GetNames(typeof(Environment.SpecialFolder));
            foreach(var name in specialFolderNames) {
                var namePlaceholder = "$" + name + "$";
                if(path.Contains(namePlaceholder)) {
                    var enumValue = (Environment.SpecialFolder)Enum.Parse(typeof(Environment.SpecialFolder), name);
                    path = path.Replace(namePlaceholder, Environment.GetFolderPath(enumValue));
                }
            }

            path = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), path);

            return path;
        }

        public static string Replace(this string str, string oldValue, string newValue, StringComparison comparison)
        {
            if(oldValue == null) {
                throw new ArgumentNullException("oldValue");
            }
            if(oldValue == "") {
                throw new ArgumentException("String cannot be of zero length.", "oldValue");
            }

            StringBuilder sb = new StringBuilder();

            int previousIndex = 0;
            int index = str.IndexOf(oldValue, comparison);
            while(index != -1) {
                sb.Append(str.Substring(previousIndex, index - previousIndex));
                sb.Append(newValue);
                index += oldValue.Length;

                previousIndex = index;
                index = str.IndexOf(oldValue, index, comparison);
            }
            sb.Append(str.Substring(previousIndex));

            return sb.ToString();
        }

        public static string FormatWith(this string str, params object[] parameters)
        {
            return string.Format(str, parameters);
        }

        public static bool IsValidKeyName(this string keyName)
        {
            return Enum.IsDefined(typeof(Keys), keyName);
        }
    }
}
