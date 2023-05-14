package sporol;

import java.math.BigInteger;
import java.util.ArrayList;
import java.util.LinkedList;
import java.util.List;
import java.util.Queue;

public class Sporol {

    static int[] f_inv_count = {1, 4, 30, 158, 757, 2830, 8774, 22188, 46879, 82880, 124124, 157668, 170854};

    public static BigInteger factorial(int n) {
        BigInteger ret = BigInteger.ONE;
        for (int i = 1; i <= n; ++i) {
            ret = ret.multiply(BigInteger.valueOf(i));
        }
        return ret;
    }

    public static BigInteger nCr(int n, int r) {
        return factorial(n).divide(factorial(r)).divide(factorial(n - r));
    }
    
    static result calculate_inner(delegate d, List<id> roots) {
        int sectors = 0;
        BigInteger nodes = BigInteger.ZERO;
        boolean[][][][] volt = new boolean[d.KSZ() + 1][d.KSZ() + 1][d.KSZ() + 1][d.KSZ() + 1];
        Queue<id> l = new LinkedList<>();
        for (id r  : roots) {
            volt[r.W][r.B][r.WF][r.BF] = true;
            l.add(r);
            sectors++;
            nodes = nodes.add(r.size());
        }
        while (!l.isEmpty()) {
            id akt = l.remove();
            for (id x : d.func(akt)) {
                if (!volt[x.W][x.B][x.WF][x.BF]) {
                    volt[x.W][x.B][x.WF][x.BF] = true;
                    l.add(x);
                    sectors++;
                    nodes = nodes.add(x.size());
                }
            }
        }
        //System.out.println(volt[3][0][6][6]?"volt":"nem");
        return new result(sectors, nodes);
    }
    
    static result calculate_full(delegate d) {
        List<id> nodes=new ArrayList();
        for (int i=0;i<=d.KSZ();i++)
            for (int j=0;j<=d.KSZ();j++)
                nodes.add(new id(0, 0, i,j));
        return calculate_inner(d,nodes);
    }

    static result calculate(delegate d) {
        List<id> nodes=new ArrayList();
        nodes.add( new id(0, 0, d.KSZ(), d.KSZ()));
        return calculate_inner(d,nodes);
    }

    static class result {

        public int sectors;
        public BigInteger nodes;

        public result(int sectors, BigInteger nodes) {
            this.sectors = sectors;
            this.nodes = nodes;
        }
    }

    static class id {

        int W, B, WF, BF;

        id(int w, int b, int wf, int bf) {
            W = w;
            B = b;
            WF = wf;
            BF = bf;
        }

        id(id x) {
            this(x.W, x.B, x.WF, x.BF);
        }

        void negate() {
            int temp = W;
            W = B;
            B = temp;
            temp = WF;
            WF = BF;
            BF = temp;
        }

        BigInteger size() {
            return nCr(24 - W, B).multiply(BigInteger.valueOf(f_inv_count[W]));
        }
    }

    static abstract class delegate {

        List<id> func(id u) {
            List<id> r = func_inner(u);
            for (id x : r) {
                x.negate();
            }
            return r;
        }

        abstract protected List<id> func_inner(id u);
        
        abstract public int KSZ();
    }
    
    static class lasker_full extends lasker {
        @Override
        public int KSZ(){
            return 12;
        }
    }
    
    static class lasker extends delegate {
        @Override
        public int KSZ(){
            return 10;
        }

        @Override
        protected List<id> func_inner(id u) {
            List<id> v = new ArrayList<>();            

            if (u.WF != 0) {
                id a=new id(u);
                id b=new id(u);
                v.add(a);
                v.add(b);
                
                a.WF--;
                a.W++;

                b.WF--;
                b.W++;
                b.B--;
            }
            if (u.W!=0) {
                id a=new id(u);
                id b=new id(u);
                v.add(a);
                v.add(b);
                
                b.B--;
            }

            List<id> r = new ArrayList<>();
            for (id it : v) {
                if (it.B + it.BF >= 3 && it.B >= 0) {
                    r.add(it);
                }
            }

            return r;
        }
    }
    
    static class lasker_filtered extends delegate {
        
        int[] t = {0, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3};
        
        @Override
        public int KSZ(){
            return 10;
        }

