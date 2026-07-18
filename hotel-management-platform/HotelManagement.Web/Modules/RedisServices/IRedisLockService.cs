namespace Management_Hotel_2025.Modules.RedisServices
{
    public interface IRedisLockService
    {

        Task<bool> AcquireAsync(string key, string value, TimeSpan expiry);
        Task ReleaseAsync(string key, string value);

    }
}
