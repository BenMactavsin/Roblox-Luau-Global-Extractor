using System;
using System.Windows.Forms;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections.Generic;

namespace RLGVF.Methods
{
    public enum ExitProgramExceptionFormat: byte
    {
        AwaitExit = 0,
        InvalidProvidedDirectory = 1,
        NonExistentDirectory = 2,
        TooManySimilarFiles = 3,
    }

    public enum OutputFormatType: byte
    {
        ProgramInformation = 0,
        DirectorySelection = 1,
        OperationsTimeSpan = 2,
        String = 3,
        AwaitInput = 4
    }

    public static class ConsoleOutputProvider
    {
        public static void Output(OutputFormatType OutputType, byte PrefixLineTerminatorCount = 0, byte PostfixLineTerminatorCount = 0, params object[] FormatArguments)
        {
            switch (OutputType)
            {
                case OutputFormatType.ProgramInformation:
                    Console.Write($"{new string('\n', PrefixLineTerminatorCount)}{FormatArguments[0]} ({FormatArguments[1]})\nFor instructions on how to use the program, please go to this link:\n{FormatArguments[2]}{new string('\n', PostfixLineTerminatorCount)}");
                    break;

                case OutputFormatType.DirectorySelection:
                    Console.Write($"{new string('\n', PrefixLineTerminatorCount)}Please select directory for the {FormatArguments[0]} on the {FormatArguments[1]} provided to you: {new string('\n', PostfixLineTerminatorCount)}");
                    break;

                case OutputFormatType.OperationsTimeSpan:
                    Console.Write($"{new string('\n', PrefixLineTerminatorCount)}Operations completed: Took {FormatArguments[0]} hours, {FormatArguments[1]} minutes, {FormatArguments[2]} seconds, {FormatArguments[3]} milliseconds.{new string('\n', PostfixLineTerminatorCount)}");
                    break;

                case OutputFormatType.String:
                    Console.Write($"{new string('\n', PrefixLineTerminatorCount)}{string.Concat(FormatArguments)}{new string('\n', PostfixLineTerminatorCount)}");
                    break;

                case OutputFormatType.AwaitInput:
                    Console.Write($"{new string('\n', PrefixLineTerminatorCount)}{string.Concat(FormatArguments)}{new string('\n', PostfixLineTerminatorCount)}");
                    Console.ReadLine();
                    break;

                default:
                    break;
            }
        }
    }

    /// <summary> 
    /// Represents an <see cref="System.Exception"/> exception that is thrown to indicate that program has exited properly.
    /// </summary>
    public class ExitProgramException: Exception
    {
        private ExitProgramExceptionFormat exceptionFormat;
        private int exitCode;
        private string[] formatArguments;

        /// <summary> 
        /// Constructs an <see cref="RLGVF.Methods.ExitProgramException"/> with given <paramref name="ExitCode"/> and <paramref name="ConsoleOutputList"/> parameters.
        /// </summary>
        /// <param name="ExitCode">Represents an <see cref="int"/> value that will be passed to <see cref="System.Environment.Exit(int)"/> method.</param>
        /// <param name="ConsoleOutputList">Represents a variadic list of <see cref="string"/> values that will be outputed to console before exiting the program.</param>
        public ExitProgramException(ExitProgramExceptionFormat ExceptionMessageFormat, int ExitCode = 0, params string[] FormatArguments)
        {
            exceptionFormat = ExceptionMessageFormat;
            exitCode = ExitCode;
            formatArguments = FormatArguments;
        }

