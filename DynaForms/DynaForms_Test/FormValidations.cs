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

            Assert.AreEqual(false, f.Validation.IsValid);

        }
        [TestMethod]
        public void FormValidation_OneValidField_ShouldValidate()
        {
            var f = new DynaForm("form2")
                .AddFormField("name", required: true);
            var formMock = new System.Collections.Specialized.NameValueCollection();

            formMock.Add("name", "name");
            f.TryUpdateModel(formMock);

            Assert.AreEqual(true, f.Validation.IsValid);

        }

    }
}
