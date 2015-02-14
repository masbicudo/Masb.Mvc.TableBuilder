using System.Web.WebPages;
using JetBrains.Annotations;

namespace Masb.Mvc.TableBuilder
{
    /// <summary>
    /// Represents the ability to render named sections.
    /// </summary>
    public interface ISectionRenderer
    {
        /// <summary>
        /// Renders a named section.
        /// </summary>
        /// <param name="sectionName">Name of the section to render.</param>
        /// <returns>A <see cref="HelperResult"/> that writes the section to the output stream.</returns>
        [NotNull]
        HelperResult RenderSection([NotNull] string sectionName);

        /// <summary>
        /// Renders a named section.
        /// </summary>
        /// <param name="sectionName">Name of the section to render.</param>
        /// <param name="required">A value indicating whether the section is required or not.</param>
        /// <returns>A <see cref="HelperResult"/> that writes the section to the output stream.</returns>
        [ContractAnnotation("canbenull <= required: false; notnull <= required: true")]
        HelperResult RenderSection([NotNull] string sectionName, bool required);

        /// <summary>
        /// Returns a value indicating whether the named section is defined or not.
        /// </summary>
        /// <param name="sectionName">Name of the section to test.</param>
        /// <returns>True if the section is defined; otherwise False.</returns>
        bool IsSectionDefined([NotNull] string sectionName);
    }
}