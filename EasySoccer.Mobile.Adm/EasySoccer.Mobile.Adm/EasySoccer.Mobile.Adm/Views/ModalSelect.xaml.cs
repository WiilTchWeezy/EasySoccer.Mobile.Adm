using EasySoccer.Mobile.Adm.ViewModels;
using Xamarin.Forms;

namespace EasySoccer.Mobile.Adm.Views
{
    public partial class ModalSelect : ContentPage
    {
        public ModalSelect()
        {
            InitializeComponent();
            srchText.TextChanged += SrchText_TextChanged;
        }

        private void SrchText_TextChanged(object sender, TextChangedEventArgs e)
        {
            var vm = this.BindingContext as ModalSelectViewModel;
            vm.Search(e.NewTextValue);
        }
    }
}
