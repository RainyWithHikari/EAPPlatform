using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WalkingTec.Mvvm.Core;
using EAPPlatform.Controllers;
using EAPPlatform.ViewModel.EAP.TestModelVMs;
using EAPPlatform.Model.EAP;
using EAPPlatform.DataAccess;


namespace EAPPlatform.Test
{
    [TestClass]
    public class TestModelControllerTest
    {
        private TestModelController _controller;
        private string _seed;

        public TestModelControllerTest()
        {
            _seed = Guid.NewGuid().ToString();
            _controller = MockController.CreateController<TestModelController>(new DataContext(_seed, DBTypeEnum.Memory), "user");
        }

        [TestMethod]
        public void SearchTest()
        {
            PartialViewResult rv = (PartialViewResult)_controller.Index();
            Assert.IsInstanceOfType(rv.Model, typeof(IBasePagedListVM<TopBasePoco, BaseSearcher>));
            string rv2 = _controller.Search((rv.Model as TestModelListVM).Searcher);
            Assert.IsTrue(rv2.Contains("\"Code\":200"));
        }

        [TestMethod]
        public void CreateTest()
        {
            PartialViewResult rv = (PartialViewResult)_controller.Create();
            Assert.IsInstanceOfType(rv.Model, typeof(TestModelVM));

            TestModelVM vm = rv.Model as TestModelVM;
            TestModel v = new TestModel();
			
            v.Name = "vYcaSm0RVvJvUSBJwCaKhNVFxUpeBWwXhoiiwAp18Op5GnXpjIOrtsXR6H4K9c08qlIc41M0P8VVv4lC3UnUcsGKGUFsLXxF";
            vm.Entity = v;
            _controller.Create(vm);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<TestModel>().Find(v.ID);
				
                Assert.AreEqual(data.Name, "vYcaSm0RVvJvUSBJwCaKhNVFxUpeBWwXhoiiwAp18Op5GnXpjIOrtsXR6H4K9c08qlIc41M0P8VVv4lC3UnUcsGKGUFsLXxF");
                Assert.AreEqual(data.CreateBy, "user");
                Assert.IsTrue(DateTime.Now.Subtract(data.CreateTime.Value).Seconds < 10);
            }

        }

        [TestMethod]
        public void EditTest()
        {
            TestModel v = new TestModel();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
       			
                v.Name = "vYcaSm0RVvJvUSBJwCaKhNVFxUpeBWwXhoiiwAp18Op5GnXpjIOrtsXR6H4K9c08qlIc41M0P8VVv4lC3UnUcsGKGUFsLXxF";
                context.Set<TestModel>().Add(v);
                context.SaveChanges();
            }

            PartialViewResult rv = (PartialViewResult)_controller.Edit(v.ID.ToString());
            Assert.IsInstanceOfType(rv.Model, typeof(TestModelVM));

            TestModelVM vm = rv.Model as TestModelVM;
            vm.Wtm.DC = new DataContext(_seed, DBTypeEnum.Memory);
            v = new TestModel();
            v.ID = vm.Entity.ID;
       		
            v.Name = "HmNaFrPt8AMWD7jyaO8uXbZCaioL0qR473OmpdRgJAdmO8u2AxZ96I1LM7xYCJgerlPh6UjifHRwbIS2RJdk376zFF1G";
            vm.Entity = v;
            vm.FC = new Dictionary<string, object>();
			
            vm.FC.Add("Entity.Name", "");
            _controller.Edit(vm);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<TestModel>().Find(v.ID);
 				
