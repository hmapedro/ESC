using System;
using System.Collections.Generic;
using Xamarin.Forms;
using SportNow.Model;
using SportNow.Services.Data.JSON;
using System.Threading.Tasks;
using System.Diagnostics;
using SportNow.CustomViews;
using System.Text.RegularExpressions;
using SportNow.Services.Camera;
using System.Net;
using System.IO;
using SkiaSharp;
using Xamarin.Essentials;
using System.Collections;
using System.Runtime.InteropServices.ComTypes;
using SportNow.Views.CompleteRegistration;
using System.Net.Http;

namespace SportNow.Views.Profile
{
    public class ProfilePageCS : DefaultPage
	{
		private ScrollView scrollView;

		private Member member;

		Label nameLabel;

        MenuButton geralButton;
		MenuButton moradaButton;
		//MenuButton socioTipoButton;
        MenuButton encEducacaoButton;


        StackLayout stackButtons;
		private Grid gridGeral;
		private Grid gridMorada;
		//private Grid gridTipoSocio;
		private Grid gridModalidade;
		private Grid gridEncEducacao;

        RoundImage memberPhotoImage;
		FormValueEdit nameValue, emailValue, nifValue;
        FormValueEditDate birthdateValue;
		FormValueEdit cc_numberValue;
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
		FormValueEdit EncEducacao2NomeValue;
		FormValueEdit EncEducacao2PhoneValue;
		FormValueEdit EncEducacao2MailValue;

		FormValueEditPicker tipoSocioValue;


        FormValueEditLongText comentariosValue;

		bool changeMember = false;

		bool enteringPage = true;

		Button activateButton;

		Label currentVersionLabel;

		Stream stream;
        ImageSource source;

		Image documentosImage, sportsImage, tipoSocioImage;
		Label sportsImageLabel;

        int documentosImage_y_index = 0, sportsImage_y_index = 0;

        //bool isDocument = false;

        protected async override void OnAppearing()
        {

            showActivityIndicator();

            Debug.Print("OnAppearing");

			if (documentosImage == null)
			{
                if (App.member.socio_tipo != "nao_socio")
                {
                    await createDocumentsButton(documentosImage_y_index);
                }
            }

            if (sportsImage == null)
            {
                if (App.member.socio_tipo == "individual")
                {
                    await createSportsButton(sportsImage_y_index);
                }
            }

            if (changeMember == true)
			{
				relativeLayout.Children.Remove(memberPhotoImage);
                relativeLayout.Children.Remove(gridGeral);
                //relativeLayout.Children.Remove(gridTipoSocio);
                relativeLayout.Children.Remove(gridMorada);
                relativeLayout.Children.Remove(gridEncEducacao);

				CreatePhoto();
                //CreateName();
                CreateGridGeral();
                //CreateGridTipoSocio();
                CreateGridMorada();
                CreateGridEncEducacao();
            }

            hideActivityIndicator();

        }

        protected async override void OnDisappearing()
        {
            Debug.Print("OnDisappearing");

			if (documentosImage != null)
			{
                relativeLayout.Children.Remove(documentosImage);
                documentosImage = null;
            }

            if (sportsImage != null)
            {
                relativeLayout.Children.Remove(sportsImage);
                sportsImage = null;
                relativeLayout.Children.Remove(sportsImageLabel);
                sportsImageLabel = null;
                
            }


            if (changeMember == false)
            {
                await UpdateMemberInfo();
            }			
        }

        public void initLayout()
		{
			Title = "PERFIL";

			var toolbarItem = new ToolbarItem
			{
				Text = "Logout"
			};
			toolbarItem.Clicked += OnLogoutButtonClicked;
			ToolbarItems.Add(toolbarItem);

		}


