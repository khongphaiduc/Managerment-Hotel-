using System.Collections.Concurrent;

namespace Management_Hotel_2025.Modules.RedisServices
{
    public sealed class InMemoryRedisLockService : IRedisLockService
    {
        private readonly ConcurrentDictionary<string, LockEntry> _locks = new();

        public Task<bool> AcquireAsync(string key, string value, TimeSpan expiry)
        {
            var entry = new LockEntry(value, DateTimeOffset.UtcNow.Add(expiry));
            var acquired = _locks.AddOrUpdate(
                key,
                entry,
                (_, existing) => existing.ExpiresAt <= DateTimeOffset.UtcNow ? entry : existing) == entry;

            return Task.FromResult(acquired);
        }

        public Task ReleaseAsync(string key, string value)
        {
            if (_locks.TryGetValue(key, out var entry) && entry.Value == value)
            {
                _locks.TryRemove(key, out _);
            }

            return Task.CompletedTask;
        }

        private sealed record LockEntry(string Value, DateTimeOffset ExpiresAt);
    }
}
