using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Web.Helpers
{
    
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString CustomLabel(this HtmlHelper htmlHelper, string name, string value)
        {
            var builder = new TagBuilder("label");
            builder.MergeAttribute("type", "text");
            builder.MergeAttribute("name", name);
            builder.MergeAttribute("value", value);
            if (String.IsNullOrEmpty(value)) {
                builder.MergeAttribute("style", "display: none");
            }
            return MvcHtmlString.Create(builder.ToString(TagRenderMode.SelfClosing));
        }


        //public static MvcHtmlString LabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression);
        //public static MvcHtmlString LabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, IDictionary<string, object> htmlAttributes);
        //public static MvcHtmlString LabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes);

        public static MvcHtmlString CustomLabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string label) {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
            string htmlFieldName = ExpressionHelper.GetExpressionText(expression);
            //string htmlFieldValue = Ex

            var temp = metadata.Model;

            if (metadata.Model == null)
                return MvcHtmlString.Empty;

            string labelText = metadata.DisplayName ?? metadata.PropertyName ?? htmlFieldName.Split('.').Last();
            if (!String.IsNullOrEmpty(label)) {
                labelText = label;
            }

            TagBuilder tag = new TagBuilder("label");
            //tag.MergeAttributes(htmlAttributes);
            tag.Attributes.Add("for", html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(htmlFieldName));
            tag.Attributes.Add("style", "display: inline-block");
            tag.SetInnerText(labelText);
            
            return MvcHtmlString.Create(tag.ToString(TagRenderMode.Normal));
        }
    }
}