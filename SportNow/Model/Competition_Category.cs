using System;
namespace SportNow.Model
{
    public class Competition_Category
    {
        public string id { get; set; }
        public string sport { get; set; }
        public string category { get; set; }
        public int idade_minima { get; set; }
        public int idade_maxima { get; set; }
        public string gender { get; set; }

        public Competition_Category()
        {
        }
    }
}
