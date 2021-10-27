using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using log4net.Config;
using Myrtille.Services.Contracts;

namespace Myrtille.RDP
{
    public class Program
    {
        [DllImport("wfreerdp-client.dll")]
        private static extern int StartRdpClient();

        private static int Main(string[] args)
        {
            // enable the code below for debug; disable otherwise
            //if (Environment.UserInteractive)
            //{
            //    MessageBox.Show("Attach the .NET debugger to the 'RDP Debug' Myrtille.RDP.exe process now for debug. Click OK when ready...", "RDP Debug");
            //}
            //else
            //{
            //    Thread.Sleep(10000);
            //}

            // logger
            XmlConfigurator.Configure();

            string argKeyValueSeparator = ":";
            foreach (string arg in args)
            {
                var argParts = arg.Trim().Split(argKeyValueSeparator.ToCharArray(), 2);
                parseCommandLineArg(argParts[0].ToLower(), (argParts.Length > 1 ? argParts[1] : ""));
            }

            if (!ValidConfig)
            {
                return (int)RemoteSessionExitCode.InvalidConfiguration;
            }

            int exitCode = (int)RemoteSessionExitCode.Success;

            try
            {
                exitCode = StartRdpClient();
            }
            catch (Exception e)
            {
                if (ConsoleOutput)
                {
                    Console.WriteLine(e.Message);
                }

                Trace.TraceError("RDP error, remote session {0} ({1})", RemoteSessionID, e);

                exitCode = (int)RemoteSessionExitCode.Unknown;
            }

            return exitCode;
        }

        #region configuration

        private static string RemoteSessionID { get; set; } // Myrtille session ID used for pipe messaging
        private static bool LoggingEnabled { get; set; } // Myrtille logging parameter
        private static bool ConsoleOutput { get; set; } // Output comms to console window
        private static uint Height { get; set; } // Height of RDP session
        private static uint Width { get; set; } // Width of RDP session

        /// <summary>
        /// Indicate all command line parameters for correct operation have been received.
        /// </summary>
        private static bool ValidConfig
        {
            get
            {
                if (string.IsNullOrEmpty(RemoteSessionID)) return false;
                if (Height == 0) return false;
                if (Width == 0) return false;
                return true;
            }
        }

        /// <summary>
        /// Parse command line arguments
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="value"></param>
        private static void parseCommandLineArg(string arg, string value)
        {
            switch (arg)
            {
                case "/myrtille-sid":
                    RemoteSessionID = value.Trim();
                    break;
                case "/myrtille-window":
                    ConsoleOutput = true;
                    break;
                case "/myrtille-log":
                    LoggingEnabled = true;
                    break;
                case "/h":
                    Height = uint.Parse(value);
                    break;
                case "/w":
                    Width = uint.Parse(value);
                    break;
            }
        }

        #endregion
    }
}