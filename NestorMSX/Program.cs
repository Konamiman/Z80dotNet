using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Konamiman.NestorMSX.Emulator;
using Konamiman.NestorMSX.Exceptions;
using Konamiman.NestorMSX.Misc;

namespace Konamiman.NestorMSX
{
    public class Program
    {
        private const string BasePathInDevelopmentEnvironment = @"c:\VS\Projects\Z80dotNet\NestorMSX\";

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

            var environment = CreateEmulationEnvironment(args);

            if(environment != null)
                environment.Run();
        }

        private static MsxEmulationEnvironment CreateEmulationEnvironment(string[] args)
        {
            MsxEmulationEnvironment environment = null;
            try {
                environment = (new Program()).CreateEmulationEnvironmentCore(args);
            }
            catch(ConfigurationException ex) {
                Tell("Sorry, something went wrong with the configuration file:\r\n\r\n{0}", ex.Message);
            }
            catch(EmulationEnvironmentCreationException ex) {
                Tell("Sorry, I couldn't create the emulation environment:\r\n\r\n{0}", ex.Message);
            }
            catch(Exception ex) {
                Tell("Ooops, something went wrong and I am not sure why... here are the ugly details:\r\n\r\n{0}",
                    ex.ToString().Replace(BasePathInDevelopmentEnvironment, "", StringComparison.InvariantCultureIgnoreCase));
            }
            return environment;
        }

        private MsxEmulationEnvironment CreateEmulationEnvironmentCore(string[] args)
        {
            string slot2FileNameOverride = null;
            var configFileName = "NestorMSX.config".AsAbsolutePath();

            ParseArgs(args, ref configFileName, ref slot2FileNameOverride);

            var config = ReadConfig(configFileName);
            if(slot2FileNameOverride != null)
                config.Slot2RomFile = slot2FileNameOverride;

            return new MsxEmulationEnvironment(config);
        }

        private void ParseArgs(string[] args, ref string configFileName, ref string slot2FileNameOverride)
        {
            var configKey = "config=";
            var slot2Key = "slot2=";

            foreach(var arg in args) {
                if(arg.StartsWith(configKey, StringComparison.InvariantCultureIgnoreCase))
                    configFileName = arg.Substring(configKey.Length).AsAbsolutePath();

                if(arg.StartsWith(slot2Key, StringComparison.InvariantCultureIgnoreCase))
                    slot2FileNameOverride = arg.Substring(slot2Key.Length).AsAbsolutePath();
            }
        }

        private static Configuration ReadConfig(string configFileName)
        {
            Configuration config;

            try {
                config = IniDeserializer.Deserialize<Configuration>(configFileName);
            }
            catch(FileNotFoundException ex) {
                throw new ConfigurationException(
                    "Configuration file not found. It is supposed to be here:\r\n{0}".FormatWith(configFileName),
                    ex);
            }
            catch(DeserializationException ex) {
                throw new ConfigurationException(
                    "The value of '{0}' is invalid. Perhaps it should be a number but it is not?".FormatWith(ex.Key),
                    ex);
            }

            VerifyMandatoryFields(config);

            return config;
        }

        private static void VerifyMandatoryFields(Configuration config)
        {
            var mandatoryStringProperties =
                config.GetType().GetProperties()
                    .Where(p => p.GetCustomAttribute<MandatoryAttribute>() != null);

            foreach(var property in mandatoryStringProperties) {
                var value = (string)property.GetValue(config);
                if(string.IsNullOrWhiteSpace(value))
                    throw new ConfigurationException("Key '{0}' must have a value.".FormatWith(property.Name));
            }

            if(config.DiskRomFile != null && config.FilesystemBaseLocation == null)
                throw new ConfigurationException(
                    "A value for 'FilesystemBaseLocation' must be provided if 'DiskRomFile' has a value'");
        }

        public static void Tell(string message, params object[] parameters)
        {
            MessageBox.Show(message.FormatWith(parameters), "NestorMSX");
        }
    }
}
