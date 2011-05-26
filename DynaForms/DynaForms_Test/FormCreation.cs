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
            Assert.AreEqual(x.Name,"formname");
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

            System.IO.File.WriteAllText(defaultOutputPath + fileName, form.Html().ToString());
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

            form.AddFormField("name")
             .AddFormField("check",type:InputType.checkbox)
             .AddFormField("value",type:InputType.textarea)
             .AddFormField("dropdown", type:InputType.select, dropDownValues: dropDownValues)
             .AddFormField("submit", type:InputType.submit);

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
             .AddFormField("Check",type:InputType.checkbox)
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
            var inputText = "{labelText} : <input type='text' {idName} />{errorMessage}<br/>";

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
