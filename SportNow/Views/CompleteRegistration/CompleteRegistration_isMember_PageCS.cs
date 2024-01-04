
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using SportNow.Model;
using SportNow.Services.Data.JSON;
using System.Diagnostics;
using SportNow.CustomViews;
using Xamarin.Essentials;
//Ausing Acr.UserDialogs;

namespace SportNow.Views.CompleteRegistration
{
    public class CompleteRegistration_isMember_PageCS : DefaultPage
	{

        Picker isMemberPicker, memberTypePicker, isFederadoPicker;
        RelativeLayout relativeLayout_isFederado;
        RelativeLayout relativeLayout_FederadoType;
        Label isMemberLabel, memberTypeLabel, isFederadoLabel;
        CheckBox checkboxConfirm;

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
            if (App.member != null)
            {
                if (App.member.id != "")
                {
                    MemberManager memberManager = new MemberManager();
                    var result = await memberManager.UpdateMemberType(App.original_member.id, App.member);
                }
            }           
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

            Image logoImage = new Image
            {
                Source = "logo_login.png",
                HorizontalOptions = LayoutOptions.Center,
            };
            relativeLayout.Children.Add(logoImage,
                xConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return ((parent.Width / 2) - (150 * App.screenHeightAdapter));
                }),
                yConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return ((parent.Height / 2) - (300 * App.screenHeightAdapter));
                }),
                widthConstraint: Constraint.Constant(300 * App.screenHeightAdapter)
            );

            
            Label labelMember = new Label { BackgroundColor = Color.Transparent, VerticalTextAlignment = TextAlignment.Start, HorizontalTextAlignment = TextAlignment.Center, FontSize = App.itemTitleFontSize, TextColor = App.topColor, LineBreakMode = LineBreakMode.WordWrap };
            labelMember.Text = "PODE USAR A APP DO ERICEIRA SURF CLUBE COMO SÓCIO OU APENAS COMO UM UTILIZADOR REGISTADO";
            relativeLayout.Children.Add(labelMember,
                xConstraint: Constraint.Constant(10 * App.screenHeightAdapter),
                yConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return ((parent.Height / 2) - (50 * App.screenHeightAdapter));
                }),
                widthConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return ((parent.Width) - (20 * App.screenHeightAdapter));
                }),
                heightConstraint: Constraint.Constant(40 * App.screenHeightAdapter)
            );

            RoundButton memberButton = new RoundButton("QUERO SER SÓCIO", 100, 50);
            memberButton.button.BackgroundColor = App.topColor;
            
            memberButton.button.Clicked += confirmMemberTypeButtonClicked;

			relativeLayout.Children.Add(memberButton,
				xConstraint: Constraint.Constant(10),
                yConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return ((parent.Height / 2) + (10 * App.screenHeightAdapter));
                }),
                widthConstraint: Constraint.RelativeToParent((parent) =>
				{
					return ((parent.Width/2) - 20 * App.screenHeightAdapter);
				}),
				heightConstraint: Constraint.Constant((80 * App.screenHeightAdapter))
			);

            RoundButton notMemberButton = new RoundButton("NÃO QUERO SER SÓCIO", 100, 50);
            notMemberButton.button.BackgroundColor = App.bottomColor;
            notMemberButton.button.Clicked += confirmNotMemberTypeButtonClicked;

            relativeLayout.Children.Add(notMemberButton,
                xConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return ((parent.Width / 2) + 10 * App.screenHeightAdapter);
                }),
                yConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return ((parent.Height / 2) + (10 * App.screenHeightAdapter));
                }),
                widthConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return ((parent.Width / 2) - 20 * App.screenHeightAdapter);
                }),
                heightConstraint: Constraint.Constant((80 * App.screenHeightAdapter))
            );

            checkboxConfirm = new CheckBox { Color = App.topColor };
            relativeLayout.Children.Add(checkboxConfirm,
                xConstraint: Constraint.Constant(10 * App.screenHeightAdapter),
                yConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return (parent.Height) - (80 * App.screenHeightAdapter); // 
                }),
                widthConstraint: Constraint.Constant(40 * App.screenHeightAdapter),
                heightConstraint: Constraint.Constant(40 * App.screenHeightAdapter)
            );

            Label labelConsentimentos = new Label { BackgroundColor = Color.Transparent, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Start, FontSize = App.itemTextFontSize, TextColor = Color.Black, LineBreakMode = LineBreakMode.WordWrap };
            labelConsentimentos.Text = "PARA CONTINUAR, TEM DE CONSENTIR COM OS TERMOS E CONDIÇÕES DISPONÍVEIS AQUI";
            relativeLayout.Children.Add(labelConsentimentos,
                xConstraint: Constraint.Constant(50 * App.screenHeightAdapter),
                yConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return (parent.Height) - (80 * App.screenHeightAdapter); // 
                }),
                widthConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return ((parent.Width) - (60 * App.screenHeightAdapter));
                }),
                heightConstraint: Constraint.Constant(40 * App.screenHeightAdapter)
            );
            TapGestureRecognizer labelConsentimentos_tap = new TapGestureRecognizer();
            labelConsentimentos_tap.Tapped += async (s, e) =>
            {
                await Navigation.PushModalAsync(new CompleteRegistration_Consent_PageCS());
            };
            labelConsentimentos.GestureRecognizers.Add(labelConsentimentos_tap);


        }



        public CompleteRegistration_isMember_PageCS()
		{
            this.initLayout();
            this.initSpecificLayout();
        }

		async void confirmMemberTypeButtonClicked(object sender, EventArgs e)
        {
            if (checkboxConfirm.IsChecked == false)
			{
                await DisplayAlert("Confirmação necessária", "Para prosseguir é necessário confirmar que aceitas as condições expostas.", "OK");
				return;
            }
            createNewMember();
            App.member.estado = "ativo";
            App.member.socio_tipo = "individual";
            await Navigation.PushAsync(new CompleteRegistration_MemberType_PageCS());
        }

        async void confirmNotMemberTypeButtonClicked(object sender, EventArgs e)
        {
            if (checkboxConfirm.IsChecked == false)
            {
                await DisplayAlert("Confirmação necessária", "Para prosseguir é necessário confirmar que aceitas as condições expostas.", "OK");
                return;
            }
            createNewMember();
            App.member.estado = "ativo";
            App.member.socio_tipo = "nao_socio";
            await Navigation.PushAsync(new CompleteRegistration_Profile_PageCS());
        }

        public void createNewMember()
        {
            App.feeCreated = false;
            App.idDocumentCreated = false;
            App.idParentDocumentCreated = false;
            App.medicalDocumentCreated = false;
            App.member = new Member();
            App.member.id = "";
            App.member.name= "";
            App.member.cc_number = "";
            App.member.gender = "female";
            App.member.federado_tipo= "";
            App.member.country = "PORTUGAL";
            //App.member.cc_number = "-1";
            //App.member.name = "temp temp";
        }
    }
}