        public ExitProgramException ThrowException()
        {
            switch (exceptionFormat)
            {
                case ExitProgramExceptionFormat.AwaitExit:
                    ConsoleOutputProvider.Output(OutputFormatType.AwaitInput, 0, 0, "Press ANY key to exit the program.");
                    break;

                case ExitProgramExceptionFormat.InvalidProvidedDirectory:
                    ConsoleOutputProvider.Output(OutputFormatType.String, 0, 0, $"Failed to read directory for {formatArguments[0]}. ");
                    ConsoleOutputProvider.Output(OutputFormatType.AwaitInput, 0, 0, "Press ANY key to exit the program.");
                    break;

                case ExitProgramExceptionFormat.NonExistentDirectory:
                    ConsoleOutputProvider.Output(OutputFormatType.String, 0, 0, $"Failed to find directory for {formatArguments[0]} in directory {formatArguments[1]}. ");
                    ConsoleOutputProvider.Output(OutputFormatType.AwaitInput, 0, 0, "Press ANY key to exit the program.");
                    break;

                case ExitProgramExceptionFormat.TooManySimilarFiles:
                    ConsoleOutputProvider.Output(OutputFormatType.String, 0, 0, $"Directory {formatArguments[0]} has too many files that follows \"{formatArguments[1]}\" pattern. ");
                    ConsoleOutputProvider.Output(OutputFormatType.AwaitInput, 0, 0, "Press ANY key to exit the program.");
                    break;

                default:
                    break;
            }

            Environment.Exit(exitCode);

            return this;
        }
    }

    /// <summary>
    /// Represents a modified version of <see cref = "System.Text.StringBuilder"/> class that implements an entry-based list-like mutable string.
    /// </summary>
    public class ListStringBuilder: object
    {
        private StringBuilder stringBuilderObject;
        private string entryFormatString = "{0}";
        private string entryPrefixString = string.Empty;
        private string entryPostfixString = string.Empty;

        /// <summary> 
        /// Gets <see cref="System.Text.StringBuilder"/> class that will be used by the <see cref = "RLGVF.Methods.ListStringBuilder"/> class internally.
        /// </summary>
        public StringBuilder StringBuilder
        {
            get => stringBuilderObject;
        }

        /// <summary> 
        /// Gets prefix <see cref="string"/> that <see cref = "RLGVF.Methods.ListStringBuilder"/> class adds to the start of each entry value.
        /// </summary>
        public string EntryPrefix
        {
            get => entryPrefixString;
        }

        /// <summary> 
        /// Gets postfix <see cref="string"/> that <see cref = "RLGVF.Methods.ListStringBuilder"/> class adds to the end of each entry value.
        /// </summary>
        public string EntryPostfix
        {
            get => entryPostfixString;
        }

        /// <summary> 
        /// Gets format <see cref="string"/> that <see cref = "RLGVF.Methods.ListStringBuilder"/> class uses to assign <see cref="RLGVF.Methods.ListStringBuilder.EntryPrefix"/> and <see cref="RLGVF.Methods.ListStringBuilder.EntryPostfix"/> properties.
        /// </summary>
        public string EntryFormat
        {
            get => entryFormatString;
        }

        /// <summary> 
        /// Constructs an <see cref="RLGVF.Methods.ListStringBuilder"/> class with given <paramref name="StringBuilder"/> parameter.
        /// </summary>
        /// <param name="StringBuilder">Represents a <see cref="System.Text.StringBuilder"/> class value that will be used by the <see cref="RLGVF.Methods.ListStringBuilder"/> class internally.</param>
        /// <
        public ListStringBuilder(StringBuilder StringBuilder)
        {
            stringBuilderObject = StringBuilder;
        }

        /// <summary> 
        /// Constructs an <see cref="RLGVF.Methods.ListStringBuilder"/> class with given <paramref name="StringBuilder"/> and <paramref name="EntryFormat"/> parameters.
        /// </summary>
        /// <param name="StringBuilder">Represents a <see cref="System.Text.StringBuilder"/> class value that will be used by the <see cref="RLGVF.Methods.ListStringBuilder"/> class internally.</param>
        /// <param name="EntryFormat">Represents a format <see cref="string"/> value that will be used to assign <see cref="RLGVF.Methods.ListStringBuilder.EntryPrefix"/> and <see cref="RLGVF.Methods.ListStringBuilder.EntryPostfix"/> properties.</param>
        public ListStringBuilder(StringBuilder StringBuilder, string EntryFormat)
        {
            string[] FormatExpressions = Regex.Split(EntryFormat, @"\{0\}", RegexOptions.Compiled);

            stringBuilderObject = StringBuilder;
            entryFormatString = EntryFormat;
            entryPrefixString = FormatExpressions[0];
            entryPostfixString = FormatExpressions[1];
        }

