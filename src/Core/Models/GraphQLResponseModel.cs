namespace Core.Models
{
    public class PageAssets
    {
        public List<Asset>? Assets { get; set; }
    }

    public class Prices
    {
        public List<Market>? Markets { get; set; }
    }

    public class Asset
    {
        public string? AssetName { get; set; }
        public string? AssetSymbol { get; set; }
        public Int64? MarketCap { get; set; }

    }

    public class Market
    {
        public string? MarketSymbol { get; set; }
        public Ticker? Ticker { get; set; }
    }

    public class Ticker
    {
        public string? LastPrice { get; set; }
    }
}
