using System.Web.Mvc.Html;
using System.Web.WebPages;
using JetBrains.Annotations;

namespace Masb.Mvc.TableBuilder
{
    public static class FluentColumnConfigExtensions
    {
        public static FluentColumnConfig<TSubProperty> Editor<TSubProperty>(
            this FluentColumnConfig<TSubProperty> columnConfig)
        {
            columnConfig.Content(item => new HelperResult(w => w.Write(item.Html.EditorFor(m => m).ToHtmlString())));
            return columnConfig;
        }

        public static FluentColumnConfig<TSubProperty> Editor<TSubProperty>(
            this FluentColumnConfig<TSubProperty> columnConfig,
            [NotNull][AspMvcEditorTemplate] string templateName)
        {
            columnConfig.Content(item => new HelperResult(w => w.Write(item.Html.EditorFor(m => m, templateName).ToHtmlString())));
            return columnConfig;
        }

        public static FluentColumnConfig<TSubProperty> DefaultHeader<TSubProperty>(
            this FluentColumnConfig<TSubProperty> columnConfig)
        {
            columnConfig.Header(item => new HelperResult(w => w.Write(item.Meta.GetDisplayName())));
            return columnConfig;
        }
    }
}