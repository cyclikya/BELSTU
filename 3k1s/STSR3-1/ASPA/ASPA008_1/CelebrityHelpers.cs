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

        //public static HtmlString BeginForm(this IHtmlHelper html, FormMethod method, object type)
        //{
        //    string enctype = string.Empty;
        //    if (type != null)
        //    {
        //        var dict = HtmlHelper.AnonymousObjectToHtmlAttributes(type);
        //        if (dict.TryGetValue("enctype", out object encValue))
        //        {
        //            enctype = $" enctype=\"{encValue}\"";
        //        }
        //    }
        //    string result = $"<form method=\"{method}\" {enctype}>";
        //    return new HtmlString(result);
        //}

        //public static HtmlString DropDownList(this IHtmlHelper html, string nat, SelectList sel, object c)
        //{
        //    string cls = string.Empty;
        //    if (c != null)
        //    {
        //        var dict = HtmlHelper.AnonymousObjectToHtmlAttributes(c);
        //        if (dict.TryGetValue("class", out object cl))
        //        {
        //           cls = $" class\"{cl}\"";
        //        }
        //    }

        //    string opts = "";
        //    foreach(var opt in sel)
        //    {
        //        string select = opt.Selected ? " selected " : "";
        //        opts += $"<option value=\"{opt.Value}\"{select}>{opt.Text}</option>";
        //    }
        //    string result = $"<select name=\"{nat}\" id=\"{nat}\" {cls}>{opts}</select>";
        //    return new HtmlString(result);
        //}


    }
}
