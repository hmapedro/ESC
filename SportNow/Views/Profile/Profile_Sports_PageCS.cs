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
    public class Profile_Sports_PageCS : DefaultPage
	{

        Picker mainSportPicker;

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

		}

		public void initLayout()
		{
			Title = "MODALIDADES";

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


            createMainSportsTypePicker();

            createOtherSportsTypeOptions();

            RoundButton confirmButton = new RoundButton("CONFIRMAR", 100, 50);
			confirmButton.button.Clicked += confirmSportsButtonClicked;

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


        public async void createMainSportsTypePicker()
        {

            Label mainSportsLabel = new Label { BackgroundColor = Color.Transparent, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center, FontSize = App.titleFontSize, TextColor = App.bottomColor, LineBreakMode = LineBreakMode.WordWrap };
            mainSportsLabel.Text = "MODALIDADE PRINCIPAL";

            relativeLayout.Children.Add(mainSportsLabel,
                xConstraint: Constraint.Constant(25 * App.screenHeightAdapter),
                yConstraint: Constraint.Constant(25 * App.screenHeightAdapter),
                widthConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return ((parent.Width) - (50 * App.screenHeightAdapter));
                }),
                heightConstraint: Constraint.Constant(40 * App.screenHeightAdapter)
            );

            //List<string> sportsList = new List<string> { "Escolha o tipo de documento", "Termo de Responsabilidade", "Atestado Médico" };
            int selectedIndex = 0;
            int selectedIndex_temp = 0;

            mainSportPicker = new Picker
            {
                Title = "",
                TitleColor = Color.Black,
                BackgroundColor = Color.Transparent,
                TextColor = App.topColor,
                HorizontalTextAlignment = TextAlignment.Center,
                FontSize = App.formValueFontSize

            };

            Debug.Print("AQUI App.member.mainsport = " + App.member.mainsport);

            List<string> sportsList = new List<string>();
            foreach (var sport in Constants.sportsList)
            {

                Debug.Print("AQUI sport.Value = " + sport.Key);
				sportsList.Add(sport.Value);
                if (sport.Key == App.member.mainsport)
                {
                    selectedIndex = selectedIndex_temp;
                }
                selectedIndex_temp++;
            }
            mainSportPicker.ItemsSource = sportsList;
            mainSportPicker.SelectedIndex = selectedIndex;

            mainSportPicker.SelectedIndexChanged += async (object sender, EventArgs e) =>
            {
                Debug.Print("sportsPicker selectedItem = " + mainSportPicker.SelectedItem.ToString());
            };

            relativeLayout.Children.Add(mainSportPicker,
                xConstraint: Constraint.Constant(25 * App.screenHeightAdapter),
                yConstraint: Constraint.Constant(65 * App.screenHeightAdapter),
                widthConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return ((parent.Width) - (50 * App.screenHeightAdapter));
                }),
                heightConstraint: Constraint.Constant(40));
        }

        public async void createOtherSportsTypeOptions()
        {

            Label otherSportsLabel = new Label { BackgroundColor = Color.Transparent, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center, FontSize = App.titleFontSize, TextColor = App.bottomColor, LineBreakMode = LineBreakMode.WordWrap };
            otherSportsLabel.Text = "OUTRAS MODALIDADES";

            relativeLayout.Children.Add(otherSportsLabel,
                xConstraint: Constraint.Constant(25 * App.screenHeightAdapter),
                yConstraint: Constraint.Constant(110 * App.screenHeightAdapter),
                widthConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return ((parent.Width) - (50 * App.screenHeightAdapter));
                }),
                heightConstraint: Constraint.Constant(40 * App.screenHeightAdapter)
            );

            Debug.Print("Member Sports = " + App.member.othersports);

            double y_index = (double) 150;
            foreach (var sport in Constants.sportsList)
            {
                CheckBox checkbox = new CheckBox { Color = App.topColor };
                checkbox.AutomationId = sport.Key;

                relativeLayout.Children.Add(checkbox,
                    xConstraint: Constraint.Constant(20 * App.screenHeightAdapter),
                    yConstraint: Constraint.Constant(y_index * App.screenHeightAdapter),
                    widthConstraint: Constraint.Constant(30 * App.screenHeightAdapter),
                    heightConstraint: Constraint.Constant(30 * App.screenHeightAdapter)
                );

                Label checkboxLabel = new Label { BackgroundColor = Color.Transparent, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Start, FontSize = App.consentFontSize, TextColor = App.topColor, LineBreakMode = LineBreakMode.WordWrap };
                checkboxLabel.Text = sport.Value;

                Debug.Print("sport.Value = " + sport.Value);

                if (App.member.othersports.Contains(sport.Key) == true)
                {
                    checkbox.IsChecked = true;
                }

                relativeLayout.Children.Add(checkboxLabel,
                    xConstraint: Constraint.Constant(55 * App.screenHeightAdapter),
                    yConstraint: Constraint.Constant(y_index * App.screenHeightAdapter),
                    widthConstraint: Constraint.RelativeToParent((parent) =>
                    {
                        return ((parent.Width) - (50 * App.screenHeightAdapter));
                    }),
                    heightConstraint: Constraint.Constant(30 * App.screenHeightAdapter)
                );

                y_index = y_index + 40 * App.screenHeightAdapter;
            }

        }

        public Profile_Sports_PageCS()
		{
            this.initLayout();
            this.initSpecificLayout();
        }

		async void confirmSportsButtonClicked(object sender, EventArgs e)
        {
            //SAVE SPORTS!!!!!
            App.member.mainsport = Constants.sportsList.KeyByValue(mainSportPicker.SelectedItem.ToString());
            App.member.othersports = "";

            foreach (var item in relativeLayout.Children)
            {
                if (item.GetType().ToString() == "Xamarin.Forms.CheckBox")
                {
                    CheckBox checkBox = (CheckBox)item;
                    if (checkBox.IsChecked == true)
                    {
                        if ((App.member.othersports == null) | (App.member.othersports == ""))
                        {
                            App.member.othersports = "^" + item.AutomationId + "^";
                        }
                        else
                        {
                            App.member.othersports = App.member.othersports +",^" + item.AutomationId + "^";
                        }
                    }
                }
            }
            Debug.Print("member.mainsport = " + App.member.mainsport );
            Debug.Print("member.othersports = " + App.member.othersports);

            showActivityIndicator();
            if (App.member.id != "")
            {
                MemberManager memberManager = new MemberManager();
                var result = await memberManager.UpdateMemberSports(App.original_member.id, App.member);
            }
            hideActivityIndicator();
            await Navigation.PopAsync();

            //await Navigation.PushAsync(new CompleteRegistration_IdCard_PageCS(""));
            //await Navigation.PushAsync(new CompleteRegistration_Profile_PageCS());
            /*
                        if (App.member.member_type == "praticante")
                        {
                            await Navigation.PushAsync(new CompleteRegistration_Documents_PageCS());
                        }
                        else
                        {

                        }*/


        }
	}

}