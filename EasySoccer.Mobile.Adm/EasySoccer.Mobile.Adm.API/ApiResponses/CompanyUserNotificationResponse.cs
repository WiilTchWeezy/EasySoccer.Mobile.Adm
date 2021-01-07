using EasySoccer.Mobile.Adm.API.ApiResponses.Enums;
using System;

namespace EasySoccer.Mobile.Adm.API.ApiResponses
{
    public class CompanyUserNotificationResponse
    {
        public Guid Id { get; set; }

        public NotificationTypeEnum NotificationTypeEnum { get; set; }

        public string Title { get; set; }

        public string Message { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool Read { get; set; }

        public long IdCompanyUser { get; set; }
    }
}
