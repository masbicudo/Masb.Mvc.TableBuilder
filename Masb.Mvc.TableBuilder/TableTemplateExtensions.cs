using JetBrains.Annotations;
using System;
using System.Linq.Expressions;
using System.Web.Mvc.Html;
using System.Web.WebPages;

namespace Masb.Mvc.TableBuilder
{
    public static class TableTemplateExtensions
    {
        public static TableTemplate<TModel, TCollectionItem> AddEditorColumnFor<TModel, TCollectionItem, TSubProperty>(
            [NotNull] this TableTemplate<TModel, TCollectionItem> tableTemplate,
            [NotNull] Expression<Func<TCollectionItem, TSubProperty>> subPropertyExpression,
            params Func<ITemplateArgs<TSubProperty>, HelperResult>[] sections)
        {
            tableTemplate.AddColumnFor(
                subPropertyExpression,
                item => new HelperResult(w => w.Write(item.Meta.GetDisplayName())),
                item => new HelperResult(w => w.Write(item.Html.EditorFor(m => m).ToHtmlString())),
                sections);

            return tableTemplate;
        }

        public static TableTemplate<TModel, TCollectionItem> AddColumnFor<TModel, TCollectionItem, TSubProperty>(
            [NotNull] this TableTemplate<TModel, TCollectionItem> tableTemplate,
            [NotNull] Expression<Func<TCollectionItem, TSubProperty>> subPropertyExpression,
            [NotNull] Func<FluentColumnConfig<TSubProperty>, FluentColumnConfig<TSubProperty>> configurator)
        {
            var config = new FluentColumnConfig<TSubProperty>(tableTemplate.ColumnSectioNames);
            config = configurator(config);
            config.ConfigTableTemplate(subPropertyExpression, tableTemplate);
            return tableTemplate;
        }

        public static TableTemplate<TModel, TCollectionItem> AddColumn<TModel, TCollectionItem, TSubProperty>(
            [NotNull] this TableTemplate<TModel, TCollectionItem> tableTemplate,
            [NotNull] Func<FluentColumnConfig<TCollectionItem>, FluentColumnConfig<TCollectionItem>> configurator)
        {
            var config = new FluentColumnConfig<TCollectionItem>(tableTemplate.ColumnSectioNames);
            config = configurator(config);
            config.ConfigTableTemplate(x => x, tableTemplate);
            return tableTemplate;
        }
    }
}