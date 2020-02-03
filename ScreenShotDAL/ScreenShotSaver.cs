using MongoDB.Driver;
using ScreenShotDAL.Entity;
using System;
using System.Collections.Generic;

namespace ScreenShotDAL
{
    public class ScreenShotSaver
    {
        private readonly IMongoCollection<ScreenShotData> _screens;

        public ScreenShotSaver(string ConnectionString, string Database, string Collection)
        {
            MongoClient dbClient = new MongoClient(ConnectionString);
            IMongoDatabase db = dbClient.GetDatabase(Database);
            _screens = db.GetCollection<ScreenShotData>(Collection);
        }

        public List<ScreenShotData> GetByUrl(string url)
        {
            var result = _screens.Find(
                 s => s.Url == url).ToList();
            return result;
        }

        public List<ScreenShotData> GetByDateInterval(DateTime min, DateTime max)
        {
            return _screens.Find(
                                    x => x.Date > min &
                                    x.Date < max
                                    ).ToList();

        }

        public ScreenShotData Create(ScreenShotData screenShot)
        {
            _screens.InsertOne(screenShot);
            return screenShot;
        }

    }
}
