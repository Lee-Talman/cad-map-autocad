# cad-map-autocad
A tool to capture entity data from DWG files using the AutoCAD .NET API.

# Installation
Using the NETLOAD command, import the `cad-map-autocad.dll` from cad-map-autocad/bin. You'll need a SQL database formatted appropriately to store the information (see below).

## Setting up the SQL Database
Download [SQL Server Express](https://www.microsoft.com/en-us/Download/details.aspx?id=101064) and follow the instructions to set it up using your computer's name as the local host. You can find your computer name on Windows by searching for "System Information" and then looking at "System Name". Then, create a database with default settings and the name `CADDB` to create a database, as well as the remaining `CreateTable_*.sql` to generate the remaining tables inside the database.

## Build and use CAD-MAP in AutoCAD
1. Install the software requirements found at this page: 
[My First AutoCAD Plug-In Overview](https://knowledge.autodesk.com/support/autocad/learn-explore/caas/simplecontent/content/my-first-autocad-plug-overview.html). Specifically, you'll need Visual Studio 2019 **not 2022**. Make sure to scroll all the way to the bottom of the page to find the AutoCAD and ObjectARX .NET wizards.

2. After installing the ObjectARX SDK, and the ObjectARX/.NET wizards, import the following References (Project -> Add Reference...) into Visual Studio: `accoremgd.dll`, `acdbmgd.dll`, and `acmgd.dll` and set their "Copy Local" status to `False`. On a default C drive-based Windows installation, you'll find these in `C:\Autodesk\ObjectARX_for_AutoCAD_2023_Win_64bit_dlm\inc`. 

3. Change the `inputConnectionString` text value in `GUI.xaml` to point at the path of your database if you'd like to make it the same database everytime by default'. If you're using a non-trusted connection, or any other type of connection, reference the [SQL server connection string types at this link](https://www.connectionstrings.com/sql-server/).

4. Finally, build the project by running it using the Build command (Ctrl+Shift+B by default) or running the Debug command (see Troubleshooting below). You'll find the compiled .dll in `cad-map-autocad\bin\debug` as `cad-map-autocad.dll`. Load this in AutoCAD using the `NETLOAD` command, then run CAD-MAP by running the `CADMAP` command.

## Troubleshooting
The first time you clone the file from a repo, like DevOps, you may need to go to the Add Reference... menu and re-select the 3 .dll files listed above. 

Additionally, to Debug the solution, you will need to go to Start -> CAD-MAP AutoCAD Plugin Debug Properties -> Start External Program -> "C:\Program Files\Autodesk\AutoCAD 2023\acad.exe" (or wherever your AutoCAD installation lives).

## Allowing network-loaded files
Depending on your network and local PC security settings, you may the following errors:
1. In Visual Studio, when attempting to build the .dll: `Couldn’t process file mainForm.resx due to its being in the Internet or Restricted zone or having the mark of the web on the file.  Remove the mark of the web if you want to process these files.` *Solution*: open `Main.resx` in file explorer and select the option to "Unblock" where it says "Security: This file came from another computer and might be blocked to help protect this computer." You can find more information about this error [here](https://www.primoprivacy.com/2021/04/16/remove-the-mark-of-the-web-visual-studio-2022-build-error/).

2. In AutoCAD, after calling `NETLOAD`: `System.NotSupportedException: An attempt was made to load an assembly from a network location which would have caused the assembly to be sandboxed in previous versions of the .NET Framework. This release of the .NET Framework does not enable CAS policy by default, so this load may be dangerous. If this load is not intended to sandbox the assembly, please enable the loadFromRemoteSources switch.`: In your AutoCAD installation folder, you can find `acad.exe.config`. Open this in a text editor and add the following flag between the `<runtime>` and `</runtime>` elements: `<loadFromRemoteSources enabled="true"/>`. You can find more information about this error [here](https://through-the-interface.typepad.com/through_the_interface/2011/07/loading-blocked-and-network-hosted-assemblies-with-net-4.html).
