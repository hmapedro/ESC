using System;
namespace SportNow.Model
{
    public class Sponsor
    {
        public string id { get; set; }
        public string name { get; set; }
        public string image { get; set; }
        public string imageSource { get; set; }
        public string website { get; set; }

        public override string ToString()
        {
            return name;
        }

    }
}
