using Acr.UserDialogs;
using EasySoccer.Mobile.Adm.API.ApiRequest;
using EasySoccer.Mobile.Adm.API.ApiResponses;
using EasySoccer.Mobile.Adm.API.Infra.Exceptions;
using EasySoccer.Mobile.Adm.API.Session;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
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
                    throw new ApiUnauthorizedException("Ops! Você não está mais autenticado. Será necessário refazer login.");
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
                var propValue = item.GetValue(parameters);
                if (propValue != null)
                {
                    if (propValue.GetType() == typeof(DateTime))
                    {
                        var dateProp = (DateTime)propValue;
                        queryString.Add(item.Name, dateProp.ToString(CultureInfo.InvariantCulture));
                    }
                    else
                        queryString.Add(item.Name, propValue.ToString());
                }
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

        public Task<object> PostSoccerPitchImageAsync(SoccerPitchImageRequest soccerPitchImageRequest)
        {
            return Post<object>("SoccerPitch/saveImage", soccerPitchImageRequest);
        }

        public async Task<List<SportTypeResponse>> GetSportTypesAsync()
        {
            return await Get<List<SportTypeResponse>>("SoccerPitch/getsporttypes");
        }

        public async Task<List<ColorsResponse>> GetColorsAsync()
        {
            return await Get<List<ColorsResponse>>("SoccerPitch/getcolors");
        }

        public async Task<SoccerPitchResponse> GetSoccerPitchByIdAsync(long id)
        {
            return await Get<SoccerPitchResponse>("SoccerPitch/getbyid?" + GenerateQueryParameters(new { Id = id }));
        }

        public async Task<SoccerPitchResponse> PostSoccerPitchAsync(SoccerPitchRequest request)
        {
            return await Post<SoccerPitchResponse>("SoccerPitch/post", request);
        }

        public async Task<SoccerPitchResponse> PatchSoccerPitchAsync(SoccerPitchRequest request)
        {
            return await Patch<SoccerPitchResponse>("SoccerPitch/patch", request);
        }

        public async Task<InserTokenResponse> InserTokenAsync(InserTokenRequest request)
        {
            return await Post<InserTokenResponse>("CompanyUser/inserttoken", request);
        }

        public async Task<object> LogOffTokenAsync(InserTokenRequest request)
        {
            return await Post<object>("CompanyUser/logofftoken", request);
        }

        public async Task<List<CompanyUserNotificationResponse>> GetNotificationsAsync()
        {
            return await Get<List<CompanyUserNotificationResponse>>("CompanyUser/getNotifications");
        }

        public Task<object> PostPaymentAsync(PaymentRequest request)
        {
            return Post<object>("CompanyUser/payment", request);
        }

        public Task<object> PostChangeReservationStatus(ChangeStatusRequest request)
        {
            return Post<object>("SoccerPitchReservation/changeStatus", request);
        }

        public async Task<ReservationsResponse> GetReservationsAsync(DateTime? initialDate, DateTime? finalDate, long? soccerPitchId, int? soccerPitchPlanId, string userName, int[] status, int page = 1, int pageSize = 10)
        {
            string statusStr = null;
            if (status != null && status.Length <= 0)
                status = null;
            else if (status != null)
            {
                statusStr = string.Join(";", status);
            }
            return await Get<ReservationsResponse>("SoccerPitchReservation/get?" + GenerateQueryParameters(new
            {
                InitialDate = initialDate,
                FinalDate = finalDate,
                SoccerPitchId = soccerPitchId,
                SoccerPitchPlanId = soccerPitchPlanId,
                UserName = userName,
                Status = statusStr,
                Page = page,
                PageSize = pageSize
            }));
        }

        public async Task<List<StatusResponse>> GetStatusAsync()
        {
            return await Get<List<StatusResponse>>("SoccerPitchReservation/getReservationStatus");
        }

        public async Task<CompanyScheduleHourResponse> GetCompanyHourStartEndAsync(long companyId, int dayOfWeek)
        {
            return await Get<CompanyScheduleHourResponse>("CompanySchedule/get?" + GenerateQueryParameters(new { companyId, dayOfWeek }));
        }

        public async Task<List<PlansResponse>> GetPlansBySoccerPitchIdAsync(long soccerPitchId)
        {
            return await Get<List<PlansResponse>>("SoccerPitchPlan/getbysoccerpitch?" + GenerateQueryParameters(new { soccerPitchId }));
        }

        public async Task<PostReservationResponse> PostReservationAsync(SoccerPitchReservationRequest request)
        {
            return await Post<PostReservationResponse>("SoccerPitchReservation/post", request);
        }

        public async Task<PostReservationResponse> PatchReservationAsync(SoccerPitchReservationRequest request)
        {
            return await Patch<PostReservationResponse>("SoccerPitchReservation/patch", request);
        }
    }
}
