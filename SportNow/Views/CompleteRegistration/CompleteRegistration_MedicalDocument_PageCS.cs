using System;
using System.Collections.Generic;
using Xamarin.Forms;
using SportNow.Model;
using SportNow.Services.Data.JSON;
using System.Threading.Tasks;
using System.Diagnostics;
using SportNow.CustomViews;
//Ausing Acr.UserDialogs;
using Xamarin.Essentials;
using SportNow.Services.Camera;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Net.Http.Headers;
using SportNow.Views.Profile;
using SkiaSharp;

namespace SportNow.Views.CompleteRegistration
{
	public class CompleteRegistration_MedicalDocument_PageCS : DefaultPage
	{

		protected async override void OnAppearing()
		{
		}


		protected async override void OnDisappearing()
		{
		}

		//Image estadoQuotaImage;


		private CollectionView collectionViewMembers;
		List<Member> members_To_Approve;
		Label titleLabel;
		RegisterButton confirmDocumentsButton;
		Image loadedDocument;
		ImageSource source;

		bool documentSubmitted;

		Stream stream;

		public void initLayout()
		{
			Title = "ATESTADO MÉDICO";
		}

		public async void initSpecificLayout()
		{
			Frame backgroundFrame = new Frame
			{
				CornerRadius = 10,
				IsClippedToBounds = true,
				BackgroundColor = Color.FromRgb(240, 240, 240),
				HasShadow = false
			};

			relativeLayout.Children.Add(backgroundFrame,
				xConstraint: Constraint.Constant(10 * App.screenHeightAdapter),
				yConstraint: Constraint.Constant(10 * App.screenHeightAdapter),
				widthConstraint: Constraint.RelativeToParent((parent) =>
				{
					return ((parent.Width) - (20 * App.screenHeightAdapter));
				}),
				heightConstraint: Constraint.RelativeToParent((parent) =>
				{
					return ((parent.Height) - (90 * App.screenHeightAdapter));
				})
			);


			Label titleLabel = new Label { Text = "FAZ UPLOAD VIA GALERIA OU CÂMARA DO TEU \n ATESTADO MÉDICO:", VerticalTextAlignment = TextAlignment.Start, HorizontalTextAlignment = TextAlignment.Center, FontSize = App.formLabelFontSize, TextColor = Color.FromRgb(100, 100, 100), LineBreakMode = LineBreakMode.WordWrap };

			relativeLayout.Children.Add(titleLabel,
				xConstraint: Constraint.Constant(15 * App.screenHeightAdapter),
				yConstraint: Constraint.Constant(20 * App.screenHeightAdapter),
				widthConstraint: Constraint.RelativeToParent((parent) =>
				{
					return ((parent.Width) - (30 * App.screenHeightAdapter));
				}),
				heightConstraint: Constraint.Constant(60 * App.screenHeightAdapter)
			);

			Image imageGallery = new Image
			{
				Source = "iconabrirgaleria.png",
				HorizontalOptions = LayoutOptions.Center,
			};

			var imageGallery_tap = new TapGestureRecognizer();
			imageGallery_tap.Tapped += OpenGalleryTapped;
			imageGallery.GestureRecognizers.Add(imageGallery_tap);

			relativeLayout.Children.Add(imageGallery,
				xConstraint: Constraint.RelativeToParent((parent) =>
				{
					return ((parent.Width / 2) - (110 * App.screenHeightAdapter));
				}),
				yConstraint: Constraint.Constant(120 * App.screenHeightAdapter),
				widthConstraint: Constraint.Constant(100 * App.screenHeightAdapter),
				heightConstraint: Constraint.Constant(100 * App.screenHeightAdapter)
			);

			Image imagePhoto = new Image
			{
				Source = "icontirarfoto.png",
				HorizontalOptions = LayoutOptions.Center,
			};
			var imagePhoto_tap = new TapGestureRecognizer();
			imagePhoto_tap.Tapped += TakeAPhotoTapped;
			imagePhoto.GestureRecognizers.Add(imagePhoto_tap);

			relativeLayout.Children.Add(imagePhoto,
				xConstraint: Constraint.RelativeToParent((parent) =>
				{
					return ((parent.Width / 2) + (10 * App.screenHeightAdapter));
				}),
				yConstraint: Constraint.Constant(120 * App.screenHeightAdapter),
				widthConstraint: Constraint.Constant(100 * App.screenHeightAdapter),
				heightConstraint: Constraint.Constant(100 * App.screenHeightAdapter)
			);

			Label atestadoLabel = new Label { Text = "Fazer download do formulário do Atestado Médico", VerticalTextAlignment = TextAlignment.Start, HorizontalTextAlignment = TextAlignment.Start, FontSize = 15 * App.screenWidthAdapter, TextColor = App.topColor, LineBreakMode = LineBreakMode.NoWrap };

			var atestadoLabel_tap = new TapGestureRecognizer();
			atestadoLabel_tap.Tapped += async (s, e) =>
			{
				try
				{
					await Browser.OpenAsync("https://www.adcpn.pt/wp-content/uploads/2019/05/Exame-Medico-Desportivo.pdf", BrowserLaunchMode.SystemPreferred);
				}
				catch (Exception ex)
				{
				}
			};
			atestadoLabel.GestureRecognizers.Add(atestadoLabel_tap);


			relativeLayout.Children.Add(atestadoLabel,
				xConstraint: Constraint.Constant(20 * App.screenHeightAdapter),
				yConstraint: Constraint.Constant(270 * App.screenHeightAdapter),
				widthConstraint: Constraint.RelativeToParent((parent) =>
				{
					return ((parent.Width) - (40 * App.screenHeightAdapter));
				}),
				heightConstraint: Constraint.Constant(30 * App.screenHeightAdapter)
			);

			confirmDocumentsButton = new RegisterButton("CONTINUAR", 100, 50);
			confirmDocumentsButton.button.Clicked += confirmDocumentsButtonClicked;

		}

