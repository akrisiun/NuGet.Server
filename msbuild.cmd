@REM @powershell .\build.ps1

@set msbuild="%ProgramFiles(x86)%\MSBuild\15.0\Bin\MSBuild.exe"
@if not exist %MSBuild% @set msbuild="%ProgramFiles%\MSBuild\14.0\Bin\MSBuild.exe"

@set config=Debug
@REM %msbuild%  /p:Platform="Any CPU" /p:Configuration="%config%" /v:m Nuget.Server.sln

%msbuild%  /p:Platform=anycpu /p:Configuration="%config%" /v:m src\NuGet.Server\NuGet.ServerLib.csproj 


@PAUSE