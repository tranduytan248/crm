using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using TSFramework.Libs.Interfaces;
using TSFramework.Libs.Models.Message;

namespace TSFramework.Libs.Providers
{
    public class MessageProvider
    {
        public MessageProvider(IStoreProcedure storeProceduror, string langCode = "vi-Vn")
        {
            var tableName = ConfigurationManager.AppSettings["App_MessageTableName"] ?? "AppMessages";
            LangCode = langCode;
            TableName = tableName;
            StoreProceduror = storeProceduror;
            Messages = LoadDataMessages(langCode);
        }

        private string TableName { get; }
        private string LangCode { get; }
        private IStoreProcedure StoreProceduror { get; }

        private List<AppMessage> Messages { get; set; }

        public static MessageProvider Instance(Dictionary<string, IStoreProcedure> dicStoreProceduror,
            string langCode = "vi-Vn")
        {
            var sTypeDataProvider =
                ConfigurationManager.AppSettings["App_MessageTypeDataProvider"] ?? "";
            var storeProceduror = dicStoreProceduror[sTypeDataProvider];

            return new MessageProvider(storeProceduror, langCode);
        }

        private List<AppMessage> LoadDataMessages(string langCode = "vi-Vn")
        {
            var queryString = $@"SELECT
	                                    am.LangCode,
	                                    am.LabelKey,
	                                    am.[Message]
                                    FROM
	                                    {TableName} AS am
                                    WHERE am.LangCode = '{langCode}'
                                    ORDER BY am.LabelKey";

            var msgs = StoreProceduror.ExecuteQueryTypedList<AppMessage>(queryString);
            return msgs;
        }

        public string GetMessage(string langCode, string labelKey)
        {
            if (Messages == null) return string.Empty;
            var message = Messages.FirstOrDefault(m => m.LabelKey == labelKey && m.LangCode == langCode);
            return message != null ? message.Message : string.Empty;
        }

        public string GetMessage(string labelKey)
        {
            if (Messages == null) return string.Empty;
            var message = Messages.FirstOrDefault(m => m.LabelKey == labelKey && m.LangCode == LangCode);
            return message != null ? message.Message : string.Empty;
        }

        public void Refresh(string langCode = "vi-Vn")
        {
            Messages = LoadDataMessages(langCode);
        }
    }
}