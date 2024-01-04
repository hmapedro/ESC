using System;
using System.Collections.Generic;
using Xamarin.Forms;
using SportNow.Model;
using SportNow.Services.Data.JSON;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Globalization;
//Ausing Acr.UserDialogs;
using System.Linq;
using SportNow.ViewModel;
using Xamarin.Essentials;
using Plugin.DeviceOrientation;
using Plugin.DeviceOrientation.Abstractions;
using SportNow.Views.Profile;
using System.Collections;
using System.Runtime.CompilerServices;
using Xamarin.Forms.Shapes;
using static System.Net.Mime.MediaTypeNames;
using SportNow.Views.Ranking;
using SportNow.CustomViews;
using System.Net;

namespace SportNow.Views
{
	public class MainPageCS : DefaultPage
	{

		protected async override void OnAppearing()
		{
            /*base.OnAppearing();
			CrossDeviceOrientation.Current.LockOrientation(DeviceOrientations.Portrait);*/

            var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;

			//Constants.ScreenWidth = mainDisplayInfo.Width;
			//Constants.ScreenHeight = mainDisplayInfo.Height;
			//Debug.Print("AQUI 1 - ScreenWidth = " + Constants.ScreenWidth + " ScreenHeight = " + Constants.ScreenHeight + "mainDisplayInfo.Density = " + mainDisplayInfo.Density);

			Constants.ScreenWidth = Xamarin.Forms.Application.Current.MainPage.Width;
			Constants.ScreenHeight = Xamarin.Forms.Application.Current.MainPage.Height;
            //Debug.Print("AQUI 0 - ScreenWidth = " + Constants.ScreenWidth + " ScreenHeight = " + Constants.ScreenHeight);

            App.AdaptScreen();
            initSpecificLayout();
            var second = TimeSpan.FromSeconds(1);
            int i = 0;
            bool goingUp = true;
            pageIsOn = true;
/*            Device.StartTimer(second, () => {

                if (pageIsOn == false)
                {
                    return false;
                }
                sponsorsCollectionView.ScrollTo(i);
                if (goingUp == true)
                {
                    i = i + 1;
                }
                else
                {
                    i = i - 1;
                }

                
                if (i > sponsors.Count())
                {
                    goingUp = false;

                }
                else if (i < 0)
                {
                    goingUp = true;
                    i = 0;
                }
                return true;
            });*/
        }

		protected override void OnDisappearing()
		{
			this.CleanScreen();
            pageIsOn = false;

        }

        public bool pageIsOn = false;

		public List<MainMenuItem> MainMenuItems { get; set; }

		Label msg;

		private CollectionView importantEventsCollectionView;
		private CollectionView sponsorsCollectionView;

		Label usernameLabel, eventsLabel;
		Label labelCurrentRanking, labelCurrentPoints;

		private List<Event> importantEvents;

        RoundImage memberPhotoImage;
        List<Sponsor> sponsors;

		int eventsHeight = 0;

		public void CleanScreen()
		{

			relativeLayout = null;
            relativeLayout = new RelativeLayout
            {
                Margin = new Thickness(10)
            };

            /*//Debug.Print("CleanScreen");
            //valida se os objetos já foram criados antes de os remover
            if (usernameLabel != null)
			{
				relativeLayout.Children.Remove(usernameLabel);
				usernameLabel = null;
			}


			if (currentVersionLabel != null)
			{
				relativeLayout.Children.Remove(currentVersionLabel);
				currentVersionLabel = null;
			}
			*/
		}

		public void initLayout()
		{
			Title = "PRINCIPAL";

            this.BackgroundColor = Color.FromRgb(255, 255, 255);

            relativeLayout = new RelativeLayout
            {
                Margin = new Thickness(10)
            };

            ScrollView scrollView = new ScrollView { Content = relativeLayout, Orientation = ScrollOrientation.Vertical };

			Content = scrollView;

            var toolbarItem = new ToolbarItem
			{
				IconImageSource = "perfil.png"
			};
			toolbarItem.Clicked += OnPerfilButtonClicked;
			ToolbarItems.Add(toolbarItem);
        }


