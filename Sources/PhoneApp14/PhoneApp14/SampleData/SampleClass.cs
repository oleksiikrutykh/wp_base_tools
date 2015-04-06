using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PhoneApp14.SampleData
{
    [DataContract]
    public class User
    {
        public User()
        {
        }

        [DataMember]
        public int Id { get; set; }



        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public List<Item> Items { get; set; }

        public int Empty { get; set; }

        [DataMember]
        public bool Test1 { get; set; }

        [DataMember]
        public byte Test2 { get; set; }

        [DataMember]
        public char Test3 { get; set; }

        [DataMember]
        public decimal Test4 { get; set; }

        [DataMember]
        public double Test5 { get; set; }

        [DataMember]
        public Int16 Test6 { get; set; }

        [DataMember]
        public Int32 Test7 { get; set; }

        [DataMember]
        public Int64 Test8 { get; set; }

        [DataMember]
        public SByte Test9 { get; set; }

        [DataMember]
        public Single Test10 { get; set; }

        [DataMember]
        public string Test11 { get; set; }

        [DataMember]
        public UInt16 Test12 { get; set; }

        [DataMember]
        public UInt32 Test13 { get; set; }

        [DataMember]
        public UInt64 Test14 { get; set; }

        [DataMember]
        public byte[] Test15 { get; set; }

        [DataMember]
        public int[][] Test16 { get; set; }

        [DataMember]
        public List<object> Test17 { get; set; }

        [DataMember]
        public object[] Test18 { get; set; }

        [DataMember]
        public object Test19 { get; set; }

        [DataMember]
        public Dictionary<string, Item> Test20 { get; set; }

        [DataMember]
        public SampleEnumInt Test21 { get; set; }

        [DataMember]
        public SampleEnumLong Test22 { get; set; }

        [DataMember]
        public int? Test23 { get; set; }

        [DataMember]
        public int? Test24 { get; set; }

        [DataMember]
        public A Test25 { get; set; }

        [DataMember]
        public MyList<string> Test26 { get; set; }

        //[DataMember(Name = "saf")]
        //public DateTime Test27 { get; set; }

        //[DataMember]
        //public DateTime Test28 { get; set; }

        [DataMember]
        public TimeSpan Test29 { get; set; }

        [DataMember]
        public Uri Test30 { get; set; }

        [DataMember]
        public Uri Test31 { get; set; }

        [DataMember]
        public Uri Test32 { get; set; }

        [OnDeserialized]
        public void OnDeserialized(StreamingContext context)
        {
            this.Name += this.Name;
        }

        //[OnDeserialized]
        //private void OnDeserialized1(StreamingContext context)
        //{
        //    this.Name += this.Id;
        //}
    }

    [DataContract]
    [KnownType(typeof(B))]
    public class A
    {
        
        public int Unvisible { get; set; }

        [DataMember]
        public int Visible1 { get; set; }

        [DataMember]
        public int SameName { get; set; }

        //[OnDeserialized]
        //private void OnDeserialized1(StreamingContext context)
        //{
        //    this.Visible1 += this.Visible1;
        //}
    }

    public class B : A
    {
        public int Visible2 { get; set; }

        //public int SameName { get; set; }
    }

    [DataContract]
    public class C : B 
    {
        [DataMember]
        public int Visible3 { get; set; }
    }
}
