using System;
using System.Windows.Forms;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections.Generic;

namespace RLGVF.Methods
{
    //Exception class to indicate that program operations are finished.
    public class ExitProgramException: Exception
    {
        public ExitProgramException(short ExitCode = 0, params string[] ConsoleOutputList)
        {
            foreach (string ConsoleOutput in ConsoleOutputList)
            {
                Console.WriteLine(ConsoleOutput);
            }

            Console.ReadLine();
            Environment.Exit(ExitCode);
        }
    }

    //A modified version of StringBuilder class.
    public class ListStringBuilder: object
    {
        private StringBuilder stringBuilderObject; //Stores the StringBuilder class that ListStringBuilder class will use.
        private string entryFormatString = "{0}"; //Stores the format string that will be applied to passed entries before being passed on to stringBuilderObject.
        private string entryPrefixString = string.Empty; //Stores the prefix string that will be applied in entryFormatString.
        private string entryPostfixString = string.Empty; //Stores the postfix string that will be applied in entryFormatString.

        public StringBuilder StringBuilder
        {
            get
            {
                return stringBuilderObject;
            }
            set {}
        }

        public string EntryPrefix
        {
            get
            {
                return entryPrefixString;
            }
            set {}
        }

        public string EntryPostfix
        {
            get
            {
                return entryPostfixString;
            }
            set {}
        }

        public string EntryFormat
        {
            get
            {
                return entryFormatString;
            }
            set {}
        }

        //Creates ListStringBuilder using StringBuilder provided.
        public ListStringBuilder(StringBuilder StringBuilder)
        {
            stringBuilderObject = StringBuilder;
        }

        //Creates ListStringBuilder using StringBuilder provided and splits EntryFormat string into Prefix and Postfix sections.
        public ListStringBuilder(StringBuilder StringBuilder, string EntryFormat)
        {
            string[] FormatExpressions = Regex.Split(EntryFormat, @"\{0\}", RegexOptions.Compiled);  

            stringBuilderObject = StringBuilder;
            entryFormatString = EntryFormat;
            entryPrefixString = FormatExpressions[0];
            entryPostfixString = FormatExpressions[1];
        }

        //Adds passed Entry to the end of the StringBuilder string.
        public ListStringBuilder AddEntry(string Entry)
        {
            StringBuilder.Append(entryPrefixString).Append(Entry).Append(entryPostfixString);

            return this;
        }

        //Adds passed String to the end of the StringBuilder string.
        public ListStringBuilder Append(string String)
        {
            StringBuilder.Append(String);

            return this;
        }

        //Return list as a string.
        public override string ToString()
        {
            return StringBuilder.ToString();
        }
    }

    //Stores methods for providing and cleaning temporary directories.
    public static class TemporaryDirectoryProvider
    {
        //Stores a generic list of temporary directories.
        private static List<string> TemporaryDirectoryList = new List<string>();

