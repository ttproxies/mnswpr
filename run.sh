if [ ! -d bin ]; then
    dotnet build
fi

./bin/Debug/net10.0/mnswpr
    