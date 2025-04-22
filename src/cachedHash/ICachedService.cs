namespace cachedHash
{
    public interface ICacheService<TValue>
    {
        TValue Get(string key);
        void Set(string key, TValue value);
        void Remove(string key);
    }
}
