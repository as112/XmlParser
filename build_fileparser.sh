
rm .out/
mkdir -p .out/FileParser

dotnet publish XmlProcessor.FileParser/XmlProcessor.FileParser.csproj -c Release -o .out/FileParser

#read -p "exit"