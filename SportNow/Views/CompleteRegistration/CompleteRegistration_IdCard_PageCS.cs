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
using System.Runtime.InteropServices.ComTypes;
using static System.Net.Mime.MediaTypeNames;

namespace SportNow.Views.CompleteRegistration
{
	public class CompleteRegistration_IdCard_PageCS : DefaultPage
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
		Xamarin.Forms.Image loadedDocument;
		ImageSource source;
		FormValueEditDate idCardExpireDate;
        FormValueEdit idCardNumber;

        bool documentSubmitted;

		Stream stream;

        string idCardType;

        public void initLayout()
		{
            if (idCardType == "parent")
            {
                Title = "DOCUMENTO ID ENC. EDUCAÇÃO";
            }
            else if (idCardType == "company")
            {
                Title = "DOCUMENTO IDENTIFICAÇÃO EMPRESA";
            }
            else
            {
                Title = "DOCUMENTO IDENTIFICAÇÃO";
            }

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


			Label titleLabel = new Label { VerticalTextAlignment = TextAlignment.Start, HorizontalTextAlignment = TextAlignment.Center, FontSize = App.formLabelFontSize, TextColor = Color.FromRgb(100, 100, 100), LineBreakMode = LineBreakMode.WordWrap };
            if (idCardType == "parent")
            {
                titleLabel.Text = "FAZ UPLOAD VIA GALERIA OU CÂMARA DO \nDOCUMENTO DE IDENTIFICAÇÃO DO TEU ENCARREGADO DE EDUCAÇÃO:";
            }
            else if (idCardType == "company")
            {
                titleLabel.Text = "FAZ UPLOAD VIA GALERIA OU CÂMARA DO TEU \nDOCUMENTO DE IDENTIFICAÇÃO DE EMPRESA:";
            }
            else if (idCardType == "")
            {
                titleLabel.Text = "FAZ UPLOAD VIA GALERIA OU CÂMARA DO TEU \nDOCUMENTO DE IDENTIFICAÇÃO:";
            }

            relativeLayout.Children.Add(titleLabel,
				xConstraint: Constraint.Constant(15 * App.screenHeightAdapter),
				yConstraint: Constraint.Constant(20 * App.screenHeightAdapter),
				widthConstraint: Constraint.RelativeToParent((parent) =>
				{
					return ((parent.Width) - (30 * App.screenHeightAdapter));
				}),
				heightConstraint: Constraint.Constant(60 * App.screenHeightAdapter)
			);

            Xamarin.Forms.Image imageGallery = new Xamarin.Forms.Image
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
				yConstraint: Constraint.Constant(100 * App.screenHeightAdapter),
				widthConstraint: Constraint.Constant(100 * App.screenHeightAdapter),
				heightConstraint: Constraint.Constant(100 * App.screenHeightAdapter)
			);

