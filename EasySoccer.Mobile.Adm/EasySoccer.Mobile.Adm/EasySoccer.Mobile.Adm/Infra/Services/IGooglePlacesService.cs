using EasySoccer.Mobile.Adm.Infra.Services.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasySoccer.Mobile.Adm.Infra.Services
{
    public interface IGooglePlacesService
    {
        void DisplayIntent(Action<PlaceDetail> onIntentResult);
    }
}