        public async void initSpecificLayout()
		{
            Debug.Print("MainPageCS - initSpecificLayout - 0001");
            /*relativeLayout.Children.Add(activityIndicator,
                xConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return (parent.Width / 2) - (50 * App.screenHeightAdapter);
                }),
                yConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return (parent.Height / 2) - (50 * App.screenHeightAdapter);
                }),
                widthConstraint: Constraint.Constant(100 * App.screenHeightAdapter),
                heightConstraint: Constraint.Constant(100 * App.screenHeightAdapter)
            );*/
            Debug.Print("MainPageCS - initSpecificLayout - 0001");
            showActivityIndicator();
            Debug.Print("MainPageCS - initSpecificLayout - 0002");

			eventsHeight = (int)(App.ItemHeight  + 10);

            createCurrentFee();
            

            CreateLogo();
            CreatePhoto();
            //CreateCurrentRankingDetail();
            //createImportantEvents();
            //CreateSponsorsColletionAsync();


            //relativeLayout.Children.Remove(activityIndicator);
            hideActivityIndicator();
        }


        public void CreateButtons()
        {
        }

        public void CreateLogo()
        {
            Xamarin.Forms.Image ericeiraImage = new Xamarin.Forms.Image { Source = "logo_login.png", Aspect = Aspect.AspectFill, Opacity = 1 };
            relativeLayout.Children.Add(ericeiraImage,
                xConstraint: Constraint.Constant(25 * App.screenHeightAdapter),
                yConstraint: Constraint.Constant((30 * App.screenHeightAdapter)),
                widthConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return ((parent.Width) - (50 * App.screenHeightAdapter)); // center of image (which is 40 wide)
                }),
                heightConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return ((parent.Width) - (50 * App.screenHeightAdapter)) / 300 * 110; // center of image (which is 40 wide)
                })
                );
        }


        public async void CreatePhoto()
        {
            Debug.Print("CreatePhoto");

            memberPhotoImage = new RoundImage(240);

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
                //memberPhotoImage_tap.Tapped += memberPhotoImageTappedAsync;
                memberPhotoImage.GestureRecognizers.Add(memberPhotoImage_tap);
            }
            else
            {
                memberPhotoImage.Source = "iconadicionarfoto.png";

                var memberPhotoImage_tap = new TapGestureRecognizer();
                //memberPhotoImage_tap.Tapped += memberPhotoImageTapped_NotAuthorized_Async;
                memberPhotoImage.GestureRecognizers.Add(memberPhotoImage_tap);

            }
            relativeLayout.Children.Add(memberPhotoImage,
                xConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return (parent.Width / 2) - (120 * App.screenHeightAdapter);
                }),
                yConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return (parent.Height / 2) - (120 * App.screenHeightAdapter);
                }),
                widthConstraint: Constraint.Constant(240 * App.screenHeightAdapter),
                heightConstraint: Constraint.Constant(240 * App.screenHeightAdapter)
            );

            var textWelcome = "";

            textWelcome = App.member.nickname;

            //USERNAME LABEL
            usernameLabel = new Label
            {
                Text = textWelcome,
                TextColor = App.bottomColor,
                HorizontalTextAlignment = TextAlignment.Center,
                FontSize = App.bigTitleFontSize
            };
            relativeLayout.Children.Add(usernameLabel,
            xConstraint: Constraint.Constant(10 * App.screenWidthAdapter),
            yConstraint: Constraint.RelativeToParent((parent) =>
            {
                return (parent.Height / 2) + (130 * App.screenHeightAdapter);
            }),
            widthConstraint: Constraint.RelativeToParent((parent) =>
            {
                return (parent.Width) - (20 * App.screenHeightAdapter);
            }),
            heightConstraint: Constraint.Constant(60 * App.screenHeightAdapter));

        }


        public void CreateCurrentRankingDetail()
        {
            createCurrentRankingCircleAsync();
        }

        public async void createCurrentFee()
        {
            if (App.member.socio_tipo != "nao_socio")
            {
                if (App.member.currentFee == null)
                {
                    Debug.Print("Current Fee NULL não devia acontecer!");
                    MemberManager memberManager = new MemberManager();
                    var res = await memberManager.GetCurrentFees(App.member);
                }

                bool hasQuotaPayed = false;

                if (App.member.currentFee != null)
                {
                    if ((App.member.currentFee.estado == "fechado") | (App.member.currentFee.estado == "recebido") | (App.member.currentFee.estado == "confirmado"))
                    {
                        hasQuotaPayed = true;
                        return;
                    }
                }

                if (hasQuotaPayed == false)
                {

                    bool answer = await DisplayAlert("A TUA QUOTA NÃO ESTÁ ATIVA.", "A sua quota para este ano não está ativa. Quer efetuar o pagamento?", "Sim", "Não");
                    Debug.WriteLine("Answer: " + answer);
                    if (answer == true)
                    {
                        await Navigation.PushAsync(new QuotasPageCS());
                    }
                    else
                    {
                        createDocumentsMessage();
                    }
                }
            }
        }


        public async void createDocumentsMessage()
        {
            Debug.Print("createDocumentsMessage");

            MemberManager memberManager = new MemberManager();
            App.member.documents = await memberManager.Get_Member_Documents(App.member.id);

            string docID_status = "";
            DateTime docID_expiration_date = DateTime.Now;
            string docID_parent_status = "";
            DateTime docID_parent_expiration_date = DateTime.Now;
            string atestado_status = "";
            DateTime atestado_expiration_date = DateTime.Now;

            bool hasDocuments = false;

            if (App.member.documents != null) {
                hasDocuments = true;
            } 

            foreach (Document document in App.member.documents)
            {
                Debug.Print("Docuemnt type = " + document.type + "document.status = " + document.status + "document.expiry_date = " + document.expiry_date);

                if (document.type == "documento_identificacao")
                {
                    docID_status = document.status;
                    if (DateTime.Compare(DateTime.Parse(document.expiry_date).Date, docID_expiration_date) > 0)
                    {
                        docID_expiration_date = DateTime.Parse(document.expiry_date).Date;
                    }
                }
                if (document.type == "documento_identificacao_encarregado")
                {
                    docID_parent_status = document.status;
                    if (DateTime.Compare(DateTime.Parse(document.expiry_date).Date, docID_parent_expiration_date) > 0)
                    {
                        docID_parent_expiration_date = DateTime.Parse(document.expiry_date).Date;
                    }
                }
                if (document.type == "atestado_medico")
                {
                    atestado_status = document.status;
                    if (DateTime.Compare(DateTime.Parse(document.expiry_date).Date, atestado_expiration_date) > 0)
                    {
                        atestado_expiration_date = DateTime.Parse(document.expiry_date).Date;
                    }
                }
            }

            if (App.member.socio_tipo == "empresa")
            {
                Debug.Print("Sócio é Empresa docID_status = " + docID_status + " docID_approved_expiration_date = " + docID_expiration_date +" Doc Expirado = " + (DateTime.Compare(docID_expiration_date, DateTime.Now) < 0));
                if ((hasDocuments == false) | (docID_status == "Rejeitado") | (DateTime.Compare(docID_expiration_date, DateTime.Now) < 0))
                {
                    displayAlertDocument();
                }
            }
            else if (App.member.socio_tipo == "individual")
            {
                Debug.Print("Sócio é Individual");
                Debug.Print("App.member.federado_tipo = " + App.member.federado_tipo);
                Debug.Print("App.member.birthdate = " + App.member.birthdate);
                //MAIOR E NÃO ATLETA NEM TREINADOR
                if (((DateTime.Now.Year - DateTime.Parse(App.member.birthdate).Year) >= 18) & ((!App.member.federado_tipo.Contains("atleta")) & (!App.member.federado_tipo.Contains("treinador"))))
                {
                    Debug.Print("Sócio é Individual MAIOR E NÃO ATLETA NEM TREINADOR");
                    if ((hasDocuments == false) | ((docID_status == "Rejeitado") | (DateTime.Compare(docID_expiration_date, DateTime.Now) < 0)))
                    {
                        displayAlertDocument();
                    }
                }
                //MAIOR E ATLETA OU TREINADOR
                else if (((DateTime.Now.Year - DateTime.Parse(App.member.birthdate).Year) >= 18) & ((App.member.federado_tipo.Contains("atleta")) | (App.member.federado_tipo.Contains("treinador"))))
                {
                    Debug.Print("Sócio é Individual MAIOR E ATLETA OU TREINADOR");
                    if ((hasDocuments == false) | (docID_status == "Rejeitado") | (DateTime.Compare(docID_expiration_date, DateTime.Now) < 0))
                    {
                        displayAlertDocument();
                    }
                }
                //MENOR E NÃO ATLETA NEM TREINADOR
                else if (((DateTime.Now.Year - DateTime.Parse(App.member.birthdate).Year) < 18) & ((!App.member.federado_tipo.Contains("atleta")) & (!App.member.federado_tipo.Contains("treinador"))))
                {
                    Debug.Print("Sócio é Individual MENOR E NÃO ATLETA NEM TREINADOR");
                    if ((hasDocuments == false) | (docID_status == "Rejeitado") | (DateTime.Compare(docID_expiration_date, DateTime.Now) < 0))
                    {
                        displayAlertDocument();
                    }
                }
                //MENOR E ATLETA OU TREINADOR
                // TEM DE TER OS DOCUMENTOS TODOS DOCID, DOCID PARENT, ATESTADO
                else if (((DateTime.Now.Year - DateTime.Parse(App.member.birthdate).Year) < 18) & ((App.member.federado_tipo.Contains("atleta")) | (App.member.federado_tipo.Contains("treinador"))))
                {
                    Debug.Print("Sócio é Individual MENOR E ATLETA OU TREINADOR");
                    if ((hasDocuments == false)
                        | (docID_status == "Rejeitado")
                        | (DateTime.Compare(docID_expiration_date, DateTime.Now) < 0)
                        | ((docID_parent_status == "Rejeitado")
                        | (DateTime.Compare(docID_parent_expiration_date, DateTime.Now) < 0))
                        | ((atestado_status == "Rejeitado") | (DateTime.Compare(atestado_expiration_date, DateTime.Now) < 0)))
                    {
                        displayAlertDocument();
                    }
                }
            }

        }

        public async void displayAlertDocument()
        {
            bool answer = await DisplayAlert("DOCUMENTOS EM FALTA.", "Os documentos submitidos não se encontram válidos. Visita a área de documentos e verifica a informação em falta", "Validar", "Mais tarde");
            Debug.WriteLine("Answer: " + answer);
            if (answer == true)
            {
                await Navigation.PushAsync(new DocumentsPageCS());
            }
        }

        public async Task createCurrentRankingCircleAsync()
        {
            Label rankingLabel = new Label
            {
                Text = "",
                TextColor = App.topColor,
                HorizontalTextAlignment = TextAlignment.Center,
                FontSize = (22 * App.screenHeightAdapter)
			};

            var rankingLabel_tap = new TapGestureRecognizer();
            rankingLabel_tap.Tapped += OnRankingPressed;
            rankingLabel.GestureRecognizers.Add(rankingLabel_tap);

            relativeLayout.Children.Add(rankingLabel,
            xConstraint: Constraint.Constant(0 * App.screenHeightAdapter),
            yConstraint: Constraint.Constant(20 * App.screenHeightAdapter),
            widthConstraint: Constraint.RelativeToParent((parent) =>
            {
				return (parent.Width);
            }),
            heightConstraint: Constraint.Constant(60 * App.screenHeightAdapter));

            labelCurrentRanking = new Label { BackgroundColor = Color.Transparent, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center, FontSize = App.rankingBigTitleFontSize, TextColor = App.topColor, LineBreakMode = LineBreakMode.WordWrap };
            labelCurrentRanking.Text = "";
            relativeLayout.Children.Add(labelCurrentRanking,
                xConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return ((parent.Width / 2) - (75 * App.screenHeightAdapter)); // center of image (which is 40 wide)
                }),
                yConstraint: Constraint.Constant((110 * App.screenHeightAdapter)),
                widthConstraint: Constraint.Constant((150 * App.screenHeightAdapter)),
                heightConstraint: Constraint.Constant(80 * App.screenHeightAdapter));

			CompetitionManager competitionManager = new CompetitionManager();

			string bestCategory = "none";
            labelCurrentPoints = new Label { BackgroundColor = Color.Transparent, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center, FontSize = App.bigTitleFontSize, TextColor = App.topColor, LineBreakMode = LineBreakMode.WordWrap };
            List<Competition_Category> competition_Categories = await competitionManager.GetCompetitionCategories_All(App.member.mainsport);
            int age = Constants.GetAge(DateTime.Parse(App.member.birthdate));
            foreach (Competition_Category category in competition_Categories)
            {
                Debug.Print("Age = " + age + " category.idade_minima = " + category.idade_minima + " category.idade_maxima = " + category.idade_maxima);
                if ((category.idade_minima <= age) & (category.idade_maxima >= age) & (bestCategory == "none"))
                {
                    if ((App.member.gender == "male") & (category.gender != "female"))
                    {
                        bestCategory = category.category;
                    }
                    else if (App.member.gender == "female")
                    {
						bestCategory = category.category;
                    }

                    Debug.Print("bestCategory = = " + category.category);
                }
            }

            List<GlobalRanking> allRankings = await competitionManager.GetRankingYearSportAll(DateTime.Now.Year.ToString(), App.member.mainsport, bestCategory);

			rankingLabel.Text = "RANKING "+ DateTime.Now.Year.ToString()+"\n" + bestCategory;

			string currentPosition = "";
            string currentPoints = "";
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

            relativeLayout.Children.Add(labelCurrentPoints,
                xConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return ((parent.Width / 2) - (75 * App.screenHeightAdapter)); // center of image (which is 40 wide)
                }),
                yConstraint: Constraint.Constant((190 * App.screenHeightAdapter)),
                widthConstraint: Constraint.Constant((150 * App.screenHeightAdapter)),
                heightConstraint: Constraint.Constant(80 * App.screenHeightAdapter));

            Xamarin.Forms.Image ericeiraImage = new Xamarin.Forms.Image { Source = "logo_login.png", Aspect = Aspect.AspectFill, Opacity = 0.2 };
            relativeLayout.Children.Add(ericeiraImage,
                xConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return ((parent.Width / 2) - (75 * App.screenHeightAdapter)); // center of image (which is 40 wide)
                }),
                yConstraint: Constraint.Constant((165 * App.screenHeightAdapter)),
                widthConstraint: Constraint.Constant((150 * App.screenHeightAdapter)),
                heightConstraint: Constraint.Constant(50 * App.screenHeightAdapter));


            Ellipse circleRanking = new Ellipse
            {
                Stroke = App.topColor,
                StrokeThickness = 6,
                WidthRequest = 200 * App.screenHeightAdapter,
                HeightRequest = 200 * App.screenHeightAdapter,
                HorizontalOptions = LayoutOptions.Start
            };
            var circleRanking_tap = new TapGestureRecognizer();
            circleRanking_tap.Tapped += OnRankingPressed;
            circleRanking.GestureRecognizers.Add(circleRanking_tap);

            relativeLayout.Children.Add(circleRanking,
                xConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return ((parent.Width / 2) - (100 * App.screenHeightAdapter)); // center of image (which is 40 wide)
                }),
                yConstraint: Constraint.Constant((90 * App.screenHeightAdapter)),
                widthConstraint: Constraint.Constant((200 * App.screenHeightAdapter)),
                heightConstraint: Constraint.Constant(200 * App.screenHeightAdapter));
        }

        public async void createImportantEvents()
		{
			List<Event> importantEventsAll = await GetImportantEvents();

			importantEvents = new List<Event>();

            foreach (Event event_i in importantEventsAll)
			{
				if ((event_i.imagemNome == "") | (event_i.imagemNome is null))
				{
					event_i.imagemSource = "logo_login.png";
				}
				else
				{
					//event_i.imagemSource = "logo_login.png";
					event_i.imagemSource = Constants.images_URL + event_i.id + "_imagem_c";
					
				}
				Debug.Print("event_i.imagemSource = " + event_i.imagemSource);

				if ((event_i.participationconfirmed == "inscrito") | (event_i.participationconfirmed == "confirmado"))
				{
					event_i.participationimage = "iconcheck.png";
				}

				bool already_exists = false;
                foreach (Event event_j in importantEvents)
                {
                    if (event_i.id == event_j.id)
                    {
                        already_exists = true;
                    }
                }
                if (already_exists == false)
                {
                    importantEvents.Add(event_i);
                }
            }

			//AULAS LABEL
			eventsLabel = new Label
			{
				Text = "PRÓXIMOS EVENTOS",
				TextColor = App.bottomColor,
				HorizontalTextAlignment = TextAlignment.Start,
				VerticalTextAlignment = TextAlignment.End,
				FontSize = App.titleFontSize
			};
			relativeLayout.Children.Add(eventsLabel,
			xConstraint: Constraint.Constant(5 * App.screenWidthAdapter),
			yConstraint: Constraint.Constant(300 * App.screenHeightAdapter),
			widthConstraint: Constraint.RelativeToParent((parent) =>
			{
				return (parent.Width); // center of image (which is 40 wide)
			}),
			heightConstraint: Constraint.Constant(30 * App.screenHeightAdapter));

			CreateProximosEventosColletion();
		}

		public void CreateProximosEventosColletion()
		{
			//COLLECTION EVENTOS IMPORTANTES
			importantEventsCollectionView = new CollectionView
			{
				SelectionMode = SelectionMode.Single,
				ItemsSource = importantEvents,
				HorizontalScrollBarVisibility = ScrollBarVisibility.Never,
				ItemsLayout = new GridItemsLayout(1, ItemsLayoutOrientation.Horizontal) { VerticalItemSpacing = 5 * App.screenHeightAdapter, HorizontalItemSpacing = 10 * App.screenWidthAdapter, },
				EmptyView = new ContentView
				{
					Content = new StackLayout
					{
						Children =
							{
								new Label { Text = "Não existem Eventos agendados.", HorizontalTextAlignment = TextAlignment.Start, TextColor = App.topColor, FontSize = App.itemTitleFontSize },
							}
					}
				}
			};

			importantEventsCollectionView.SelectionChanged += OnProximosEventosCollectionViewSelectionChanged;

			importantEventsCollectionView.ItemTemplate = new DataTemplate(() =>
			{
				RelativeLayout itemRelativeLayout = new RelativeLayout
				{
					HeightRequest = App.ItemHeight,
					WidthRequest = App.ItemWidth
				};

				Frame itemFrame = new Frame
				{
					CornerRadius = 5 * (float)App.screenHeightAdapter,
					IsClippedToBounds = true,
					BorderColor = Color.Transparent,
					BackgroundColor = Color.Black,
					Padding = new Thickness(0, 0, 0, 0),
					HeightRequest = (App.ItemHeight),
					VerticalOptions = LayoutOptions.Center,
					HasShadow = false
				};

                Xamarin.Forms.Image eventoImage = new Xamarin.Forms.Image { Aspect = Aspect.AspectFill, Opacity = 0.4, BackgroundColor = Color.Black }; //, HeightRequest = 60, WidthRequest = 60
				eventoImage.SetBinding(Xamarin.Forms.Image.SourceProperty, "imagemSource");

				itemFrame.Content = eventoImage;

				itemRelativeLayout.Children.Add(itemFrame,
					xConstraint: Constraint.Constant(0),
					yConstraint: Constraint.Constant(0),
					widthConstraint: Constraint.RelativeToParent((parent) =>
					{
						return (parent.Width);
					}),
					heightConstraint: Constraint.RelativeToParent((parent) =>
					{
						return (parent.Height);
					}));

				Label nameLabel = new Label { BackgroundColor = Color.Transparent, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center, FontSize = App.itemTitleFontSize, TextColor = Color.White, LineBreakMode = LineBreakMode.WordWrap, MaxLines = 2 };
				nameLabel.SetBinding(Label.TextProperty, "name");

				itemRelativeLayout.Children.Add(nameLabel,
					xConstraint: Constraint.Constant(3 * App.screenWidthAdapter),
					yConstraint: Constraint.Constant(15 * App.screenHeightAdapter),
					widthConstraint: Constraint.RelativeToParent((parent) =>
					{
						return (parent.Width - (10 * App.screenWidthAdapter));
					}),
					heightConstraint: Constraint.Constant(((App.ItemHeight - (15 * App.screenHeightAdapter)) /2)));

				Label categoryLabel = new Label { VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center, FontSize = App.itemTextFontSize, TextColor = Color.White, LineBreakMode = LineBreakMode.WordWrap };
				categoryLabel.SetBinding(Label.TextProperty, "category");

				itemRelativeLayout.Children.Add(categoryLabel,
					xConstraint: Constraint.Constant(0),
					yConstraint: Constraint.Constant(((App.ItemHeight - (15 * App.screenHeightAdapter)) / 2) ),
					widthConstraint: Constraint.RelativeToParent((parent) =>
					{
						return (parent.Width); // center of image (which is 40 wide)
					}),
					heightConstraint: Constraint.Constant(((App.ItemHeight - (15 * App.screenHeightAdapter)) / 4)));

				Label dateLabel = new Label { VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center, FontSize = App.itemTextFontSize, TextColor = Color.White, LineBreakMode = LineBreakMode.WordWrap };
				dateLabel.SetBinding(Label.TextProperty, "detailed_date");

				itemRelativeLayout.Children.Add(dateLabel,
					xConstraint: Constraint.Constant(0),
					yConstraint: Constraint.Constant((App.ItemHeight -15) - ((App.ItemHeight - 15) / 4)),
					widthConstraint: Constraint.RelativeToParent((parent) =>
					{
						return (parent.Width); // center of image (which is 40 wide)
					}),
					heightConstraint: Constraint.Constant(((App.ItemHeight - (15 * App.screenHeightAdapter)) / 4)));


                Xamarin.Forms.Image participationImagem = new Xamarin.Forms.Image { Aspect = Aspect.AspectFill }; //, HeightRequest = 60, WidthRequest = 60
				participationImagem.SetBinding(Xamarin.Forms.Image.SourceProperty, "participationimage");

				itemRelativeLayout.Children.Add(participationImagem,
					xConstraint: Constraint.RelativeToParent((parent) =>
					{
						return (parent.Width - (20 * App.screenHeightAdapter));
					}),
					yConstraint: Constraint.Constant(5),
					widthConstraint: Constraint.Constant(20 * App.screenHeightAdapter),
					heightConstraint: Constraint.Constant(20 * App.screenHeightAdapter));

				return itemRelativeLayout;
			});

			 relativeLayout.Children.Add(importantEventsCollectionView,
				xConstraint: Constraint.Constant(0),
				yConstraint: Constraint.Constant(360 * App.screenHeightAdapter),
				widthConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width); // center of image (which is 40 wide)
				}),
				heightConstraint: Constraint.Constant(eventsHeight));
		}

        public async Task CreateSponsorsColletionAsync()
        {
			SponsorManager sponsorManager = new SponsorManager();
			sponsors = await sponsorManager.GetSponsors();

            
            /* List<Sponsor>();
			sponsors.Add(new Sponsor { name = "Billabong", image = "billabong_logo.png", website = "www.billabong.com" });
            sponsors.Add(new Sponsor { name = "Native Açai", image = "nativeacai_logo.png", website = "www.nativefoods.pt" });
            sponsors.Add(new Sponsor { name = "CM Mafra", image = "mafra_logo.png", website = "www.cm-mafra.pt" });
            sponsors.Add(new Sponsor { name = "Semente", image = "semente.jpg", website = "www.semente.pt" });*/

            foreach (Sponsor sponsor in sponsors)
            {
                sponsor.imageSource= Constants.images_URL + sponsor.id + "_logo";
            }

                //COLLECTION EVENTOS IMPORTANTES
            sponsorsCollectionView = new CollectionView
            {
                SelectionMode = SelectionMode.Single,
                ItemsSource = sponsors,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Never,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
                ItemsLayout = new GridItemsLayout(1, ItemsLayoutOrientation.Horizontal) { VerticalItemSpacing = 5 * App.screenHeightAdapter, HorizontalItemSpacing = 10 * App.screenWidthAdapter, },
                EmptyView = new ContentView
                {
                    Content = new StackLayout
                    {
                        Children =
                            {
                                new Label { Text = "", HorizontalTextAlignment = TextAlignment.Start, TextColor = App.topColor, FontSize = App.itemTitleFontSize },
                            }
                    }
                }
            };

            sponsorsCollectionView.SelectionChanged += OnSponsorsCollectionViewSelectionChanged;

            sponsorsCollectionView.ItemTemplate = new DataTemplate(() =>
            {
                RelativeLayout itemRelativeLayout = new RelativeLayout
                {
                    HeightRequest = 70 * App.screenHeightAdapter,
                    WidthRequest = 120 * App.screenHeightAdapter
                };

                Xamarin.Forms.Image sponsorImage = new Xamarin.Forms.Image { Aspect = Aspect.AspectFit }; //, HeightRequest = 60, WidthRequest = 60
                sponsorImage.SetBinding(Xamarin.Forms.Image.SourceProperty, "imageSource");

                itemRelativeLayout.Children.Add(sponsorImage,
                    xConstraint: Constraint.Constant(0),
                    yConstraint: Constraint.Constant(0),
					heightConstraint: Constraint.RelativeToParent((parent) =>
                    {
                        return (parent.Height); // center of image (which is 40 wide)
                    }),
                    widthConstraint: Constraint.RelativeToParent((parent) =>
                    {
                        return (parent.Width); // center of image (which is 40 wide)
                    }));

            return itemRelativeLayout;
            });

            relativeLayout.Children.Add(sponsorsCollectionView,
               xConstraint: Constraint.Constant(0),
               yConstraint: Constraint.RelativeToParent((parent) =>
               {
                   return (parent.Height - 90 * App.screenHeightAdapter); // center of image (which is 40 wide)
               }),
               widthConstraint: Constraint.RelativeToParent((parent) =>
               {
                   return (parent.Width); // center of image (which is 40 wide)
               }),
               heightConstraint: Constraint.Constant(70));
        }

        public MainPageCS ()
		{


            this.initLayout();


            //this.initSpecificLayout(App.members);

        }

		void OnSendClick(object sender, EventArgs e)
		{
			/*notificationNumber++;
			string title = $"Local Notification #{notificationNumber}";
			string message = $"You have now received {notificationNumber} notifications!";
			notificationManager.SendNotification(title, message);*/
		}

		void OnScheduleClick(object sender, EventArgs e)
		{
			/*notificationNumber++;
			string title = $"Local Notification #{notificationNumber}";
			string message = $"You have now received {notificationNumber} notifications!";
			notificationManager.SendNotification(title, message, DateTime.Now.AddSeconds(10));*/
		}

		void ShowNotification(string title, string message)
		{
			Device.BeginInvokeOnMainThread(() =>
			{
				msg.Text = $"Notification Received:\nTitle: {title}\nMessage: {message}";
			});
		}

		async void OnPerfilButtonClicked(object sender, EventArgs e)
		{
			await Navigation.PushAsync(new ProfilePageCS());
		}

		async Task<ObservableCollection<Class_Schedule>> GetStudentClass_Schedules(string begindate, string enddate)
		{
			Debug.WriteLine("GetStudentClass_Schedules");
			ClassManager classManager = new ClassManager();
			ObservableCollection<Class_Schedule> class_schedules_i = await classManager.GetStudentClass_Schedules_obs(App.member.id, begindate, enddate);
			if (class_schedules_i == null)
			{
                Xamarin.Forms.Application.Current.MainPage = new NavigationPage(new LoginPageCS("Verifique a sua ligação à Internet e tente novamente."))
				{
					BarBackgroundColor = Color.White,
					BarTextColor = Color.Black
				};
				return null;
			}
			return class_schedules_i;
		}

		async Task<List<Event>> GetImportantEvents()
		{
			Debug.WriteLine("GetImportantEvents");
			EventManager eventManager = new EventManager();
			List<Event> events = await eventManager.GetImportantEvents(App.member.id);
			if (events == null)
			{
                Xamarin.Forms.Application.Current.MainPage = new NavigationPage(new LoginPageCS("Verifique a sua ligação à Internet e tente novamente."))
				{
					BarBackgroundColor = Color.White,
					BarTextColor = Color.Black
				};
				return null;
			}
			return events;
		}


        async void OnRankingPressed(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RankingPageCS());
        }

        
        async void OnSponsorsCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Debug.WriteLine("OnCollectionViewProximosEstagiosSelectionChanged " + (sender as CollectionView).SelectedItem.GetType().ToString());

            if ((sender as CollectionView).SelectedItem != null)
            {
                Sponsor sponsor= (sender as CollectionView).SelectedItem as Sponsor;
                if (sponsor.website != "")
                {
                    try
                    {
                        await Browser.OpenAsync(sponsor.website, BrowserLaunchMode.SystemPreferred);
                    }
                    catch (Exception ex)
                    {
                        Debug.Print("Can't open sponsor webpage");
                    }
                }
            }
        }

        async void OnProximosEventosCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Debug.WriteLine("OnCollectionViewProximosEstagiosSelectionChanged " + (sender as CollectionView).SelectedItem.GetType().ToString());

			if ((sender as CollectionView).SelectedItem != null)
			{
				Event event_v = (sender as CollectionView).SelectedItem as Event;
				if (event_v.type == "competicao")
				{
					await Navigation.PushAsync(new DetailCompetitionPageCS(event_v.id));
				}
				else if (event_v.type == "sessaoexame")
				{
					await Navigation.PushAsync(new ExaminationSessionPageCS(event_v.id));
				}
				else
                //if (event_v.type == "estagio")
                {
                    await Navigation.PushAsync(new DetailEventPageCS(event_v));
                }

            }
		}

        /*public void showActivityIndicator()
        {
            //indicator.IsRunning = true;

            relativeLayout.Children.Add(stack,
                xConstraint: Constraint.Constant(0),
                yConstraint: Constraint.Constant(0),
                widthConstraint: Constraint.RelativeToParent((parent) => { return (parent.Width); }),
                heightConstraint: Constraint.RelativeToParent((parent) => { return (parent.Height); }));
            relativeLayout.Children.Add(loading,
                xConstraint: Constraint.RelativeToParent((parent) => { return ((parent.Width / 2) - 50); }),
                yConstraint: Constraint.RelativeToParent((parent) => { return ((parent.Height / 2) - 50); }),
                widthConstraint: Constraint.Constant(100),
                heightConstraint: Constraint.Constant(100));
        }

        public void hideActivityIndicator()
        {
            relativeLayout.Children.Remove(stack);
            relativeLayout.Children.Remove(loading);
            //indicator.IsRunning = false;
        }*/
    }
}
