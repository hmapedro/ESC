using System;
using Xamarin.Forms;
using SportNow.Model;
using System.Diagnostics;
using SportNow.CustomViews;
using System.Net;
using SportNow.Services.Data.JSON;
using System.Collections.Generic;
//Ausing Acr.UserDialogs;
using System.Globalization;
using System.Threading.Tasks;

namespace SportNow.Views.CompleteRegistration
{
	public class CompleteRegistration_Payment_PageCS : DefaultPage
	{

		protected async override void OnAppearing()
		{
		}


		protected async override void OnDisappearing()
		{

		}


		private ScrollView scrollView;

		//private Member member;


		FormValue valueQuota;
		double valorQuota;
		string paymentID;
		Payment payment;

		bool paymentDetected;

		public void initLayout()
		{
			Title = "PAGAMENTO";

			relativeLayout = new RelativeLayout
			{
				Margin = new Thickness(10)
			};
			Content = relativeLayout;

		}


		public async void initSpecificLayout()
		{
			showActivityIndicator();
			MemberManager memberManager = new MemberManager();
			string season = DateTime.Now.ToString("yyyy");

            /*
			if (DateTime.Now.Month >= 8)
			{
				season = DateTime.Now.ToString("yyyy") + "_" + DateTime.Now.AddYears(1).ToString("yyyy");
			}
			else
			{
				season = DateTime.Now.AddYears(-1).ToString("yyyy") + "_" + DateTime.Now.ToString("yyyy");
			}*/


            paymentID = await memberManager.CreateAllFees(App.original_member.id, App.member.id, season);
			List<Fee> allFees = await memberManager.GetFees(App.member.id, season);

			hideActivityIndicator();

			valorQuota = allFees[0].valor;

			Frame backgroundFrame = new Frame
			{
				CornerRadius = 10,
				IsClippedToBounds = true,
				BackgroundColor = Color.FromRgb(240, 240, 240),
				HasShadow = false
			};

			relativeLayout.Children.Add(backgroundFrame,
				xConstraint: Constraint.Constant(10 * App.screenHeightAdapter),
				yConstraint: Constraint.Constant(0 * App.screenHeightAdapter),
				widthConstraint: Constraint.RelativeToParent((parent) =>
				{
					return ((parent.Width) - (20 * App.screenHeightAdapter));
				}),
				heightConstraint: Constraint.RelativeToParent((parent) =>
				{
					return ((parent.Height) - (20 * App.screenHeightAdapter));
				})
			);

			int y_index = CreateHeader();
			y_index = y_index + 10;

			Label labelQuota = new Label { BackgroundColor = Color.Transparent, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Start, FontSize = App.titleFontSize, TextColor = Color.Black, LineBreakMode = LineBreakMode.WordWrap };
            labelQuota.Text = "QUOTA ANUAL";
			relativeLayout.Children.Add(labelQuota,
				xConstraint: Constraint.Constant(20 * App.screenHeightAdapter),
				yConstraint: Constraint.Constant(y_index * App.screenHeightAdapter),
				widthConstraint: Constraint.RelativeToParent((parent) =>
				{
					return ((parent.Width) - (120 * App.screenHeightAdapter));
				}),
				heightConstraint: Constraint.Constant(30 * App.screenHeightAdapter)
			);

			valueQuota = new FormValue(valorQuota.ToString("0.00") + "€", App.titleFontSize, Color.White, App.normalTextColor, TextAlignment.End);
			//valueQuotaADCPN.Text = calculateQuotaADCPN();
			relativeLayout.Children.Add(valueQuota,
				xConstraint: Constraint.RelativeToParent((parent) =>
				{
					return ((parent.Width) - (100 * App.screenHeightAdapter));
				}),
				yConstraint: Constraint.Constant(y_index * App.screenHeightAdapter),
				widthConstraint: Constraint.Constant(70 * App.screenHeightAdapter),
				heightConstraint: Constraint.Constant(30 * App.screenHeightAdapter)
			);

            y_index = y_index + 50;

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
				xConstraint: Constraint.Constant(40),
				yConstraint: Constraint.Constant(y_index * App.screenHeightAdapter),
				widthConstraint: Constraint.Constant(102 * App.screenHeightAdapter),
				heightConstraint: Constraint.Constant(120 * App.screenHeightAdapter)
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
				xConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width - (142 * App.screenHeightAdapter)); // center of image (which is 40 wide)
				}),
				yConstraint: Constraint.Constant(y_index * App.screenHeightAdapter),
				widthConstraint: Constraint.Constant(102 * App.screenHeightAdapter),
				heightConstraint: Constraint.Constant(120 * App.screenHeightAdapter)
			);

		}



		public double getValorQuota(List<Fee> allFees, string tipoQuota)
		{
			foreach (Fee fee in allFees)
			{
				if (fee.tipo == tipoQuota)
				{
					return Convert.ToDouble(fee.valor);
				}
			}
			return 0;
		}

		public int CreateHeader()
		{
			int y_index = 10;

			Label nameLabel = new Label
			{
				Text = App.member.nickname,
				VerticalTextAlignment = TextAlignment.Center,
				HorizontalTextAlignment = TextAlignment.Center,
				TextColor = Color.Gray,
				LineBreakMode = LineBreakMode.NoWrap,
				FontSize = App.bigTitleFontSize
			};
			relativeLayout.Children.Add(nameLabel,
				xConstraint: Constraint.Constant(10 * App.screenWidthAdapter),
				yConstraint: Constraint.Constant(y_index * App.screenHeightAdapter),
				widthConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width) - (20 * App.screenWidthAdapter); // center of image (which is 40 wide)
				}),
				heightConstraint: Constraint.Constant(30 * App.screenHeightAdapter));

			return y_index + 30;
		}

		public CompleteRegistration_Payment_PageCS()
		{

			this.initLayout();
			this.initSpecificLayout();

			paymentDetected = false;

			int sleepTime = 5;
			Device.StartTimer(TimeSpan.FromSeconds(sleepTime), () =>
			{
				if ((paymentID != null) & (paymentID != ""))
				{
					//this.checkPaymentStatus(paymentID);
					if (paymentDetected == false)
					{
						return true;
					}
					else
					{
						return false;
					}
				}
				return true;
			});
		}

		async void checkPaymentStatus(string paymentID)
		{
			Debug.Print("checkPaymentStatus");
			this.payment = await GetPayment(paymentID);
			if ((payment.status == "confirmado") | (payment.status == "fechado"))
			{
				App.member.estado = "activo";
				App.original_member.estado = "activo";

				if (paymentDetected == false)
				{
					paymentDetected = true;

					await DisplayAlert("Pagamento Confirmado", "O seu pagamento foi recebido com sucesso. Já pode aceder à nossa App!", "Ok");
					App.Current.MainPage = new NavigationPage(new MainTabbedPageCS("", ""))
					{
						BarBackgroundColor = Color.White,
						BackgroundColor = Color.White,
						BarTextColor = Color.Black
					};

				}
			}
		}

		async Task<Payment> GetPayment(string paymentID)
		{
			Debug.WriteLine("GetPayment");
			PaymentManager paymentManager = new PaymentManager();

			Payment payment = await paymentManager.GetPayment(this.paymentID);

			if (payment == null)
			{
				Application.Current.MainPage = new NavigationPage(new LoginPageCS("Verifique a sua ligação à Internet e tente novamente."))
				{
					BarBackgroundColor = Color.White,
					BarTextColor = Color.Black
				};
				return null;
			}
			return payment;
		}




		async void OnMBButtonClicked(object sender, EventArgs e)
		{
			Debug.Print("OnMBButtonClicked");
            await Navigation.PushAsync(new CompleteRegistration_PaymentMB_PageCS(paymentID));
		}


		async void OnMBWayButtonClicked(object sender, EventArgs e)
		{
			await Navigation.PushAsync(new CompleteRegistration_PaymentMBWay_PageCS(paymentID));
		}

	}

}