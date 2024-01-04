using System;
using System.Collections.Generic;
using Xamarin.Forms;
using SportNow.Model;
using SportNow.Services.Data.JSON;
using System.Threading.Tasks;
using System.Diagnostics;
using SportNow.CustomViews;
using Xamarin.Essentials;
using SportNow.ViewModel;
using System.Collections.ObjectModel;

namespace SportNow.Views
{
	public class DetailCompetitionPageCS : DefaultPage
	{

		protected override void OnAppearing()
		{
			refreshCompetitionStatus(competition);
			/*competition_participation = App.competition_participation;

			if (competition_participation != null) { 

				Debug.Print("competition_participation.estado=" + competition_participation.estado);

				if ((competition_participation.estado == "confirmado") & (competition.participationconfirmed != 1))
				{
					competition.participationconfirmed = 1;		
				}
			}*/
			//initSpecificLayout();
		}

		protected override void OnDisappearing()
		{
			Debug.Print("OnDisappearing");
			CleanScreen();
		}


		FormValue estadoValue = new FormValue("");



		private Competition competition;
		private List<Competition> competitions;

		private List<Payment> payments;

		RegisterButton registerButton;
		Image competitionImage;

        ObservableCollection<Competition_Participation> competition_Participations_obs;


        private Grid gridCompetiton;
		CollectionView competitionParticipationsCollectionView;


        public void initLayout()
		{
            Title = competition.name;

			relativeLayout = new RelativeLayout
			{
				Margin = new Thickness(0)
			};
			Content = relativeLayout;

			NavigationPage.SetBackButtonTitle(this, "");
		}


		public void CleanScreen()
		{
			if (gridCompetiton != null)
			{
				relativeLayout.Children.Remove(gridCompetiton);
				gridCompetiton = null;
			}
			if (registerButton != null)
            {
				relativeLayout.Children.Remove(registerButton);
				registerButton = null;
			}

            if (competitionImage != null)
            {
                relativeLayout.Children.Remove(competitionImage);
                competitionImage = null;
            }
        }

