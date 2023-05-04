#!/bin/bash

docker container stop nz-rubbish  >/dev/null 2>&1
docker container rm nz-rubbish >/dev/null 2>&1
docker build -t nz-rubbish -f NZRubbishCollection.Api/Dockerfile .
if [ $? -ne 0 ]; then
    echo "Failed to build docker, exiting"
    exit 1
fi

if [ $# -eq 0 ]; then
  exit 0
fi
read -p "Do you want to publish to $1 on Dockerhub ? (Y/N) " answer
if [ "$answer" == "Y" ] || [ "$answer" == "y" ]; then
    echo Publishing docker image
    docker tag nz-rubbish $1
    docker push $1
else
    # do something else
    echo Exited without pushing 
fi