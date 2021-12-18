local ServerScriptService = game:GetService("ServerScriptService")

local PossibleGlobalArray = require(ServerScriptService.RobloxLuauGlobalVariableFetcher.RobloxLuauGlobalList)
local EnviromentTable = getfenv()

local GlobalListString = "return {\n"

local EntryFormat = "%s%s = true,\n"
local EntryTableStartFormat = "%s%s = {\n"
local EntryTableEndFormat = "%s},\n"

local function CheckTable(SearchTable, EntryDepth)
	for Index, EntryName in ipairs(PossibleGlobalArray) do 
		local Entry = SearchTable[EntryName]

		if Entry ~= nil then 
			if type(Entry) == "table" then 
				GlobalListString ..= string.format(EntryTableStartFormat, string.rep("\t", EntryDepth), EntryName)

				CheckTable(Entry, EntryDepth + 1)

				GlobalListString ..= string.format(EntryTableEndFormat, string.rep("\t", EntryDepth))
			else
				GlobalListString ..= string.format(EntryFormat, string.rep("\t", EntryDepth), EntryName)
			end
		end
	end
end

CheckTable(EnviromentTable, 1)

GlobalListString ..= "}"

local GlobalList = Instance.new("ModuleScript")
GlobalList.Source = GlobalListString
GlobalList.Name = "FinalGlobalList"
GlobalList.Parent = ServerScriptService.RobloxLuauGlobalVariableFetcher