        //Creates a temporary Roblox Place XML file.
        public static string CreateRobloxPlaceXMLFile(ref ListStringBuilder ListStringBuilder)
        {
            string TemporaryDirectory = Path.GetTempFileName();

            File.WriteAllText(TemporaryDirectory, $@"<roblox xmlns:xmime=""http://www.w3.org/2005/05/xmlmime"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:noNamespaceSchemaLocation=""http://www.roblox.com/roblox.xsd"" version=""4"">
	<External>null</External>
	<External>nil</External>
	<Item class=""ServerScriptService"" referent=""RBXE58624FCF2064CA98A0372920240E2CB"">
		<Properties>
			<BinaryString name=""AttributesSerialize""></BinaryString>
			<bool name=""LoadStringEnabled"">false</bool>
			<string name=""Name"">ServerScriptService</string>
			<int64 name=""SourceAssetId"">-1</int64>
			<BinaryString name=""Tags""></BinaryString>
			<UniqueId name=""UniqueId"">29ce6252600f462801d8c6460001a3d0</UniqueId>
		</Properties>
		<Item class=""ModuleScript"" referent=""RBXE3AC97D461BF457FA60D302A6420744A"">
			<Properties>
				<BinaryString name=""AttributesSerialize"">AQAAABEAAABHbG9iYWxTY2FuRW5hYmxlZAMB</BinaryString>
				<Content name=""LinkedSource""><null></null></Content>
				<string name=""Name"">RobloxLuauGlobalList</string>
				<string name=""ScriptGuid"">{{19B8D159-572F-4338-9397-C51007FFAEB7}}</string>
				<ProtectedString name=""Source""><![CDATA[{ListStringBuilder.Append("}").ToString()}]]></ProtectedString>
				<int64 name=""SourceAssetId"">-1</int64>
				<BinaryString name=""Tags""></BinaryString>
				<UniqueId name=""UniqueId"">29ce6252600f462801d8c64600024b97</UniqueId>
			</Properties>
		</Item>
	</Item>
</roblox>");

            TemporaryDirectoryList.Add(TemporaryDirectory);
            TemporaryDirectoryList.Add(TemporaryDirectory + ".lock");

            return TemporaryDirectory;
        }

        //Creates a temporary plugin file.
        public static string CreatePluginFile(string FolderDirectory)
        {
            byte FileNumber = 0;

            while (FileNumber <= 255)
            {
                string DirectoryCandidate = Path.Combine(FolderDirectory, $"TemporaryPlugin_{FileNumber}.lua");

                if (File.Exists(DirectoryCandidate) == false)
                {
                    using (FileStream PluginFileDirectory = File.Create(DirectoryCandidate))
                    {
                        using (StreamWriter PluginFileWriter = new StreamWriter(PluginFileDirectory))
                        {
                            PluginFileWriter.Write(@"local ServerScriptService = game:GetService(""ServerScriptService"")
local RobloxLuauGlobalList = ServerScriptService.RobloxLuauGlobalList

local PossibleGlobalArray = require(RobloxLuauGlobalList)
local EnviromentTable = getfenv()

local GlobalListString = ""return {\n""

local EntryFormat = ""%s%s = true,\n""
local TableEntryStartFormat = ""%s%s = {\n""
local TableEntryEndFormat = ""%s},\n""

local function CheckTable(SearchTable, EntryDepth)
	for Index, EntryName in ipairs(PossibleGlobalArray) do 
		local Entry = SearchTable[EntryName]

		if Entry ~= nil then
			if type(Entry) == ""table"" and next(Entry) ~= nil and not (EntryName == ""_G"" or EntryName == ""shared"") then
				GlobalListString ..= string.format(TableEntryStartFormat, string.rep(""\t"", EntryDepth), EntryName) 
				
				CheckTable(Entry, EntryDepth + 1)
				
				GlobalListString ..= string.format(TableEntryEndFormat, string.rep(""\t"", EntryDepth))
			else
				GlobalListString ..= string.format(EntryFormat, string.rep(""\t"", EntryDepth), EntryName)
			end
		end
	end
end

CheckTable(EnviromentTable, 1)

GlobalListString ..= ""}""

plugin:SetSetting(""GlobalList"", GlobalListString)
plugin:SetSetting(""GlobalListCheckFinished"", true)");
                        }
                    }

                    TemporaryDirectoryList.Add(DirectoryCandidate);

                    return DirectoryCandidate;
                }
                else
                {
                    FileNumber++;
                }
            }

            throw new ExitProgramException(0, $"{FolderDirectory} directory has too many files that follows \"TemporaryPlugin_X.lua\" pattern. Press ANY key to exit the program.");
        }

        //Clears TemporaryDirectoryList and directories in TemporaryDirectoryList.
        public static void ClearDirectories()
        {
            foreach (string TemporaryDirectory in TemporaryDirectoryList)
            {
                if (File.Exists(TemporaryDirectory) == true)
                {
                    File.Delete(TemporaryDirectory);
                }
                else if (Directory.Exists(TemporaryDirectory) == true)
                {
                    Directory.Delete(TemporaryDirectory);
                }
            }

            TemporaryDirectoryList.Clear();
        }
    }

    //Stores methods for providing Directories.
    public static class DirectoryProvider
    {
        //Returns folder directory that setting.json file is stored in.
        public static string GetPluginSettingsFolderDirectory(string FolderDirectory)
        {
            foreach (string DirectoryPath in Directory.GetDirectories(FolderDirectory))
            {
                if (Regex.Match(new DirectoryInfo(DirectoryPath).Name, "^[1-9][0-9]*$").Success == true)
                {
                    return Path.Combine(DirectoryPath, "InstalledPlugins", "0");
                }
            }

            throw new ExitProgramException(0, $"Failed to find local plugin folder in directory {FolderDirectory}. Press ANY key to exit the program.");
        }

        //Returns directory for setting.json file.
        public static string GetPluginSettingsFileDirectory(string FolderDirectory)
        {
            string PluginSettingsFileDirectory = Path.Combine(FolderDirectory, "settings.json");

            if (File.Exists(PluginSettingsFileDirectory) == true)
            {
                return PluginSettingsFileDirectory;
            }

            throw new ExitProgramException(0, $"Failed to find local plugin settings file in directory {FolderDirectory}. Press ANY key to exit the program.");
        }
    }

    //Stores methods for outputing text to console with preset string formats.
    public static class ConsoleOutputProvider
    {
        //Program related information
        private const string ProgramName = "Roblox Luau Global Variable Fetcher";
        private const string VersionInfo = "v.1.3.Beta";
        private const string InstructionsLink = "Instructions link does not exist for beta version.";

        //Outputs program information on console.
        public static void WriteProgramInformation()
        {
            Console.WriteLine($"{ProgramName} ({VersionInfo})\n");
            Console.WriteLine($"For instructions on how to use the program, please go to this link:\n{InstructionsLink}\n");
        }

        //TODO: Find proper explanation to this method.
        public static void WriteSelectDirectory(string DirectoryPurpose, string DialogType)
        {
            Console.Write($"Please select directory of the {DirectoryPurpose} on the {DialogType} provided to you: ");
        }

        //TODO: Find proper explanation to this method.
        public static void WriteSelectDirectory(string Directory, string DirectoryPurpose, string DialogType)
        {
            Console.Write($"{Directory}\n\nPlease select directory of the {DirectoryPurpose} on the {DialogType} provided to you: ");
        }

        //Outputs passed String on console.
        public static void Write(string String)
        {
            Console.Write(String);
        }

        //Outputs passed String with a line terminator at the end on console.
        public static void WriteLine(string String)
        {
            Console.WriteLine(String);
        }

        //Outputs passed TimeSpan in a human readable way on console.
        public static void WriteOperationsTimeSpan(TimeSpan TimeSpan)
        {
            Console.WriteLine($"Operations completed: Took {TimeSpan.Hours} hours, {TimeSpan.Minutes} minutes, {TimeSpan.Seconds} seconds, {TimeSpan.Milliseconds} milliseconds.");
        }
    }

    //Stores methods for getting TimeSpan between 2 DateTimes.
    public static class OperationsTimeSpanProvider
    {
        private static DateTime SetDateTime = DateTime.Now;

        //Sets SetDateTime to current Datetime.
        public static void SetTimer()
        {
            SetDateTime = DateTime.Now;
        }

        //Gets TimeSpan between current Datetime and SetDateTime.
        public static TimeSpan GetTimeSpan()
        {
            return DateTime.Now - SetDateTime;
        }
    }

    //Stores methods that does not belong to a specific category.
    public static class UncategorizedMethodsProvider
    {
        //Checks MatchCollection and modifies EntryListStringBuilder, IterationCount and DuplicateMatchCount parameters in process.
        public static void CheckMatches(MatchCollection MatchCollection, ref ListStringBuilder ListStringBuilder, ref int IteratedMatchCount, ref int DuplicateMatchCount)
        {
            ConsoleOutputProvider.WriteLine($"Found {MatchCollection.Count} regular expression matches.");

            Dictionary<string, bool> GlobalDictionary = new Dictionary<string, bool>();

            foreach (Match GlobalVariable in MatchCollection)
            {
                IteratedMatchCount++;

                //Checking if we have gone through the same global name before.
                if (GlobalDictionary.ContainsKey(GlobalVariable.Value) == false)
                {
                    ListStringBuilder.AddEntry(GlobalVariable.Value);

                    //Adding the global name to the Dictionary to prevent duplicates in later iterations.
                    GlobalDictionary.Add(GlobalVariable.Value, true);
                }
                else
                {
                    DuplicateMatchCount++;
                }

                ConsoleOutputProvider.Write($"\rIterated through {IteratedMatchCount}/{MatchCollection.Count} matches.");
            }
        }

        //Returns FileDialog path if DialogResult is OK, exists the program if otherwise.
        public static string ShowDialog(FileDialog Dialog, string FinalConsoleOutputFormatString, params string[] FormatStringParametersList)
        {
            if (Dialog.ShowDialog() == DialogResult.OK)
            {
                return Dialog.FileName;
            }
            else
            {
                throw new ExitProgramException(0, string.Format(FinalConsoleOutputFormatString, FormatStringParametersList));
            }
        }

        //Returns FolderBrowserDialog path if DialogResult is OK, exists the program if otherwise.
        public static string ShowDialog(FolderBrowserDialog Dialog, string FinalConsoleOutputFormatString, params string[] FormatStringParametersList)
        {
            if (Dialog.ShowDialog() == DialogResult.OK)
            {
                return Dialog.SelectedPath;
            }
            else
            {
                throw new ExitProgramException(0, string.Format(FinalConsoleOutputFormatString, FormatStringParametersList));
            }
        }
    }
}