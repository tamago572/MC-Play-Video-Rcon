using MinecraftConnection;

string address = "localhost";
ushort port = 25575;
string pswd = "minecraft";
MinecraftCommands command = new MinecraftCommands(address, port, pswd);

command.DisplayTitle("Hello, World!");

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

