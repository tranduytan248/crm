using Core.Cate.Biz;
using Core.Cate.Caches;
using Core.Cate.Models;
using Core.Cate.Services;
using Core.Sys.BaseApp;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using OpenXmlPowerTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Enums;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;

namespace Modules.Cate.Areas.Cate.Controllers
{
    public class ProjectOverviewController : AppController
    {
        private readonly RM_ProjectCache _projectCache;
        private readonly RM_ProductProjectCache _productProjectCache;
        private readonly RM_ContractsCache _contractsCache;
        private readonly RM_ContractsBiz _contractsBiz;
        private readonly RM_TaskManagementCache _taskManagementCache;
        private readonly RM_CommentCache _commentCache;
        private readonly RM_ReviewBatchItemCache _reviewBatchItemCache;
        private readonly RM_ReviewBatchItemBiz _reviewBatchItemBiz;
        private readonly TaskFileService _fileService;

        public ProjectOverviewController()
        {
            _projectCache = new RM_ProjectCache();
            _productProjectCache = new RM_ProductProjectCache();
            _contractsCache = new RM_ContractsCache();
            _contractsBiz = new RM_ContractsBiz();
            _taskManagementCache = new RM_TaskManagementCache();
            _commentCache = new RM_CommentCache();
            _reviewBatchItemCache = new RM_ReviewBatchItemCache();
            _reviewBatchItemBiz = new RM_ReviewBatchItemBiz();
            _fileService = new TaskFileService();
        }

        public ActionResult Index(int id, int? reviewBatchID)
        {
            ProjectOverviewModel model = new ProjectOverviewModel();
            var project = _projectCache.GetById(id);
            var productProject = _productProjectCache.GetByProjectID(id);
            var contracts = _contractsCache.GetByProjectID(id);
            var reviewHistory = _BuildReviewHistoryForm(2, id);

            foreach (var item in contracts)
            {
                item.ExistingFiles = _contractsBiz.GetFilePaths(item.ContractID);
            }
            foreach (var item in reviewHistory)
            {
                item.ExistingFiles = _reviewBatchItemBiz.GetFilePaths(item.ReviewHistoryID);
            }
            model.ProjectModel = project;
            model.ProductProjects = productProject;
            model.Contracts = contracts;
            model.ReviewHistory = reviewHistory;

            // thêm cờ hiển thị nút rà soát
            model.ReviewBatchID = reviewBatchID;

            if (productProject != null && productProject.Any())
            {
                model.TotalContract = contracts.Count;
                model.TotalProduct = productProject.Count;
                model.TotalExpectedRevenue = productProject.Sum(x => x.ExpectedRevenue);
                model.TotalRevenue = productProject.Sum(x => x.TotalRevenue);
                model.TotalCost = productProject.Sum(x => x.TotalCost);
            }

            if (reviewHistory != null && reviewHistory.Any())
            {
                model.TotalReviewHistory = reviewHistory.Count;
            }

            return View(model);
        }

        public ActionResult ListByProject(int id)
        {
            ViewData["ProjectID"] = id;
            var data = _productProjectCache.GetByProjectID(id);
            return PartialView("_Product", data);
        }

