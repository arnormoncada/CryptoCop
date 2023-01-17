Final project for my web API course in Reykjavík University.
Consists of three microservices.
Received full marks.

Cryptocop api
To run the application use the following commands:
    -dotnet restore
    -dotnet build
    -dotnet run in the /Cryptocop.Software.API folder
This application runs on port 5000 (http) and 5001 (https). At least in my macOS environment.

Cryptocop emails
To run the application use the following commands:
    - pip3 install -r requirements.txt
    - python3 email_service.py

Cryptocop payments
To run the application use the following commands:
    - pip3 install -r requirements.txt
    - python3 payment_service.py


Docker
To run the entire microservice architecture use the following commands
in the /Cryptocop folder where the docker-compose.yml file is located:
    - dotnet restore
    - docker-compose build 
    - docker-compose up -d

Both the python microservice should retry running until the rabbitmq container is up and running fully.
The Cryptocop api only runs on port 5000 (http). I could not get the https to work due to some issues with the certificates.
See lines 16-19 in the program.cs 

The images are also available on docker hub:
Use "docker compose pull" (in the directory of the docker-compose-yml) to pull the images from docker hub.