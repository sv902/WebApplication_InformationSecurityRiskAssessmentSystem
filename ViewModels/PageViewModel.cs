namespace WebApplication_InformationSecurityRiskAssessmentSystem.ViewModels
{
    public class PageViewModel
    {
        public int PageNumber { get; set; }

        public int TotalPages { get; set; }

        public int PageSize { get; set; }

        public PageViewModel(int count, int pageNumder, int pageSize)
        {
            PageNumber = pageNumder;
            PageSize = pageSize;
            TotalPages = (int) Math.Ceiling((double) count / pageSize);
        }

        public bool HasPreviousPage
        {
            get { return (PageNumber > 1); }
        }

        public bool HasNextPage
        {
            get { return (PageNumber < TotalPages); }
        }
    }
}
