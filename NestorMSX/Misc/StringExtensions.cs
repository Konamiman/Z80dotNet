using System;
using System.IO;
using System.Reflection;

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
    }
}
