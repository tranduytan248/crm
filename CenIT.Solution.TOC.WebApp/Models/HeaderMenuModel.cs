namespace CenIT.Solution.TOC.WebApp.Models
{
    public class HeaderMenuModel
    {
        public string MenuKey { get; set; }
        public string MenuName { get; set; }
        public string MenuHref { get; set; }
        public bool IsActive { get; set; } = false;
    }
}