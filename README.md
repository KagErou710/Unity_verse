# Unity_verse

##Features
This is a framework for 3D game.

This framework can
 - Communicate up to 100 players with sharing their position every 150ms per one server.
 - Build some buildings on bases


##Built with
- Unity C#(Client)
  -2020.3.22f1 .NET Framework 4.7.1
- Golang (Server)
  -1.19.2
  
  
##Getting start
###Client
- Open Unity from Client folder
- Just run the project

###Server
- change PATH to .../Unity_verse/Server
- run ```go run server.go```

###Client tester
- Disable "Player", "ClientManager", "moveController". And enable "ClientTester".
- Default test players are 100.
![image](https://user-images.githubusercontent.com/56529285/201485590-179591ec-0644-4288-88ef-11866233b7b6.png)
