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
using SportNow.Views.Profile;

namespace SportNow.Views
{
	public class EquipamentsPageCS : DefaultPage
	{

		protected override void OnAppearing()
		{
			
		}

		protected override void OnDisappearing()
		{
			//this.CleanScreen();
		}

		private RelativeLayout equipamentosRelativeLayout;

		private StackLayout stackButtons;

		List<Equipment> equipments;
		public List<EquipmentGroup> equipmentsGroupAll;

		private MenuButton fatoTreinoButton, equipamentoTreinoButton;

		private CollectionView collectionViewEquipments;

		public void initLayout()
		{
			Title = "EQUIPAMENTOS";

            var toolbarItem = new ToolbarItem
            {
                //Text = "Logout",
                IconImageSource = "perfil.png",

            };
            toolbarItem.Clicked += OnPerfilButtonClicked;
            ToolbarItems.Add(toolbarItem);

        }


		public void CleanScreen()
		{
			Debug.Print("CleanScreen");
			//valida se os objetos já foram criados antes de os remover
			if (stackButtons != null)
			{
				relativeLayout.Children.Remove(stackButtons);
				//relativeLayout.Children.Remove(equipamentosRelativeLayout);

				stackButtons = null;
				equipamentosRelativeLayout = null;
			}

		}

		public async void initSpecificLayout()
		{
			await GetEquipmentsData();

            CreateEquipmentColletionView();
			collectionViewEquipments.ItemsSource = equipmentsGroupAll;

			/*if (App.EQUIPAMENTOS_activetab == "fato_treino")
            {
				OnFatoTreinoButtonClicked(null, null);
            }
			else if (App.EQUIPAMENTOS_activetab == "equipamento_treino")
			{
				OnEquipamentoTreinoButtonClicked(null, null);
			}*/
		}


		public async Task GetEquipmentsData()
		{
			equipments = await GetEquipments();
			equipmentsGroupAll = new List<EquipmentGroup>();


			foreach (Equipment equipment in equipments)
			{
				equipment.valueFormatted = String.Format("{0:0.00}", equipment.value) + "€";

                //equipmentsFatoTreino.Add(equipment);
                EquipmentGroup equipmentGroup = getSubTypeEquipmentGroup(equipmentsGroupAll, equipment.subtype);

                if (equipmentGroup == null)
                {
                    List<Equipment> equipments = new List<Equipment>();
                    equipments.Add(equipment);
                    equipmentsGroupAll.Add(new EquipmentGroup(equipment.subtype.ToUpper(), equipments));
                }
                else
                {
                    equipmentGroup.Add(equipment);
                }
			}
		}

		public EquipmentGroup getSubTypeEquipmentGroup(List<EquipmentGroup> equipmentGroups, string subtype)
		{
			foreach (EquipmentGroup equipmentGroup in equipmentGroups)
			{
				if (equipmentGroup.Name.ToUpper() == subtype.ToUpper())
				{
					return equipmentGroup;
				}
			}
			return null;
		}

