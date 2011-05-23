using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DynaForms;

namespace DynaForms_Test
{
    [TestClass]
    public class TryUpdateModel
    {
        [TestMethod]
        public void TryUpdateModel_StringsAndInteger()
        {
            var project = new Project();

            var name = "Proj name";
            var address1 = "address";
            var someNumber = 100;

            var formMock = new System.Collections.Specialized.NameValueCollection();

            formMock.Add("Name", name);
            formMock.Add("Address1", address1);
            formMock.Add("SomeNumber", someNumber.ToString());

            var x = new DynaForms.DynaForm("formname");

            x.TryUpdateModel(formMock, project);

            var newProject = (Project)x.Model;

            Assert.AreEqual(name, newProject.Name);
            Assert.AreEqual(someNumber, newProject.SomeNumber);
            Assert.AreEqual(address1, newProject.Address1);
        }
        [TestMethod]
        public void TryUpdateModel_StringsAndInvalidInteger()
        {
            var project = new Project();

            var name = "Proj name";
            var address1 = "address";
            var someNumber = "";

            var formMock = new System.Collections.Specialized.NameValueCollection();

            formMock.Add("Name", name);
            formMock.Add("Address1", address1);
            formMock.Add("SomeNumber", someNumber);

            var x = new DynaForms.DynaForm("formname");
            x.TryUpdateModel(formMock, project);
            var newProject = (Project)x.Model;
            Assert.AreEqual(name, newProject.Name);
            Assert.AreEqual(0, newProject.SomeNumber);
            Assert.AreEqual(address1, newProject.Address1);

        }

        [TestMethod]
        public void TryUpdateModel_SubsetOfFields()
        {
            var project = new Project();

            var name = "Proj name";
            var address1 = "address";
            var someNumber = "";

            var formMock = new System.Collections.Specialized.NameValueCollection();

            formMock.Add("Name", name);
            formMock.Add("Address1", address1);
            formMock.Add("SomeNumber", someNumber);

            var x = new DynaForms.DynaForm("formname");
            x.AddFormField("Name");

            x.TryUpdateModel(formMock, project);
            var newProject = (Project)x.Model;
            Assert.AreEqual(name, newProject.Name);
            Assert.AreEqual(0, newProject.SomeNumber);
            Assert.AreEqual(null, newProject.Address1);

        }

        [TestMethod]
        public void TryUpdateModel_StringsAndInteger_Ref()
        {
            var project = new Project();

            var name = "Proj name";
            var address1 = "address";
            var someNumber = 100;

            var formMock = new System.Collections.Specialized.NameValueCollection();

            formMock.Add("Name", name);
            formMock.Add("Address1", address1);
            formMock.Add("SomeNumber", someNumber.ToString());

            var x = new DynaForms.DynaForm("formname");

            x.TryUpdateModel(formMock, project);

            var newProject = (Project)x.Model;

            Assert.AreEqual(name, newProject.Name);
            Assert.AreEqual(someNumber, newProject.SomeNumber);
            Assert.AreEqual(address1, newProject.Address1);
        }


    }
}
