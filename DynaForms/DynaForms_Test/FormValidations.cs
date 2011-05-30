using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DynaForms;

namespace DynaForms_Test
{
    [TestClass]
    public class FormValidations
    {
        [TestMethod]
        public void FormValidation_OneMissingField_ShouldNotValidate()
        {
            var f = new DynaForm("form2")
                .AddFormField("name", required: true);
            var formMock = new System.Collections.Specialized.NameValueCollection();

            formMock.Add("name", "");
            f.TryUpdateModel(formMock);

            Assert.IsFalse(f.Validation.IsValid);

        }
        [TestMethod]
        public void FormValidation_InvalidNumericField_ShouldNotValidate()
        {
            var f = new DynaForm("form2")
                .AddFormField("number", numeric:true);
            var formMock = new System.Collections.Specialized.NameValueCollection();
            formMock.Add("number", "not number");
            f.TryUpdateModel(formMock);
            Assert.IsFalse(f.Validation.IsValid);
        }
        [TestMethod]
        public void FormValidation_Minimum_ShouldNotValidate()
        {
            var f = new DynaForm("form2")
                .AddFormField("number", numeric: true, min:4);
            var formMock = new System.Collections.Specialized.NameValueCollection();
            formMock.Add("number", "2");
            f.TryUpdateModel(formMock);
            Assert.IsFalse(f.Validation.IsValid);
        }
        [TestMethod]
        public void FormValidation_Minimum_ShouldValidate()
        {
            var f = new DynaForm("form2")
                .AddFormField("number", numeric: true, min: 4);
            var formMock = new System.Collections.Specialized.NameValueCollection();
            formMock.Add("number", "7");
            f.TryUpdateModel(formMock);
            Assert.IsFalse(f.Validation.IsValid);
        }
        [TestMethod]
        public void FormValidation_Maximum_ShouldNotValidate()
        {
            var f = new DynaForm("form2")
                .AddFormField("number", numeric: true, max: 4);
            var formMock = new System.Collections.Specialized.NameValueCollection();
            formMock.Add("number", "8");
            f.TryUpdateModel(formMock);
            Assert.IsFalse(f.Validation.IsValid);
        }
        [TestMethod]
        public void FormValidation_Maximum_ShouldValidate()
        {
            var f = new DynaForm("form2")
                .AddFormField("number", numeric: true, max: 4);
            var formMock = new System.Collections.Specialized.NameValueCollection();
            formMock.Add("number", "4");
            f.TryUpdateModel(formMock);
            Assert.IsFalse(f.Validation.IsValid);
        }
        [TestMethod]
        public void FormValidation_Email_ShouldNotValidate()
        {
            var f = new DynaForm("form")
                .AddFormField("emailaddress", email: true);
            var formMock = new System.Collections.Specialized.NameValueCollection();
            formMock.Add("emailaddress", "invalidaddress@com");
            f.TryUpdateModel(formMock);
            Assert.IsFalse(f.Validation.IsValid);
        }
        [TestMethod]
        public void FormValidation_Email_ShouldValidate()
        {
            var f = new DynaForm("form")
                .AddFormField("emailaddress", email: true);
            var formMock = new System.Collections.Specialized.NameValueCollection();
            formMock.Add("emailaddress", "invalidaddress@company.com");
            f.TryUpdateModel(formMock);
            Assert.IsTrue(f.Validation.IsValid);
        }

        [TestMethod]
        public void FormValidation_OneValidField_ShouldValidate()
        {
            var f = new DynaForm("form2")
                .AddFormField("name", required: true);
            var formMock = new System.Collections.Specialized.NameValueCollection();

            formMock.Add("name", "name");
            f.TryUpdateModel(formMock);

            Assert.IsTrue(f.Validation.IsValid);

        }

    }
}
