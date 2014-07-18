using JetBrains.Annotations;
using System;
using System.Web.WebPages;

namespace Masb.Mvc.TableBuilder
{
    public static class SectionRendererExtensions
    {
        /// <summary>
        /// Renders a section if it is defined.
        /// Otherwise, the helper defined by `<paramref name="defaultFunc"/>` is used.
        /// </summary>
        /// <param name="renderer">Renderer that we want to render to.</param>
        /// <param name="sectionName">Section name to render.</param>
        /// <param name="defaultFunc">Helper that is rendered when the section is not defined.</param>
        /// <returns>HelperResult containing the rendered information.</returns>
        public static HelperResult RenderSection(
            [NotNull] this ISectionRenderer renderer,
            [NotNull] string sectionName,
            [NotNull] Func<object, HelperResult> defaultFunc)
        {
            if (renderer == null)
                throw new ArgumentNullException("renderer");

            if (defaultFunc == null)
                throw new ArgumentNullException("defaultFunc");

            if (renderer.IsSectionDefined(sectionName))
                return renderer.RenderSection(sectionName);

            return defaultFunc(null);
        }

        /// <summary>
        /// Renders a section, passing it's HelperResult to another helper for rendering.
        /// If the section is not defined, nothing is rendered.
        /// </summary>
        /// <param name="renderer">Renderer that we want to render to.</param>
        /// <param name="sectionName">Section name to render.</param>
        /// <param name="renderWith">Helper that renders the section's HelperResult.</param>
        /// <returns>HelperResult containing the rendered information.</returns>
        public static HelperResult RenderSectionWith(
          [NotNull] this ISectionRenderer renderer,
          [NotNull] string sectionName,
          [NotNull] Func<HelperResult, HelperResult> renderWith)
        {
            if (renderer == null)
                throw new ArgumentNullException("renderer");

            if (renderWith == null)
                throw new ArgumentNullException("renderWith");

            if (renderer.IsSectionDefined(sectionName))
                return renderWith(renderer.RenderSection(sectionName));

            return null;
        }

        /// <summary>
        /// Renders a section, passing it's HelperResult to another helper for rendering.
        /// If the section is not defined, the helper defined by `<paramref name="defaultFunc"/>` is used.
        /// </summary>
        /// <param name="renderer">Renderer that we want to render to.</param>
        /// <param name="sectionName">Section name to render.</param>
        /// <param name="renderWith">Helper that renders the section's HelperResult.</param>
        /// <param name="defaultFunc">Helper that is rendered when the section is not defined.</param>
        /// <returns>HelperResult containing the rendered information.</returns>
        public static HelperResult RenderSectionWith(
          [NotNull] this ISectionRenderer renderer,
          [NotNull] string sectionName,
          [NotNull] Func<HelperResult, HelperResult> renderWith,
          [NotNull] Func<HelperResult, HelperResult> defaultFunc)
        {
            if (renderer == null)
                throw new ArgumentNullException("renderer");

            if (renderWith == null)
                throw new ArgumentNullException("renderWith");

            if (renderer.IsSectionDefined(sectionName))
                return renderWith(renderer.RenderSection(sectionName));

            return defaultFunc(null);
        }
    }
}
