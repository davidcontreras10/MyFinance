using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Services
{
	public interface IBccrExchangeCache
	{
		BccrSingleVentanillaModel Get(string indicator, DateTime dateTime);
		public void Set(string indicator, BccrSingleVentanillaModelResponse response);
	}

	public class BccrExchangeCache : IBccrExchangeCache
	{
		private const string IndexPrefix = "BccrExchangeCache";
		private const int CacheHours = 3;
		private const int LastUpdateMinutesCache = 30;
		private readonly TimeSpan MaxTimeLastItem = TimeSpan.FromMinutes(2);
		private readonly IAppMemoryCache appMemoryCache;

		public BccrExchangeCache(IAppMemoryCache appMemoryCache)
		{
			this.appMemoryCache = appMemoryCache;
		}

		public BccrSingleVentanillaModel Get(string indicator, DateTime dateTime)
		{
			var key = GetKey(indicator);
			var cacheItems = appMemoryCache.Get<CacheIndicatorItems>(key);
			if (cacheItems?.Items == null || !cacheItems.Items.Any())
			{
				return null;
			}

			var cacheItemsRange = cacheItems.Items.FirstOrDefault(x => x.RequestedInit <= dateTime && x.RequestedEnd > dateTime);
			if (cacheItemsRange == null)
			{
				return null;
			}

			var orderedItems = cacheItemsRange.Items.OrderByDescending(x => x.LastUpdate);
			foreach (var item in orderedItems)
			{
				if (dateTime >= item.LastUpdate)
				{
					return item;
				}
			}

			throw new Exception("Expected item but not found");
		}

		public void Set(string indicator, BccrSingleVentanillaModelResponse response)
		{
			var endReqDate = response.EndReqDate > DateTime.Now ? DateTime.Now.Add(MaxTimeLastItem) : response.EndReqDate;
			var current = appMemoryCache.Get<CacheIndicatorItems>(GetKey(indicator));
			if (current == null)
			{
				var newItems = new CacheItemsRange(Guid.NewGuid(), response.InitialReqDate, endReqDate, response.BccrSingleVentanillaModels.ToList());
				Set(indicator, new CacheIndicatorItems(new List<CacheItemsRange> { newItems }));
				return;
			}

			if (IsCoverByAnyRange(current, response, endReqDate))
			{
				return;
			}

			var overlappedRangesIds = OverlappedRanges(current, response, endReqDate).ToList();
			if (overlappedRangesIds.Any())
			{
				var overlappedRanges = current.Items.Where(current => overlappedRangesIds.Contains(current.Id)).ToList();
				var mergedRange = MergeRanges(overlappedRanges, response, endReqDate);
				current.Items.RemoveAll(x => overlappedRangesIds.Contains(x.Id));
				current.Items.Add(mergedRange);
				Set(indicator, current);
			}
			else
			{
				var newRange = new CacheItemsRange(Guid.NewGuid(), response.InitialReqDate, endReqDate, response.BccrSingleVentanillaModels.ToList());
				current.Items.Add(newRange);
				Set(indicator, current);
			}
		}

		private void Set(string indicator, CacheIndicatorItems cacheIndicatorItems)
		{
			var key = GetKey(indicator);
			appMemoryCache.Set(key, cacheIndicatorItems, TimeSpan.FromHours(CacheHours));
		}

		private static CacheItemsRange MergeRanges(IReadOnlyCollection<CacheItemsRange> itemsRanges, BccrSingleVentanillaModelResponse response, DateTime endDate)
		{
			var mergedItems = response.BccrSingleVentanillaModels.ToList();
			foreach (var range in itemsRanges)
			{
				foreach (var rangeItem in range.Items)
				{
					if (mergedItems.All(mi => mi.LastUpdate != rangeItem.LastUpdate))
					{
						mergedItems.Add(rangeItem);
					}
				}
			}

			var minMax = GetMaxMin(itemsRanges, response, endDate);
			return new CacheItemsRange(Guid.NewGuid(), minMax.Item1, minMax.Item2, mergedItems.OrderBy(m => m.LastUpdate).ToList());
		}

		private static Tuple<DateTime, DateTime> GetMaxMin(IReadOnlyCollection<CacheItemsRange> itemsRanges, BccrSingleVentanillaModelResponse response, DateTime endDate)
		{
			var min = itemsRanges.Min(x => x.RequestedInit);
			if (response.InitialReqDate < min)
			{
				min = response.InitialReqDate;
			}

			var max = itemsRanges.Max(x => x.RequestedEnd);
			if (endDate > max)
			{
				max = endDate;
			}

			return new Tuple<DateTime, DateTime>(min, max);
		}

		private static IEnumerable<Guid> OverlappedRanges(CacheIndicatorItems cacheIndicatorItems, BccrSingleVentanillaModelResponse response, DateTime reqEndDate)
		{
			foreach (var range in cacheIndicatorItems.Items.OrderBy(r => r.RequestedInit))
			{
				if (Between(response.InitialReqDate, range.RequestedInit, range.RequestedEnd) || Between(reqEndDate, range.RequestedInit, range.RequestedEnd))
				{
					yield return range.Id;
				}
				if (response.InitialReqDate < range.RequestedInit && reqEndDate > range.RequestedEnd)
				{
					yield return range.Id;
				}
			}
		}


		private static bool IsCoverByAnyRange(CacheIndicatorItems cacheIndicatorItems, BccrSingleVentanillaModelResponse response, DateTime endDate)
		{
			foreach (var range in cacheIndicatorItems.Items)
			{
				if (range.RequestedInit <= response.InitialReqDate && range.RequestedEnd >= endDate)
				{
					return true;
				}
			}

			return false;
		}

		private static bool Between(DateTime value, DateTime start, DateTime end)
		{
			return value >= start && value <= end;
		}

		private static string GetKey(string indicator)
		{
			return $"{IndexPrefix}_{indicator}";
		}

		private record class CacheIndicatorItems(List<CacheItemsRange> Items);

		private record class CacheItemsRange(Guid Id, DateTime RequestedInit, DateTime RequestedEnd, List<BccrSingleVentanillaModel> Items);
	}
}
