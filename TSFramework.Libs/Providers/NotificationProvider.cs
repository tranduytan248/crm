using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using TSFramework.Libs.Enums;
using TSFramework.Libs.Helpers;
using TSFramework.Libs.Hubs;

namespace TSFramework.Libs.Providers
{
    public class NotificationProvider
    {
        private static IHubContext _notifyHub;
        private static MessageProvider _message;

        public NotificationProvider(MessageProvider message)
        {
            _notifyHub = GlobalHost.ConnectionManager.GetHubContext<AppSignalRHub>();
            _message = message;
        }

        public string CreateMessage(string messageContent, EnumProcessType typeProcess, EnumMsgIcon icon,
            string sUrl = "", string sTarget = "", string sPlacement = "bl")
        {
            var sType = string.Empty;
            var sIcon = string.Empty;
            var sMessage = string.Empty;

            switch (icon)
            {
                case EnumMsgIcon.Success:
                    sType = "success";
                    sIcon = "fa fa-check-circle";
                    break;
                case EnumMsgIcon.Error:
                    sType = "danger";
                    sIcon = "fa fa-exclamation-circle";
                    break;
                case EnumMsgIcon.Info:
                    sType = "info";
                    sIcon = "fa fa-info-circle";
                    break;
                case EnumMsgIcon.Warning:
                    sType = "warning";
                    sIcon = "fa fa-exclamation-triangle";
                    break;
            }

            switch (typeProcess)
            {
                case EnumProcessType.Add:
                    sMessage = _message.GetMessage("Common_Add" + icon);
                    break;
                case EnumProcessType.Edit:
                    sMessage = _message.GetMessage("Common_Update" + icon);
                    break;
                case EnumProcessType.Delete:
                    sMessage = _message.GetMessage("Common_Delete" + icon);
                    break;
                case EnumProcessType.Confirm:
                    sMessage = _message.GetMessage("Common_Confirm" + icon);
                    break;
                case EnumProcessType.Destroy:
                    sMessage = _message.GetMessage("Common_Destroy" + icon);
                    break;
                case EnumProcessType.Recreate:
                    sMessage = _message.GetMessage("Common_Recreate" + icon);
                    break;
                case EnumProcessType.Return:
                    sMessage = _message.GetMessage("Common_Return" + icon);
                    break;
                case EnumProcessType.Create:
                    sMessage = _message.GetMessage("Common_Create" + icon);
                    break;
                case EnumProcessType.DataExisted:
                    sMessage = _message.GetMessage("Common_DataExisted");
                    break;
                case EnumProcessType.DataNotExist:
                    sMessage = _message.GetMessage("Common_DataNotExist");
                    break;
                case EnumProcessType.NonFormat:
                    sMessage = _message.GetMessage("Common_NonFormat");
                    break;
                case EnumProcessType.Convert:
                    sMessage = _message.GetMessage("Common_Convert" + icon);
                    break;
                case EnumProcessType.DataUsed:
                    sMessage = _message.GetMessage("Common_DataUsed");
                    break;
            }

            var sTitle = _message.GetMessage($"{EnumHelper.GetDescription(icon)}_Title");
            var formattedMessage = string.Format(sMessage, "<b>" + (messageContent ?? string.Empty) + "</b>");

            return string.Format(
                "showNotify('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');",
                EncodeJsString(sTitle),
                EncodeJsString(sIcon),
                EncodeJsString(formattedMessage),
                EncodeJsString(sUrl),
                EncodeJsString(sTarget),
                EncodeJsString(sType),
                EncodeJsString(sPlacement));
        }

