using Core.Interfaces;
using Core.Models;
using GraphQL;
using GraphQL.Client.Abstractions;
using System.Text;


namespace Core.Services
{
    public class PriceGetterService : IPriceGetterService
    {
        private readonly IGraphQLClient _client;
        public PriceGetterService(IGraphQLClient client)
        {
            _client = client;
        }

        /// <inheritdoc />
        public async Task<Prices> GetPrices()
        {
            Prices prices = new()
            {
                Markets = new List<Market>()
            };
            var pageAssets = await GetAssets();
            var first100assets = pageAssets?.Assets?.Take(100);

            for(var number = 0; number < 100 ; number += 20)
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.Append("[\"");
                var firstBatch = first100assets?.Skip(number).Take(20);
                foreach (var asset in firstBatch!)
                {
                    stringBuilder.Append(asset?.AssetSymbol);
                    stringBuilder.Append("\", \"");
                }
                
                stringBuilder.Append("]");
                var batchPrices = await GetBatchPrices(stringBuilder.ToString().Replace(", \"]", "]"));
                prices?.Markets?.AddRange(batchPrices?.Markets!);
            }

            return prices!;
        }

        private async Task<PageAssets> GetAssets()
        {

            var query = new GraphQLRequest
            {
                Query = @"query PageAssets {
                            assets(sort: [{marketCapRank: ASC}]) {
                              assetName
                              assetSymbol
                              marketCap
                           }
                          }",
                Variables = null
            };

            var response = await _client.SendQueryAsync<PageAssets>(query);
            return response.Data;
        }

        private async Task<Prices> GetBatchPrices(string assetSymbolList)
        {
            var query = @"query MarketsByBaseSymbol {
                            markets(filter: {
                              baseSymbol: {
                                _in :" + assetSymbolList +
                          @"    }, 
                              quoteSymbol: {_eq: ""EUR""}, 
                              exchangeSymbol: {_eq: ""Binance""}}
                            ) {
                             marketSymbol
                             ticker {
                               lastPrice
                             }
                            }
                           }";

            var requestQuery = new GraphQLRequest
            {
                Query = query,
                Variables = null
            };

            var response = await _client.SendQueryAsync<Prices>(requestQuery);
            return response.Data;
        }
    }
}
