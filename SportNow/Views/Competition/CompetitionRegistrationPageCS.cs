using System;
using System.Collections.Generic;
using Xamarin.Forms;
using SportNow.Model;
using SportNow.Services.Data.JSON;
using System.Threading.Tasks;
using System.Diagnostics;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using SportNow.CustomViews;
using SportNow.ViewModel;
using System.Collections.ObjectModel;
using System.Reflection;

namespace SportNow.Views
{
	public class CompetitionRegistrationPageCS : DefaultPage
	{

		protected override void OnAppearing()
		{
			if (App.isToPop == true)
			{
				App.isToPop = false;
				Navigation.PopAsync();
			}
			
		}

		
		protected override void OnDisappearing()
		{
		}

		private Competition competition_v;

        public Xamarin.Forms.Picker escalaoPicker;

		RegisterButton registerButton;

        CollectionView competitionParticipationsCollectionView;
        ObservableCollection<Competition_Participation> competition_Participations_obs;
        Competition_ParticipationCollection competition_ParticipationCollection;

        double y_index = 0;

        public void initLayout()
		{
			Title = "INSCRIÇÃO";
		}


		public async void initSpecificLayout()
		{
            await createCurrentRegistrations();
            //createRegistrationOptions();
		}

		public async Task<string> createCurrentRegistrations()
		{
			CompetitionManager competitionManager = new CompetitionManager();
            competition_Participations_obs = await competitionManager.GetCompetitionParticipations_byCompetitionID(App.member.id, this.competition_v.id);

            CompleteCompetitionParticipation();
            competition_ParticipationCollection = new Competition_ParticipationCollection();
            competition_ParticipationCollection.Items = competition_Participations_obs;

            Label competitionParticipationsLabel = new Label
            {
                Text = "Escolha as Categorias em que se pretende inscrever:",
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                TextColor = App.normalTextColor,
                //LineBreakMode = LineBreakMode.NoWrap,
                FontSize = App.bigTitleFontSize
            };

            relativeLayout.Children.Add(competitionParticipationsLabel,
                xConstraint: Constraint.Constant(0),
                yConstraint: Constraint.Constant(10 * App.screenHeightAdapter),
                widthConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return (parent.Width - (20 * App.screenWidthAdapter)); // center of image (which is 40 wide)
                }),
                heightConstraint: Constraint.Constant(80 * App.screenHeightAdapter)
                );
            y_index = y_index + 90;

            createCompetitionParticipationCollection();

