using System;
using System.Collections.Generic;
using Xamarin.Forms;
using SportNow.Model;
using SportNow.Services.Data.JSON;
using System.Diagnostics;
using SportNow.CustomViews;
//Ausing Acr.UserDialogs;

namespace SportNow.Views.CompleteRegistration
{
    public class Profile_MemberType_PageCS : DefaultPage
	{

        Picker isMemberPicker, memberTypePicker, isFederadoPicker;
        RelativeLayout relativeLayout_isFederado;
        RelativeLayout relativeLayout_FederadoType;
        Label isMemberLabel, memberTypeLabel, isFederadoLabel;

        protected async override void OnAppearing()
		{
			initLayout();
			initSpecificLayout();
		}


		protected async override void OnDisappearing()
		{
			if (relativeLayout != null)
			{
				relativeLayout = null;
				this.Content = null;
			}
            //showActivityIndicator();
            if (App.member.id != "")
            {
                MemberManager memberManager = new MemberManager();
                var result = await memberManager.UpdateMemberType(App.original_member.id, App.member);
            }
            //hideActivityIndicator();

        }

        public void initLayout()
		{
			Title = "TIPO DE SÓCIO";
		}


		public async void initSpecificLayout()
		{
			if (relativeLayout == null)
			{
				initBaseLayout();
            }

            Frame backgroundFrame= new Frame
			{
				CornerRadius = 10,
				IsClippedToBounds = true,
				BackgroundColor = Color.FromRgb(240,240,240),
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
					return ((parent.Height) - (90 * App.screenHeightAdapter));
				})
			);


            //createIsMemberPicker();

            createMemberTypePicker();

            createIsFederadoPicker();

            showCorrectItems();

            RoundButton confirmButton = new RoundButton("CONFIRMAR", 100, 50);
			confirmButton.button.Clicked += confirmMemberTypeButtonClicked;

