using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using PhoneApp14.Resources;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Json;
using Polenter.Serialization;
using Newtonsoft.Json;
using PhoneApp14.BinarySerialzier;
using System.Reflection;
using System.Collections;
using PhoneApp14.SampleData;

namespace PhoneApp14
{
    public class Statistic
    {
        public Statistic(string name)
        {
            this.Name = name;
        }

        public string Name { get; set; }

        public int TotalRuns { get; set; }

        public int SuccessRuns { get; set; }

        public long DeserialzingTime { get; set; }

        public long SerializingTime { get; set; }
    }

    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        public string results = string.Empty;



        public void MeasureAll()
        {
            //this.Measure(new Statistic("StandartJson"), new JsonSerialzier<List<User>>());
            ////this.Measure("PolenterBinary", new PolenterSerializer<List<User>>());
            
            ////this.Measure("NewtonsoftBson", new NewtonsoftBsonSerialzier<List<User>>());
            //this.Measure("SimpleBinary", new BinarySerializer<List<User>>());
            this.Measure(new Statistic("NewSerialzier"), new NewSerializer<List<User>>());
            //this.Measure(new Statistic("NewtonsoftJson"), new NewtonsoftJsonSerialzier<List<User>>());
                
            MessageBox.Show(results);
            results = string.Empty;
        }

        private void Measure(Statistic statistics, Serialzier<List<User>> serializer)
        {
            for (int i = 0; i < 1; i++)
            {
                this.MeasureSingle(serializer, statistics);
            }

            results += statistics.Name + ": s" 
                     + statistics.SerializingTime / statistics.TotalRuns +
                      " d: " + statistics.DeserialzingTime / statistics.TotalRuns +
                    " isCorrect: " + (statistics.SuccessRuns == statistics.TotalRuns) + "\n";

            //var data = this.GenerateData();
            //using (var memoryStream = new MemoryStream())
            //{
            //    Stopwatch sw = Stopwatch.StartNew();

            //    serializer.Serialize(memoryStream, data);
            //    sw.Stop();
                
            //    memoryStream.Seek(0, SeekOrigin.Begin);
            //    sw.Reset();
            //    sw.Start();
            //    var copy = serializer.Deserialize(memoryStream);
                
            //    sw.Stop();
                
                
            //    results += " d: " + sw.ElapsedTicks;
            //    var isCorrect = IsCorrect(data, copy);
            //    results += " isCorrect: " + isCorrect;
            //    results += "\n";
            //}
        }

        private void MeasureSingle(Serialzier<List<User>> serializer, Statistic statistic)
        {
            var data = this.GenerateData();
            using (var memoryStream = new MemoryStream())
            {
                Stopwatch sw = Stopwatch.StartNew();

                serializer.Serialize(memoryStream, data);
                sw.Stop();
                statistic.SerializingTime += sw.ElapsedTicks;

                //results += name + ": s" + sw.ElapsedTicks;
                memoryStream.Seek(0, SeekOrigin.Begin);
                sw.Reset();
                sw.Start();
                var copy = serializer.Deserialize(memoryStream);

                sw.Stop();
                statistic.DeserialzingTime += sw.ElapsedTicks;

                //results += " d: " + sw.ElapsedTicks;
                var validCopy = CreateValidCopy();
                var isCorrect = IsCorrect(validCopy, copy);
                statistic.TotalRuns++;
                if (isCorrect)
                {
                    statistic.SuccessRuns++;
                }

                //results += " isCorrect: " + isCorrect;
                //results += "\n";
            }
        }

        private List<User> validCopy;

        public List<User> CreateValidCopy()
        {

            if (validCopy == null)
            {
                var data = this.GenerateData();
                using (var memoryStream = new MemoryStream())
                {
                    var seriazlier = new JsonSerialzier<List<User>>();
                    seriazlier.Serialize(memoryStream, data);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    validCopy = seriazlier.Deserialize(memoryStream);
                }
            }

            return validCopy;
        }

        private List<User> GenerateData()
        {
            var users = new List<User>();
            for (int i = 0; i < 200; i++)
            {
                var user = new User
                {
                    Id = i,
                    Name = new string((char)('a' + i % 5), 20 + i % 3),
                    Test1 = true,
                    Test2 = Byte.MaxValue,
                    Test3 = Char.MaxValue,
                    Test4 = Decimal.MaxValue,
                    Test5 = Double.MaxValue,
                    Test6 = Int16.MaxValue,
                    Test7 = Int32.MaxValue,
                    Test8 = Int64.MaxValue,
                    Test9 = SByte.MaxValue,
                    Test10 = float.MaxValue,
                    Test11 = "asfaf",
                    Test12 = UInt16.MaxValue,
                    Test13 = UInt32.MaxValue,
                    Test14 = UInt64.MaxValue,
                    Test15 = new byte[5] { 0, 1, 2, 3, 4 },
                    Test16 = new int[3][] { new int[] { 4, 5, 8 }, new int[] { 5, 5 }, null },
                    Test17 = new List<object> { 1, 2, 4 },
                    Test18 = new object[5] { 3, 4, 5, 7, 7 },
                    //Test19 = new Item(),
                    Test20 = new Dictionary<string, Item> { { "test1", new Item() { Id = 1 } },  },
                    Test21 = SampleEnumInt.Second,
                    Test22 = SampleEnumLong.Second,
                    Test24 = 5,
                    Test25 = new B { Visible1 = 1, Visible2 = 1, Unvisible = 1, SameName = 2 },
                    Test26 = new MyList<string> { "dsaf", "sdfsdgsdgdg" },
                    //Test27 = new DateTime(2355, DateTimeKind.Local),
                    //Test28 = DateTime.UtcNow,
                    Test29 = TimeSpan.FromDays(3),
                    Test30 = new Uri("http://saf.com"),
                    Test32 = null
                };

                user.Items = new List<Item>();
                for (int j = 0; j < 30 + i % 15; j++)
                {
                    var item = new Item()
                    {
                        Id = j,
                        Name = new string((char)('a' + j % 5), 20 + i % 3),
                    };

                    user.Items.Add(item);
                }

                users.Add(user);
                
            };

            return users;
        }

