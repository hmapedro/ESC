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
	public class DocumentsPageCS : DefaultPage
	{

		protected async override void OnAppearing()
		{
			if (documentsCollectionView != null)
			{
				relativeLayout.Children.Remove(documentsCollectionView);
				createDocumentsCollection();
            }

		}


		protected async override void OnDisappearing()
		{
		}

		//Image estadoQuotaImage;

		List<Member> members_To_Approve;
		Label titleLabel;
		RegisterButton uploadDocumentButton;
		Image loadedDocument;
		ImageSource source;
		Stream streamSource;

		Picker documentTypePicker;

		Stream stream;


        private CollectionView documentsCollectionView;

		public void initLayout()
		{
			Title = "DOCUMENTOS";
		}


		public async void initSpecificLayout()
		{

            Label documentsLabel = new Label { Text = "DOCUMENTOS SUBMETIDOS", VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center, FontSize = App.titleFontSize, TextColor = App.bottomColor, LineBreakMode = LineBreakMode.NoWrap };

            relativeLayout.Children.Add(documentsLabel,
                xConstraint: Constraint.Constant(20 * App.screenHeightAdapter),
                yConstraint: Constraint.Constant(0 * App.screenHeightAdapter),
                widthConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return ((parent.Width) - (40 * App.screenHeightAdapter));
                }),
                heightConstraint: Constraint.Constant(40 * App.screenHeightAdapter)
            );

            createDocumentsCollection();


            uploadDocumentButton = new RegisterButton("UPLOAD DOCUMENTO", 100, 50);
            uploadDocumentButton.button.Clicked += uploadDocumentButtonClicked;
            relativeLayout.Children.Add(uploadDocumentButton,
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

        public async void createDocumentsCollection()
		{

            

			foreach (Document document in App.member.documents)
			{
				document.imagesourceObject = new UriImageSource
				{
					Uri = new Uri(Constants.images_URL + document.imagesource),
					CachingEnabled = true,
					CacheValidity = new TimeSpan(5, 0, 0, 0)
				};

				if (document.status== "Aprovado")
				{
					document.statusimage = "iconcheck.png";
				}
				else if (document.status == "Under Review")
				{
					document.statusimage = "iconporconfirmar.png";
				}
				else
				{
					document.statusimage = "iconinativo.png";
				}

				if (document.type == "atestado_medico")
				{
					document.typeString = "Atestado Médico";
				}
                else if (document.type == "documento_identificacao")
                {
                    document.typeString = "Documento Identificação";
                }
                else if (document.type == "documento_identificacao_encarregado")
                {
                    document.typeString = "Documento Identificação\nEnc. Educação";
                }

				document.expiry_dateString = "Expira a:" + document.expiry_date;
            }

			//COLLECTION Classes
			documentsCollectionView = new CollectionView
			{
				SelectionMode = SelectionMode.Multiple,
				ItemsSource = App.member.documents,
				HorizontalScrollBarVisibility = ScrollBarVisibility.Never,
				ItemsLayout = new GridItemsLayout(1, ItemsLayoutOrientation.Vertical) { VerticalItemSpacing = 5 * App.screenHeightAdapter, HorizontalItemSpacing = 10 * App.screenWidthAdapter, },
				EmptyView = new ContentView
				{
					Content = new StackLayout
					{
						Children =
							{
								new Label { Text = "Não existem documentos.", HorizontalTextAlignment = TextAlignment.Center, TextColor = App.topColor, FontSize = App.itemTitleFontSize },
							}
					}
				}
			};

			documentsCollectionView.SelectionChanged += OnDocumentsCollectionViewSelectionChanged;

			documentsCollectionView.ItemTemplate = new DataTemplate(() =>
			{
				RelativeLayout itemRelativeLayout = new RelativeLayout
				{
                    HeightRequest = 300 * App.screenHeightAdapter,
                    WidthRequest = App.ItemWidth,
                };

				Frame itemFrame = new Frame
				{
					CornerRadius = 5 * (float)App.screenHeightAdapter,
					IsClippedToBounds = true,
					BorderColor = Color.Transparent,
					BackgroundColor = Color.White,
					Padding = new Thickness(0, 0, 0, 0),
					HeightRequest = 300 * App.screenHeightAdapter,// -(10 * App.screenHeightAdapter),
					VerticalOptions = LayoutOptions.Center,
					HasShadow = false
				};

				Image eventoImage = new Image
				{
					Aspect = Aspect.AspectFill,
					BackgroundColor = Color.White,
					//Opacity = 0.4,
				};
				eventoImage.SetBinding(Image.SourceProperty, "imagesourceObject");

				itemFrame.Content = eventoImage;

                itemRelativeLayout.Children.Add(itemFrame,
                    xConstraint: Constraint.Constant(0),
                    yConstraint: Constraint.Constant(0),
                    widthConstraint: Constraint.RelativeToParent((parent) =>
                    {
                        return (parent.Width);// - (5 * App.screenWidthAdapter));
                    }),
                    heightConstraint: Constraint.RelativeToParent((parent) =>
                    {
                        return (parent.Height - (40 * App.screenHeightAdapter));
                    }));

                Label nameLabel = new Label { BackgroundColor = Color.Transparent, VerticalTextAlignment = TextAlignment.Start, HorizontalTextAlignment = TextAlignment.Center, FontSize = App.itemTitleFontSize, TextColor = App.normalTextColor, LineBreakMode = LineBreakMode.WordWrap };
				nameLabel.SetBinding(Label.TextProperty, "typeString");

				itemRelativeLayout.Children.Add(nameLabel,
					xConstraint: Constraint.Constant(3 * App.screenWidthAdapter),
					yConstraint: Constraint.RelativeToParent((parent) =>
					{
						return (parent.Height - (40 * App.screenHeightAdapter));
					}),
					widthConstraint: Constraint.RelativeToParent((parent) =>
					{
						return (parent.Width - (6 * App.screenWidthAdapter));
					}),
					heightConstraint: Constraint.Constant(20 * App.screenHeightAdapter));

				Label dateLabel = new Label { VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center, FontSize = App.itemTextFontSize, TextColor = App.normalTextColor, LineBreakMode = LineBreakMode.WordWrap };
				dateLabel.SetBinding(Label.TextProperty, "expiry_dateString");

				itemRelativeLayout.Children.Add(dateLabel,
					xConstraint: Constraint.Constant(25 * App.screenWidthAdapter),
					yConstraint: Constraint.RelativeToParent((parent) =>
					{
						return (parent.Height - (20 * App.screenHeightAdapter));
					}),
					widthConstraint: Constraint.RelativeToParent((parent) =>
					{
						return (parent.Width - (50 * App.screenWidthAdapter)); // center of image (which is 40 wide)
					}),
					heightConstraint: Constraint.Constant((20 * App.screenHeightAdapter)));


				Image participationImagem = new Image { Aspect = Aspect.AspectFill }; //, HeightRequest = 60, WidthRequest = 60
				participationImagem.SetBinding(Image.SourceProperty, "statusimage");

				itemRelativeLayout.Children.Add(participationImagem,
					xConstraint: Constraint.RelativeToParent((parent) =>
					{
						return (parent.Width - (20 * App.screenHeightAdapter));
					}),
					yConstraint: Constraint.Constant(3 * App.screenHeightAdapter),
					widthConstraint: Constraint.Constant((20 * App.screenHeightAdapter)),
					heightConstraint: Constraint.Constant((20 * App.screenHeightAdapter)));

				return itemRelativeLayout;
			});
			relativeLayout.Children.Add(documentsCollectionView,
				xConstraint: Constraint.Constant(15 * App.screenHeightAdapter),
				yConstraint: Constraint.Constant((40 * App.screenHeightAdapter)),
				widthConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width - (30 * App.screenHeightAdapter)); // center of image (which is 40 wide)
				}),
                heightConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return (parent.Height - (110 * App.screenHeightAdapter)); // center of image (which is 40 wide)
                }));
        }

		public DocumentsPageCS()
		{
			this.initLayout();
			this.initSpecificLayout();
		}


        async void uploadDocumentButtonClicked(object sender, EventArgs e)
        {
            Debug.WriteLine("DocumentsPageCS.uploadDocumentButtonClicked");

			await Navigation.PushAsync(new DocumentsUploadPageCS());
        }


        async void OnDocumentsCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			showActivityIndicator();
			Debug.WriteLine("DocumentsPageCS.OnDocumentsCollectionViewSelectionChanged");

			if ((sender as CollectionView).SelectedItems.Count != 0)
			{
				Document document = (sender as CollectionView).SelectedItems[0] as Document;
				await Navigation.PushAsync(new DocumentsDetailPageCS(document));

				((CollectionView)sender).SelectedItems.Clear();
                hideActivityIndicator();
            }
		}



    }

}