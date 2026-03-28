// using MongoDB.Driver;
// using System;
// using Vulpes.Zinc.Domain.Models;

// namespace Vulpes.Zinc.External.Mongo.Indexes;

// public class UniqueEmailIndex : MongoIndexDefinition<ZincUser>
// {
//     public override string Name => "email_unique";

//     public UniqueEmailIndex(IMongoProvider mongoProvider) : base(mongoProvider) { }

//     protected override async Task CreateIndexInternalAsync()
//     {
//         var collection = GetCollection();

//         var index = new CreateIndexModel<ZincUser>(
//             Builders<ZincUser>.IndexKeys.Ascending(user => user.Email),
//             new CreateIndexOptions()
//             {
//                 Unique = true,
//                 Name = Name
//             }
//         );

//         await collection.Indexes.CreateOneAsync(index);
//     }
// }
