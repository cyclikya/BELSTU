using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace ASPA008_1
{
    public static class CelebrityHelpers
    {
        public static HtmlString CelebrityPhoto(this IHtmlHelper html, int id, string title, string src, int height = 0, int width = 0)
        {
            string onclick = "location.href = `/${this.id}`";
            string onload =
                "let k = this.naturalWidth / this.naturalHeight;" +
                $"if ({height} != 0 && {width} == 0) this.width = k * {height};" +
                $"if ({height} == 0 && {width} != 0) this.height = {width} / k;";
            string result = $"<" +
                                $"img id=\"{id}\"" +
                                $"class=\"celebrity-photo\"" +
                                $"title=\"{title}\"" +
                                $"src=\"{src}\"" +
                                $"onclick=\"{onclick}\"" +
                                $"onload=\"{onload}\"" +
                            $"/>";
            return new HtmlString(result);
        }


    }
}
