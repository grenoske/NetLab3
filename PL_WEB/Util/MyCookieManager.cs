
namespace PL_WEB.Util
{
    public interface IMyCookieManager
    {
        int PageMove(string PageSaveName, int pageAction, HttpRequest httpRequest, HttpResponse httpResponse);
    }

    public class MyCookieManager: IMyCookieManager
    {
        public int PageMove(string PageSaveName, int pageAction, HttpRequest httpRequest, HttpResponse httpResponse)
        {
            int page = pageAction;
            if (httpRequest.Cookies.TryGetValue(PageSaveName, out string pageNumber1))
            {
                page = int.Parse(pageNumber1);
                if (pageAction == -1 && page > 1)
                    page--;              // prev page
                else if (pageAction == 2)
                    page++;              // next  page           
            }

            // saving page number in cookie
            httpResponse.Cookies.Append(PageSaveName, page.ToString());
            return page;
        }
    }
}
