using EasySoccer.Mobile.Adm.API.Session;
using EasySoccer.Mobile.Adm.Infra.Enums;
using Prism.Navigation;

namespace EasySoccer.Mobile.Adm.Infra
{
    public class Application
    {
        private static Application _application;
        public static Application Instance
        {
            get
            {
                if (_application == null)
                    _application = new Application();
                return _application;
            }
        }

        private string _baseUrl = "https://easysoccer.blob.core.windows.net";

        public void LogOff(INavigationService navigationService)
        {
            CurrentUser.Instance.LogOff();
            navigationService.NavigateAsync("/Login");
        }

        public string GetImage(string fileName, BlobContainerEnum blobContainer)
        {
            if (string.IsNullOrEmpty(fileName))
                fileName = "default.png";
            return string.Format("{0}/{1}/{2}", _baseUrl, this.GetContainerDescription(blobContainer), fileName);
        }

        private string GetContainerDescription(BlobContainerEnum blobContainer)
        {
            string container = "default";
            switch (blobContainer)
            {
                case BlobContainerEnum.Company:
                    container = "company";
                    break;
                case BlobContainerEnum.SoccerPitch:
                    container = "soccerpitch";
                    break;
                default:
                    break;
            }
            return container;
        }
    }
}
