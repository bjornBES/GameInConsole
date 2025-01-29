dotnet build --os Windows -p:DefineConstants=WINDOWS -o ./Builds/WINDOWS
dotnet build --os Linux -p:DefineConstants=UNIX -o ./Builds/UNIX