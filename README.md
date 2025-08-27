# SrfInfo

**SrfInfo** is a Windows console utility that displays metadata of a **Siebel CRM repository file** (`.srf`).  

The tool allows you to quickly check when, where, and by whom an SRF file was compiled, and whether it was a full or incremental compilation.  

---

## Features

- Detects whether the repository is the result of a **full** or **incremental** compilation (`IsFullCompile`).  
- Displays compilation date (`CompilationDate`).  
- Shows the user who performed the compilation (`CompiledBy`).  
- Shows the computer name where the compilation was executed (`MachineName`).  
- Displays the repository language in Siebel format (three-letter code, e.g., `ENU`) (`Language`).  
- Shows the name of the browser scripts folder (`BsFolder`).  
- For incremental compilations (`IsFullCompile = False`), additional information about the corresponding full compilation is displayed.  

---

## Usage

Standard run format:  

```bash
SrfInfo.exe <path_to_srf_file>
```  
Example:
```bash
SrfInfo.exe C:\Siebel\enu\siebel_sia.srf
```
## Parameters:
+ /fn — text color
+ /bn — background color
+ n — color number (0..15)
+ Example:
```cmd
SrfInfo.exe C:\Siebel\enu\siebel_sia.srf /f7 /b0
```
(white text on black background)
## Output Example:
```cmd
IsFullCompile    False
CompilationDate  07.01.2023 12:07:14
CompiledBy       DEVELOPER1
MachineName      DEV-SBL02
Language         ENU
BsFolder         srf##########_###

----------- First compile: -----------
IsFullCompile    True
CompilationDate  07.01.2023 07:12:26
CompiledBy       SADMIN
MachineName      DEV-SBL01
Language         ENU
BsFolder         srf##########_444
```
## Requirements
- Windows (x64/x86)
- NETFramework, Version=v4.8 

## Dependencies:
This project uses [OpenMCDF](https://github.com/ironfede/openmcdf) to work with the Compound File Binary Format (CFBF) used by Siebel SRF files.

## Acknowledgements:
+ ironfede/openmcdf for the great library to handle the CFBF format.
+ **Kirill Frolov** — for the article Quick GenBScript By fkirill, which inspired research of SRF and browser scripts.

**The file SrfInfo.zip.b64.txt contains a base64 encoded zip archive of the project in case downloading is prohibited**
