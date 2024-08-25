using System;

namespace Domain.Services
{
	public interface IAppMemoryCache
	{
		T Get<T>(string key);
		void Set<T>(string key, T value, TimeSpan expiration);
	}
}
