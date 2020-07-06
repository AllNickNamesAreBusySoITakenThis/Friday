using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FridayLib.Models
{
    public class MainModel
    {
        public async static void WriteToDatabse(ICollection<ControlledProject> projects)
        {
			try
			{
                MongoClient client = new MongoClient("mongodb://localhost:27017");
                IMongoDatabase database = client.GetDatabase("ProjectsDatabase");
                var collection = database.GetCollection<BsonDocument>("ProjectsData");
                foreach(var prj in projects)
                {
                    var filter = Builders<BsonDocument>.Filter.Eq("Name", prj.Name);
                    collection.ReplaceOne(filter, prj.ToBsonDocument(), new ReplaceOptions { IsUpsert = true });
                    //var doc = prj.ToBsonDocument();
                    //collection.InsertOne(doc);

                }
            }
			catch (Exception ex)
			{

				throw;
			}
        }
        public static ICollection<ControlledProject> ReadFromDatabase()
        {
            try
            {
                ObservableCollection<ControlledProject> result = new ObservableCollection<ControlledProject>();
                MongoClient client = new MongoClient("mongodb://localhost:27017");
                IMongoDatabase database = client.GetDatabase("ProjectsDatabase");
                var collection = database.GetCollection<BsonDocument>("ProjectsData");
                var filter = new BsonDocument();
                var data = collection.Find(filter).ToList();
                foreach (var doc in data)
                {
                    result.Add(ControlledProject.FromBsonDocument(doc));
                }
                return result;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
