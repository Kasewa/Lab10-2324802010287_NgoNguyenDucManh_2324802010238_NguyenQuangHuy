using Microsoft.AspNetCore.Http;

namespace ASC.Tests
{
    public class FakeSession : ISession
    {
        private readonly Dictionary<string, byte[]> _sessionStorage = new();

        public bool IsAvailable => true;
        public string Id => "FakeSessionId";
        public IEnumerable<string> Keys => _sessionStorage.Keys;

        public void Clear() => _sessionStorage.Clear();
        public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public void Remove(string key) => _sessionStorage.Remove(key);

        public void Set(string key, byte[] value)
        {
            if (_sessionStorage.ContainsKey(key))
                _sessionStorage[key] = value;
            else
                _sessionStorage.Add(key, value);
        }

        public bool TryGetValue(string key, out byte[] value)
        {
            return _sessionStorage.TryGetValue(key, out value!);
        }
    }
}
