using System.Collections.Generic;

namespace TSFramework.Libs.Models.Breadcrumb
{
    public class BreadcrumbModel
    {
        public string Title { get; set; }

        public string Href { get; set; }

        public bool IsCurrent { get; set; } = false;

        public Queue<BreadcrumbModel> PrevBreadcrumbs { get; set; } = new Queue<BreadcrumbModel>();
    }
}