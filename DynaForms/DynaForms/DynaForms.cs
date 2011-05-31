using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Dynamic;
using System.Text;
using System.Reflection;

namespace DynaForms
{
    public enum InputType
    {
        undefined,
        html,
        text,
        select,
        textarea,
        checkbox,
        hidden,
        submit
    }

    public static class ObjectExtensions
    {
        public static Dictionary<string, object> ToDictionary(this object o)
        {

            var result = new Dictionary<string, object>();
            if (o.GetType() == typeof(NameValueCollection) || o.GetType().IsSubclassOf(typeof(NameValueCollection)))
            {
                var nv = (NameValueCollection)o;
                nv.Cast<string>().Select(key => new KeyValuePair<string, object>(key, nv[key])).ToList().ForEach(i => result.Add(i.Key, i));
            }
            else
            {
                var props = o.GetType().GetProperties();
                foreach (var item in props)
                {
                    result.Add(item.Name, item.GetValue(o, null));
                }
            }
            return result;
        }
    }

    public partial class DynaFormTemplates
    {
        public static string TemplateFormHeader = @"<form {idName} action='{action}' method='{method}'{optionalClassCssName}>";

        public static string TemplateErrorMessage = @"<div class='error'>{errorMessage}</div>";

        public static string TemplateIdName = @"id='{name}' name='{name}'";

        public static string TemplateInputText = @"
 <div{optionalClassCssName}>
  <label for='{name}'{optionalClassLabelCssName}>{labelText}</label>
  <input type='text' {idName} value='{value}'/>{errorMessage}
 </div>";
        public static string TemplateTextArea = @"
 <div{optionalClassCssName}>
  <label for='{name}'{optionalClassLabelCssName}>{labelText}</label>
  <textarea {idName}>{value}</textarea>{errorMessage}
 </div>";
        public static string TemplateCheckbox = @"
 <div{optionalClassCssName}>
  <label for='{name}'{optionalClassLabelCssName}>{labelText}</label>
  <input type='checkbox' {idName} {optional} value='{value}'/>{errorMessage}
 </div>";
        public static string TemplateSelect = @"
 <div{optionalClassCssName}>
  <label for='{name}'{optionalClassLabelCssName}>{labelText}</label>
  <select {idName}>{optional}
  </select>
 </div>";
        public static string TemplateSelectOption = @"
    <option value='{key}'>{value}</option>";
        public static string TemplateSubmit = @"
 <div{optionalClassCssName}>
  <input type='submit' {idName} value='{name}'/>{errorMessage}
 </div>";
        public static string TemplateHidden = @"
  <input type='hidden' {idName} value='{value}'/>";

        public static string MessageRequiredField = "Required field ";
        public static string MessageInvalidEmailAddress = "Invalid email address ";
        public static string MessageMinimumLengthIs = "Minimum length is ";
        public static string MessageMaximumLengthIs = "Maximum length is ";
        public static string MessageInvalid = "Invalid ";
        public static string MessageNumeric = "Numeric value required ";
        public static string MessageMinValueIs = "Minimum value is ";
        public static string MessageMaxValueIs = "Maximum value is ";

    }

    public class DynaForm
    {
        public List<FormField> Fields { get; set; }
        public string Name { get; set; }
        public string CssName { get; set; }
        public object Model { get; set; }
        public bool AutoPopulateModel { get; set; }
        public bool AutoAddSubmit { get; set; }
        private ValidationResult validationResult;

        public IDictionary<string, object> ModelDictionary
        {
            get
            {
                if (Model.GetType() == typeof(ExpandoObject))
                {
                    return (IDictionary<string, object>)(Model);
                }
                else
                {
                    return Model.ToDictionary();
                }
            }
        }

        public class FormField
        {

            public string LabelText { get; set; }
            public InputType Type { get; set; }
            public string FieldName { get; set; }
            public bool Required { get; set; }
            public bool Email { get; set; }
            public int MinLength { get; set; }
            public int MaxLength { get; set; }
            public string RegEx { get; set; }
            public int? Min { get; set; }
            public int? Max { get; set; }
            public bool Numeric { get; set; }
            public string Html { get; set; }
            public string Template { get; set; }
            public object DefaultValue { get; set; }
            public string CssName { get; set; }
            public string LabelCssName { get; set; }
            public string ErrorMessage { get; set; }

