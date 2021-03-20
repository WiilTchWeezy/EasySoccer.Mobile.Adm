using EasySoccer.Mobile.Adm.ViewModels;
using Xamarin.Forms;

namespace EasySoccer.Mobile.Adm.Views
{
    public partial class ReservationInfo : ContentPage
    {
        public ReservationInfo()
        {
            InitializeComponent();
        }

        protected override bool OnBackButtonPressed()
        {
            var vm = (this.BindingContext as ReservationInfoViewModel);
            if(vm != null)
            {
                vm.BackButtonPress();
            }
            return false;
        }

    }
}
