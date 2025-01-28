using System.Security.Cryptography;
using System.Text;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace MessengerInterfaces.Utils;

public static class Utils
{
	//private Container container;
	//public Utils(CosmosClient client)
	//{
	//	container = client.GetContainer("cactus-messenger", "cactus-messenger");
	//}

	public static async Task<List<T>> ToListAsync<T>(IAsyncEnumerable<T> source)
	{
		List<T> result = [];

		await foreach (T? item in source)
		{
			result.Add(item);
		}

		return result;
	}

	public static string GetStringSha256Hash(string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			return string.Empty;
		}

		byte[] textData = Encoding.UTF8.GetBytes(text);
		byte[] hash = SHA256.HashData(textData);
		return BitConverter.ToString(hash).Replace("-", string.Empty);
	}

	public static async IAsyncEnumerable<T> ExecuteQuery<T>(IQueryable<T> query)
	{
		FeedIterator<T> iterator = query
			.ToFeedIterator();

		while (iterator.HasMoreResults)
		{
			FeedResponse<T> page = await iterator.ReadNextAsync();

			foreach (T? item in page)
			{
				yield return item;
			}
		}
	}

	public static async Task DeleteQuery<T>(IQueryable<T> query)
	{
		FeedIterator<T> iterator = query
			.ToFeedIterator();

		while (iterator.HasMoreResults)
		{
			FeedResponse<T> page = await iterator.ReadNextAsync();

			foreach (T? item in page) { }
		}
	}

	public static string Relativize(DateTime date1, DateTime date2)
	{
		TimeSpan diff = date2 - date1;
		TimeSpan dayDiff = date2.Date - date1.Date;

		if (dayDiff.Days > 7)
		{
			return date1.ToString();
		}

		if (dayDiff.Days >= 1)
		{
			return $"{dayDiff.Days} " + (dayDiff.Days != 1 ? "days" : "day") + " ago at " +
				(date1.Hour > 9 ? date1.Hour.ToString() : "0" + date1.Hour) + ":" +
				(date1.Minute > 9 ? date1.Minute.ToString() : "0" + date1.Minute);
		}

		if (diff.Hours >= 1)
		{
			return $"{diff.Hours} " + (diff.Hours != 1 ? "hours" : "hour") + " ago";
		}

		if (diff.Minutes == 0)
		{
			return "now";
		}

		return $"{diff.Minutes} " + (diff.Minutes != 1 ? "minutes" : "minute") + " ago";
	}

	public static List<(int, T)> Enumerate<T>(List<T> enumerable)
	{
		List<(int, T)> output = [];

		for (int i = 0; i < enumerable.Count; i++)
		{
			output.Add((i, enumerable[i]));
		}

		return output;
	}
}