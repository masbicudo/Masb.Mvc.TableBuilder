using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.WebPages;
using JetBrains.Annotations;
using Masb.Mvc.TableBuilder.Helpers;

namespace Masb.Mvc.TableBuilder
{
    /// <summary>
    /// Fluent configuration object for table columns.
    /// </summary>
    /// <typeparam name="TSubProperty">Type of the sub-property, i.e. the type to be rendered inside column's data cells.</typeparam>
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
}