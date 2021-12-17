using System;
using System.IO;
using System.Collections;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Text;

namespace RobloxLuauGlobalVariableFetcher
{
    class Program
    {
        //This attribute is required to use Dialogs functionality.
        [STAThread]
        static void Main(string[] args)
        {
            //Creating constants and variables to be used in the program later.
            const string EntryStartString = "    \"";
            const string EntryEndString = "\",\n";
            ulong IteratedMatchCount = 0;
            ulong DuplicateMatchCount = 0;
            Hashtable GlobalList = new Hashtable();
            StringBuilder GlobalListDataString = new StringBuilder("");
            Regex GlobalRegex = new Regex(@"[a-zA-Z_][0-9a-zA-Z_]+", RegexOptions.Compiled);
            string StudioExecutableDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

            SaveFileDialog SaveResultDialog = new SaveFileDialog();
            SaveResultDialog.Filter = "Lua Code File(*.lua)|*.lua|Luau Code File(*.luau)|*.luau";
            SaveResultDialog.Title = "Save Global List";
            SaveResultDialog.FilterIndex = 1;
            SaveResultDialog.AddExtension = true;
            SaveResultDialog.AutoUpgradeEnabled = true;
            SaveResultDialog.RestoreDirectory = true;
            SaveResultDialog.DefaultExt = "lua";
            SaveResultDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            SaveResultDialog.CheckPathExists = true;

            OpenFileDialog OpenStudioDirectoryDialog = new OpenFileDialog();
            OpenStudioDirectoryDialog.Filter = "Executable File(*.exe)|*.exe";
            OpenStudioDirectoryDialog.Title = "Select Studio Executable";
            OpenStudioDirectoryDialog.FilterIndex = 1;
            OpenStudioDirectoryDialog.AddExtension = false;
            OpenStudioDirectoryDialog.AutoUpgradeEnabled = true;
            OpenStudioDirectoryDialog.RestoreDirectory = true;
            OpenStudioDirectoryDialog.DefaultExt = "exe";
            OpenStudioDirectoryDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            OpenStudioDirectoryDialog.CheckPathExists = true;

            //Checking if provided directy is correct or not.
            if (OpenStudioDirectoryDialog.ShowDialog() == DialogResult.OK)
            {
                StudioExecutableDirectory = OpenStudioDirectoryDialog.FileName;
            }
            else
            {
                Console.WriteLine($"Failed to fetch directory {OpenStudioDirectoryDialog.FileName}, press ANY key to exit.");
                Console.ReadLine();
                Environment.Exit(0);
            }

            Console.WriteLine("Starting Operations.");

            //Matching all strings in executable file that has 2 or more alphanumerical + underscore characters in them.
            MatchCollection PossibleGlobalsCollection = GlobalRegex.Matches(File.ReadAllText(StudioExecutableDirectory));

            Console.WriteLine($"Found {PossibleGlobalsCollection.Count} matches.");

            //Going through the MatchCollection with possible global names.
            foreach (Match GlobalVariable in PossibleGlobalsCollection)
            {
                IteratedMatchCount += 1;

                //Checking if we have gone through the same global name before.
                if (GlobalList.ContainsKey(GlobalVariable.Value) == false)
                {
                    GlobalListDataString.Append(EntryStartString);
                    GlobalListDataString.Append(GlobalVariable.Value);
                    GlobalListDataString.Append(EntryEndString);

                    //Adding the global name to the Hashtable to prevent duplicates in later iterations.
                    GlobalList.Add(GlobalVariable.Value, true);
                }
                else
                {
                    DuplicateMatchCount += 1;
                }

                Console.Write($"\rGone through {IteratedMatchCount} matches.");
            }

            Console.WriteLine($"Finished Operations: Gone through {IteratedMatchCount} matches with {DuplicateMatchCount} duplicates ignored.");

            if (SaveResultDialog.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(SaveResultDialog.FileName, $"return {{\n{GlobalListDataString.ToString()}}}");
                Console.WriteLine($"Data is saved to {SaveResultDialog.FileName}. Press ANY key to exit.");
            }
            else
            {
                Console.WriteLine($"Failed to save data to {SaveResultDialog.FileName}. Press ANY key to exit.");
            }

            Console.ReadLine();
            Environment.Exit(0);
        }
    }
}
