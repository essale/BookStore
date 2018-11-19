using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ML.Runtime.Api;

using Microsoft.ML.Legacy;
using Microsoft.ML.Legacy.Data;
using Microsoft.ML.Legacy.Trainers;
using Microsoft.ML.Legacy.Transforms;
using System.IO;

namespace BookStore
{
    public class BookData 
    {
        [Column("0")]
        public float genre;

        [Column("1")]
        public float releaseTime;

        [Column("2")]
        public float price;
    }

    public class ClusterPrediction
    {
        [ColumnName("PredictedLabel")]
        public uint PredictedClusterId;

        [ColumnName("Score")]
        public float[] Distances;
    }

    public class BookClustering
    {

        static readonly string _dataPath =  "booksDataset.csv";
        static readonly string _modelPath = "ClusteringModel.zip";

        private static PredictionModel<BookData, ClusterPrediction> Train()
        {
            var pipeline = new LearningPipeline();
           // pipeline.Add(new TextLoader(_dataPath).CreateFrom<BookData>(separator: ','));

            //building dataset of BookData
            List<BookData> data = new List<BookData>();
            string line;
            using (var reader = File.OpenText(_dataPath))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    string convertedData = line;
                    List<string> BookFeaturesSet = convertedData.Split(',').ToList();
                    BookData bd = new BookData
                    {
                        genre = float.Parse(BookFeaturesSet[0]), //book.Genre,
                        releaseTime = float.Parse(BookFeaturesSet[1]),
                        price = float.Parse(BookFeaturesSet[2])//book.Price
                    };
                    data.Add(bd);
                }
            }

            var collection = CollectionDataSource.Create(data);
            pipeline.Add(collection);
            pipeline.Add(new ColumnConcatenator(
                "Features",
                "price",
                "genre",
                "releaseTime")
                );

            pipeline.Add(new KMeansPlusPlusClusterer() { K = 5 });

            var model = pipeline.Train<BookData, ClusterPrediction>();

            return model;
        }

        public ClusterPrediction Predict(BookData bookData)
        {
            var model = Train();
            return model.Predict(bookData);
        }

        private static async Task Main(string[] args)
        {
            var model = Train();
            BookData t1 = new BookData
            {
                genre = 5.1f,
                releaseTime = 3.5f,
                price = 1.4f,
            };

            var prediction = model.Predict(t1);

        }
    }
}