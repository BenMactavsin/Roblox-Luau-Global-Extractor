using System;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text.Json;
using RLGVF.Methods;
using RLGVF.Properties;

namespace RLGVF
{
    class Program
    {
        //This attribute is required to use CommonDialogs.
        [STAThread]
        static void Main(string[] Arguments)
        {
            Settings ProgramSettings = Settings.Default;
            ConsoleOutputProvider.Output(OutputFormatType.ProgramInformation, 0, 2, ProgramSettings.ProgramName, ProgramSettings.VersionInfo, ProgramSettings.InstructionsLink);

            //Creating and getting necessary directories.
            string RobloxFolderDirectory = string.Empty, RobloxStudioExecutableDirectory = string.Empty, SaveFileDirectory = string.Empty;

            ConsoleOutputProvider.Output(OutputFormatType.DirectorySelection, 0, 1, "Roblox folder", "FolderBrowserDialog");

            using (FolderBrowserDialog OpenRobloxFolderDirectoryDialog = new FolderBrowserDialog()
            {
                ShowNewFolderButton = false,
                AutoUpgradeEnabled = true,
                Description = "Select Roblox Folder",
                UseDescriptionForTitle = true,
                ClientGuid = new Guid(ProgramSettings.FolderBrowserDialogGuid),
            })
            {
                RobloxFolderDirectory = UncategorizedMethodsProvider.ShowDialog(OpenRobloxFolderDirectoryDialog, new ExitProgramException(ExitProgramExceptionFormat.InvalidProvidedDirectory, 0, "Roblox folder"));
            }

            ConsoleOutputProvider.Output(OutputFormatType.String, 0, 2, RobloxFolderDirectory);
            ConsoleOutputProvider.Output(OutputFormatType.DirectorySelection, 0, 1, "Roblox Studio Executable file", "OpenFileDialog");

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
                RobloxStudioExecutableDirectory = UncategorizedMethodsProvider.ShowDialog(OpenRobloxStudioExecutableDirectoryDialog, new ExitProgramException(ExitProgramExceptionFormat.InvalidProvidedDirectory, 0, "Roblox Studio Executable file"));
            }

            ConsoleOutputProvider.Output(OutputFormatType.String, 0, 2, RobloxStudioExecutableDirectory);
            ConsoleOutputProvider.Output(OutputFormatType.DirectorySelection, 0, 1, "save file", "SaveFileDialog");

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
                SaveFileDirectory = UncategorizedMethodsProvider.ShowDialog(SaveFinalResultDirectoryDialog, new ExitProgramException(ExitProgramExceptionFormat.InvalidProvidedDirectory, 0, "save file"));
            }

            ConsoleOutputProvider.Output(OutputFormatType.String, 0, 2, SaveFileDirectory);
            ConsoleOutputProvider.Output(OutputFormatType.String, 0, 2, "Necessary directories found. Starting operations.");

            string PluginSettingsFolderDirectory = DirectoryProvider.GetPluginSettingsFolderDirectory(RobloxFolderDirectory);
            string PluginSettingsFileDirectory = DirectoryProvider.GetPluginSettingsFileDirectory(PluginSettingsFolderDirectory);

            //DateTime to get operations TimeSpawn later.
            OperationsTimeSpanProvider.SetTimer();

            //Storing the current data in settings.json file to restore it later.
            string PreviousPluginSettingsFileData = File.ReadAllText(PluginSettingsFileDirectory);

            //FirstGlobalEntryList EntryListStringBuilder to create a list of globals.
            ListStringBuilder FirstGlobalEntryListStringBuilder = new ListStringBuilder(new StringBuilder("return {\n"), "\t\"{0}\",\n");

            //Iteration counters.
            int IteratedMatchCount = 0;
            int DuplicateMatchCount = 0;

