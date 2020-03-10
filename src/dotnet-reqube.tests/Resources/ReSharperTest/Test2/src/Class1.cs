using System;

namespace Test2
{
    public class Class1
    {
        public void func() { int a = 0, b = 0, c; var d = a + (b + c);  }

        public void Func2()
        {
            var i = 0;
            while (Test(ref i) < 50) ;
        }

        public int Test(ref int counter)
        {
            return ++counter;
        }
    }
}
