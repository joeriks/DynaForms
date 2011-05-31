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
        public static string SaveToContour(this DynaForm dynaform, string formGuid, string userIpAddress = "", int umbracoPageId = 0)
        {
            var retval = "";

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

                        if (dynaform.ModelDictionary.ContainsKey(contourFieldName))
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

                        retval = record.Id.ToString();
                    }
                    recordService.Submit();
                    recordService.SaveFormToRecord();

                }
            }
            return retval;

        }
    }
}