		public async void initSpecificLayout()
		{

            competitionImage = new Image { Aspect = Aspect.AspectFill, Opacity = 0.15, Source = competition.imagemSource };

            relativeLayout.Children.Add(competitionImage,
                xConstraint: Constraint.Constant(0),
                yConstraint: Constraint.Constant(0),
                widthConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return (parent.Width); // center of image (which is 40 wide)
                }),
                heightConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return (parent.Height); // center of image (which is 40 wide)
                })
            );

            gridCompetiton = new Grid { Padding = 0, HorizontalOptions = LayoutOptions.FillAndExpand };
			gridCompetiton.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridCompetiton.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridCompetiton.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridCompetiton.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            gridCompetiton.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            gridCompetiton.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
            //gridCompetiton.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            gridCompetiton.RowDefinitions.Add(new RowDefinition { Height = 80 });
			gridCompetiton.RowDefinitions.Add(new RowDefinition { Height = 120 });
			//gridCompetiton.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star});
			//gridGeral.RowDefinitions.Add(new RowDefinition { Height = 1 });
			gridCompetiton.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto }); //GridLength.Auto
			gridCompetiton.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star }); //GridLength.Auto 


            relativeLayout.Children.Add(gridCompetiton,
                xConstraint: Constraint.Constant(10 * App.screenWidthAdapter),
                yConstraint: Constraint.Constant(10 * App.screenHeightAdapter),
                widthConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return (parent.Width - 20 * App.screenWidthAdapter); // center of image (which is 40 wide)
                }),
                heightConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return (parent.Height - 60 * App.screenHeightAdapter); // center of image (which is 40 wide)
                })
            );

            FormLabel dateLabel = new FormLabel { Text = "DATA" };
			FormValue dateValue = new FormValue(competition.detailed_date);

			FormLabel placeLabel = new FormLabel { Text = "LOCAL" };
			FormValue placeValue = new FormValue(competition.place);

            /*FormLabel typeLabel = new FormLabel { Text = "TIPO" };
			FormValue typeValue = new FormValue(Constants.competition_type[competition.type]);*/

           // FormLabel escaloesLabel = new FormLabel { Text = "ESCALÕES" };
		//	FormValue escaloesValue = new FormValue(this.competition.competitionCategoriesString());

			//FormLabel valueLabel = new FormLabel { Text = "VALOR" };
            //FormValue valueValue = new FormValue(String.Format("{0:0.00}", competition.value + "€"));

            FormLabel websiteLabel = new FormLabel { Text = "WEBSITE" };
			FormValue websiteValue = new FormValue(competition.website);

			websiteValue.GestureRecognizers.Add(new TapGestureRecognizer
			{
				Command = new Command(async () => {
					try
					{
						await Browser.OpenAsync(competition.website, BrowserLaunchMode.SystemPreferred);
					}
					catch (Exception ex)
					{
						// An unexpected error occured. No browser may be installed on the device.
					}
				})
			});

			FormLabel estadoLabel = new FormLabel { Text = "ESTADO" }; ;
			estadoValue = new FormValue("");

			//List<Competition_Participation> competitionCall = await GetCompetitionCall();

			Debug.Print("competition.registrationbegindate = " + competition.registrationbegindate);
			DateTime currentTime = DateTime.Now.Date;
			DateTime registrationbegindate_datetime = new DateTime();
			DateTime registrationlimitdate_datetime = new DateTime();


			if ((competition.registrationbegindate != "") & (competition.registrationbegindate != null))
			{
				registrationbegindate_datetime = DateTime.Parse(competition.registrationbegindate).Date;
			}
			if ((competition.registrationlimitdate != "") & (competition.registrationlimitdate != null))
			{
				registrationlimitdate_datetime = DateTime.Parse(competition.registrationlimitdate).Date;
			}

			int registrationOpened = -1;
			string limitDateLabelText = "";

			if ((competition.registrationbegindate == "") | (competition.registrationbegindate == null))
			{
				Debug.Print("Data início de inscrições ainda não está definida");
				limitDateLabelText = "As inscrições ainda não estão abertas.";
			}
			else if ((currentTime - registrationbegindate_datetime).Days < 0)
			{
				Debug.Print("Inscrições ainda não abriram");
				limitDateLabelText = "As inscrições abrem no dia " + competition.registrationbegindate + ".";
			}
			else
			{

				Debug.Print("Inscrições já abriram " + (registrationlimitdate_datetime - currentTime).Days);
				if ((registrationlimitdate_datetime - currentTime).Days < 0)
				{
					Debug.Print("Inscrições já fecharam");
					limitDateLabelText = "Ohhh...As inscrições já terminaram.";
					registrationOpened = 0;
				}
				else
				{
					registrationOpened = 1;
					Debug.Print("Inscrições estão abertas!");
					limitDateLabelText = "As inscrições estão abertas e terminam no dia " + competition.registrationlimitdate + ".";
				}
			}

			if ((competition.participationconfirmed == "inscrito") | (competition.participationconfirmed == "confirmado"))

            {
				estadoValue = new FormValue("INSCRITO");
				estadoValue.label.TextColor = Color.FromRgb(96, 182, 89);
				limitDateLabelText = "BOA SORTE!";
			}
			else //if (competition.participationconfirmed == "convocado")
			{
				if (registrationOpened == 1) {
                    estadoValue = new FormValue("NÃO INSCRITO");
                    estadoValue.label.TextColor = Color.Red;
                    registerButton = new RegisterButton("INSCREVER", 100, 50);
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
				else
				{
                    estadoValue = new FormValue("-");
                    estadoValue.label.TextColor = App.normalTextColor;
                }
				
			}
			/*else if (competition.participationconfirmed == null)
			{
				if (competitionCall == null)
				{
					estadoValue = new FormValue("-");
				}
				else if (competitionCall.Count == 0)
				{
					estadoValue = new FormValue("-");
				}
				else
				{
					estadoValue = new FormValue("NÃO INSCRITO");
				}
				estadoValue.label.TextColor = Color.White;
			}*/


			Label limitDateLabel = new Label
			{
				Text = limitDateLabelText,
				TextColor = App.topColor,
				WidthRequest = 200,
				HeightRequest = 30,
				FontSize = 20,
				HorizontalTextAlignment = TextAlignment.Center
			};

			gridCompetiton.Children.Add(dateLabel, 0, 0);
			gridCompetiton.Children.Add(dateValue, 1, 0);

			gridCompetiton.Children.Add(placeLabel, 0, 1);
			gridCompetiton.Children.Add(placeValue, 1, 1);

			/*gridCompetiton.Children.Add(typeLabel, 0, 2);
			gridCompetiton.Children.Add(typeValue, 1, 2);*/

			gridCompetiton.Children.Add(websiteLabel, 0, 2);
			gridCompetiton.Children.Add(websiteValue, 1, 2);

			gridCompetiton.Children.Add(estadoLabel, 0, 3);
			gridCompetiton.Children.Add(estadoValue, 1, 3);


            Label categoriesLabel = new Label
            {
                Text = "CATEGORIAS",
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                TextColor = App.normalTextColor,
                //LineBreakMode = LineBreakMode.NoWrap,
                FontSize = App.bigTitleFontSize
            };
            gridCompetiton.Children.Add(categoriesLabel, 0, 4);
            Grid.SetColumnSpan(categoriesLabel, 2);

            CompetitionManager competitionManager = new CompetitionManager();
            competition.competitionParticipations = await competitionManager.GetCompetitionParticipations_byCompetitionID(App.member.id, this.competition.id);

            CompleteCompetitionParticipation();
            createCompetitionParticipationCollection();

            gridCompetiton.Children.Add(competitionParticipationsCollectionView, 0, 5);
            Grid.SetColumnSpan(competitionParticipationsCollectionView, 2);

			gridCompetiton.Children.Add(limitDateLabel, 0, 6);
            Grid.SetColumnSpan(limitDateLabel, 2);


        }


        public void CompleteCompetitionParticipation()
        {
            competition_Participations_obs = new ObservableCollection<Competition_Participation>();
            foreach (Competition_Category competition_Category in competition.competitionCategories)
            {
				Debug.Print("competition_Category = " + competition_Category.category);
                bool didbreak = false;
                foreach (Competition_Participation competition_Participation in competition.competitionParticipations)
                {
                    Debug.Print("Competition Participation = " + competition_Participation.name);
                    if (competition_Participation.categoria == competition_Category.category)
                    {
						if ((competition_Participation.estado == "inscrito") | (competition_Participation.estado == "confirmado"))
						{
							competition_Participation.color = App.topColor;
							competition_Participation.estadoTextColor = Color.White;
						}
						else
                        {
                            competition_Participation.color = Color.Yellow;
                            competition_Participation.estadoTextColor = App.normalTextColor;
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
                competition_Participation_new.estadoTextColor = App.normalTextColor;
                competition_Participations_obs.Add(competition_Participation_new);
				/*
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

                }*/
            }

        }

        public void createCompetitionParticipationCollection()
        {


			//COLLECTION Classes
			competitionParticipationsCollectionView = new CollectionView
			{
				SelectionMode = SelectionMode.None,
				ItemsSource = competition_Participations_obs,// competition.competitionCategories,
                //HorizontalScrollBarVisibility = ScrollBarVisibility.Never,
                ItemsLayout = new GridItemsLayout(2, ItemsLayoutOrientation.Vertical) { VerticalItemSpacing = 5 * App.screenHeightAdapter, HorizontalItemSpacing = 5 * App.screenWidthAdapter, },
                EmptyView = new ContentView
                {
                    Content = new StackLayout
                    {
                        Children =
                            {
                                new Label { Text = "Não existem escalões.", HorizontalTextAlignment = TextAlignment.Start, TextColor = Color.White, FontSize = App.itemTitleFontSize },
                            }
                    }
                }
            };

            competitionParticipationsCollectionView.ItemTemplate = new DataTemplate(() =>
            {
                RelativeLayout itemRelativeLayout = new RelativeLayout
                {
                    HeightRequest = 30
                };

                FormValue nameLabel = new FormValue("");

                nameLabel.label.SetBinding(Label.TextProperty, "categoria");
                nameLabel.SetBinding(Frame.BackgroundColorProperty, "color");
				nameLabel.label.SetBinding(Label.TextColorProperty, "estadoTextColor");



                itemRelativeLayout.Children.Add(nameLabel,
                    xConstraint: Constraint.Constant(0),
                    yConstraint: Constraint.Constant(0),
                    widthConstraint: Constraint.RelativeToParent((parent) =>
                    {
                        return (parent.Width - 5 * App.screenWidthAdapter);
                    }),
                    heightConstraint: Constraint.Constant(30));

                return itemRelativeLayout;
            });
        }



        public DetailCompetitionPageCS(Competition competition)
		{
			this.competition = competition;
			//Debug.Print("AQUI 2 competition ImageSource = " + competition.imagemSource);
			this.initLayout();
			//this.initSpecificLayout();
		}

		public DetailCompetitionPageCS(string competitionId)
		{
			this.competition = new Competition();
			competition.id = competitionId;
            this.initLayout();
			//this.initSpecificLayout();
		}


		async void refreshCompetitionStatus(Competition competition_v)
		{
			if (competition_v != null)
			{
				CompetitionManager competitionManager = new CompetitionManager();
				if (competition_v.participationid != null)

				{
					competitions = await competitionManager.GetCompetitionByParticipationID(App.member.id, competition.participationid);
					this.competition = competitions[0];

				}
				else
				{
					competitions = await competitionManager.GetCompetitionByID(App.member.id, competition.id);
					this.competition = competitions[0];
				}

				if ((this.competition.imagemNome == "") | (this.competition.imagemNome is null))
				{
					this.competition.imagemSource = "logo_login.png";
				}
				else
				{
					this.competition.imagemSource = Constants.images_URL + competition.id + "_imagem_c";
				}

                this.competition.competitionCategories = await competitionManager.GetCompetitionCategories(this.competition.id);
				

            }
            Title = competition.name;
            initSpecificLayout();
		}

		async Task<List<Competition_Participation>> GetCompetitionCall()
		{
			CompetitionManager competitionManager = new CompetitionManager();

			List<Competition_Participation> futureCompetitionParticipations = await competitionManager.GetCompetitionCall(competition.id);
			if (futureCompetitionParticipations == null)
			{
				Application.Current.MainPage = new NavigationPage(new LoginPageCS("Verifique a sua ligação à Internet e tente novamente."))
				{
					BarBackgroundColor = Color.White,
					BarTextColor = Color.Black
				};
				return null;
			}
			return futureCompetitionParticipations;
		}

		async void OnRegisterButtonClicked(object sender, EventArgs e)
		{

			registerButton.IsEnabled = false;

			await Navigation.PushAsync(new CompetitionRegistrationPageCS(competition));


		}
	}
}

