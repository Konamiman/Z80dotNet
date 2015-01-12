using System;
using System.IO;
using Konamiman.NestorMSX.Exceptions;

namespace Konamiman.NestorMSX.Misc
{
    public static class FileUtils
    {
        public static string ReadAllText(string fileName)
        {
            return ReadAllContents(File.ReadAllText, fileName);
        }

        public static string[] ReadAllLines(string fileName)
        {
            return ReadAllContents(File.ReadAllLines, fileName);
        }

        public static byte[] ReadAllBytes(string fileName)
        {
            return ReadAllContents(File.ReadAllBytes, fileName);
        }

        private static T ReadAllContents<T>(Func<string, T> readContents, string fileName)
        {
            try {
                return readContents(fileName.AsAbsolutePath());
            }
            catch(Exception ex) {
                throw new EmulationEnvironmentCreationException(
                    "{0}.\r\n\r\nFile name:\r\n{1}".FormatWith(ex.Message, fileName)
                    , ex);
            }
        }
    }
}
