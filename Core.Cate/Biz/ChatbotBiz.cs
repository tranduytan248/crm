using System;
using System.Collections.Generic;
using Core.Cate.Models;
using TSFramework.Libs.Processors;

namespace Core.Cate.Biz
{
    public class ChatbotBiz
    {
        private const string DATA_PROVIDER_NAME = "CenIT.Provider.Major";
        private readonly string _Chatbot_Get = "Project_Opportunity_GetBotData";


        /// <summary>
        /// Lấy toàn bộ danh sách theo giá trị lọc
        /// </summary>
        /// <returns>Danh sách Chatbot</returns>
        public List<ChatbotModel> GetData(string username)
        {
            return AppProcessor.ProcedureProvider.ExecuteTypedList<ChatbotModel>(_Chatbot_Get, DATA_PROVIDER_NAME, username);
        }
    }
}