        @Override
        protected List<id> func_inner(id u) {
            List<id> v = new ArrayList<>();            

            if (u.WF != 0) {
                id a=new id(u);
                id b=new id(u);
                v.add(a);
                v.add(b);
                
                a.WF--;
                a.W++;

                b.WF--;
                b.W++;
                b.B--;
            }
            if (u.W!=0) {
                id a=new id(u);
                id b=new id(u);
                v.add(a);
                v.add(b);
                
                b.B--;
            }

            List<id> r = new ArrayList<>();
            for (id it : v) {
                if (it.B + it.BF >= 3 && it.B >= 0 && t[KSZ() - it.B - it.BF] <= KSZ() - it.WF && t[KSZ() - it.W - it.WF] <= KSZ() - it.BF) {
                    r.add(it);
                }
            }

            return r;
        }
    }
    
    static class morabaraba_full extends morabaraba {
        @Override
        public int KSZ(){
            return 12;
        }
    }
    
    static class morabaraba extends delegate {
        @Override
        public int KSZ(){
            return 12;
        }

        @Override
        protected List<id> func_inner(id u) {
            List<id> v = new ArrayList<>();
            v.add(new id(u));
            v.add(new id(u));

            if (u.WF != 0) {
                v.get(0).WF--;
                v.get(0).W++;

                v.get(1).WF--;
                v.get(1).W++;
                v.get(1).B--;
            } else {
                v.get(1).B--;
            }

            List<id> r = new ArrayList<>();
            for (id it : v) {
                if (it.B + it.BF >= 3 && it.B >= 0) {
                    r.add(it);
                }
            }

            return r;
        }
    }
    
    static class morabaraba_filtered extends delegate {
        @Override
        public int KSZ(){
            return 12;
        }

        int[] t = {0, 3, 5, 7, 8, 9, 11, 12, 12, 12, 12, 12, 12};

        @Override
        protected List<id> func_inner(id u) {
            List<id> v = new ArrayList<>();
            v.add(new id(u));
            v.add(new id(u));

            if (u.WF != 0) {
                v.get(0).WF--;
                v.get(0).W++;

                v.get(1).WF--;
                v.get(1).W++;
                v.get(1).B--;
            } else {
                v.get(1).B--;
            }

            List<id> r = new ArrayList<>();
            for (id it : v) {
                if (it.B + it.BF >= 3 && it.B >= 0 && t[KSZ() - it.B - it.BF] <= KSZ() - it.WF && t[KSZ() - it.W - it.WF] <= KSZ() - it.BF) {
                    r.add(it);
                }
            }

            return r;
        }
    }
    
    static class std_full extends std {
        @Override
        public int KSZ(){
            return 12;
        }
    }

    static class std extends delegate {
        @Override
        public int KSZ(){
            return 9;
        }

        @Override
        protected List<id> func_inner(id u) {
            List<id> v = new ArrayList<>();
            v.add(new id(u));
            v.add(new id(u));

            if (u.WF != 0) {
                v.get(0).WF--;
                v.get(0).W++;

                v.get(1).WF--;
                v.get(1).W++;
                v.get(1).B--;
            } else {
                v.get(1).B--;
            }

            List<id> r = new ArrayList<>();
            for (id it : v) {
                if (it.B + it.BF >= 3 && it.B >= 0) {
                    r.add(it);
                }
            }

            return r;
        }
    }

    static class std_filtered extends delegate {
        @Override
        public int KSZ(){
            return 9;
        }

        int[] t = {0, 3, 5, 7, 8, 9, 9, 9, 9, 9};

        @Override
        protected List<id> func_inner(id u) {
            List<id> v = new ArrayList<>();
            v.add(new id(u));
            v.add(new id(u));

            if (u.WF != 0) {
                v.get(0).WF--;
                v.get(0).W++;

                v.get(1).WF--;
                v.get(1).W++;
                v.get(1).B--;
            } else {
                v.get(1).B--;
            }

            List<id> r = new ArrayList<>();
            for (id it : v) {
                if (it.B + it.BF >= 3 && it.B >= 0 && t[KSZ() - it.B - it.BF] <= KSZ() - it.WF && t[KSZ() - it.W - it.WF] <= KSZ() - it.BF) {
                    r.add(it);
                }
            }

            return r;
        }
    }

    public static void main(String[] args) {
        //System.out.println(new id(0,0,0,0).size().toString());
        result big = calculate_full(new morabaraba_full());
        result small = calculate(new lasker());
        System.out.println(big.sectors + " " + big.nodes);
        System.out.println(small.sectors + " " + small.nodes);
        System.out.println((double) small.sectors / big.sectors + " " + small.nodes.doubleValue() / big.nodes.doubleValue());
    }
}
