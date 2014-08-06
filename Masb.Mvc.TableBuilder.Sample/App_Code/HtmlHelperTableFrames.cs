using System.Web.Mvc;
using System.Web.WebPages;

namespace Masb.Mvc.TableBuilder.Sample
{
    public static class HtmlHelperTableFrames
    {
        public static HelperResult MyCustomTable<TModel>(this HtmlHelper<TModel> html, ITableTemplate<TModel> tableTemplate)
        {
            var result = ASP.TableFrames.MyCustomTable(html, tableTemplate);
            return result;
        }

        public static HelperResult MyCustomTableNoSections<TModel>(this HtmlHelper<TModel> html, ITableTemplate<TModel> tableTemplate)
        {
            var result = ASP.TableFrames.MyCustomTableNoSections(html, tableTemplate);
            return result;
        }
    }
}