using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DynaForms;

namespace DynamicForms_Test
{
    [TestClass]
    public class FormCreationWithHtml
    {
        [TestMethod]
        public void CreateFormWithInputText()
        {
            var inputText = "{labelText} : <input type='text' {idName} />{errorMessage}<br/>";

            var testForm = new DynaForm("formname", autoAddSubmit: false)
                   .AddFormField("Name", template: inputText)
                   .AddFormField("Phonenumber", "Phone number", template: inputText)
                   .AddHtml("<input type='submit'/>");

            var result = testForm.Html();
            Assert.AreEqual("", result);

        }
    }
}
