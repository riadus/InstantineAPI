using System;

namespace InstantineAPI.IntegrationTests.Configuration
{
    public class OrderAttribute : Attribute
    {
        public int I { get; }

        public OrderAttribute(int i)
        {
            I = i;
        }
    }
}
