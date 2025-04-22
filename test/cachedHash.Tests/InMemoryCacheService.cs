namespace cachedHash.Tests
{
    public class InMemoryCacheService<TValue> : ICacheService<TValue>
    {
        private readonly Dictionary<string, TValue> _cache = new Dictionary<string, TValue>();

        public TValue Get(string key)
        {
            return _cache.TryGetValue(key, out TValue value) ? value : default(TValue);
        }

        public void Set(string key, TValue value)
        {
            _cache[key] = value;
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }
    }
}
