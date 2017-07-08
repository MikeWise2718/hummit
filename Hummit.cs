using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hummit
{
    public struct Vk3
    {
        //
        // Static Fields
        //
        public static Vk3 zero = new Vk3(0, 0, 0);

        public const float kEpsilon = 1E-05F;
        public Vk3( float x,float y,float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public static Vk3 rotateY(float deg,Vk3 v1)
        {
            var s = (float) Math.Sin(deg);
            var c = (float) Math.Cos(deg);
            var x =  c * v1.x + s * v1.z;
            var z = -s * v1.x + c * v1.z;
            return (new Vk3(x, v1.y, z));
        }
        //
        // Fields
        //
        public float x;
        public float y;
        public float z;
        public static Vk3 operator -(Vk3 a, Vk3 b)
        {
            Vk3 v = new Vk3( a.x-b.x, a.y-b.y, a.z-b.z );
            return (v);
        }
        public static Vk3 operator +(Vk3 a, Vk3 b)
        {
            Vk3 v = new Vk3( a.x+b.x, a.y+b.y, a.z+b.z );
            return (v);
        }
        public static Vk3 operator *(float a, Vk3 b)
        {
            Vk3 v = new Vk3( a*b.x, a*b.y, a*b.z );
            return (v);
        }
        public override string ToString()
        {

            return "("+x.ToString()+","+y.ToString()+","+z.ToString()+")";
        }
    }
    public struct Pt3
    {
        //
        // Static Fields
        //
        public const float kEpsilon = 1E-05F;
        public Pt3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        //
        // Fields
        //
        public float x;
        public float y;
        public float z;
        public static Vk3 operator-(Pt3 a, Pt3 b)
        {
            var v = new Vk3( a.x-b.x, a.y-b.y, a.z-b.z );
            return (v);
        }
        public static Pt3 operator +(Pt3 p, Vk3 v)
        {
            var q = new Pt3( p.x+v.x, p.y+v.y, p.z+v.z);
            return (q);
        }
        public static List<Pt3> getCircPathList(int n)
        {
            var lst = new List<Pt3>();
            Pt3 cen = new Pt3(0, 0, 0);
            var rad = 0.5f;
            for (int i = 0; i <= n; i++)
            {
                var ang = 2*Math.PI * i / n;
                var v = new Vk3((float)Math.Sin(ang), 0, (float)Math.Cos(ang));
                lst.Add(cen + rad * v);
            }
            return (lst);
        }
        public static List<Pt3> getNamedPathList(string setname, string name)
        {
            var lst = new List<Pt3>();
            switch (setname) {
                case "PathSetMin":
                    {
                        switch (name) {
                            case "Kitchen":
                                {
                                    lst.Add(new Pt3(0.0f, 0, 0.0f));
                                    lst.Add(new Pt3(11.0f, 0, 0.0f));
                                    lst.Add(new Pt3(11.0f, 0, 12.5f));
                                    lst.Add(new Pt3(14.2f, 0, 12.5f));
                                    lst.Add(new Pt3(14.2f, 0, 6.0f));
                                    break;
                                }
                            case "Room-1011":
                                {
                                    lst.Add(new Pt3(0.0f, 0, 0.0f));
                                    lst.Add(new Pt3(11.0f, 0, 0.0f));
                                    lst.Add(new Pt3(11.0f, 0, 1.0f));
                                    lst.Add(new Pt3(18.0f, 0, 1.0f));
                                    lst.Add(new Pt3(18.0f, 0, 4.0f));
                                    lst.Add(new Pt3(22.0f, 0, 4.0f));
                                    break;
                                }
                        }
                        break;
                    }
            }
            return (lst);
        }
    }
    public class PathMan {
        public static List<string> GetPathManSets()
        {
            var a = new List<string>();
            a.Add("PathSetCirc");
            a.Add("PathSetMin");
            return (a);
        }
        public string setname = "";
        List<string> pathnamelist = new List<string>();
        Dictionary<string, List<Pt3>> pathdict = new Dictionary<string, List<Pt3>>();
        public void setpathpoints(string setname, string pathname)
        {
            var plst = Pt3.getNamedPathList(setname,pathname);
            pathdict.Add(pathname, plst);
            pathnamelist.Add(pathname);
        }
        public PathMan(string setname,int nn=5)
        {
            this.setname = setname;
            switch (setname)
            {
                case "PathSetCirc":
                    {
                        for (int n=0; n<nn; n++)
                        {
                            var pname = "Circ-" + n.ToString();
                            var plst = Pt3.getCircPathList(n);
                            pathdict.Add(pname, plst);
                            pathnamelist.Add(pname);
                        }
                        break;
                    }
                case "PathSetMin":
                    {
                        setpathpoints(setname, "Kitchen");
                        setpathpoints(setname, "Room-1011");
                        break;
                    }
            }
        }
        public List<string> GetPathNames()
        {
            return (pathnamelist);
        }
        public List<Pt3> GetPath(string pathname)
        {
            if (pathdict.ContainsKey(pathname)) { 
               return (pathdict[pathname]);
            }
            return (null);
        }
    }

    public enum optTypeSelectorE { rotY,transXYZ,rotYtransXYZ }
    public enum optStatusE { initialized, converged, didnotconverge }


    public class optimAnchorPoint
    {
        public string name;
        public Vk3 source;
        public Vk3 target;
        public optimAnchorPoint(string name,Vk3 source,Vk3 target)
        {
            this.name = name;
            this.source = source;
            this.target = target;
        }
    }
    public class oapOptimizer
    {
        optTypeSelectorE mode;
        LinkedList<optimAnchorPoint> oaplist = null;
        public optStatusE status;
        public int iter;
        int npars;
        float [] curpars;
        float [] lstpars;

        float convergenceEps = 1e-7f;
        float stepSize = 0.01f; 
        int maxiter = 100;

        public oapOptimizer(optTypeSelectorE mode)
        {
            this.mode = mode;
            oaplist = new LinkedList<optimAnchorPoint>();
            switch(mode)
            {
                case optTypeSelectorE.rotY:
                     npars = 1;
                     break;
                case optTypeSelectorE.transXYZ:
                     npars = 3;
                     break;
                case optTypeSelectorE.rotYtransXYZ:
                     npars = 4;
                     break;
            }
            curpars = new float[npars];
            status = optStatusE.initialized;
        }
        public void addOap(optimAnchorPoint oap)
        {
            oaplist.AddLast(oap);
        }

        public Vk3 rotvek = new Vk3(0, 0, 0);
        public Vk3 trnvek = new Vk3(0, 0, 0);
        public Vk3 scavek = new Vk3(1, 1, 1);
        void unpackIntoF(float [] pars)
        {
            switch (mode)
            {
                case optTypeSelectorE.rotY:
                    rotvek.y = pars[0];
                    break;
                case optTypeSelectorE.transXYZ:
                    trnvek.x = pars[0];
                    trnvek.y = pars[1];
                    trnvek.z = pars[2];
                    break;
                case optTypeSelectorE.rotYtransXYZ:
                    rotvek.y = pars[0];
                    trnvek.x = pars[1];
                    trnvek.y = pars[2];
                    trnvek.z = pars[3];
                    break;
            }
        }
        public static Vk3 rotateY(float deg, Vk3 v1)
        {
            var s = (float)Math.Sin(deg);
            var c = (float)Math.Cos(deg);
            var x = c * v1.x + s * v1.z;
            var z = -s * v1.x + c * v1.z;
            return (new Vk3(x, v1.y, z));
        }
        void packcurpars()
        {
            switch (mode)
            {
                case optTypeSelectorE.rotY:
                    curpars[0] = rotvek.y;
                    break;
                case optTypeSelectorE.transXYZ:
                    curpars[0] = trnvek.x;
                    curpars[1] = trnvek.y;
                    curpars[2] = trnvek.z;
                    break;
                case optTypeSelectorE.rotYtransXYZ:
                    curpars[0] = rotvek.y;
                    curpars[1] = trnvek.x;
                    curpars[2] = trnvek.y;
                    curpars[3] = trnvek.z;
                    break;
            }
        }
        float F(optimAnchorPoint oap)
        {
            var v0 = oap.source;

            // scale
            var v1 = new Vk3(v0.x * scavek.x, v0.y * scavek.y, v0.z * scavek.z);
            // rotate (just y for now)
            var v2 = rotateY(rotvek.y, v1);

            // translate
            var v3 = v2 + trnvek;

            var dt = v3 - oap.target;
            float err = dt.x*dt.x + dt.y*dt.y + dt.z*dt.z;

            return (err);
        }
        float F(float [] pars)
        {
            unpackIntoF(pars); // this makes F use pars
            float sum = 0;
            var oap = oaplist.First;
            while(oap!=null)
            {
                sum = sum + F(oap.Value);
                oap = oap.Next;
            }
            return (sum);
        }
        float [] peturbcurpars(int idx,float perturbsize,int dir)
        {
            var rv = new float[npars];
            for (int i=0; i<npars; i++ )
            {
                rv[i] = curpars[i];
                if (i == idx)
                {
                    rv[i] += dir*perturbsize;
                }
            }
            return (rv);
        }
        public bool converged()
        {
            // check curpar to lstpar distance less than convergenceEps
            if (iter == 0) return (false);
            float err = 0;
            for (int i = 0; i < npars; i++)
            {
                var dlt = (curpars[i] - lstpars[i]);
                //var c = curpars[i];
                //var l = lstpars[i];
                //                System.Console.WriteLine("    i:" + i + " l:" + l + " c:" + c + " dlt:" + dlt + "  err:" + err);
                err += dlt * dlt;
            }
            err = (float) Math.Sqrt(err);
            System.Console.WriteLine("iter:" + iter + " err:" + err);
            return (err < convergenceEps);
        }
        public float [] calcGradPoint(float [] pars, float a, float [] grad)
        {
            var newpars = new float[npars];
            for (int i = 0; i < npars; i++)
            {
                newpars[i] = pars[i] - a * grad[i];
            }
            return (newpars);
        }
        public float findBestA(float [] pars,float [] grad)
        {
            int maxn = 100000;
            var maxa = 8.0f / npars;
            var lsta = maxa;
            var lstminpars = calcGradPoint(pars,lsta,grad);
            var lstminval = F(lstminpars);
            var lstn = maxn;
            for (int n = 1; n < maxn; n++)
            {
                // back up until the values start getting bigger again
                var nxta = (((float)n)/maxn)*maxa;
                var nxtminpars = calcGradPoint(pars, nxta, grad);
                var nxtminval = F(nxtminpars);
                if (nxtminval < lstminval)
                {
                    lsta = nxta;
                    lstminpars = nxtminpars;
                    lstminval = nxtminval;
                    lstn = n;
                }
            }
            Console.WriteLine("   besta:" + lsta + " lstn:"+lstn+" F:"+lstminval);
            return (lsta);
        }
        public void optimize()
        {
            curpars = new float[npars];
            lstpars = new float[npars];
            // initialize lstpars so as not to converge
           // for (int i = 0; i < npars; i++) lstpars[i] = 10 * convergenceEps;

            iter = 0;
            while (!converged() && iter<maxiter)
            {
                // calculate gradient at curpnt
                var baseval = F(curpars);
                var grad = new float[npars];
                System.Console.WriteLine("baseval:"+baseval);
                for (int i = 0; i < npars; i++)
                {                  
                    var pparsP = peturbcurpars(i, stepSize, +1);
                    var pparsM = peturbcurpars(i, stepSize, -1);
                    var valP = F(pparsP);
                    var valM = F(pparsM);
                    grad[i] = (valP - valM)/(2*stepSize);
                    var c = curpars[i];
                    var l = lstpars[i];
                    var g = grad[i];
                    System.Console.WriteLine("    i:" + i + " l:" + l + " c:" + c + "  valP:"+valP+ "  valM:" + valM + " g:" +g);
                }
                var a = findBestA(curpars, grad);
                var nxtpars = calcGradPoint(curpars, a, grad);

                lstpars = curpars;
                curpars = nxtpars;
                iter += 1;
            }
            if (iter >= 100)
            {
                status = optStatusE.didnotconverge;
                Console.WriteLine("Did not converge");
            } else
            {
                status = optStatusE.converged;
            }
            // optimized value is now in curpars
            unpackIntoF(curpars);
            Console.WriteLine("Optimied status" + status + " iter:" + iter);
        }
    }



}
