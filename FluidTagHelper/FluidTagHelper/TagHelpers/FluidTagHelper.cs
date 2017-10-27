using Fluid;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace FluidTagHelper.TagHelpers
{
    public class FluidTagHelper : TagHelper
    {
        public string Template { get; set; }
        public object Model { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if(FluidTemplate.TryParse(Template, out var template))
            {
                var templateContext = new TemplateContext();
                templateContext.SetValue("model", Model);
                output.Content.SetHtmlContent(template.Render(templateContext));
            }

            base.Process(context, output);
        }
    }
}
