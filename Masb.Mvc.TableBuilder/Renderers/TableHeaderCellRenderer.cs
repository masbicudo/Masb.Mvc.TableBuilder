using System;
using System.Web.Mvc;
using System.Web.WebPages;
using JetBrains.Annotations;

namespace Masb.Mvc.TableBuilder
{
    public class TableHeaderCellRenderer<TSubProperty> :
        ITableHeaderCellRenderer,
        IViewTemplate<TSubProperty>
    {
        private readonly ITableColumnTemplateTo<TSubProperty> tableColumnTemplate;
        private readonly IViewTemplate<TSubProperty> viewTemplate;

        public TableHeaderCellRenderer(ITableColumnTemplateTo<TSubProperty> tableColumnTemplate, IViewTemplate<TSubProperty> viewTemplate)
        {
            this.tableColumnTemplate = tableColumnTemplate;
            this.viewTemplate = viewTemplate;
        }

        public HelperResult Render()
        {
            var helperResult = this.GetHelperResult(null);
            return helperResult;
        }

        public HelperResult RenderSection(string sectionName)
        {
            return this.RenderSection(sectionName, true);
        }

        [ContractAnnotation("null <= required: false; notnull <= required: true")]
        public HelperResult RenderSection(string sectionName, bool required)
        {
            var helperResult = this.GetHelperResult(sectionName);

            if (helperResult == null && required)
                throw new Exception(string.Format("Section must be defined: {0}", sectionName));

            return helperResult;
        }

        public bool IsSectionDefined([NotNull] string sectionName)
        {
            if (sectionName == null)
                throw new ArgumentNullException("sectionName");

            return this.tableColumnTemplate.IsSectionDefined(sectionName);
        }

        [CanBeNull]
        private HelperResult GetHelperResult([CanBeNull] string sectionName)
        {
            if (sectionName != null && !this.tableColumnTemplate.IsSectionDefined(sectionName))
                return null;

            var result = sectionName == null
                ? this.tableColumnTemplate.GetHeaderHelperResult(this.viewTemplate)
                : this.tableColumnTemplate.GetSectionHelperResult(sectionName, this.viewTemplate);

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
            get { return this.viewTemplate.Ajax; }
        }

        /// <summary>
        /// Gets the <see cref="T:System.Web.Mvc.HtmlHelper"/> object that is used to render HTML elements.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Web.Mvc.HtmlHelper"/> object that is used to render HTML elements.
        /// </returns>
        HtmlHelper<TSubProperty> IViewTemplate<TSubProperty>.Html
        {
            get { return this.viewTemplate.Html; }
        }

        /// <summary>
        /// Gets the current model object or value.
        /// </summary>
        public TSubProperty Model
        {
            get { throw new Exception("There is no model while rendering the headings of the table."); }
        }

        public ModelMetadata Meta
        {
            get { return this.viewTemplate.Meta; }
        }

        /// <summary>
        /// Gets the <see cref="T:System.Web.Mvc.UrlHelper"/> of the rendered snippet.
        /// </summary>
        public UrlHelper Url
        {
            get { return this.viewTemplate.Url; }
        }

        /// <summary>
        /// Gets the <see cref="T:System.Web.Mvc.AjaxHelper"/> object that is used to render HTML markup using Ajax.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Web.Mvc.AjaxHelper"/> object that is used to render HTML markup using Ajax.
        /// </returns>
        AjaxHelper IViewTemplate.Ajax
        {
            get { return this.viewTemplate.Ajax; }
        }

        /// <summary>
        /// Gets the <see cref="T:System.Web.Mvc.HtmlHelper"/> object that is used to render HTML elements.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Web.Mvc.HtmlHelper"/> object that is used to render HTML elements.
        /// </returns>
        HtmlHelper IViewTemplate.Html
        {
            get { return this.viewTemplate.Html; }
        }
    }
}