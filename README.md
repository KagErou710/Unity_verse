# Unity_verse

## Features
This is a framework for 3D game.

This framework can
 - Communicate up to 100 players with sharing their position every 150ms per one server.
 - Build some buildings on bases
 - Control NPC


## Built with
- Unity C#(Client)
  -2020.3.22f1 .NET Framework 4.7.1
- Golang (Server)
  -1.19.2
  
  
## Getting start
### Client
- Open Unity from Client folder
- Just run the project

- If you want to run several Clients
 click "ParrelSync" in top menu -> Clones Manager -> Add new clone -> Open in New Editor
 then you can run cloned project. 

### Server
- change PATH to .../Unity_verse/Server
- run ```go run server.go```

### Client tester
- Disable "Player", "ClientManager", "moveController". And enable "ClientTester".
- Default test players are 100.

![image](https://user-images.githubusercontent.com/56529285/201485590-179591ec-0644-4288-88ef-11866233b7b6.png)

### Manage NPCs
- NPC will move when clients receive from server.
 you can set time when you want to make NPC to move through server.

message code for not spawning on spawner and not walking on the road
```
7,7,0/init x position/init y position/init z position:movement:;
```
for example
```
7,7:0/455/1/483:0/436/0.6/483:1/5:0/436/0.6/502:1/5:0/455/1/483:;
```

message code for spawning on spawner and walking on the road
```
7,8,spawner ID/goal ID:0:5;
```
