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
    public class CompleteRegistration_Consent_PageCS : DefaultPage
	{

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

		//Image estadoQuotaImage;

		private CollectionView collectionViewMembers;
		List<Member> members_To_Approve;
		Label titleLabel;
		CheckBox checkboxConfirm;

        Button confirmConsentButton;

		CheckBox checkBoxAssembleiaGeral, checkBoxRegulamentoInterno, checkBoxTratamentoDados, checkBoxRegistoImagens, checkBoxFotografiaSocio, checkBoxWhatsApp;

		public void initLayout()
		{
			Title = "CONSENTIMENTOS";

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
				yConstraint: Constraint.Constant(40 * App.screenHeightAdapter),
				widthConstraint: Constraint.RelativeToParent((parent) =>
				{
					return ((parent.Width) - (20 * App.screenHeightAdapter));
				}),
				heightConstraint: Constraint.RelativeToParent((parent) =>
				{
					return ((parent.Height) - (120 * App.screenHeightAdapter));
				})
			);

			int y_index = (int)(60 * App.screenHeightAdapter);

			Label labelRegulamentoInterno = new Label { BackgroundColor = Color.Transparent, VerticalTextAlignment = TextAlignment.Start, HorizontalTextAlignment = TextAlignment.Start, FontSize = App.consentFontSize, TextColor = Color.Black, LineBreakMode = LineBreakMode.WordWrap };
			labelRegulamentoInterno.Text = "Declaro que as informações e dados pessoais transmitidos são verdadeiros e atuais.\n\nAutorizo o tratamento dos meus dados pessoais e/ou do meu educando, por parte do Ericeira Surf Clube, para efeitos de processos associados a faturação, a atividades desportivas, em particular para filiação/refiliação em federações desportivas, incluindo inscrições em eventos desportivos nacionais ou internacionais, a contratação de seguros desportivos, e, bem assim, ao envio de mensagens sobre a atividade desportiva e corrente do Ericeira Surf Clube (SMS, MMS, APP e correio eletrónico).\n\nAutorizo igualmente o registo, gravação, captação de imagens e testemunhos dos treinos, competições e outros eventos de cariz desportivo, formativo e lúdico para utilização com finalidades pedagógicas e/ou promocionais. Neste âmbito, o Ericeira Surf Clube pode proceder à divulgação, total ou parcial, dessas atividades, imagens e testemunhos que lhe estão associadas através das suas páginas eletrónicas, portais ou redes sociais, incluindo plataformas e canais digitais pertencentes a órgãos de comunicação social. \n\nFace ao exposto, cedo, a título gratuito os direitos de imagem associados à minha participação e/ou do meu educando nas várias iniciativas desportivas, pedagógicas, formativas e lúdicas promovidas pelo Ericeira Surf Clube. \n\nAutorizo o Ericeira Surf Clube a recolher a foto tipo passe do sócio para uso na ficha de sócio e para a emissão de credenciais de eventos. \n\nLi e concordo com o Regulamento Interno do Ericeira Surf Clube disponível em www.ericeirasurfclube.com.\n";
			relativeLayout.Children.Add(labelRegulamentoInterno,
				xConstraint: Constraint.Constant(30 * App.screenHeightAdapter),
				yConstraint: Constraint.Constant(y_index * App.screenHeightAdapter),
				widthConstraint: Constraint.RelativeToParent((parent) =>
				{
					return ((parent.Width) - (60 * App.screenHeightAdapter));
				}),
                heightConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return ((parent.Height) - (160 * App.screenHeightAdapter));
                })
            );


            /*Label labelConfirm = new Label { BackgroundColor = Color.Transparent, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Start, FontSize = App.consentFontSize, TextColor = Color.Black, LineBreakMode = LineBreakMode.WordWrap };
            labelConfirm.Text = "CONFIRMO QUE ACEITO OS CONSENTIMENTOS APRESENTADOS.";
            relativeLayout.Children.Add(labelConfirm,
                xConstraint: Constraint.Constant(60 * App.screenHeightAdapter),
                yConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return (parent.Height) - (150 * App.screenHeightAdapter);
                }),
                widthConstraint: Constraint.RelativeToParent((parent) =>
                {
                    return ((parent.Width) - (60 * App.screenHeightAdapter));
                }),
                heightConstraint: Constraint.Constant(30 * App.screenHeightAdapter)
            );

            checkboxConfirm = new CheckBox { Color = App.topColor};
            relativeLayout.Children.Add(checkboxConfirm,
				xConstraint: Constraint.Constant(30 * App.screenHeightAdapter),
				yConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Height) - (150 * App.screenHeightAdapter);
				}),
				widthConstraint: Constraint.Constant(30 * App.screenHeightAdapter),
                heightConstraint: Constraint.Constant(30 * App.screenHeightAdapter)
            );*/

            RoundButton confirmButton = new RoundButton("FECHAR", 100, 50);
			confirmButton.button.Clicked += confirmConsentButtonClicked;

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

		public CompleteRegistration_Consent_PageCS()
		{
            this.initLayout();
            this.initSpecificLayout();
        }

		async void confirmConsentButtonClicked(object sender, EventArgs e)
		{

/*            if (checkboxConfirm.IsChecked == false)
			{
                await DisplayAlert("Confirmação necessária", "Para prosseguir é necessário confirmar que aceitas as condições expostas.", "OK");
				return;
            }*/
			await Navigation.PopModalAsync();

        }
	}

}