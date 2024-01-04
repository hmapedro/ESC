using System;
using System.Collections.Generic;
using Xamarin.Forms;
using SportNow.Model;
using SportNow.Services.Data.JSON;
using System.Threading.Tasks;
using System.Diagnostics;
using SportNow.CustomViews;
//Ausing Acr.UserDialogs;
using System.Text.RegularExpressions;
using System.Net;
using SportNow.Services.Camera;
using System.IO;
using SportNow.Views.CompleteRegistration;
using SkiaSharp;
using Xamarin.Essentials;

namespace SportNow.Views.CompleteRegistration
{
    public class CompleteRegistration_Profile_PageCS : DefaultPage
	{

		protected async override void OnAppearing()
		{
		}


		protected async override void OnDisappearing()
		{
			if (moveForward != true)
			{
                //await UpdateMemberInfo(true);
            }
			moveForward = false;


        }

		private ScrollView scrollView;

		private Member member;

		MenuButton geralButton;
		MenuButton moradaButton;
		MenuButton encEducacaoButton;
		MenuButton infoEscolarButton;

		StackLayout stackButtons;
		private Grid gridGeral;
		private Grid gridMorada;
		private Grid gridEncEducacao;
		private Grid gridInfoEscolar;

		RoundImage memberPhotoImage;
        FormValueEdit nameValue;
        FormValueEdit emailValue;
		FormValueEdit nifValue;
        FormValueEdit cc_number; 
        FormValueEditDate birthdateValue;
		FormValueEditPicker countryValue;
		FormValueEditPicker genderValue;
		FormValueEdit phoneValue;
		FormValueEdit addressValue;
		FormValueEdit cityValue;
		FormValueEditCodPostal postalcodeValue;
		FormValueEdit nameEmergencyContactValue;
		FormValueEdit phoneEmergencyContactValue;
		FormValueEdit EncEducacao1NomeValue;
		FormValueEdit EncEducacao1PhoneValue;
		FormValueEdit EncEducacao1MailValue;
		FormValueEdit EncEducacao1cc_numberValue;

		bool enteringPage = true;
		bool moveForward = false;

        Stream stream;


		public void initLayout()
		{

			if (App.member.socio_tipo == "empresa")
			{
				Title = "COMPLETAR DADOS EMPRESA";
			}
			else if ((App.member.socio_tipo == "individual") | (App.member.socio_tipo == "nao_socio"))
			{
                Title = "COMPLETAR DADOS PESSOAIS";
            }


        }


