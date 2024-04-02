// using Microsoft.Azure.Cosmos;

// namespace StringLibraryTest;

// [TestClass]
// public class cosmos
// {
//     [TestMethod]
//     public  void deleteAsync()
//     {
// try {
//         CosmosClient cosmosClient = new CosmosClient(
//             "AccountEndpoint=https://odg-usextdev-us-cosmosdb.documents.azure.com:443/;AccountKey=l0TSZqh2D10R2jFris65OB2v8KhD61wpSStUiz92OVRlHwmrzVztFW04Wpfb4tdsMLA1ayoMNIwm4Iowhi2MqA==;"
//            );

//             Database db =  cosmosClient.GetDatabase("assessment_management");
//            Container container =  db.GetContainer("org_assessment_history");      

//            // Query for an item

        
//             using (FeedIterator<dynamic> feedIterator = container.GetItemQueryIterator<dynamic>(
//                 "select * from T where T.organizationId = '4e29cbd4-a979-5c1a-04d1-4bcb7e5eb4ce'"))
//             {
//                 while (feedIterator.HasMoreResults)
//                 {
//                     FeedResponse<dynamic> response =  feedIterator.ReadNextAsync().GetAwaiter().GetResult();
//                     foreach (var item in response)
//                     {
//                          var delresult =  container.DeleteItemAsync<dynamic>(item.id        
//                                           ,new PartitionKey("4e29cbd4-a979-5c1a-04d1-4bcb7e5eb4ce")).GetAwaiter().GetResult();
                         

//                         Console.WriteLine(item);
//                     }
//                 }
//             }

//     }
//     catch(Exception ex){

//     }
//     }

       
       



// }
