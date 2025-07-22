using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WalkingTec.Mvvm.Core;
using EAPPlatform.Controllers;
using EAPPlatform.ViewModel.EAP.ClientLogVMs;
using EAPPlatform.Model.EAP;
using EAPPlatform.DataAccess;


namespace EAPPlatform.Test
{
    [TestClass]
    public class ClientLogControllerTest
    {
        private ClientLogController _controller;
        private string _seed;

        public ClientLogControllerTest()
        {
            _seed = Guid.NewGuid().ToString();
            _controller = MockController.CreateController<ClientLogController>(new DataContext(_seed, DBTypeEnum.Memory), "user");
        }

        [TestMethod]
        public void SearchTest()
        {
            PartialViewResult rv = (PartialViewResult)_controller.Index();
            Assert.IsInstanceOfType(rv.Model, typeof(IBasePagedListVM<TopBasePoco, BaseSearcher>));
            string rv2 = _controller.Search((rv.Model as ClientLogListVM).Searcher);
            Assert.IsTrue(rv2.Contains("\"Code\":200"));
        }

        [TestMethod]
        public void CreateTest()
        {
            PartialViewResult rv = (PartialViewResult)_controller.Create();
            Assert.IsInstanceOfType(rv.Model, typeof(ClientLogVM));

            ClientLogVM vm = rv.Model as ClientLogVM;
            ClientLog v = new ClientLog();
			
            v.DateTime = DateTime.Parse("2021-11-19 14:32:49");
            v.EQID = "Zrt3dmaP6lSnNeisYDlZ1g95KW67Pm";
            v.LogType = "X8cKsvE15B6le4S1Z7MJqPZzAJfxd4NPR3KzFXZTNBQZKL";
            v.LogLevel = "ThiJUa9rhN9gjhB53UHuNB21QjIG8rpW7C";
            v.LogContent = "rN9YXx9sfrANqLOXtlST22qXCLWUR8SoC9w94fmpBDW5LJ76YBMaot0hJLbZioIMyv5f4f6ANMRuyk2z4QsD2ZuRpqBrWCQuwWjEORu7Q2vC90VbT1xjK6itFCod9VAc6sSvYEFS7jJNCYnpfXbUXlKSWAMC1lzAEVG2jy7uekUOHnW2rqf9XpuKoolvjqXFMsgwGg24gYJ20yLE51n4h7UeejK9hLpniG8USQNCRqMylOzL2eckWWsQmMAXYQMxD40JIWkWWrcN7UdQMfe1zpePMTIoHxpCpeff1R2E5emwHoR2G7UeZTXLKScwHvOOv6SpeKrVuf7HAtIvkfHFETrjP0rn8rV4BPiwSBjhIfTHRcO50D8eI4DFpD0SfuZmX2DTI1SMT5HtEKgFUYAwuek25uQnbLzU";
            vm.Entity = v;
            _controller.Create(vm);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<ClientLog>().Find(v.ID);
				
                Assert.AreEqual(data.DateTime, DateTime.Parse("2021-11-19 14:32:49"));
                Assert.AreEqual(data.EQID, "Zrt3dmaP6lSnNeisYDlZ1g95KW67Pm");
                Assert.AreEqual(data.LogType, "X8cKsvE15B6le4S1Z7MJqPZzAJfxd4NPR3KzFXZTNBQZKL");
                Assert.AreEqual(data.LogLevel, "ThiJUa9rhN9gjhB53UHuNB21QjIG8rpW7C");
                Assert.AreEqual(data.LogContent, "rN9YXx9sfrANqLOXtlST22qXCLWUR8SoC9w94fmpBDW5LJ76YBMaot0hJLbZioIMyv5f4f6ANMRuyk2z4QsD2ZuRpqBrWCQuwWjEORu7Q2vC90VbT1xjK6itFCod9VAc6sSvYEFS7jJNCYnpfXbUXlKSWAMC1lzAEVG2jy7uekUOHnW2rqf9XpuKoolvjqXFMsgwGg24gYJ20yLE51n4h7UeejK9hLpniG8USQNCRqMylOzL2eckWWsQmMAXYQMxD40JIWkWWrcN7UdQMfe1zpePMTIoHxpCpeff1R2E5emwHoR2G7UeZTXLKScwHvOOv6SpeKrVuf7HAtIvkfHFETrjP0rn8rV4BPiwSBjhIfTHRcO50D8eI4DFpD0SfuZmX2DTI1SMT5HtEKgFUYAwuek25uQnbLzU");
            }

        }

