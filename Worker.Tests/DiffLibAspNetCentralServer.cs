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
    public class DiffLibAspNetCentralServer
    {
        /// <summary>
        /// Create id test
        /// </summary>
        [TestMethod]
        public void CreateIdShouldWork()
        {
            var mock = new Moq.Mock<DiffLib.ICentralServerState>();
            mock.Setup(x => x.NewId("mydata")).Returns("id1");

            DiffLib.ICentralServer ancs = new DiffLib.AspNetCentralServer("w1", mock.Object);
            var id = ancs.CreateId("w1", "mydata");

            Assert.AreEqual(id, "id1");
        }

        /// <summary>
        /// Complete id test.
        /// </summary>
        [TestMethod]
        public void CompleteIdShouldWork()
        {
            var mock = new Moq.Mock<DiffLib.ICentralServerState>();
            mock.Setup(x => x.CompleteId("id1x", "mydata")).Returns(true);

            DiffLib.ICentralServer ancs = new DiffLib.AspNetCentralServer("w1", mock.Object);
            var id = ancs.CompleteId("w1", "id1x", "mydata");

            Assert.AreEqual(id, true);
        }
        
        /// <summary>
        /// Diff test. Both arrays are the same. Result should be Equal
        /// </summary>
        [TestMethod]
        public void GetDiffShouldWorkAndReturnSameContent()
        {
            byte[] B1 = new byte[] { 0x01, 0x76, 0x1F, 0x87, 0xA1, 0x43, 0x44, 0x46, 0x45 };
            byte[] B2 = new byte[] { 0x01, 0x76, 0x1F, 0x87, 0xA1, 0x43, 0x44, 0x46, 0x45 };

            var mock = new Moq.Mock<DiffLib.ICentralServerState>();
            mock.Setup(x => x.Get("id1")).Returns(new DiffLib.IdObject()
            {
                Data1 = Convert.ToBase64String(B1),
                Data2 = Convert.ToBase64String(B2),
                Id = "id1"
            });

            DiffLib.ICentralServer ancs = new DiffLib.AspNetCentralServer("w1", mock.Object);
            var result = ancs.GetDiff("w1", "id1");

            Assert.AreEqual(result.Result, DiffLib.DiffResultEnum.Equal);
            Assert.IsTrue(result.Offsets.Count == 0, "No offsets should be created");
        }

        /// <summary>
        /// Diff test. Byte arrays have different content same length. Result should be ContentNotEqual
        /// </summary>
        [TestMethod]
        public void GetDiffShouldReturnSameSizeContentNotEqual()
        {
            byte[] B1 = new byte[] { 0x01, 0x76, 0x1F, 0x87, 0xA1, 0x43, 0x44, 0x46, 0x45 };
            byte[] B2 = new byte[] { 0x01, 0x76, 0x18, 0x87, 0xA1, 0x43, 0x44, 0x46, 0x45 };

            var mock = new Moq.Mock<DiffLib.ICentralServerState>();
            mock.Setup(x => x.Get("id1")).Returns(new DiffLib.IdObject()
            {
                Data1 = Convert.ToBase64String(B1),
                Data2 = Convert.ToBase64String(B2),
                Id = "id1"
            });

            DiffLib.ICentralServer ancs = new DiffLib.AspNetCentralServer("w1", mock.Object);
            var result = ancs.GetDiff("w1", "id1");

            Assert.AreEqual(result.Result, DiffLib.DiffResultEnum.SameSize_ContentNotEqual);
            Assert.IsTrue(result.Offsets.Count > 0, "Offsets should be created");
        }

        /// <summary>
        /// Diff test. Byte arrays have different length. Result should be NotEqualSize
        /// </summary>
        [TestMethod]
        public void GetDiffShouldReturnDifferentSize()
        {
            byte[] B1 = new byte[] { 0x01, 0x76, 0x1F, 0x87, 0xA1, 0x43, 0x44, 0x46, 0x45 };
            byte[] B2 = new byte[] { 0x01, 0x76, 0x18, 0x87, 0xA1, 0x43, 0x44, 0x46, 0x45, 0x34 };

            var mock = new Moq.Mock<DiffLib.ICentralServerState>();
            mock.Setup(x => x.Get("id1")).Returns(new DiffLib.IdObject()
            {
                Data1 = Convert.ToBase64String(B1),
                Data2 = Convert.ToBase64String(B2),
                Id = "id1"
            });

            DiffLib.ICentralServer ancs = new DiffLib.AspNetCentralServer("w1", mock.Object);
            var result = ancs.GetDiff("w1", "id1");

            Assert.AreEqual(result.Result, DiffLib.DiffResultEnum.NotEqualSize);
            Assert.IsTrue(result.Offsets.Count == 0, "No offsets should be created");
        }

        /// <summary>
        /// Diff test. Byte array have a different byte on the last position
        /// </summary>
        [TestMethod]
        public void GetDiffTestOffsetLastIndex()
        {
            byte[] B1 = new byte[] { 0x01, 0x76, 0x1F, 0x87, 0xA1, 0x43, 0x44, 0x46, 0x45 };
            byte[] B2 = new byte[] { 0x01, 0x76, 0x1F, 0x87, 0xA1, 0x43, 0x44, 0x46, 0x48 };

            var mock = new Moq.Mock<DiffLib.ICentralServerState>();
            mock.Setup(x => x.Get("id1")).Returns(new DiffLib.IdObject()
            {
                Data1 = Convert.ToBase64String(B1),
                Data2 = Convert.ToBase64String(B2),
                Id = "id1"
            });

            DiffLib.ICentralServer ancs = new DiffLib.AspNetCentralServer("w1", mock.Object);
            var result = ancs.GetDiff("w1", "id1");

            Assert.AreEqual(result.Result, DiffLib.DiffResultEnum.SameSize_ContentNotEqual);
            Assert.IsTrue(result.Offsets.Count > 0, "Offsets should be created");
        }

        /// <summary>
        /// Worker Id whitelist test.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException), "Worker is not authorized. Id: ")]
        public void CreateIdShouldFailBecauseUnauthorizedWorker()
        {
            var mock = new Moq.Mock<DiffLib.ICentralServerState>();
            mock.Setup(x => x.NewId("mydata")).Returns("id1");

            DiffLib.ICentralServer ancs = new DiffLib.AspNetCentralServer("w1", mock.Object);
            var id = ancs.CreateId("w1x", "mydata");

            Assert.AreEqual(id, "id1");
        }
    }
}