			relativeLayout.Children.Add(confirmButton,
				xConstraint: Constraint.Constant(10),
				yConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Height) - (60 * App.screenHeightAdapter); // 
				}),
				widthConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width - 20 * App.screenHeightAdapter);
				}),
				heightConstraint: Constraint.Constant((50 * App.screenHeightAdapter))
			);


		}

        public async void createMemberTypePicker()
        {

            memberTypeLabel = new Label { BackgroundColor = Color.Transparent, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center, FontSize = App.titleFontSize, TextColor = App.bottomColor, LineBreakMode = LineBreakMode.WordWrap };
            memberTypeLabel.Text = "TIPO DE SÓCIO";

            relativeLayout.Children.Add(memberTypeLabel,
                xConstraint: Constraint.Constant(25 * App.screenHeightAdapter),
                yConstraint: Constraint.Constant(25 * App.screenHeightAdapter),
                widthConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return ((parent.Width) - (50 * App.screenHeightAdapter));
                }),
                heightConstraint: Constraint.Constant(40 * App.screenHeightAdapter)
            );

            memberTypePicker = new Picker
            {
                Title = "",
                TitleColor = Color.Black,
                BackgroundColor = Color.Transparent,
                TextColor = App.topColor,
                HorizontalTextAlignment = TextAlignment.Center,
                FontSize = App.formValueFontSize

            };
			List<string> typeMemberList = new List<string>();
            foreach (var typeMember in Constants.memberTypeList_All)
            {
                typeMemberList.Add(typeMember.Value);
            }
            memberTypePicker.ItemsSource = typeMemberList;

            if (App.member.socio_tipo == "individual")
            {
                memberTypePicker.SelectedIndex = 0;
            }
            else
            {
                memberTypePicker.SelectedIndex = 1;
            }


            memberTypePicker.SelectedIndexChanged += async (object sender, EventArgs e) =>
            {
                Debug.Print("memberTypePicker selectedItem = " + memberTypePicker.SelectedItem.ToString());

                if (memberTypePicker.SelectedItem.ToString() != "Individual")
                {
                    App.member.federado_tipo = "";
                }
                showCorrectItems();
                selectCorrectItemFederadoPicker();

            };

            relativeLayout.Children.Add(memberTypePicker,
                xConstraint: Constraint.Constant(25 * App.screenHeightAdapter),
                yConstraint: Constraint.Constant(65 * App.screenHeightAdapter),
                widthConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return ((parent.Width) - (50 * App.screenHeightAdapter));
                }),
                heightConstraint: Constraint.Constant(40 * App.screenHeightAdapter));
        }

        public async void createIsFederadoPicker()
        {

            isFederadoLabel = new Label { BackgroundColor = Color.Transparent, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center, FontSize = App.titleFontSize, TextColor = App.bottomColor, LineBreakMode = LineBreakMode.WordWrap };
            isFederadoLabel.Text = "PRETENDES SER FEDERADO?";

            isFederadoPicker = new Picker
            {
                Title = "",
                TitleColor = Color.Black,
                BackgroundColor = Color.Transparent,
                TextColor = App.topColor,
                HorizontalTextAlignment = TextAlignment.Center,
                FontSize = App.itemTextFontSize

            };

            int selectedIndex = 0;
            List<string> federadoTypeList = new List<string>();
            foreach (var federadoType in Constants.federadoTypeList)
            {
                federadoTypeList.Add(federadoType.Value);
                if (App.member.federado_tipo.Contains(federadoType.Key))
                {
                    isFederadoPicker.SelectedIndex = selectedIndex;
                }
                selectedIndex++;
            }
            isFederadoPicker.ItemsSource = federadoTypeList;
            selectCorrectItemFederadoPicker();

            isFederadoPicker.SelectedIndexChanged += async (object sender, EventArgs e) =>
            {
                Debug.Print("isFederadoPicker selectedItem = " + isFederadoPicker.SelectedItem.ToString());
                App.member.federado_tipo = Constants.federadoTypeList.KeyByValue(isFederadoPicker.SelectedItem.ToString());
                //App.member.federado_tipo = "";
                //loadFederadoTipo_checkboxes();
                showCorrectItems();
            };
        }

        public void showCorrectItems()
        {
            if (isFederadoLabel != null)
            {
                relativeLayout.Children.Remove(isFederadoLabel);
            }
            if (isFederadoPicker != null)
            {
                relativeLayout.Children.Remove(isFederadoPicker);
            }

            Debug.Print("App.member.socio_tipo = " + App.member.socio_tipo);
            Debug.Print("memberTypePicker.SelectedIndex = " + memberTypePicker.SelectedIndex);

            if (memberTypePicker.SelectedIndex == 0)
            {
                relativeLayout.Children.Add(isFederadoLabel,
                    xConstraint: Constraint.Constant(25 * App.screenHeightAdapter),
                    yConstraint: Constraint.Constant(120 * App.screenHeightAdapter),
                    widthConstraint: Constraint.RelativeToParent((parent) =>
                    {
                        return ((parent.Width) - (50 * App.screenHeightAdapter));
                    }),
                    heightConstraint: Constraint.Constant(40 * App.screenHeightAdapter)
                );

                relativeLayout.Children.Add(isFederadoPicker,
                    xConstraint: Constraint.Constant(25 * App.screenHeightAdapter),
                    yConstraint: Constraint.Constant(160 * App.screenHeightAdapter),
                    widthConstraint: Constraint.RelativeToParent((parent) =>
                    {
                        return ((parent.Width) - (50 * App.screenHeightAdapter));
                    }),
                    heightConstraint: Constraint.Constant(40 * App.screenHeightAdapter));
            }

        }

        public void selectCorrectItemFederadoPicker()
        {
            int selectedIndex = 0;
            isFederadoPicker.SelectedIndex = 1;
            foreach (var federadoType in Constants.federadoTypeList)
            {
                if (App.member.federado_tipo.Contains(federadoType.Key))
                {
                    isFederadoPicker.SelectedIndex = selectedIndex;
                    break;
                }
                selectedIndex++;
            }
        }

        public void loadFederadoTipo_checkboxes()
        {
            foreach (var item in relativeLayout_FederadoType.Children)
            {
                if (item.GetType().ToString() == "Xamarin.Forms.CheckBox")
                {
                    //Debug.Print("É check box " + item.AutomationId);
                    CheckBox checkBox = (CheckBox)item;
                    if (App.member.federado_tipo.Contains(checkBox.AutomationId))
                    {
                        checkBox.IsChecked = true;
                    }
                    else
                    {
                        checkBox.IsChecked = false;
                    }
                }
            }
        }

        public Profile_MemberType_PageCS()
		{
            this.initLayout();
            this.initSpecificLayout();
        }

		async void confirmMemberTypeButtonClicked(object sender, EventArgs e)
        {
            //SAVE SPORTS!!!!!
            App.member.socio_tipo = Constants.memberTypeList_All.KeyByValue(memberTypePicker.SelectedItem.ToString());
            App.member.federado_tipo = Constants.federadoTypeList.KeyByValue(isFederadoPicker.SelectedItem.ToString());

            /*foreach (var item in relativeLayout_FederadoType.Children)
            {
                if (item.GetType().ToString() == "Xamarin.Forms.CheckBox")
                {
                    //Debug.Print("É check box " + item.AutomationId);
                    CheckBox checkBox = (CheckBox)item;
                    if (checkBox.IsChecked == true)
                    {
                        //Debug.Print("É check box e está checked " + item.AutomationId);
                        if ((App.member.federado_tipo == null) | (App.member.federado_tipo == ""))
                        {
                            App.member.federado_tipo = "^" + item.AutomationId + "^";
                        }
                        else
                        {
                            App.member.federado_tipo = App.member.federado_tipo + ",^" + item.AutomationId + "^";
                        }
                    }
                }
            }*/
            Debug.Print("member.socio_tipo = " + App.member.socio_tipo);
            Debug.Print("member.federado_tipo = " + App.member.federado_tipo);

            await Navigation.PopAsync();


        }
	}

}