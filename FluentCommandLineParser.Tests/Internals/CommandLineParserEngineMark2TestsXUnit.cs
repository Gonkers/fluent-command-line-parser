﻿#region License
// CommandLineParserEngineMark2TestsXUnit.cs
// Copyright (c) 2013, Simon Williams
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provide
// d that the following conditions are met:
// 
// Redistributions of source code must retain the above copyright notice, this list of conditions and the
// following disclaimer.
// 
// Redistributions in binary form must reproduce the above copyright notice, this list of conditions and
// the following disclaimer in the documentation and/or other materials provided with the distribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED 
// WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A 
// PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR
// ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
// TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
// POSSIBILITY OF SUCH DAMAGE.
#endregion

using System.Linq;
using Fclp.Internals;
using Fclp.Tests.TestContext;
using FluentCommandLineParser.Tests;
using Machine.Specifications;
using Xunit;
using Xunit.Extensions;

namespace Fclp.Tests.Internals
{
    public class SingleOptionInlineDataAttribute : InlineDataAttribute
    {
        public SingleOptionInlineDataAttribute(
            string arguments,
            string expectedKeyChar,
            string expectedKey,
            string expectedValue) 
            : base(string.Format(arguments, expectedValue.WrapInDoubleQuotes()), expectedKeyChar, expectedKey, expectedValue)
        {
        }
    }

    public class DoubleOptionInlineDataAttribute : InlineDataAttribute
    {
        public DoubleOptionInlineDataAttribute(
            string arguments,
            string firstExpectedKeyChar,
            string firstExpectedKey,
            string firstExpectedValue,
            string secondExpectedKeyChar,
            string secondExpectedKey,
            string secondExpectedValue)
            : base(string.Format(arguments, firstExpectedValue.WrapInDoubleQuotes(), secondExpectedValue.WrapInDoubleQuotes()), 
            firstExpectedKeyChar, 
            firstExpectedKey, 
            firstExpectedValue, 
            secondExpectedKeyChar, 
            secondExpectedKey, 
            secondExpectedValue)
        {
        }
    }

    public class CommandLineParserEngineMark2TestsXUnit: TestContextBase<CommandLineParserEngineMark2>
    {
        [Theory]
        [SingleOptionInlineData("-f", "-", "f", null)]
        [SingleOptionInlineData("/f", "/", "f", null)]
        [SingleOptionInlineData("--f", "--", "f", null)]
        [SingleOptionInlineData("-f apple", "-", "f", "apple")]
        [SingleOptionInlineData("/f apple", "/", "f", "apple")]
        [SingleOptionInlineData("--f apple", "--", "f", "apple")]
        [SingleOptionInlineData("-fruit", "-", "fruit", null)]
        [SingleOptionInlineData("/fruit", "/", "fruit", null)]
        [SingleOptionInlineData("--fruit", "--", "fruit", null)]
        [SingleOptionInlineData("/fruit apple", "/", "fruit", "apple")]
        [SingleOptionInlineData("--fruit apple", "--", "fruit", "apple")]
        [SingleOptionInlineData("-fruit apple", "-", "fruit", "apple")]
        [SingleOptionInlineData("/fruit:apple", "/", "fruit", "apple")]
        [SingleOptionInlineData("--fruit:apple", "--", "fruit", "apple")]
        [SingleOptionInlineData("-fruit:apple", "-", "fruit", "apple")]
        [SingleOptionInlineData("/fruit=apple", "/", "fruit", "apple")]
        [SingleOptionInlineData("--fruit=apple", "--", "fruit", "apple")]
        [SingleOptionInlineData("-fruit=apple", "-", "fruit", "apple")]
        [SingleOptionInlineData("/fruit {0}", "/", "fruit", "apple pear plum")]
        [SingleOptionInlineData("--fruit {0}", "--", "fruit", "apple pear plum")]
        [SingleOptionInlineData("-fruit {0}", "-", "fruit", "apple pear plum")]
        [SingleOptionInlineData("/fruit:{0}", "/", "fruit", "apple pear plum")]
        [SingleOptionInlineData("--fruit:{0}", "--", "fruit", "apple pear plum")]
        [SingleOptionInlineData("-fruit:{0}", "-", "fruit", "apple pear plum")]
        [SingleOptionInlineData("/fruit={0}", "/", "fruit", "apple pear plum")]
        [SingleOptionInlineData("--fruit={0}", "--", "fruit", "apple pear plum")]
        [SingleOptionInlineData("-fruit={0}", "-", "fruit", "apple pear plum")]
        public void should_parse_single_options_correctly(
            string arguments,
            string expectedKeyChar,
            string expectedKey,
            string expectedValue)
        {
            var convertedArgs = TestHelpers.ParseArguments(arguments);

            InitialiseFixture();
            CreatSut();

            var result = sut.Parse(convertedArgs);

            result.ParsedOptions.Count().ShouldEqual(1);
            result.AdditionalValues.ShouldBeEmpty();

            var actualParsedOption = result.ParsedOptions.First();

            actualParsedOption.Key.ShouldEqual(expectedKey);
            actualParsedOption.Value.ShouldEqual(expectedValue);
            actualParsedOption.KeyChar.ShouldEqual(expectedKeyChar);
        }