            //Matching all strings in executable file that has 2 or more alphanumerical + underscore characters in them.
            UncategorizedMethodsProvider.CheckMatches(Regex.Matches(File.ReadAllText(RobloxStudioExecutableDirectory), @"[a-zA-Z_][0-9a-zA-Z_]+", RegexOptions.Compiled), ref FirstGlobalEntryListStringBuilder, ref IteratedMatchCount, ref DuplicateMatchCount);

            ConsoleOutputProvider.Output(OutputFormatType.String, 2, 2, "Roblox Studio Executable file alphanumerical character pattern check finished.\nMoving to runtime Luau global enviroment check. Running Roblox Studio Executable file, please do not interfere.");

            //Creating temporary XML place file and plugin file to run an enviroment check.
            string TemporaryPlaceFileDirectory = TemporaryDirectoryProvider.CreateRobloxPlaceXMLFile(ref FirstGlobalEntryListStringBuilder);
            TemporaryDirectoryProvider.CreatePluginFile(Path.Combine(RobloxFolderDirectory, "Plugins"));

            //FinalGlobalEntryList to save the final list of globals.
            string FinalGlobalEntryList = string.Empty;

            //Process to run RobloxStudioBeta executable with given parameters later.
            using (Process RobloxStudioRuntimeLuauGlobalEnviromentCheckProcess = new Process()
            {
                StartInfo = new ProcessStartInfo(RobloxStudioExecutableDirectory, $"-task EditFile -localPlaceFile {TemporaryPlaceFileDirectory}")
            })
            {
                //FileSystemWatcher to track settings.json file changes.
                using (FileSystemWatcher PluginSettingsFolderDirectoryWatcher = new FileSystemWatcher(PluginSettingsFolderDirectory, "settings.json")
                {
                    EnableRaisingEvents = true,
                    NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size,
                    IncludeSubdirectories = false
                })
                {
                    PluginSettingsFolderDirectoryWatcher.Changed += (object SenderSource, FileSystemEventArgs EventArguments) =>
                    {
                        using (FileStream PluginSettingsFileStream = new FileStream(EventArguments.FullPath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                        {
                            using (StreamReader PluginSettingsFileStreamReader = new StreamReader(PluginSettingsFileStream))
                            {
                                try
                                {
                                    Dictionary<string, JsonElement> PluginSettingsFileJsonData = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(PluginSettingsFileStreamReader.ReadToEnd());

                                    if (PluginSettingsFileJsonData.ContainsKey("GlobalList") == true && PluginSettingsFileJsonData.ContainsKey("GlobalListCheckFinished") == true && PluginSettingsFileJsonData["GlobalListCheckFinished"].GetBoolean() == true)
                                    {
                                        PluginSettingsFolderDirectoryWatcher.EnableRaisingEvents = false;
                                        FinalGlobalEntryList = PluginSettingsFileJsonData["GlobalList"].GetString();

                                        try
                                        {
                                            RobloxStudioRuntimeLuauGlobalEnviromentCheckProcess.Kill();
                                        }
                                        catch (Win32Exception)
                                        {
                                            //Apperently calling Process.Kill() on a process you started can throw Win32Exception.
                                        }
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

            File.WriteAllText(SaveFileDirectory, FinalGlobalEntryList);
            File.WriteAllText(PluginSettingsFileDirectory, PreviousPluginSettingsFileData);

            TimeSpan OperationsTimeSpan = OperationsTimeSpanProvider.GetTimeSpan();
            ConsoleOutputProvider.Output(OutputFormatType.OperationsTimeSpan, 0, 2, OperationsTimeSpan.Hours, OperationsTimeSpan.Minutes, OperationsTimeSpan.Seconds, OperationsTimeSpan.Milliseconds);
            ConsoleOutputProvider.Output(OutputFormatType.String, 0, 2, $"List is saved to directory: {SaveFileDirectory}");

            TemporaryDirectoryProvider.ClearDirectories();

            throw new ExitProgramException(ExitProgramExceptionFormat.AwaitExit, 0).ThrowException();
        }
    }
}