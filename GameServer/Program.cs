// See https://aka.ms/new-console-template for more information
using GameServer;

MainServer server = new MainServer();
server.InitConfig("Any", 7777);

server.CreateStartServer();

while(true) { }