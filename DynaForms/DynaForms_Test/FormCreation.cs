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

            var assemblyFile = new System.IO.FileInfo(this.GetType().Assembly.Location);
            var form1 = assemblyFile.DirectoryName + "\\form1.htm";

            //System.IO.File.WriteAllText(@"D:\data\Dropbox\09_OTHER_PROJECTS\DynamicForms\DynamicForms\DynamicForms_Test\resources\form1.htm", f.Html().ToString());
            var expectedResult = System.IO.File.ReadAllText(form1);            
            Assert.AreEqual(expectedResult, f.Html().ToString());
        }

        [TestMethod]
        [DeploymentItem("resources//form2.htm")]
        public void FormCreationHtml_CompareToFile_Form2()
        {
            var f = new DynaForms.DynaForm("form2");
            var dropDownValues = new Dictionary<string, string>();
            dropDownValues.Add("1", "one");
            dropDownValues.Add("2", "two");
            dropDownValues.Add("3", "three");

            f.AddFormField("name")
                .AddFormField("check",type:DynaForm.FormField.InputType.checkbox)
                .AddFormField("value",type:DynaForm.FormField.InputType.textarea)
             .AddFormField("dropdown", type:DynaForm.FormField.InputType.select, dropDownValues: dropDownValues)
             .AddFormField("submit", type:DynaForm.FormField.InputType.submit);

            //System.IO.File.WriteAllText(@"D:\data\Dropbox\09_OTHER_PROJECTS\DynamicForms\DynamicForms\DynamicForms_Test\resources\form2.htm", f.Html().ToString());
            var expectedResult = System.IO.File.ReadAllText("form2.htm");
            Assert.AreEqual(expectedResult, f.Html().ToString());
        }



    }
}
