﻿using System;
using System.Collections.Generic;
using Xamarin.Forms;
using SportNow.Model;
using SportNow.Services.Data.JSON;
using System.Threading.Tasks;
using System.Diagnostics;
using SportNow.CustomViews;
using Xamarin.Essentials;
using SportNow.Views.CompleteRegistration;

namespace SportNow.Views
{
    public class CompleteRegistration_ChangePassword_PageCS : DefaultPage
	{

		private Grid grid;
		FormEntryPassword newPasswordEntry;
		FormEntryPassword newPasswordConfirmEntry;
		RoundButton changePasswordButton;
		Label messageLabel;

		public void initLayout()
		{
			Title = "DEFINIR PALAVRA-PASSE";
		}


		public void initSpecificLayout()
		{

			//member = App.members[0];


			grid = new Grid { Padding = 0, HorizontalOptions = LayoutOptions.FillAndExpand };
			grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			//gridGeral.RowDefinitions.Add(new RowDefinition { Height = 1 });
			grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto }); //GridLength.Auto
			grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star }); //GridLength.Auto 

			FormLabel newPasswordLabel = new FormLabel { Text = "PALAVRA-PASSE" };
			newPasswordEntry = new FormEntryPassword("", "");

			FormLabel newPasswordConfirmLabel = new FormLabel { Text = "PALAVRA-PASSE CONFIRMAÇÃO" };
			newPasswordConfirmEntry = new FormEntryPassword("", "");

			changePasswordButton = new RoundButton("Confirmar Palavra-Passe", 100, 40);
			changePasswordButton.button.Clicked += OnChangePasswordButtonClicked;

			messageLabel = new Label
			{
				HorizontalOptions = LayoutOptions.Center,
				TextColor = Color.Red,
				FontSize = App.itemTitleFontSize
			};


			grid.Children.Add(newPasswordLabel, 0, 0);
			grid.Children.Add(newPasswordEntry, 1, 0);

			grid.Children.Add(newPasswordConfirmLabel, 0, 1);
			grid.Children.Add(newPasswordConfirmEntry, 1, 1);

			/*grid.Children.Add(changePasswordButton, 0, 2);
			Grid.SetColumnSpan(changePasswordButton, 2);*/

			grid.Children.Add(messageLabel, 0, 2);
			Grid.SetColumnSpan(messageLabel, 2);

			relativeLayout.Children.Add(grid,
				xConstraint: Constraint.Constant(0),
				yConstraint: Constraint.Constant(60),
				widthConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width - 10); // center of image (which is 40 wide)
				}),
				heightConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Height) - 20; // center of image (which is 40 wide)
				})
			);

            relativeLayout.Children.Add(changePasswordButton,
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

		public CompleteRegistration_ChangePassword_PageCS()
		{
			this.initLayout();
			this.initSpecificLayout();

		}

		async void OnChangePasswordButtonClicked(object sender, EventArgs e)
		{
			changePasswordButton.IsEnabled = false;
			messageLabel.TextColor = Color.Red;
			if (newPasswordEntry.entry.Text.Length < 6) {
				messageLabel.Text = "A Palavra-Passe tem de ter pelo menos 6 caracteres.";
			}
			else if (newPasswordEntry.entry.Text != newPasswordConfirmEntry.entry.Text)
			{
				messageLabel.Text = "A Palavra-Passe não coincide com a Palavra-Passe de confirmação.";
			}
			else 
			{

				Debug.WriteLine("password ok");
				int changePasswordResult = await ChangePassword(App.member.email, newPasswordEntry.entry.Text);
				if (changePasswordResult == 1)
				{
					messageLabel.TextColor = Color.Green;
					messageLabel.Text = "A Palavra-Passe foi definida com sucesso.";

                    MemberManager memberManager = new MemberManager();

                    App.member.estado = "activo";
                    await memberManager.Update_Member_Approved_Status(App.member.id, App.member.name, App.member.email, App.member.estado);
                    List<Member> members = await memberManager.GetMembers(App.member.email);
                    Debug.Print("members.Count = " + members.Count);

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
                        await DisplayAlert("REGISTO CONCLUÍDO", "O Registo foi concluído com sucesso. Clique em OK para voltar ao Login.", "OK"); ;
                        Application.Current.MainPage = new NavigationPage(new LoginPageCS(""))
                        {
                            BarBackgroundColor = Color.White,
                            BarTextColor = Color.Black
                        };

                    }
                }
				else
                {
					messageLabel.Text = "A definiçao de Palavra-Passe falhou. Tente novamente mais tarde ou contacte o Apoio ao Cliente.";
				}
			}
			changePasswordButton.IsEnabled = true;


        }

		async Task<int> ChangePassword(string email, string newpassword)
		{
			Debug.WriteLine("ChangePassword");
			MemberManager memberManager = new MemberManager();

			int result = await memberManager.ChangePassword(email, newpassword);

			return result;
			
		}

	}
}