            public Dictionary<string, string> DropDownValueList { get; set; }

            public string Label()
            {
                if (LabelText == "")
                    return FieldName;
                else
                    return LabelText;
            }

            //public string RequiredMessage { get; set; }
            //public string EmailMessage { get; set; }
            //public string MinLengthMessage { get; set; }
            //public string MaxLengthMessage { get; set; }
            //public string RegExMessage { get; set; }            


        }

        public DynaForm(string name, object model = null, string cssName = "", bool autoAddSubmit = true)
        {
            Name = name;
            CssName = cssName;
            if (model != null)
            {

                //if (model.GetType() == typeof(WebMatrix.Data.DynamicRecord))
                //{
                //    // convert to expando
                //    var expandoModel = new ExpandoObject();
                //    var dictionary = expandoModel as IDictionary<string, object>;

                //    var dr = model as WebMatrix.Data.DynamicRecord;
                //    foreach (var c in dr.Columns)
                //    {
                //        dictionary.Add(c, dr[c]);
                //    }
                //    model = expandoModel;
                //}

                Model = model;
                AutoPopulateModel = false;
            }
            else
            {
                Model = new ExpandoObject();
                AutoPopulateModel = true;
            }
            AutoAddSubmit = autoAddSubmit;

            Fields = new List<FormField>();
        }

        public class ValidationResult
        {
            public struct FieldError
            {
                public string Field;
                public string Error;
                public string Label;
            }
            public bool IsValid { get; set; }
            public List<FieldError> Errors { get; set; }
            public ValidationResult()
            {
                Errors = new List<FieldError>();
                IsValid = true;
            }
            public void AddError(string fieldName, string errorMessage, string label)
            {
                Errors.Add(new FieldError() { Field = fieldName, Error = errorMessage, Label = label });
            }
            public string ValidationResultMessage(string rowTemplate)
            {
                string retval = "";
                if (!IsValid)
                {
                    foreach (var r in Errors)
                    {
                        retval += rowTemplate.Replace("{name}", r.Field).Replace("{label}", r.Label).Replace("{error}", r.Error);
                    }
                }
                return retval;
            }
        }

        public bool ContainsFieldName(string fieldName)
        {
            foreach (var x in Fields)
            {
                if (x.FieldName == fieldName)
                {
                    return true;
                }
            }
            return false;
        }

        private static string ReplacerIdName(string fieldName)
        {
            return DynaFormTemplates.TemplateIdName.Replace("{id}", fieldName).Replace("{name}", fieldName);
        }
        public static string ReplacerKeyValue(string template, string key, string value, string optional)
        {
            var retval = template
                .Replace("{key}", key)
                .Replace("{value}", value)
                .Replace("{optional}", optional);
            return retval;
        }
        public static string ReplacerFormHeader(string template, string name, string method, string action, string cssName = "")
        {
            var retval = template.Replace("{idName}", ReplacerIdName(name))
                        .Replace("{method}", method)
                        .Replace("{action}", action);

            if (cssName != "")
            {
                retval = retval.Replace("{optionalClassCssName}", " class='{cssName}'");
            }
            else
            {
                retval = retval.Replace("{optionalClassCssName}", "");
            }
            retval = retval.Replace("{cssName}", cssName);

            return retval;
        }
        public static string ReplacerField(string template, string fieldName, string labelText, string value = "", string optional = "", string cssName = "", string errorMessage = "", string labelCssName = "")
        {
            var retval = template.Replace("{idName}", ReplacerIdName(fieldName))
                        .Replace("{name}", fieldName)
                        .Replace("{labelText}", labelText)
                        .Replace("{value}", value)
                        .Replace("{optional}", optional)
                        .Replace("{errorMessage}", errorMessage);

            if (cssName != "")
            {
                retval = retval.Replace("{optionalClassCssName}", " class='{cssName}'");
            }
            else
            {
                retval = retval.Replace("{optionalClassCssName}", "");
            }
            retval = retval.Replace("{cssName}", cssName);

            if (labelCssName != "")
            {
                retval = retval.Replace("{optionalClassLabelCssName}", " class='{labelCssName}'");
            }
            else
            {
                retval = retval.Replace("{optionalClassLabelCssName}", "");
            }
            retval = retval.Replace("{labelCssName}", labelCssName);

            
            return retval;
        }

