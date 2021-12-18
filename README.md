# Roblox Luau Global Variable Fetcher

A barebone implementation of searching ASCII strings in Roblox Studio executable to create a list of possible built-in Luau globals.

# Why does this exist?

Recently, I was creating a list of Luau globals every version update manualy to update another repository and doing the entire process by hand by hand was starting to become irritating. 

So I search the internet and found [matthewdean's implementation](https://github.com/matthewdean/roblox-global-variable-enumerator) on the matter. 

However, despite trying quite a bit, I couldn't get his code working and there were a few other thing with his implementation that made me uncomfortable:

1. Source code included an executable file named `Strings.exe` that I couldn't find the actual source code for.

2. It was last updated back in 2015. Which means it was rather unlikely that author was going to update it anytime soon.

So with these facts in mind, I decided to write one for myself. After writing the program, I decided to make the program public so everyone can benefit from it for it's rather niche use case.

# How do I use it?

1. \*Download the executable file named `RobloxLuauGlobalVariableFetcher.exe` and the Roblox XML model file named `RobloxLuauGlobalVariableFetcher.rbxmx` from the releases tab.

![image](https://user-images.githubusercontent.com/69454747/146631408-2708da17-147b-4eab-921e-b4de2fee46a7.png)

![image](https://user-images.githubusercontent.com/69454747/146631418-b3150938-7cb1-453c-aa7d-5eaf39666b93.png)

\* You can also copy the source code from `src` and `Luau-Files` folders and build the executable and Lua files yourself.

![image](https://user-images.githubusercontent.com/69454747/146631446-4bae963e-964d-4498-b85c-9b9f6e97a386.png)

![image](https://user-images.githubusercontent.com/69454747/146631456-9d3efd02-8bbe-4b58-a4e1-24f3fb1bd2ed.png)

2. Run the `RobloxLuauGlobalVariableFetcher.exe`. You will be met with a `OpenFileDialog` to select the Roblox Studio executable file first.

![image](https://user-images.githubusercontent.com/69454747/146631480-054a6b69-5198-4a39-a169-b2f94aa872f4.png)


>To get the original executable file:
>> For people that use unmodified version of Roblox Studio:
>>1. Right click the shortcut on desktop and click properties.
>>
>>![image](https://user-images.githubusercontent.com/69454747/146631492-f617baac-4bc9-4ccc-90f1-a0486c128af2.png)
>>
>>2. Click "OpenFileLocation" button.
>>
>>![image](https://user-images.githubusercontent.com/69454747/146631500-2d349f23-9c3d-4169-8277-19a9c9ada99f.png)
>>
>>3. Right click the address bar and select "Copy Address as Text" option.
>>
>>![image](https://user-images.githubusercontent.com/69454747/146631522-f5e8afb1-023a-4db5-8d99-b7b6c164c3e7.png)
>>
>>4. Right click address bar on the `OpenFileDialog` and press "Edit address".
>>
>>![image](https://user-images.githubusercontent.com/69454747/146631602-7e52f7c8-3f54-4aab-8f9a-bc41c2f3debd.png)
>>
>>5. Paste address to the `OpenFileDialog`'s address bar and press <kbd>Enter</kbd>.
>>
>>![image](https://user-images.githubusercontent.com/69454747/146631673-20a3f4ff-190a-4098-8190-b8fa02b6bd9c.png)
>>
>>6. Select `RobloxStudioBeta.exe` and click "Open" button.
>>
>>![image](https://user-images.githubusercontent.com/69454747/146631725-bc9ae8b0-626b-4f4c-8b85-22735c156517.png)
>>
>
>>For people that use [MaximumADHD's Roblox Studio Mod Manager](https://github.com/MaximumADHD/Roblox-Studio-Mod-Manager):
>>1. Run Roblox Studio Mod Manager executable.
>>
>>![image](https://user-images.githubusercontent.com/69454747/146631749-16dec9f3-f7b0-44d7-ae29-cd0450262669.png)
>>
>>2. Press "Edit Class Icons" button.
>>
>>![image](https://user-images.githubusercontent.com/69454747/146631760-faea7cd6-1472-49a6-94f7-437fef4b36b4.png)
>>
>>3. Press "Open Icon Folder" button.
>>
>>![image](https://user-images.githubusercontent.com/69454747/146631770-40ab470d-8601-42f8-8592-64007a693774.png)
>>
>>4. Go back to `Roblox Studio` folder on the address bar.
>>
>>![image](https://user-images.githubusercontent.com/69454747/146631786-8b034c8e-3afc-4f72-99a3-86da657ad0e3.png)
>>
>>5. Right click the address bar and select "Copy Address as Text" option.
>>
>>![image](https://user-images.githubusercontent.com/69454747/146631808-15ee8745-8d1b-4909-a21d-7b5068fe80f7.png)
>>
>>6. Right click address bar on the `OpenFileDialog` and press "Edit address".
>>
>>![image](https://user-images.githubusercontent.com/69454747/146631602-7e52f7c8-3f54-4aab-8f9a-bc41c2f3debd.png)
>>
>>7. Paste address to the `OpenFileDialog`'s address bar and press <kbd>Enter</kbd>.
>>
>>![image](https://user-images.githubusercontent.com/69454747/146631876-b11fc177-ae83-4e70-8542-d02ebcae9b73.png)
>>
>>8. Select `RobloxStudioBeta.exe` and click "Open" button.
>>
>>![image](https://user-images.githubusercontent.com/69454747/146631908-cc6952bd-02b4-446f-a15b-a02cb226af11.png)
3. Select or name a file to save the list of possible globals when met with a `SaveFileDiolog` and click "Save" button.

![image](https://user-images.githubusercontent.com/69454747/146632007-7f8196cd-4522-4530-a6e7-f007b9a43c16.png)

4. Open a place in Roblox Studio and insert the folder named `RobloxLuauGlobalVariableFetcher` in `RobloxLuauGlobalVariableFetcher.rbxmx` to the `ServerScriptService`.

![image](https://user-images.githubusercontent.com/69454747/146632076-4629b3e0-22b2-4189-8658-6489be5c594d.png)

![image](https://user-images.githubusercontent.com/69454747/146632092-32b12885-5859-40e3-a467-e27840e5d7ba.png)

![image](https://user-images.githubusercontent.com/69454747/146632095-9219fb28-8a98-4c1a-8c8d-198f0ad59855.png)

5. Copy the contents of file that you saved earlier, into the `ModuleScript` named `RobloxLuauGlobalList` that came with `RobloxLuauGlobalVariableFetcher.rbxmx` file, inside the `RobloxLuauGlobalVariableFetcher` folder.

![image](https://user-images.githubusercontent.com/69454747/146632131-02758455-d6d9-4336-9a9c-46c50a7e8cd6.png)

![image](https://user-images.githubusercontent.com/69454747/146632140-9dfdc54f-5dc0-4d69-b53d-b52b6f3c61f2.png)

![image](https://user-images.githubusercontent.com/69454747/146632191-8fc3c87d-8769-417d-952f-580f191547b4.png)


6. Copy the code inside the `Script` named `RobloxLuauGlobalVariableFetcher.server` that came with `RobloxLuauGlobalVariableFetcher.rbxmx` file, inside the `RobloxLuauGlobalVariableFetcher` folder, and run it in studio command bar by pressing <kbd>Enter</kbd> button. This will create a `ModuleScript` named `FinalGlobalList` inside the `RobloxLuauGlobalVariableFetcher` folder that will contain \*almost all of the valid globals inside a table.

![image](https://user-images.githubusercontent.com/69454747/146632237-50b80121-08f3-498e-a129-6244ccbda9fe.png)

![image](https://user-images.githubusercontent.com/69454747/146632251-bd91d44b-9db5-4d7a-8eed-8216964d07b0.png)

![image](https://user-images.githubusercontent.com/69454747/146632292-e3ff30e6-da36-4046-a935-05a1e91ac8c9.png)

\* Currently the only exceptions to this are `plugin` and `script` globals and *some* data in `AutocompleteMetadata.xml` that might not exist in Roblox Studio executable file yet.

# Can I create issues/pull request?

Yes. You can post your bugs, suggestions and questions in issues tab. You can also propose changes by creating a pull request.

# Links
Repository Page: https://github.com/Mactavsin/Roblox-Studio-Global-Variable-Fetcher
