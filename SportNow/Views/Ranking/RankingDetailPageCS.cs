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
using static System.Net.Mime.MediaTypeNames;

namespace SportNow.Views.Ranking
{
	public class RankingDetailPageCS : DefaultPage
	{

		List<MemberRanking> memberRankings;

        CollectionView currentRankingCollectionView;
        string sport, category, year;

        List<Competition_Category> competition_Categories;
        Label labelCurrentRanking, labelCurrentPoints;

        Ellipse circleRanking;

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
			Title = "DETALHE RANKING";
        }


		public void CleanScreen()
		{
			Debug.Print("CleanScreen");
            this.relativeLayout = null;
		}

		public async void initSpecificLayout()
		{
			CompetitionManager competitionManager = new CompetitionManager();

            memberRankings = await competitionManager.GetRankingDetail(App.member.id, year, sport, category);
            createCurrentRankingCircle();
            createCurrentRankingCollection();

        }


        public void createCurrentRankingCircle()
        {
            Label rankingLabel = new Label
            {
                Text = year + "\n"+ category,
                TextColor = App.topColor,
                HorizontalTextAlignment = TextAlignment.Center,
                FontSize = (22 * App.screenHeightAdapter)
            };

            relativeLayout.Children.Add(rankingLabel,
            xConstraint: Constraint.Constant(0 * App.screenHeightAdapter),
            yConstraint: Constraint.Constant(20 * App.screenHeightAdapter),
            widthConstraint: Constraint.RelativeToParent((parent) =>
            {
                return (parent.Width);
            }),
            heightConstraint: Constraint.Constant(80 * App.screenHeightAdapter));


            Xamarin.Forms.Image ericeiraImage = new Xamarin.Forms.Image { Source = "logo_login.png", Aspect = Aspect.AspectFill, Opacity = 0.2 };
            relativeLayout.Children.Add(ericeiraImage,
                xConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return ((parent.Width / 2) - (75 * App.screenHeightAdapter)); // center of image (which is 40 wide)
                }),
                yConstraint: Constraint.Constant((175 * App.screenHeightAdapter)),
                widthConstraint: Constraint.Constant((150 * App.screenHeightAdapter)),
                heightConstraint: Constraint.Constant(50 * App.screenHeightAdapter));


            circleRanking = new Ellipse
            {
                Stroke = App.topColor,
                StrokeThickness = 6,
                WidthRequest = 200 * App.screenHeightAdapter,
                HeightRequest = 200 * App.screenHeightAdapter,
                HorizontalOptions = LayoutOptions.Start
            };

            labelCurrentRanking = new Label { BackgroundColor = Color.Transparent, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center, FontSize = App.rankingBigTitleFontSize, TextColor = App.topColor, LineBreakMode = LineBreakMode.WordWrap };
            labelCurrentRanking.Text = "";
            relativeLayout.Children.Add(labelCurrentRanking,
                xConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return ((parent.Width / 2) - (75 * App.screenHeightAdapter)); // center of image (which is 40 wide)
                }),
                yConstraint: Constraint.Constant((120 * App.screenHeightAdapter)),
                widthConstraint: Constraint.Constant((150 * App.screenHeightAdapter)),
                heightConstraint: Constraint.Constant(80 * App.screenHeightAdapter));


            labelCurrentPoints = new Label { BackgroundColor = Color.Transparent, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center, FontSize = App.bigTitleFontSize, TextColor = App.topColor, LineBreakMode = LineBreakMode.WordWrap };
            labelCurrentPoints.Text = "";
            relativeLayout.Children.Add(labelCurrentPoints,
                xConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return ((parent.Width / 2) - (75 * App.screenHeightAdapter)); // center of image (which is 40 wide)
                }),
                yConstraint: Constraint.Constant((200 * App.screenHeightAdapter)),
                widthConstraint: Constraint.Constant((150 * App.screenHeightAdapter)),
                heightConstraint: Constraint.Constant(80 * App.screenHeightAdapter));

            relativeLayout.Children.Add(circleRanking,
                xConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return ((parent.Width / 2) - (100 * App.screenHeightAdapter)); // center of image (which is 40 wide)
                }),
                yConstraint: Constraint.Constant((100 * App.screenHeightAdapter)),
                widthConstraint: Constraint.Constant((200 * App.screenHeightAdapter)),
                heightConstraint: Constraint.Constant(200 * App.screenHeightAdapter));

            if (App.member.gender == "male")
            {
                labelCurrentRanking.Text = currentPosition + "º";
            }
            else
            {
                labelCurrentRanking.Text = currentPosition + "ª";
            }
            labelCurrentPoints.Text = currentPoints + "\npontos";

        }

        public void createCurrentRankingCollection()
        {

            foreach (MemberRanking memberRanking in memberRankings)
            {
                if (App.member.gender == "male")
                {
                    memberRanking.classification = memberRanking.classification + "º";
                }
                else
                {
                    memberRanking.classification = memberRanking.classification + "ª";
                }
                //memberRanking.pontos = memberRanking.pontos + "";

            }

            //COLLECTION Classes
            currentRankingCollectionView = new CollectionView
            {
                SelectionMode = SelectionMode.Single,
                ItemsSource = memberRankings,
                //HorizontalScrollBarVisibility = ScrollBarVisibility.Never,
                ItemsLayout = new GridItemsLayout(1, ItemsLayoutOrientation.Vertical) { VerticalItemSpacing = 10 * App.screenHeightAdapter, HorizontalItemSpacing = 10 * App.screenWidthAdapter, },
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
                    HeightRequest = 160
                };

                Label competiton_nameLabel = new Label { BackgroundColor = Color.Transparent, VerticalTextAlignment = TextAlignment.End, HorizontalTextAlignment = TextAlignment.Center, FontSize = App.rankingTitleFontSize, TextColor = App.bottomColor, LineBreakMode = LineBreakMode.WordWrap };

                competiton_nameLabel.SetBinding(Label.TextProperty, "competition_name");

                itemRelativeLayout.Children.Add(competiton_nameLabel,
                    xConstraint: Constraint.Constant(0),
                    yConstraint: Constraint.Constant(0),
                    widthConstraint: Constraint.RelativeToParent((parent) =>
                    {
                        return (parent.Width); // center of image (which is 40 wide)
                    }),
                    heightConstraint: Constraint.Constant(70));

                Label competiton_dateLabel = new Label { BackgroundColor = Color.Transparent, VerticalTextAlignment = TextAlignment.Start, HorizontalTextAlignment = TextAlignment.Center, FontSize = App.itemTitleFontSize, TextColor = App.bottomColor, LineBreakMode = LineBreakMode.WordWrap };

                competiton_dateLabel.SetBinding(Label.TextProperty, "competition_date");

                itemRelativeLayout.Children.Add(competiton_dateLabel,
                    xConstraint: Constraint.Constant(0),
                    yConstraint: Constraint.Constant(70),
                    widthConstraint: Constraint.RelativeToParent((parent) =>
                    {
                        return (parent.Width); // center of image (which is 40 wide)
                    }),
                    heightConstraint: Constraint.Constant(30));


                Label classificationLabel = new Label { BackgroundColor = Color.Transparent, VerticalTextAlignment = TextAlignment.Start, HorizontalTextAlignment = TextAlignment.Center, FontSize = App.rankingTitleFontSize, TextColor = App.topColor, LineBreakMode = LineBreakMode.WordWrap };

                classificationLabel.SetBinding(Label.TextProperty, "classification");

                itemRelativeLayout.Children.Add(classificationLabel,
                    xConstraint: Constraint.Constant(0),
                    yConstraint: Constraint.Constant(100),
                    widthConstraint: Constraint.RelativeToParent((parent) =>
                    {
                        return (parent.Width)/2; // center of image (which is 40 wide)
                    }),
                    heightConstraint: Constraint.Constant(40));



                Label classificationTextLabel = new Label { Text = "classificado", BackgroundColor = Color.Transparent, VerticalTextAlignment = TextAlignment.Start, HorizontalTextAlignment = TextAlignment.Center, FontSize = App.itemTextFontSize, TextColor = App.topColor, LineBreakMode = LineBreakMode.WordWrap }; ;
                if (App.member.gender == "female")
                {
                    classificationTextLabel.Text = "classificada"; 
                }



                itemRelativeLayout.Children.Add(classificationTextLabel,
                    xConstraint: Constraint.Constant(0),
                    yConstraint: Constraint.Constant(140),
                    widthConstraint: Constraint.RelativeToParent((parent) =>
                    {
                        return (parent.Width) / 2; // center of image (which is 40 wide)
                    }),
                    heightConstraint: Constraint.Constant(20));

                Label pointsLabel = new Label { BackgroundColor = Color.Transparent, VerticalTextAlignment = TextAlignment.Start, HorizontalTextAlignment = TextAlignment.Center, FontSize = App.rankingTitleFontSize, TextColor = App.topColor, LineBreakMode = LineBreakMode.WordWrap };

                pointsLabel.SetBinding(Label.TextProperty, "pontos");

                itemRelativeLayout.Children.Add(pointsLabel,
                    xConstraint: Constraint.RelativeToParent((parent) =>
                    {
                        return (parent.Width) / 2; // center of image (which is 40 wide)
                    }),
                    yConstraint: Constraint.Constant(100),
                    widthConstraint: Constraint.RelativeToParent((parent) =>
                    {
                        return (parent.Width) / 2; // center of image (which is 40 wide)
                    }),
                    heightConstraint: Constraint.Constant(40));

                Label pointsTextLabel = new Label { Text = "pontos", BackgroundColor = Color.Transparent, VerticalTextAlignment = TextAlignment.Start, HorizontalTextAlignment = TextAlignment.Center, FontSize = App.itemTextFontSize, TextColor = App.topColor, LineBreakMode = LineBreakMode.WordWrap };


                itemRelativeLayout.Children.Add(pointsTextLabel,
                    xConstraint: Constraint.RelativeToParent((parent) =>
                    {
                        return (parent.Width) / 2; // center of image (which is 40 wide)
                    }),
                    yConstraint: Constraint.Constant(140),
                    widthConstraint: Constraint.RelativeToParent((parent) =>
                    {
                        return (parent.Width) / 2; // center of image (which is 40 wide)
                    }),
                    heightConstraint: Constraint.Constant(20));

                return itemRelativeLayout;
            });

            relativeLayout.Children.Add(currentRankingCollectionView,
                xConstraint: Constraint.Constant(15 * App.screenHeightAdapter),
                yConstraint: Constraint.Constant((300 * App.screenHeightAdapter)),
                widthConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return (parent.Width - (30 * App.screenHeightAdapter)); // center of image (which is 40 wide)
                }),
                heightConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return (parent.Height - (300 * App.screenHeightAdapter)); // center of image (which is 40 wide)
                }));
        }


        public RankingDetailPageCS(string currentPosition, string currentPoints, string year, string sport, string category)
		{
            this.currentPosition = currentPosition;
            this.currentPoints = currentPoints;

            this.year = year;
            this.sport = sport;
            this.category = category;
            this.initLayout();
            
		}
	}
}
