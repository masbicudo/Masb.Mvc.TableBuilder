using System;
using System.Web.Mvc;
using System.Web.WebPages;
using JetBrains.Annotations;

namespace Masb.Mvc.TableBuilder
{
    /// <summary>
    /// Represents a table data cell rendering helper, that renders based on a column template.
    /// </summary>
    /// <typeparam name="TCollectionItem">Type of the collection item, rendered as data rows.</typeparam>
    /// <typeparam name="TSubProperty">Type of the sub-property of the item, rendered as a cell.</typeparam>
    public class TableDataCellRenderer<TCollectionItem, TSubProperty> :
        ITableDataCellRenderer,
        IHelperContext<TSubProperty>
    {
        private readonly ITableColumnTemplate<TCollectionItem, TSubProperty> tableColumnTemplate;
        private readonly ITemplateArgs<TSubProperty> templateArgs;

        public TableDataCellRenderer(
            ITableColumnTemplate<TCollectionItem, TSubProperty> tableColumnTemplate,
            ITemplateArgs<TSubProperty> templateArgs)
        {
            this.tableColumnTemplate = tableColumnTemplate;
            this.templateArgs = templateArgs;
        }

        /// <summary>
        /// Renders the current data cell contents.
        /// </summary>
        /// <returns>A <see cref="HelperResult"/> that writes the contents of the cell to the output stream.</returns>
        public HelperResult Render()
        {
            var result = this.tableColumnTemplate.GetDataHelperResult(this.templateArgs);
            return result;
        }

        /// <summary>
        /// Renders a named section.
        /// </summary>
        /// <param name="sectionName">Name of the section to render.</param>
        /// <returns>A <see cref="HelperResult"/> that writes the section to the output stream.</returns>
        public HelperResult RenderSection(string sectionName)
        {
            return this.RenderSection(sectionName, true);
        }

        /// <summary>
        /// Renders a named section.
        /// </summary>
        /// <param name="sectionName">Name of the section to render.</param>
        /// <param name="required">A value indicating whether the section is required or not.</param>
        /// <returns>A <see cref="HelperResult"/> that writes the section to the output stream.</returns>
        [ContractAnnotation("null <= required: false; notnull <= required: true")]
        public HelperResult RenderSection(string sectionName, bool required)
        {
            var helperResult = this.GetHelperResult(sectionName);

            if (helperResult == null && required)
                throw new Exception(string.Format("Section must be defined: {0}", sectionName));

            return helperResult;
        }

        /// <summary>
        /// Returns a value indicating whether the named section is defined or not.
        /// </summary>
        /// <param name="sectionName">Name of the section to test.</param>
        /// <returns>True if the section is defined; otherwise False.</returns>
        public bool IsSectionDefined(string sectionName)
        {
            if (sectionName == null)
                throw new ArgumentNullException("sectionName");

            return this.tableColumnTemplate.IsSectionDefined(sectionName);
        }

        private HelperResult GetHelperResult(string sectionName)
        {
            if (sectionName != null && !this.tableColumnTemplate.IsSectionDefined(sectionName))
                return null;

            var result = sectionName == null
                ? this.tableColumnTemplate.GetHeaderHelperResult(this.templateArgs)
                : this.tableColumnTemplate.GetSectionHelperResult(sectionName, this.templateArgs);

            return result;
        }

        /// <summary>
        /// Gets the <see cref="T:System.Web.Mvc.AjaxHelper"/> object that is used to render HTML markup using Ajax.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Web.Mvc.AjaxHelper"/> object that is used to render HTML markup using Ajax.
        /// </returns>
        public AjaxHelper<TSubProperty> Ajax
        {
            get { return this.templateArgs.Ajax; }
        }

        /// <summary>
        /// Gets the <see cref="T:System.Web.Mvc.HtmlHelper"/> object that is used to render HTML elements.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Web.Mvc.HtmlHelper"/> object that is used to render HTML elements.
        /// </returns>
        HtmlHelper<TSubProperty> IHelperContext<TSubProperty>.Html
        {
            get { return this.templateArgs.Html; }
        }

        /// <summary>
        /// Gets the current model object or value.
        /// </summary>
        public TSubProperty Model
        {
            get { return this.templateArgs.Model; }
        }

        /// <summary>
        /// Gets the <see cref="ModelMetadata"/> associated with this object.
        /// </summary>
        public ModelMetadata Meta
        {
            get { return this.templateArgs.Meta; }
        }

        /// <summary>
        /// Gets the <see cref="T:System.Web.Mvc.UrlHelper"/> of the rendered snippet.
        /// </summary>
        public UrlHelper Url
        {
            get { return this.templateArgs.Url; }
        }

        /// <summary>
        /// Gets the <see cref="T:System.Web.Mvc.AjaxHelper"/> object that is used to render HTML markup using Ajax.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Web.Mvc.AjaxHelper"/> object that is used to render HTML markup using Ajax.
        /// </returns>
        AjaxHelper IHelperContext.Ajax
        {
            get { return this.templateArgs.Ajax; }
        }

        /// <summary>
        /// Gets the <see cref="T:System.Web.Mvc.HtmlHelper"/> object that is used to render HTML elements.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Web.Mvc.HtmlHelper"/> object that is used to render HTML elements.
        /// </returns>
        HtmlHelper IHelperContext.Html
        {
            get { return this.templateArgs.Html; }
        }
    }
}