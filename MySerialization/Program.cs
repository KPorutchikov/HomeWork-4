using System;
using System.Collections.Generic;
using AutoFixture;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace MySerialization
{
    public class Program
    {
        private static Fixture _fixture = new Fixture();
        public static List<Order> MyOrders = new List<Order>() {};


        public class SerializeTest
        {
            public List<Order> Value { get; set; }

            [GlobalSetup] 
            public void GlobalSetup()
            {
                Value = MyOrders;
            }

            [Benchmark]
            public void MySerializator()
            {
                for (int i = 0; i < 1000; i++)
                {
                    var SrToCSV = new SerializatorToCSV<Order>("orders.csv");
                    SrToCSV.Serialize(Value);
                }
            }

            [Benchmark]
            public void MyDserializator()
            {
                for (int i = 0; i < 1000; i++)
                {
                    var SrToCSV = new SerializatorToCSV<Order>("orders.csv");
                    SrToCSV.Deserialize();
                }
            }

            [Benchmark]
            public void NewtonSerializator()
            {
                for (int i = 0; i < 1000; i++)
                {
                    var SrToJson = new SerializatorToJson<Order>("orders.json");
                    SrToJson.Serialize(Value);
                }
            }

            [Benchmark]
            public void NewtonDeserializator()
            {
                for (int i = 0; i < 1000; i++)
                {
                    var SrToJson = new SerializatorToJson<Order>("orders.json");
                    SrToJson.Deserialize();
                }
            }
        }

        static void Main(string[] args)
        {
            DateTime StartDate;
            DateTime EndDate;
            IList<Order> DeserializedOrders = null;

            // Добавим тестовых данных
            MyOrders.AddRange(new List<Order>() {new Order ( 1, DateTime.Now, "Хлеб", "без дрожжевой", "ул.Гагарина д.18, кв.10", 1, 86.4 ),
                                                 new Order ( 1, DateTime.Now, "Кефир", "0.5Л, жирность 1%", "ул.Гагарина д.18, кв.10", 2, 142.6),
                                                 new Order ( 2, DateTime.Now, "Мороженное", "эскимо на палочке", "ул.Мира д.2, кв.118", 3, 210.0)    });

            // Добавим еще автосгенерированных данных ....
            for (int i = 0; i < 1000; i++)
            {
                MyOrders.Add(_fixture.Create<Order>());
            }


            // Сериалиузем данные в файл CSV
            StartDate = DateTime.Now;
            for (int i = 0; i < 1000; i++)
            {
                var SrToCSV = new SerializatorToCSV<Order>("orders.csv");
                SrToCSV.Serialize(MyOrders);
            }
            EndDate = DateTime.Now;
            Console.WriteLine($"Затраченое время на сериализацию в CSV файл: {(StartDate - EndDate).Duration().TotalMilliseconds} (ms)");


            // Десериалиузем данные из файла CSV
            StartDate = DateTime.Now;
            for (int i = 0; i < 1000; i++)
            {
                var SrFromCSV = new SerializatorToCSV<Order>("orders.csv");
                DeserializedOrders = SrFromCSV.Deserialize();
            }
            EndDate = DateTime.Now;
            Console.WriteLine($"Затраченое время на десериализацию из CSV файла: {(StartDate - EndDate).Duration().TotalMilliseconds} (ms)");


            // Сериалиузем данные в файл JSON
            StartDate = DateTime.Now;
            for (int i = 0; i < 1000; i++)
            {
                var SrToJson = new SerializatorToJson<Order>("orders.json");
                SrToJson.Serialize(MyOrders);
            }
            EndDate = DateTime.Now;
            Console.WriteLine($"Затраченое время на сериализацию в JSON файл: {(StartDate - EndDate).Duration().TotalMilliseconds} (ms)");

            // Десериалиузем данные из файла JSON
            StartDate = DateTime.Now;
            for (int i = 0; i < 1000; i++)
            {
                var SrToJson = new SerializatorToJson<Order>("orders.json");
                DeserializedOrders = SrToJson.Deserialize();
            }
            EndDate = DateTime.Now;
            Console.WriteLine($"Затраченое время на десериализацию из JSON файл: {(StartDate - EndDate).Duration().TotalMilliseconds} (ms)");



            // Измерение производительности кастомной сериализации в CSV файл, 
            // против сериализации Newtonsoft в JSON файл, используя библиотеку BenchmarkDotNet.
            BenchmarkRunner.Run<SerializeTest>();

            Console.ReadLine();
        }
    }
}
