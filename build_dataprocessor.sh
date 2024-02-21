
rm .out/
mkdir -p .out/DataProcessor

dotnet publish XmlProcessor.DataProcessor/XmlProcessor.DataProcessor.csproj -c Release -o .out/DataProcessor

#read -p "exit"