        [TestMethod]
        public void EditTest()
        {
            ClientLog v = new ClientLog();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
       			
                v.DateTime = DateTime.Parse("2021-11-19 14:32:49");
                v.EQID = "Zrt3dmaP6lSnNeisYDlZ1g95KW67Pm";
                v.LogType = "X8cKsvE15B6le4S1Z7MJqPZzAJfxd4NPR3KzFXZTNBQZKL";
                v.LogLevel = "ThiJUa9rhN9gjhB53UHuNB21QjIG8rpW7C";
                v.LogContent = "rN9YXx9sfrANqLOXtlST22qXCLWUR8SoC9w94fmpBDW5LJ76YBMaot0hJLbZioIMyv5f4f6ANMRuyk2z4QsD2ZuRpqBrWCQuwWjEORu7Q2vC90VbT1xjK6itFCod9VAc6sSvYEFS7jJNCYnpfXbUXlKSWAMC1lzAEVG2jy7uekUOHnW2rqf9XpuKoolvjqXFMsgwGg24gYJ20yLE51n4h7UeejK9hLpniG8USQNCRqMylOzL2eckWWsQmMAXYQMxD40JIWkWWrcN7UdQMfe1zpePMTIoHxpCpeff1R2E5emwHoR2G7UeZTXLKScwHvOOv6SpeKrVuf7HAtIvkfHFETrjP0rn8rV4BPiwSBjhIfTHRcO50D8eI4DFpD0SfuZmX2DTI1SMT5HtEKgFUYAwuek25uQnbLzU";
                context.Set<ClientLog>().Add(v);
                context.SaveChanges();
            }

            PartialViewResult rv = (PartialViewResult)_controller.Edit(v.ID.ToString());
            Assert.IsInstanceOfType(rv.Model, typeof(ClientLogVM));

            ClientLogVM vm = rv.Model as ClientLogVM;
            vm.Wtm.DC = new DataContext(_seed, DBTypeEnum.Memory);
            v = new ClientLog();
            v.ID = vm.Entity.ID;
       		
