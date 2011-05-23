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
        [TestMethod]
        public void FormCreation_NoErrors()
        {
            var project = new Project();
            var x = new DynaForms.DynaForm("formname", project);
            Assert.AreEqual(x.Name,"formname");
        }

        [TestMethod]
        [DeploymentItem("resources//form1.htm")]
        public void FormCreationHtml_CompareToFile_Form1()
        {
            var f = new DynaForms.DynaForm("myform");
            f.AddFormField("name").AddFormField("value");

            //System.IO.File.WriteAllText(@"C:\Users\jonas\DynaForms\DynaForms\DynaForms_Test\resources\form1.htm", f.Html().ToString());
            var expectedResult = System.IO.File.ReadAllText("form1.htm");            
            Assert.AreEqual(expectedResult, f.Html().ToString());
        }

        [TestMethod]
        [DeploymentItem("resources//form4.htm")]
        public void FormCreationHtml_CompareToFile_Form2()
        {
            var f = new DynaForms.DynaForm("form4");
            var dropDownValues = new Dictionary<string, string>();
            dropDownValues.Add("1", "one");
            dropDownValues.Add("2", "two");
            dropDownValues.Add("3", "three");

            f.AddFormField("name")
             .AddFormField("check",type:DynaForm.FormField.InputType.checkbox)
             .AddFormField("value",type:DynaForm.FormField.InputType.textarea)
             .AddFormField("dropdown", type:DynaForm.FormField.InputType.select, dropDownValues: dropDownValues)
             .AddFormField("submit", type:DynaForm.FormField.InputType.submit);
            
            //System.IO.File.WriteAllText(@"C:\Users\jonas\DynaForms\DynaForms\DynaForms_Test\resources\form4.htm", f.Html().ToString());
            var expectedResult = System.IO.File.ReadAllText("form4.htm");
            Assert.AreEqual(expectedResult, f.Html().ToString());
        }

        [TestMethod]
        [DeploymentItem("resources//form5.htm")]
        public void FormCreationHtml_CompareToFile_Form5()
        {
            var p = new Project();
            p.Check = true;
            p.Name = "existing";
            p.PrintedDateTime = null;
            p.CreationDate = new DateTime(2000, 10, 20);

            var f = new DynaForms.DynaForm("myform", p);
            f.AddFormField("Name")
             .AddFormField("Check",type:DynaForm.FormField.InputType.checkbox)
             .AddFormField("PrintedDateTime")
             .AddFormField("CreationDate");

            //System.IO.File.WriteAllText(@"C:\Users\jonas\DynaForms\DynaForms\DynaForms_Test\resources\form5.htm", f.Html().ToString());
            var expectedResult = System.IO.File.ReadAllText("form5.htm");
            Assert.AreEqual(expectedResult, f.Html().ToString());
        }


    }
}
