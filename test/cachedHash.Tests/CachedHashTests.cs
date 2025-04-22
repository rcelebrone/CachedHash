using Xunit;
using cachedHash;
using System.Collections.Generic;
using System.Text.Json;

namespace cachedHash.Tests
{
    public class CachedHashTests
    {
        [Fact]
        public void GenerateCacheKey_WithDiverseCharacters()
        {
            string[] parameters = { "A1!", "b2@", "C#3" };
            string expectedKey = "c6418271945a76991199d151221333960199f07173965a96a6c1372691417958";
            Assert.Equal(expectedKey, CachedHash<string>.GenerateCacheKey(parameters));
        }

        [Fact]
        public void GenerateCacheKey_WithValidCharacters()
        {
            string[] parameters = { "abc1", "def2", "ghi3" };
            string expectedKey = "698510d79434c71a93033b0b8c696556435965757509399948b7294880107159";
            Assert.Equal(expectedKey, CachedHash<string>.GenerateCacheKey(parameters));
        }

        [Fact]
        public void GenerateCacheKey_WithNumericParameters()
        {
            string[] parameters = { "123", "456", "789" };
            string expectedKey = "364c4499935c240882ba149505d84110c446254080725b869247429511862361";
            Assert.Equal(expectedKey, CachedHash<string>.GenerateCacheKey(parameters));
        }

        [Fact]
        public void GenerateCacheKey_WithNullParameters()
        {
            string[] parameters = { "abc", null, "def" };
            string expectedKey = "819a6f79159d94f6b169f58553f3259592665769657669947468699973956682";
            Assert.Equal(expectedKey, CachedHash<string>.GenerateCacheKey(parameters));
        }

        [Fact]
        public void From_ShouldCacheAndReturnResult()
        {
            var cacheService = new InMemoryCacheService<List<string>>();
            var cache = new CachedHash<List<string>>(cacheService);
            string query = "SELECT Name FROM Users WHERE Id = {0}";
            string[] parameters = { "1" };

            Func<(string queryBuilt, List<string> result)> valueFactory = () =>
            {
                return (string.Format(query, parameters), new List<string> { "John" });
            };

            List<string> result1 = cache.From(parameters, query, valueFactory);
            List<string> result2 = cache.From(parameters, query, valueFactory); // Deverá vir do cache

            Assert.Equal("[\"John\"]", JsonSerializer.Serialize(result1));
            Assert.Equal("[\"John\"]", JsonSerializer.Serialize(result2));
            Assert.NotNull(cacheService.Get(CachedHash<List<string>>.GenerateCacheKey(parameters)));
        }

        [Fact]
        public void GetFromCache_ShouldReturnCachedValue()
        {
            var cacheService = new InMemoryCacheService<string>();
            var cache = new CachedHash<string>(cacheService);
            string query = "SELECT Email FROM Users WHERE Id = {0}";
            string[] parameters = { "2" };
            string cacheKey = CachedHash<string>.GenerateCacheKey(parameters);
            cacheService.Set(cacheKey, "john.doe@example.com");

            string cachedEmail = cache.GetFromCache(parameters, query);
            Assert.Equal("john.doe@example.com", cachedEmail);
        }

        [Fact]
        public void RemoveFromCache_ShouldRemoveCachedValue()
        {
            var cacheService = new InMemoryCacheService<string>();
            var cache = new CachedHash<string>(cacheService);
            string query = "DELETE FROM Users WHERE Id = {0}";
            string[] parameters = { "3" };
            string cacheKey = CachedHash<string>.GenerateCacheKey(parameters);
            cacheService.Set(cacheKey, "userToDelete");
            Assert.NotNull(cacheService.Get(cacheKey));

            cache.RemoveFromCache(parameters, query);
            Assert.Null(cacheService.Get(cacheKey));
        }
    }
}
