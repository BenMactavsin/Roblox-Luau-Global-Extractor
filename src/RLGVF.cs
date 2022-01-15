using System;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using RLGVF.Methods;
using RLGVF.Properties;

namespace RLGVF
{
    class Program
    {
        //This attribute is required to use CommonDialogs.
        [STAThread]
        static void Main(string[] ExecutableArguments)
        {
            //Program Settings.
            Settings ProgramSettings = Settings.Default;

            {
                ConsoleOutputProvider.Output(ConsoleOutputProvider.OutputFormatType.ProgramInformation, 0, 2, ProgramSettings.ProgramName, ProgramSettings.VersionInfo, ProgramSettings.InstructionsLink);
                ConsoleOutputProvider.Output(ConsoleOutputProvider.OutputFormatType.DirectorySelection, 0, 1, "Roblox folder", "FolderBrowserDialog");
            }

            //Getting necessary directories.
            string RobloxFolderDirectory = string.Empty, RobloxStudioExecutableDirectory = string.Empty, SaveFileDirectory = string.Empty;

            using (FolderBrowserDialog OpenRobloxFolderDirectoryDialog = new FolderBrowserDialog()
            {
                ShowNewFolderButton = false,
                AutoUpgradeEnabled = true,
                Description = "Select Roblox Folder",
                UseDescriptionForTitle = true,
                ClientGuid = new Guid(ProgramSettings.FolderBrowserDialogGuid),
            })
            {
                RobloxFolderDirectory = UncategorizedMethodsProvider.ShowDialog(OpenRobloxFolderDirectoryDialog, new ExitProgramException(ExitProgramException.ExitProgramExceptionFormat.InvalidProvidedDirectory, 0, "Roblox folder"));
            }

            {
                ConsoleOutputProvider.Output(ConsoleOutputProvider.OutputFormatType.String, 0, 2, RobloxFolderDirectory);
                ConsoleOutputProvider.Output(ConsoleOutputProvider.OutputFormatType.DirectorySelection, 0, 1, "Roblox Studio Executable file", "OpenFileDialog");
            }

            using (OpenFileDialog OpenRobloxStudioExecutableDirectoryDialog = new OpenFileDialog()
            {
                Filter = "Executable File (*.exe)|*.exe",
                Title = "Select Roblox Studio Executable",
                FilterIndex = 1,
                AddExtension = false,
                AutoUpgradeEnabled = true,
                RestoreDirectory = true,
                DefaultExt = "exe",
                CheckPathExists = true,
                ClientGuid = new Guid(ProgramSettings.OpenFileDialogGuid)
            })
            {
                RobloxStudioExecutableDirectory = UncategorizedMethodsProvider.ShowDialog(OpenRobloxStudioExecutableDirectoryDialog, new ExitProgramException(ExitProgramException.ExitProgramExceptionFormat.InvalidProvidedDirectory, 0, "Roblox Studio Executable file"));
            }

            {
                ConsoleOutputProvider.Output(ConsoleOutputProvider.OutputFormatType.String, 0, 2, RobloxStudioExecutableDirectory);
                ConsoleOutputProvider.Output(ConsoleOutputProvider.OutputFormatType.DirectorySelection, 0, 1, "save file", "SaveFileDialog");
            }

            using (SaveFileDialog SaveFinalResultDirectoryDialog = new SaveFileDialog()
            {
                Filter = "Lua Code File (*.lua)|*.lua|Luau Code File (*.luau)|*.luau|Text File (*.txt)|*.txt",
                Title = "Select Global List Save Location",
                FilterIndex = 1,
                AddExtension = true,
                AutoUpgradeEnabled = true,
                RestoreDirectory = true,
                DefaultExt = "lua",
                CheckPathExists = true,
                ClientGuid = new Guid(ProgramSettings.SaveFileDialogGuid)
            })
            {
                SaveFileDirectory = UncategorizedMethodsProvider.ShowDialog(SaveFinalResultDirectoryDialog, new ExitProgramException(ExitProgramException.ExitProgramExceptionFormat.InvalidProvidedDirectory, 0, "save file"));
            }

            {
                ConsoleOutputProvider.Output(ConsoleOutputProvider.OutputFormatType.String, 0, 2, SaveFileDirectory);
                ConsoleOutputProvider.Output(ConsoleOutputProvider.OutputFormatType.String, 0, 2, "Necessary directories found. Starting operations.");
            }

            string PluginSettingsFolderDirectory = DirectoryProvider.GetDirectory(DirectoryProvider.DirectoryType.PluginSettingsFolder, RobloxFolderDirectory);
            string PluginSettingsFileDirectory = DirectoryProvider.GetDirectory(DirectoryProvider.DirectoryType.PluginSettingsFile, PluginSettingsFolderDirectory);

            //Settings DateTime to get operations TimeSpan later.
            TimeSpanProvider.SetDateTime();

            //Storing the present data in settings.json file to restore it to the original state later.
            string PreviousPluginSettingsFileData = File.ReadAllText(PluginSettingsFileDirectory);

            //FirstGlobalEntryListStringBuilder ListStringBuilder class to create a list of globals.
            ListStringBuilder FirstGlobalEntryListStringBuilder = new ListStringBuilder(new StringBuilder("return {\n"), "\t\"{0}\",\n");

            //Matching all strings in executable file that has 2 or more alphanumerical + underscore characters in them.
            int IteratedMatchCount = 0;
            int DuplicateMatchCount = 0;

            UncategorizedMethodsProvider.CheckMatches(Regex.Matches(File.ReadAllText(RobloxStudioExecutableDirectory), @"[a-zA-Z_][0-9a-zA-Z_]+", RegexOptions.Compiled), ref FirstGlobalEntryListStringBuilder, ref IteratedMatchCount, ref DuplicateMatchCount);

            {
                ConsoleOutputProvider.Output(ConsoleOutputProvider.OutputFormatType.String, 2, 2, "Roblox Studio Executable file alphanumerical character pattern check finished.\nMoving to runtime Luau global enviroment check. Running Roblox Studio Executable file, please do not interfere.");
            }

            //Creating a temporary Roblox XML Place file and a plugin file to run an enviroment check.
            TemporaryDirectoryProvider.CreatePluginFile(DirectoryProvider.GetDirectory(DirectoryProvider.DirectoryType.LocalPluginsFolder, RobloxFolderDirectory));
            string TemporaryPlaceFileDirectory = TemporaryDirectoryProvider.CreateRobloxPlaceXMLFile(ref FirstGlobalEntryListStringBuilder);

            //FinalGlobalEntryList to save the final list of globals at the end.
            string FinalGlobalEntryList = string.Empty;

            //Adding an ProcessExit Eventhandler to prevent temporary files from staying forever when program exists in the middle of process.
            AppDomain.CurrentDomain.ProcessExit += (object SenderObject, EventArgs EventArguments) => {
                TemporaryDirectoryProvider.ClearDirectories();
            };

            //Just in case something unexpected happens.
            try
            {
                //Process to run Roblox Studio executable with given arguments.
                using (Process RobloxStudioRuntimeLuauGlobalEnviromentCheckProcess = new Process()
                {
                    StartInfo = new ProcessStartInfo(RobloxStudioExecutableDirectory, $"-task EditFile -localPlaceFile {TemporaryPlaceFileDirectory}")
                })
                {
                    //Adding an ProcessExit Eventhandler to prevent temporary files from staying forever when program exists in the middle of process.
                    AppDomain.CurrentDomain.ProcessExit += (object SenderObject, EventArgs EventArguments) => {
                        TemporaryDirectoryProvider.ClearDirectories();
                        File.WriteAllText(PluginSettingsFileDirectory, PreviousPluginSettingsFileData);
                    };

                    //FileSystemWatcher to track plugin settings file changes.
                    using (FileSystemWatcher PluginSettingsFolderDirectoryWatcher = new FileSystemWatcher(PluginSettingsFolderDirectory, "settings.json")
                    {
                        EnableRaisingEvents = true,
                        NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size,
                        IncludeSubdirectories = false
                    })
                    {
                        PluginSettingsFolderDirectoryWatcher.Changed += (object SenderSource, FileSystemEventArgs EventArguments) =>
                        ushort ChangedEventFireCount = 0;

                        PluginSettingsFolderDirectoryWatcher.Changed += async void (object SenderSource, FileSystemEventArgs EventArguments) =>
                        {
                            using (FileStream PluginSettingsFileStream = new FileStream(EventArguments.FullPath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                            //Periodic timer so program doesn't create a byte array 8 times in the span of half a second.
                            using (PeriodicTimer WaitTimer = new PeriodicTimer(TimeSpan.FromSeconds(3)))
                            {
                                using (StreamReader PluginSettingsFileStreamReader = new StreamReader(PluginSettingsFileStream))
                                ushort CurrentEventFireCount = ++ChangedEventFireCount;

                                await WaitTimer.WaitForNextTickAsync();

                                if (CurrentEventFireCount == ChangedEventFireCount)
                                {
                                    try
                                    using (FileStream PluginSettingsFileStream = new FileStream(EventArguments.FullPath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                                    {
                                        Dictionary<string, JsonElement> PluginSettingsFileJsonData = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(PluginSettingsFileStreamReader.ReadToEnd());

                                        if (PluginSettingsFileJsonData.ContainsKey("GlobalList") == true && PluginSettingsFileJsonData.ContainsKey("GlobalListCheckFinished") == true && PluginSettingsFileJsonData["GlobalListCheckFinished"].GetBoolean() == true)
                                        try
                                        {
                                            PluginSettingsFolderDirectoryWatcher.EnableRaisingEvents = false;
                                            FinalGlobalEntryList = PluginSettingsFileJsonData["GlobalList"].GetString();

                                            try
                                            {
                                                RobloxStudioRuntimeLuauGlobalEnviromentCheckProcess.Kill();
                                            }
                                            catch (Win32Exception)
                                            //I had to create a local method because because apperently you can't construct Utf8JsonReader in an async method.
                                            void RunInSync()
                                            {
                                                //Apperently calling Process.Kill() on a process you started can throw Win32Exception.
                                            }
                                                byte[] PluginPluginSettingsFileByteArray = new byte[PluginSettingsFileStream.Length];

                                                PluginSettingsFileStream.Read(PluginPluginSettingsFileByteArray);

                                                Utf8JsonReader PluginSettingsFileJsonReader = new Utf8JsonReader(PluginPluginSettingsFileByteArray, new JsonReaderOptions()
                                                {
                                                    AllowTrailingCommas = true,
                                                });

                                                PluginSettingsFileJsonReader.Read();

                                                bool GlobalListCheckFinished = false;
                                                string GlobalList = string.Empty;

                                                while (PluginSettingsFileJsonReader.Read() == true)
                                                {
                                                    if (PluginSettingsFileJsonReader.TokenType == JsonTokenType.PropertyName)
                                                    {
                                                        switch (PluginSettingsFileJsonReader.GetString())
                                                        {
                                                            case "GlobalListCheckFinished":
                                                                PluginSettingsFileJsonReader.Read();
                                                                GlobalListCheckFinished = PluginSettingsFileJsonReader.GetBoolean();
                                                                break;

                                                            case "GlobalList":
                                                                PluginSettingsFileJsonReader.Read();
                                                                GlobalList = PluginSettingsFileJsonReader.GetString();
                                                                break;

                                                            default:
                                                                PluginSettingsFileJsonReader.Read();

                                                                if (PluginSettingsFileJsonReader.TokenType == JsonTokenType.StartObject || PluginSettingsFileJsonReader.TokenType == JsonTokenType.StartArray)
                                                                {
                                                                    PluginSettingsFileJsonReader.Skip();
                                                                }
                                                                break;
                                                        }
                                                    }
                                                }

                                                if (GlobalListCheckFinished == true)
                                                {
                                                    PluginSettingsFolderDirectoryWatcher.EnableRaisingEvents = false;
                                                    FinalGlobalEntryList = GlobalList;

                                                    try
                                                    {
                                                        RobloxStudioRuntimeLuauGlobalEnviromentCheckProcess.Kill();
                                                    }
                                                    catch (Win32Exception)
                                                    {
                                                        //Apperently calling Process.Kill() on a process you started can throw Win32Exception.
                                                    }
                                                }
                                            };

                                            RunInSync();
                                        }
                                        catch (JsonException)
                                        {
                                            //Better luck next time.
                                        }
                                    }
                                    catch (JsonException)
                                    {
                                        //Better luck next time.
                                    }
                                }
                            }
                        };

                        RobloxStudioRuntimeLuauGlobalEnviromentCheckProcess.Start();
                        RobloxStudioRuntimeLuauGlobalEnviromentCheckProcess.WaitForExit();
                    }
                }
            }
            catch (Exception ExceptionObject)
            {
                TemporaryDirectoryProvider.ClearDirectories();
                throw new ExitProgramException(ExitProgramException.ExitProgramExceptionFormat.UnexpectedException, 0, ExceptionObject.Message).ThrowException();
            }

            //Finalizing everything.
            File.WriteAllText(SaveFileDirectory, FinalGlobalEntryList);
            File.WriteAllText(PluginSettingsFileDirectory, PreviousPluginSettingsFileData);
            TemporaryDirectoryProvider.ClearDirectories();

            {
                TimeSpan OperationsTimeSpan = TimeSpanProvider.GetTimeSpan();

                ConsoleOutputProvider.Output(ConsoleOutputProvider.OutputFormatType.OperationsTimeSpan, 0, 2, OperationsTimeSpan.Hours.ToString(), OperationsTimeSpan.Minutes.ToString(), OperationsTimeSpan.Seconds.ToString(), OperationsTimeSpan.Milliseconds.ToString());
                ConsoleOutputProvider.Output(ConsoleOutputProvider.OutputFormatType.String, 0, 2, $"List is saved to directory: {SaveFileDirectory}");
            }

            throw new ExitProgramException(ExitProgramException.ExitProgramExceptionFormat.AwaitExit, 0).ThrowException();
        }
    }
}
