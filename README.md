# DreamNav BotServer
Alternative OTCV8 BotServer

Made by Discord: VivoDibra#1182

Oficial Discord Server: https://discord.gg/RkQ9nyPMBH

Oficial Youtube Channel: https://www.youtube.com/@vivodibra/videos

You Can Download the modified BotServer.lua in the Discord Server (work like the original BotServer) !!!

You can download the BotServer here: https://github.com/GabrielPiM/DreamNavBotServer/releases/download/V1.0.0/DibraServer.Win.x64.rar

[ENG] THIS PROGRAM IS FREE AND CAN NOT BE SOLD !!

[BR] ESTE PROGRAMA É GRATIS E NÃO PODE SER VENDIDO !!

Usage Example (OTCV8 .lua script):

```lua
--[REQUIRED] Change BotServer URL
BotServer.url = "ws://127.0.0.1:5000/send"

local playerName = name()
local channel = "1"
BotServer.init(playerName, channel)

--[REQUIRED] Keep Connection Alive
macro(1000, function()
  BotServer._websocket.send({type="ping"})
end)

BotServer.listen("Test", function(name, mens)
  print("Recived Mens From "..name)
  print("Mens:"..mens)
end)

macro(1000, function()
  print("Sending Message...")
  BotServer.send("Test", "Test Message")
end)
```

[ENG] Liked it? consider buying a script!

[BR] Gostou? considere comprar um script!