            v.DateTime = DateTime.Parse("2021-04-07 14:32:49");
            v.EQID = "th2ChDreWkUu7WGLfNb5nQYvKsfXimdC2YMarRyivcvVpF7hO12nb1";
            v.LogType = "L3GOi5X6ZFgbgxCNL5Rl5M8MxyXhTX5BDDbPZiffeOoV1KKnhrUFIB9eGELTJ5LzwenPRqHU";
            v.LogLevel = "PMdxRn0IutsHvgDT9Q6GlvoGkGhygzITIRU8jPtfGMw75Lflzio6X5jL2B0IS3vRwh0aAYjzfC";
            v.LogContent = "3vSildCiwthQSROgPQ0pEr4lYubHbSZXakjuxNItAScvpfYljbbthQW6mlRdhie9cd7ZTmKLMzv0w9cmLPgwFNCRf3WyVAQNEPlwBKp9NZwy9JHzi2Nifizb7QC0QvMQfYkDbRMnersaAVlNutujMA503xqjh5Y4LhtKp71SjHvMVfrEHcRV6AMmDZOxk9loatuY1lrFyqXG3Bfmj4SHjxYSQi3d93xaLJrGLq9NbBHgpWWNtFtcXgkk7VNDND1oYTDl93Ma89eUCoAcQWW7tuUfQ5akiW9qOLdjE0wXq3PFe0INvBjK7DsZKd2fRkEZ1zxNA60ikaJJJwa7JPB9LpK8AFzZbnUPNyJEE1rhlzk4KoEnpu5DL8fpktBBhG3vnMSq1SNOcOdHcefmWarvIXIYg9BuFAvyBSJHUEYknLlP2z04Qaxm6CeSiRTxUGhfdP1tf5lfHFn8nhiBbMiWCf56uC0XH5V13hkTFdy6s11Z8VPcxaGP072IOwSkXjPTKVJa7XeBJWIb4rSUGMqrMaaUCTIzdsAHaqQJIe6jAsdqm9OAsNYZX4qDd8diYllzBY2rVqhvgFc6LBr1GolIfUqcETQuY3oBDiP6c24P5RBYxYvrfgvfq8SfRghVoiiEovtp6SAGd336CPjiOmUFENiDvHqRAWhf4PDbS1lDoegWwwu2esGib2nzZhCDaYiittsddUJGcYbZ4PVm9YhOWCZo8fCeBNGiPbtupZjzk9NDbN8BAVuOXInMSzi5DCiD8Snj1eO10p03ke7AaNBjDfZghXj0tdFqZgzTlmgoQ0Epn4GAqbfOV2SqBlI5RYgGUiK0zXvu6wMdbQ5mfzQhUJ3B6X6E2ta5Wh5FA69hfNjUCPOAuoChwHc8FZGRXK9iUAiskUj8kciPzDa6OAXoRd62CodUyfPXY23iFN7gAru50EWMgnanQzPVHDWWDutqiyYMgyh7fQ7UhEKDCP78Eqhhh2b5y9sqQBbVSWk1lKMGG7TP4XzfdLY6chstADaItyfZEMhXbibsTHTMwrM1pu3Qbk7oQEK1xwdVd0s5c4bIbZ6B1V0YpsoHFeJ0AaKbGMXjTnHM0QKm0MWvHHR9qhB3bOkS3SaKvIhN3OHRgJZLB7Y0KBV12Q9X0t61ki476sONtw1Atd1V44EDjv0fPJsCD1IREXKPsZMXrpeuXS8F73LMljJUzxLJanQ3nyguT69gm2quUJE6GlY6dbEjs6pocC2aZnHD7CiAlTSiyZtToAeukHWStlfz09iZlfikIcydIxyGweAle1pI3r2NbbVmlb94qNGpvEbWksdghcpnETsJUgHHzIjLnnKi2xkGrpUZVJ9VWGjP19vZREVLy8eydkZos30EKPCudoas29XLFwpEffJ3okwMlrRuPJx7jeJ8IUUbmbm5y5U";
            vm.Entity = v;
            vm.FC = new Dictionary<string, object>();
			
            vm.FC.Add("Entity.DateTime", "");
            vm.FC.Add("Entity.EQID", "");
            vm.FC.Add("Entity.LogType", "");
            vm.FC.Add("Entity.LogLevel", "");
            vm.FC.Add("Entity.LogContent", "");
            _controller.Edit(vm);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<ClientLog>().Find(v.ID);
 				
