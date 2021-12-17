# Roblox Luau Global Variable Fetcher

A barebone implementation of searching ASCII strings in Roblox Studio executable to create a list of possible Luau globals.

# Why does this exist?

Because I needed to get a list of Luau globals every version update to update another repository and doing it by hand was starting to become irritating. So I search the internet and found [matthewdean's implementation](https://github.com/matthewdean/roblox-global-variable-enumerator) on the matter. However, for some unknown reason I couldn't get his code working and since it was last updated back in 2015 I decided to write one for myself. After writing the program, I decided to make the program public.

# How do I use it?

1. Download the executable file named `RobloxLuauGlobalVariableFetcher.exe` and the Roblox XML model file named `RobloxLuauGlobalVariableFetcher.exe` from the releases tab.
2. Run the `RobloxLuauGlobalVariableFetcher.exe` to find the list of possible globals and save it to a file when prompted a `SaveFileDiolog` .
3. Open Roblox Studio and insert the contents of `RobloxLuauGlobalVariableFetcher.rbxmx` to the `ServerScriptService` .
4. Copy the contents of file that stores the list of globals into the `ModuleScript` named `GlobalList` that came with `RobloxLuauGlobalVariableFetcher.rbxmx` file.
5. Copy the code inside `Script` named `GlobalFetcher` that came with `RobloxLuauGlobalVariableFetcher.rbxmx` file and run it in studio command bar. This will output \*almost all of the valid globals in studio output window.

\* Currently the only exceptions to this are `plugin` and `script` globals and *some* data in `AutocompleteMetadata.xml` that might not exist in Roblox Studio executable file yet.

# Can I post issues/pull request?

Yes. Feedback is welcome. You can post your suggestions and questions in issues tab too.

# Link
https://github.com/Mactavsin/Roblox-Studio-Global-Variable-Fetcher
