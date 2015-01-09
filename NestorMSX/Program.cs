using System;
using System.IO;
using System.Runtime.Serialization;
using System.Windows.Forms;
using Konamiman.NestorMSX.Emulator;
using Konamiman.NestorMSX.Host;
using Konamiman.NestorMSX.Misc;

namespace Konamiman.NestorMSX
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length > 0 && args[0].ToLower() == "keytest") {
                Application.Run(new KeyTestForm());
                return;
            }

            var configFileName = "NestorMSX.config".AsAbsolutePath();

            var config = ReadConfig(configFileName);
            if(config == null)
                return;

            (new MsxEmulationEnvironment(config)).Run();
        }

        private static Configuration ReadConfig(string configFileName)
        {
            Configuration config = null;
            try {
                config = IniDeserializer.Deserialize<Configuration>(configFileName);
            }
            catch(FileNotFoundException) {
                MessageBox.Show(
                    "Sorry, I couldn't find the configuration file. It is supposed to be here:\r\n" + configFileName,
                    "NestorMSX");
            }
            catch(DeserializationException ex) {
                MessageBox.Show(
                    "Sorry, I couldn't parse the configuration file. The value of '" + ex.Key + "' is invalid.",
                    "NestorMSX");
            }
            return config;
        }
    }
}
