local GlobalArray = require(game.ServerScriptService.RobloxStudioGlobalFetcher.GlobalList)
local EnviromentTable = getfenv()

local GlobalList = {}

local function CheckTable(SearchTable, GlobalListTable)
	for Index, EntryName in ipairs(GlobalArray) do 
		local Entry = SearchTable[EntryName]

		if Entry ~= nil then 
			if type(Entry) == "table" then 
				GlobalListTable[EntryName] = {}
				
				CheckTable(Entry, GlobalListTable[EntryName])
			else
				GlobalListTable[EntryName] = true
			end
		end
	end
end

CheckTable(EnviromentTable, GlobalList)

print(GlobalList)