        public DynaForms.DynaForm AddHtml(string html)
        {
            var f = new FormField();
            f.Html = html;
            f.Type = InputType.html;
            Fields.Add(f);

            return this;
        }
        public DynaForms.DynaForm UpdateFormField(string fieldName, string labelText = null, InputType type = InputType.undefined, string cssName = null, bool? required = null, bool? email = null, bool? isNumeric = null, int? maxLength = null, int? minLength = null, int? max = null, int? min = null, string regEx = null, Dictionary<string, string> dropDownValues = null, string template = null, string errorMessage = null, object defaultValue = null, bool? updateModel = null)
        {
            var f = Fields.Where(w => w.FieldName == fieldName).SingleOrDefault();
            if (f == null)
            {
                //return AddFormField(fieldName, labelText, type, cssName, required, email, isNumeric, maxLength, minLength, max, min, regEx, dropDownValues, template, errorMessage, defaultValue, updateModel);
                return this;
            }
            else
            {
                if (labelText!=null) f.LabelText = labelText;
                if (type != InputType.undefined) f.Type = type;
                if (cssName!=null) f.CssName = cssName;
                if (required != null) f.Required = required.GetValueOrDefault();
                if (email != null) f.Email = email.GetValueOrDefault();
                if (minLength != null) f.MinLength = minLength.GetValueOrDefault();
                if (maxLength != null) f.MaxLength = maxLength.GetValueOrDefault();
                if (regEx!=null) f.RegEx = regEx??"";
                if (min!=null) f.Min = min;
                if (max!=null) f.Max = max;
                if (template != null) f.Template = template??"";
                if (dropDownValues!=null) f.DropDownValueList = dropDownValues;
                if (defaultValue!=null) f.DefaultValue = defaultValue;
                if (errorMessage != null) f.ErrorMessage = errorMessage??"";
                
                return this;
            }
        }
        public DynaForms.DynaForm RemoveFormField(string fieldName)
        {
            for (int n = 0; n < this.Fields.Count; n++)
            {
                if (this.Fields[n].FieldName == fieldName) this.Fields.RemoveAt(n);
            }
            //if (ModelDictionary.ContainsKey(fieldName)) ModelDictionary.Remove(fieldName);
            return this;

        }
        public DynaForms.DynaForm AddFormField(string fieldName, string labelText = "", InputType type = InputType.text, string cssName = "", bool required = false, bool email = false, bool numeric = false, int maxLength = 0, int minLength = 0, int? max = null, int? min = null, string regEx = "", Dictionary<string, string> dropDownValues = null, string template = "", string errorMessage = "", object defaultValue = null, bool updateModel = false, string labelCssName="")
        {
            var f = new FormField();
            f.FieldName = fieldName;
            f.LabelText = labelText;
            f.Type = type;
            f.CssName = cssName;
            f.Required = required;
            f.Email = email;
            f.MinLength = minLength;
            f.MaxLength = maxLength;
            f.RegEx = regEx;
            f.Min = min;
            f.Max = max;
            f.Numeric = numeric;
            f.Template = template;
            f.DropDownValueList = dropDownValues;
            f.ErrorMessage = errorMessage;
            f.LabelCssName = labelCssName;

            if ((updateModel || this.AutoPopulateModel) && this.Model.GetType() == typeof(ExpandoObject))
            {
                //if (this.Model == null) this.Model = new ExpandoObject();
                //var d = this.Model as IDictionary<string, object>;

                if (f.Type == InputType.checkbox)
                {
                    defaultValue = defaultValue ?? false;
                }
                else if (min != null || max != null)
                {
                    defaultValue = defaultValue ?? 0;
                }
                else if (type == InputType.select && f.DropDownValueList != null)
                {
                    var ddl = (Dictionary<string, string>)f.DropDownValueList;
                    defaultValue = ddl.FirstOrDefault().Value;
                }
                else
                {
                    defaultValue = defaultValue ?? "";
                }
                ModelDictionary.Add(f.FieldName, defaultValue);
            }

            f.DefaultValue = defaultValue;
            Fields.Add(f);

            return this;
        }

