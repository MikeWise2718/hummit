using System;
using System.Collections.Generic;
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
        public void test1case(float rotrad,float trnx, float trny, float trnz,bool noise=false,float noisefak=1)
        {
            var ranman = new Random(123);
            var oaplist = new LinkedList<optimAnchorPoint>();
            var o1 = new optimAnchorPoint("pt1", new Vk3(-1, 0, 0), new Vk3(-1, 0,  0));
            var o2 = new optimAnchorPoint("pt2", new Vk3( 0, 0, 1), new Vk3(  0, 0, 1));
            var o3 = new optimAnchorPoint("pt3", new Vk3( 1, 0, 0), new Vk3( 1, 0,  0));
            oaplist.AddLast(o1);
            oaplist.AddLast(o2);
            oaplist.AddLast(o3);

            float rx=0, ry=0, rz=0;
            var op = oaplist.First;
            while (op != null)
            {
                var t1 = op.Value.target;
                var t2 = Vk3.rotateY(rotrad, t1);
                var t3 = new Vk3(t2.x+trnx, t2.y+trny, t2.z+trnz);
                if (noise)
                {
                    rx = noisefak*(float)(2 * ranman.NextDouble() - 1) / 10;
                    ry = noisefak*(float)(2 * ranman.NextDouble() - 1) / 10;
                    rz = noisefak*(float)(2 * ranman.NextDouble() - 1) / 10;
                }
                var t4 = new Vk3(t3.x+rx, t3.y+ry, t3.z+rz);
                op.Value.target = t4;
                op = op.Next;
            }


            // Add them all to the opimizer
            var oapo = new oapOptimizer(optTypeSelectorE.rotYtransXYZ);
            op = oaplist.First;
            while (op != null)
            {
                oapo.addOap(op.Value);
                op = op.Next;
            }
            oapo.verbose = false;
            oapo.optimize();

            var scavek = oapo.scavek;
            var rotvek = oapo.rotvek;
            var twopi = (float)(2 * Math.PI);
            if (rotvek.y < 0) rotvek.y += twopi;
            if (twopi < rotvek.y) rotvek.y -= twopi;
            if (rotvek.y < 0) rotvek.y += twopi; // can underflow twice
            var trnvek = oapo.trnvek;
            var rotdif = Math.Abs(rotvek.y - rotrad);
            Assert.IsTrue(oapo.status==optStatusE.converged);
            if (!noise)
            {
                Assert.IsTrue(Math.Abs(rotvek.y - rotrad) < 0.0005); // 0.028 degrees
            }
            Assert.IsTrue(Math.Abs(trnvek.x - trnx) < (noisefak * 0.1));
            Assert.IsTrue(Math.Abs(trnvek.y - trny) < (noisefak * 0.1));
            Assert.IsTrue(Math.Abs(trnvek.z - trnz) < (noisefak * 0.1));
            Console.WriteLine("  rot:" + rotrad + "  rot.y:" + rotvek.y+"  diff:"+(rotrad-rotvek.y));
            Console.WriteLine("  trnx:" + trnx + "  trn.x:" + trnvek.x);
            Console.WriteLine("  trny:" + trny + "  trn.y:" + trnvek.y);
            Console.WriteLine("  trnz:" + trnz + "  trn.z:" + trnvek.z);
            Console.WriteLine(oapo.status+" s:" + scavek + " r:" + rotvek + " t:" + trnvek + 
                              " iter:" + oapo.iter + "  fcalled:" + oapo.fcalled+" ferr:"+ oapo.ferr);
        }

        [TestMethod]
        public void testOptimClean()
        {
            var ranman = new Random(123);
            var ntest = 1000;
            for (int i = 0; i < ntest; i++)
            {
                float rot  = (float) (2*Math.PI*ranman.NextDouble());
                float trnx = (float) (20*ranman.NextDouble() - 10);
                float trny = (float) (20 *ranman.NextDouble() - 10);
                float trnz = (float) (20 *ranman.NextDouble() - 10);
                test1case(rot,trnx,trny,trnz,noise:false);
            }
        }
        [TestMethod]
        public void testOptimNoise()
        {
            var ranman = new Random(123);
            var ntest = 1000;
            for (int i = 0; i < ntest; i++)
            {
                float rot = (float)(2 * Math.PI * ranman.NextDouble());
                float trnx = (float)(20 * ranman.NextDouble() - 10);
                float trny = (float)(20 * ranman.NextDouble() - 10);
                float trnz = (float)(20 * ranman.NextDouble() - 10);
                test1case(rot, trnx, trny, trnz, noise: true);
            }
        }
        [TestMethod]
        public void testOptimNoiseFak()
        {
            var ranman = new Random(123);
            var ntest = 1000;
            for (int i = 0; i < ntest; i++)
            {
                float rot = (float)(2 * Math.PI * ranman.NextDouble());
                float trnx = (float)(20 * ranman.NextDouble() - 10);
                float trny = (float)(20 * ranman.NextDouble() - 10);
                float trnz = (float)(20 * ranman.NextDouble() - 10);
                test1case(rot, trnx, trny, trnz, noise: true,noisefak:2);
            }
        }
    }
}
