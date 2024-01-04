using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace SportNow.Model
{
    public class Competition
    {
        public string id { get; set; }
        public string name { get; set; }
        public string date { get; set; }
        public string detailed_date { get; set; }
        public string place { get; set; }
        public string type { get; set; }
        public double value { get; set; }
        public string website { get; set; }
        public string imagemNome { get; set; }
        public string imagemSource { get; set; }
        public string registrationbegindate { get; set; }
        public string registrationlimitdate { get; set; }

        public List<Competition_Category> competitionCategories{ get; set; }

        public ObservableCollection<Competition_Participation> competitionParticipations { get; set; }

        public string participationid { get; set; }
        public string participationcategory { get; set; }
        public string participationclassification { get; set; }
        public string participationconfirmed { get; set; }
        public string participationimage { get; set; }

        public override string ToString()
        {
            return name;
        }

        public string competitionCategoriesString()
        {
            string result = competitionCategories[0].category;
            foreach (Competition_Category competition_Category in competitionCategories)
            {
                if (result.Contains(competition_Category.category) == false)
                {
                    result = result + ", "+ competition_Category.category;
                }

            }
            return result;
        }

        public List<string> competitionCategoriesListStrings()
        {
            List<string> result = new List<string>();
            result.Add(competitionCategories[0].category);
            
            foreach (Competition_Category competition_Category in competitionCategories)
            {
                if (result.Contains(competition_Category.category) == false)
                {
                    result.Add(competition_Category.category);
                }

            }
            return result;
        }

        public List<string> competitionCategoriesListStrings(int age, string gender, List<Competition_Participation> competition_Participations)
        {
            List<string> result = new List<string>();

            foreach (Competition_Category competition_Category in competitionCategories)
            {
                bool didbreak = false;
                foreach (Competition_Participation competition_Participation in competition_Participations)
                {
                    Debug.Print("competition_Participation.categoria == competition_Category.category " + competition_Participation.categoria + "== " + competition_Category.category);
                    if (competition_Participation.categoria == competition_Category.category)
                    {
                        didbreak = true;
                        break;
                    }

                }

                if (didbreak)
                {
                    continue;
                }

                if ((competition_Category.idade_minima <= age) & (competition_Category.idade_maxima >= age))
                {
                    Debug.Print("competition_Category.gender = " + competition_Category.gender);
                    if ((gender == "male") & (competition_Category.gender != "female"))
                    {
                        result.Add(competition_Category.category);
                    }
                    else if (gender == "female")
                    {
                        result.Add(competition_Category.category);
                    }
                    
                }

            }


            return result;
        }

        public string getIdbyName(string category)
        {
            string result = "0";
            foreach (Competition_Category competition_Category in competitionCategories)
            {
                if (competition_Category.category == category)
                {
                    return competition_Category.id;
                }

            }
            return result;
        }
    }
}