        public ActionResult GetProjectInfo(int id)
        {
            ProjectOverviewModel model = new ProjectOverviewModel();
            var project = _projectCache.GetById(id);
            var productProject = _productProjectCache.GetByProjectID(id);
            model.ProjectModel = project;
            model.ProductProjects = productProject;
            if (productProject != null && productProject.Any())
            {
                model.TotalProduct = productProject.Count;

                model.TotalExpectedRevenue = productProject.Sum(x => x.ExpectedRevenue);

                model.TotalRevenue = productProject.Sum(x => x.TotalRevenue);

                model.TotalCost = productProject.Sum(x => x.TotalCost);
            }
            return PartialView("_Project", model);
        }
        [AjaxOnly]
        [HttpGet]
        public ActionResult GetTaskManagementsByProjectID(int id)
        {
            var data = _commentCache.LoadListByProjectID(id);
            var model = new List<RM_CommentByProjectModel>();
            if (data != null)
            {
                //for (int i = 0; i < data.Count; i++)
                //{
                //    data[i].FilePaths = _fileService.GetFilePaths(
                //        commentId: data[i].CommentID
                //    );
                //}

                // danh sách ngày
                List<SelectListItem> listNgay = data
                     .OrderByDescending(x => x.CreatedDate)
                     .GroupBy(x => x.CreatedDate.GetValueOrDefault().Date) // Gom nhóm theo ngày (bỏ giờ)
                     .Select(g => new SelectListItem
                     {
                         Value = g.Key.ToString("yyyyMMdd"),
                         Text = g.Key.ToString("dd/MM/yyyy")
                     }).ToList();

                for (var i = 0; i < listNgay.Count; i++)
                {
                    var CommentByProject = new RM_CommentByProjectModel { Ngay = listNgay[i].Text };
                    List<SelectListItem> LstTask = data
                        .Where(x => x.CreatedDate.GetValueOrDefault().ToString("yyyyMMdd") == listNgay[i].Value)
                        .OrderByDescending(x => x.CreatedDate)
                        .GroupBy(x => x.TaskManagementID)
                        .Select(g => g.First())
                        .Select(x => new SelectListItem
                        {
                            Value = x.TaskManagementID.ToString(),
                            Text = x.TaskName
                        })
                        .ToList() ?? new List<SelectListItem>();

                    var cmtByTask = new List<RM_CommentByTaskModel>();

                    for (var j = 0; j < LstTask.Count; j++)
                    {
                        var task = data.Where(x => x.TaskManagementID.ToString() == LstTask[j].Value).FirstOrDefault();
                        cmtByTask.Add(new RM_CommentByTaskModel
                        {
                            TaskManagementID = task.TaskManagementID,
                            TaskName = task.TaskName,
                            AssigneeNames = task.AssigneeNames,
                            PriorityName = task.PriorityName,
                            Comments = data.Where(x => x.TaskManagementID.ToString() == LstTask[j].Value & x.CreatedDate.GetValueOrDefault().ToString("yyyyMMdd") == listNgay[i].Value)
                            .OrderByDescending(x => x.CreatedDate).ToList(),
                        });
                    }
                    model.Add(new RM_CommentByProjectModel
                    {
                        Ngay = listNgay[i].Text,
                        CommentsByTask = cmtByTask
                    });

                    cmtByTask = new List<RM_CommentByTaskModel>(); // reset
                }
            }
            //var data = _taskManagementCache.GetAll(id);
            //for (int i = 0; i < data.Count; i++)
            //{
            //    data[i].FilePaths = _fileService.GetFilePaths(
            //        taskManagementId: data[i].TaskManagementID);
            //}
            return PartialView("_TaskManagementsData", model);
        }
        public ActionResult GetContracts(int id)
        {
            ViewData["ProjectID"] = id;
            var data = _contractsCache.GetByProjectID(id);
            foreach (var item in data)
            {
                item.ExistingFiles = _contractsBiz.GetFilePaths(item.ContractID);
            }
            return PartialView("_Contracts", data);
        }

        /// <summary>
        /// Xây dựng model lịch sử rà soát kèm file đính kèm cho dự án.
        /// </summary>
        /// <returns>Model lịch sử rà soát phục vụ tab overview.</returns>
        private List<RM_ReviewHistoryModel> _BuildReviewHistoryForm(int objectType, int objectID)
        {
            // Lấy lịch sử rà soát
            var histories = _reviewBatchItemCache.GetHistory(objectType, objectID);

            var result = new List<RM_ReviewHistoryModel>();

            if (histories != null && histories.Count > 0)
            {
                int index = 1;

                foreach (var item in histories)
                {
                    // File đính kèm của từng lịch sử rà soát
                    var files = _reviewBatchItemBiz.GetFilePaths(item.ReviewHistoryID);

                    result.Add(new RM_ReviewHistoryModel
                    {
                        ReviewBatchID = item.ReviewBatchID,
                        ReviewBatchItemID = item.ReviewBatchItemID,
                        BatchCode = item.BatchCode,
                        BatchName = item.BatchName,
                        ReviewLevel = item.ReviewLevel,
                        ReviewComment = item.ReviewComment,
                        IsConfirmed = item.IsConfirmed,
                        ExistingFiles = files ?? new List<RM_ReviewBatchFilePathModel>(),
                        ReviewHistoryID = item.ReviewHistoryID,
                        Reviewer = item.Reviewer,
                        CanEdit = item.CreatedBy == User.UserName,
                        CreatedDate = item.CreatedDate
                    });

                    index++;
                }
            }
            return result;
        }
    }
}