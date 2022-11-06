using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MystiickWeb.Core.Tests
{
    internal static class TestData
    {
        internal static class User
        {
            public static readonly Guid ID = new("7ADA9356-80D0-4638-AE8A-08A4C7500C01");
            public const string Username = "Test Username";
            public const string PasswordHash = "Test Password Hash";
        }
    }
}
