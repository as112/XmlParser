Развертывание сисетмы:

1. На хосте установить .NET 8 SDK

2. На хосте установить GIT

3. Выполнить команду git clone https://github.com/as112/XmlParser.git

4. Перейти в директорию .out/FileParser (.out/DataProcessor) и отредектировать файл appsettings.json, заполнить
"rabbitmq_host" или "rabbitmq_connection_string" - для подключения к брокеру rabbitmq, 
"rabbitmq_queue_name" - название очереди,
"DefaultConnection" - данные для подключения к базе данных (для DataProcessor)
"xmlDirecrory" - папка с xml файлами (для FileParser)

5. Инициализировать базу данных командой

CREATE TABLE "Data" (
    "ModuleCategoryID" TEXT NOT NULL CONSTRAINT "PK_Data" PRIMARY KEY,
    "ModuleState" TEXT NOT NULL
)

6. Запустить скрипты run_dataprocessor.sh и run_fileparser.sh на соответствующих хостах
