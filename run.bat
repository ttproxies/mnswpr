if not exist "bin" (
    dotnet build
)
start bin/Debug/net10.0/mnswpr.exe