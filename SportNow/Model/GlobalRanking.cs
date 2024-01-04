using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace SportNow.Model
{
    public class GlobalRanking
    {
        public string ranking { get; set; }
        public string memberid { get; set; }
        public string membername { get; set; }
        public string competition_year { get; set; }
        public string sport { get; set; }
        public string category { get; set; }
        public string pontos { get; set; }
        public Color color { get; set; }

        public GlobalRanking()
        {
        }
    }


}
