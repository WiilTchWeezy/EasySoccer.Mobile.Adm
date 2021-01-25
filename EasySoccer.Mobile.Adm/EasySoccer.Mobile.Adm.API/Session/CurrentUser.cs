using EasySoccer.Mobile.Adm.API.Events;
using Newtonsoft.Json;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace EasySoccer.Mobile.Adm.API.Session
{
    public class CurrentUser
    {
        private static CurrentUser _currentUser;
        public static CurrentUser Instance
        {
            get
            {
                if (_currentUser == null)
                    _currentUser = new CurrentUser();
                return _currentUser;
            }
        }

        private IEventAggregator _eventAggregator;

        public DateTime? AuthExpiresDate
        {
            get
            {
                if (Preferences.ContainsKey("AuthExpiresDate"))
                    return Preferences.Get("AuthExpiresDate", DateTime.MinValue);
                else
                    return null;
            }
        }

        public string AuthToken
        {
            get
            {
                if (Preferences.ContainsKey("AuthToken"))
                    return Preferences.Get("AuthToken", String.Empty);
                else
                    return null;
            }
        }

        public bool IsLoggedIn
        {
            get
            {
                return string.IsNullOrEmpty(AuthToken) == false;
            }
        }

        public void SetEventAggregator(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public long? UserId
        {
            get
            {
                var claimValue = GetClaimByType("UserId");
                if (string.IsNullOrEmpty(claimValue) == false)
                {
                    long userId;
                    if (long.TryParse(claimValue, out userId))
                        return userId;
                    return null;
                }
                return null;
            }
        }

        public async void LogOff()
        {

            var fcmToken = Preferences.Get("FcmToken", string.Empty);
            if (Preferences.ContainsKey("FcmToken") && string.IsNullOrEmpty(fcmToken) == false)
            {
                try
                {
                    if (this.UserId.HasValue)
                    {
                        var request = new ApiRequest.InserTokenRequest { Token = fcmToken, CompanyUserId = this.UserId.Value };
                        var json = JsonConvert.SerializeObject(request);
                        if (CurrentUser.Instance.IsLoggedIn)
                            await ApiClient.Instance.LogOffTokenAsync(request);
                    }
                }
                catch (Exception e)
                {
                    Preferences.Remove("AuthToken");
                    Preferences.Remove("AuthExpiresDate");
                    _eventAggregator?.GetEvent<UserLoggedInEvent>().Publish(false);
                    return;
                }
            }
            Preferences.Remove("AuthToken");
            Preferences.Remove("AuthExpiresDate");
            _eventAggregator?.GetEvent<UserLoggedInEvent>().Publish(false);
        }

        public void Login(string token, DateTime expireDate)
        {
            Preferences.Remove("AuthToken");
            Preferences.Remove("AuthExpiresDate");
            Preferences.Set("AuthToken", token);
            Preferences.Set("AuthExpiresDate", expireDate);
        }

        public List<Claim> DecryptToken()
        {
            if (string.IsNullOrEmpty(AuthToken) == false)
            {
                var jwtToken = new JwtSecurityToken(AuthToken);
                if (jwtToken != null && jwtToken.Claims != null && jwtToken.Claims.ToList().Count > 0)
                {
                    return jwtToken.Claims.ToList();
                }
            }
            return new List<Claim>();
        }

        public string GetClaimByType(string type)
        {
            var claims = DecryptToken();
            if (claims.Count > 0)
            {
                return claims.Where(x => x.Type == type).Select(x => x.Value).FirstOrDefault();
            }
            return String.Empty;
        }
    }
}