        /// <summary> 
        /// Appends a <see cref="string"/> to the <see cref="RLGVF.Methods.ListStringBuilder.StringBuilder"/>'s mutable <see cref="string"/>, that formats given <paramref name="Entry"/> parameter to <see cref="RLGVF.Methods.ListStringBuilder.EntryFormat"/>.
        /// </summary>
        /// <param name="Entry">Represents a <see cref="string"/> value that will formated into the <see cref="RLGVF.Methods.ListStringBuilder.EntryFormat"/>.</param>
        public ListStringBuilder AddEntry(string Entry)
        {
            StringBuilder.Append(entryPrefixString).Append(Entry).Append(entryPostfixString);

            return this;
        }

        /// <summary> 
        /// Appends given <paramref name="String"/> to the <see cref="RLGVF.Methods.ListStringBuilder.StringBuilder"/>'s mutable <see cref="string"/> while ignoring <see cref="RLGVF.Methods.ListStringBuilder.EntryFormat"/>. .
        /// </summary>
        /// <param name="String">Represents a <see cref="string"/> value that will be added to the end of the <see cref="RLGVF.Methods.ListStringBuilder.StringBuilder"/>'s mutable <see cref="string"/>.</param>
        public ListStringBuilder Add(string String)
        {
            StringBuilder.Append(String);

            return this;
        }

        /// <summary> 
        /// Returns to the <see cref="RLGVF.Methods.ListStringBuilder.StringBuilder"/>'s mutable <see cref="string"/>.
        /// </summary>
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
				<ProtectedString name=""Source""><![CDATA[{ListStringBuilder.Add("}")}]]></ProtectedString>
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

            throw new ExitProgramException(ExitProgramExceptionFormat.TooManySimilarFiles, 0, FolderDirectory, "TemporaryPlugin_XXX.lua").ThrowException();
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
                    string PluginSettingsFolderDirectory = Path.Combine(DirectoryPath, "InstalledPlugins", "0");

                    if (Directory.Exists(PluginSettingsFolderDirectory) == true)
                    {
                        return PluginSettingsFolderDirectory;
                    }
                }
            }

            throw new ExitProgramException(ExitProgramExceptionFormat.NonExistentDirectory, 0, "local plugin settings folder", FolderDirectory).ThrowException();
        }

        //Returns directory for setting.json file.
        public static string GetPluginSettingsFileDirectory(string FolderDirectory)
        {
            string PluginSettingsFileDirectory = Path.Combine(FolderDirectory, "settings.json");

            if (File.Exists(PluginSettingsFileDirectory) == true)
            {
                return PluginSettingsFileDirectory;
            }

            throw new ExitProgramException(ExitProgramExceptionFormat.NonExistentDirectory, 0, "local plugin settings file", FolderDirectory).ThrowException();
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
            ConsoleOutputProvider.Output(OutputFormatType.String, 0, 1, $"Found {MatchCollection.Count} regular expression matches.");

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

                ConsoleOutputProvider.Output(OutputFormatType.String, 0, 0, $"\rIterated through {IteratedMatchCount}/{MatchCollection.Count} matches.");
            }
        }

        //Returns FileDialog path if DialogResult is OK, exists the program if otherwise.
        public static string ShowDialog(FileDialog Dialog, ExitProgramException ExceptionObject)
        {
            if (Dialog.ShowDialog() == DialogResult.OK)
            {
                return Dialog.FileName;
            }
            else
            {
                throw ExceptionObject.ThrowException();
            }
        }

        //Returns FolderBrowserDialog path if DialogResult is OK, exists the program if otherwise.
        public static string ShowDialog(FolderBrowserDialog Dialog, ExitProgramException ExceptionObject)
        {
            if (Dialog.ShowDialog() == DialogResult.OK)
            {
                return Dialog.SelectedPath;
            }
            else
            {
                throw ExceptionObject.ThrowException();
            }
        }
    }
}