@ECHO OFF

SET msbuild="c:\Program Files\Microsoft Visual Studio\2022\Professional\Msbuild\Current\Bin\MSBuild.exe"

%msbuild% "%CD%\Library\Library.sln" /m /t:rebuild /p:Configuration="Release" /p:Platform="Any Cpu"

IF %ERRORLEVEL% NEQ 0 GOTO eof

FOR %%L IN (Mnk.Library.CodePlex Mnk.Library.Common Mnk.Library.ScriptEngine Mnk.Library.WPFControls Mnk.Library.WPFSyntaxHighlighter) DO (
  rmdir nuget\ /s /q
  mkdir nuget\
  IF "%%L" == "Mnk.Library.WPFControls" (
    CALL :copyFile *WpfWinForms
    CALL :copyFile *Localization*
  )
  CALL :copyFile %%L

nuget pack nuspec/%%L.nuspec -BasePath nuget\ -exclude "*.pdb"

rmdir nuget\ /s /q
move *.nupkg ../nuget/
)

GOTO eof

:copyFile
xcopy Library\bin\Release\%1.dll nuget\lib\net48\ /s /q
EXIT /B


:eof