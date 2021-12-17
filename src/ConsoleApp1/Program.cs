using System;
using System.IO;
using System.Collections;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Text;

namespace RobloxStudioGlobalFetcher
{
    class Program
    {
        //This attribute is required to use Clipboard functionality.
        [STAThread]
        static void Main(string[] args)
        {
            //Creating constants and variables to be used in the program later.
            const string EntryStartString = "    \"";
            const string EntryEndString = "\",\n";
            ulong IteratedMatchCount = 0;
            ulong DuplicateMatchCount = 0;
            Hashtable GlobalList = new Hashtable();
            StringBuilder ClipboardString = new StringBuilder("");
            Regex GlobalRegex = new Regex(@"[a-zA-Z_][a-zA-Z_]+", RegexOptions.Compiled);
            string StudioExecutableDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Roblox Studio", "RobloxStudioBeta.exe");

            SaveFileDialog SaveResultDialog = new SaveFileDialog();
            SaveResultDialog.Filter = "Lua Code File(*.lua)|*.lua|Luau Code File(*.luau)|*.luau";
            SaveResultDialog.Title = "Save Global List";
            SaveResultDialog.FilterIndex = 1;
            SaveResultDialog.AddExtension = true;
            SaveResultDialog.AutoUpgradeEnabled = true;
            SaveResultDialog.RestoreDirectory = true;
            SaveResultDialog.DefaultExt = ".lua";
            SaveResultDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            SaveResultDialog.CheckPathExists = true;

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
                    ClipboardString.Append(EntryStartString);
                    ClipboardString.Append(GlobalVariable.Value);
                    ClipboardString.Append(EntryEndString);

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
                File.WriteAllText(SaveResultDialog.FileName, $"return {{\n{ClipboardString.ToString()}}}");
            }

            Console.WriteLine($"Data is saved to {SaveResultDialog.FileName}. Press ANY key to exit.");
            Console.ReadLine();
        }
    }
}