                Assert.AreEqual(data.Name, "HmNaFrPt8AMWD7jyaO8uXbZCaioL0qR473OmpdRgJAdmO8u2AxZ96I1LM7xYCJgerlPh6UjifHRwbIS2RJdk376zFF1G");
                Assert.AreEqual(data.UpdateBy, "user");
                Assert.IsTrue(DateTime.Now.Subtract(data.UpdateTime.Value).Seconds < 10);
            }

        }


        [TestMethod]
        public void DeleteTest()
        {
            TestModel v = new TestModel();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
        		
                v.Name = "vYcaSm0RVvJvUSBJwCaKhNVFxUpeBWwXhoiiwAp18Op5GnXpjIOrtsXR6H4K9c08qlIc41M0P8VVv4lC3UnUcsGKGUFsLXxF";
                context.Set<TestModel>().Add(v);
                context.SaveChanges();
            }

            PartialViewResult rv = (PartialViewResult)_controller.Delete(v.ID.ToString());
            Assert.IsInstanceOfType(rv.Model, typeof(TestModelVM));

            TestModelVM vm = rv.Model as TestModelVM;
            v = new TestModel();
            v.ID = vm.Entity.ID;
            vm.Entity = v;
            _controller.Delete(v.ID.ToString(),null);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<TestModel>().Find(v.ID);
                Assert.AreEqual(data, null);
          }

        }


        [TestMethod]
        public void DetailsTest()
        {
            TestModel v = new TestModel();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
				
                v.Name = "vYcaSm0RVvJvUSBJwCaKhNVFxUpeBWwXhoiiwAp18Op5GnXpjIOrtsXR6H4K9c08qlIc41M0P8VVv4lC3UnUcsGKGUFsLXxF";
                context.Set<TestModel>().Add(v);
                context.SaveChanges();
            }
            PartialViewResult rv = (PartialViewResult)_controller.Details(v.ID.ToString());
            Assert.IsInstanceOfType(rv.Model, typeof(IBaseCRUDVM<TopBasePoco>));
            Assert.AreEqual(v.ID, (rv.Model as IBaseCRUDVM<TopBasePoco>).Entity.GetID());
        }

        [TestMethod]
        public void BatchEditTest()
        {
            TestModel v1 = new TestModel();
            TestModel v2 = new TestModel();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
				
                v1.Name = "vYcaSm0RVvJvUSBJwCaKhNVFxUpeBWwXhoiiwAp18Op5GnXpjIOrtsXR6H4K9c08qlIc41M0P8VVv4lC3UnUcsGKGUFsLXxF";
                v2.Name = "HmNaFrPt8AMWD7jyaO8uXbZCaioL0qR473OmpdRgJAdmO8u2AxZ96I1LM7xYCJgerlPh6UjifHRwbIS2RJdk376zFF1G";
                context.Set<TestModel>().Add(v1);
                context.Set<TestModel>().Add(v2);
                context.SaveChanges();
            }

            PartialViewResult rv = (PartialViewResult)_controller.BatchDelete(new string[] { v1.ID.ToString(), v2.ID.ToString() });
            Assert.IsInstanceOfType(rv.Model, typeof(TestModelBatchVM));

            TestModelBatchVM vm = rv.Model as TestModelBatchVM;
            vm.Ids = new string[] { v1.ID.ToString(), v2.ID.ToString() };
            
            vm.LinkedVM.Name = "DEjYJzUXcA";
            vm.FC = new Dictionary<string, object>();
			
            vm.FC.Add("LinkedVM.Name", "");
            _controller.DoBatchEdit(vm, null);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data1 = context.Set<TestModel>().Find(v1.ID);
                var data2 = context.Set<TestModel>().Find(v2.ID);
 				
                Assert.AreEqual(data1.Name, "DEjYJzUXcA");
                Assert.AreEqual(data2.Name, "DEjYJzUXcA");
                Assert.AreEqual(data1.UpdateBy, "user");
                Assert.IsTrue(DateTime.Now.Subtract(data1.UpdateTime.Value).Seconds < 10);
                Assert.AreEqual(data2.UpdateBy, "user");
                Assert.IsTrue(DateTime.Now.Subtract(data2.UpdateTime.Value).Seconds < 10);
            }
        }


        [TestMethod]
        public void BatchDeleteTest()
        {
            TestModel v1 = new TestModel();
            TestModel v2 = new TestModel();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
				
                v1.Name = "vYcaSm0RVvJvUSBJwCaKhNVFxUpeBWwXhoiiwAp18Op5GnXpjIOrtsXR6H4K9c08qlIc41M0P8VVv4lC3UnUcsGKGUFsLXxF";
                v2.Name = "HmNaFrPt8AMWD7jyaO8uXbZCaioL0qR473OmpdRgJAdmO8u2AxZ96I1LM7xYCJgerlPh6UjifHRwbIS2RJdk376zFF1G";
                context.Set<TestModel>().Add(v1);
                context.Set<TestModel>().Add(v2);
                context.SaveChanges();
            }

            PartialViewResult rv = (PartialViewResult)_controller.BatchDelete(new string[] { v1.ID.ToString(), v2.ID.ToString() });
            Assert.IsInstanceOfType(rv.Model, typeof(TestModelBatchVM));

            TestModelBatchVM vm = rv.Model as TestModelBatchVM;
            vm.Ids = new string[] { v1.ID.ToString(), v2.ID.ToString() };
            _controller.DoBatchDelete(vm, null);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data1 = context.Set<TestModel>().Find(v1.ID);
                var data2 = context.Set<TestModel>().Find(v2.ID);
                Assert.AreEqual(data1, null);
            Assert.AreEqual(data2, null);
            }
        }

        [TestMethod]
        public void ExportTest()
        {
            PartialViewResult rv = (PartialViewResult)_controller.Index();
            Assert.IsInstanceOfType(rv.Model, typeof(IBasePagedListVM<TopBasePoco, BaseSearcher>));
            IActionResult rv2 = _controller.ExportExcel(rv.Model as TestModelListVM);
            Assert.IsTrue((rv2 as FileContentResult).FileContents.Length > 0);
        }


    }
}
