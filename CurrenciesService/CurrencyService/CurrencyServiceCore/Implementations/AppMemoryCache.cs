using Domain.Services;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace CurrencyServiceCore.Implementations
{
	public class AppMemoryCache : IAppMemoryCache
	{
		private readonly IMemoryCache memoryCache;

		public AppMemoryCache(IMemoryCache memoryCache)
		{
			this.memoryCache = memoryCache;
		}

		public T Get<T>(string key)
		{
			return memoryCache.Get<T>(key);
		}

		public void Set<T>(string key, T value, TimeSpan expiration)
		{
			memoryCache.Set(key, value, expiration);
		}
	}
}
