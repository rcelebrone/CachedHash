using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace cachedHash
{
    public class CachedHash<T>
    {
        private readonly ICacheService<T> _cacheService;

        public CachedHash(ICacheService<T> cacheService)
        {
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        }

        /// <summary>
        /// Recupera um resultado do cache ou executa a função para obter o resultado e o armazena em cache.
        /// </summary>
        /// <param name="parameters">Array de parâmetros para a query.</param>
        /// <param name="query">A string da query (usada apenas para construir a query na função callback).</param>
        /// <param name="valueFactory">Função callback que retorna a query construída e o resultado da execução.</param>
        /// <returns>O resultado do cache ou o resultado da execução da função.</returns>
        public T From(string[] parameters, string query, Func<(string queryBuilt, T result)> valueFactory)
        {
            string cacheKey = GenerateCacheKey(parameters);
            T cachedResult = _cacheService.Get(cacheKey);

            if (cachedResult != null)
            {
                return cachedResult;
            }

            (string queryBuilt, T result) = valueFactory();
            _cacheService.Set(cacheKey, result);
            return result;
        }

        /// <summary>
        /// Gera uma chave de cache única com base nos parâmetros.
        /// </summary>
        /// <param name="parameters">Array de parâmetros.</param>
        /// <returns>Uma chave de cache única.</returns>
        private static string GenerateCacheKey(string[] parameters)
        {
            if (parameters == null || !parameters.Any())
            {
                return string.Empty;
            }

            var keyBuilder = new StringBuilder();
            foreach (var param in parameters)
            {
                if (param != null)
                {
                    string normalizedParam = Regex.Replace(param.ToLowerInvariant(), "[^a-z0-9]", "");
                    keyBuilder.Append($"_{normalizedParam}");
                }
                else
                {
                    keyBuilder.Append("_null"); // Lidar com parâmetros nulos de forma consistente
                }
            }

            using (var sha256 = SHA256.Create())
            {
                var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(keyBuilder.ToString()));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            }
        }

        /// <summary>
        /// Remove um valor do cache usando os parâmetros da query.
        /// </summary>
        /// <param name="parameters">Array de parâmetros da query.</param>
        /// <param name="query">A string da query (não usada para gerar a chave, apenas para consistência com o método From).</param>
        public void RemoveFromCache(string[] parameters, string query)
        {
            string cacheKey = GenerateCacheKey(parameters);
            _cacheService.Remove(cacheKey);
        }

        /// <summary>
        /// Obtém um valor diretamente do cache usando os parâmetros.
        /// </summary>
        /// <param name="parameters">Array de parâmetros da query.</param>
        /// <param name="query">A string da query (não usada para gerar a chave, apenas para consistência com o método From).</param>
        /// <returns>O valor do cache, se existir, caso contrário, o valor padrão de T.</returns>
        public T GetFromCache(string[] parameters, string query)
        {
            string cacheKey = GenerateCacheKey(parameters);
            return _cacheService.Get(cacheKey);
        }
    }
}
