using System;
using System.Collections.Generic;
using Xamarin.Forms;
using SportNow.Model;
using SportNow.Services.Data.JSON;
using System.Threading.Tasks;
using System.Diagnostics;
using SportNow.CustomViews;
using Xamarin.Essentials;
using SportNow.Services.Camera;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Net.Http.Headers;
using SportNow.Views.Profile;
using static System.Net.WebRequestMethods;
using SkiaSharp;
using SportNow.Views.CompleteRegistration;

namespace SportNow.Views.Profile
{
	public class DocumentsUploadPageCS : DefaultPage
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
		Stream streamSource;

		Picker documentTypePicker;

		Stream stream;

        FormValueEditDate idCardExpireDate;
        FormLabel validadeLabel;
        Label estrangeirosLabel, bothsidesLabel;

        private CollectionView documentsCollectionView;

		public void initLayout()
		{
			Title = "UPLOAD DOCUMENTO";
		}


		public async void initSpecificLayout()
		{
			Label titleLabel = new Label { Text = "ESCOLHA O TIPO DE DOCUMENTO E FAÇA UPLOAD VIA GALERIA OU CÂMARA:", VerticalTextAlignment = TextAlignment.Start, HorizontalTextAlignment = TextAlignment.Center, FontSize = App.formLabelFontSize, TextColor = App.bottomColor, LineBreakMode = LineBreakMode.WordWrap };

			relativeLayout.Children.Add(titleLabel,
				xConstraint: Constraint.Constant(0 * App.screenHeightAdapter),
                yConstraint: Constraint.Constant(0 * App.screenHeightAdapter),
                widthConstraint: Constraint.RelativeToParent((parent) =>
				{
					return ((parent.Width) - (0 * App.screenHeightAdapter));
				}),
				heightConstraint: Constraint.Constant(40 * App.screenHeightAdapter)
			);

			createDocumentTypePicker();

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
					return ((parent.Width / 2) - (80 * App.screenHeightAdapter));
				}),
				yConstraint: Constraint.Constant(40 * App.screenHeightAdapter),
				widthConstraint: Constraint.Constant(60 * App.screenHeightAdapter),
				heightConstraint: Constraint.Constant(60 * App.screenHeightAdapter)
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
					return ((parent.Width / 2) + (20 * App.screenHeightAdapter));
				}),
				yConstraint: Constraint.Constant(40 * App.screenHeightAdapter),
				widthConstraint: Constraint.Constant(60 * App.screenHeightAdapter),
				heightConstraint: Constraint.Constant(60 * App.screenHeightAdapter)
			);

			
			confirmDocumentsButton = new RegisterButton("GRAVAR", 100, 50);
			confirmDocumentsButton.button.Clicked += confirmDocumentsButtonClicked;

            loadedDocument = new Image() { };
            relativeLayout.Children.Add(loadedDocument,
                xConstraint: Constraint.Constant(20 * App.screenHeightAdapter),
                yConstraint: Constraint.Constant(170 * App.screenHeightAdapter),
                widthConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return ((parent.Width) - (40 * App.screenHeightAdapter));
                }),
                heightConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return ((parent.Height) - (420 * App.screenHeightAdapter));
                })
            );

        }

        public async void showDocumentExpiryDate()
        {
            if (documentTypePicker.SelectedItem.ToString().Contains("Documento Identificação"))
            {
                removeDocumentExpiryDate();

                Debug.Print("App.member.country = " + App.member.country);
                if (App.member.country != "PORTUGAL")
                {
                    estrangeirosLabel = new Label { BackgroundColor = Color.Transparent, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Start, FontSize = 11, TextColor = App.normalTextColor, LineBreakMode = LineBreakMode.WordWrap };
                    estrangeirosLabel.Text = "Para os indivíduos que não sejam naturais de Portugal, é necessário submeter um documento comprovativo:\n * para os cidadãos comunitários(naturais de qualquer país na União Europeia), um Certificado de Residência;\n * para os cidadãos de nacionalidade Brasileira, é necessário apresentar o documento que comprove a atribuição do Estatuto de Igualdade conforme a Resolução da Assembleia da República n.º 83 / 2000";

                    relativeLayout.Children.Add(estrangeirosLabel,
                        xConstraint: Constraint.Constant(15),
                        yConstraint: Constraint.RelativeToParent((parent) =>
                        {
                            return (parent.Height) - (245 * App.screenHeightAdapter); // 
                        }),
                        widthConstraint: Constraint.RelativeToParent((parent) =>
                        {
                            return (parent.Width - 30 * App.screenHeightAdapter); // center of image (which is 40 wide)
                        }),
                        heightConstraint: Constraint.Constant((75 * App.screenHeightAdapter))
                    );
                }

                bothsidesLabel = new Label { BackgroundColor = Color.Transparent, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center, FontSize = App.itemTextFontSize, TextColor = App.bottomColor, LineBreakMode = LineBreakMode.WordWrap };
                bothsidesLabel.Text = "CÓPIA DO DOCUMENTO DE IDENTIFICAÇÃO - FRENTE E VERSO";

                relativeLayout.Children.Add(bothsidesLabel,
                    xConstraint: Constraint.Constant(10),
                    yConstraint: Constraint.RelativeToParent((parent) =>
                    {
                        return (parent.Height) - (170 * App.screenHeightAdapter); // 
                    }),
                    widthConstraint: Constraint.RelativeToParent((parent) =>
                    {
                        return (parent.Width - 20 * App.screenHeightAdapter); // center of image (which is 40 wide)
                    }),
                    heightConstraint: Constraint.Constant((50 * App.screenHeightAdapter))
                );

                validadeLabel = new FormLabel { Text = "VALIDADE DOCUMENTO" };
                validadeLabel.TextColor = App.bottomColor;
                relativeLayout.Children.Add(validadeLabel,
                    xConstraint: Constraint.RelativeToParent((parent) =>
                    {
                        return ((parent.Width / 2) - (150 * App.screenHeightAdapter));
                    }),
                    yConstraint: Constraint.RelativeToParent((parent) =>
                    {
                        return (parent.Height) - (120 * App.screenHeightAdapter); // 
                    }),
                    widthConstraint: Constraint.Constant(150 * App.screenHeightAdapter),
                    heightConstraint: Constraint.Constant(40 * App.screenHeightAdapter)
                );

                idCardExpireDate = new FormValueEditDate("");//?.ToString("yyyy-MM-dd"));
                relativeLayout.Children.Add(idCardExpireDate,
                    xConstraint: Constraint.RelativeToParent((parent) =>
                    {
                        return ((parent.Width / 2) + (25 * App.screenHeightAdapter));
                    }),
                    yConstraint: Constraint.RelativeToParent((parent) =>
                    {
                        return (parent.Height) - (120 * App.screenHeightAdapter); // 
                    }),
                    widthConstraint: Constraint.Constant(125 * App.screenHeightAdapter),
                    heightConstraint: Constraint.Constant(40 * App.screenHeightAdapter));
            }
            else
            {
                removeDocumentExpiryDate();
            }
        }

        public async void removeDocumentExpiryDate()
        {
            if (idCardExpireDate != null)
            {
                relativeLayout.Children.Remove(validadeLabel); 
                relativeLayout.Children.Remove(idCardExpireDate);
                if (estrangeirosLabel != null)
                {
                    relativeLayout.Children.Remove(estrangeirosLabel);
                }
                if (bothsidesLabel != null)
                {
                    relativeLayout.Children.Remove(bothsidesLabel);
                }

            }
        }

        public async void createDocumentTypePicker()
		{
			List<string> tipoDocumentoList = new List<string>();
            int age = Constants.GetAge(DateTime.Parse(App.member.birthdate));
			if (App.member.socio_tipo == "empresa") 
			{
                tipoDocumentoList.Add("Documento Identificação");
            }
            else if (App.member.socio_tipo == "individual")
            {
				tipoDocumentoList.Add("Documento Identificação");
                if (age < 18)
                {
					tipoDocumentoList.Add("Documento Identificação Enc. Educação");
                }
                if ((App.member.federado_tipo.Contains("atleta")) | (App.member.federado_tipo.Contains("treinador")))
                {
                    tipoDocumentoList.Add("Atestado Médico");
                }
            }
            
			int selectedIndex = 0;
			int selectedIndex_temp = 0;

			documentTypePicker = new Picker
			{
				Title = "",
				TitleColor = Color.Black,
				BackgroundColor = Color.Transparent,
				TextColor = App.topColor,
				HorizontalTextAlignment = TextAlignment.Center,
				FontSize = App.formLabelFontSize
            };
			documentTypePicker.ItemsSource = tipoDocumentoList;


            documentTypePicker.SelectedIndexChanged += async (object sender, EventArgs e) =>
            {
                Debug.Print("documentTypePicker selectedItem = " + documentTypePicker.SelectedItem.ToString());
                showDocumentExpiryDate();
                /*if (documentTypePicker.SelectedItem.ToString().Contains("Documento Identificação") == false)
                {
                    removeDocumentIDElements();
                }*/
            };
            documentTypePicker.SelectedIndex = selectedIndex;

            relativeLayout.Children.Add(documentTypePicker,
				xConstraint: Constraint.Constant(0),
				yConstraint: Constraint.Constant(110 * App.screenHeightAdapter),
				widthConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width); // center of image (which is 40 wide)
				}),
				heightConstraint: Constraint.Constant(50));
		}


		public DocumentsUploadPageCS()
		{
			this.initLayout();
			this.initSpecificLayout();
		}


        async void confirmDocumentsButtonClicked(object sender, EventArgs e)
        {
            Debug.Print("confirmDocumentsButtonClicked");

            string enddate = "";// DateTime.Now.AddYears(1).ToString("yyyy-MM-dd");

            if (documentTypePicker.SelectedItem.ToString().Contains("Documento Identificação"))
            {
                if (idCardExpireDate.entry.Text == "")
                {
                    await DisplayAlert("Documento não submetido", "Para continuar tem de indicar a validade do documento.", "OK");
                    return;
                }
                else if (DateTime.Parse(idCardExpireDate.entry.Text) < DateTime.Now)
                {
                    await DisplayAlert("Documento não submetido", "O documento de identificação já não é válido.", "OK");
                    return;
                }
                else
                {
                    enddate = idCardExpireDate.entry.Text;
                }
            }
            else
            {
                if (DateTime.Now.Month < 8)
                {
                    enddate = DateTime.Now.Year + "-08-31";
                }
                else
                {
                    enddate = DateTime.Now.AddYears(1).Year + "-08-31";
                }
            }
            if (stream != null)
            {
                MemberManager memberManager = new MemberManager();

                string documentname = "";
                string type = "";

                Debug.Print("App.member.aulatipo = " + App.member.aulatipo);

                if (documentTypePicker.SelectedItem.ToString() == "Documento Identificação")
                {
                    documentname = "Documento Identificação - " + DateTime.Now.ToString("dd") + "-" + DateTime.Now.ToString("MM") + "-" + DateTime.Now.ToString("yyyy")+" - "+ App.member.name;
                    type = "documento_identificacao";
                }
                else if (documentTypePicker.SelectedItem.ToString() == "Documento Identificação Enc. Educação")
                {
                    documentname = "Documento Identificação Enc. Educação - " + DateTime.Now.ToString("dd") + "-" + DateTime.Now.ToString("MM") + "-" + DateTime.Now.ToString("yyyy") + " - " + App.member.name;
                    type = "documento_identificacao_encarregado";
                }
                else if (documentTypePicker.SelectedItem.ToString() == "Atestado Médico")
                {
                    documentname = "Atestado Médico - " + DateTime.Now.ToString("dd") + "-" + DateTime.Now.ToString("MM") + "-" + DateTime.Now.ToString("yyyy") + " - " + App.member.name;
                    type = "atestado_medico";
                }

                string filename = documentname + ".png";
                string status = "Under Review";
                string startdate = DateTime.Now.ToString("yyyy-MM-dd");// "2022-07-22";

                showActivityIndicator();
                //documentSubmitted = true;
                _ = await memberManager.Upload_Member_Document(stream, App.member.id, filename, documentname, status, type, startdate, enddate);
                App.member.documents = await memberManager.Get_Member_Documents(App.member.id);
                await Navigation.PopAsync();
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

                //MemberManager memberManager = new MemberManager();
                //await memberManager.Upload_Member_Photo(stream);
                showConfirmDocumentsButton();
                //showDocumentExpiryDate();
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

                //MemberManager memberManager = new MemberManager();
                //await memberManager.Upload_Member_Photo(stream);
                showConfirmDocumentsButton();
                //showDocumentExpiryDate();
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

                /*using (var surface = new SKCanvas(rotatedBitmap))
                {
                    surface.Translate(rotatedBitmap.Width, 0);
                    surface.DrawBitmap(bitmap, 0, 0);
                }*/
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