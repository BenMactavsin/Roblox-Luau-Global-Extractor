using System;
using System.Windows.Forms;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections.Generic;

namespace RLGVF.Methods
{
    /// <summary> 
    /// Represents a static class for console output functionality that uses <see cref="System.Console"/> class internally.
    /// </summary>
    public static class ConsoleOutputProvider
    {
        /// <summary> 
        /// Provides enumrated values that <see cref="RLGVF.Methods.ConsoleOutputProvider.Output(OutputFormatType, byte, byte, string[])"/> will use to format output messages.
        /// </summary>
        public enum OutputFormatType : byte
        {
            /// <summary> 
            /// Specifies that <see cref="RLGVF.Methods.ConsoleOutputProvider.Output(OutputFormatType, byte, byte, string[])"/> will output program information on console. This option accepts 3 <see href="FormatArguments"/>.
            /// </summary>
            ProgramInformation = 0,
            /// <summary> 
            /// Specifies that <see cref="RLGVF.Methods.ConsoleOutputProvider.Output(OutputFormatType, byte, byte, string[])"/> will output a message on console to specify that what type of file or folder user needs to select on the provided <see cref="System.Windows.Forms.CommonDialog"/>. This option accepts 2 <see href="FormatArguments"/>.
            /// </summary>
            DirectorySelection = 1,
            /// <summary> 
            /// Specifies that <see cref="RLGVF.Methods.ConsoleOutputProvider.Output(OutputFormatType, byte, byte, string[])"/> will output operations execution time on console. This option accepts 4 <see href="FormatArguments"/>.
            /// </summary>
            OperationsTimeSpan = 2,
            /// <summary> 
            /// Specifies that <see cref="RLGVF.Methods.ConsoleOutputProvider.Output(OutputFormatType, byte, byte, string[])"/> will output a custom string on console. This option concats all provided <see href="FormatArguments"/> to a single string.
            /// </summary>
            String = 3,
            /// <summary> 
            /// Specifies that <see cref="RLGVF.Methods.ConsoleOutputProvider.Output(OutputFormatType, byte, byte, string[])"/> will output a custom string on console and wait for an input. This option concats all provided <see href="FormatArguments"/> string.
            /// </summary>
            AwaitInput = 4
        }

