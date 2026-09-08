using System.Linq;
using System.Web.Mvc;
using TSFramework.Libs.Principals;
using TSFramework.Libs.Processors;

namespace TSFramework.Libs.BaseApps
{
    public abstract class BaseViewPage : WebViewPage
    {
        protected new virtual AppPrincipal User => base.User as AppPrincipal;

        public string RenderButton(bool isModal, string modalId, string eleClass, string urlAction, string icon,
            string title)
        {
            var pActions = !string.IsNullOrEmpty(urlAction) ? urlAction.Split('/') : new string[] { };
            pActions = pActions.Where(x => !string.IsNullOrEmpty(x)).ToArray();

            if (pActions.Length <= 2) return string.Empty;
            if (pActions.Length == 2)
                if (!AppProcessor.Author.IsAllow(Request.RequestContext.HttpContext, Request.RequestContext.HttpContext.User.Identity.Name, null, pActions[0], pActions[1]))
                    return string.Empty;

            if (pActions.Length == 3)
                if (!AppProcessor.Author.IsAllow(Request.RequestContext.HttpContext, Request.RequestContext.HttpContext.User.Identity.Name, pActions[0], pActions[1], pActions[2]))
                    return string.Empty;

            var sModal = isModal ? "data-modal=''" : "";
            var buttonTemplate =
                $"<a {sModal} data-modal-id='{modalId}' class='{eleClass}' href='{urlAction}'>{icon}&nbsp;{title}</a>";
            return buttonTemplate;
        }
    }

    public abstract class BaseViewPage<TModel> : WebViewPage<TModel>
    {
        public string Title
        {
            get => ViewBag.Title;
            set => ViewBag.Title = value;
        }

        protected new virtual AppPrincipal User => base.User as AppPrincipal;

        protected string RenderButton(bool isModal, string modalId, string eleClass, string urlAction, string icon,
            string title)
        {
            var pActions = !string.IsNullOrEmpty(urlAction) ? urlAction.Split('/') : new string[] { };
            pActions = pActions.Where(x => !string.IsNullOrEmpty(x)).ToArray();

            if (pActions.Length <= 2) return string.Empty;
            if (pActions.Length == 2)
                if (!AppProcessor.Author.IsAllow(Request.RequestContext.HttpContext, Request.RequestContext.HttpContext.User.Identity.Name, null, pActions[0], pActions[1]))
                    return string.Empty;

            if (pActions.Length == 3)
                if (!AppProcessor.Author.IsAllow(Request.RequestContext.HttpContext, Request.RequestContext.HttpContext.User.Identity.Name, pActions[0], pActions[1], pActions[2]))
                    return string.Empty;

            var sModal = isModal ? "data-modal=''" : "";
            var buttonTemplate =
                $"<a {sModal} data-modal-id='{modalId}' class='{eleClass}' href='{urlAction}'>{icon}&nbsp;{title}</a>";
            return buttonTemplate;
        }

        private ViewDataDictionary<TModel> _viewData;

        public new ViewDataDictionary<TModel> ViewData
        {
            get
            {
                if (_viewData == null)
                {
                    SetViewData(new ViewDataDictionary<TModel>());
                }
                return _viewData;
            }
            set => SetViewData(value);
        }

        protected override void SetViewData(ViewDataDictionary viewData)
        {
            _viewData = new ViewDataDictionary<TModel>(viewData);

            base.SetViewData(_viewData);
        }
    }
}