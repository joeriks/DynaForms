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
    public static class ObjectExtensions
    {
        public static dynamic ToExpando(this object o)
        {
            // Stolen from Rob Conery's Massive.cs

            var result = new ExpandoObject();
            var d = result as IDictionary<string, object>; //work with the Expando as a Dictionary
            if (o.GetType() == typeof(ExpandoObject)) return o; //shouldn't have to... but just in case
            if (o.GetType() == typeof(NameValueCollection) || o.GetType().IsSubclassOf(typeof(NameValueCollection)))
            {
                var nv = (NameValueCollection)o;
                nv.Cast<string>().Select(key => new KeyValuePair<string, object>(key, nv[key])).ToList().ForEach(i => d.Add(i));
            }
            else
            {
                var props = o.GetType().GetProperties();
                foreach (var item in props)
                {
                    d.Add(item.Name, item.GetValue(o, null));
                }
            }
            return result;
        }
        public static Dictionary<string, object> ToDictionary(this object o)
        {
            // Stolen from Rob Conery's Massive.cs
                        
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
    public class DynaForm
    {
        public List<FormField> Fields { get; set; }
        public string Name { get; set; }
        public object Model { get; set; }         
        public bool AutoPopulateModel { get; set; }
        public bool AutoAddSubmit { get; set; }


        public class FormField
        {
            public enum InputType
            {
                text,
                select,
                textarea,
                checkbox,
                hidden,
                submit
            }

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
        public DynaForm(string name, object model = null, bool autoAddSubmit = true)
        {
            Name = name;
            if (model != null)
            {
                Model = model;
                AutoPopulateModel = false;
            }
            else
            {
                Model = new ExpandoObject();
                AutoPopulateModel = true;
            }
            AutoAddSubmit = autoAddSubmit;
            //Model = model.ToExpando();
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
        public DynaForms.DynaForm AddFormField(string fieldName, string labelText = "", FormField.InputType type = FormField.InputType.text, bool required = false, bool email = false, bool isNumeric = false, int maxLength = 0, int minLength = 0, int? max = null, int? min = null, string regEx = "", Dictionary<string, string> dropDownValues = null)
        {
            var f = new FormField();
            f.FieldName = fieldName;
            f.LabelText = labelText;
            f.Type = type;
            f.Required = required;
            f.Email = email;
            f.MinLength = minLength;
            f.MaxLength = maxLength;
            f.RegEx = regEx;
            f.Min = min;
            f.Max = max;
            f.DropDownValueList = dropDownValues;
            Fields.Add(f);

            if (this.AutoPopulateModel && this.Model.GetType() == typeof(ExpandoObject))
            {
                if (this.Model == null) this.Model = new ExpandoObject();
                var d = this.Model as IDictionary<string, object>;                
                d.Add(f.FieldName, null);
            }

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
        private ValidationResult validationResult;
        public ValidationResult Validation
        {
            get { return validationResult; }
            set { validationResult = value; }
        }

        public ValidationResult Validate(object model, ValidationResult validationResult = null)
        {
            if (validationResult == null)
                validationResult = new ValidationResult();
            var modelDictionary = (IDictionary<string, object>)(model.ToExpando());

            foreach (var x in Fields)
            {
                // var property = model.GetType().GetProperty(x.FieldName);
                // if (property != null) value = (property.GetValue(model, null) ?? "").ToString();

                var value = "";

                if (modelDictionary.ContainsKey(x.FieldName))
                {
                    value = modelDictionary[x.FieldName].ToString();
                }

                if (x.Required && string.IsNullOrEmpty(value))
                {
                    validationResult.AddError(x.FieldName, "Required field", x.Label());
                }
                if (x.Email && !IsValidEmail(value))
                {
                    validationResult.AddError(x.FieldName, "Invalid email address", x.Label());
                }
                if (x.MinLength != 0 && value.ToString().Length < x.MinLength)
                {
                    validationResult.AddError(x.FieldName, "Mininum length is " + x.MinLength.ToString(), x.Label());
                }
                if (x.MaxLength != 0 && value.ToString().Length > x.MaxLength)
                {
                    validationResult.AddError(x.FieldName, "Maximum length is " + x.MaxLength.ToString(), x.Label());
                }
                if (x.RegEx != "" && !IsValidRegex(value.ToString(), x.RegEx))
                {
                    validationResult.AddError(x.FieldName, "Invalid", x.Label());
                }
            }
            validationResult.IsValid = (validationResult.Errors.Count == 0);
            this.validationResult = validationResult;
            return validationResult;
        }
        public HtmlString Html(object model = null, string action = "#", string method = "post")
        {

            var sb = new StringBuilder();
            sb.Append("<form id=\"" + Name + "\" method=\"" + method + "\" action=\"" + action + "\">\n");

            if (model == null)
            {
                if (this.Model != null)
                    model = this.Model;
                else
                    model = new ExpandoObject();                
            }

            var modelDictionary = (IDictionary<string, object>)model;

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
            if (errorMessage != "") sb.Append("<div>" + errorMessage + "</div>");

            // Create the Html tags for the form
            foreach (var h in Fields)
            {
                var labelText = h.Label();

                string value = "";
                if (model != null)
                {
                    foreach (var item in modelDictionary)
                    {
                        if (item.Key == h.FieldName)
                        {
                            object v = item.Value;
                            if (v != null)
                                value = v.ToString();
                        }
                    }
                }

                errorMessage = "";
                if (validationResult != null && !validationResult.IsValid)
                {
                    foreach (var e in validationResult.Errors)
                    {
                        if (e.Field == h.FieldName)
                        {
                            if (errorMessage != "") errorMessage += ", ";
                            errorMessage += e.Error;
                        }
                    }
                }
                if (h.Type == FormField.InputType.text)
                {
                    sb.Append(" <div class=\"labelinput\">\n");
                    sb.Append("  <label for=\"" + h.FieldName + "\">" + labelText + "</label>\n");
                    sb.Append("  <input type=\"" + h.Type + "\" id=\"" + h.FieldName + "\" name=\"" + h.FieldName + "\" value=\"" + value + "\"/>" + errorMessage + "\n");
                    sb.Append(" </div>\n");
                }
                if (h.Type == FormField.InputType.checkbox)
                {
                    sb.Append(" <div class=\"labelcheckbox\">\n");
                    sb.Append("  <label for=\"" + h.FieldName + "\">" + labelText + "</label>\n");
                    sb.Append("  <input type=\"" + h.Type + "\" id=\"" + h.FieldName + "\" name=\"" + h.FieldName + "\" value=\"" + value + "\"/>" + errorMessage + "\n");
                    sb.Append(" </div>\n");
                }

                if (h.Type == FormField.InputType.select)
                {
                    sb.Append(" <div class=\"labelselect\">\n");
                    sb.Append("  <label for=\"" + h.FieldName + "\">" + labelText + "</label>\n");
                    sb.Append("  <select id=\"" + h.FieldName + "\" name=\"" + h.FieldName + "\">\n");
                    foreach (var item in h.DropDownValueList)
                    {
                        if (item.Key != value)
                            sb.Append("  <option value=\"" + item.Key + "\">");
                        else
                            sb.Append("  <option value=\"" + item.Key + "\" selected=\"selected\">");

                        sb.Append(item.Value.ToString());
                        sb.Append("</option>\n");
                    }
                    sb.Append("  </select>\n");
                    sb.Append(" </div>\n");
                }

                if (h.Type == FormField.InputType.submit)
                {
                    sb.Append(" <div class=\"submit\">\n");
                    sb.Append("  <input type=\"" + h.Type + "\" id=\"" + h.FieldName + "\" name=\"" + h.FieldName + "\" value=\"" + labelText + "\"/>" + errorMessage + "\n");
                    sb.Append(" </div>\n");
                }
                if (h.Type == FormField.InputType.hidden)
                {
                    sb.Append(" <input type=\"" + h.Type + "\" id=\"" + h.FieldName + "\" name=\"" + h.FieldName + "\" value=\"" + value + "\"/>\n");
                }

            }

            if (this.AutoAddSubmit && !(Fields.Where(f => f.Type == FormField.InputType.submit).Any()))
            {
                sb.Append(" <input type=\"submit\" id=\"submit\" name=\"submit\"/>\n");
            }

            sb.Append("</form>\n");
            return new HtmlString(sb.ToString());
        }
        public string ClientSideScript()
        {
            var script = @"<script type=""text/javascript"">
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

            }
            jsonString += rules;
            jsonString += "}\n";

            return jsonString;
        }

        //public ValidationResult TryUpdateModel(NameValueCollection newValuesDictionary, ref dynamic model)
        //{
        //    TryUpdateModel(newValuesDictionary, model);
        //    model = (dynamic)this.Model;
        //    return validationResult;
        //}
        //public ValidationResult TryUpdateModel(ref dynamic model)
        //{
        //    TryUpdateModel(model:model);
        //    model = (dynamic)this.Model;
        //    return validationResult;
        //}

        /// <summary>
        /// Loop through each member of the model and try set the value with a given dictionary (f ex a Request.Form)
        /// </summary>
        /// <param name="newValuesDictionary"></param>
        /// <returns></returns>
        public ValidationResult TryUpdateModel(NameValueCollection newValuesDictionary=null, object model=null)
        {
            
            if (newValuesDictionary == null)
                newValuesDictionary = System.Web.HttpContext.Current.Request.Form;

            if (model == null) model = this.Model;

            var validationResult = new ValidationResult();
            IDictionary<string, object> modelDictionary;

            if (model.GetType() == typeof(ExpandoObject))
            {
                modelDictionary = (IDictionary<string, object>)model;
            }
            else
            {
                modelDictionary = model.ToDictionary();
            }

            try
            {
                foreach (var newValueKey in newValuesDictionary.AllKeys.ToArray<string>())
                {
                    if (modelDictionary.Keys.Contains(newValueKey) && (this.Fields.Count==0 || this.ContainsFieldName(newValueKey)))
                    {
                        var newValue = newValuesDictionary[newValueKey];

                        if (model.GetType() == typeof(ExpandoObject))
                        {
                            ((IDictionary<string, object>)model)[newValueKey] = newValue;
                        }
                        else
                        {
                            var p = model.GetType().GetProperty(newValueKey);
                            if (p.PropertyType == typeof(Int32))
                            {
                                Int32 setValue = 0;
                                Int32.TryParse(newValue, out setValue);
                                p.SetValue(model, setValue, null);
                            }
                            if (p.PropertyType == typeof(Int64))
                            {
                                Int64 setValue = 0;
                                Int64.TryParse(newValue, out setValue);
                                p.SetValue(model, setValue, null);
                            }
                            else if (p.PropertyType == typeof(Single))
                            {
                                Single setValue = 0;
                                Single.TryParse(newValue, out setValue);
                                p.SetValue(model, setValue, null);
                            }
                            else if (p.PropertyType == typeof(Double))
                            {
                                Double setValue = 0;
                                Double.TryParse(newValue, out setValue);
                                p.SetValue(model, setValue, null);
                            }
                            else if (p.PropertyType == typeof(Decimal))
                            {
                                Decimal setValue = 0;
                                Decimal.TryParse(newValue, out setValue);
                                p.SetValue(model, setValue, null);
                            }
                            else if (p.PropertyType == typeof(String))
                            {
                                p.SetValue(model, newValue, null);
                            }                            
                            else
                            {
                                new InvalidCastException("this type is not handled");
                            }
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