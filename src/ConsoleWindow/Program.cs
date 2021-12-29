using System;
using System.Windows.Forms;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text.Json;
using RLGVF.Methods;

namespace RLGVF
{
    class Program
    {
        //This attribute is required to use CommonDialogs.
        [STAThread]
        static void Main(string[] Arguments)
        {
            ConsoleOutputProvider.WriteProgramInformation();

            //Creating and getting necessary directories.
            string RobloxFolderDirectory = string.Empty, RobloxStudioExecutableDirectory = string.Empty, SaveFileDirectory = string.Empty;

            ConsoleOutputProvider.WriteSelectDirectory("Roblox folder", "FolderBrowserDialog");

            using (FolderBrowserDialog OpenRobloxFolderDirectoryDialog = new FolderBrowserDialog()
            {
                ShowNewFolderButton = false,
                RootFolder = Environment.SpecialFolder.LocalApplicationData
            })
            {
                RobloxFolderDirectory = UncategorizedMethodsProvider.ShowDialog(OpenRobloxFolderDirectoryDialog, "\nFailed to read directory of the Roblox folder. Press ANY key to exit the program.");
            }

            ConsoleOutputProvider.WriteSelectDirectory(RobloxFolderDirectory, "Roblox Studio Executable file", "OpenFileDialog");

            using (OpenFileDialog OpenRobloxStudioExecutableDirectoryDialog = new OpenFileDialog()
            {
                Filter = "Executable File (*.exe)|*.exe",
                Title = "Select Roblox Studio Executable",
                FilterIndex = 1,
                AddExtension = false,
                AutoUpgradeEnabled = true,
                RestoreDirectory = true,
                DefaultExt = "exe",
                CheckPathExists = true
            })
            {
                RobloxStudioExecutableDirectory = UncategorizedMethodsProvider.ShowDialog(OpenRobloxStudioExecutableDirectoryDialog, "\nFailed to read directory of the Roblox Studio Executable file. Press ANY key to exit the program.");
            }

            ConsoleOutputProvider.WriteSelectDirectory(RobloxStudioExecutableDirectory, "save file", "SaveFileDialog");

            using (SaveFileDialog SaveFinalResultDirectoryDialog = new SaveFileDialog()
            {
                Filter = "Lua Code File (*.lua)|*.lua|Luau Code File (*.luau)|*.luau|Text File (*.txt)|*.txt",
                Title = "Select Global List Save Location",
                FilterIndex = 1,
                AddExtension = true,
                AutoUpgradeEnabled = true,
                RestoreDirectory = true,
                DefaultExt = "lua",
                CheckPathExists = true
            })
            {
                SaveFileDirectory = UncategorizedMethodsProvider.ShowDialog(SaveFinalResultDirectoryDialog, "\nFailed to read directory of the save file. Press ANY key to exit the program.");
            }

            ConsoleOutputProvider.WriteLine(SaveFileDirectory);
            ConsoleOutputProvider.WriteLine("\nNecessary directories found. Starting operations:\n");

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

            ConsoleOutputProvider.WriteLine("\n\nRoblox Studio Executable file alphanumerical character pattern check finished.");
            ConsoleOutputProvider.WriteLine("Moving to runtime Luau global enviroment check. Running Roblox Studio Executable file, please do not interfere.\n");

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
                        }
                    };

                    RobloxStudioRuntimeLuauGlobalEnviromentCheckProcess.Start();
                    RobloxStudioRuntimeLuauGlobalEnviromentCheckProcess.WaitForExit();
                }
            }
            
            File.WriteAllText(SaveFileDirectory, FinalGlobalEntryList);
            File.WriteAllText(PluginSettingsFileDirectory, PreviousPluginSettingsFileData);

            ConsoleOutputProvider.WriteOperationsTimeSpan(OperationsTimeSpanProvider.GetTimeSpan());
            ConsoleOutputProvider.WriteLine($"\nList is saved to directory: {SaveFileDirectory}\n");

            TemporaryDirectoryProvider.ClearDirectories();

            throw new ExitProgramException(0, "Press ANY key to exit the program.");
        }
    }
}
