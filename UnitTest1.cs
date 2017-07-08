using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Hummit;

namespace HummitTest
{
    [TestClass]
    public class BaseTests
    {
        [TestMethod]
        public void TestVectors()
        {
            var v = new Vk3(1, 2, 3);
            Assert.AreEqual(v.x, 1, "x not eq 1");
            Assert.AreEqual(v.y, 2, "y not eq 2");
            Assert.AreEqual(v.z, 3, "z not eq 3");
        }
        [TestMethod]
        public void TestPoints()
        {
            var p = new Pt3(1, 2, 3);
            Assert.AreEqual(p.x, 1, "x not eq 1");
            Assert.AreEqual(p.y, 2, "y not eq 2");
            Assert.AreEqual(p.z, 3, "z not eq 3");
        }
        [TestMethod]
        public void TestOperators()
        {
            var v1 = new Vk3(1, 2, 3);
            var v2 = 2 * v1;
            Assert.AreEqual(v2.x, 2, "x not eq 2");
            Assert.AreEqual(v2.y, 4, "y not eq 4");
            Assert.AreEqual(v2.z, 6, "z not eq 6");
            var v3 = v2 - v1;
            Assert.AreEqual(v3.x, 1, "x not eq 1");
            Assert.AreEqual(v3.y, 2, "y not eq 2");
            Assert.AreEqual(v3.z, 3, "z not eq 3");
            var v4 = v2 + v1;
            Assert.AreEqual(v4.x, 3, "x not eq 3");
            Assert.AreEqual(v4.y, 6, "y not eq 6");
            Assert.AreEqual(v4.z, 9, "z not eq 9");
            var p1 = new Pt3(11, 22, 33);
            var p2 = new Pt3(1, 2, 3);
            var v = p1 - p2;
            Assert.AreEqual(v.x, 10, "x not eq 10");
            Assert.AreEqual(v.y, 20, "y not eq 20");
            Assert.AreEqual(v.z, 30, "z not eq 33");
        }
        [TestMethod]
        public void TestPathMan()
        {
            var pml = PathMan.GetPathManSets();
            Assert.AreEqual(pml[0], "PathSetCirc");
            var pm = new PathMan("PathSetCirc", 5);
            var pathlist = pm.GetPathNames();
            Assert.AreEqual(pathlist.Count, 5);
            var ppl = pm.GetPath(pathlist[4]);
            Assert.AreEqual(ppl.Count, 5);
        }
        [TestMethod]
        public void TestOptim()
        {
            var pml = PathMan.GetPathManSets();
            Assert.AreEqual(pml[0], "PathSetCirc");
            var pm = new PathMan("PathSetCirc", 5);
            var pathlist = pm.GetPathNames();
            Assert.AreEqual(pathlist.Count, 5);
            var ppl = pm.GetPath(pathlist[4]);
            Assert.AreEqual(ppl.Count, 5);
        }
        [TestMethod]
        public void testOptim()
        {
            var oapo = new oapOptimizer(optTypeSelectorE.rotYtransXYZ);
            var o1 = new optimAnchorPoint("pt1", new Vk3(-1, 0, 0), new Vk3(-1, 0,11.5f));
            var o2 = new optimAnchorPoint("pt2", new Vk3(0, 0, 1), new Vk3(0.1f, 0, 12.3f));
            var o3 = new optimAnchorPoint("pt3", new Vk3(1, 0, 0), new Vk3(1, 0, 11.4f));
            oapo.addOap(o1);
            oapo.addOap(o2);
            oapo.addOap(o3);
            oapo.optimize();

            var scavek = oapo.scavek;
            var rotvek = oapo.rotvek;
            var trnvek = oapo.trnvek;
            Console.WriteLine("s:" + scavek + " r:" + rotvek + " t:" + trnvek);
        }
    }
}
