using Wishlist.API.Models;

namespace Wishlist.API.Repositories;

public interface IWishlistRepository
{
    Task<CustomerWishlist> GetWishlistAsync(string customerId);
    Task<CustomerWishlist> UpdateWishlistAsync(CustomerWishlist wishlist);
    Task<bool> DeleteWishlistAsync(string id);
}
