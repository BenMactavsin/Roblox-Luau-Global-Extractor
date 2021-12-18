using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Text;

namespace RobloxLuauGlobalVariableFetcher
{
    class Program
    {
        static void DisposeDialogs(params CommonDialog[] Dialogs)
        {
            foreach (CommonDialog Dialog in Dialogs)
            {
                Dialog.Dispose();
            }
        }

        static void ExitProgram(short ExitCode = 0)
        {
            Console.ReadLine();
            Environment.Exit(ExitCode);
        }

        //This attribute is required to use FileDialogs.
        [STAThread]
        static void Main(string[] Arguments)
        {
            //Creating constants and variables to be used in the program later.
            //Program information strings tha will be outputted on console.
            const string ProgramName = "Roblox Luau Global Variable Fetcher";
            const string VersionInfo = "v.1.2.Stable";
            const string InstructionsLink = "https://github.com/Mactavsin/Roblox-Luau-Global-Variable-Fetcher/blob/master/README.md#how-do-i-use-it";

            //Variables to be used by StringBuilder later.
            StringBuilder GlobalListDataString = new StringBuilder("return {\n");
            const string EntryStartString = "\t\"";
            const string EntryEndString = "\",\n";

            //Iteration counters.
            ulong IteratedMatchCount = 0;
            ulong DuplicateMatchCount = 0;

            //Dictionary for preventing duplicates in final list.
            Dictionary<string, bool> GlobalDictionary = new Dictionary<string, bool>();

            //Regex and Directory information.
            Regex GlobalRegex = new Regex(@"[a-zA-Z_][0-9a-zA-Z_]+", RegexOptions.Compiled);
            string StudioExecutableDirectory = string.Empty;

            //User facing FileDialogs.
            OpenFileDialog OpenStudioDirectoryDialog = new OpenFileDialog()
            {
                Filter = "Executable File (*.exe)|*.exe",
                Title = "Select Studio Executable",
                FilterIndex = 1,
                AddExtension = false,
                AutoUpgradeEnabled = true,
                RestoreDirectory = true,
                DefaultExt = "exe",
                CheckPathExists = true
            };

            SaveFileDialog SaveResultDialog = new SaveFileDialog()
            {
                Filter = "Lua Code File (*.lua)|*.lua|Luau Code File (*.luau)|*.luau|Text File (*.txt)|*.txt",
                Title = "Save Global List",
                FilterIndex = 1,
                AddExtension = true,
                AutoUpgradeEnabled = true,
                RestoreDirectory = true,
                DefaultExt = "lua",
                CheckPathExists = true
            };

            Console.WriteLine($"{ProgramName} ({VersionInfo})\n");
            Console.WriteLine($"For instructions on how to use the program, please go to this link:\n{InstructionsLink}\n");
            Console.WriteLine("Please select the Roblox Studio executable on the file dialog provided to you:\n");

            //Checking if user provided a directory or not.
            if (OpenStudioDirectoryDialog.ShowDialog() == DialogResult.OK)
            {
                StudioExecutableDirectory = OpenStudioDirectoryDialog.FileName;
            }
            else
            {
                Console.WriteLine($"Failed to read directory {OpenStudioDirectoryDialog.FileName}. Press ANY key to exit the program.");
                DisposeDialogs(OpenStudioDirectoryDialog, SaveResultDialog);
                ExitProgram();
            }

            Console.WriteLine("Directory found. Starting operations.");

            //Start time to output operations time span later.
            DateTime OperationsStartTime = DateTime.Now;

            //Matching all strings in executable file that has 2 or more alphanumerical + underscore characters in them.
            MatchCollection PossibleGlobalsCollection = GlobalRegex.Matches(File.ReadAllText(StudioExecutableDirectory));

            Console.WriteLine($"Found {PossibleGlobalsCollection.Count} matches.");

            //Going through the MatchCollection with possible global names.
            foreach (Match GlobalVariable in PossibleGlobalsCollection)
            {
                IteratedMatchCount += 1;

                //Checking if we have gone through the same global name before.
                if (GlobalDictionary.ContainsKey(GlobalVariable.Value) == false)
                {
                    GlobalListDataString.Append(EntryStartString).Append(GlobalVariable.Value).Append(EntryEndString);

                    //Adding the global name to the Hashtable to prevent duplicates in later iterations.
                    GlobalDictionary.Add(GlobalVariable.Value, true);
                }
                else
                {
                    DuplicateMatchCount += 1;
                }

                Console.Write($"\rIterated through {IteratedMatchCount} matches.");
            }

            TimeSpan OperationsTimeSpan = DateTime.Now.Subtract(OperationsStartTime);

            Console.WriteLine($"\n\nFinished Operations: Gone through {IteratedMatchCount} matches with {DuplicateMatchCount} duplicates ignored.");
            Console.WriteLine($"Performed task in {OperationsTimeSpan.Hours} hours, {OperationsTimeSpan.Minutes} minutes, {OperationsTimeSpan.Seconds} seconds, {OperationsTimeSpan.Milliseconds} milliseconds.\n");
            Console.WriteLine("Please specify where to save the list on the file dialog provided to you:\n");

            //Checking if user provided a directory or not.
            if (SaveResultDialog.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(SaveResultDialog.FileName, GlobalListDataString.Append("}").ToString());
                Console.WriteLine($"Data is saved to directory {SaveResultDialog.FileName}. Press ANY key to exit the program.");
            }
            else
            {
                Console.WriteLine($"Failed to save data to directory {SaveResultDialog.FileName}. Press ANY key to exit program.");
            }

            DisposeDialogs(OpenStudioDirectoryDialog, SaveResultDialog);
            ExitProgram();
        }
    }
}
