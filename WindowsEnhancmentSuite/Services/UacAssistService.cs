using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Principal;

namespace WindowsEnhancementSuite.Services
{
    public class UacAssistService
    {
        private readonly string[] parameters;

        private const string commandFormat = "{0} \"{1}\" \"{2}\"";
        public static void TryAsAdmin(UacAssistCommand command, string file1, string file2)
        {
            string assemblyPath = Assembly.GetEntryAssembly().Location;
            string commandText = String.Format(commandFormat, command, file1, file2);
            var processInfo = new ProcessStartInfo(assemblyPath, commandText)
            {
                Verb = "runas",
                UseShellExecute = true
            };

            System.Diagnostics.Process.Start(processInfo);
        }

        public UacAssistService(string[] parameter)
        {
            parameters = parameter;
        }

        public bool Process()
        {          
            if (!isAdministrator()) return false;

            try
            {
                switch (getCommand())
                {
                    case UacAssistCommand.COPY:
                        File.Copy(parameters[1], parameters[2]);
                        return true;
                    case UacAssistCommand.MOVE:
                        File.Move(parameters[1], parameters[2]);
                        return true;
                    default:
                        return false;
                }
            }
            catch (Exception)
            {
            }

            return false;
        }

        private UacAssistCommand getCommand()
        {
            if (parameters.Length < 3) return UacAssistCommand.NONE;
            if (!File.Exists(parameters[1])) return UacAssistCommand.NONE;

            string directroyName = Path.GetDirectoryName(parameters[2]);
            if (String.IsNullOrWhiteSpace(directroyName)) return UacAssistCommand.NONE;
            if (!Directory.Exists(directroyName)) return UacAssistCommand.NONE;

            var commands = Enum.GetNames(typeof(UacAssistCommand));
            for (sbyte i = 0; i < commands.Length; i++)
            {
                if (parameters[0].ToUpper() == commands[i]) return (UacAssistCommand)i;
            }

            return UacAssistCommand.NONE;
        }

        private bool isAdministrator()
        {
            var identity = WindowsIdentity.GetCurrent();
            if (identity == null) return false;

            return new WindowsPrincipal(identity).IsInRole(WindowsBuiltInRole.Administrator);
        }
    }

    public enum UacAssistCommand
    {
        NONE = -1,
        COPY = 0,
        MOVE = 1
    }
}
