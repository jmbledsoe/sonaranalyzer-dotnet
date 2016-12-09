using System.Collections.Generic;

namespace Tests.Diagnostics
{
    public interface IInterface
    {
        void Method(int i = 42); // Noncompliant
//                        ^^^^
    }

    public class Base
    {
        public virtual void Method(int i = 42) // Noncompliant {{Use the overloading mechanism instead of the optional parameters.}}
        { }
    }

    public class OptionalParameter : Base
    {
        public override void Method(int i = 42) // Compliant
        {
            base.Method(i);
        }
        public OptionalParameter(int i = 0, // Compliant, default
            int j = 1) // Noncompliant
        {
        }
        public OptionalParameter()
        {
        }
        private OptionalParameter(int i = 100) // Compliant, private
        {
        }

        public void Method1(bool b = false) // Compliant
        {
        }
        public void Method2(bool b = default(bool)) // Compliant
        {
        }
        public void Method2(bool b = default(System.Boolean)) // Compliant
        {
        }

        public void Method1(double i = 0.0) // Compliant
        {
        }
        public void Method2(double i = 0.1) // Noncompliant
        {
        }
        public void Method3(double i = double.Epsilon) // Noncompliant
        {
        }
        public void Method4(double i = 0) // Noncompliant, not same type
        {
        }

        public void Method1(decimal d = decimal.Zero) // Noncompliant
        {
        }

        public void Method1(string s = null) // Compliant
        {
        }
        public void Method2(string s = "") // Noncompliant
        {
        }

        public void Method1(int? i = null) // Compliant
        {
        }
        public void Method2(int? i = 0) // Noncompliant
        {
        }
        public void Method3(int? i = default(int)) // Noncompliant
        {
        }

        public void Method1(object o = null) // Compliant
        {
        }
        public void Method2(object o = default(object)) // Compliant, default value
        {
        }
        public void Method3(object o = default(int)) // Noncompliant, different value
        {
        }
        public void Method4(object o = default(string)) // Noncompliant, same value but use the correct type
        {
        }
    }

    public class CallerMember
    {
        public void Method([System.Runtime.CompilerServices.CallerLineNumberAttribute] int line = 100) { }
    }
}
