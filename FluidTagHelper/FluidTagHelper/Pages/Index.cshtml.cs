using FluidTagHelper.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace FluidTagHelper.Pages
{
    public class IndexModel : PageModel
    {
        public string FluidTemplate { get; set; }

        public Fruit[] FluidModel { get; set; } 

        public void OnGet()
        {
            FluidTemplate = @"<ul class=""list-group"">
    {% for fruit in model %}
    <li class=""list-group-item"">{{fruit.Name}} - {{fruit.Weight}} lbs</li>
    {% endfor %}
</ul>";

            FluidModel = new[]
            {
                new Fruit()
                {
                    Name = "Banana",
                    Color = "Yellow",
                    Weight = 0.75m
                },
                new Fruit()
                {
                    Name = "Orange",
                    Color = "Orange",
                    Weight = 1.0m
                }
            };
        }

        public void OnPost(string fluidTemplate, string fluidModel)
        {
            FluidTemplate = fluidTemplate;

            FluidModel = JsonConvert.DeserializeObject<Fruit[]>(fluidModel);
        }
    }
}