		public void CreateEquipmentColletionView()
		{
			collectionViewEquipments = new CollectionView
			{
				SelectionMode = SelectionMode.Single,
                //ItemsSource = equipmentsGroupAll,
                IsGrouped = true,
				ItemsLayout = new GridItemsLayout(1, ItemsLayoutOrientation.Vertical),
				ItemSizingStrategy = ItemSizingStrategy.MeasureFirstItem,
				EmptyView = new ContentView
				{
					Content = new StackLayout
					{
						Children =
						{
							new Label { Text = "Não existem equipamentos disponíveis de momento.", HorizontalTextAlignment = TextAlignment.Center, TextColor = App.topColor, FontSize = 20 },
						}
					}
				}
			};
			
			collectionViewEquipments.SelectionChanged += OnCollectionViewEquipmentsSelectionChanged;

			collectionViewEquipments.GroupHeaderTemplate = new DataTemplate(() =>
			{
				RelativeLayout itemRelativeLayout = new RelativeLayout
				{
					Margin = new Thickness(3)
				};

				Label nameLabel = new Label { BackgroundColor = Color.Transparent, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Start, FontSize = App.titleFontSize, TextColor = App.topColor, LineBreakMode = LineBreakMode.WordWrap };
				nameLabel.SetBinding(Label.TextProperty, "Name");
				//nameLabel.SetBinding(Label.TextColorProperty, "estadoTextColor");

				itemRelativeLayout.Children.Add(nameLabel,
					xConstraint: Constraint.Constant(0),
					yConstraint: Constraint.Constant(0),
					widthConstraint: Constraint.RelativeToParent((parent) =>
					{
						return (parent.Width / 5 * 4) - 10; // center of image (which is 40 wide)
					}),
					heightConstraint: Constraint.Constant(30 * App.screenHeightAdapter));

				Label valueLabel = new Label { Text = "VALOR", BackgroundColor = Color.Transparent, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center, FontSize = App.titleFontSize, TextColor = App.topColor, LineBreakMode = LineBreakMode.WordWrap };

				itemRelativeLayout.Children.Add(valueLabel,
					xConstraint: Constraint.RelativeToParent((parent) =>
					{
						return (parent.Width / 5 * 4); // center of image (which is 40 wide)
					}),
					yConstraint: Constraint.Constant(0),
					widthConstraint: Constraint.RelativeToParent((parent) =>
					{
						return (parent.Width / 5) - 10; // center of image (which is 40 wide)
					}),
					heightConstraint: Constraint.Constant(30 * App.screenHeightAdapter));

				
				return itemRelativeLayout;
			});
			
			collectionViewEquipments.ItemTemplate = new DataTemplate(() =>
			{
				RelativeLayout itemRelativeLayout = new RelativeLayout
				{
					Margin = new Thickness(3)
				};

				Label nameLabel = new Label { BackgroundColor = Color.Transparent, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Start, FontSize = App.itemTitleFontSize, TextColor = App.normalTextColor, LineBreakMode = LineBreakMode.WordWrap };
				nameLabel.SetBinding(Label.TextProperty, "name");

				Frame nameFrame = new Frame
				{
					BackgroundColor = Color.White,
					BorderColor = Color.FromRgb(43, 53, 129),
					CornerRadius = 10,
					IsClippedToBounds = true,
					Padding = new Thickness(5, 0, 0, 0),
					HasShadow = false
				};
				nameFrame.Content = nameLabel;

				itemRelativeLayout.Children.Add(nameFrame,
					xConstraint: Constraint.Constant(0),
					yConstraint: Constraint.Constant(0),
					widthConstraint: Constraint.RelativeToParent((parent) =>
					{
						return (parent.Width / 5 * 4) - 10; // center of image (which is 40 wide)
					}),
					heightConstraint: Constraint.Constant(30));
				
				Label valueLabel = new Label { BackgroundColor = Color.Transparent, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center, FontSize = App.itemTitleFontSize, TextColor = App.normalTextColor, LineBreakMode = LineBreakMode.WordWrap };
				valueLabel.SetBinding(Label.TextProperty, "valueFormatted");

				Frame valueFrame = new Frame
				{
					BackgroundColor = Color.White,
					BorderColor = Color.FromRgb(43, 53, 129),
					CornerRadius = 10,
					IsClippedToBounds = true,
					Padding = new Thickness(5, 0, 0, 0),
					HasShadow = false
				};
				valueFrame.Content = valueLabel;

				itemRelativeLayout.Children.Add(valueFrame,
					xConstraint: Constraint.RelativeToParent((parent) =>
					{
						return (parent.Width / 5 * 4); // center of image (which is 40 wide)
					}),
					yConstraint: Constraint.Constant(0),
					widthConstraint: Constraint.RelativeToParent((parent) =>
					{
						return (parent.Width / 5) - 10; // center of image (which is 40 wide)
					}),
					heightConstraint: Constraint.Constant(30));

				
				return itemRelativeLayout;
			});

			relativeLayout.Children.Add(collectionViewEquipments,
				xConstraint: Constraint.Constant(0),
				yConstraint: Constraint.Constant(0),
				widthConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width); // center of image (which is 40 wide)
				}),
				heightConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Height) - 80; // 
				})
			);
        }


		public void CreateStackButtons()
		{
			var width = Constants.ScreenWidth;
			var buttonWidth = (width-50) / 2;


			fatoTreinoButton = new MenuButton("FATO TREINO OFICIAL", buttonWidth, 40 * App.screenHeightAdapter);
			fatoTreinoButton.button.Clicked += OnFatoTreinoButtonClicked;

			equipamentoTreinoButton = new MenuButton("EQUIPAMENTO PARA TREINO", buttonWidth, 40 * App.screenHeightAdapter);
			equipamentoTreinoButton.button.Clicked += OnEquipamentoTreinoButtonClicked;

			stackButtons = new StackLayout
			{
				//WidthRequest = 370,
				Margin = new Thickness(0),
				Spacing = 5,
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				HeightRequest = 40,
				Children =
				{
					fatoTreinoButton,
					equipamentoTreinoButton,
				}
			};

			relativeLayout.Children.Add(stackButtons,
			xConstraint: Constraint.Constant(0),
			yConstraint: Constraint.Constant(0),
			widthConstraint: Constraint.RelativeToParent((parent) =>
			{
				return (parent.Width);
			}),
			heightConstraint: Constraint.Constant(40));

		}



		public EquipamentsPageCS(string type)
		{
			App.EQUIPAMENTOS_activetab = type;
			this.initLayout();
			initSpecificLayout();
		}


        public EquipamentsPageCS()
        {
            this.initLayout();
            initSpecificLayout();
			//OnFatoTreinoButtonClicked(null, null);
        }

        async Task<List<Equipment>> GetEquipments()
		{
			EquipmentManager equipmentManager = new EquipmentManager();
			List<Equipment> equipments = await equipmentManager.GetEquipments();
			if (equipments == null)
			{
				Application.Current.MainPage = new NavigationPage(new LoginPageCS("Verifique a sua ligação à Internet e tente novamente."))
				{
					BarBackgroundColor = Color.White,
					BarTextColor = Color.Black
				};
				return null;
			}
			return equipments;
		}

		async void OnFatoTreinoButtonClicked(object sender, EventArgs e)
		{
			/*fatoTreinoButton.activate();
			equipamentoTreinoButton.deactivate();*/

			//collectionViewEquipments.ItemsSource = equipmentsGroupFatoTreino;
			//App.EQUIPAMENTOS_activetab = "fato_treino";

		}

		async void OnEquipamentoTreinoButtonClicked(object sender, EventArgs e)
		{
			fatoTreinoButton.deactivate();
			equipamentoTreinoButton.activate();

			//collectionViewEquipments.ItemsSource = equipmentsGroupEquipamentoTreino;
			App.EQUIPAMENTOS_activetab = "equipamento_treino";
		}

		async void OnCollectionViewEquipmentsSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Debug.WriteLine("OnCollectionViewEquipmentsSelectionChanged");
			if ((sender as CollectionView).SelectedItem != null)
			{

				Equipment equipment = (sender as CollectionView).SelectedItem as Equipment;

				Debug.Print("SELECTED equipment NAME = "+equipment.name);
				await Navigation.PushAsync(new EquipamentsOrderPageCS(equipment));
				

				Debug.WriteLine("OnCollectionViewEquipmentsSelectionChanged equipment = " + equipment.name);

			}
		}

        async void OnPerfilButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ProfilePageCS());
        }

    }
}
