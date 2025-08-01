using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

const string States = nameof(States);

var entryOptions = new MemoryCacheEntryOptions()
{
	AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(5),
}.RegisterPostEvictionCallback(EvictionNotification);

var services = new ServiceCollection();
services.AddMemoryCache();
var provider = services.BuildServiceProvider();
var memoryCache = provider.GetRequiredService<IMemoryCache>();
var startTime = Stopwatch.StartNew();

Console.WriteLine($"{startTime} - Getting states...");
var states = await GetStatesAsync();
Console.WriteLine($"{startTime} - States retrieved, count is {states.Count}, Minnesota is indexed at {states.IndexOf("Minnesota")}");
Console.WriteLine();
Console.WriteLine();

Console.WriteLine($"{startTime} - Getting states (again)...");
var statesAgain = await GetStatesAsync();
Console.WriteLine($"{startTime} - States retrieved (again), count is {statesAgain.Count}, Minnesota is indexed at {statesAgain.IndexOf("Minnesota")}");
Console.WriteLine();
Console.WriteLine();

Console.WriteLine($"{startTime} - Using local cached states, count is {states.Count}, Minnesota is indexed at {states.IndexOf("Minnesota")}");
Console.WriteLine();
Console.WriteLine();

Console.WriteLine($"{startTime} - Getting IMemoryCache states...");
var cachedStates = (await memoryCache.GetOrCreateAsync(
	States,
	entry => GetStatesAsync(), 
	entryOptions))!;
Console.WriteLine($"{startTime} - IMemoryCache states retrieved, count is {cachedStates.Count}, Minnesota is indexed at {cachedStates.IndexOf("Minnesota")}");
Console.WriteLine();
Console.WriteLine();

Console.WriteLine($"{startTime} - Getting IMemoryCache states (again)...");
var cachedStatesAgain = (await memoryCache.GetOrCreateAsync(
	States,
	entry => GetStatesAsync(),
	entryOptions))!;
Console.WriteLine($"{startTime} - IMemoryCache states retrieved (again), count is {cachedStatesAgain.Count}, Minnesota is indexed at {cachedStatesAgain.IndexOf("Minnesota")}");
Console.WriteLine();
Console.WriteLine();

Console.WriteLine($"{startTime} - Waiting 7 seconds...");
await Task.Delay(TimeSpan.FromSeconds(7));
Console.WriteLine();
Console.WriteLine();

Console.WriteLine($"{startTime} - Getting IMemoryCache states (after eviction)...");
var evictedStates = (await memoryCache.GetOrCreateAsync(
	States,
	entry => GetStatesAsync(),
	entryOptions))!;
Console.WriteLine($"{startTime} - IMemoryCache states retrieved (after eviction), count is {evictedStates.Count}, Minnesota is indexed at {evictedStates.IndexOf("Minnesota")}");
Console.WriteLine();
Console.WriteLine();

static async Task<List<string>> GetStatesAsync()
{
	await Task.Delay(TimeSpan.FromSeconds(1));

	return [
		"Alabama", "Alaska", "Arizona", "Arkansas", "California",
		"Colorado", "Connecticut", "Delaware", "Florida", "Georgia",
		"Hawaii", "Idaho", "Illinois", "Indiana", "Iowa",
		"Kansas", "Kentucky", "Louisiana", "Maine", "Maryland",
		"Massachusetts", "Michigan", "Minnesota", "Mississippi", "Missouri",
		"Montana", "Nebraska", "Nevada", "New Hampshire", "New Jersey",
		"New Mexico", "New York", "North Carolina", "North Dakota", "Ohio",
		"Oklahoma", "Oregon", "Pennsylvania", "Rhode Island", "South Carolina",
		"South Dakota", "Tennessee", "Texas", "Utah", "Vermont",
		"Virginia", "Washington", "West Virginia", "Wisconsin", "Wyoming"
	];
}

static void EvictionNotification(object key, object? value, EvictionReason reason, object? state) =>
	Console.WriteLine($"Evicted! key = {key}, value = {value}, reason = {reason}");