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

        private async Task<TReturn> Patch<TReturn>(string apiMethod, object request)
        {
            TReturn response;
            UserDialogs.Instance.ShowLoading("");
            using (var httpClient = CreateClient())
            {
                var method = new HttpMethod("PATCH");

                var jsonRequest = JsonConvert.SerializeObject(request);
                var httpRequest = new HttpRequestMessage(method, $"{ApiUrl}{apiMethod}")
                {
                    Content = new StringContent(jsonRequest, Encoding.UTF8, "application/json")
                };
                response = await TreatApiReturn<TReturn>(await httpClient.SendAsync(httpRequest));
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
            return Get<List<CompanySchedulesResponse>>("SoccerPitchReservation/getcompanyschedules?" + GenerateQueryParameters(new { selectedDate }));
        }

        public Task<CompanyInfoResponse> GetCompanyInfoAsync()
        {
            return Get<CompanyInfoResponse>("Company/getcompanyinfo");
        }

        public Task<object> PostCompanyImageAsync(CompanyImageRequest companyImageRequest)
        {
            return Post<object>("Company/saveImage", companyImageRequest);
        }

        public Task<object> PatchCompanyAsync(PatchCompanyInfoRequest patchCompanyInfoRequest)
        {
            return Patch<object>("company/patchcompanyinfo", patchCompanyInfoRequest);
        }

        public Task<object> ActiveAsync(bool active)
        {
            return Post<object>("company/active", new { Active = active });
        }

        public Task<List<StatesResponse>> GetStatesAsync()
        {
            return Get<List<StatesResponse>>("Company/getstates");
        }

        public Task<List<CityResponse>> GetCitiesAsync(int idState)
        {
            return Get<List<CityResponse>>("Company/getcitiesbystate?" + GenerateQueryParameters(new { IdState = idState }));
        }

        public Task<UserInfoResponse> GetUserInfo()
        {
            return Get<UserInfoResponse>("CompanyUser/getInfo");
        }

        public Task<UserInfoResponse> PatchUserAsync(PatchUserInfoRequest request)
        {
            return Patch<UserInfoResponse>("CompanyUser/patch", request);
        }

        public Task<bool> ChangePasswordAsync(ChangePasswordRequest request)
        {
            return Post<bool>("CompanyUser/changepassword", request);
        }

        public async Task<ReservationInfoResponse> GetReservationInfoAsync(Guid reservationId)
        {
            return await Get<ReservationInfoResponse>("SoccerPitchReservation/getInfo?" + GenerateQueryParameters(new { reservationId }));
        }

        public async Task<List<PlansResponse>> GetPlansAsync()
        {
            return await Get<List<PlansResponse>>("SoccerPitchPlan/get");
        }

        public async Task<PlansResponse> PostSoccerPitchPlanAsync(SoccerPitchPlanRequest request)
        {
            return await Post<PlansResponse>("SoccerPitchPlan/post", request);
        }

        public async Task<PlansResponse> PatchSoccerPitchPlanAsync(SoccerPitchPlanRequest request)
        {
            return await Patch<PlansResponse>("SoccerPitchPlan/patch", request);
        }

        public async Task<List<SoccerPitchResponse>> GetSoccerPitchsAsync()
        {
            return await Get<List<SoccerPitchResponse>>("SoccerPitch/get");
        }
    }
}
