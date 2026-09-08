using Core.Cate.Caches;
using Core.Cate.Models;
using Core.Sys.BaseApp;
using DocumentFormat.OpenXml.Office2010.Excel;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Web;
using TSFramework.Libs.Models.Base;
using System.Web.Mvc;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Enums;
using TSFramework.Libs.Processors;
using TSFramework.Libs.Utils;
using System.Collections.Generic;
using Core.Cate.Biz;
using DocumentFormat.OpenXml.EMMA;
using System.Reflection;

namespace Modules.Cate.Areas.Cate.Controllers
{
    public class CustomerContactController : AppController
    {
        private readonly RM_CustomerContactCache _customerContactCache;
        private readonly RM_ContactPersonsCache _contactPersonsCache;
        private readonly string _CustomerContactTitle = AppProcessor.Messagor.GetMessage("Customer_Title");

        public CustomerContactController()
        {
            _customerContactCache = new RM_CustomerContactCache();
            _contactPersonsCache = new RM_ContactPersonsCache();
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult Get(int CustomerID)
        {
            var draw = Request.Form.GetValues("draw")?[0];
            var startRec = Convert.ToInt32(Request.Form.GetValues("start")?[0]);
            var pageSize = Convert.ToInt32(Request.Form.GetValues("length")?[0]);
            int total = 0;

            List<RM_CustomerContactModel> data;
            if (CustomerID == 0)
            {
                data = Session["SessionCustomerContacts"] as List<RM_CustomerContactModel> ?? new List<RM_CustomerContactModel>();
                total = data.Count;
                data = data.Skip(startRec).Take(pageSize).ToList();
            }
            else
            {
                var Search = "";
                var order = Request.Form.GetValues("order[0][column]")?[0];
                var orderDir = Request.Form.GetValues("order[0][dir]")?[0];
                var dataSearch = new BaseSearchModel
                {
                    Search = Search,
                    Order = order,
                    OrderDir = orderDir,
                    StartIndex = startRec,
                    PageSize = pageSize,
                };
                data = _customerContactCache.Get(out total, CustomerID, dataSearch);

            }

            return Json(
                new { draw = Convert.ToInt32(draw), recordsTotal = total, recordsFiltered = total, data },
                JsonRequestBehavior.AllowGet);
        }

        [AjaxOnly]
        [ActionType(Type = EnumActionType.Create)]
        [HttpGet]
        public ActionResult AddCustomerContactDetail(int CustomerID, int ContactPersonID)
        {
            var modelContactPersons = _contactPersonsCache.GetById(ContactPersonID);
            var model = new RM_CustomerContactModel
            {
                CustomerID = CustomerID,
                ContactPersonID = ContactPersonID,
                FullName = modelContactPersons.FullName,
                Phone = modelContactPersons.Phone,
                Mobile = modelContactPersons.Mobile,
                Zalo = modelContactPersons.Zalo,
                Address = modelContactPersons.Address
            };

            // Lấy tên khách hàng cho tiêu đề
            var customerCache = new RM_CustomerCache();
            ViewBag.CustomerName = CustomerID > 0
                ? customerCache.GetById(CustomerID)?.CustomerName ?? ""
                : "Khách hàng mới";

            return PartialView("_AddCustomerContactDetail", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Create)]
        [ValidateInput(false)]
        public ActionResult AddCustomerContactDetail(RM_CustomerContactModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_CustomerContactDetail", model);
            }
            var contactPerson = _contactPersonsCache.GetById(model.ContactPersonID);

            if (contactPerson != null)
            {
                model.FullName = contactPerson.FullName;
                model.Phone = contactPerson.Phone;
                model.Mobile = contactPerson.Mobile;
               // model.Email = contactPerson.Email;
                model.Zalo = contactPerson.Zalo;
                model.Address = contactPerson.Address;
            }
            string response;

            // CASE TEMP
            if (model.CustomerID == 0)
            {
                var tempContacts = Session["SessionCustomerContacts"] as List<RM_CustomerContactModel>
                                   ?? new List<RM_CustomerContactModel>();

                model.TempID = tempContacts.Count > 0
                    ? tempContacts.Max(x => x.TempID) + 1
                    : 1;

                tempContacts.Add(model);
                Session["SessionCustomerContacts"] = tempContacts;
                response = CreateMessage($"{_CustomerContactTitle} [{model.FullName}]", EnumProcessType.Add, EnumMsgIcon.Success);

                return Json(new { status = true, message = response }, JsonRequestBehavior.AllowGet);
            }

            var result = _customerContactCache.Save(model, User.UserName);

            if (result == 0)
                response = CreateMessage($"{_CustomerContactTitle} [{model.FullName}]", EnumProcessType.Add, EnumMsgIcon.Error);
            else if (result == -9)
                response = CreateMessage($"{_CustomerContactTitle} [{model.FullName}]", EnumProcessType.DataExisted, EnumMsgIcon.Error);
            else
                response = CreateMessage($"{_CustomerContactTitle} [{model.FullName}]", EnumProcessType.Add, EnumMsgIcon.Success);

            return Json(new { status = true, message = response }, JsonRequestBehavior.AllowGet);

        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult EditCustomerContactDetail(int id, int ContactPersonID)
        {
            RM_CustomerContactModel model = null;

            // CASE TEMP (Customer chưa lưu)
            if (id == 0)
            {
                var tempContacts = Session["SessionCustomerContacts"] as List<RM_CustomerContactModel>
                                   ?? new List<RM_CustomerContactModel>();

                model = tempContacts.FirstOrDefault(x => x.ContactPersonID == ContactPersonID);
            }
            else
            {
                // CASE DB
                model = _customerContactCache.GetById(id);
            }

            if (model == null)
            {
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_CustomerContactTitle}",
                    EnumProcessType.DataNotExist,
                    EnumMsgIcon.Error)
                });
            }

            return PartialView("_EditCustomerContactDetail", model);
        }


        [AjaxOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionType(Type = EnumActionType.Edit)]
        [ValidateInput(false)]
        public ActionResult EditCustomerContactDetail(RM_CustomerContactModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_CustomerContactDetail", model);
            }

            string response;
            if (model.CustomerID == 0)
            {
                var tempContacts = Session["SessionCustomerContacts"] as List<RM_CustomerContactModel>;

                var item = tempContacts.FirstOrDefault(x => x.ContactPersonID == model.ContactPersonID);

                if (item != null)
                {
                    item.Position = model.Position;
                    item.Note = model.Note;
                    item.Email = model.Email;
                }

                Session["SessionCustomerContacts"] = tempContacts;
                response = CreateMessage($"{_CustomerContactTitle} [{item.FullName}]", EnumProcessType.Edit, EnumMsgIcon.Success);
                return Json(new { status = true, message = response });
            }
            var result = _customerContactCache.Save(model, User.UserName);

            if (result == 0)
                response = CreateMessage($"{_CustomerContactTitle} [{model.FullName}]", EnumProcessType.Edit, EnumMsgIcon.Error);
            else if (result == -9)
                response = CreateMessage($"{_CustomerContactTitle} [{model.FullName}]", EnumProcessType.DataExisted, EnumMsgIcon.Error);
            else
                response = CreateMessage($"{_CustomerContactTitle} [{model.FullName}]", EnumProcessType.Edit, EnumMsgIcon.Success);

            return Json(new { status = true, message = response }, JsonRequestBehavior.AllowGet);
        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult DeleteCustomerContactDetail(int id, int ContactPersonID)
        {
            RM_CustomerContactModel model = null;

            // CASE TEMP
            if (id == 0)
            {
                var tempContacts = Session["SessionCustomerContacts"] as List<RM_CustomerContactModel>
                                   ?? new List<RM_CustomerContactModel>();

                model = tempContacts.FirstOrDefault(x => x.ContactPersonID == ContactPersonID);
            }
            else
            {
                // CASE DB
                model = _customerContactCache.GetById(id);
            }

            if (model == null)
            {
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_CustomerContactTitle}",
                    EnumProcessType.DataNotExist,
                    EnumMsgIcon.Error)
                });
            }

            ViewBag.ConfirmMessage = string.Format(
                AppProcessor.Messagor.GetMessage("Message_Confirm_Delete"),
                $"{_CustomerContactTitle} [{model.FullName}]"
            );

            return PartialView("_DeleteCustomerContactDetail", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult DeleteCustomerContactDetail(RM_CustomerContactModel model)
        {
            string response;
            // Customer chưa lưu → dùng Session
            if (model.CustomerID == 0)
            {
                var tempContacts = Session["SessionCustomerContacts"] as List<RM_CustomerContactModel>
                                   ?? new List<RM_CustomerContactModel>();

                tempContacts = tempContacts
                    .Where(x => x.TempID != model.TempID)
                    .ToList();

                Session["SessionCustomerContacts"] = tempContacts;
                response = CreateMessage($"{_CustomerContactTitle} [{model.FullName}]", EnumProcessType.Delete, EnumMsgIcon.Success);

                return Json(new
                {
                    status = true,
                    message = response
                });
            }

            // DB
            var deleted = _customerContactCache.Delete(model, User.UserName);

            response = CreateMessage($"{_CustomerContactTitle} [{model.FullName}]",
                EnumProcessType.Delete,
                deleted > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error);

            return Json(new { status = true, message = response });
        }
    }
}