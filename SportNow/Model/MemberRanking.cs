using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace SportNow.Model
{
    public class MemberRanking
    {
        public string id { get; set; }
        public string name { get; set; }
        public string competition_name { get; set; }
        public string competition_year { get; set; }
        public string competition_date { get; set; }
        public string category { get; set; }
        public string sport { get; set; }
        public string classification { get; set; }
        public string pontos { get; set; }


        public MemberRanking()
        {
        }
    }


}
