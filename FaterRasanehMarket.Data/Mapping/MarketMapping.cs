using Microsoft.EntityFrameworkCore;


namespace FaterRasanehMarket.Data.Mapping
{
    public static class MarketMapping
    {
        public static void AddCustomMarketMappings(this ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ProductCategoryMapping());
            modelBuilder.ApplyConfiguration(new ProductDiscountMapping());
            modelBuilder.ApplyConfiguration(new LikeMapping());
        }
    }
}
