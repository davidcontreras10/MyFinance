using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Services
{
	public interface IBccrExchangeCache
	{
		BccrSingleVentanillaModel Get(string indicator, DateTime dateTime);
		void Set(string indicator, IEnumerable<BccrSingleVentanillaModel> items);
	}

	public class BccrExchangeCache : IBccrExchangeCache
	{
		private const string IndexPrefix = "BccrExchangeCache";
		private const int CacheHours = 12;
		private const int LastUpdateMinutesCache = 30;
		private readonly IAppMemoryCache appMemoryCache;

		public BccrExchangeCache(IAppMemoryCache appMemoryCache)
		{
			this.appMemoryCache = appMemoryCache;
		}

		public BccrSingleVentanillaModel Get(string indicator, DateTime dateTime)
		{
			var key = GetKey(indicator);
			var current = appMemoryCache.Get<CacheIndicatorBody>(key);
			if (current?.Items == null || !current.Items.Any())
			{
				return null;
			}

			var items = current.Items.OrderByDescending(x => x.LastUpdate);
			if (dateTime < items.Min(x => x.LastUpdate))
			{
				return null;
			}

			if (dateTime > items.First().LastUpdate)
			{
				var lastItemLastUpdated = current.UtcLastItemUpdatedDate;
				return DateTime.UtcNow - lastItemLastUpdated > TimeSpan.FromMinutes(LastUpdateMinutesCache) ? null : items.First();
			}

			return items.First(x => x.LastUpdate <= dateTime);
		}

		public void Set(string indicator, IEnumerable<BccrSingleVentanillaModel> items)
		{
			var key = GetKey(indicator);
			var current = appMemoryCache.Get<CacheIndicatorBody>(key);
			if (current == null)
			{
				current = new CacheIndicatorBody
				{
					UtcLastItemUpdatedDate = DateTime.UtcNow,
					Items = items
				};

				appMemoryCache.Set(key, current, TimeSpan.FromDays(CacheHours));
				return;
			}

			var updateLastItemDate = current.UtcLastItemUpdatedDate <= items.Max(x => x.LastUpdate);
			current.UtcLastItemUpdatedDate = updateLastItemDate ? DateTime.UtcNow : current.UtcLastItemUpdatedDate;
			var newItems = GetCombinedList(current.Items, items);
			appMemoryCache.Set(key, current, TimeSpan.FromDays(CacheHours));
		}

		private static IEnumerable<BccrSingleVentanillaModel> GetCombinedList(IEnumerable<BccrSingleVentanillaModel> memoryItems, IEnumerable<BccrSingleVentanillaModel> newItems)
		{
			var result = memoryItems.ToList();
			foreach (var item in newItems)
			{
				var existing = result.FirstOrDefault(x => x == item);
				if (existing != null)
				{
					result.Remove(existing);
				}
				result.Add(item);
			}
			return result;
		}

		private static string GetKey(string indicator)
		{
			return $"{IndexPrefix}_{indicator}";
		}

		private class CacheIndicatorBody
		{
			public DateTime UtcLastItemUpdatedDate { get; set; }
			public IEnumerable<BccrSingleVentanillaModel> Items { get; set; }
		}
	}
}
