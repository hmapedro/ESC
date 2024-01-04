using System;
using System.Collections.Generic;
using Xamarin.Forms;
using SportNow.Model;
using SportNow.Services.Data.JSON;
using System.Threading.Tasks;
using System.Diagnostics;
using SportNow.CustomViews;
using System.Collections.ObjectModel;
using Syncfusion.SfChart.XForms;
using SportNow.Model.Charts;
//Ausing Acr.UserDialogs;
using SportNow.Views.Profile;
using SportNow.ViewModel;
using Xamarin.Forms.Shapes;
using System.Drawing;

namespace SportNow.Views.Ranking
{
	public class RankingPageCS : DefaultPage
	{

		List<GlobalRanking> allRankings;
        List<GlobalRanking> currentRankings;

        CollectionView currentRankingCollectionView;
        Picker sportsPicker, categoriesPicker, yearsPicker;

        List<Competition_Category> competition_Categories;
        Label labelCurrentRanking, labelCurrentPoints;

        Ellipse circleRanking;
        TapGestureRecognizer rankingLabel_tap;


        string currentPosition, currentPoints;

        protected override void OnAppearing()
		{
            initSpecificLayout();
        }

		protected override void OnDisappearing()
		{
			this.CleanScreen();
            relativeLayout = new RelativeLayout
            {
                Margin = new Thickness(10)
            };
            Content = relativeLayout;
        }
	

		public void initLayout()
		{
			Title = "RANKING";

			var toolbarItem = new ToolbarItem
			{
				//Text = "Logout",
				IconImageSource = "perfil.png",

			};
			toolbarItem.Clicked += OnPerfilButtonClicked;
			ToolbarItems.Add(toolbarItem);
        }

		public void CleanScreen()
		{
			Debug.Print("CleanScreen");
            this.relativeLayout = null;
		}

		public async void initSpecificLayout()
		{
			CompetitionManager competitionManager = new CompetitionManager();

            //allRankings = await competitionManager.GetRankingDetail(App.member.id);
            //allRankings = await competitionManager.GetRankingDetail(App.member.id);

            createYearsPicker();
            _ = await createSportsTypePicker();
            _ = await createCategoriesPicker();

            allRankings = await competitionManager.GetRankingYearSportAll(yearsPicker.SelectedItem.ToString(), sportsPicker.SelectedItem.ToString(), categoriesPicker.SelectedItem.ToString());
            
            CreateCurrentRankingDetail();
            await refreshCurrentRankingDetailAsync();

        }