		public async void initSpecificLayout()
		{
			MemberManager memberManager = new MemberManager();

			//App.member.id = await memberManager.createNewMember(App.member);
			createNewMember_Test();

            member = App.member;

			scrollView = new ScrollView { Orientation = ScrollOrientation.Vertical };

			relativeLayout.Children.Add(scrollView,
				xConstraint: Constraint.Constant(0),
				yConstraint: Constraint.Constant(250 * App.screenHeightAdapter),
				widthConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width); // center of image (which is 40 wide)
				}),
				heightConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Height) - 340 * App.screenHeightAdapter; // center of image (which is 40 wide)
				})
			);

			CreatePhoto();
			CreateName();
			CreateStackButtons();
			CreateGridGeral();
			CreateGridMorada();
			CreateGridEncEducacao();
			
			CreateGridButtons();

			OnGeralButtonClicked(null, null);

			RegisterButton confirmPersonalDataButton = new RegisterButton("CONTINUAR", 100, 50);
			confirmPersonalDataButton.button.Clicked += confirmPersonalDataButtonClicked;

			relativeLayout.Children.Add(confirmPersonalDataButton,
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

		public void CreateName()
		{
			Label nameLabel = new Label
			{
				Text = member.nickname,
				VerticalTextAlignment = TextAlignment.Center,
				HorizontalTextAlignment = TextAlignment.Center,
				TextColor = App.normalTextColor,
				LineBreakMode = LineBreakMode.WordWrap,
				FontSize = App.bigTitleFontSize,
				TextTransform = TextTransform.Uppercase
			};
			relativeLayout.Children.Add(nameLabel,
				xConstraint: Constraint.Constant(10 * App.screenHeightAdapter),
				yConstraint: Constraint.Constant(160 * App.screenHeightAdapter),
				widthConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width - 20 * App.screenHeightAdapter); // center of image (which is 40 wide)
				}),
				heightConstraint: Constraint.Constant((70 * App.screenHeightAdapter))
			);

		}

        public void createNewMember_Test()
        {
            /*App.member.id = "";
            App.member.number_member = "";
            App.member.name = "";
            App.member.description = "";
            App.member.nickname = "";
            App.member.email = "";
            App.member.phone = "";
            App.member.gender = "";
            App.member.country = "";
            App.member.nameEmergencyContact = "";
            App.member.phoneEmergencyContact = "";

            App.member.aulaid = "";
            App.member.aulanome = "";
            App.member.aulatipo = "";
            App.member.aulavalor = "";

            App.member.birthdate = "";
            App.member.registrationdate  = DateTime.Today;
        
            App.member.nif  = "";
            App.member.cc_number  = "";
            App.member.number_fnkp  = "";

            App.member.address  = "";
            App.member.city  = "";
            App.member.postalcode  = "";

            App.member.name_enc1  = "";
            App.member.mail_enc1  = "";
            App.member.phone_enc1  = "";
            App.member.cc_number_enc1  = "";
        
            App.member.name_enc2  = "";
            App.member.mail_enc2  = "";
            App.member.phone_enc2  = "";

            App.member.member_type  = "";
            App.member.isInstrutorResponsavel  = "";
            App.member.isResponsavelAdministrativo  = "";
            App.member.isExaminador = "";
            App.member.isTreinador  = "";
            App.member.isAprovado  = "";
            App.member.estado  = "";

            App.member.schoolname  = "";
            App.member.schoolnumber  = "";
            App.member.schoolyear  = "";
            App.member.schoolclass  = "";

            App.member.mainsport  = "";
            App.member.othersports  = "";*/

            App.member.id = "";
            App.member.number_member = "";
            App.member.name = "Hugo Pedro";
            App.member.description = "";
            App.member.nickname = "";
            App.member.email = "hmap@hotmail.com";
            App.member.phone = "918798854";
            App.member.gender = "male";
            App.member.country = "PORTUGAL";
            App.member.nameEmergencyContact = "Renata";
            App.member.phoneEmergencyContact = "911812603";

            App.member.aulaid = "";
            App.member.aulanome = "";
            App.member.aulatipo = "";
            App.member.aulavalor = "";

            App.member.birthdate = "1979-12-26";
            App.member.registrationdate = DateTime.Today;

            App.member.nif = "205084060";
            App.member.cc_number = "312312321";
            App.member.number_fnkp = "";

            App.member.address = "Rua";
            App.member.city = "Odivelas";
            App.member.postalcode = "2675-503";

            App.member.name_enc1 = "";
            App.member.mail_enc1 = "";
            App.member.phone_enc1 = "";
            App.member.cc_number_enc1 = "";

            App.member.name_enc2 = "";
            App.member.mail_enc2 = "";
            App.member.phone_enc2 = "";

            App.member.member_type = "";
            App.member.isInstrutorResponsavel = "";
            App.member.isResponsavelAdministrativo = "";
            App.member.isExaminador = "";
            App.member.isTreinador = "";
            App.member.isAprovado = "";
            App.member.estado = "";

            //App.member.mainsport = "";
            //App.member.othersports = "";

            //App.member.socio_tipo = "individual";
            //App.member.federado_tipo = "";

            App.original_member = App.member;

            App.member.consentimento_regulamento = "1";
        }
		
        public void CreatePhoto()
		{

			memberPhotoImage = new RoundImage();

			if (App.member.id != null)
			{
				Debug.Print("App.member.id AQUI = " + App.member.id);

                WebResponse response;
				HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(Constants.images_URL + App.member.id + "_photo");
			
				request.Method = "HEAD";
				bool exists;
				try
				{
					response = request.GetResponse();
					Debug.Print("response.Headers.GetType()= " + response.Headers.GetType());
					exists = true;
				}
				catch (Exception ex)
				{
					exists = false;
				}

				Debug.Print("Photo exists? = " + exists);

				if (exists)
				{
					memberPhotoImage.Source = new UriImageSource
					{
						Uri = new Uri(Constants.images_URL + App.member.id + "_photo"),
						CachingEnabled = false,
						CacheValidity = new TimeSpan(0, 0, 0, 1)
					};
			
				}
				else
				{
					memberPhotoImage.Source = "iconadicionarfoto.png";
				}

				var memberPhotoImage_tap = new TapGestureRecognizer();
				memberPhotoImage_tap.Tapped += memberPhotoImageTapped;
				memberPhotoImage.GestureRecognizers.Add(memberPhotoImage_tap);

				/*relativeLayout.Children.Add(memberPhotoImage,
					xConstraint: Constraint.RelativeToParent((parent) =>
					{
						return (parent.Width / 2) - (80 * App.screenHeightAdapter);
					}),
					yConstraint: Constraint.Constant(0),
					widthConstraint: Constraint.Constant(160 * App.screenHeightAdapter),
					heightConstraint: Constraint.Constant(160 * App.screenHeightAdapter) // size of screen -80
				);*/
			}
            else
            {
                memberPhotoImage.Source = "iconadicionarfoto.png";

                var memberPhotoImage_tap = new TapGestureRecognizer();
                memberPhotoImage_tap.Tapped += memberPhotoImageTapped;
                memberPhotoImage.GestureRecognizers.Add(memberPhotoImage_tap);
            }



            relativeLayout.Children.Add(memberPhotoImage,
				xConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width / 2) - (90 * App.screenHeightAdapter);
				}),
				yConstraint: Constraint.Constant(0),
				widthConstraint: Constraint.Constant(180 * App.screenHeightAdapter),
				heightConstraint: Constraint.Constant(180 * App.screenHeightAdapter) // size of screen -80
			);

		}

		public CompleteRegistration_Profile_PageCS()
		{

            
            this.initLayout();
			this.initSpecificLayout();

			/*MessagingCenter.Subscribe<byte[]>(this, "memberPhoto", (args) =>
			{
				Device.BeginInvokeOnMainThread(() =>
				{
					showActivityIndicator();
					streamSource = new MemoryStream(args);
					memberPhotoImage.photo.Source = ImageSource.FromStream(() => new MemoryStream(args));


					MemberManager memberManager = new MemberManager();
					memberManager.Upload_Member_Photo(streamSource);
                    hideActivityIndicator();
                    //AUserDialogs.Instance.HideLoading();   //Hide loader
                });

			});*/

		}

		public void CreateStackButtons() {
            var width = Constants.ScreenWidth;
            Debug.WriteLine("App.member.socio_tipo AQUI = " + App.member.socio_tipo);
            Debug.WriteLine("width = " + width);
            Debug.WriteLine("App.screenWidthAdapter = " + App.screenWidthAdapter);

            var buttonWidth = 0.0;

            if ((App.member.socio_tipo == "individual")) // & ((DateTime.Now.Year - DateTime.Parse(App.member.birthdate).Year) < 18))
            {
                buttonWidth = (width - (50 * App.screenWidthAdapter)) / 3;
            }
            else
            {
                buttonWidth = (width - (50 * App.screenWidthAdapter)) / 2;
            }

            Debug.WriteLine("buttonWidth = " + buttonWidth);

            geralButton = new MenuButton("GERAL", buttonWidth, 40 * App.screenHeightAdapter);
            geralButton.button.Clicked += OnGeralButtonClicked;
            moradaButton = new MenuButton("MORADA", buttonWidth, 40 * App.screenHeightAdapter);
            moradaButton.button.Clicked += OnMoradaButtonClicked;

			if ((App.member.socio_tipo == "empresa") | (App.member.socio_tipo == "nao_socio"))
			{
                stackButtons = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    Children =
						{
							geralButton,
							moradaButton
						}
                };
                geralButton.activate();
                moradaButton.deactivate();
            }
			else if (App.member.socio_tipo == "individual") //& ((DateTime.Now.Year - DateTime.Parse(App.member.birthdate).Year) < 18))
			{
				encEducacaoButton = new MenuButton("ENC\nEDUCAÇÃO", buttonWidth, 40 * App.screenHeightAdapter);
				encEducacaoButton.button.Clicked += OnEncEducacaoButtonClicked;
                stackButtons = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    Children =
						{
							geralButton,
							moradaButton,
							encEducacaoButton,
							//infoEscolarButton
						}
                };

                geralButton.activate();
                encEducacaoButton.deactivate();
                moradaButton.deactivate();
            }
			/*else if ((App.member.socio_tipo == "individual") & ((DateTime.Now.Year - DateTime.Parse(App.member.birthdate).Year) >= 18))
			{
                stackButtons = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    Children =
					{
						geralButton,
						moradaButton
						//infoEscolarButton
					}
                };

                geralButton.activate();
                moradaButton.deactivate();
            }
			*/
			relativeLayout.Children.Add(stackButtons,
				xConstraint: Constraint.Constant(0),
				yConstraint: Constraint.Constant(200 * App.screenHeightAdapter),
				widthConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width);
				}),
				heightConstraint: Constraint.Constant(40 * App.screenHeightAdapter) // size of screen -80
				);


		}

		public void CreateGridButtons()
		{

			Image changePasswordImage = new Image
			{
				Source = "botaoalterarpass.png",
				Aspect = Aspect.AspectFit
			};

			TapGestureRecognizer changePasswordImage_tapEvent = new TapGestureRecognizer();
			changePasswordImage_tapEvent.Tapped += OnChangePasswordButtonClicked;
			changePasswordImage.GestureRecognizers.Add(changePasswordImage_tapEvent);

			/*relativeLayout.Children.Add(changePasswordImage,
				xConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width - 50);
				}),
				yConstraint: Constraint.Constant(0 * App.screenHeightAdapter),
				widthConstraint: Constraint.Constant(40 * App.screenHeightAdapter), // size of screen -80
				heightConstraint: Constraint.Constant(40 * App.screenHeightAdapter) // size of screen -80
			);
			*/
			//gridButtons.Children.Add(changePasswordImage, 0, 0);
		}

		public void CreateGridGeral()
		{

			gridGeral = new Grid { Padding = 0, HorizontalOptions = LayoutOptions.FillAndExpand };
            gridGeral.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            gridGeral.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridGeral.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridGeral.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridGeral.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridGeral.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridGeral.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridGeral.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            gridGeral.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridGeral.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto }); //GridLength.Auto
            gridGeral.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });

            FormLabel nameLabel = new FormLabel { Text = "NOME" };
            nameValue = new FormValueEdit(member.name);

            FormLabel emailLabel = new FormLabel { Text = "EMAIL" };
            emailValue = new FormValueEdit(member.email);

            FormLabel cc_numberLabel = new FormLabel { Text = "Nº IDENTIFICAÇÃO" };
            cc_number = new FormValueEdit(member.cc_number);

            FormLabel nifLabel = new FormLabel { Text = "NIF" };
			nifValue = new FormValueEdit(member.nif);

			FormLabel birthdateLabel = new FormLabel { Text = "DT. NASCIMENTO" };
			birthdateValue = new FormValueEditDate(member.birthdate);//?.ToString("yyyy-MM-dd"));

			List<string> gendersList = new List<string>();
			foreach (KeyValuePair<string, string> entry in Constants.genders)
			{
				gendersList.Add(entry.Value);
			}

			FormLabel genderLabel = new FormLabel { Text = "GÉNERO" };
			genderValue = new FormValueEditPicker(Constants.genders[member.gender], gendersList);


			List<string> countriesList = new List<string>();
			foreach (KeyValuePair<string, string> entry in Constants.countries)
			{
				countriesList.Add(entry.Value);
			}
			FormLabel countryLabel = new FormLabel { Text = "NACIONALIDADE" };
			countryValue = new FormValueEditPicker(Constants.countries[member.country], countriesList);

			FormLabel phoneLabel = new FormLabel { Text = "TELEFONE" };
			phoneValue = new FormValueEdit(member.phone, Keyboard.Telephone);

            FormLabel emergencyContactLabel = new FormLabel { Text = "CONTACTO EMERGÊNCIA", FontSize = App.itemTitleFontSize };

            FormLabel nameEmergencyContactLabel = new FormLabel { Text = "NOME" };
            nameEmergencyContactValue = new FormValueEdit(member.nameEmergencyContact);

            FormLabel phoneEmergencyContactLabel = new FormLabel { Text = "TELEFONE" };
            phoneEmergencyContactValue = new FormValueEdit(member.phoneEmergencyContact, Keyboard.Telephone);

            gridGeral.Children.Add(nameLabel, 0, 0);
            gridGeral.Children.Add(nameValue, 1, 0);

			gridGeral.Children.Add(emailLabel, 0, 1);
            gridGeral.Children.Add(emailValue, 1, 1);

            gridGeral.Children.Add(cc_numberLabel, 0, 2);
            gridGeral.Children.Add(cc_number, 1, 2);

            gridGeral.Children.Add(nifLabel, 0, 3);
            gridGeral.Children.Add(nifValue, 1, 3);

            gridGeral.Children.Add(birthdateLabel, 0, 4);
			gridGeral.Children.Add(birthdateValue, 1, 4);

			gridGeral.Children.Add(genderLabel, 0, 5);
			gridGeral.Children.Add(genderValue, 1, 5);

			gridGeral.Children.Add(countryLabel, 0, 6);
			gridGeral.Children.Add(countryValue, 1, 6);

			gridGeral.Children.Add(phoneLabel, 0, 7);
			gridGeral.Children.Add(phoneValue, 1, 7);

			/*gridGeral.Children.Add(emailLabel, 0, 7);
			gridGeral.Children.Add(emailValue, 1, 7);*/
		}

		async void editor_Focused(System.Object sender, Xamarin.Forms.FocusEventArgs e)
		{
			if (Device.RuntimePlatform == Device.iOS)
			{
				Debug.Print("scrollView.ScrollY = " + scrollView.ScrollY);
				scrollView.ScrollToAsync(scrollView.ScrollX, 300, true);
			}
			Debug.Print("scrollView.ScrollY after = " + scrollView.ScrollY);
		}

		public void CreateGridMorada()
		{

			gridMorada = new Grid { Padding = 10 };
			gridMorada.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridMorada.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridMorada.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridMorada.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto }); //GridLength.Auto
			gridMorada.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star }); //GridLength.Auto 


			FormLabel addressLabel = new FormLabel { Text = "ENDEREÇO" };
			addressValue = new FormValueEdit(member.address);

			FormLabel postalcodeLabel = new FormLabel { Text = "CÓD POSTAL" };
			postalcodeValue = new FormValueEditCodPostal(member.postalcode);

			FormLabel cityLabel = new FormLabel { Text = "LOCALIDADE" };
			cityValue = new FormValueEdit(member.city);


			/*gridMorada.Children.Add(phoneLabel, 0, 1);
			gridMorada.Children.Add(phoneValue, 1, 1);*/

			gridMorada.Children.Add(addressLabel, 0, 0);
			gridMorada.Children.Add(addressValue, 1, 0);

			gridMorada.Children.Add(postalcodeLabel, 0, 1);
			gridMorada.Children.Add(postalcodeValue, 1, 1);

			gridMorada.Children.Add(cityLabel, 0, 2);
			gridMorada.Children.Add(cityValue, 1, 2);
		}

		public void CreateGridEncEducacao()
		{

			gridEncEducacao = new Grid { Padding = 10 };
			gridEncEducacao.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridEncEducacao.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridEncEducacao.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridEncEducacao.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridEncEducacao.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto }); 
			gridEncEducacao.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });

			FormLabel EncEducacao1NomeLabel = new FormLabel { Text = "NOME" };
			EncEducacao1NomeValue = new FormValueEdit(member.name_enc1);

            FormLabel EncEducacao1MailLabel = new FormLabel { Text = "EMAIL" };
            EncEducacao1MailValue = new FormValueEdit(member.mail_enc1, Keyboard.Email);

            FormLabel EncEducacao1cc_numberLabel = new FormLabel { Text = "Nº IDENTIFICAÇÃO" };
            EncEducacao1cc_numberValue = new FormValueEdit(member.cc_number_enc1);

            FormLabel EncEducacao1PhoneLabel = new FormLabel { Text = "TELEFONE" };
			EncEducacao1PhoneValue = new FormValueEdit(member.phone_enc1, Keyboard.Telephone);

			gridEncEducacao.Children.Add(EncEducacao1NomeLabel, 0, 0);
			gridEncEducacao.Children.Add(EncEducacao1NomeValue, 1, 0);

            gridEncEducacao.Children.Add(EncEducacao1MailLabel, 0, 1);
            gridEncEducacao.Children.Add(EncEducacao1MailValue, 1, 1);

			gridEncEducacao.Children.Add(EncEducacao1cc_numberLabel, 0, 2);
            gridEncEducacao.Children.Add(EncEducacao1cc_numberValue, 1, 2);

            gridEncEducacao.Children.Add(EncEducacao1PhoneLabel, 0, 3);
			gridEncEducacao.Children.Add(EncEducacao1PhoneValue, 1, 3);



        }


        async void OnGeralButtonClicked(object sender, EventArgs e)
		{
			geralButton.activate();
			moradaButton.deactivate();
            if (encEducacaoButton != null)
            {
                encEducacaoButton.deactivate();
            }

			scrollView.Content = gridGeral;

			if (enteringPage == false) {
				//await UpdateMemberInfo(false);
				enteringPage = false;
			}
			

		}

		async void OnMoradaButtonClicked(object sender, EventArgs e)
		{
			Debug.WriteLine("OnMoradaButtonClicked");

			geralButton.deactivate();
			moradaButton.activate();
            if (encEducacaoButton != null)
            {
                encEducacaoButton.deactivate();
            }

            scrollView.Content = gridMorada;

			//await UpdateMemberInfo(false);
		}

		async void OnEncEducacaoButtonClicked(object sender, EventArgs e)
		{
			Debug.WriteLine("OnEncEducacaoButtonClicked");

			geralButton.deactivate();
			moradaButton.deactivate();
			if (encEducacaoButton != null)
			{
                encEducacaoButton.activate();
            }
			

			scrollView.Content = gridEncEducacao;

			//await UpdateMemberInfo(false);
		}

		async void OnChangePasswordButtonClicked(object sender, EventArgs e)
		{
			Navigation.PushModalAsync(new CompleteRegistration_ChangePassword_PageCS());
		}


		async Task<string> UpdateMemberInfo(bool validate)
		{
			Debug.Print("UpdateMemberInfo");
			if (App.member != null)
			{
				if (string.IsNullOrEmpty(postalcodeValue.entry.Text))
				{
					postalcodeValue.entry.Text = "";
				}
				if ((nameValue.entry.Text == "") | (CountWords(nameValue.entry.Text) < 2))
				{
					nameValue.entry.Text = App.member.name;
					DisplayAlert("DADOS INVÁLIDOS", "O nome introduzido não é válido.", "Ok");
					return "-1";
				}
				if ((phoneValue.entry.Text != "") & (phoneValue.entry.Text != null))
				{
					if ((phoneValue.entry.Text.Length > 0) & (phoneValue.entry.Text.Length < 9))
					{
                        await DisplayAlert("DADOS INVÁLIDOS", "O telefone introduzido tem de ter pelo menos 9 dígitos.", "OK");
						OnGeralButtonClicked(null, null);
						return "-1";
					}
					else if (!Regex.IsMatch(phoneValue.entry.Text, @"^\+?(\d[\d-. ]+)?(\([\d-. ]+\))?[\d-. ]+\d$"))
					{
						phoneValue.entry.Text = App.member.phone;
						OnGeralButtonClicked(null, null);
                        await DisplayAlert("DADOS INVÁLIDOS", "O telefone introduzido não é válido.", "OK");
                        //UserDialogs.Instance.Alert(new AlertConfig() { Title = "DADOS INVÁLIDOS", Message = "O telefone introduzido não é válido.", OkText = "Ok" });
                        return "-1";
					}
				}
				if ((postalcodeValue.entry.Text != "") & (!Regex.IsMatch((postalcodeValue.entry.Text), @"^\d{4}-\d{3}$")))
				{
					postalcodeValue.entry.Text = App.member.postalcode;

                    await DisplayAlert("DADOS INVÁLIDOS", "O código postal introduzido não é válido.", "OK");
                    //UserDialogs.Instance.Alert(new AlertConfig() { Title = "DADOS INVÁLIDOS", Message = "O código postal introduzido não é válido.", OkText = "Ok" });
					OnMoradaButtonClicked(null, null);
					return "-1";
				}

                App.member.name = nameValue.entry.Text;
                App.member.cc_number = cc_number.entry.Text;
                App.member.email = emailValue.entry.Text;
				App.member.birthdate = birthdateValue.entry.Text; //DateTime.Parse(birthdateValue.entry.Text);
				App.member.nif = nifValue.entry.Text;
				App.member.country = Constants.KeyByValue(Constants.countries, countryValue.picker.SelectedItem.ToString());
				App.member.gender = Constants.KeyByValue(Constants.genders, genderValue.picker.SelectedItem.ToString());
				App.member.phone = phoneValue.entry.Text;
				App.member.address = addressValue.entry.Text;
				App.member.city = cityValue.entry.Text;
				App.member.postalcode = postalcodeValue.entry.Text;
				App.member.nameEmergencyContact = nameEmergencyContactValue.entry.Text;
				App.member.phoneEmergencyContact = phoneEmergencyContactValue.entry.Text;
				App.member.name_enc1 = EncEducacao1NomeValue.entry.Text;
				App.member.phone_enc1 = EncEducacao1PhoneValue.entry.Text;
				App.member.mail_enc1 = EncEducacao1MailValue.entry.Text;
                App.member.cc_number_enc1 = EncEducacao1cc_numberValue.entry.Text;

                Debug.Print("App.member.country aqui11 = " + App.member.country);

                MemberManager memberManager = new MemberManager();
				string result = "";

				if (App.member.id == "")
				{
                    result = await memberManager.createNewMember(App.member);

                    if (result == "-1")
                    {
                        DisplayAlert("DADOS INVÁLIDOS", "Não foi possível criar o sócio pois existem dados em falta", "OK");
                        return "-1";
                    }
                    if (result == "-2")
					{
						DisplayAlert("MEMBRO JÁ EXISTENTE", "Já existe um membro com este Número de Identificação no nosso sistema.", "OK");
						return "-1";
					}

					App.member.id = result;
                }
				else
				{
                    result = await memberManager.UpdateMemberInfo(App.member.id, App.member);
                    return result;
                }
                

			}
			return "";
		}

		public void memberPhotoImageTapped(object sender, System.EventArgs e)
		{

			memberPhotoImageTappedAsync();


        }

		public async Task memberPhotoImageTappedAsync()
		{
			string result = await UpdateMemberInfo(true);

			if (result != "-1")
			{
				displayMemberPhotoImageActionSheet();
			}
		}

        public static async Task createNewMemberAsync()
		{
            MemberManager memberManager = new MemberManager();
            App.member.id = await memberManager.createNewMember(App.member);

        }

		async Task<string> displayMemberPhotoImageActionSheet()
		{
			var actionSheet = await DisplayActionSheet("Fotografia Sócio " + App.member.nickname, "Cancel", null, "Tirar Foto", "Galeria de Imagens");

			string result = "";
			switch (actionSheet)
			{
				case "Cancel":
					break;
				case "Tirar Foto":
					TakeAPhotoTapped();
					break;
				case "Galeria de Imagens":
					OpenGalleryTapped();
					break;
			}

			/*Device.BeginInvokeOnMainThread(() =>
			{
				var fileName = SetImageFileName();
				DependencyService.Get<CameraInterface>().LaunchCamera(FileFormatEnum.JPEG, fileName);
			});*/

			return "";
		}

        async void OpenGalleryTapped()
        {
            var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
            {
                Title = "Por favor escolha uma foto"
            });

            if (result != null)
            {
                Stream stream_aux = await result.OpenReadAsync();
                Stream localstream = await result.OpenReadAsync();

                memberPhotoImage.Source = ImageSource.FromStream(() => localstream);
                if (Device.RuntimePlatform == Device.iOS)
                {
                    memberPhotoImage.Rotation = 0;
                    stream = RotateBitmap(stream_aux, 0);
                }
                else
                {
                    memberPhotoImage.Rotation = 90;
                    stream = RotateBitmap(stream_aux, 90);
                }

                MemberManager memberManager = new MemberManager();
                memberManager.Upload_Member_Photo(stream);
            }
        }

        async void TakeAPhotoTapped()
        {
            var result = await MediaPicker.CapturePhotoAsync();

            if (result != null)
            {
                Stream stream_aux = await result.OpenReadAsync();
                Stream localstream = await result.OpenReadAsync();

                memberPhotoImage.Source = ImageSource.FromStream(() => localstream);
                memberPhotoImage.Rotation = 90;
                stream = RotateBitmap(stream_aux, 90);

				MemberManager memberManager = new MemberManager();
                memberManager.Upload_Member_Photo(stream);
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

       
        async void confirmPersonalDataButtonClicked(object sender, EventArgs e)
		{
            moveForward = true;
			string res = await UpdateMemberInfo(true);
			if (res != "-1")
			{
				MemberManager memberManager = new MemberManager();
				List<Member> members = await memberManager.GetMembers(App.member.email);
                Debug.Print("members.Count = " + members.Count);

                if (members.Count == 1)
				{
					Navigation.PushAsync(new CompleteRegistration_ChangePassword_PageCS());
				}
				else
				{
					Debug.Print("App.member.socio_tipo = " + App.member.socio_tipo);
                    if (App.member.socio_tipo == "empresa")
                    {
                        await Navigation.PushAsync(new CompleteRegistration_IdCard_PageCS("company"));
                    }
                    else if (App.member.socio_tipo == "individual")
                    {
                        await Navigation.PushAsync(new CompleteRegistration_IdCard_PageCS(""));
                    }
                    else if (App.member.socio_tipo == "nao_socio")
                    {
                        App.member.estado = "activo";
                        await memberManager.Update_Member_Approved_Status(App.member.id, App.member.name, App.member.email, App.member.estado);

                        await DisplayAlert("REGISTO CONCLUÍDO", "O Registo foi concluído com sucesso. Uma vez que este email já existe no sistema, deverá ligar-se à sua conta com as credenciais que já possui.", "OK"); ;
                        Application.Current.MainPage = new NavigationPage(new LoginPageCS(""))
                        {
                            BarBackgroundColor = Color.White,
                            BarTextColor = Color.Black
                        };

                    }
                }

            }
			else
			{
				//await DisplayAlert("UPDATE FALHOU", "A atualização de dados do membro falhou. Tente novamente mais tarde", "OK");
			}

		}

        public static int CountWords(string test)
        {
            int count = 0;
            bool wasInWord = false;
            bool inWord = false;

            for (int i = 0; i < test.Length; i++)
            {
                if (inWord)
                {
                    wasInWord = true;
                }

                if (Char.IsWhiteSpace(test[i]))
                {
                    if (wasInWord)
                    {
                        count++;
                        wasInWord = false;
                    }
                    inWord = false;
                }
                else
                {
                    inWord = true;
                }
            }

            // Check to see if we got out with seeing a word
            if (wasInWord)
            {
                count++;
            }

            return count;
        }
    }

}