        [Theory]
        [DoubleOptionInlineData("-f -v", "-", "f", null, "-", "v", null)]
        [DoubleOptionInlineData("/f /v", "/", "f", null, "/", "v", null)]
        [DoubleOptionInlineData("--f --v", "--", "f", null, "--", "v", null)]
        [DoubleOptionInlineData("-f apple -v onion", "-", "f", "apple", "-", "v", "onion")]
        [DoubleOptionInlineData("/f apple /v onion", "/", "f", "apple", "/", "v", "onion")]
        [DoubleOptionInlineData("--f apple --v onion", "--", "f", "apple", "--", "v", "onion")]
        [DoubleOptionInlineData("-fruit -vegetable", "-", "fruit", null, "-", "vegetable", null)]
        [DoubleOptionInlineData("/fruit /vegetable", "/", "fruit", null, "/", "vegetable", null)]
        [DoubleOptionInlineData("--fruit --vegetable", "--", "fruit", null, "--", "vegetable", null)]
        [DoubleOptionInlineData("/fruit apple /vegetable onion", "/", "fruit", "apple", "/", "vegetable", "onion")]
        [DoubleOptionInlineData("--fruit apple --vegetable onion", "--", "fruit", "apple", "--", "vegetable", "onion")]
        [DoubleOptionInlineData("-fruit apple -vegetable onion", "-", "fruit", "apple", "-", "vegetable", "onion")]
        [DoubleOptionInlineData("/fruit:apple /vegetable:onion", "/", "fruit", "apple", "/", "vegetable", "onion")]
        [DoubleOptionInlineData("--fruit:apple --vegetable:onion", "--", "fruit", "apple", "--", "vegetable", "onion")]
        [DoubleOptionInlineData("-fruit:apple -vegetable: onion", "-", "fruit", "apple", "-", "vegetable", "onion")]
        [DoubleOptionInlineData("/fruit=apple /vegetable=onion", "/", "fruit", "apple", "/", "vegetable", "onion")]
        [DoubleOptionInlineData("--fruit=apple --vegetable=onion", "--", "fruit", "apple", "--", "vegetable", "onion")]
        [DoubleOptionInlineData("-fruit=apple -vegetable=onion", "-", "fruit", "apple", "-", "vegetable", "onion")]
        [DoubleOptionInlineData("/fruit {0} /vegetable {1}", "/", "fruit", "apple pear plum", "/", "vegetable", "onion carrot peas")]
        [DoubleOptionInlineData("--fruit {0} --vegetable {1}", "--", "fruit", "apple pear plum", "--", "vegetable", "onion carrot peas")]
        [DoubleOptionInlineData("-fruit {0} -vegetable {1}", "-", "fruit", "apple pear plum", "-", "vegetable", "onion carrot peas")]
        [DoubleOptionInlineData("/fruit:{0} /vegetable:{1}", "/", "fruit", "apple pear plum", "/", "vegetable", "onion carrot peas")]
        [DoubleOptionInlineData("--fruit:{0} --vegetable:{1}", "--", "fruit", "apple pear plum", "--", "vegetable", "onion carrot peas")]
        [DoubleOptionInlineData("-fruit:{0} -vegetable:{1}", "-", "fruit", "apple pear plum", "-", "vegetable", "onion carrot peas")]
        [DoubleOptionInlineData("/fruit={0} /vegetable={1}", "/", "fruit", "apple pear plum", "/", "vegetable", "onion carrot peas")]
        [DoubleOptionInlineData("--fruit={0} --vegetable={1}", "--", "fruit", "apple pear plum", "--", "vegetable", "onion carrot peas")]
        [DoubleOptionInlineData("-fruit={0} -vegetable={1}", "-", "fruit", "apple pear plum", "-", "vegetable", "onion carrot peas")]
        public void should_parse_double_options_correctly(
            string arguments,
            string firstExpectedKeyChar,
            string firstExpectedKey,
            string firstExpectedValue,
            string secondExpectedKeyChar,
            string secondExpectedKey,
            string secondExpectedValue)
        {
            var convertedArgs = TestHelpers.ParseArguments(arguments);

            InitialiseFixture();
            CreatSut();

            var result = sut.Parse(convertedArgs);

            result.ParsedOptions.Count().ShouldEqual(2);
            result.AdditionalValues.ShouldBeEmpty();

            var first = result.ParsedOptions.First();

            first.Key.ShouldEqual(firstExpectedKey);
            first.Value.ShouldEqual(firstExpectedValue);
            first.KeyChar.ShouldEqual(firstExpectedKeyChar);

            var second = result.ParsedOptions.ElementAt(1);

            second.Key.ShouldEqual(secondExpectedKey);
            second.Value.ShouldEqual(secondExpectedValue);
            second.KeyChar.ShouldEqual(secondExpectedKeyChar);
        }

        [Fact]
        public void DummyFactToGetTheoryToExecuteInNCrunch()
        {

        }
    }
}