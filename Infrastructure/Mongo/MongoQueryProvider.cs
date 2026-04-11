// using Vulpes.Electrum.Domain.Extensions;
// using Vulpes.Perpendicularity.Core.Data;

// namespace Vulpes.Perpendicularity.Infrastructure.Mongo;

// public class MongoQueryProvider<TResponse> : IQueryProvider<TResponse>
// {
//     private readonly IMongoProvider mongoProvider;

//     public MongoQueryProvider(IMongoProvider mongoProvider)
//     {
//         this.mongoProvider = mongoProvider;
//     }

//     public Task<IQueryable<TResponse>> BeginQueryAsync()
//     {
//         var result = mongoProvider.GetQuery<TResponse>(CqrsType.Query).AsQueryable();
//         return result.FromResult();
//     }
// }