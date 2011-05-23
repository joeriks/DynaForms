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

            var someNumber = 100;
            var name = "Proj name";
            var address1 = "address";
            
            var formMock = new System.Collections.Specialized.NameValueCollection();

            formMock.Add("SomeNumber", someNumber.ToString()); 
            formMock.Add("Name", name);
            formMock.Add("Address1", address1);
            
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
        public void TryUpdateModel_CheckboxTrue()
        {
            var project = new Project();
            var check = true;

            var formMock = new System.Collections.Specialized.NameValueCollection();
            formMock.Add("Check", "on");

            var x = new DynaForms.DynaForm("formname");

            x.TryUpdateModel(formMock, project);

            var newProject = (Project)x.Model;

            Assert.AreEqual(check, newProject.Check);

        }
        [TestMethod]
        public void TryUpdateModel_CheckboxFalse()
        {
            var project = new Project();
            var check = false;

            var formMock = new System.Collections.Specialized.NameValueCollection();
            formMock.Add("Check", "");

            var x = new DynaForms.DynaForm("formname");

            x.TryUpdateModel(formMock, project);

            var newProject = (Project)x.Model;

            Assert.AreEqual(check, newProject.Check);

        }

        [TestMethod]
        public void TryUpdateModel_DateTime()
        {
            var project = new Project();

            var date = DateTime.Now;

            var formMock = new System.Collections.Specialized.NameValueCollection();

            formMock.Add("CreationDate", date.ToShortDateString());
            formMock.Add("PrintedDateTime", date.ToString());

            var x = new DynaForms.DynaForm("formname");

            x.TryUpdateModel(formMock, project);

            var newProject = (Project)x.Model;

            Assert.AreEqual(date.ToShortDateString(), newProject.CreationDate.ToShortDateString());
            Assert.AreEqual(date.ToString(), newProject.PrintedDateTime.ToString());
        }

        [TestMethod]
        public void TryUpdateModel_NullableDateTime()
        {
            var project = new Project();

            var date = DateTime.Now;

            var formMock = new System.Collections.Specialized.NameValueCollection();
            formMock.Add("PrintedDateTime", ""); // null

            var x = new DynaForms.DynaForm("formname");

            x.TryUpdateModel(formMock, project);

            var newProject = (Project)x.Model;

            Assert.AreEqual(null, newProject.PrintedDateTime);
        }

    }
}
