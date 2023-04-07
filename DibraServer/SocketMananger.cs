using System.Net.WebSockets;

namespace DibraServer
{
    public class SocketMananger
    {       
        Dictionary<string, List<WebSocket>> connections = new Dictionary<string, List<WebSocket>>();

        private void addChannel(string channel)
        {
            connections[channel] = new List<WebSocket>();
        }

        public void addSocket(WebSocket socket, string channel)
        {
            if (!connections.ContainsKey(channel))
            {
                addChannel(channel);
                BotServer.print("Channel added: " + channel);
            }

            connections[channel].Add(socket);            
        }

        public void removeSocket(WebSocket webSocket, string channel)
        {            
            if (connections.ContainsKey(channel))
            {
                connections[channel].RemoveAll(x => x.GetHashCode() == webSocket.GetHashCode());
            }
        }

        public List<WebSocket> getChannelConnections(string channel)
        {
            return connections[channel];    
        }
    }
}