        public static bool IsValidRegex(string value, string regEx)
        {
            var rx = new Regex(regEx);
            return rx.IsMatch(value);
        }
        public static bool IsValidEmail(string email)
        {
            // source: http://thedailywtf.com/Articles/Validating_Email_Addresses.aspx
            var emailRegEx = @"^[-!#$%&'*+/0-9=?A-Z^_a-z{|}~](\.?[-!#$%&'*+/0-9=?A-Z^_a-z{|}~])*@[a-zA-Z](-?[a-zA-Z0-9])*(\.[a-zA-Z](-?[a-zA-Z0-9])*)+$";
            return IsValidRegex(email, emailRegEx);
        }

        public ValidationResult Validation
        {
            get { return validationResult; }
            set { validationResult = value; }
        }

        public void ClearModel()
        {
            foreach (var f in Fields)
            {
                if (ModelDictionary.ContainsKey(f.FieldName))
                {
                    if (Model.GetType() == typeof(ExpandoObject))
                    {
                        ModelDictionary[f.FieldName] = f.DefaultValue;
                    }
                    else
                    {
                        Model.GetType().GetProperty(f.FieldName).SetValue(Model, f.DefaultValue, null);
                    }
                }
            }


        }

        public ValidationResult Validate(object model, ValidationResult validationResult = null)
        {
            if (validationResult == null)
                validationResult = new ValidationResult();

            this.Model = model;

            foreach (var x in Fields.Where(f => f.Type != InputType.html))
            {

                var dictionaryValueString = "";
                bool isNumeric = false;
                decimal dictionaryValueDecimal = 0;

                if (ModelDictionary.ContainsKey(x.FieldName) && ModelDictionary[x.FieldName] != null)
                {
                    dictionaryValueString = ModelDictionary[x.FieldName].ToString();
                    if (Decimal.TryParse(dictionaryValueString, out dictionaryValueDecimal))
                        isNumeric = true;
                }

                if (x.Required && string.IsNullOrEmpty(dictionaryValueString))
                {
                    validationResult.AddError(x.FieldName, DynaFormTemplates.MessageRequiredField, x.Label());
                }
                if (x.Email && !IsValidEmail(dictionaryValueString))
                {
                    validationResult.AddError(x.FieldName, DynaFormTemplates.MessageInvalidEmailAddress, x.Label());
                }
                if (x.MinLength != 0 && dictionaryValueString.Length < x.MinLength)
                {
                    validationResult.AddError(x.FieldName, DynaFormTemplates.MessageMinimumLengthIs + x.MinLength.ToString(), x.Label());
                }
                if (x.MaxLength != 0 && dictionaryValueString.Length > x.MaxLength)
                {
                    validationResult.AddError(x.FieldName, DynaFormTemplates.MessageMaximumLengthIs + x.MaxLength.ToString(), x.Label());
                }
                if (x.Numeric && !isNumeric)
                {
                    validationResult.AddError(x.FieldName, DynaFormTemplates.MessageNumeric, x.Label());
                }
                if (x.Min != null && (isNumeric || dictionaryValueDecimal < x.Min))
                {
                    validationResult.AddError(x.FieldName, DynaFormTemplates.MessageMinValueIs + x.Min.ToString(), x.Label());
                }
                if (x.Max != null && (isNumeric || dictionaryValueDecimal > x.Max))
                {
                    validationResult.AddError(x.FieldName, DynaFormTemplates.MessageMaxValueIs + x.Max.ToString(), x.Label());
                }
                if (x.RegEx != "" && !IsValidRegex(dictionaryValueString, x.RegEx))
                {
                    validationResult.AddError(x.FieldName, DynaFormTemplates.MessageInvalid, x.Label());
                }
            }
            validationResult.IsValid = (validationResult.Errors.Count == 0);
            this.validationResult = validationResult;
            return validationResult;
        }
        public HtmlString Html(object model = null, string action = "#", string method = "post", bool omitFormTag = false, string formHeaderTemplate = "")
        {

            var sb = new StringBuilder();
            if (!omitFormTag)
            {
                if (formHeaderTemplate == "") formHeaderTemplate = DynaFormTemplates.TemplateFormHeader;
                var formHeader = ReplacerFormHeader(formHeaderTemplate, this.Name, method, action, this.CssName);
                sb.Append(formHeader);
            }

            if (model != null)
            {
                this.Model = model;
            }

            if (Fields.Count() == 0)
            {
                AutoPopulateFormFields();
            }

            //if (model == null)
            //{
            //    var createExpando = new ExpandoObject();
            //    var expandoDictionary = createExpando as IDictionary<string, object>;
            //    foreach (var f in Fields)
            //    {
            //        expandoDictionary.Add(f.FieldName, "");
            //    }
            //    this.Model = createExpando;                
            //}

            // Is there a global error message?
            string errorMessage = "";
            if (validationResult != null && !validationResult.IsValid)
            {
                foreach (var e in validationResult.Errors)
                {
                    if (e.Field == "")
                    {
                        if (errorMessage != "") errorMessage += ", ";
                        errorMessage += e.Error;
                    }
                }
            }
            if (errorMessage != "") sb.Append(DynaFormTemplates.TemplateErrorMessage.Replace("{errorMessage}", errorMessage));

            //
            // Create the Html tags for the form
            //

            foreach (var f in Fields)
            {

                if (f.Type == InputType.html)
                {
                    sb.Append(f.Html);
                }
                else
                {

                    var labelText = f.Label();

                    string value = "";

                    if (this.Model != null && this.ModelDictionary.ContainsKey(f.FieldName) && this.ModelDictionary[f.FieldName] != null)
                    {
                        value = this.ModelDictionary[f.FieldName].ToString();
                    }
                    else
                    {
                        value = f.DefaultValue.ToString();
                    }

                    errorMessage = "";
                    if (validationResult != null && !validationResult.IsValid)
                    {
                        foreach (var e in validationResult.Errors)
                        {
                            if (e.Field == f.FieldName)
                            {
                                if (errorMessage != "") errorMessage += ", ";
                                errorMessage += e.Error;
                            }
                        }
                    }
                    if (f.Type == InputType.text)
                    {
                        var template = (f.Template != "") ? f.Template : DynaFormTemplates.TemplateInputText;

                        var html = ReplacerField(template, f.FieldName, labelText, value,cssName:f.CssName,errorMessage: errorMessage,labelCssName:f.LabelCssName);
                        sb.Append(html);
                    }
                    if (f.Type == InputType.textarea)
                    {
                        var template = (f.Template != "") ? f.Template : DynaFormTemplates.TemplateTextArea;
                        var html = ReplacerField(template, f.FieldName, labelText, value, cssName: f.CssName, errorMessage: errorMessage, labelCssName: f.LabelCssName);
                        sb.Append(html);
                    }
                    if (f.Type == InputType.checkbox)
                    {
                        var template = (f.Template != "") ? f.Template : DynaFormTemplates.TemplateCheckbox;

                        var boolValue = false;
                        Boolean.TryParse(value, out boolValue);

                        var optional = boolValue ? "checked='checked'" : "";
                        var html = ReplacerField(template, f.FieldName, labelText, value, optional, cssName: f.CssName, errorMessage: errorMessage, labelCssName: f.LabelCssName);
                        sb.Append(html);

                    }

                    if (f.Type == InputType.select)
                    {
                        var template = (f.Template != "") ? f.Template : DynaFormTemplates.TemplateSelect;

                        var htmlOptional = new StringBuilder();
                        foreach (var item in f.DropDownValueList)
                        {
                            var optional = (item.Key != value) ? "selected='selected'" : "";
                            var htmlChild = ReplacerKeyValue(DynaFormTemplates.TemplateSelectOption, item.Key, item.Value, optional);
                            htmlOptional.Append(htmlChild);
                        }
                        var html = ReplacerField(template, f.FieldName, labelText, value, htmlOptional.ToString(), cssName: f.CssName, errorMessage: errorMessage, labelCssName: f.LabelCssName);
                        sb.Append(html);
                    }

                    if (f.Type == InputType.submit)
                    {
                        var template = (f.Template != "") ? f.Template : DynaFormTemplates.TemplateSubmit;

                        var html = ReplacerField(template, f.FieldName, labelText, value, cssName: f.CssName, errorMessage: errorMessage, labelCssName: f.LabelCssName);
                        sb.Append(html);
                    }
                    if (f.Type == InputType.hidden)
                    {
                        var template = (f.Template != "") ? f.Template : DynaFormTemplates.TemplateHidden;

                        var html = ReplacerField(template, f.FieldName, labelText, value:value);
                        sb.Append(html);
                    }
                }
            }

            if (this.AutoAddSubmit && !(Fields.Where(f => f.Type == InputType.submit).Any()))
            {
                sb.Append(ReplacerField(DynaFormTemplates.TemplateSubmit, "submit", "Submit", "Submit", ""));
            }

            if (!omitFormTag)
                sb.Append("</form>\n");
            return new HtmlString(sb.ToString());
        }
        public string ClientSideScript()
        {
            var script = @"<script type='text/javascript'>
jQuery(document).ready(function() { 
jQuery('#{formname}').validate({{json}});
});
</script>";

            var retval = script.Replace("{formname}", Name).Replace("{json}", jQueryValidateRulesJson());
            return retval;

        }
        public string jQueryValidateRulesJson()
        {
            var jsonString = "rules: {\n";
            var rules = "";
            var messages = "";
            foreach (var h in Fields)
            {
                var fieldRules = "";

                if (h.Required)
                {
                    fieldRules += "required:true,";
                }
                if (h.Email)
                {
                    fieldRules += "email:true,";
                }
                if (h.MaxLength != 0)
                {
                    fieldRules += "maxlength:" + h.MaxLength.ToString() + ",";
                }
                if (h.MinLength != 0)
                {
                    fieldRules += "minlength:" + h.MinLength.ToString() + ",";
                }
                if (h.Max != null)
                {
                    fieldRules += "max :" + h.Max.ToString() + ",";
                }
                if (h.Min != null)
                {
                    fieldRules += "min :" + h.Min.ToString() + ",";
                }

                if (fieldRules != "")
                {
                    fieldRules = fieldRules.TrimEnd(',');
                    if (rules != "") { rules += ",\n"; }
                    rules += h.FieldName + ": {";
                    rules += fieldRules + "}";
                }
                if (h.ErrorMessage != "")
                {
                    messages += h.FieldName + ": '" + h.ErrorMessage + "', ";
                }

            }
            jsonString += rules;

            jsonString += "}\n";

            if (messages != "")
            {
                messages = messages.TrimEnd(',');
                jsonString += ",\nmessages: {\n" + messages + "\n}";
            }
            

            return jsonString;
        }

