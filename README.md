# Dream Nav Bot Server
Bot Server alternativo ao do OTCV8

Usage Example:

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
