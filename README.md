# WebScreenShotApp
Bulk Screen Shotter, captures screen of given website links.


## Installation

If you don't want to pollute your system, install [docker](https://docs.docker.com/install/), locate to the folder which contains `docker-compose.yml` and hit the command below: 

```bash
docker-compose up --build 
```

If you want to try on your system; you need to download&install:
 - [.NET Core 3.1](https://dotnet.microsoft.com/download/dotnet-core/3.1)
 - [Mongo DB](https://www.mongodb.com/download-center)


## Usage

Basically, there is a console application to communicate with service workers. There are 4 different choices to capture web screens with saving into database and to download captured images. You have to give two arguments as input: operation and input option.

 Capture screens from url and Save:
```bash
 1 "http://www.google.com;http://www.microsoft.com" 
  ```
 Capture screens from file which contains comma separated links and also named with `Input*.txt` pattern:
```bash
 2 "C:/sometempfolder/Input1.txt" 
```
Retrieve captures by url and Download:
```bash
 3 "http://www.google.com;http://www.microsoft.com" 
```
Retrieve captures by file which contains comma separated links and also named with `Output*.txt` pattern:
```bash
 4 "C:/sometempfolder/Output1.txt" 
```

- Retrieving can be made by giving two different inputs with comma separated values: date or site url.

  >http://www.google.com;http://www.microsoft.com
  
  >2005-05-05 22:12-20015-05-05 23:12;2020-01-05 22:12-2020-02-05 23:12

##### Running from Docker 
After mapping local folders to virtual images, you can execute commands. 
  In the `app.config` file of console application and in the appsettings.json file of Service application; we need to map InputFilesFolder and OutputFilesFolder parameters.
```bash
 docker run -i screenshotconsole 2 "C:\ScreenShot\Input1.txt"
```

## Scalability
Since `ScreenShotService` is multithreaded service application, you can use `appsettings.json` file to set thread count for application-wide utilizations. For storage-wide scaling a supersonic document oriented NoSQL database helps us : [mongoDB](https://www.mongodb.com/what-is-mongodb). 
With sharding(horizontal partitioning) data into different servers, mongoDB also comes with high performance additional to scalability. 


## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

