using Acr.UserDialogs;
using EasySoccer.Mobile.Adm.API.ApiRequest;
using EasySoccer.Mobile.Adm.API.ApiResponses;
using EasySoccer.Mobile.Adm.API.Infra.Exceptions;
using EasySoccer.Mobile.Adm.API.Session;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace EasySoccer.Mobile.Adm.API
{
    public class ApiClient
    {
        private static ApiClient _instance;
        public static ApiClient Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ApiClient();
                return _instance;
            }
        }

        const string ApiUrl = "https://apieasysoccer.azurewebsites.net/api/";
        //const string ApiUrl = "http://localhost:56284/api/";

        private HttpClient CreateClient()
        {
            try
            {
                if (Connectivity.NetworkAccess != NetworkAccess.Internet)
                {
                    var tConfig = new ToastConfig("Você não esta conectado. Verifique sua conexão com internet.")
                    {
                        MessageTextColor = System.Drawing.Color.Yellow,
                        Duration = TimeSpan.FromSeconds(30)
                    };
                    UserDialogs.Instance.Toast(tConfig);
                }
                var httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(ApiUrl);
                httpClient.DefaultRequestHeaders.Clear();
                if (Preferences.ContainsKey("AuthToken"))
                {
                    var token = Preferences.Get("AuthToken", String.Empty);
                    httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                }
                return httpClient;
            }
            catch (Exception)
            {
                UserDialogs.Instance.HideLoading();
                throw;
            }
        }

        private async Task<T> TreatApiReturn<T>(HttpResponseMessage httpResponse)
        {
            try
            {
                if (httpResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var response = await httpResponse.Content.ReadAsStringAsync();
                    var objectResponse = JsonConvert.DeserializeObject<T>(response, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
                    return objectResponse;
                }
                else if (httpResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    UserDialogs.Instance.HideLoading();
                    CurrentUser.Instance.LogOff();
                    throw new ApiUnauthorizedException("Ops! Você não está mais autenticado.");
                }
                else
                {
                    UserDialogs.Instance.HideLoading();
                    var response = await httpResponse.Content.ReadAsStringAsync();
                    if (string.IsNullOrEmpty(response))
                        throw new ApiException("Ops! Ocorreu um erro.");
                    var error = JsonConvert.DeserializeObject<ApiError>(response);
                    var apiException = new ApiException(error.message);
                    throw apiException;
                }
            }
            catch (Exception)
            {
                UserDialogs.Instance.HideLoading();
                throw;
            }
        }

        private async Task<T> Get<T>(string apiMethod)
        {
            try
            {
                T response;
                UserDialogs.Instance.ShowLoading("");
                using (var httpClient = CreateClient())
                {
                    response = await TreatApiReturn<T>(await httpClient.GetAsync(ApiUrl + apiMethod));
                }
                UserDialogs.Instance.HideLoading();
                return response;
            }
            catch (Exception)
            {
                UserDialogs.Instance.HideLoading();
                throw;
            }
        }

        private async Task<TReturn> Post<TReturn>(string apiMethod, object request)
        {
            TReturn response;
            UserDialogs.Instance.ShowLoading("");
            using (var httpClient = CreateClient())
            {
                response = await TreatApiReturn<TReturn>(await httpClient.PostAsJsonAsync(apiMethod, request));
            }
            UserDialogs.Instance.HideLoading();
            return response;
        }


        private string GenerateQueryParameters(object parameters)
        {
            NameValueCollection queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);
            foreach (var item in parameters.GetType().GetProperties())
            {
                queryString.Add(item.Name, item.GetValue(parameters).ToString());
            }
            return queryString.ToString();
        }

        private void SetUserPreferences(string token, DateTime expireDate)
        {
            Preferences.Remove("AuthToken");
            Preferences.Remove("AuthExpiresDate");
            Preferences.Set("AuthToken", token);
            Preferences.Set("AuthExpiresDate", expireDate);
        }

        public Task<List<PlansInfoResponse>> GetPlansInfoAsync()
        {
            return Get<List<PlansInfoResponse>>("financial/getPlansInfo");
        }

        public Task<object> PostCompanyFormInputAsync(CompanyFormInputRequest companyFormInputRequest)
        {
            return Post<object>("Company/companyforminput", companyFormInputRequest);
        }

        public Task<TokenResponse> LoginAsync(string email, string password)
        {
            return Get<TokenResponse>("login/tokencompany?" + GenerateQueryParameters(new { email, password }));
        }

        public Task<List<CompanySchedulesResponse>> GetCompanySchedulesAsync(DateTime selectedDate)
        {
            return Get<List<CompanySchedulesResponse>>("SoccerPitchReservation/getschedules?" + GenerateQueryParameters(new { selectedDate }));
        }

    }
}
