using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DynaForms;

namespace DynaForms_Test
{
    [TestClass]
    public class FormCreation
    {
        static string defaultOutputPath = @"C:\Users\joeriks\Documents\GitHub\DynaForms\DynaForms\DynaForms_Test\resources\";

        [TestMethod]
        public void FormCreation_NoErrors()
        {
            var project = new Project();
            var x = new DynaForms.DynaForm("formname", project);
            Assert.AreEqual(x.Name, "formname");
        }

        [TestMethod]
        [DeploymentItem("resources//CustomHtml.htm")]
        public void FormCreation_CustomHtml()
        {
            string formName = "CustomHtml";
            string fileName = formName + ".htm";

            var project = new Project();
            var form = new DynaForms.DynaForm(formName, project);

            form.AddHtml("<fieldset>\n");
            form.AddHtml("  <legend>Fieldset 1</legend>\n");
            form.AddFormField("Name");
            form.AddFormField("Address");
            form.AddHtml("</fieldset>\n");

            form.AddHtml("<fieldset>\n");
            form.AddHtml("  <legend>Fieldset 2</legend>\n");
            form.AddFormField("Phone 1");
            form.AddFormField("Phone 2");
            form.AddHtml("</fieldset>\n");

            form.AddHtml("<fieldset>\n");
            form.AddFormField("Submit", type: InputType.submit);
            form.AddHtml("</fieldset>\n");

            // Uncomment the following row to re-create output html - then look at it manually and open it in a browser
            System.IO.File.WriteAllText(defaultOutputPath + fileName, form.Html().ToString());
            // After you recreated it you need to include the file in the project and also mark it (properties on the file) for "Copy to output"

            var expectedResult = System.IO.File.ReadAllText(fileName);
            Assert.AreEqual(expectedResult, form.Html().ToString());

        }

        [TestMethod]
        [DeploymentItem("resources//Simple.htm")]
        public void FormCreation_Simple()
        {
            string formName = "Simple";
            string fileName = formName + ".htm";

            var f = new DynaForms.DynaForm(formName);
            f.AddFormField("name").AddFormField("value");

            System.IO.File.WriteAllText(defaultOutputPath + fileName, f.Html().ToString());
            var expectedResult = System.IO.File.ReadAllText(fileName);
            Assert.AreEqual(expectedResult, f.Html().ToString());
        }

        [TestMethod]
        [DeploymentItem("resources//WithDropDown.htm")]
        public void FormCreation_WithDropDown()
        {
            string formName = "WithDropDown";
            string fileName = formName + ".htm";

            var form = new DynaForms.DynaForm(formName);
            var dropDownValues = new Dictionary<string, string>();
            dropDownValues.Add("1", "one");
            dropDownValues.Add("2", "two");
            dropDownValues.Add("3", "three");

            form.AddFormField("name", cssName: "inputfield")
             .AddFormField("check", cssName: "inputfield", type: InputType.checkbox)
             .AddFormField("hidden", type: InputType.hidden, defaultValue:"value")
             .AddFormField("value", cssName: "inputfield", type: InputType.textarea)
             .AddFormField("dropdown", cssName: "inputfield", type: InputType.select, dropDownValues: dropDownValues)
             .AddFormField("submit", cssName: "inputfield", type: InputType.submit);

            System.IO.File.WriteAllText(defaultOutputPath + fileName, form.Html().ToString());
            var expectedResult = System.IO.File.ReadAllText(fileName);
            Assert.AreEqual(expectedResult, form.Html().ToString());
        }

        [TestMethod]
        [DeploymentItem("resources//SetDefaultValue.htm")]
        public void FormCreation_DefaultValueOnAutoCreatedModel()
        {
            string formName = "SetDefaultValue";
            string fileName = formName + ".htm";

            // autoCreateModelFromFields is true by default, just setting it here for explicity

            var form = new DynaForm(formName, autoCreateModelFromFields:true);
            form.AddFormField("defaultvalue1");
            form.AddFormField("defaultvalue2");
            form.AddFormField("defaultvalue3");

            // this default value will not be used in .Html() because it does not update the autocreated model
            
            form.Fields[0].DefaultValue = "default1";
            form.Fields[0].Type = InputType.hidden;

            // here are better ways to set default values in those cases

            form.UpdateFormField("defaultvalue2", defaultValue: "default2", type:InputType.hidden);
            form.UpdateFormField(form.Fields[2], defaultValue: "default3", type: InputType.hidden);

            System.IO.File.WriteAllText(defaultOutputPath + fileName, form.Html().ToString());
            var expectedResult = System.IO.File.ReadAllText(fileName);
            Assert.AreEqual(expectedResult, form.Html().ToString());

            

        }

        [TestMethod]
        [DeploymentItem("resources//WithDataObject.htm")]
        public void FormCreation_WithDataObject()
        {
            string formName = "WithDataObject";
            string fileName = formName + ".htm";

            var p = new Project();
            p.Check = true;
            p.Name = "existing";
            p.PrintedDateTime = null;
            p.CreationDate = new DateTime(2000, 10, 20);

            var form = new DynaForms.DynaForm(formName, p);
            form.AddFormField("Name")
             .AddFormField("Check", type: InputType.checkbox)
             .AddFormField("PrintedDateTime")
             .AddFormField("CreationDate");

            System.IO.File.WriteAllText(defaultOutputPath + fileName, form.Html().ToString());
            var expectedResult = System.IO.File.ReadAllText(fileName);
            Assert.AreEqual(expectedResult, form.Html().ToString());
        }

        [TestMethod]
        [DeploymentItem("resources//FormCreation_CustomBasicTemplate.htm")]
        public void FormCreation_CustomBasicTemplate()
        {
            var inputText = "{labelText} : <input type='text' {idName} />{errorMessage}<br/>\n";

            var testForm = new DynaForm("formname", autoAddSubmit: false)
                   .AddFormField("Name", template: inputText)
                   .AddFormField("Phonenumber", "Phone number", template: inputText)
                   .AddHtml("<input type='submit'/>");

            // Uncomment the following row to re-create output html - then look at it manually and open it in a browser
            System.IO.File.WriteAllText(defaultOutputPath + "FormCreation_CustomBasicTemplate.htm", testForm.Html().ToString());
            // After you recreated it you need to include the file in the project and also mark it (properties on the file) for "Copy to output"

            var expectedResult = System.IO.File.ReadAllText("FormCreation_CustomBasicTemplate.htm");
            Assert.AreEqual(expectedResult, testForm.Html().ToString());

        }

    }
}
