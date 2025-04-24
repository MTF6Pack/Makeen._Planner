using System.Collections.Concurrent;

namespace Infrastructure.SignalR
{
    public static class UserConnectionManager
    {
        private static readonly ConcurrentDictionary<string, HashSet<string>> _userConnections = new();
        private static readonly ConcurrentDictionary<string, string> _connectionUserMap = new();

        public static void AddConnection(string userId, string connectionId)
        {
            _connectionUserMap[connectionId] = userId;
            _userConnections.AddOrUpdate(userId,
                new HashSet<string> { connectionId },
                (_, connections) =>
                {
                    connections.Add(connectionId);
                    return connections;
                });
        }

        public static bool TryGetConnections(string userId, out IReadOnlyList<string> connectionIds)
        {
            if (_userConnections.TryGetValue(userId, out var connections))
            {
                connectionIds = connections.ToList();
                return true;
            }
            connectionIds = Array.Empty<string>();
            return false;
        }

        public static void RemoveConnection(string connectionId)
        {
            if (_connectionUserMap.TryRemove(connectionId, out var userId))
            {
                _userConnections.AddOrUpdate(userId,
                    new HashSet<string>(),
                    (_, connections) =>
                    {
                        connections.Remove(connectionId);
                        return connections;
                    });
            }
        }

        public static string? GetUserIdFromConnection(string connectionId)
        {
            return _connectionUserMap.TryGetValue(connectionId, out var userId) ? userId : null;
        }
    }
}