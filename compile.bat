
C:/Windows/System32/taskkill.exe /f /im "Spaceflight Simulator.exe"
C:/Windows/System32/taskkill.exe /f /im "UnityCrashHandler64.exe"

dotnet build

@echo off

copy "C:\Users\%USERNAME%\source\repos\WorldBuild\WorldBuild.Mod\bin\Debug\WorldBuild.Mod.dll" "C:\Program Files (x86)\Steam\steamapps\common\Spaceflight Simulator\Spaceflight Simulator Game\Mods\WorldBuild.Mod"
START "async runner" "C:/Program Files (x86)/Steam/steamapps/common/Spaceflight Simulator/Spaceflight Simulator Game/Spaceflight Simulator.exe"
