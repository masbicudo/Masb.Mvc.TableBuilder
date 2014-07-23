using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc.Html;
using System.Web.WebPages;

namespace Masb.Mvc.TableBuilder
{
    public static class TableTemplateExtensions
    {
        public static TableTemplate<TModel, TCollectionItem> AddEditorColumnFor<TModel, TCollectionItem, TSubProperty>(
            [NotNull]this TableTemplate<TModel, TCollectionItem> tableTemplate,
            [NotNull]Expression<Func<TCollectionItem, TSubProperty>> subPropertyExpression,
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

    public class FluentColumnConfig<TSubProperty>
    {
        [NotNull]
        private readonly string[] sectionNames;

        private readonly Dictionary<string, Func<ITemplateArgs<TSubProperty>, HelperResult>> sectionFuncs
            = new Dictionary<string, Func<ITemplateArgs<TSubProperty>, HelperResult>>();

        private Func<ITemplateArgs, HelperResult> headerFunc;

        private Func<ITemplateArgs<TSubProperty>, HelperResult> dataFunc;

        public FluentColumnConfig(IEnumerable<string> sectionNames)
        {
            sectionNames = sectionNames ?? Enumerable.Empty<string>();
            this.sectionNames = sectionNames.ToArray();
        }

        public FluentColumnConfig<TSubProperty> Header([NotNull] Func<ITemplateArgs, HelperResult> header)
        {
            if (header == null)
                throw new ArgumentNullException("header");

            if (this.headerFunc != null)
                throw new InvalidOperationException("Header template already set.");

            this.headerFunc = header;
            return this;
        }

        public FluentColumnConfig<TSubProperty> Content([NotNull] Func<ITemplateArgs<TSubProperty>, HelperResult> data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            if (this.dataFunc != null)
                throw new InvalidOperationException("Content template already set.");

            this.dataFunc = data;
            return this;
        }

        public FluentColumnConfig<TSubProperty> Section(
            [NotNull] string sectionName,
            [NotNull] Func<ITemplateArgs<TSubProperty>, HelperResult> section)
        {
            if (sectionName == null)
                throw new ArgumentNullException("sectionName");

            if (section == null)
                throw new ArgumentNullException("section");

            if (!this.sectionNames.Contains(sectionName))
                throw new InvalidOperationException(
                    string.Format(
                        "Section name not defined using SetColumnSections: {0}",
                        sectionName));

            if (this.sectionFuncs.ContainsKey(sectionName))
                throw new InvalidOperationException(
                    string.Format(
                        "Section template already set for {0}",
                        sectionName));

            this.sectionFuncs.Add(sectionName, section);
            return this;
        }

        public FluentColumnConfig<TSubProperty> Sections(
            params Func<ITemplateArgs<TSubProperty>, HelperResult>[] sections)
        {
            if (this.sectionFuncs.Count > 0)
                throw new InvalidOperationException(
                    "Sections were already added. "
                    + "You must either add them one by one (with Section method), "
                    + "or all at once (with Sections method).");

            var definedCount = this.sectionNames.Length;
            var sectionsCount = sections == null ? 0 : sections.Length;

            if (definedCount != sectionsCount)
                throw new Exception("Number of defined sections is different from the number of passed sections.");

            for (int it = 0; it < sectionsCount; it++)
            {
                // ReSharper disable PossibleNullReferenceException
                this.sectionFuncs.Add(this.sectionNames[it], sections[it]);
                // ReSharper restore PossibleNullReferenceException
            }

            return this;
        }

        internal void ConfigTableTemplate<TModel, TCollectionItem>(
            [NotNull] Expression<Func<TCollectionItem, TSubProperty>> subPropertyExpression,
            TableTemplate<TModel, TCollectionItem> tableTemplate)
        {
            tableTemplate.AddColumnFor(
                subPropertyExpression,
                this.headerFunc,
                this.dataFunc,
                this.sectionNames.Select(this.sectionFuncs.GetValueOrDefault).ToArray());
        }
    }

    internal static class DicExtensions
    {
        internal static TValue GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dic, TKey key)
        {
            TValue value;
            dic.TryGetValue(key, out value);
            return value;
        }
    }
}