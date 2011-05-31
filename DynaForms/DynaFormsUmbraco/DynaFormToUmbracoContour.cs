using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Services;
using Umbraco.Forms.Data.Storage;
using DynaForms;

namespace DynaFormsExtensionMethods
{
    public static class DynaFormToUmbracoContour
    {
        public static string AddFieldsFromContour(this DynaForm dynaform, string formGuid)
        {
            var retval = "";
            using (var recordStorage = new RecordStorage())
            using (var formStorage = new FormStorage())
            {
                var form = formStorage.GetForm(new Guid(formGuid));
                foreach (var field in form.AllFields.OrderBy(f => f.SortOrder))
                {
                    var inputType = InputType.text;
                    var fieldName = "f" + field.Id.ToString().Replace('-', '_');
                    var defaultValue = "";
                    if (field.Values.Count > 0)
                        defaultValue = field.Values[0].ToString();
                    var cssName = "";
                    if (field.Mandatory)
                        cssName = "required";

                    if (field.FieldType.ToString() == "Umbraco.Forms.Core.Providers.FieldTypes.DropDownList")
                    {
                        dynaform.AddFormField(fieldName, field.Caption, type: InputType.select, defaultValue: defaultValue, cssName: cssName);
                    }
                    else if (field.FieldType.ToString() == "Umbraco.Forms.Core.Providers.FieldTypes.Hidden")
                    {
                        dynaform.AddFormField(fieldName, field.Caption, type: InputType.hidden, defaultValue: defaultValue, cssName: cssName);
                    }
                    else
                    {
                        dynaform.AddFormField(fieldName, field.Caption, type: InputType.text, defaultValue: defaultValue, required: field.Mandatory, errorMessage: field.RequiredErrorMessage, cssName: cssName);
                    }
                    
                }
            }
            return retval;
        }

        public static string SaveToContour(this DynaForm dynaform, string formGuid, string userIpAddress = "", int umbracoPageId = 0)
        {
            var ignoreId = "";
            return SaveToContour(dynaform, formGuid, out ignoreId, userIpAddress, umbracoPageId);
        }
        public static string SaveToContour(this DynaForm dynaform, string formGuid, out string insertedRecordId, string userIpAddress = "", int umbracoPageId = 0)
        {
            var message = "";
            insertedRecordId = "";

            using (var recordStorage = new RecordStorage())
            using (var formStorage = new FormStorage())
            {
                var form = formStorage.GetForm(new Guid(formGuid));

                using (var recordService = new RecordService(form))
                {
                    recordService.Open();

                    var record = recordService.Record;
                    record.IP = userIpAddress;
                    record.UmbracoPageId = umbracoPageId;
                    recordStorage.InsertRecord(record, form);

                    foreach (var field in recordService.Form.AllFields)
                    {

                        string currentFieldValue = "";
                        string contourFieldName = field.Caption.TrimStart('#');

                        if (dynaform.ModelDictionary.ContainsKey("f" + field.Id.ToString().Replace('-', '_')))
                        {
                            currentFieldValue = dynaform.ModelDictionary["f" + field.Id.ToString().Replace('-', '_')].ToString();
                        }
                        else if (dynaform.ModelDictionary.ContainsKey(contourFieldName))
                        {
                            currentFieldValue = dynaform.ModelDictionary[contourFieldName].ToString();
                        }

                        var key = Guid.NewGuid();
                        var recordField = new RecordField
                        {
                            Field = field,
                            Key = key,
                            Record = record.Id,
                            Values = new List<object> { currentFieldValue }
                        };

                        using (var recordFieldStorage = new RecordFieldStorage())
                            recordFieldStorage.InsertRecordField(recordField);

                        record.RecordFields.Add(key, recordField);

                        insertedRecordId = record.Id.ToString();
                    }
                    recordService.Submit();
                    recordService.SaveFormToRecord();
                    

                }
                message=form.MessageOnSubmit;
            }
            return message;

        }
    }
}