		public CompleteRegistration_MedicalDocument_PageCS()
		{
			documentSubmitted = false;
			this.initLayout();
			this.initSpecificLayout();

            loadedDocument = new Image() { };
            relativeLayout.Children.Add(loadedDocument,
                xConstraint: Constraint.Constant(20 * App.screenHeightAdapter),
                yConstraint: Constraint.Constant(300 * App.screenHeightAdapter),
                widthConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return ((parent.Width) - (40 * App.screenHeightAdapter));
                }),
                heightConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return ((parent.Height) - (390 * App.screenHeightAdapter));
                })
            );

            /*MessagingCenter.Subscribe<byte[]>(this, "documentPhoto", (args) =>
			{
				Device.BeginInvokeOnMainThread(() =>
				{
					documentSubmitted = false;
					if (loadedDocument != null)
					{
						relativeLayout.Children.Remove(loadedDocument);
						loadedDocument = null;
					}
					streamSource = new MemoryStream(args);
					source = ImageSource.FromStream(() => new MemoryStream(args));
					loadedDocument = new Image() { };
					relativeLayout.Children.Add(loadedDocument,
						xConstraint: Constraint.Constant(20 * App.screenHeightAdapter),
						yConstraint: Constraint.Constant(300 * App.screenHeightAdapter),
						widthConstraint: Constraint.RelativeToParent((parent) =>
						{
							return ((parent.Width) - (40 * App.screenHeightAdapter));
						}),
						heightConstraint: Constraint.RelativeToParent((parent) =>
						{
							return ((parent.Height) - (390 * App.screenHeightAdapter));
						})
					);
					loadedDocument.Source = source;
				});

			});*/
        }

		async void confirmDocumentsButtonClicked(object sender, EventArgs e)
		{
			Debug.Print("confirmDocumentsButtonClicked");

			
			//if (documentSubmitted == false)
			//{
				if (stream != null)
				{
					MemberManager memberManager = new MemberManager();

					string documentname = "";
					string type = "";

                documentname = "Atestado Médico  - " + DateTime.Now.ToString("dd") + "-" + DateTime.Now.ToString("MM") + "-" + DateTime.Now.ToString("yyyy") + " - " + App.member.name;
                type = "atestado_medico";

					string filename = documentname + ".png";
					string status = "Under Review";
					string startdate = DateTime.Now.ToString("yyyy-MM-dd");// "2022-07-22";

					string enddate = DateTime.Now.Year + "-12-31";
					showActivityIndicator();
					//documentSubmitted = true;
					_ = await memberManager.Upload_Member_Document(stream, App.member.id, filename, documentname, status, type, startdate, enddate);
					//await Navigation.PushAsync(new CompleteRegistration_Profile_PageCS());
					await Navigation.PushAsync(new CompleteRegistration_Payment_PageCS());
					hideActivityIndicator();


                }
				else
				{
                    await DisplayAlert("Documento não submetido", "Para continuar tem de submeter um documento.", "OK");
                    
                    //UserDialogs.Instance.Alert(new AlertConfig() { Title = "Documento não submetido", Message = "Para continuar tem de submeter um documento.", OkText = "Ok" });
					//await Navigation.PushAsync(new CompleteRegistration_Profile_PageCS());
				}
			//}
			/*else
			{
				await Navigation.PushAsync(new CompleteRegistration_Profile_PageCS());
			}*/
			
			


		}

        async void OpenGalleryTapped(System.Object sender, System.EventArgs e)
        {
            var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
            {
                Title = "Por favor escolha uma foto"
            });

            if (result != null)
            {
                Stream stream_aux = await result.OpenReadAsync();
                Stream localstream = await result.OpenReadAsync();

                loadedDocument.Source = ImageSource.FromStream(() => localstream);
                if (Device.RuntimePlatform == Device.iOS)
                {
                    loadedDocument.Rotation = 0;
                    stream = RotateBitmap(stream_aux, 0);
                }
                else
                {
                    loadedDocument.Rotation = 90;
                    stream = RotateBitmap(stream_aux, 90);
                }
                //documentSubmitted = true;
                showConfirmDocumentsButton();
            }
        }

        async void TakeAPhotoTapped(System.Object sender, System.EventArgs e)
        {
            var result = await MediaPicker.CapturePhotoAsync();

            if (result != null)
            {
                Stream stream_aux = await result.OpenReadAsync();
                Stream localstream = await result.OpenReadAsync();

                loadedDocument.Source = ImageSource.FromStream(() => localstream);
                loadedDocument.Rotation = 90;
                stream = RotateBitmap(stream_aux, 90);
				//documentSubmitted = true;
                showConfirmDocumentsButton();
            }

        }

        public Stream RotateBitmap(Stream _stream, int angle)
        {
            Stream streamlocal = null;
            SKBitmap bitmap = SKBitmap.Decode(_stream);
            SKBitmap rotatedBitmap = new SKBitmap(bitmap.Height, bitmap.Width);
            if (angle != 0)
            {
                using (var surface = new SKCanvas(rotatedBitmap))
                {
                    surface.Translate(rotatedBitmap.Width, 0);
                    surface.RotateDegrees(angle);
                    surface.DrawBitmap(bitmap, 0, 0);
                }
            }
            else
            {
                rotatedBitmap = bitmap;
            }

            using (MemoryStream memStream = new MemoryStream())
            using (SKManagedWStream wstream = new SKManagedWStream(memStream))
            {
                rotatedBitmap.Encode(wstream, SKEncodedImageFormat.Jpeg, 40);
                byte[] data = memStream.ToArray();
                streamlocal = new MemoryStream(data);
            }
            return streamlocal;

        }

        public void showConfirmDocumentsButton()
        {
            relativeLayout.Children.Add(confirmDocumentsButton,
                xConstraint: Constraint.Constant(10),
                yConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return (parent.Height) - (60 * App.screenHeightAdapter); // 
                }),
                widthConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return (parent.Width - 20 * App.screenHeightAdapter); // center of image (which is 40 wide)
                }),
                heightConstraint: Constraint.Constant((50 * App.screenHeightAdapter))
            );
        }
    }

}