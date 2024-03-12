using System.Text.Json;
using System.Text.Json.Serialization;
using StackExchange.Redis;
using Wishlist.API.Models;

namespace Wishlist.API.Repositories;
public class RedisWishlistRepository(ILogger<RedisWishlistRepository> logger, IConnectionMultiplexer redis) : IWishlistRepository
{
    private readonly IDatabase _database = redis.GetDatabase();

    // implementation:

    // - /wishlist/{id} "string" per unique wishlist
    private static RedisKey WishlistKeyPrefix = "/wishlist/"u8.ToArray();
    // note on UTF8 here: library limitation (to be fixed) - prefixes are more efficient as blobs

    private static RedisKey GetWishlistKey(string userId) => WishlistKeyPrefix.Append(userId);

    public async Task<bool> DeleteWishlistAsync(string id)
    {
        return await _database.KeyDeleteAsync(GetWishlistKey(id));
    }

    public async Task<CustomerWishlist> GetWishlistAsync(string customerId)
    {
        using var data = await _database.StringGetLeaseAsync(GetWishlistKey(customerId));

        if (data is null || data.Length == 0)
        {
            return null;
        }
        return JsonSerializer.Deserialize(data.Span, WishlistSerializationContext.Default.CustomerWishlist);
    }

    public async Task<CustomerWishlist> UpdateWishlistAsync(CustomerWishlist wishlist)
    {
        var json = JsonSerializer.SerializeToUtf8Bytes(wishlist, WishlistSerializationContext.Default.CustomerWishlist);
        var created = await _database.StringSetAsync(GetWishlistKey(wishlist.BuyerId), json);

        if (!created)
        {
            logger.LogInformation("Problem occurred persisting the item.");
            return null;
        }


        logger.LogInformation("Wishlist item persisted successfully.");
        return await GetWishlistAsync(wishlist.BuyerId);
    }
}

[JsonSerializable(typeof(CustomerWishlist))]
[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
public partial class WishlistSerializationContext : JsonSerializerContext
{

}

