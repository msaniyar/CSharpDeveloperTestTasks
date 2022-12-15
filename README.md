# CSharpDeveloperTestTasks
This is .NET 6 solution which consists two projects. WebApi project is executable web api and it uses Core project as dependent project.
Core project is class library which includes services.

Web Api has four endpoints, represents four given tasks.

**1. GetReversedString:** Returns reverse string of given string as query parameter. 
   Example request: 
   
         curl -X 'GET' 'http://localhost:5269/GetReversedString?initialString=abc' -H 'accept: */*'
         
**2. ProcessNumbers:** It is a POST request which takes number of processes as request body. It sends number requests to backend processor and returns response. Process handled by server hub asynchronously and it can be seen in terminal logs. Number has limitiation from 0 to 1000.
                   
        curl -X 'POST' 'http://localhost:5269/ProcessNumbers' -H 'accept: */*' -H 'Content-Type: application/json' -d '{ "number": 10}'
                   
**3. FileHash:** You can get file hash of given file as hex format. File has to be exist in the host machine and whole directory with file name should be set as input. Service doesn't keep it in its memory while calculating. 
                   
        curl -X 'GET' 'http://localhost:5269/FileHash?filePath=%5Csource%5Crepos%5CCSharpDeveloperTestTasks' -H 'accept: */*'
                   
**4. GetPrices:** There isn't any parameter for this endpoint. It gets prices from api.blocktap.io according to given queries. Inside the service it gets by batches but endpoint returns total result.
                   
        curl -X 'GET' 'http://localhost:5269/GetPrices' -H 'accept: */*'
                                                         
## Running Project
WebApi project can be run through Visual Studio with or without Docker. Can be run from cli as well. Go to project folder under src/WebApi then run following command

        dotnet run

## Running Project inside Docker Container
Project has docker-compose.yml file in its root directory. In order to run project in your PC, you have to have Docker Desktop then you need to go to the directory of project and run following command.


        docker-compose up

So docker compose file will handle the rest with already defined environment variables and config. Then swagger page can be seen http:/localhost:90/swagger (if port is used in your computer then please change docker-compose.yml file's port mapping, value on the left should be changed.)

## Config
Two config parameters in appsettings.json is added. ServerHubUrl is for SignalR server hub url and it is running same host with WebApi. Second one is GraphQLUrl and it is for api.blocktap.io url.

## Services
Four endpoint calls four different services which represent four tasks respectively.

**Task Number 1:** StringReverseService

**Task Number 2:** NumberProcessorService

**Task Number 3:** FileHashService

**Task Number 4:** PriceGetterService


## Swagger
Project has a Swagger page as well, swagger page url is **<host:port>/swagger/index.html**
There isn't any authorization in the endpoints since it is not required in the tasks so they can be called directly.


## UnitTests
Solution has also UnitTests project under tests directory. UnitTests can be run via Visual Studio Test Explorer or via following command (command should be project root directory or directory should be added).

        dotnet test

