using System;
using System.Collections.Generic;
using Xamarin.Forms;
using SportNow.Model;
using SportNow.Services.Data.JSON;
using System.Threading.Tasks;
using System.Diagnostics;
using SportNow.CustomViews;
using SportNow.ViewModel;


namespace SportNow.Views
{
	public class CompetitionPaymentPageCS : DefaultPage
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


        private Payment payment;
        private Competition competition_v;

		private List<Payment> payments;

		private Grid gridPaymentOptions;

		double paymentTotal;

		public void initLayout()
		{
			Title = "INSCRIÇÃO";
		}


		public async void initSpecificLayout()
		{

            payments = await GetCompetitionParticipationPayment(this.competition_v);
			completePayments();
			payment = payments[0];
            if (payment.value == 0)
			{
				createRegistrationConfirmed();
			}
			else
			{
                createPaymentOptions();
			}
		}


		public void completePayments()
		{
			paymentTotal = 0;

            foreach (Payment payment in payments)
			{
				payment.valuestring = payment.value.ToString("0.00") + "€";
                paymentTotal = paymentTotal + payment.value;
            }
		}

		public async void createRegistrationConfirmed()
		{
			Label inscricaoOKLabel = new Label
			{
				Text = "A sua Inscrição na Competição " + competition_v.name + " está Confirmada. \n Boa sorte e nunca se esqueça de se divertir!",
				VerticalTextAlignment = TextAlignment.Center,
				HorizontalTextAlignment = TextAlignment.Center,
				TextColor = App.normalTextColor,
				//LineBreakMode = LineBreakMode.NoWrap,
				HeightRequest = 200,
				FontSize = 30
			};

			relativeLayout.Children.Add(inscricaoOKLabel,
				xConstraint: Constraint.Constant(0),
				yConstraint: Constraint.Constant(10),
				widthConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width); // center of image (which is 40 wide)
				}),
				heightConstraint: Constraint.Constant(200)
			);

			Image competitionImage = new Image { Aspect = Aspect.AspectFill, Opacity = 0.25 };
			competitionImage.Source = competition_v.imagemSource;

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

			CompetitionManager competitionManager = new CompetitionManager();

			await competitionManager.Update_Competition_Participation_Status(competition_v.participationid, "confirmado");
			competition_v.participationconfirmed = "confirmado";

		}

		public void createPaymentOptions() {

            Label competitionLabel = new Label
            {
                Text = competition_v.name,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                TextColor = App.topColor,
                //LineBreakMode = LineBreakMode.NoWrap,
                FontSize = App.bigTitleFontSize
            };

            relativeLayout.Children.Add(competitionLabel,
                xConstraint: Constraint.Constant(0),
                yConstraint: Constraint.Constant(0 * App.screenHeightAdapter),
                widthConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return (parent.Width - (20 * App.screenHeightAdapter)); // center of image (which is 40 wide)
                }),
                heightConstraint: Constraint.Constant(80 * App.screenHeightAdapter)
            );

            createCompetitionParticipationCollection();

            Label valorTotalLabel = new Label
            {
                Text = "VALOR TOTAL:" + paymentTotal.ToString("0.00") + "€",
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                TextColor = App.bottomColor,
                //LineBreakMode = LineBreakMode.NoWrap,
                FontSize = App.bigTitleFontSize
            };

            relativeLayout.Children.Add(valorTotalLabel,
                xConstraint: Constraint.Constant(0),
                yConstraint: Constraint.Constant(200 * App.screenHeightAdapter),
                widthConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return (parent.Width - (20 * App.screenHeightAdapter)); // center of image (which is 40 wide)
                }),
                heightConstraint: Constraint.Constant(60 * App.screenHeightAdapter)
            );

            Label selectPaymentModeLabel = new Label
			{
				Text = "Escolha o modo de pagamento pretendido:",
				VerticalTextAlignment = TextAlignment.Center,
				HorizontalTextAlignment = TextAlignment.Center,
				TextColor = App.normalTextColor,
				//LineBreakMode = LineBreakMode.NoWrap,
				FontSize = App.bigTitleFontSize
			};

			relativeLayout.Children.Add(selectPaymentModeLabel,
				xConstraint: Constraint.Constant(0),
				yConstraint: Constraint.Constant(280 * App.screenHeightAdapter),
				widthConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width - (20 * App.screenHeightAdapter)); // center of image (which is 40 wide)
				}),
				heightConstraint: Constraint.Constant(60 * App.screenHeightAdapter)
			);

			Image MBLogoImage = new Image
			{
				Source = "logomultibanco.png",
				MinimumHeightRequest = 115 * App.screenHeightAdapter,
				//WidthRequest = 100 * App.screenHeightAdapter,
				HeightRequest = 115 * App.screenHeightAdapter,
				//BackgroundColor = Color.Red,
			};

			var tapGestureRecognizerMB = new TapGestureRecognizer();
			tapGestureRecognizerMB.Tapped += OnMBButtonClicked;
			MBLogoImage.GestureRecognizers.Add(tapGestureRecognizerMB);

			relativeLayout.Children.Add(MBLogoImage,
				xConstraint: Constraint.Constant(0),
				yConstraint: Constraint.Constant(350 * App.screenHeightAdapter),
				widthConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width - (20 * App.screenHeightAdapter)); // center of image (which is 40 wide)
							}),
				heightConstraint: Constraint.Constant(115 * App.screenHeightAdapter)
			);


			Image MBWayLogoImage = new Image
			{
				Source = "logombway.png",
				//BackgroundColor = Color.Green,
				//WidthRequest = 184 * App.screenHeightAdapter,
				MinimumHeightRequest = 115 * App.screenHeightAdapter,
				HeightRequest = 115 * App.screenHeightAdapter
			};

			var tapGestureRecognizerMBWay = new TapGestureRecognizer();
			tapGestureRecognizerMBWay.Tapped += OnMBWayButtonClicked;
			MBWayLogoImage.GestureRecognizers.Add(tapGestureRecognizerMBWay);

			relativeLayout.Children.Add(MBWayLogoImage,
				xConstraint: Constraint.Constant(0),
				yConstraint: Constraint.Constant(500 * App.screenHeightAdapter),
				widthConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width - (20 * App.screenHeightAdapter)); // center of image (which is 40 wide)
							}),
				heightConstraint: Constraint.Constant(115 * App.screenHeightAdapter)
			);

			
        }


        public void createCompetitionParticipationCollection()
        {

            //COLLECTION Classes
            CollectionView competitionParticipationsCollectionView = new CollectionView
            {
                SelectionMode = SelectionMode.Single,
                ItemsSource = payments,
                //HorizontalScrollBarVisibility = ScrollBarVisibility.Never,
                ItemsLayout = new GridItemsLayout(1, ItemsLayoutOrientation.Vertical) { VerticalItemSpacing = 5 * App.screenHeightAdapter, HorizontalItemSpacing = 10 * App.screenWidthAdapter, },
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
                nameLabel.label.SetBinding(Label.TextProperty, "category");


                itemRelativeLayout.Children.Add(nameLabel,
                    xConstraint: Constraint.Constant(0),
                    yConstraint: Constraint.Constant(0),
                    widthConstraint: Constraint.RelativeToParent((parent) =>
                    {
                        return (parent.Width - 80);
                    }),
                    heightConstraint: Constraint.Constant(30));

                FormValue valueLabel = new FormValue("");
                valueLabel.label.SetBinding(Label.TextProperty, "valuestring");


                itemRelativeLayout.Children.Add(valueLabel,
                    xConstraint: Constraint.RelativeToParent((parent) =>
                    {
                        return (parent.Width - 70);
                    }),
                    yConstraint: Constraint.Constant(0),
                    widthConstraint: Constraint.Constant(60),
                    heightConstraint: Constraint.Constant(30));

                return itemRelativeLayout;
            });
            relativeLayout.Children.Add(competitionParticipationsCollectionView,
                xConstraint: Constraint.Constant(10 * App.screenHeightAdapter),
                yConstraint: Constraint.Constant((90 * App.screenHeightAdapter)),
                widthConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return (parent.Width - (20 * App.screenHeightAdapter)); // center of image (which is 40 wide)
                }),
                heightConstraint: Constraint.Constant(100 * App.screenHeightAdapter));
        }

        public CompetitionPaymentPageCS(Competition competition_v)
		{

			this.competition_v = competition_v;

			//App.event_participation = event_participation;

			this.initLayout();
			this.initSpecificLayout();

		}


        async Task<List<Payment>> GetCompetitionParticipationPayment(Competition competition)
        {
            Debug.WriteLine("GetCompetitionParticipationPayment");
            CompetitionManager competitionManager = new CompetitionManager();

            payments = await competitionManager.GetCompetitionParticipation_Payment(competition);
            if (payments == null)
            {
                Application.Current.MainPage = new NavigationPage(new LoginPageCS("Verifique a sua ligação à Internet e tente novamente."))
                {
                    BarBackgroundColor = Color.FromRgb(15, 15, 15),
                    BarTextColor = Color.White
                };
                return null;
            }
            return payments;
        }

        async void OnMBButtonClicked(object sender, EventArgs e)
		{
			await Navigation.PushAsync(new CompetitionMBPageCS(this.competition_v));
		}


		async void OnMBWayButtonClicked(object sender, EventArgs e)
		{
			Debug.Print("OnMBWayButtonClicked");
			await Navigation.PushAsync(new CompetitionMBWayPageCS(this.competition_v));
		}

	}
}