        public void AutoPopulateFormFields()
        {
            foreach (var n in ModelDictionary)
            {
                AddFormField(n.Key);
            }
        }

        public ValidationResult TryUpdateModel(NameValueCollection newValuesDictionary = null, object model = null)
        {

            if (newValuesDictionary == null)
                newValuesDictionary = System.Web.HttpContext.Current.Request.Form;

            if (model == null) model = this.Model; else this.Model = model;

            if (Fields.Count() == 0)
            {
                AutoPopulateFormFields();
            }

            var validationResult = new ValidationResult();

            try
            {
                foreach (var varFields in Fields.Where(f => f.Type != InputType.html))
                {
                    var newValueKey = varFields.FieldName;
                    var newValueString = "";
                    if (newValuesDictionary.AllKeys.Contains(newValueKey))
                    {
                        newValueString = newValuesDictionary[newValueKey];
                    }

                    if (ModelDictionary.Keys.Contains(newValueKey))
                    {

                        System.Type updateType;

                        object newTypedValue;

                        if (model.GetType() == typeof(ExpandoObject))
                        {
                            if (ModelDictionary[newValueKey] != null)
                                updateType = ModelDictionary[newValueKey].GetType();
                            else
                                updateType = typeof(String);
                        }
                        else
                        {
                            updateType = model.GetType().GetProperty(newValueKey).PropertyType;
                        }

                        if (updateType == typeof(Int32))
                        {
                            Int32 setValue = 0;
                            Int32.TryParse(newValueString, out setValue);
                            newTypedValue = setValue;
                        }
                        else if (updateType == typeof(Int32?))
                        {
                            Int32 setValue = 0;
                            if (Int32.TryParse(newValueString, out setValue))
                            {
                                newTypedValue = setValue;
                            }
                            else
                            {
                                newTypedValue = null;
                            }
                        }
                        else if (updateType == typeof(Int64))
                        {
                            Int64 setValue = 0;
                            Int64.TryParse(newValueString, out setValue);
                            newTypedValue = setValue;
                        }
                        else if (updateType == typeof(Int64?))
                        {
                            Int64 setValue = 0;
                            if (Int64.TryParse(newValueString, out setValue))
                            {
                                newTypedValue = setValue;
                            }
                            else
                            {
                                newTypedValue = null;
                            }
                        }
                        else if (updateType == typeof(Single))
                        {
                            Single setValue = 0;
                            Single.TryParse(newValueString, out setValue);
                            newTypedValue = setValue;
                        }
                        else if (updateType == typeof(Single?))
                        {
                            Single setValue = 0;
                            if (Single.TryParse(newValueString, out setValue))
                            {
                                newTypedValue = setValue;
                            }
                            else
                            {
                                newTypedValue = null;
                            }
                        }
                        else if (updateType == typeof(Double))
                        {
                            Double setValue = 0;
                            Double.TryParse(newValueString, out setValue);
                            newTypedValue = setValue;
                        }
                        else if (updateType == typeof(Double?))
                        {
                            Double setValue = 0;
                            if (Double.TryParse(newValueString, out setValue))
                            {
                                newTypedValue = setValue;
                            }
                            else
                            {
                                newTypedValue = null;
                            }
                        }
                        else if (updateType == typeof(Decimal))
                        {
                            Decimal setValue = 0;
                            Decimal.TryParse(newValueString, out setValue);
                            newTypedValue = setValue;
                        }
                        else if (updateType == typeof(Decimal?))
                        {
                            Decimal setValue = 0;
                            if (Decimal.TryParse(newValueString, out setValue))
                            {
                                newTypedValue = setValue;
                            }
                            else
                            {
                                newTypedValue = null;
                            }
                        }
                        else if (updateType == typeof(Boolean))
                        {
                            Boolean setValue = (newValueString.ToString() != "");
                            newTypedValue = setValue;
                        }
                        else if (updateType == typeof(String))
                        {
                            newTypedValue = newValueString;
                        }
                        else if (updateType == typeof(DateTime?))
                        {
                            DateTime setValue;
                            if (DateTime.TryParse(newValueString, out setValue))
                            {
                                newTypedValue = setValue;
                            }
                            else
                            {
                                newTypedValue = null;
                            }
                        }
                        else if (updateType == typeof(DateTime))
                        {
                            DateTime setValue = DateTime.MinValue;
                            DateTime.TryParse(newValueString, out setValue);
                            newTypedValue = setValue;
                        }
                        else
                        {
                            newTypedValue = null;
                            new InvalidCastException("this type is not handled");
                        }

                        if (model.GetType() == typeof(ExpandoObject))
                        {
                            ((IDictionary<string, object>)model)[newValueKey] = newTypedValue;
                        }
                        else
                        {
                            var p = model.GetType().GetProperty(newValueKey);
                            p.SetValue(model, newTypedValue, null);
                        }
                    }
                }
                this.Model = model;
            }
            catch (Exception ex)
            {
                validationResult.AddError("", System.Web.HttpUtility.HtmlEncode(ex.Message), "");
                validationResult.IsValid = false;
            }
            this.validationResult = validationResult;

            if (!validationResult.IsValid)
                return validationResult;
            else
                return Validate(model, validationResult);
        }
    }
}