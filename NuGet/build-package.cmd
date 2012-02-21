C:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild.exe ..\Lucky.AssetManager.sln /p:Configuration=Release
copy ..\Lucky.AssetManager\bin\Release\Lucky.AssetManager.dll lib /Y
nuget pack ..\LuckyAssetManager.nuspec
pause