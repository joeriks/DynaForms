using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynaForms
{
    public class DynaFormTemplates
    {
        public const string TemplateIdName = @"id='{fieldName}' name='{fieldName}'";

        public const string TemplateInputText = @"
 <div class='labelinput'>
  <label for='{fieldName}'>{labelText}</label>
  <input type='text' {idName} value='{value}'/>{errorMessage}
 </div>";
        public const string TemplateTextArea = @"
 <div class='labeltextarea'>
  <label for='{fieldName}'>{labelText}</label>
  <textarea {idName}>{value}</textarea>{errorMessage}
 </div>";
        public const string TemplateCheckbox = @"
 <div class='labelcheckbox'>
  <label for='{fieldName}'>{labelText}</label>
  <input type='checkbox' {idName} {optional} value='{value}'/>{errorMessage}
 </div>";
        public const string TemplateSelect = @"
 <div class='labelselect'>
  <label for='{fieldName}'>{labelText}</label>
  <select {idName}>{optional}
  </select>
 </div>";
        public const string TemplateSelectOption = @"
    <option value='{key}'>{value}</option>";
        public const string TemplateSubmit = @"
 <div class='submit'>
  <input type='submit' {idName} value='{fieldName}'/>{errorMessage}
 </div>";
        public const string TemplateHidden = @"
  <input type='hidden' {idName} value='{value}'/>";

        public const string MessageRequiredField = "Required field ";
        public const string MessageInvalidEmailAddress = "Invalid email address ";
        public const string MessageMinimumLengthIs = "Minimum length is ";
        public const string MessageMaximumLengthIs = "Maximum length is ";
        public const string MessageInvalid = "Invalid ";
        public const string MessageNumeric= "Numeric value required ";
        public const string MessageMinValueIs = "Minimum value is ";
        public const string MessageMaxValueIs = "Maximum value is ";

    }

}