        public string CreateNotify(string messageContent, EnumProcessType typeProcess, EnumMsgIcon icon)
        {
            var sMessage = string.Empty;

            var sFunction = EnumHelper.GetDescription(icon);
            sFunction = sFunction?.ToLower();

            switch (typeProcess)
            {
                case EnumProcessType.Add:
                    sMessage = _message.GetMessage("Common_Add" + icon);
                    break;
                case EnumProcessType.Edit:
                    sMessage = _message.GetMessage("Common_Update" + icon);
                    break;
                case EnumProcessType.Delete:
                    sMessage = _message.GetMessage("Common_Delete" + icon);
                    break;
                case EnumProcessType.Confirm:
                    sMessage = _message.GetMessage("Common_Confirm" + icon);
                    break;
                case EnumProcessType.Destroy:
                    sMessage = _message.GetMessage("Common_Destroy" + icon);
                    break;
                case EnumProcessType.Recreate:
                    sMessage = _message.GetMessage("Common_Recreate" + icon);
                    break;
                case EnumProcessType.Return:
                    sMessage = _message.GetMessage("Common_Return" + icon);
                    break;
                case EnumProcessType.Create:
                    sMessage = _message.GetMessage("Common_Create" + icon);
                    break;
                case EnumProcessType.DataExisted:
                    sMessage = _message.GetMessage("Common_DataExisted");
                    break;
                case EnumProcessType.DataNotExist:
                    sMessage = _message.GetMessage("Common_DataNotExist");
                    break;
                case EnumProcessType.NonFormat:
                    sMessage = _message.GetMessage("Common_NonFormat");
                    break;
                case EnumProcessType.Convert:
                    sMessage = _message.GetMessage("Common_Convert" + icon);
                    break;
                case EnumProcessType.DataUsed:
                    sMessage = _message.GetMessage("Common_DataUsed");
                    break;
            }

            sMessage = string.Format(sMessage, "<b>" + (messageContent ?? string.Empty) + "</b>");
            return $"toastr.{sFunction}('{EncodeJsString(sMessage)}');";
        }

        public void PushNotify(string sReceiver, string sMessage)
        {
            var sSender = "Sys";
            Task.Run(() => _notifyHub.Clients.User(sReceiver).OnNotify(sSender, sMessage));
        }

        public void PushNotify(string sReceiver, string sMessage, EnumProcessType typeProcess, EnumMsgIcon icon)
        {
            var sSender = "Sys";
            sMessage = CreateNotify(sMessage, typeProcess, icon);
            Task.Run(() => _notifyHub.Clients.User(sReceiver).OnNotify(sSender, sMessage));
        }

        public void PushNotifyToGroup(string sSender, string sGroupName, string sMessage, EnumProcessType typeProcess,
            EnumMsgIcon icon)
        {
            sMessage = CreateNotify(sMessage, typeProcess, icon);
            Task.Run(() => _notifyHub.Clients.Group(sGroupName).OnNotify(sSender, sMessage));
        }

        public void PushNotifyToGroup(string sSender, string sGroupName, string sMessage)
        {
            Task.Run(() => _notifyHub.Clients.Group(sGroupName).OnNotify(sSender, sMessage));
        }

        public void PushNotifyToUser(string sSender, string sReceiver, string sMessage, EnumProcessType typeProcess,
            EnumMsgIcon icon)
        {
            sMessage = CreateNotify(sMessage, typeProcess, icon);
            Task.Run(() => _notifyHub.Clients.User(sReceiver).OnNotify(sSender, sMessage));
        }

        public void PushNotifyToUser(string sSender, string sReceiver, string sMessage)
        {
            Task.Run(() => _notifyHub.Clients.User(sReceiver).OnNotify(sSender, sMessage));
        }

        public void BroadcastNotify(string sSender, string sMessage)
        {
            Task.Run(() => _notifyHub.Clients.Group("Authenticated").OnBroadcast(sSender, sMessage));
        }

        public void ForceLogout(string userName)
        {
            Task.Run(() => _notifyHub.Clients.User(userName).OnForceLogout(userName));
        }

        public static void Broadcast(string sMessage, int iType = 1)
        {
            Task.Run(() => _notifyHub.Clients.All.OnBroadcast(sMessage, iType));
        }

        private static string EncodeJsString(string value)
        {
            return HttpUtility.JavaScriptStringEncode(value ?? string.Empty);
        }
    }
}
