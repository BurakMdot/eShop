namespace Wishlist.API.Models;

public class CustomerWishlist
{
    public string BuyerId { get; set; }

    public List<WishlistItem> Items { get; set; } = [];

    public CustomerWishlist() { }

    public CustomerWishlist(string customerId)
    {
        BuyerId = customerId;
    }

}
