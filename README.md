# Roblox Studio Global Variable Fetcher
 A barebone implementation of searching ASCII strings in Roblox Studio executable to find possible list of globals.

# Why?
 Because I needed to get a list of globals every version update and doing it by hand was starting to become irritating and [matthewdean's implementation](https://github.com/matthewdean/roblox-global-variable-enumerator) wasn't working for me for some unknown reason.

# How do I use it?
 1. Download the executable file named `RobloxStudioGlobalFetcher.exe` and the Roblox XML model file named `RobloxStudioGlobalFetcher.rbxmx` from the releases tab.
 2. Run the `RobloxStudioGlobalFetcher.exe` to find the list of possible globals and save it to a file when prompted a `SaveFileDiolog`.
 3. Open Roblox Studio and insert the contents of `RobloxStudioGlobalFetcher.rbxmx` to the `ServerScriptService`.
 4. Copy the contents of file that stores the list of globals into the `ModuleScript` named `GlobalList` that came with `RobloxStudioGlobalFetcher.rbxmx` file.
 5. Copy the code inside `Script` named `GlobalFetcher` that came with `RobloxStudioGlobalFetcher.rbxmx` file and run it in studio command bar. This will output \*almost all of the valid globals in studio output window.

 \* Currently the only exceptions to this are `plugin` global and *some* data in `AutocompleteMetadata.xml` that doesn't exist in Roblox Studio executable file yet.

# Can I post issues/pull request?
 Yes. Feedback is welcome. Though I'm not sure how much will I update this repository. You can post suggestions in issues tab too.
