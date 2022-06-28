#!/bin/bash
docker compose down
docker volume rm quickstart_data

echo 
echo Cleanup complete. You may want to delete the images added through quickstart if desired:
echo -e '\t'adminer:latest
echo -e '\t'mysql:latest
echo -e '\t'mystiick/mystiick-web:latest
echo -e '\t'mystiick/image-processor:example