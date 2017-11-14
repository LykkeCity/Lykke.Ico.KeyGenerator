using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Xunit;

namespace Lykke.Ico.KeyGenerator.Tests
{
    public class ProgramTests
    {
        public static IEnumerable<object[]> InvalidArgs => new List<object[]>
        {
            new object[] { new string[] {} },                                                       // count not specified 
            new object[] { new string[] { Program.numberArg, "-1" } },                              // invalid count
            new object[] { new string[] { Program.numberArg, "0" } },                               // invalid count
            new object[] { new string[] { Program.numberArg, "1", Program.publicKeysPathArg } },    // public keys path not specified for arg -p
            new object[] { new string[] { Program.numberArg, "1", Program.secretKeysPathArg } },    // secret keys path not specified for arg -s
            new object[] { new string[] { Program.numberArg, "1", Program.netArg } },               // BTC net name not specified for arg -net
        };

        [Theory]
        [MemberData(nameof(InvalidArgs))]
        public void ShouldExitWithError_IfArgsNotWellFormed(string[] args)
        {
            // Arrange

            // Act
            var retValue = Program.Main(args);

            // Assert
            Assert.NotEqual(retValue, 0);
        }

        [Fact]
        public void ShouldCreateFiles()
        {
            // Arrange
            var args = new string[] { Program.numberArg, "1" };

            // Act
            var retValue = Program.Main(args);

            // Assert
            Assert.Equal(retValue, 0);
            Assert.True(File.Exists(Program.defaultPublicKeysPath));
            Assert.True(File.Exists(Program.defaultSecretKeysPath));
        }

        [Fact]
        public void ShouldCreateFilesWithSpecifiedPaths()
        {
            // Arrange
            var pubPath = "pub.csv";
            var pvtPath = "pvt.csv";
            var args = new string[]
            {
                Program.numberArg, "1",
                Program.publicKeysPathArg, pubPath,
                Program.secretKeysPathArg, pvtPath
            };

            // Act
            var retValue = Program.Main(args);

            // Assert
            Assert.Equal(retValue, 0);
            Assert.True(File.Exists(pubPath));
            Assert.True(File.Exists(pvtPath));
        }
    }
}
