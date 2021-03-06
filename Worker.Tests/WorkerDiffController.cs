﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class WorkerDiffController
    {
        /// <summary>
        /// Worker action method. Create id test
        /// </summary>
        [TestMethod]
        public void CreateShouldWork()
        {
            var mock = new Moq.Mock<DiffLib.ICentralEndpoint>();
            var ret = new DiffLib.Packets.CreateIdResponse() { Id = "id1" };
            mock.Setup(x => x.CreateIdAsync("mydata")).Returns(Task.Run(() => ret));

            Worker.Controllers.DiffController df = new Worker.Controllers.DiffController(mock.Object);
            var task = df.Create(new DiffLib.Packets.CreateIdWorkerRequest() { Data = "mydata" });

            var result = task.GetAwaiter().GetResult();

            Assert.AreEqual(result.Id, "id1");
        }

        /// <summary>
        /// Worker action method. Complete id test
        /// </summary>
        [TestMethod]
        public void CompleteShouldWork()
        {
            var mock = new Moq.Mock<DiffLib.ICentralEndpoint>();
            var ret = new DiffLib.Packets.CompleteIdResponse() { Id = "id1x" };
            mock.Setup(x => x.CompleteIdAsync("id1x", "mydata")).Returns(Task.Run(() => ret));

            Worker.Controllers.DiffController df = new Worker.Controllers.DiffController(mock.Object);
            var task = df.Complete("id1x", new DiffLib.Packets.CompleteIdWorkerRequest() { Data = "mydata" });

            var result = task.GetAwaiter().GetResult();
            
            Assert.AreEqual(result.Id, "id1x");
        }

        /// <summary>
        /// Worker action method. Create id test with null request object
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NullReferenceException), "data is null")]
        public void CreateShouldThrowException()
        {
            var mock = new Moq.Mock<DiffLib.ICentralEndpoint>();

            mock.Setup(x => x.CreateIdAsync("mydata")).Returns(Task.Run(() => { return default(DiffLib.Packets.CreateIdResponse); }));

            Worker.Controllers.DiffController df = new Worker.Controllers.DiffController(mock.Object);
            var task = df.Create(null);

            var result = task.GetAwaiter().GetResult();

            Assert.AreEqual(result.Id, "id1");
        }

        /// <summary>
        /// Worker action method. Complete id test with null request object
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NullReferenceException), "data is null")]
        public void CompleteShouldThrowException()
        {
            var mock = new Moq.Mock<DiffLib.ICentralEndpoint>();

            mock.Setup(x => x.CreateIdAsync("mydata")).Returns(Task.Run(() => { return default(DiffLib.Packets.CreateIdResponse); }));

            Worker.Controllers.DiffController df = new Worker.Controllers.DiffController(mock.Object);
            var task = df.Complete("id", null);

            var result = task.GetAwaiter().GetResult();

            Assert.AreEqual(result.Id, "id1");
        }
    }
}
