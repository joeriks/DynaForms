using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynaForms
{
    public class DynaFormTemplates
    {
        public const string TemplateInputText = @"
 <div class='labelinput'>
  <label for='{fieldName}'>{labelText}</label>
  <input type='text' id='{fieldName}' name='{fieldName}' value='{value}'/>{errorMessage}
 </div>";
        public const string TemplateTextArea = @"
 <div class='labeltextarea'>
  <label for='{fieldName}'>{labelText}</label>
  <textarea id='{fieldName}' name='{fieldName}'>{value}</textarea>{errorMessage}
 </div>";
        public const string TemplateCheckbox = @"
 <div class='labelcheckbox'>
  <label for='{fieldName}'>{labelText}</label>
  <input type='checkbox' id='{fieldName}' name='{fieldName}' {optional} value='{value}'/>{errorMessage}
 </div>";
        public const string TemplateSelect = @"
 <div class='labelselect'>
  <label for='{fieldName}'>{labelText}</label>
  <select id='{fieldName}' name='{fieldName}'>{optional}
  </select>
 </div>";
        public const string TemplateSelectOption = @"
    <option value='{key}'>{value}</option>";
        public const string TemplateSubmit = @"
 <div class='submit'>
  <input type='submit' id='{fieldName}' name='{fieldName}' value='{fieldName}'/>{errorMessage}
 </div>";
        public const string TemplateHidden = @"
  <input type='hidden' id='{fieldName}' name='{fieldName}' value='{value}'/>";

        public const string MessageRequiredField = "Required field ";
        public const string MessageInvalidEmailAddress = "Invalid email address ";
        public const string MessageMinimumLengthIs = "Minimum length is ";
        public const string MessageMaximumLengthIs = "Maximum length is ";
        public const string MessageInvalid = "Invalid ";

    }

}