            return "";
        }

        public void CompleteCompetitionParticipation()
        {
            competition_Participations_obs = new ObservableCollection<Competition_Participation>();
            foreach (Competition_Category competition_Category in competition_v.competitionCategories)
            {
                Debug.Print("competition_Category = " + competition_Category.category);
                bool didbreak = false;
                foreach (Competition_Participation competition_Participation in competition_Participations_obs)
                {
                    if (competition_Participation.categoria == competition_Category.category)
                    {
                        if ((competition_Participation.estado == "inscrito") | (competition_Participation.estado == "confirmado"))
                        {
                            competition_Participation.color = App.topColor;
                        }
                        competition_Participations_obs.Add(competition_Participation);
                        didbreak = true;
                        break;
                    }
                }
                if (didbreak)
                {
                    continue;
                }



                Competition_Participation competition_Participation_new = new Competition_Participation();
                competition_Participation_new.categoria = competition_Category.category;
                competition_Participation_new.estado = "";
                competition_Participation_new.color = Color.Transparent;

                int age = Constants.GetAge(DateTime.Parse(App.member.birthdate));

                if ((competition_Category.idade_minima <= age) & (competition_Category.idade_maxima >= age))
                {
                    Debug.Print("competition_Category.gender = " + competition_Category.gender);
                    if ((App.member.gender == "male") & (competition_Category.gender != "female"))
                    {
                        competition_Participations_obs.Add(competition_Participation_new);
                    }
                    else if (App.member.gender == "female")
                    {
                        competition_Participations_obs.Add(competition_Participation_new);
                    }

                }   
            }

        }

        public void createCompetitionParticipationCollection()
        {

            //COLLECTION Classes
            competitionParticipationsCollectionView = new CollectionView
            {
                SelectionMode = SelectionMode.Single,
                //ItemsSource = competition_Participations,
                //HorizontalScrollBarVisibility = ScrollBarVisibility.Never,
                ItemsLayout = new GridItemsLayout(1, ItemsLayoutOrientation.Vertical) { VerticalItemSpacing = 5 * App.screenHeightAdapter, HorizontalItemSpacing = 10 * App.screenWidthAdapter, },
                EmptyView = new ContentView
                {
                    Content = new StackLayout
                    {
                        Children =
                            {
                                new Label { Text = "Não existem Categorias associadas a esta competição nas quais se possa inscrever.", HorizontalTextAlignment = TextAlignment.Start, TextColor = App.normalTextColor, FontSize = App.itemTitleFontSize },
                            }
                    }
                }
            };


            this.BindingContext = competition_ParticipationCollection;
            competitionParticipationsCollectionView.SetBinding(ItemsView.ItemsSourceProperty, "Items");
            competitionParticipationsCollectionView.SelectionChanged += OnCompetitionParticipationCollectionViewSelectionChanged;
            //competitionParticipationsCollectionView.SelectionChanged += OnCompetitionParticipationsCollectionViewSelectionChanged;

            competitionParticipationsCollectionView.ItemTemplate = new DataTemplate(() =>
            {
                RelativeLayout itemRelativeLayout = new RelativeLayout
                {
                    HeightRequest = 30
                };

                FormValue nameLabel = new FormValue("");
                nameLabel.label.SetBinding(Label.TextProperty, "categoria");


                itemRelativeLayout.Children.Add(nameLabel,
                    xConstraint: Constraint.Constant(0),
                    yConstraint: Constraint.Constant(0),
                    widthConstraint: Constraint.RelativeToParent((parent) =>
                    {
                        return (parent.Width - 50);
                    }),
                    heightConstraint: Constraint.Constant(30));

                Frame competitionParticipationStatus_Frame = new Frame()
                {
                    CornerRadius = 5,
                    IsClippedToBounds = true,
                    BorderColor = App.topColor,
                    Padding = new Thickness(2, 2, 2, 2),
                    HeightRequest = 30,
                    VerticalOptions = LayoutOptions.Center,
                    HasShadow = false
                };

                competitionParticipationStatus_Frame.SetBinding(Frame.BackgroundColorProperty, "color");


                itemRelativeLayout.Children.Add(competitionParticipationStatus_Frame,
                    xConstraint: Constraint.RelativeToParent((parent) =>
                    {
                        return (parent.Width - 40);
                    }),
                    yConstraint: Constraint.Constant(0),
                    widthConstraint: Constraint.Constant(30),
                    heightConstraint: Constraint.Constant(30));

                return itemRelativeLayout;
            });
            relativeLayout.Children.Add(competitionParticipationsCollectionView,
                xConstraint: Constraint.Constant(15 * App.screenHeightAdapter),
                yConstraint: Constraint.Constant((y_index * App.screenHeightAdapter)),
                widthConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return (parent.Width - (30 * App.screenHeightAdapter)); // center of image (which is 40 wide)
                }),
                heightConstraint: Constraint.Constant(300*App.screenHeightAdapter));

            y_index = y_index + 200;

            registerButton = new RegisterButton("CONFIRMAR INSCRIÇÃO", 100, 50);
            registerButton.BackgroundColor = App.topColor;
            registerButton.button.Clicked += OnRegisterButtonClicked;

            relativeLayout.Children.Add(registerButton,
                xConstraint: Constraint.Constant(10 * App.screenWidthAdapter),
                yConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return (parent.Height - (70 * App.screenHeightAdapter));
                }),
                widthConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return (parent.Width - (20 * App.screenWidthAdapter)); // center of image (which is 40 wide)
                }),
                heightConstraint: Constraint.Constant(50 * App.screenHeightAdapter)
            );

        }

		public CompetitionRegistrationPageCS(Competition competition_v)
		{

			this.competition_v = competition_v;

			//App.event_participation = event_participation;

			this.initLayout();
			this.initSpecificLayout();

		}


        async void OnCompetitionParticipationCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Debug.WriteLine("OnCompetitionParticipationCollectionViewSelectionChanged ");

            if ((sender as CollectionView).SelectedItem != null)
            {
                Competition_Participation competition_Participation_i = (sender as CollectionView).SelectedItem as Competition_Participation;

                //Debug.WriteLine("OnClassAttendanceCollectionViewSelectionChanged selected item = " + competition_Participation.classattendanceid);
                //class_attendance.statuschanged = true;
                if (competition_Participation_i.estado == "")
                {
                    competition_Participation_i.estado = "pendente";
                    competition_Participation_i.color = App.topColor;
                }
                else
                {
                    competition_Participation_i.estado = "";
                    competition_Participation_i.color = Color.Transparent;
                }

                competitionParticipationsCollectionView.SelectedItem = null;
            }
            else
            {
                Debug.WriteLine("OnCompetitionParticipationCollectionViewSelectionChanged selected item = nulll");
            }
        }

        async void OnRegisterButtonClicked(object sender, EventArgs e)
        {
            Debug.Print("OnRegisterButtonClicked");

            showActivityIndicator();
            registerButton.IsEnabled = false;
            CompetitionManager competitionManager = new CompetitionManager();
            string result = await competitionManager.Delete_Competition_Participations(App.member.id, this.competition_v.id);

            foreach (Competition_Participation competition_Participation in competition_Participations_obs)
            {

                Debug.Print("competition_Participation " + competition_Participation.categoria+ " " + competition_Participation.estado);

                if (competition_Participation.estado == "pendente")
                {
                    string competitionCategoryId = competition_v.getIdbyName(competition_Participation.categoria);

                    result = await competitionManager.Create_Competition_Participation(App.member.id, this.competition_v.id, competitionCategoryId, competition_Participation.estado);
                }
            }

            await Navigation.PushAsync(new CompetitionPaymentPageCS(this.competition_v));

            hideActivityIndicator();
            
            registerButton.IsEnabled = true;
        }
    }
}