        private bool IsCorrect(object left, object right)
        {
            if (left is Uri)
            {
                left.ToString();
            }

            bool isCorrectCopy = false;
            bool isSimple = true;
            if (left != null)
            {
                isSimple = left.GetType().IsValueType || left is string || left is Uri;
            }


            if (isSimple)
            {
                isCorrectCopy = Object.Equals(left, right);
            }
            else
            {
                if (left != right)
                {
                    if (left != null && right != null)
                    {
                        if (left.GetType() == right.GetType())
                        {

                            if (left is IList)
                            {
                                if (right is IList)
                                {
                                    var leftList = (IList)left;
                                    var rightList = (IList)right;
                                    if (leftList.Count == rightList.Count)
                                    {
                                        isCorrectCopy = true;
                                        for (int i = 0; i < leftList.Count; i++)
                                        {
                                            isCorrectCopy = IsCorrect(leftList[i], rightList[i]);
                                            if (!isCorrectCopy)
                                            {
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                            else if (left is IDictionary)
                            {
                                if (right is IDictionary)
                                {
                                    var leftDictionary = (IDictionary)left;
                                    var rightDictionary = (IDictionary)right;
                                    if (leftDictionary.Count == rightDictionary.Count)
                                    {
                                        isCorrectCopy = true;
                                        for (int i = 0; i < leftDictionary.Count; i++)
                                        {
                                            isCorrectCopy = IsCorrect(leftDictionary[i], rightDictionary[i]);
                                            if (!isCorrectCopy)
                                            {
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                var allProperties = left.GetType().GetProperties();
                                isCorrectCopy = true;
                                foreach (var property in allProperties)
                                {
                                    var leftValue = property.GetValue(left);
                                    var rightValue = property.GetValue(right);
                                    isCorrectCopy = IsCorrect(leftValue, rightValue);
                                    if (!isCorrectCopy)
                                    {
                                        isCorrectCopy.ToString();
                                        break;
                                    }

                                }
                            }
                        }
                    }
                }
                else
                {
                    isCorrectCopy = true;
                }
            }

            if (!isCorrectCopy)
            {
                isCorrectCopy.ToString();
            }

            return isCorrectCopy;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.MeasureAll();
        }
    }

    public abstract class Serialzier<T>
    {
        public abstract void Serialize(Stream stream, T data);

        public abstract T Deserialize(Stream stream);
    }

    public class JsonSerialzier<T> : Serialzier<T>
    {
        public override T Deserialize(Stream stream)
        {
            var seriazlier = new DataContractJsonSerializer(typeof(T));
            return (T)seriazlier.ReadObject(stream);
        }

        public override void Serialize(Stream stream, T data)
        {
            var seriazlier = new DataContractJsonSerializer(typeof(T));
            seriazlier.WriteObject(stream, data);
        }
    }

    public class NewtonsoftJsonSerialzier<T> : Serialzier<T>
    {
        public override T Deserialize(Stream stream)
        {
            var reader = new StreamReader(stream);
            var text = reader.ReadToEnd();
            return JsonConvert.DeserializeObject<T>(text);
        }

        public override void Serialize(Stream stream, T data)
        {
            var str = JsonConvert.SerializeObject(data);
            var writer = new StreamWriter(stream);
            writer.Write(str);
            writer.Flush();
        }
    }

    public class NewtonsoftBsonSerialzier<T> : Serialzier<T>
    {
        public override T Deserialize(Stream stream)
        {
            var reader = new BinaryReader(stream);
            var bsonReader = new Newtonsoft.Json.Bson.BsonReader(reader);
            bsonReader.ReadRootValueAsArray = true;
            var serializer = new Newtonsoft.Json.JsonSerializer();
            return (T)serializer.Deserialize(bsonReader, typeof(T));
        }

        public override void Serialize(Stream stream, T data)
        {
            var writer = new BinaryWriter(stream);
            var bsonWriter = new Newtonsoft.Json.Bson.BsonWriter(writer);
            var serializer = new Newtonsoft.Json.JsonSerializer();
            serializer.Serialize(bsonWriter, data, typeof(T));
            bsonWriter.Flush();
            writer.Flush();
        }
    }

    public class PolenterSerializer<T> : Serialzier<T>
    {
        public override T Deserialize(Stream stream)
        {
            var seriazlier = new SharpSerializer(true);
            return (T)seriazlier.Deserialize(stream);
        }

        public override void Serialize(Stream stream, T data)
        {
            var seriazlier = new SharpSerializer(true);
            seriazlier.Serialize(data, stream);
        }
    }

    public class NewSerializer<T> : Serialzier<T>
    {
        public override T Deserialize(Stream stream)
        {
            var serialzier = new BinarySerializing.BinaryFormatter(typeof(T));
            return (T)serialzier.Deserialze(stream);
        }

        public override void Serialize(Stream stream, T data)
        {
            var serialzier = new BinarySerializing.BinaryFormatter(typeof(T));
            serialzier.Seriazlie(stream, data);
        }
    }
}