            Xamarin.Forms.Image imagePhoto = new Xamarin.Forms.Image
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
				yConstraint: Constraint.Constant(100 * App.screenHeightAdapter),
				widthConstraint: Constraint.Constant(100 * App.screenHeightAdapter),
				heightConstraint: Constraint.Constant(100 * App.screenHeightAdapter)
			);


           /* FormLabel cc_numberLabel = new FormLabel();
            cc_numberLabel.Text = "NÚMERO DE IDENTIFICAÇÃO";
            relativeLayout.Children.Add(cc_numberLabel,
                xConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return ((parent.Width / 2) - (150 * App.screenHeightAdapter));
                }),
                yConstraint: Constraint.Constant(210 * App.screenHeightAdapter),
                widthConstraint: Constraint.Constant(150 * App.screenHeightAdapter),
                heightConstraint: Constraint.Constant(40 * App.screenHeightAdapter)
            );

            idCardNumber = new FormValueEdit(App.member.cc_number);//?.ToString("yyyy-MM-dd"));
            relativeLayout.Children.Add(idCardNumber,
                xConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return ((parent.Width / 2) + (25 * App.screenHeightAdapter));
                }),
                yConstraint: Constraint.Constant(210 * App.screenHeightAdapter),
                widthConstraint: Constraint.Constant(125 * App.screenHeightAdapter),
                heightConstraint: Constraint.Constant(40 * App.screenHeightAdapter)
            );*/

            FormLabel validadeLabel = new FormLabel { Text = "VALIDADE DOCUMENTO" };
            relativeLayout.Children.Add(validadeLabel,
                xConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return ((parent.Width / 2) - (150 * App.screenHeightAdapter));
                }),
                yConstraint: Constraint.Constant(210 * App.screenHeightAdapter),
                widthConstraint: Constraint.Constant(150 * App.screenHeightAdapter),
                heightConstraint: Constraint.Constant(40 * App.screenHeightAdapter)
            );

            idCardExpireDate = new FormValueEditDate("");//?.ToString("yyyy-MM-dd"));
            relativeLayout.Children.Add(idCardExpireDate,
                xConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return ((parent.Width / 2) + (25 * App.screenHeightAdapter));
                }),
                yConstraint: Constraint.Constant(210 * App.screenHeightAdapter),
                widthConstraint: Constraint.Constant(125 * App.screenHeightAdapter),
                heightConstraint: Constraint.Constant(40 * App.screenHeightAdapter)
            );

            Debug.Print("App.member.country = " + App.member.country);
            if (App.member.country != "PORTUGAL")
            {
                Label estrangeirosLabel = new Label { BackgroundColor = Color.Transparent, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Start, FontSize = 11, TextColor = Color.Black, LineBreakMode = LineBreakMode.WordWrap };
                estrangeirosLabel.Text = "Para os indivíduos que não sejam naturais de Portugal, é necessário submeter um documento comprovativo:\n * para os cidadãos comunitários(naturais de qualquer país na União Europeia), um Certificado de Residência;\n * para os cidadãos de nacionalidade Brasileira, é necessário apresentar o documento que comprove a atribuição do Estatuto de Igualdade conforme a Resolução da Assembleia da República n.º 83 / 2000";

                relativeLayout.Children.Add(estrangeirosLabel,
                    xConstraint: Constraint.Constant(15),
                    yConstraint: Constraint.RelativeToParent((parent) =>
                    {
                        return (parent.Height) - (185 * App.screenHeightAdapter); // 
                    }),
                    widthConstraint: Constraint.RelativeToParent((parent) =>
                    {
                        return (parent.Width - 30 * App.screenHeightAdapter); // center of image (which is 40 wide)
                    }),
                    heightConstraint: Constraint.Constant((75 * App.screenHeightAdapter))
                );
            }

            Label bothsidesLabel = new Label { BackgroundColor = Color.Transparent, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center, FontSize = App.itemTextFontSize, TextColor = Color.Black, LineBreakMode = LineBreakMode.WordWrap };
            bothsidesLabel.Text = "CÓPIA DO DOCUMENTO DE IDENTIFICAÇÃO - FRENTE E VERSO";

            relativeLayout.Children.Add(bothsidesLabel,
                xConstraint: Constraint.Constant(10),
                yConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return (parent.Height) - (120 * App.screenHeightAdapter); // 
                }),
                widthConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return (parent.Width - 20 * App.screenHeightAdapter); // center of image (which is 40 wide)
                }),
                heightConstraint: Constraint.Constant((50 * App.screenHeightAdapter))
            );



            confirmDocumentsButton = new RegisterButton("CONTINUAR", 100, 50);
			confirmDocumentsButton.button.Clicked += confirmDocumentsButtonClicked;
		}

		public CompleteRegistration_IdCard_PageCS(string type)
		{
			documentSubmitted = false;
            this.idCardType = type;
			this.initLayout();
			this.initSpecificLayout();

            loadedDocument = new Xamarin.Forms.Image() { };
            relativeLayout.Children.Add(loadedDocument,
                xConstraint: Constraint.Constant(20 * App.screenHeightAdapter),
                yConstraint: Constraint.Constant(280 * App.screenHeightAdapter),
                widthConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return ((parent.Width) - (40 * App.screenHeightAdapter));
                }),
                heightConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return (parent.Height) - (455 * App.screenHeightAdapter); // 
                })
            );
        }

		async void confirmDocumentsButtonClicked(object sender, EventArgs e)
		{
			Debug.Print("confirmDocumentsButtonClicked");

            if (idCardExpireDate.entry.Text == "")
            {
                await DisplayAlert("Documento não submetido", "Para continuar tem de indicar a validade do documento.", "OK");
            }
            else if (DateTime.Parse(idCardExpireDate.entry.Text) < DateTime.Now)
            {
                await DisplayAlert("Documento não submetido", "O documento de identificação já não é válido.", "OK");
            }
            else if (stream != null)
            {
                MemberManager memberManager = new MemberManager();

                string documentname = "";
                string type = "";
                if (idCardType == "parent")
                {
                    type = "documento_identificacao_encarregado";
                    documentname = "Documento Identificação Enc.Educação  - " + DateTime.Now.ToString("dd") + "-" + DateTime.Now.ToString("MM") + "-" + DateTime.Now.ToString("yyyy") + " - " + App.member.name;
                }
                else
                {
                    type = "documento_identificacao";
                    documentname = "Documento Identificação - " + DateTime.Now.ToString("dd") + "-" + DateTime.Now.ToString("MM") + "-" + DateTime.Now.ToString("yyyy") + " - " + App.member.name;
                    //filename = "IDdoc - " + App.member.id + ".png";
                }

                string filename = documentname + ".png";
                string status = "Under Review";
                string startdate = DateTime.Now.ToString("yyyy-MM-dd");// "2022-07-22";
                string enddate = idCardExpireDate.entry.Text;

                showActivityIndicator();
                //documentSubmitted = true;
                _ = await memberManager.Upload_Member_Document(stream, App.member.id, filename, documentname, status, type, startdate, enddate);
                //App.member.cc_number = idCardNumber.entry.Text;
                //_ = await memberManager.UpdateMemberInfo(App.original_member.id, App.member);

                int age = Constants.GetAge(DateTime.Parse(App.member.birthdate));

                if ((this.idCardType == "") & (age < 18))
                {
                    await Navigation.PushAsync(new CompleteRegistration_IdCard_PageCS("parent"));
                }
                else if ((App.member.federado_tipo.Contains("atleta")) | (App.member.federado_tipo.Contains("treinador")))
                {
                    await Navigation.PushAsync(new CompleteRegistration_MedicalDocument_PageCS());
                }
                else
                {
                    await Navigation.PushAsync(new CompleteRegistration_Payment_PageCS());
                    //await Navigation.PushAsync(new CompleteRegistration_Profile_PageCS());
                }


                hideActivityIndicator();


            }
            else
            {
                await DisplayAlert("Documento não submetido", "Para continuar tem de submeter um documento.", "OK");
            }
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