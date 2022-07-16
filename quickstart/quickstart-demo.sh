#!/bin/bash

docker compose -f dev-docker-compose.yml up -d
# docker pull {PATH_TO_IMAGE_PROCESSOR}:example

echo Sleeping 30s to let mysql fully boot up
sleep 10s
echo 20s
sleep 10s
echo 10s
sleep 10s

docker cp ./create-database.sql mwdb:/tmp/create-database.sql

# Execute the script
echo Executing ./create-database.sql
docker exec mwdb sh -c "mysql -h localhost -u root --password=mysqlpw < /tmp/create-database.sql"

# docker run -v ${PWD}/samples:/img -e ProcessorConfig__DatabaseConnectionString='server=localhost;port=3306;userid=root;password=mysqlpw;database=mystiick' -e ProcessorConfig__SourceFolder='/img/src' -e ProcessorConfig__ArchiveFolder='/img/archive' --network quickstart_default  mystiick/imageprocessor:1.0