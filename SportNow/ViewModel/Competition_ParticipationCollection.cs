using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using SportNow.Model;
using Xamarin.Forms;

namespace SportNow.ViewModel
{
    public class Competition_ParticipationCollection //: INotifyPropertyChanged
    {

        public ObservableCollection<Competition_Participation> Items { get; set; }

        public Competition_ParticipationCollection()
        {

        }

    }
}