		public async Task<string> initSpecificLayout()
		{
			Debug.Print("ProfilePageCS - initSpecificLayout");
			if (relativeLayout == null)
			{
                relativeLayout = new RelativeLayout
                {
                    Margin = new Thickness(10)
                };
                Content = relativeLayout;
            }
            member = App.member;

			scrollView = new ScrollView { Orientation = ScrollOrientation.Vertical, HeightRequest = 300, IsClippedToBounds = true, BackgroundColor = Color.White };

			relativeLayout.Children.Add(scrollView,
				xConstraint: Constraint.Constant(0),
				yConstraint: Constraint.Constant(230 * App.screenHeightAdapter),
				widthConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width); // center of image (which is 40 wide)
				}),
				heightConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Height) - 300 * App.screenHeightAdapter; // center of image (which is 40 wide)
				})
			);

			int countStudents = App.original_member.students_count;

			CreatePhoto();
			//CreateName();
			CreateStackButtons();
			CreateGridGeral();
			CreateGridMorada();
			CreateGridEncEducacao();
			//CreateGridInfoEscolar();
			CreateGridButtons();

			/*gridIdentificacao.IsVisible = false;
			gridMorada.IsVisible = false;
			gridEncEducacao.IsVisible = false;*/
			OnGeralButtonClicked(null, null);

			return "";
		}

		public void CreateName()
		{
			nameLabel = new Label
			{
				Text = App.member.nickname,
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


            /*var nameLabel_tap = new TapGestureRecognizer();
            nameLabel_tap.Tapped += memberNameLabelTappedAsync;
            nameLabel.GestureRecognizers.Add(nameLabel_tap);*/
        }

        public async void CreatePhoto()
		{
            Debug.Print("CreatePhoto");

            memberPhotoImage = new RoundImage();

            if (App.member.id != "")
			{


                WebResponse response;
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(Constants.images_URL + App.member.id + "_photo/");

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
                memberPhotoImage_tap.Tapped += memberPhotoImageTappedAsync;
                memberPhotoImage.GestureRecognizers.Add(memberPhotoImage_tap);
            }
			else
			{
				memberPhotoImage.Source = "iconadicionarfoto.png";

				var memberPhotoImage_tap = new TapGestureRecognizer();
				memberPhotoImage_tap.Tapped += memberPhotoImageTapped_NotAuthorized_Async;
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

		public ProfilePageCS()
		{
            this.initLayout();
			this.initSpecificLayout();
        }

		public void CreateStackButtons()
		{
			Debug.Print("CreateStackButtons");
			var width = Constants.ScreenWidth;

            var buttonWidth = 0.0;

            if (((App.member.socio_tipo == "individual")) & ((DateTime.Now.Year - DateTime.Parse(App.member.birthdate).Year) < 18))
            {
                buttonWidth = (width - (50 * App.screenWidthAdapter)) / 3;
            }
            else
            {
                buttonWidth = (width - (50 * App.screenWidthAdapter)) / 2;
            }

            geralButton = new MenuButton("GERAL", buttonWidth, 40 * App.screenHeightAdapter);
            geralButton.button.Clicked += OnGeralButtonClicked;
            //socioTipoButton = new MenuButton("TIPO SÓCIO", buttonWidth, 40 * App.screenHeightAdapter);
            //socioTipoButton.button.Clicked += OnSocioTipoButtonClicked;
            moradaButton = new MenuButton("MORADA", buttonWidth, 40 * App.screenHeightAdapter);
            moradaButton.button.Clicked += OnMoradaButtonClicked;


            encEducacaoButton = new MenuButton("ENC\nEDUCAÇÃO", buttonWidth, 40 * App.screenHeightAdapter);
			encEducacaoButton.button.Clicked += OnEncEducacaoButtonClicked;

            if ((App.member.socio_tipo == "empresa") | (App.member.socio_tipo == "nao_socio"))
            {
                stackButtons = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    Children =
                        {
                            geralButton,
							//socioTipoButton,
                            moradaButton
                        }
                };
                geralButton.activate();
				//socioTipoButton.deactivate();
                moradaButton.deactivate();
            }
            else if ((App.member.socio_tipo == "individual") & ((DateTime.Now.Year - DateTime.Parse(App.member.birthdate).Year) < 18))
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
                            //socioTipoButton,
                            moradaButton,
                            encEducacaoButton
						}
                };

                geralButton.activate();
                //socioTipoButton.deactivate();
                encEducacaoButton.deactivate();
                moradaButton.deactivate();
            }
            else if ((App.member.socio_tipo == "individual") & ((DateTime.Now.Year - DateTime.Parse(App.member.birthdate).Year) >= 18))
			{
                stackButtons = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    Children =
					{
						geralButton,
						//socioTipoButton,
						moradaButton
					}
                };

                geralButton.activate();
                //socioTipoButton.deactivate();
                moradaButton.deactivate();
            }
			

            relativeLayout.Children.Add(stackButtons,
			xConstraint: Constraint.Constant(0),
			yConstraint: Constraint.Constant(180 * App.screenHeightAdapter),
			widthConstraint: Constraint.RelativeToParent((parent) =>
			{
				return (parent.Width);
			}),
			heightConstraint: Constraint.Constant(40 * App.screenHeightAdapter) // size of screen -80
			);

			geralButton.activate();
            //socioTipoButton.deactivate();
            moradaButton.deactivate();
			encEducacaoButton.deactivate();
		}

		public async void CreateGridButtons()
		{
            Debug.Print("CreateGridButtons");

			MemberManager memberManager = new MemberManager();

            int y_button_left = 0;
			int y_button_right = 0;

			Image changePasswordImage = new Image
			{
				Source = "botaoalterarpass.png",
				Aspect = Aspect.AspectFit
			};

			TapGestureRecognizer changePasswordImage_tapEvent = new TapGestureRecognizer();
			changePasswordImage_tapEvent.Tapped += OnChangePasswordButtonClicked;
			changePasswordImage.GestureRecognizers.Add(changePasswordImage_tapEvent);

			relativeLayout.Children.Add(changePasswordImage,
				xConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width - 47.5);
				}),
				yConstraint: Constraint.Constant(y_button_right * App.screenHeightAdapter),
				widthConstraint: Constraint.Constant(35 * App.screenHeightAdapter),
				heightConstraint: Constraint.Constant(35 * App.screenHeightAdapter)
			);

			//Debug.Print("y_button_right 0 = " + y_button_right);

			Label changePasswordLabel = new Label
			{
				Text = "Segurança",
				TextColor = App.normalTextColor,
				HorizontalTextAlignment = TextAlignment.Center,
				VerticalTextAlignment = TextAlignment.Start,
				FontSize = App.smallTextFontSize
			};

			relativeLayout.Children.Add(changePasswordLabel,
					xConstraint: Constraint.RelativeToParent((parent) =>
					{
						return (parent.Width - 60);
					}),
					yConstraint: Constraint.Constant((y_button_right + 37) * App.screenHeightAdapter),
					widthConstraint: Constraint.Constant(60 * App.screenHeightAdapter),
					heightConstraint: Constraint.Constant(15 * App.screenHeightAdapter)
			);

			//Debug.Print("y_button_right 1 = " + y_button_right);

			y_button_right = y_button_right + 60;


			Debug.Print("App.member.socio_tipo = " + App.member.socio_tipo);
			Debug.Print("App.member.member_type = " + App.member.member_type);

            
            if (App.member.socio_tipo != "nao_socio")
            {
                y_button_right = await createDocumentsButton(y_button_right);
            }


			if (App.members.Count > 1)
			{

				Button changeMemberButton = new Button { HorizontalOptions = LayoutOptions.Center, BackgroundColor = Color.Transparent, ImageSource = "botaoalmudarconta.png", HeightRequest = 30 };

				/*gridButtons.ColumnDefinitions.Add(new ColumnDefinition { Width = buttonWidth });
				//RoundButton changeMemberButton = new RoundButton("Login Outro Sócio", buttonWidth-5, 40);
				changeMemberButton.Clicked += OnChangeMemberButtonClicked;*/

				Image changeMemberImage = new Image
				{
					Source = "botaoalmudarconta.png",
					Aspect = Aspect.AspectFit
				};

				TapGestureRecognizer changeMemberImage_tapEvent = new TapGestureRecognizer();
				changeMemberImage_tapEvent.Tapped += OnChangeMemberButtonClicked;
				changeMemberImage.GestureRecognizers.Add(changeMemberImage_tapEvent);

				relativeLayout.Children.Add(changeMemberImage,
					xConstraint: Constraint.Constant(12.5 * App.screenHeightAdapter),
					yConstraint: Constraint.Constant(y_button_left * App.screenHeightAdapter),
					widthConstraint: Constraint.Constant(35 * App.screenHeightAdapter),
					heightConstraint: Constraint.Constant(35 * App.screenHeightAdapter)
				);

				Label changeMemberLabel = new Label
				{
					Text = "Mudar Utilizador",
					TextColor = App.normalTextColor,
					HorizontalTextAlignment = TextAlignment.Center,
					VerticalTextAlignment = TextAlignment.Start,
					FontSize = App.smallTextFontSize
				};

				relativeLayout.Children.Add(changeMemberLabel,
					xConstraint: Constraint.Constant(0),
					yConstraint: Constraint.Constant((y_button_left + 37) * App.screenHeightAdapter),
					widthConstraint: Constraint.Constant(60 * App.screenHeightAdapter),
					heightConstraint: Constraint.Constant(15 * App.screenHeightAdapter)
				);

				//Debug.Print("y_button_left 0 = " + y_button_left);

				y_button_left = y_button_left + 60;

				//Debug.Print("y_button_left 1 = " + y_button_left);
			}

			if (App.original_member.students_count > 1)
			{
				Image changeStudentImage = new Image
				{
					Source = "iconescolheraluno.png",
					Aspect = Aspect.AspectFit
				};

				TapGestureRecognizer changeStudentImage_tapEvent = new TapGestureRecognizer();
				changeStudentImage_tapEvent.Tapped += OnChangeStudentButtonClicked;
				changeStudentImage.GestureRecognizers.Add(changeStudentImage_tapEvent);

				relativeLayout.Children.Add(changeStudentImage,
					xConstraint: Constraint.Constant(12.5 * App.screenHeightAdapter),
					yConstraint: Constraint.Constant(y_button_left * App.screenHeightAdapter),
					widthConstraint: Constraint.Constant(35 * App.screenHeightAdapter),
					heightConstraint: Constraint.Constant(35 * App.screenHeightAdapter)
				);

				Label changeStudentLabel = new Label
				{
					Text = "Login Sócio",
					TextColor = App.normalTextColor,
					HorizontalTextAlignment = TextAlignment.Center,
					VerticalTextAlignment = TextAlignment.Start,
					FontSize = App.smallTextFontSize
				};

				relativeLayout.Children.Add(changeStudentLabel,
					xConstraint: Constraint.Constant(0),
					yConstraint: Constraint.Constant((y_button_left + 37) * App.screenHeightAdapter),
					widthConstraint: Constraint.Constant(60 * App.screenHeightAdapter),
					heightConstraint: Constraint.Constant(15 * App.screenHeightAdapter)
				);

				y_button_left = y_button_left + 60;

				//Debug.Print("y_button_left 2 = " + y_button_left);
			}

            y_button_left = await createTipoSocioButton(y_button_left);

            if (App.member.socio_tipo == "individual")
            {
                y_button_left = await createSportsButton(y_button_left);
            }

            currentVersionLabel = new Label
			{
				Text = "Version "+ App.VersionNumber + " "+App.BuildNumber,
				TextColor = App.topColor,
				HorizontalTextAlignment = TextAlignment.Start,
				VerticalTextAlignment = TextAlignment.Start,
				FontSize = 10 * App.screenHeightAdapter
			};

			relativeLayout.Children.Add(currentVersionLabel,
				xConstraint: Constraint.Constant(10),
				yConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Height - 20);
				}),
				widthConstraint: Constraint.Constant(150 * App.screenHeightAdapter),
				heightConstraint: Constraint.Constant(20 * App.screenHeightAdapter)
			);

		}

        public async Task<int> createTipoSocioButton(int y_button_left)
        {
            Debug.Print("createTipoSocioButton");

            tipoSocioImage = new Image
            {
                Source = "botaoalmudarconta.png",
                Aspect = Aspect.AspectFit
            };

            TapGestureRecognizer tipoSocioImage_tapEvent = new TapGestureRecognizer();
            tipoSocioImage_tapEvent.Tapped += OnTipoSocioButtonClicked;
            tipoSocioImage.GestureRecognizers.Add(tipoSocioImage_tapEvent);

            relativeLayout.Children.Add(tipoSocioImage,
                xConstraint: Constraint.Constant((12.5 * App.screenHeightAdapter)),
                yConstraint: Constraint.Constant(y_button_left * App.screenHeightAdapter),
                widthConstraint: Constraint.Constant(35 * App.screenHeightAdapter),
                heightConstraint: Constraint.Constant(35 * App.screenHeightAdapter)
            );

            Label tipoSocioImageLabel = new Label
            {
                Text = "Tipo Sócio",
                TextColor = App.normalTextColor,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Start,
                FontSize = App.smallTextFontSize
            };

            relativeLayout.Children.Add(tipoSocioImageLabel,
                xConstraint: Constraint.Constant(0),
                yConstraint: Constraint.Constant((y_button_left + 37) * App.screenHeightAdapter),
                widthConstraint: Constraint.Constant(60 * App.screenHeightAdapter),
                heightConstraint: Constraint.Constant(15 * App.screenHeightAdapter)
            );
            

            y_button_left = y_button_left + 60;

			sportsImage_y_index = y_button_left;

            return y_button_left;
        }


        public async Task<int> createSportsButton(int y_button_left)
        {
            Debug.Print("createSportsButton");

            sportsImage = new Image
            {
                Source = "botaoalmudarconta.png",
                Aspect = Aspect.AspectFit
            };

            TapGestureRecognizer sportsImage_tapEvent = new TapGestureRecognizer();
            sportsImage_tapEvent.Tapped += OnSportsButtonClicked;
            sportsImage.GestureRecognizers.Add(sportsImage_tapEvent);

            relativeLayout.Children.Add(sportsImage,
                xConstraint: Constraint.Constant((12.5 * App.screenHeightAdapter)),
                yConstraint: Constraint.Constant(y_button_left * App.screenHeightAdapter),
                widthConstraint: Constraint.Constant(35 * App.screenHeightAdapter),
                heightConstraint: Constraint.Constant(35 * App.screenHeightAdapter)
            );

            sportsImageLabel = new Label
            {
                Text = "Modalidades",
                TextColor = App.normalTextColor,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Start,
                FontSize = App.smallTextFontSize
            };

            relativeLayout.Children.Add(sportsImageLabel,
				xConstraint: Constraint.Constant(0),
                yConstraint: Constraint.Constant((y_button_left + 37) * App.screenHeightAdapter),
                widthConstraint: Constraint.Constant(60 * App.screenHeightAdapter),
                heightConstraint: Constraint.Constant(15 * App.screenHeightAdapter)
            );
            sportsImage_y_index = y_button_left;

            y_button_left = y_button_left + 60;

            return y_button_left;
        }

        public async Task<int> createDocumentsButton(int y_button_right)
		{
			Debug.Print("createDocumentsButton");

			documentosImage = new Image
			{
				Aspect = Aspect.AspectFit
			};

            MemberManager memberManager = new MemberManager();
            App.member.documents = await memberManager.Get_Member_Documents(App.member.id);


            string docID_status = "";
			DateTime docID_approved_expiration_date = DateTime.Now;
			DateTime docID_review_expiration_date = DateTime.Now;
			string docID_parent_status = "";
			DateTime docID_parent_approved_expiration_date = DateTime.Now;
			DateTime docID_parent_review_expiration_date = DateTime.Now;

			string atestado_status = "";
			DateTime atestado_approved_expiration_date = DateTime.Now;
			DateTime atestado_review_expiration_date = DateTime.Now;

			foreach (Document document in App.member.documents)
			{
				if (document.type == "documento_identificacao")
				{
					docID_status = document.status;
					if (docID_status == "Aprovado")
					{
						if (DateTime.Compare(DateTime.Parse(document.expiry_date).Date, docID_approved_expiration_date) > 0)
						{
							docID_approved_expiration_date = DateTime.Parse(document.expiry_date).Date;
						}
					}
					else if (docID_status == "Under Review")
					{
						if (DateTime.Compare(DateTime.Parse(document.expiry_date).Date, docID_review_expiration_date) > 0)
						{
							docID_review_expiration_date = DateTime.Parse(document.expiry_date).Date;
						}
					}
				}
				if (document.type == "documento_identificacao_encarregado")
				{
					docID_parent_status = document.status;
					if (docID_parent_status == "Aprovado")
					{
						if (DateTime.Compare(DateTime.Parse(document.expiry_date).Date, docID_parent_approved_expiration_date) > 0)
						{
							docID_parent_approved_expiration_date = DateTime.Parse(document.expiry_date).Date;
						}
					}
					else if (docID_parent_status == "Under Review")
					{
						if (DateTime.Compare(DateTime.Parse(document.expiry_date).Date, docID_parent_review_expiration_date) > 0)
						{
							docID_parent_review_expiration_date = DateTime.Parse(document.expiry_date).Date;
						}
					}
				}
				if (document.type == "atestado_medico")
				{
					atestado_status = document.status;
					if (atestado_status == "Aprovado")
					{
						if (DateTime.Compare(DateTime.Parse(document.expiry_date).Date, atestado_approved_expiration_date) > 0)
						{
							atestado_approved_expiration_date = DateTime.Parse(document.expiry_date).Date;
						}
					}
					else if (atestado_status == "Under Review")
					{
						if (DateTime.Compare(DateTime.Parse(document.expiry_date).Date, atestado_review_expiration_date) > 0)
						{
							atestado_review_expiration_date = DateTime.Parse(document.expiry_date).Date;
						}
					}
				}
			}
            Debug.Print("AQUI");

            if (App.member.socio_tipo == "empresa")
			{
                Debug.Print("Sócio é Empresa");
                if ((docID_status == "Aprovado") & (DateTime.Compare(docID_approved_expiration_date, DateTime.Now) > 0))
				{
					documentosImage.Source = "icondocumentosaprovados.png";
				}
				else if ((docID_status == "Under Review") & (DateTime.Compare(docID_review_expiration_date, DateTime.Now) > 0))
				{
					documentosImage.Source = "icondocumentosporconfirmar.png";
				}
				else
				{
					documentosImage.Source = "icondocumentosrejeitados.png";
				}
			}
            else if (App.member.socio_tipo == "individual")
            {
				Debug.Print("Sócio é Individual");
                Debug.Print("App.member.federado_tipo = "+ App.member.federado_tipo);
                Debug.Print("App.member.birthdate = " + App.member.birthdate);
                //MAIOR E NÃO ATLETA NEM TREINADOR
                if (((DateTime.Now.Year - DateTime.Parse(App.member.birthdate).Year) >= 18) & ((!App.member.federado_tipo.Contains("atleta")) & (!App.member.federado_tipo.Contains("treinador"))))
                {
                    Debug.Print("Sócio é Individual MAIOR E NÃO ATLETA NEM TREINADOR");
                    if ((docID_status == "Aprovado") & (DateTime.Compare(docID_approved_expiration_date, DateTime.Now) > 0))
                    {
                        documentosImage.Source = "icondocumentosaprovados.png";
                    }
                    else if ((docID_status == "Under Review") & (DateTime.Compare(docID_review_expiration_date, DateTime.Now) > 0))
                    {
                        documentosImage.Source = "icondocumentosporconfirmar.png";
                    }
                    else
                    {
                        documentosImage.Source = "icondocumentosrejeitados.png";
                    }
                }
                //MAIOR E ATLETA OU TREINADOR
                else if (((DateTime.Now.Year - DateTime.Parse(App.member.birthdate).Year) >= 18) & ((App.member.federado_tipo.Contains("atleta")) | (App.member.federado_tipo.Contains("treinador"))))
                {
                    Debug.Print("Sócio é Individual MAIOR E ATLETA OU TREINADOR");
                    if (((docID_status == "Aprovado") & (DateTime.Compare(docID_approved_expiration_date, DateTime.Now) > 0)) & ((atestado_status == "Aprovado") & (DateTime.Compare(atestado_approved_expiration_date, DateTime.Now) > 0)))
                    {
                        documentosImage.Source = "icondocumentosaprovados.png";
                    }
                    else if (((docID_status == "Under Review") & (DateTime.Compare(docID_review_expiration_date, DateTime.Now) > 0)) & ((atestado_status == "Under Review") & (DateTime.Compare(atestado_review_expiration_date, DateTime.Now) > 0)))
                    {
                        documentosImage.Source = "icondocumentosporconfirmar.png";
                    }
                    else
                    {
                        documentosImage.Source = "icondocumentosrejeitados.png";
                    }
                }
                //MENOR E NÃO ATLETA NEM TREINADOR
                else if (((DateTime.Now.Year - DateTime.Parse(App.member.birthdate).Year) < 18) & ((!App.member.federado_tipo.Contains("atleta")) & (!App.member.federado_tipo.Contains("treinador"))))
                {
                    Debug.Print("Sócio é Individual MENOR E NÃO ATLETA NEM TREINADOR");
                    if (((docID_status == "Aprovado") & (DateTime.Compare(docID_approved_expiration_date, DateTime.Now) > 0)) & ((docID_parent_status == "Aprovado") & (DateTime.Compare(docID_parent_approved_expiration_date, DateTime.Now) > 0)))
                    {
                        documentosImage.Source = "icondocumentosaprovados.png";
                    }
                    else if (((docID_status == "Under Review") & (DateTime.Compare(docID_review_expiration_date, DateTime.Now) > 0)) & ((docID_parent_status == "Under Review") & (DateTime.Compare(docID_parent_review_expiration_date, DateTime.Now) > 0)))
                    {
                        documentosImage.Source = "icondocumentosporconfirmar.png";
                    }
                    else
                    {
                        documentosImage.Source = "icondocumentosrejeitados.png";
                    }
                }
                //MENOR E ATLETA OU TREINADOR
				// TEM DE TER OS DOCUMENTOS TODOS DOCID, DOCID PARENT, ATESTADO
                else if (((DateTime.Now.Year - DateTime.Parse(App.member.birthdate).Year) < 18) & ((App.member.federado_tipo.Contains("atleta")) | (App.member.federado_tipo.Contains("treinador"))))
                {
                    Debug.Print("Sócio é Individual MENOR E ATLETA OU TREINADOR");
                    if (((docID_status == "Aprovado") & (DateTime.Compare(docID_approved_expiration_date, DateTime.Now) > 0)) & ((docID_parent_status == "Aprovado") & (DateTime.Compare(docID_parent_approved_expiration_date, DateTime.Now) > 0))
                        & ((atestado_status == "Aprovado") & (DateTime.Compare(atestado_approved_expiration_date, DateTime.Now) > 0)))
                    {
                        documentosImage.Source = "icondocumentosaprovados.png";
                    }
                    else if (((docID_status == "Under Review") & (DateTime.Compare(docID_review_expiration_date, DateTime.Now) > 0)) & ((docID_parent_status == "Under Review") & (DateTime.Compare(docID_parent_review_expiration_date, DateTime.Now) > 0))
                        & ((atestado_status == "Under Review") & (DateTime.Compare(atestado_review_expiration_date, DateTime.Now) > 0)))
                    {
                        documentosImage.Source = "icondocumentosporconfirmar.png";
                    }
                    else
                    {
                        documentosImage.Source = "icondocumentosrejeitados.png";
                    }
                }
            }


            TapGestureRecognizer documentosImage_tapEvent = new TapGestureRecognizer();
			documentosImage_tapEvent.Tapped += OnDocumentosButtonClicked;
			documentosImage.GestureRecognizers.Add(documentosImage_tapEvent);

			relativeLayout.Children.Add(documentosImage,
				xConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width - 47.5);
				}),
				yConstraint: Constraint.Constant(y_button_right * App.screenHeightAdapter),
				widthConstraint: Constraint.Constant(35 * App.screenHeightAdapter),
				heightConstraint: Constraint.Constant(35 * App.screenHeightAdapter)
			);

			Label documentosImageLabel = new Label
			{
				Text = "Documentos",
				TextColor = App.normalTextColor,
				HorizontalTextAlignment = TextAlignment.Center,
				VerticalTextAlignment = TextAlignment.Start,
				FontSize = App.smallTextFontSize
			};

			relativeLayout.Children.Add(documentosImageLabel,
				xConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width - 60);
				}),
				yConstraint: Constraint.Constant((y_button_right + 37) * App.screenHeightAdapter),
				widthConstraint: Constraint.Constant(60 * App.screenHeightAdapter),
				heightConstraint: Constraint.Constant(15 * App.screenHeightAdapter)
			);
            documentosImage_y_index = y_button_right;

            y_button_right = y_button_right + 60;

            return y_button_right;
		}

        /*public void CreateGridTipoSocio()
        {
            gridTipoSocio = new Grid { Padding = 0, HorizontalOptions = LayoutOptions.FillAndExpand };
            gridTipoSocio.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            gridTipoSocio.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            gridTipoSocio.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            gridTipoSocio.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto }); //GridLength.Auto
            gridTipoSocio.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });

            FormLabel tipoSocioLabel = new FormLabel { Text = "TIPO SÓCIO" };
            List<string> typeMemberList = new List<string>();
            foreach (var typeMember in Constants.memberTypeList)
            {
                typeMemberList.Add(typeMember.Value);
            }
            tipoSocioValue = new FormValueEditPicker(Constants.memberTypeList[App.member.socio_tipo], typeMemberList);


            gridEncEducacao.Children.Add(tipoSocioLabel, 0, 0);
            gridEncEducacao.Children.Add(tipoSocioValue, 1, 0);

        }*/


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
            cc_numberValue = new FormValueEdit(member.cc_number);

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
            gridGeral.Children.Add(cc_numberValue, 1, 2);

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
			gridEncEducacao.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridEncEducacao.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridEncEducacao.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridEncEducacao.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridEncEducacao.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridEncEducacao.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridEncEducacao.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridEncEducacao.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
			gridEncEducacao.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });

			FormLabel emergencyContactLabel = new FormLabel { Text = "CONTACTO EMERGÊNCIA", FontSize = App.itemTitleFontSize };

			FormLabel nameEmergencyContactLabel = new FormLabel { Text = "NOME" };
			nameEmergencyContactValue = new FormValueEdit(member.nameEmergencyContact);

			FormLabel phoneEmergencyContactLabel = new FormLabel { Text = "TELEFONE" };
			phoneEmergencyContactValue = new FormValueEdit(member.phoneEmergencyContact, Keyboard.Telephone);

			FormLabel EncEducacao1Label = new FormLabel { Text = "ENCARREGADO DE EDUCAÇÃO 1", FontSize = App.itemTitleFontSize };

			FormLabel EncEducacao1NomeLabel = new FormLabel { Text = "NOME" };
			EncEducacao1NomeValue = new FormValueEdit(member.name_enc1);

			FormLabel EncEducacao1PhoneLabel = new FormLabel { Text = "TELEFONE" };
			EncEducacao1PhoneValue = new FormValueEdit(member.phone_enc1, Keyboard.Telephone);

			FormLabel EncEducacao1MailLabel = new FormLabel { Text = "MAIL" };
			EncEducacao1MailValue = new FormValueEdit(member.mail_enc1, Keyboard.Email);

			FormLabel EncEducacao2Label = new FormLabel { Text = "ENCARREGADO DE EDUCAÇÃO 2", FontSize = App.itemTitleFontSize };

			FormLabel EncEducacao2NomeLabel = new FormLabel { Text = "NOME" };
			EncEducacao2NomeValue = new FormValueEdit(member.name_enc2);

			FormLabel EncEducacao2PhoneLabel = new FormLabel { Text = "TELEFONE" };
			EncEducacao2PhoneValue = new FormValueEdit(member.phone_enc2, Keyboard.Telephone);

			FormLabel EncEducacao2MailLabel = new FormLabel { Text = "MAIL" };
			EncEducacao2MailValue = new FormValueEdit(member.mail_enc2, Keyboard.Email);


			gridEncEducacao.Children.Add(emergencyContactLabel, 0, 0);
			Grid.SetColumnSpan(emergencyContactLabel, 2);

			gridEncEducacao.Children.Add(nameEmergencyContactLabel, 0, 1);
			gridEncEducacao.Children.Add(nameEmergencyContactValue, 1, 1);

			gridEncEducacao.Children.Add(phoneEmergencyContactLabel, 0, 2);
			gridEncEducacao.Children.Add(phoneEmergencyContactValue, 1, 2);


			gridEncEducacao.Children.Add(EncEducacao1Label, 0, 3);
			Grid.SetColumnSpan(EncEducacao1Label, 2);

			gridEncEducacao.Children.Add(EncEducacao1NomeLabel, 0, 4);
			gridEncEducacao.Children.Add(EncEducacao1NomeValue, 1, 4);

			gridEncEducacao.Children.Add(EncEducacao1PhoneLabel, 0, 5);
			gridEncEducacao.Children.Add(EncEducacao1PhoneValue, 1, 5);

			gridEncEducacao.Children.Add(EncEducacao1MailLabel, 0, 6);
			gridEncEducacao.Children.Add(EncEducacao1MailValue, 1, 6);

			gridEncEducacao.Children.Add(EncEducacao2Label, 0, 7);
			Grid.SetColumnSpan(EncEducacao2Label, 2);

			gridEncEducacao.Children.Add(EncEducacao2NomeLabel, 0, 8);
			gridEncEducacao.Children.Add(EncEducacao2NomeValue, 1, 8);

			gridEncEducacao.Children.Add(EncEducacao2PhoneLabel, 0, 9);
			gridEncEducacao.Children.Add(EncEducacao2PhoneValue, 1, 9);

			gridEncEducacao.Children.Add(EncEducacao2MailLabel, 0, 10);
			gridEncEducacao.Children.Add(EncEducacao2MailValue, 1, 10);
		}


		async void OnGeralButtonClicked(object sender, EventArgs e)
		{
			geralButton.activate();
            //socioTipoButton.deactivate();
            moradaButton.deactivate();
			encEducacaoButton.deactivate();

			scrollView.Content = gridGeral;

			if (enteringPage == false)
			{
				await UpdateMemberInfo();
				enteringPage = false;
			}


		}

		async void OnMoradaButtonClicked(object sender, EventArgs e)
		{
			Debug.WriteLine("OnMoradaButtonClicked");

			geralButton.deactivate();
            //socioTipoButton.deactivate();
            moradaButton.activate();
			encEducacaoButton.deactivate();

			scrollView.Content = gridMorada;

			await UpdateMemberInfo();
		}

		async void OnEncEducacaoButtonClicked(object sender, EventArgs e)
		{
			Debug.WriteLine("OnEncEducacaoButtonClicked");

			geralButton.deactivate();
            //socioTipoButton.deactivate();
            moradaButton.deactivate();
			encEducacaoButton.activate();

			scrollView.Content = gridEncEducacao;

			await UpdateMemberInfo();
		}


        async void OnSocioTipoButtonClicked(object sender, EventArgs e)
        {
            Debug.WriteLine("OnSocioTipoButtonClicked");

            geralButton.deactivate();
            //socioTipoButton.activate();
			moradaButton.deactivate();
            encEducacaoButton.deactivate();


            //scrollView.Content = gridTipoSocio;

            await UpdateMemberInfo();
        }

        async void OnLogoutButtonClicked(object sender, EventArgs e)
		{
			Debug.WriteLine("OnLogoutButtonClicked");

			Application.Current.Properties.Remove("EMAIL");
			Application.Current.Properties.Remove("PASSWORD");
			Application.Current.Properties.Remove("SELECTEDUSER");

			App.member = null;
			App.members = null;

			Application.Current.SavePropertiesAsync();

			Application.Current.MainPage = new NavigationPage(new LoginPageCS(""))
			{
				BarBackgroundColor = Color.FromRgb(15, 15, 15),
				BarTextColor = Color.White//FromRgb(75, 75, 75)
			};
		}

		async void OnChangePasswordButtonClicked(object sender, EventArgs e)
		{
			Navigation.PushAsync(new ChangePasswordPageCS(member));
		}


		async void OnConsentimentosButtonClicked(object sender, EventArgs e)
		{
			Navigation.PushAsync(new ConsentPageCS());
		}

		async void OnDocumentosButtonClicked(object sender, EventArgs e)
		{
			//isDocument = true;
			Navigation.PushAsync(new DocumentsPageCS());
		}


        async void OnTipoSocioButtonClicked(object sender, EventArgs e)
        {
            //isDocument = true;
            Navigation.PushAsync(new Profile_MemberType_PageCS());
        }

        async void OnSportsButtonClicked(object sender, EventArgs e)
        {
            //isDocument = true;
            Navigation.PushAsync(new Profile_Sports_PageCS());
        }

        async void membersToApproveImage_Clicked(object sender, EventArgs e)
		{
			Navigation.PushAsync(new ApproveRegistrationPageCS());
		}
		

		async void OnChangeMemberButtonClicked(object sender, EventArgs e)
		{

            await UpdateMemberInfo();
            changeMember = true;
			Navigation.PushAsync(new SelectMemberPageCS());
			//await Navigation.PopAsync();
			//await Navigation.PopAsync();
		}

		async void OnChangeStudentButtonClicked(object sender, EventArgs e)
		{
            await UpdateMemberInfo();
            changeMember = true;
			Navigation.PushAsync(new SelectStudentPageCS());
			//await Navigation.PopAsync();
			//await Navigation.PopAsync();

			//Navigation.PushAsync(new SelectStudentPageCS());
		}

		/*async void OnBackOriginalButtonClicked(object sender, EventArgs e)
		{
			//changeMember = true;
			App.member = App.original_member;
			//Navigation.PushAsync(new MainTabbedPageCS(""));
			Navigation.InsertPageBefore(new MainTabbedPageCS("",""), this);
			await Navigation.PopAsync();
			await Navigation.PopAsync();

		}*/


		async Task<int> GetCurrentFees(Member member)
		{
			Debug.WriteLine("GetCurrentFees");
			MemberManager memberManager = new MemberManager();

			var result = await memberManager.GetCurrentFees(member);
			if (result == -1)
			{
				Application.Current.MainPage = new NavigationPage(new LoginPageCS("Verifique a sua ligação à Internet e tente novamente."))
				{
					BarBackgroundColor = Color.White,
					BarTextColor = Color.Black
				};
				return -1;
			}
			return result;
		}


		async Task<string> UpdateMemberInfo()
		{
			Debug.Print("UpdateMemberInfo");
			if (App.member != null)
			{
				if (string.IsNullOrEmpty(postalcodeValue.entry.Text))
				{
					postalcodeValue.entry.Text = "";
				}
				/*if (nameValue.entry.Text == "")
				{
					nameValue.entry.Text = App.member.name;
					UserDialogs.Instance.Alert(new AlertConfig() { Title = "DADOS INVÁLIDOS", Message = "O nome introduzido não é válido.", OkText = "Ok" });
					return "-1";
				}*/
				if ((phoneValue.entry.Text != "") & (phoneValue.entry.Text != null))
				{
					if ((phoneValue.entry.Text.Length > 0) & (phoneValue.entry.Text.Length < 9))
					{
                        await DisplayAlert("DADOS INVÁLIDOS", "O telefone introduzido tem de ter pelo menos 9 dígitos.", "OK");
                        //UserDialogs.Instance.Alert(new AlertConfig() { Title = "DADOS INVÁLIDOS", Message = "O telefone introduzido tem de ter pelo menos 9 dígitos.", OkText = "Ok" });
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
				App.member.email = emailValue.entry.Text;
				App.member.birthdate = birthdateValue.entry.Text; //DateTime.Parse(birthdateValue.entry.Text);
				App.member.nif = nifValue.entry.Text;
				App.member.cc_number = cc_numberValue.entry.Text;
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
				App.member.name_enc2 = EncEducacao2NomeValue.entry.Text;
				App.member.phone_enc2 = EncEducacao2PhoneValue.entry.Text;
				App.member.mail_enc2 = EncEducacao2MailValue.entry.Text;

				MemberManager memberManager = new MemberManager();
				var result = await memberManager.UpdateMemberInfo(App.original_member.id, App.member);
				if (result == "-1")
				{
					Application.Current.MainPage = new NavigationPage(new LoginPageCS("Verifique a sua ligação à Internet e tente novamente."))
					{
						BarBackgroundColor = Color.FromRgb(15, 15, 15),
						BarTextColor = Color.White
					};
					return "-1";
				}
				return result;
			}
			return "";
		}

		void memberPhotoImageTappedAsync(object sender, System.EventArgs e)
		{
			displayMemberPhotoImageActionSheet();
		}

        void memberNameLabelTappedAsync(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new QuotasListPageCS());
        }


        async void memberPhotoImageTapped_NotAuthorized_Async(object sender, System.EventArgs e)
		{
			//DisplayAlert("Consentimento Foto Sócio", "Para poder fazer upload da sua foto, tem de dar o seu consentimento para que possamos fazer o tratamento da sua foto. Aceda a 'Consentimentos' e escolha essa opção. ", "OK");
			bool display_result = await DisplayAlert("Consentimento Foto Sócio", "Para poder fazer upload da sua foto, tem de dar o seu consentimento para que possamos fazer o tratamento da mesma. Pretende dar consentimento?", "Sim", "Não");
			if (display_result == true)
			{
				showActivityIndicator();

				MemberManager memberManager = new MemberManager();

				relativeLayout.Children.Remove(memberPhotoImage);
				CreatePhoto();

				//var result = await memberManager.Update_Member_Authorizations(App.member.id, App.member.consentimento_assembleia, App.member.consentimento_regulamento, App.member.consentimento_dados, App.member.consentimento_imagem, App.member.consentimento_fotosocio, App.member.consentimento_whatsapp);
                hideActivityIndicator();
            }

		}


		async Task<string> displayMemberPhotoImageActionSheet()
		{
			var actionSheet = await DisplayActionSheet("Fotografia Sócio " + App.member.nickname, "Cancel", null, "Tirar Foto", "Galeria de Imagens");

			MemberManager memberManager = new MemberManager();
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
            var result = await MediaPicker.CapturePhotoAsync ();

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

        /*void TakeAPhotoTapped()
		{
			Device.BeginInvokeOnMainThread(() =>
			{
				var fileName = "memberPhoto";//SetImageFileName();
				Debug.Print("fileName TakeAPhotoTapped = " + fileName);
				DependencyService.Get<CameraInterface>().LaunchCamera(FileFormatEnum.JPEG, fileName);
			});
		}

		void OpenGalleryTapped()
		{
			Debug.Print("OpenGalleryTapped");

			Device.BeginInvokeOnMainThread(() =>
			{
				var fileName = "memberPhoto";//SetImageFileName();
				Debug.Print("fileName OpenGalleryTapped = " + fileName);
				try
				{
					CameraInterface cameraInterface = DependencyService.Get<CameraInterface>();
					cameraInterface.LaunchGallery(FileFormatEnum.JPEG, fileName);

				}
				catch (Exception ex)
				{
					System.Diagnostics.Debug.Print(ex.Message);
					System.Diagnostics.Debug.Print(ex.StackTrace);
				}
				//loadedDocument.Source = fileName;
			});
		}*/

        private string SetImageFileName(string fileName = null)
		{
			if (Device.RuntimePlatform == Device.Android)
			{
				if (fileName != null)
					App.ImageIdToSave = fileName;
				else
					App.ImageIdToSave = App.DefaultImageId;

				return App.ImageIdToSave;
			}
			else
			{
				if (fileName != null)
				{
					App.ImageIdToSave = fileName;
					return fileName;
				}
				else
					return null;
			}
		}
	}
}