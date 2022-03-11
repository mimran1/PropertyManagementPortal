using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentalPropertyManagement.Services
{
    public class PageHistoryService
    {
        private List<string> previousPages;
        public bool FirstVisit { get; set; }

        public PageHistoryService()
        {
            previousPages = new List<string>();
            FirstVisit = true;
        }
        public void AddPageToHistory(string pageName)
        {
            previousPages.Add(pageName);
        }

        public string GetGoBackPage()
        {
            if (previousPages.Count > 1)
            {
                // You add a page on initialization, so you need to return the 2nd from the last
                return previousPages.ElementAt(previousPages.Count - 2);
            }

            // Can't go back because you didn't navigate enough
            return previousPages.FirstOrDefault();
        }

        public bool CanGoBack()
        {
            return previousPages.Count > 1;
        }
    }
}