        /// <summary> 
        /// Outputs a message with given <paramref name="OutputType"/>, <paramref name="PrefixLineTerminatorCount"/>, <paramref name="PostfixLineTerminatorCount"/> and <paramref name="FormatArguments"/> parameters.
        /// </summary>
        /// <param name="OutputType">Represents an <see cref="RLGVF.Methods.ConsoleOutputProvider.OutputFormatType"/> value that will be used to determine the output message format.</param>
        /// <param name="PrefixLineTerminatorCount">Represents a <see cref="byte"/> value that specifies the amount of newline characters that will be added before the string.</param>
        /// <param name="PostfixLineTerminatorCount">Represents a <see cref="byte"/> value that specifies the amount of newline characters that will be added after the string.</param>
        /// <param name="FormatArguments">Represents a variadic list of <see cref="string"/> values that will used depending on provided <paramref name="OutputType"/> parameter.</param>
        public static void Output(OutputFormatType OutputType, byte PrefixLineTerminatorCount = 0, byte PostfixLineTerminatorCount = 0, params string[] FormatArguments)
        {
            switch (OutputType)
            {
                case OutputFormatType.ProgramInformation:
                    Console.Write($"{new string('\n', PrefixLineTerminatorCount)}{FormatArguments[0]} ({FormatArguments[1]})\n\nFor instructions on how to use the program, please go to this link:\n{FormatArguments[2]}{new string('\n', PostfixLineTerminatorCount)}");
                    break;

                case OutputFormatType.DirectorySelection:
                    Console.Write($"{new string('\n', PrefixLineTerminatorCount)}Please select directory for the {FormatArguments[0]} on the {FormatArguments[1]} provided to you: {new string('\n', PostfixLineTerminatorCount)}");
                    break;

                case OutputFormatType.OperationsTimeSpan:
                    Console.Write($"{new string('\n', PrefixLineTerminatorCount)}Operations completed: Took {FormatArguments[0]} hour{(FormatArguments[0] == "1" ? "" : "s")}, {FormatArguments[1]} minute{(FormatArguments[1] == "1" ? "" : "s")}, {FormatArguments[2]} second{(FormatArguments[2] == "1" ? "" : "s")}, {FormatArguments[3]} millisecond{(FormatArguments[3] == "1" ? "" : "s")}.{new string('\n', PostfixLineTerminatorCount)}");
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
    /// Represents an <see cref="RLGVF.Methods.ExitProgramException"/> that is thrown to indicate that program has exited properly.
    /// </summary>
    public class ExitProgramException: Exception
    {
        private ExitProgramExceptionFormat exceptionFormat;
        private int exitCode;
        private string[] formatArguments;

        /// <summary> 
        /// Provides enumrated values that <see cref="RLGVF.Methods.ExitProgramException.ThrowException"/> will use to format exit message on console.
        /// </summary>
        public enum ExitProgramExceptionFormat : byte
        {
            /// <summary> 
            /// Specifies that <see cref="RLGVF.Methods.ExitProgramException.ThrowException"/> won't output any extra message on console. This option accepts 0 <see href="FormatArguments"/>.
            /// </summary>
            AwaitExit = 0,
            /// <summary> 
            /// Specifies that <see cref="RLGVF.Methods.ExitProgramException.ThrowException"/> will output a message to specify that provided directory on <see cref="System.Windows.Forms.CommonDialog"/> is not valid. This option accepts 1 <see href="FormatArguments"/>.
            /// </summary>
            InvalidProvidedDirectory = 1,
            /// <summary> 
            /// Specifies that <see cref="RLGVF.Methods.ExitProgramException.ThrowException"/> will output a message to specify that provided directory does not exist. This option accepts 2 <see href="FormatArguments"/>.
            /// </summary>
            NonExistentDirectory = 2,
            /// <summary> 
            /// Specifies that <see cref="RLGVF.Methods.ExitProgramException.ThrowException"/> will output a message to specify that there are too many files that follow a specific file name pattern in the provided directory. This option accepts 2 <see href="FormatArguments"/>.
            /// </summary>
            TooManySimilarFiles = 3,
            /// <summary> 
            /// Specifies that <see cref="RLGVF.Methods.ExitProgramException.ThrowException"/> will output a message to specify that program got an unexpected exception. This option accepts 1 <see href="FormatArguments"/>.
            /// </summary>
            UnexpectedException = 4
        }

        /// <summary> 
        /// Constructs an <see cref="RLGVF.Methods.ExitProgramException"/> with given <paramref name="ExceptionMessageFormat"/>, <paramref name="ExitCode"/> and <paramref name="FormatArguments"/> parameters.
        /// </summary>
        /// <param name="ExceptionMessageFormat">Represents an <see cref="RLGVF.Methods.ExitProgramException.ExitProgramExceptionFormat"/> value that will be used to determine the exit message format.</param>
        /// <param name="ExitCode">Represents an <see cref="int"/> value that will be passed to <see cref="System.Environment.Exit(int)"/> method when <see cref="RLGVF.Methods.ExitProgramException.ThrowException"/> method is called.</param>
        /// <param name="FormatArguments">Represents a variadic list of <see cref="string"/> values that will be used while formatting the exit message.</param>
        public ExitProgramException(ExitProgramExceptionFormat ExceptionMessageFormat, int ExitCode = 0, params string[] FormatArguments)
        {
            exceptionFormat = ExceptionMessageFormat;
            exitCode = ExitCode;
            formatArguments = FormatArguments;
        }

        /// <summary> 
        /// Exits the program by calling <see cref="System.Environment.Exit(int)"/> method internally after waiting for input on console.
        /// </summary>
        public ExitProgramException ThrowException()
        {
            switch (exceptionFormat)
            {
                case ExitProgramExceptionFormat.AwaitExit:
                    break;

                case ExitProgramExceptionFormat.InvalidProvidedDirectory:
                    ConsoleOutputProvider.Output(ConsoleOutputProvider.OutputFormatType.String, 0, 0, $"Failed to read directory for {formatArguments[0]}. ");
                    break;

                case ExitProgramExceptionFormat.NonExistentDirectory:
                    ConsoleOutputProvider.Output(ConsoleOutputProvider.OutputFormatType.String, 0, 0, $"Failed to find directory for {formatArguments[0]} in directory {formatArguments[1]}. ");
                    break;

                case ExitProgramExceptionFormat.TooManySimilarFiles:
                    ConsoleOutputProvider.Output(ConsoleOutputProvider.OutputFormatType.String, 0, 0, $"Directory {formatArguments[0]} has too many files that follows \"{formatArguments[1]}\" pattern. ");
                    break;

                case ExitProgramExceptionFormat.UnexpectedException:
                    ConsoleOutputProvider.Output(ConsoleOutputProvider.OutputFormatType.String, 0, 2, $"Program exited unexpectedly with error: {formatArguments[0]} ");
                    break;

                default:
                    break;
            }

            ConsoleOutputProvider.Output(ConsoleOutputProvider.OutputFormatType.AwaitInput, 0, 0, "Press ANY key to exit the program.");
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
            stringBuilderObject.Append(entryPrefixString).Append(Entry).Append(entryPostfixString);

            return this;
        }

        /// <summary> 
        /// Appends given <paramref name="String"/> to the <see cref="RLGVF.Methods.ListStringBuilder.StringBuilder"/>'s mutable <see cref="string"/> while ignoring <see cref="RLGVF.Methods.ListStringBuilder.EntryFormat"/>.
        /// </summary>
        /// <param name="String">Represents a <see cref="string"/> value that will be added to the end of the <see cref="RLGVF.Methods.ListStringBuilder.StringBuilder"/>'s mutable <see cref="string"/>.</param>
        public ListStringBuilder Add(string String)
        {
            stringBuilderObject.Append(String);

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

    /// <summary> 
    /// Represents a static class for providing and creating temporary directories.
    /// </summary>
    public static class TemporaryDirectoryProvider
    {
        private static List<string> TemporaryDirectoryList = new List<string>();

        /// <summary> 
        /// Creates a temporary Roblox Place XML file (.rbxlx) by calling <see cref="System.IO.Path.GetTempFileName"/> method internally and formatting <paramref name="ListStringBuilder"/>'s mutable <see cref="string"/> to the file's contents.
        /// </summary>
        /// <param name="ListStringBuilder">Represents a <see cref="RLGVF.Methods.ListStringBuilder"/> value that will be turned into a <see cref="string"/> and formatted into file's contents.</param>
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

        /// <summary> 
        /// Creates a temporary plugin file at provided <paramref name="FolderDirectory"/>.
        /// </summary>
        /// <param name="FolderDirectory">Represents a <see cref="string"/> value that points to a directory on the disk for temporary plugin file to be created in.</param>
        /// <exception cref="RLGVF.Methods.ExitProgramException"></exception>
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

            throw new ExitProgramException(ExitProgramException.ExitProgramExceptionFormat.TooManySimilarFiles, 0, FolderDirectory, "TemporaryPlugin_XXX.lua").ThrowException();
        }

        /// <summary> 
        /// Deletes all temporary files created by one of the methods in static <see cref="RLGVF.Methods.TemporaryDirectoryProvider"/> class.
        /// </summary>
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

    /// <summary> 
    /// Represents a static class for providing specific directories.
    /// </summary>
    public static class DirectoryProvider
    {
        /// <summary> 
        /// Provides enumrated values that <see cref="RLGVF.Methods.DirectoryProvider.GetDirectory(DirectoryType, string)"/> will use to return a specific directory.
        /// </summary>
        public enum DirectoryType: byte
        {
            /// <summary> 
            /// Specifies that <see cref="RLGVF.Methods.DirectoryProvider.GetDirectory(DirectoryType, string)"/> will return location of the plugin settings folder from provided <see href="FolderDirectory"/>.
            /// </summary>
            PluginSettingsFolder = 0,
            /// <summary> 
            /// Specifies that <see cref="RLGVF.Methods.DirectoryProvider.GetDirectory(DirectoryType, string)"/> will return location of the plugin settings file from provided <see href="FolderDirectory"/>.
            /// </summary>
            PluginSettingsFile = 1,
            /// <summary> 
            /// Specifies that <see cref="RLGVF.Methods.DirectoryProvider.GetDirectory(DirectoryType, string)"/> will return location of the local plugin folder from provided <see href="FolderDirectory"/>.
            /// </summary>
            LocalPluginsFolder = 2
        }

        /// <summary> 
        /// Returns a directory with requested <paramref name="RequiredDirectoryType"/> at the given <paramref name="FolderDirectory"/>.
        /// </summary>
        /// <param name="RequiredDirectoryType">Represents a <see cref="RLGVF.Methods.DirectoryProvider.DirectoryType"/> value that method will use to get a specific required directory.</param>
        /// <param name="FolderDirectory">Represents a <see cref="string"/> value that points to a directory that method will search the required directory in.</param>
        /// <exception cref="RLGVF.Methods.ExitProgramException"></exception>
        public static string GetDirectory(DirectoryType RequiredDirectoryType, string FolderDirectory)
        {
            string ErrorMessage = string.Empty;

            switch (RequiredDirectoryType)
            {
                case DirectoryType.PluginSettingsFolder:
                    ErrorMessage = "local plugin settings folder";

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

                    break;

                case DirectoryType.PluginSettingsFile:
                    ErrorMessage = "local plugin settings file";
                    string PluginSettingsFileDirectory = Path.Combine(FolderDirectory, "settings.json");

                    if (File.Exists(PluginSettingsFileDirectory) == true)
                    {
                        return PluginSettingsFileDirectory;
                    }

                    break;

                case DirectoryType.LocalPluginsFolder:
                    ErrorMessage = "local plugins folder";
                    string LocalPluginsFolderDirectory = Path.Combine(FolderDirectory, "Plugins");

                    if (Directory.Exists(LocalPluginsFolderDirectory) == true)
                    {
                        return LocalPluginsFolderDirectory;
                    }

                    break;
            }

            throw new ExitProgramException(ExitProgramException.ExitProgramExceptionFormat.NonExistentDirectory, 0, ErrorMessage, FolderDirectory).ThrowException();
        }
    }

    /// <summary> 
    /// Represents a static class for getting <see cref="System.TimeSpan"/> between 2 <see cref="System.DateTime"/> structs.
    /// </summary>
    public static class TimeSpanProvider
    {
        private static DateTime InternalDateTime = DateTime.Now;

        /// <summary> 
        /// Sets the internal <see cref="System.DateTime"/> value to the current <see cref="DateTime"/>.
        /// </summary>
        public static void SetDateTime()
        {
            InternalDateTime = DateTime.Now;
        }

        /// <summary> 
        /// Returns a <see cref="System.TimeSpan"/> by subtracting current <see cref="DateTime"/> value from internally saved <see cref="DateTime"/> that was created with <see cref="RLGVF.Methods.TimeSpanProvider.SetDateTime"/>.
        /// </summary>
        public static TimeSpan GetTimeSpan()
        {
            return DateTime.Now - InternalDateTime;
        }
    }

    /// <summary> 
    /// Represents a static class for storing methods that does not belong to a specific directory.
    /// </summary>
    public static class UncategorizedMethodsProvider
    {
        /// <summary> 
        /// Goes through all the <see cref="System.Text.RegularExpressions.Match"/> objects in provided <paramref name="MatchCollection"/> and appends <paramref name="ListStringBuilder"/>'s mutable <see cref="string"/> by calling <see cref="RLGVF.Methods.ListStringBuilder.AddEntry(string)"/> after each successful iteration.
        /// </summary>
        /// <param name="MatchCollection">Represents a <see cref="System.Text.RegularExpressions.MatchCollection"/> value that will be iterated through.</param>
        /// <param name="ListStringBuilder">Represents a <see cref="RLGVF.Methods.ListStringBuilder"/> value that will be called <see cref="RLGVF.Methods.ListStringBuilder.AddEntry(string)"/> method on after every successful iteration.</param>
        /// <param name="IteratedMatchCount">Represents a <see cref="int"/> value that will be incremented after every iteration.</param>
        /// <param name="DuplicateMatchCount">Represents a <see cref="int"/> value that will be incremented after every failed iteration.</param>
        public static void CheckMatches(MatchCollection MatchCollection, ref ListStringBuilder ListStringBuilder, ref int IteratedMatchCount, ref int DuplicateMatchCount)
        {
            {
                ConsoleOutputProvider.Output(ConsoleOutputProvider.OutputFormatType.String, 0, 1, $"Found {MatchCollection.Count} regular expression matches.");
            }

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

                {
                    ConsoleOutputProvider.Output(ConsoleOutputProvider.OutputFormatType.String, 0, 0, $"\rIterated through {IteratedMatchCount}/{MatchCollection.Count} matches.");
                }
            }

            GlobalDictionary.Clear();
        }

        /// <summary> 
        /// Returns the provided directory from <see cref="System.Windows.Forms.CommonDialog.ShowDialog"/> if <see cref="System.Windows.Forms.DialogResult"/> equals <see cref="System.Windows.Forms.DialogResult.OK"/>.
        /// </summary>
        /// <param name="Dialog">Represents a <see cref="System.Windows.Forms.FileDialog"/> value that will be shown to user.</param>
        /// <param name="ExceptionObject">Represents a <see cref="RLGVF.Methods.ExitProgramException"/> value that will be thrown if <see cref="System.Windows.Forms.DialogResult"/> does not equal to <see cref="System.Windows.Forms.DialogResult.OK"/>.</param>
        /// <exception cref="RLGVF.Methods.ExitProgramException"></exception>
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

        /// <summary> 
        /// Returns the provided directory from <see cref="System.Windows.Forms.CommonDialog.ShowDialog"/> if <see cref="System.Windows.Forms.DialogResult"/> equals <see cref="System.Windows.Forms.DialogResult.OK"/>.
        /// </summary>
        /// <param name="Dialog">Represents a <see cref="System.Windows.Forms.FolderBrowserDialog"/> value that will be shown to user.</param>
        /// <param name="ExceptionObject">Represents a <see cref="RLGVF.Methods.ExitProgramException"/> value that will be thrown if <see cref="System.Windows.Forms.DialogResult"/> does not equal to <see cref="System.Windows.Forms.DialogResult.OK"/>.</param>
        /// <exception cref="RLGVF.Methods.ExitProgramException"></exception>
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