        public async void createYearsPicker()
        {
            int selectedIndex = 0;
            int selectedIndex_temp = 0;

            yearsPicker = new Picker
            {
                Title = "",
                TitleColor = Xamarin.Forms.Color.Black,
                BackgroundColor = Xamarin.Forms.Color.Transparent,
                TextColor = App.topColor,
                HorizontalTextAlignment = TextAlignment.Center,
                FontSize = App.formValueFontSize
            };

            int begin_year = 2022;
            int current_year = DateTime.Now.Year;


            List<string> yearsList = new List<string>();
            while (begin_year <= current_year)
            {
                yearsList.Add(begin_year.ToString());
                begin_year = begin_year + 1;
                selectedIndex = selectedIndex + 1;
            }


            yearsPicker.ItemsSource = yearsList;
            yearsPicker.SelectedIndex = selectedIndex;

            yearsPicker.SelectedIndexChanged += async (object sender, EventArgs e) =>
            {
                Debug.Print("sportsPicker selectedItem = " + sportsPicker.SelectedItem.ToString());

                Debug.Print("Update Years");
                await refreshCurrentRankingDetailAsync();
            };

            relativeLayout.Children.Add(yearsPicker,
                xConstraint: Constraint.Constant(0 * App.screenHeightAdapter),
                yConstraint: Constraint.Constant(0),
                widthConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return ((parent.Width / 10 * 1.5));
                }),
                heightConstraint: Constraint.Constant(40 * App.screenHeightAdapter));
        }

        public async Task<string> createSportsTypePicker()
        {
            int selectedIndex = 0;
            int selectedIndex_temp = 0;

            sportsPicker = new Picker
            {
                Title = "",
                TitleColor = Xamarin.Forms.Color.Black,
                BackgroundColor = Xamarin.Forms.Color.Transparent,
                TextColor = App.topColor,
                HorizontalTextAlignment = TextAlignment.Center,
                FontSize = App.gridTextFontSize

            };
            List<string> sportsList = new List<string>();
            foreach (var sport in Constants.sportsList)
            {
                sportsList.Add(sport.Value);
            }
            sportsPicker.ItemsSource = sportsList;
            sportsPicker.SelectedIndex = selectedIndex;
            CompetitionManager competitionManager = new CompetitionManager();
            competition_Categories = await competitionManager.GetCompetitionCategories_All(sportsPicker.SelectedItem.ToString());

            sportsPicker.SelectedIndexChanged += async (object sender, EventArgs e) =>
            {
                Debug.Print("sportsPicker selectedItem = " + sportsPicker.SelectedItem.ToString());
                //CompetitionManager competitionManager = new CompetitionManager();
                competition_Categories = await competitionManager.GetCompetitionCategories_All(sportsPicker.SelectedItem.ToString());

                List<string> categoriesList = new List<string>();
                foreach (Competition_Category category in competition_Categories)
                {
                    categoriesList.Add(category.category);
                }
                categoriesPicker.ItemsSource = categoriesList;
                categoriesPicker.SelectedIndex = 0;

                Debug.Print("Update Sports");
                //await refreshCurrentRankingDetailAsync();
            };

            relativeLayout.Children.Add(sportsPicker,
                xConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return ((parent.Width / 10 * 1.5));
                }),
                yConstraint: Constraint.Constant(0),
                widthConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return ((parent.Width / 10 * 4));
                }),
                heightConstraint: Constraint.Constant(40 * App.screenHeightAdapter));

            return "";
        }

        public async Task<string> createCategoriesPicker()
        {
            categoriesPicker = new Picker
            {
                Title = "",
                TitleColor = Xamarin.Forms.Color.Black,
                BackgroundColor = Xamarin.Forms.Color.Transparent,
                TextColor = App.topColor,
                HorizontalTextAlignment = TextAlignment.Center,
                FontSize = App.gridTextFontSize

            };
            List<string> categoriesList = new List<string>();

            int selectedIndex = -1;
            int selectedIndex_temp = 0;
            int age = Constants.GetAge(DateTime.Parse(App.member.birthdate));

            foreach (Competition_Category category in competition_Categories)
            {
                Debug.Print("Age = " + age + " category.idade_minima = " + category.idade_minima + " category.idade_maxima = " + category.idade_maxima);
                categoriesList.Add(category.category);
                if ((category.idade_minima <= age) & (category.idade_maxima >= age) & (selectedIndex == -1))
                {
                    if ((App.member.gender == "male") & (category.gender != "female"))
                    {
                        selectedIndex = selectedIndex_temp;
                    }
                    else if (App.member.gender == "female")
                    {
                        selectedIndex = selectedIndex_temp;
                    }

                    Debug.Print("selectedIndex = " + selectedIndex);
                }
                selectedIndex_temp++;
            }
            categoriesPicker.ItemsSource = categoriesList;
            categoriesPicker.SelectedIndex = selectedIndex;
            //pickBestCategory();

            categoriesPicker.SelectedIndexChanged += async (object sender, EventArgs e) =>
            {
                Debug.Print("sportsPicker selectedItem = " + sportsPicker.SelectedItem.ToString());
                Debug.Print("Update Categories");
                await refreshCurrentRankingDetailAsync();

                /*if (categoriesPicker.SelectedIndex != 0)
                {
                    
                }
                else
                {
                    currentPosition = "-";
                    currentPoints = "-";
                    labelCurrentRanking.Text = currentPosition;
                    labelCurrentPoints.Text = currentPoints + "\npontos";

                }*/
                

            };

            relativeLayout.Children.Add(categoriesPicker,
                xConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return ((parent.Width / 10 * 5.5));
                }),
                yConstraint: Constraint.Constant(0),
                widthConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return ((parent.Width / 10 * 4.5));
                }),
                heightConstraint: Constraint.Constant(40 * App.screenHeightAdapter));

            return "";
        }

        public void pickBestCategory()
        {
        }


        public void CreateCurrentRankingDetail()
		{

            createCurrentRankingCircle();
            createCurrentRankingCollection();
        }

        public void createCurrentRankingCircle()
        {
            Image ericeiraImage = new Image { Source = "logo_login.png", Aspect = Aspect.AspectFill, Opacity = 0.2 };
            relativeLayout.Children.Add(ericeiraImage,
                xConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return ((parent.Width / 2) - (75 * App.screenHeightAdapter)); // center of image (which is 40 wide)
                }),
                yConstraint: Constraint.Constant((135 * App.screenHeightAdapter)),
                widthConstraint: Constraint.Constant((150 * App.screenHeightAdapter)),
                heightConstraint: Constraint.Constant(50 * App.screenHeightAdapter));

            circleRanking = new Ellipse
            {
                //Stroke = App.topColor,
                StrokeThickness = 6,
                WidthRequest = 200 * App.screenHeightAdapter,
                HeightRequest = 200 * App.screenHeightAdapter,
                HorizontalOptions = LayoutOptions.Start
            };
            circleRanking.Stroke = new SolidColorBrush(App.topColor);

            rankingLabel_tap = new TapGestureRecognizer();
            rankingLabel_tap.Tapped += OnRankingPressed;

            labelCurrentRanking = new Label { BackgroundColor = Xamarin.Forms.Color.Transparent, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center, FontSize = App.rankingBigTitleFontSize, TextColor = App.topColor, LineBreakMode = LineBreakMode.WordWrap };
            labelCurrentRanking.Text = "";
            relativeLayout.Children.Add(labelCurrentRanking,
                xConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return ((parent.Width / 2) - (75 * App.screenHeightAdapter)); // center of image (which is 40 wide)
                }),
                yConstraint: Constraint.Constant((80 * App.screenHeightAdapter)),
                widthConstraint: Constraint.Constant((150 * App.screenHeightAdapter)),
                heightConstraint: Constraint.Constant(80 * App.screenHeightAdapter));

            labelCurrentPoints = new Label { BackgroundColor = Xamarin.Forms.Color.Transparent, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center, FontSize = App.bigTitleFontSize, TextColor = App.topColor, LineBreakMode = LineBreakMode.WordWrap };
            labelCurrentPoints.Text = "";
            relativeLayout.Children.Add(labelCurrentPoints,
                xConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return ((parent.Width / 2) - (75 * App.screenHeightAdapter)); // center of image (which is 40 wide)
                }),
                yConstraint: Constraint.Constant((160 * App.screenHeightAdapter)),
                widthConstraint: Constraint.Constant((150 * App.screenHeightAdapter)),
                heightConstraint: Constraint.Constant(80 * App.screenHeightAdapter));

            relativeLayout.Children.Add(circleRanking,
                xConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return ((parent.Width / 2) - (100 * App.screenHeightAdapter)); // center of image (which is 40 wide)
                }),
                yConstraint: Constraint.Constant((60 * App.screenHeightAdapter)),
                widthConstraint: Constraint.Constant((200 * App.screenHeightAdapter)),
                heightConstraint: Constraint.Constant(200 * App.screenHeightAdapter));
        }

        public async Task refreshCurrentRankingDetailAsync()
        {
            Debug.Print("refreshCurrentRankingDetailAsync");
            CompetitionManager competitionManager = new CompetitionManager();
            if (yearsPicker.SelectedItem == null)
            {
                Debug.Print("yearsPicker.SelectedItem == null");
            }
            if (sportsPicker.SelectedItem == null)
            {
                Debug.Print("sportsPicker.SelectedItem == null");
            }
            if (categoriesPicker.SelectedItem == null)
            {
                currentPosition = "-";
                currentPoints = "-";
                if (App.member.gender == "male")
                {
                    labelCurrentRanking.Text = currentPosition + "º";
                }
                else
                {
                    labelCurrentRanking.Text = currentPosition + "ª";
                }
                labelCurrentPoints.Text = currentPoints + "\npontos";
                currentRankingCollectionView.ItemsSource = null;

                circleRanking.GestureRecognizers.Remove(rankingLabel_tap);
            }
            else
            {

                allRankings = await competitionManager.GetRankingYearSportAll(yearsPicker.SelectedItem.ToString(), sportsPicker.SelectedItem.ToString(), categoriesPicker.SelectedItem.ToString());

                currentRankingCollectionView.ItemsSource = null;
                currentRankingCollectionView.ItemsSource = allRankings;

                //currentRankings = new List<GlobalRanking>();

                bool found = false;
                foreach (GlobalRanking globalRanking in allRankings)
                {
                    Debug.Print("globalRanking memberid = " + globalRanking.memberid);
                    Debug.Print("App.member.id = " + App.member.id);
                    if (globalRanking.memberid == App.member.id)
                    {
                        Debug.Print("globalRanking memberid = App.member.id " + globalRanking.ranking + " - " + globalRanking.pontos);
                        currentPosition = globalRanking.ranking;
                        currentPoints = globalRanking.pontos;
                        globalRanking.color = App.topColor;
                        found = true;
                    }
                    else
                    {
                        globalRanking.color = App.normalTextColor;
                    }
                }
                if (found == false)
                {
                    currentPosition = "-";
                    currentPoints = "-";
                    circleRanking.GestureRecognizers.Remove(rankingLabel_tap);
                }
                else
                {
                    circleRanking.GestureRecognizers.Add(rankingLabel_tap);
                }
                if (App.member.gender == "male")
                {
                    labelCurrentRanking.Text = currentPosition + "º";
                }
                else
                {
                    labelCurrentRanking.Text = currentPosition + "ª";
                }
                labelCurrentPoints.Text = currentPoints + "\npontos";

                currentRankingCollectionView.ItemsSource = allRankings;
            }
        }

        public void createCurrentRankingCollection()
        {

            //COLLECTION Classes
            currentRankingCollectionView = new CollectionView
            {
                SelectionMode = SelectionMode.Single,
                ItemsSource = allRankings,
                //HorizontalScrollBarVisibility = ScrollBarVisibility.Never,
                ItemsLayout = new GridItemsLayout(1, ItemsLayoutOrientation.Vertical) { VerticalItemSpacing = 5 * App.screenHeightAdapter, HorizontalItemSpacing = 10 * App.screenWidthAdapter, },
                EmptyView = new ContentView
                {
                    Content = new StackLayout
                    {
                        Children =
                            {
                                new Label { Text = "Este Ranking não existe.", HorizontalTextAlignment = TextAlignment.Center, TextColor = App.normalTextColor, FontSize = App.itemTitleFontSize },
                            }
                    }
                }
            };

            //currentRankingCollectionView.SelectionChanged += OnCompetitionParticipationCollectionViewSelectionChanged;
            //competitionParticipationsCollectionView.SelectionChanged += OnCompetitionParticipationsCollectionViewSelectionChanged;

            currentRankingCollectionView.ItemTemplate = new DataTemplate(() =>
            {
                RelativeLayout itemRelativeLayout = new RelativeLayout
                {
                    HeightRequest = 30
                };

                FormValue rankingLabel = new FormValue("");
                rankingLabel.label.HorizontalTextAlignment = TextAlignment.Center;
                rankingLabel.label.SetBinding(Label.TextProperty, "ranking");
                rankingLabel.label.SetBinding(Label.TextColorProperty, "color");


                itemRelativeLayout.Children.Add(rankingLabel,
                    xConstraint: Constraint.Constant(0),
                    yConstraint: Constraint.Constant(0),
                    widthConstraint: Constraint.Constant(30 * App.screenWidthAdapter),
                    heightConstraint: Constraint.Constant(30));

                FormValue nameLabel = new FormValue("");
                nameLabel.label.SetBinding(Label.TextProperty, "membername");
                nameLabel.label.SetBinding(Label.TextColorProperty, "color");


                itemRelativeLayout.Children.Add(nameLabel,
                    xConstraint: Constraint.Constant(35 * App.screenWidthAdapter),
                    yConstraint: Constraint.Constant(0),
                    widthConstraint: Constraint.RelativeToParent((parent) =>
                    {
                        return ((parent.Width) - (110 * App.screenWidthAdapter)); // center of image (which is 40 wide)
                    }),
                    heightConstraint: Constraint.Constant(30));


                FormValue pointsLabel = new FormValue("");
                pointsLabel.label.HorizontalTextAlignment = TextAlignment.Center;
                pointsLabel.label.SetBinding(Label.TextProperty, "pontos");
                pointsLabel.label.SetBinding(Label.TextColorProperty, "color");


                itemRelativeLayout.Children.Add(pointsLabel,
                    xConstraint: Constraint.RelativeToParent((parent) =>
                    {
                        return ((parent.Width) - (70 * App.screenWidthAdapter)); // center of image (which is 40 wide)
                    }),
                    yConstraint: Constraint.Constant(0),
                    widthConstraint: Constraint.Constant(60 * App.screenWidthAdapter),
                    heightConstraint: Constraint.Constant(30));

                return itemRelativeLayout;
            });

            relativeLayout.Children.Add(currentRankingCollectionView,
                xConstraint: Constraint.Constant(15 * App.screenHeightAdapter),
                yConstraint: Constraint.Constant((300 * App.screenHeightAdapter)),
                widthConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return (parent.Width - (30 * App.screenHeightAdapter)); // center of image (which is 40 wide)
                }),
                heightConstraint: Constraint.Constant(300 * App.screenHeightAdapter));
        }


        public RankingPageCS()
		{
			this.initLayout();
            
		}

		async void OnPerfilButtonClicked(object sender, EventArgs e)
		{
			await Navigation.PushAsync(new ProfilePageCS());
		}

        async void OnRankingPressed(object sender, EventArgs e)
        {
            Debug.Print("categoriesPicker.SelectedItem.ToString() = "+ categoriesPicker.SelectedItem.ToString());
            await Navigation.PushAsync(new RankingDetailPageCS(currentPosition, currentPoints, yearsPicker.SelectedItem.ToString(), sportsPicker.SelectedItem.ToString(), categoriesPicker.SelectedItem.ToString()));

        }
    }
}