                Assert.AreEqual(data.DateTime, DateTime.Parse("2021-04-07 14:32:49"));
                Assert.AreEqual(data.EQID, "th2ChDreWkUu7WGLfNb5nQYvKsfXimdC2YMarRyivcvVpF7hO12nb1");
                Assert.AreEqual(data.LogType, "L3GOi5X6ZFgbgxCNL5Rl5M8MxyXhTX5BDDbPZiffeOoV1KKnhrUFIB9eGELTJ5LzwenPRqHU");
                Assert.AreEqual(data.LogLevel, "PMdxRn0IutsHvgDT9Q6GlvoGkGhygzITIRU8jPtfGMw75Lflzio6X5jL2B0IS3vRwh0aAYjzfC");
                Assert.AreEqual(data.LogContent, "3vSildCiwthQSROgPQ0pEr4lYubHbSZXakjuxNItAScvpfYljbbthQW6mlRdhie9cd7ZTmKLMzv0w9cmLPgwFNCRf3WyVAQNEPlwBKp9NZwy9JHzi2Nifizb7QC0QvMQfYkDbRMnersaAVlNutujMA503xqjh5Y4LhtKp71SjHvMVfrEHcRV6AMmDZOxk9loatuY1lrFyqXG3Bfmj4SHjxYSQi3d93xaLJrGLq9NbBHgpWWNtFtcXgkk7VNDND1oYTDl93Ma89eUCoAcQWW7tuUfQ5akiW9qOLdjE0wXq3PFe0INvBjK7DsZKd2fRkEZ1zxNA60ikaJJJwa7JPB9LpK8AFzZbnUPNyJEE1rhlzk4KoEnpu5DL8fpktBBhG3vnMSq1SNOcOdHcefmWarvIXIYg9BuFAvyBSJHUEYknLlP2z04Qaxm6CeSiRTxUGhfdP1tf5lfHFn8nhiBbMiWCf56uC0XH5V13hkTFdy6s11Z8VPcxaGP072IOwSkXjPTKVJa7XeBJWIb4rSUGMqrMaaUCTIzdsAHaqQJIe6jAsdqm9OAsNYZX4qDd8diYllzBY2rVqhvgFc6LBr1GolIfUqcETQuY3oBDiP6c24P5RBYxYvrfgvfq8SfRghVoiiEovtp6SAGd336CPjiOmUFENiDvHqRAWhf4PDbS1lDoegWwwu2esGib2nzZhCDaYiittsddUJGcYbZ4PVm9YhOWCZo8fCeBNGiPbtupZjzk9NDbN8BAVuOXInMSzi5DCiD8Snj1eO10p03ke7AaNBjDfZghXj0tdFqZgzTlmgoQ0Epn4GAqbfOV2SqBlI5RYgGUiK0zXvu6wMdbQ5mfzQhUJ3B6X6E2ta5Wh5FA69hfNjUCPOAuoChwHc8FZGRXK9iUAiskUj8kciPzDa6OAXoRd62CodUyfPXY23iFN7gAru50EWMgnanQzPVHDWWDutqiyYMgyh7fQ7UhEKDCP78Eqhhh2b5y9sqQBbVSWk1lKMGG7TP4XzfdLY6chstADaItyfZEMhXbibsTHTMwrM1pu3Qbk7oQEK1xwdVd0s5c4bIbZ6B1V0YpsoHFeJ0AaKbGMXjTnHM0QKm0MWvHHR9qhB3bOkS3SaKvIhN3OHRgJZLB7Y0KBV12Q9X0t61ki476sONtw1Atd1V44EDjv0fPJsCD1IREXKPsZMXrpeuXS8F73LMljJUzxLJanQ3nyguT69gm2quUJE6GlY6dbEjs6pocC2aZnHD7CiAlTSiyZtToAeukHWStlfz09iZlfikIcydIxyGweAle1pI3r2NbbVmlb94qNGpvEbWksdghcpnETsJUgHHzIjLnnKi2xkGrpUZVJ9VWGjP19vZREVLy8eydkZos30EKPCudoas29XLFwpEffJ3okwMlrRuPJx7jeJ8IUUbmbm5y5U");
            }

        }


        [TestMethod]
        public void DeleteTest()
        {
            ClientLog v = new ClientLog();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
        		
                v.DateTime = DateTime.Parse("2021-11-19 14:32:49");
                v.EQID = "Zrt3dmaP6lSnNeisYDlZ1g95KW67Pm";
                v.LogType = "X8cKsvE15B6le4S1Z7MJqPZzAJfxd4NPR3KzFXZTNBQZKL";
                v.LogLevel = "ThiJUa9rhN9gjhB53UHuNB21QjIG8rpW7C";
                v.LogContent = "rN9YXx9sfrANqLOXtlST22qXCLWUR8SoC9w94fmpBDW5LJ76YBMaot0hJLbZioIMyv5f4f6ANMRuyk2z4QsD2ZuRpqBrWCQuwWjEORu7Q2vC90VbT1xjK6itFCod9VAc6sSvYEFS7jJNCYnpfXbUXlKSWAMC1lzAEVG2jy7uekUOHnW2rqf9XpuKoolvjqXFMsgwGg24gYJ20yLE51n4h7UeejK9hLpniG8USQNCRqMylOzL2eckWWsQmMAXYQMxD40JIWkWWrcN7UdQMfe1zpePMTIoHxpCpeff1R2E5emwHoR2G7UeZTXLKScwHvOOv6SpeKrVuf7HAtIvkfHFETrjP0rn8rV4BPiwSBjhIfTHRcO50D8eI4DFpD0SfuZmX2DTI1SMT5HtEKgFUYAwuek25uQnbLzU";
                context.Set<ClientLog>().Add(v);
                context.SaveChanges();
            }

            PartialViewResult rv = (PartialViewResult)_controller.Delete(v.ID.ToString());
            Assert.IsInstanceOfType(rv.Model, typeof(ClientLogVM));

            ClientLogVM vm = rv.Model as ClientLogVM;
            v = new ClientLog();
            v.ID = vm.Entity.ID;
            vm.Entity = v;
            _controller.Delete(v.ID.ToString(),null);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<ClientLog>().Find(v.ID);
                Assert.AreEqual(data, null);
            }

        }


        [TestMethod]
        public void DetailsTest()
        {
            ClientLog v = new ClientLog();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
				
                v.DateTime = DateTime.Parse("2021-11-19 14:32:49");
                v.EQID = "Zrt3dmaP6lSnNeisYDlZ1g95KW67Pm";
                v.LogType = "X8cKsvE15B6le4S1Z7MJqPZzAJfxd4NPR3KzFXZTNBQZKL";
                v.LogLevel = "ThiJUa9rhN9gjhB53UHuNB21QjIG8rpW7C";
                v.LogContent = "rN9YXx9sfrANqLOXtlST22qXCLWUR8SoC9w94fmpBDW5LJ76YBMaot0hJLbZioIMyv5f4f6ANMRuyk2z4QsD2ZuRpqBrWCQuwWjEORu7Q2vC90VbT1xjK6itFCod9VAc6sSvYEFS7jJNCYnpfXbUXlKSWAMC1lzAEVG2jy7uekUOHnW2rqf9XpuKoolvjqXFMsgwGg24gYJ20yLE51n4h7UeejK9hLpniG8USQNCRqMylOzL2eckWWsQmMAXYQMxD40JIWkWWrcN7UdQMfe1zpePMTIoHxpCpeff1R2E5emwHoR2G7UeZTXLKScwHvOOv6SpeKrVuf7HAtIvkfHFETrjP0rn8rV4BPiwSBjhIfTHRcO50D8eI4DFpD0SfuZmX2DTI1SMT5HtEKgFUYAwuek25uQnbLzU";
                context.Set<ClientLog>().Add(v);
                context.SaveChanges();
            }
            PartialViewResult rv = (PartialViewResult)_controller.Details(v.ID.ToString());
            Assert.IsInstanceOfType(rv.Model, typeof(IBaseCRUDVM<TopBasePoco>));
            Assert.AreEqual(v.ID, (rv.Model as IBaseCRUDVM<TopBasePoco>).Entity.GetID());
        }

        [TestMethod]
        public void BatchEditTest()
        {
            ClientLog v1 = new ClientLog();
            ClientLog v2 = new ClientLog();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
				
                v1.DateTime = DateTime.Parse("2021-11-19 14:32:49");
                v1.EQID = "Zrt3dmaP6lSnNeisYDlZ1g95KW67Pm";
                v1.LogType = "X8cKsvE15B6le4S1Z7MJqPZzAJfxd4NPR3KzFXZTNBQZKL";
                v1.LogLevel = "ThiJUa9rhN9gjhB53UHuNB21QjIG8rpW7C";
                v1.LogContent = "rN9YXx9sfrANqLOXtlST22qXCLWUR8SoC9w94fmpBDW5LJ76YBMaot0hJLbZioIMyv5f4f6ANMRuyk2z4QsD2ZuRpqBrWCQuwWjEORu7Q2vC90VbT1xjK6itFCod9VAc6sSvYEFS7jJNCYnpfXbUXlKSWAMC1lzAEVG2jy7uekUOHnW2rqf9XpuKoolvjqXFMsgwGg24gYJ20yLE51n4h7UeejK9hLpniG8USQNCRqMylOzL2eckWWsQmMAXYQMxD40JIWkWWrcN7UdQMfe1zpePMTIoHxpCpeff1R2E5emwHoR2G7UeZTXLKScwHvOOv6SpeKrVuf7HAtIvkfHFETrjP0rn8rV4BPiwSBjhIfTHRcO50D8eI4DFpD0SfuZmX2DTI1SMT5HtEKgFUYAwuek25uQnbLzU";
                v2.DateTime = DateTime.Parse("2021-04-07 14:32:49");
                v2.EQID = "th2ChDreWkUu7WGLfNb5nQYvKsfXimdC2YMarRyivcvVpF7hO12nb1";
                v2.LogType = "L3GOi5X6ZFgbgxCNL5Rl5M8MxyXhTX5BDDbPZiffeOoV1KKnhrUFIB9eGELTJ5LzwenPRqHU";
                v2.LogLevel = "PMdxRn0IutsHvgDT9Q6GlvoGkGhygzITIRU8jPtfGMw75Lflzio6X5jL2B0IS3vRwh0aAYjzfC";
                v2.LogContent = "3vSildCiwthQSROgPQ0pEr4lYubHbSZXakjuxNItAScvpfYljbbthQW6mlRdhie9cd7ZTmKLMzv0w9cmLPgwFNCRf3WyVAQNEPlwBKp9NZwy9JHzi2Nifizb7QC0QvMQfYkDbRMnersaAVlNutujMA503xqjh5Y4LhtKp71SjHvMVfrEHcRV6AMmDZOxk9loatuY1lrFyqXG3Bfmj4SHjxYSQi3d93xaLJrGLq9NbBHgpWWNtFtcXgkk7VNDND1oYTDl93Ma89eUCoAcQWW7tuUfQ5akiW9qOLdjE0wXq3PFe0INvBjK7DsZKd2fRkEZ1zxNA60ikaJJJwa7JPB9LpK8AFzZbnUPNyJEE1rhlzk4KoEnpu5DL8fpktBBhG3vnMSq1SNOcOdHcefmWarvIXIYg9BuFAvyBSJHUEYknLlP2z04Qaxm6CeSiRTxUGhfdP1tf5lfHFn8nhiBbMiWCf56uC0XH5V13hkTFdy6s11Z8VPcxaGP072IOwSkXjPTKVJa7XeBJWIb4rSUGMqrMaaUCTIzdsAHaqQJIe6jAsdqm9OAsNYZX4qDd8diYllzBY2rVqhvgFc6LBr1GolIfUqcETQuY3oBDiP6c24P5RBYxYvrfgvfq8SfRghVoiiEovtp6SAGd336CPjiOmUFENiDvHqRAWhf4PDbS1lDoegWwwu2esGib2nzZhCDaYiittsddUJGcYbZ4PVm9YhOWCZo8fCeBNGiPbtupZjzk9NDbN8BAVuOXInMSzi5DCiD8Snj1eO10p03ke7AaNBjDfZghXj0tdFqZgzTlmgoQ0Epn4GAqbfOV2SqBlI5RYgGUiK0zXvu6wMdbQ5mfzQhUJ3B6X6E2ta5Wh5FA69hfNjUCPOAuoChwHc8FZGRXK9iUAiskUj8kciPzDa6OAXoRd62CodUyfPXY23iFN7gAru50EWMgnanQzPVHDWWDutqiyYMgyh7fQ7UhEKDCP78Eqhhh2b5y9sqQBbVSWk1lKMGG7TP4XzfdLY6chstADaItyfZEMhXbibsTHTMwrM1pu3Qbk7oQEK1xwdVd0s5c4bIbZ6B1V0YpsoHFeJ0AaKbGMXjTnHM0QKm0MWvHHR9qhB3bOkS3SaKvIhN3OHRgJZLB7Y0KBV12Q9X0t61ki476sONtw1Atd1V44EDjv0fPJsCD1IREXKPsZMXrpeuXS8F73LMljJUzxLJanQ3nyguT69gm2quUJE6GlY6dbEjs6pocC2aZnHD7CiAlTSiyZtToAeukHWStlfz09iZlfikIcydIxyGweAle1pI3r2NbbVmlb94qNGpvEbWksdghcpnETsJUgHHzIjLnnKi2xkGrpUZVJ9VWGjP19vZREVLy8eydkZos30EKPCudoas29XLFwpEffJ3okwMlrRuPJx7jeJ8IUUbmbm5y5U";
                context.Set<ClientLog>().Add(v1);
                context.Set<ClientLog>().Add(v2);
                context.SaveChanges();
            }

            PartialViewResult rv = (PartialViewResult)_controller.BatchDelete(new string[] { v1.ID.ToString(), v2.ID.ToString() });
            Assert.IsInstanceOfType(rv.Model, typeof(ClientLogBatchVM));

            ClientLogBatchVM vm = rv.Model as ClientLogBatchVM;
            vm.Ids = new string[] { v1.ID.ToString(), v2.ID.ToString() };
            
            vm.FC = new Dictionary<string, object>();
			
            _controller.DoBatchEdit(vm, null);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data1 = context.Set<ClientLog>().Find(v1.ID);
                var data2 = context.Set<ClientLog>().Find(v2.ID);
 				
            }
        }


        [TestMethod]
        public void BatchDeleteTest()
        {
            ClientLog v1 = new ClientLog();
            ClientLog v2 = new ClientLog();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
				
                v1.DateTime = DateTime.Parse("2021-11-19 14:32:49");
                v1.EQID = "Zrt3dmaP6lSnNeisYDlZ1g95KW67Pm";
                v1.LogType = "X8cKsvE15B6le4S1Z7MJqPZzAJfxd4NPR3KzFXZTNBQZKL";
                v1.LogLevel = "ThiJUa9rhN9gjhB53UHuNB21QjIG8rpW7C";
                v1.LogContent = "rN9YXx9sfrANqLOXtlST22qXCLWUR8SoC9w94fmpBDW5LJ76YBMaot0hJLbZioIMyv5f4f6ANMRuyk2z4QsD2ZuRpqBrWCQuwWjEORu7Q2vC90VbT1xjK6itFCod9VAc6sSvYEFS7jJNCYnpfXbUXlKSWAMC1lzAEVG2jy7uekUOHnW2rqf9XpuKoolvjqXFMsgwGg24gYJ20yLE51n4h7UeejK9hLpniG8USQNCRqMylOzL2eckWWsQmMAXYQMxD40JIWkWWrcN7UdQMfe1zpePMTIoHxpCpeff1R2E5emwHoR2G7UeZTXLKScwHvOOv6SpeKrVuf7HAtIvkfHFETrjP0rn8rV4BPiwSBjhIfTHRcO50D8eI4DFpD0SfuZmX2DTI1SMT5HtEKgFUYAwuek25uQnbLzU";
                v2.DateTime = DateTime.Parse("2021-04-07 14:32:49");
                v2.EQID = "th2ChDreWkUu7WGLfNb5nQYvKsfXimdC2YMarRyivcvVpF7hO12nb1";
                v2.LogType = "L3GOi5X6ZFgbgxCNL5Rl5M8MxyXhTX5BDDbPZiffeOoV1KKnhrUFIB9eGELTJ5LzwenPRqHU";
                v2.LogLevel = "PMdxRn0IutsHvgDT9Q6GlvoGkGhygzITIRU8jPtfGMw75Lflzio6X5jL2B0IS3vRwh0aAYjzfC";
                v2.LogContent = "3vSildCiwthQSROgPQ0pEr4lYubHbSZXakjuxNItAScvpfYljbbthQW6mlRdhie9cd7ZTmKLMzv0w9cmLPgwFNCRf3WyVAQNEPlwBKp9NZwy9JHzi2Nifizb7QC0QvMQfYkDbRMnersaAVlNutujMA503xqjh5Y4LhtKp71SjHvMVfrEHcRV6AMmDZOxk9loatuY1lrFyqXG3Bfmj4SHjxYSQi3d93xaLJrGLq9NbBHgpWWNtFtcXgkk7VNDND1oYTDl93Ma89eUCoAcQWW7tuUfQ5akiW9qOLdjE0wXq3PFe0INvBjK7DsZKd2fRkEZ1zxNA60ikaJJJwa7JPB9LpK8AFzZbnUPNyJEE1rhlzk4KoEnpu5DL8fpktBBhG3vnMSq1SNOcOdHcefmWarvIXIYg9BuFAvyBSJHUEYknLlP2z04Qaxm6CeSiRTxUGhfdP1tf5lfHFn8nhiBbMiWCf56uC0XH5V13hkTFdy6s11Z8VPcxaGP072IOwSkXjPTKVJa7XeBJWIb4rSUGMqrMaaUCTIzdsAHaqQJIe6jAsdqm9OAsNYZX4qDd8diYllzBY2rVqhvgFc6LBr1GolIfUqcETQuY3oBDiP6c24P5RBYxYvrfgvfq8SfRghVoiiEovtp6SAGd336CPjiOmUFENiDvHqRAWhf4PDbS1lDoegWwwu2esGib2nzZhCDaYiittsddUJGcYbZ4PVm9YhOWCZo8fCeBNGiPbtupZjzk9NDbN8BAVuOXInMSzi5DCiD8Snj1eO10p03ke7AaNBjDfZghXj0tdFqZgzTlmgoQ0Epn4GAqbfOV2SqBlI5RYgGUiK0zXvu6wMdbQ5mfzQhUJ3B6X6E2ta5Wh5FA69hfNjUCPOAuoChwHc8FZGRXK9iUAiskUj8kciPzDa6OAXoRd62CodUyfPXY23iFN7gAru50EWMgnanQzPVHDWWDutqiyYMgyh7fQ7UhEKDCP78Eqhhh2b5y9sqQBbVSWk1lKMGG7TP4XzfdLY6chstADaItyfZEMhXbibsTHTMwrM1pu3Qbk7oQEK1xwdVd0s5c4bIbZ6B1V0YpsoHFeJ0AaKbGMXjTnHM0QKm0MWvHHR9qhB3bOkS3SaKvIhN3OHRgJZLB7Y0KBV12Q9X0t61ki476sONtw1Atd1V44EDjv0fPJsCD1IREXKPsZMXrpeuXS8F73LMljJUzxLJanQ3nyguT69gm2quUJE6GlY6dbEjs6pocC2aZnHD7CiAlTSiyZtToAeukHWStlfz09iZlfikIcydIxyGweAle1pI3r2NbbVmlb94qNGpvEbWksdghcpnETsJUgHHzIjLnnKi2xkGrpUZVJ9VWGjP19vZREVLy8eydkZos30EKPCudoas29XLFwpEffJ3okwMlrRuPJx7jeJ8IUUbmbm5y5U";
                context.Set<ClientLog>().Add(v1);
                context.Set<ClientLog>().Add(v2);
                context.SaveChanges();
            }

            PartialViewResult rv = (PartialViewResult)_controller.BatchDelete(new string[] { v1.ID.ToString(), v2.ID.ToString() });
            Assert.IsInstanceOfType(rv.Model, typeof(ClientLogBatchVM));

            ClientLogBatchVM vm = rv.Model as ClientLogBatchVM;
            vm.Ids = new string[] { v1.ID.ToString(), v2.ID.ToString() };
            _controller.DoBatchDelete(vm, null);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data1 = context.Set<ClientLog>().Find(v1.ID);
                var data2 = context.Set<ClientLog>().Find(v2.ID);
                Assert.AreEqual(data1, null);
            Assert.AreEqual(data2, null);
            }
        }


    }
}
