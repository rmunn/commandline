using System;
using Xunit;
using FluentAssertions;
using CommandLine.Core;
using CommandLine.Tests.Fakes;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace CommandLine.Tests.Unit
{
    public class AutoHelpTests
    {
        public AutoHelpTests()
        {
        }

        public static object[][] ValidArgsData = new []
        {
            new [] { "--help" },
            new [] { "--help", "256" },
            new [] { "--help", "--stringvalue", "foo" },
            new [] { "--help", "--stringvalue=foo" },
            new [] { "--stringvalue", "foo", "--help" },
            new [] { "--stringvalue=foo", "--help" },
            new [] { "--help", "--stringvalue", "foo", "-x" },
            new [] { "--help", "--stringvalue=foo", "-x" },
            new [] { "--stringvalue", "foo", "--help", "-x" },
            new [] { "--stringvalue=foo", "--help", "-x" },
            new [] { "--stringvalue", "foo", "-x", "256", "--help" },
            new [] { "--stringvalue=foo", "-x", "256", "--help" },
            new [] { "--stringvalue", "foo", "--help", "-x", "256" },
            new [] { "--stringvalue=foo", "--help", "-x", "256" },
            new [] { "--help", "--stringvalue", "foo", "-x", "256" },
            new [] { "--help", "--stringvalue=foo", "-x", "256" },
        };

        public static object[][] InvalidArgsData = new []
        {
            new [] { "--help", "foo" },
            new [] { "--help", "-s" },
            new [] { "--help", "-i", "foo" },
            new [] {"--help", "--invalid-switch", "foo" },
            new [] {"--invalid-switch", "foo", "--help" },
            new [] {"--invalid-switch", "foo" },
        };

        public static object[][] ConsumedDashDashHelpValidArgsData = new []
        {
            new [] { "--stringvalue", "--help" },
            new [] { "--stringvalue=--help" },
            new [] { "--stringvalue", "--help", "-s", "--help" },
            new [] { "--stringvalue=--help", "-s", "--help" },
            new [] { "--stringvalue", "--help", "-s=--help" },
            new [] { "--stringvalue=--help", "-s=--help" },
        };

        public static object[][] MixOfConsumedAndUnconsumedDashDashHelpValidArgsData = new []
        {
            new [] { "--stringvalue", "--help", "--help" },
            new [] { "--help", "--stringvalue", "--help" },
            new [] { "--stringvalue=--help", "--help" },
            new [] { "--help", "--stringvalue=--help" },
            new [] { "--stringvalue", "--help", "-s", "--help", "--help" },
            new [] { "--stringvalue", "--help", "--help", "-s", "--help" },
            new [] { "--help", "--stringvalue", "--help", "-s", "--help" },
            new [] { "--stringvalue=--help", "-s", "--help", "--help" },
            new [] { "--stringvalue=--help", "--help", "-s", "--help" },
            new [] { "--help", "--stringvalue=--help", "-s", "--help" },
            new [] { "--stringvalue", "--help", "-s=--help", "--help" },
            new [] { "--stringvalue", "--help", "--help", "-s=--help", "--help" },
            new [] { "--help", "--stringvalue", "--help", "-s=--help" },
            new [] { "--stringvalue=--help", "-s=--help", "--help" },
            new [] { "--stringvalue=--help", "--help", "-s=--help" },
            new [] { "--help", "--stringvalue=--help", "-s=--help" },
        };

        public static object[][] ConsumedDashDashHelpInvalidArgsData = new []
        {
            new [] { "--stringvalue", "--help", "foo" },
            new [] { "-s", "--help", "--stringvalue" },
            new [] { "-s", "--help", "-i", "foo" },
            new [] { "--stringvalue", "--help", "--invalid-switch", "256" },
            new [] { "--stringvalue=--help", "--invalid-switch", "256" },
            new [] { "--invalid-switch", "-s", "--help" },
        };

        public static IEnumerable<object[]> ConvertDataToShortOption(object[][] data)
        {
            foreach (object[] row in data)
            {
                var strings = row as string[];
                if (strings != null)
                {
                    yield return strings.Select(item => item.Replace("--help", "-h")).ToArray();
                }
            }
        }

        [Theory]
        [MemberData(nameof(ValidArgsData))]
        public void Explicit_help_command_with_valid_args_produces_just_one_HelpRequestedError(params string[] args)
        {
            // Arrange
            var help = new StringWriter();
            var sut = new Parser(config => {
                config.AutoHelp = true;
                config.HelpWriter = help;
            });

            // Act
            var result = sut.ParseArguments<Simple_Options>(args);

            // Assert
            result.Should().BeOfType<NotParsed<Simple_Options>>();
            result.As<NotParsed<Simple_Options>>().Errors.Should().HaveCount(x => x == 1);
            result.As<NotParsed<Simple_Options>>().Errors.First().Should().BeOfType<HelpRequestedError>();
        }

        public static IEnumerable<object[]> ValidArgsDataWithShortOption =
            ConvertDataToShortOption(ValidArgsData);

        [Theory]
        [MemberData(nameof(ValidArgsDataWithShortOption))]
        public void Explicit_help_command_with_valid_args_and_short_option_produces_just_one_HelpRequestedError(params string[] args)
        {
            // Arrange
            var help = new StringWriter();
            var sut = new Parser(config => {
                config.AutoHelp = true;
                // config.AutoHelpShortOption = true;  // Not implemented yet
                config.HelpWriter = help;
            });

            // Act
            var result = sut.ParseArguments<Simple_Options>(args);

            // Assert
            result.Should().BeOfType<NotParsed<Simple_Options>>();
            result.As<NotParsed<Simple_Options>>().Errors.Should().HaveCount(x => x == 1);
            result.As<NotParsed<Simple_Options>>().Errors.First().Should().BeOfType<HelpRequestedError>();
        }

        [Theory]
        [MemberData(nameof(InvalidArgsData))]
        public void Explicit_help_command_with_invalid_args_produces_just_one_HelpRequestedError(params string[] args)
        {
            // Arrange
            var help = new StringWriter();
            var sut = new Parser(config => {
                config.AutoHelp = true;
                config.HelpWriter = help;
            });

            // Act
            var result = sut.ParseArguments<Simple_Options>(args);

            // Assert
            result.Should().BeOfType<NotParsed<Simple_Options>>();
            result.As<NotParsed<Simple_Options>>().Errors.Should().HaveCount(x => x == 1);
            result.As<NotParsed<Simple_Options>>().Errors.First().Should().BeOfType<HelpRequestedError>();
        }

        public static IEnumerable<object[]> InvalidArgsDataWithShortOption =
            ConvertDataToShortOption(InvalidArgsData);

        [Theory]
        [MemberData(nameof(InvalidArgsDataWithShortOption))]
        public void Explicit_help_command_with_invalid_args_and_short_option_produces_just_one_HelpRequestedError(params string[] args)
        {
            // Arrange
            var help = new StringWriter();
            var sut = new Parser(config => {
                config.AutoHelp = true;
                // config.AutoHelpShortOption = true;  // Not implemented yet
                config.HelpWriter = help;
            });

            // Act
            var result = sut.ParseArguments<Simple_Options>(args);

            // Assert
            result.Should().BeOfType<NotParsed<Simple_Options>>();
            result.As<NotParsed<Simple_Options>>().Errors.Should().HaveCount(x => x == 1);
            result.As<NotParsed<Simple_Options>>().Errors.First().Should().BeOfType<HelpRequestedError>();
        }

        [Theory]
        [MemberData(nameof(ConsumedDashDashHelpValidArgsData))]
        public void Dash_dash_help_in_a_string_value_does_not_produce_HelpRequestedError(params string[] args)
        {
            // Arrange
            var help = new StringWriter();
            var sut = new Parser(config => {
                config.AutoHelp = true;
                config.HelpWriter = help;
            });

            // Act
            var result = sut.ParseArguments<Simple_Options>(args);

            // Assert
            // result.Should().BeOfType<NotParsed<Simple_Options>>();
            // result.As<NotParsed<Simple_Options>>().Errors.Should().HaveCount(x => x == 1);
            // result.As<NotParsed<Simple_Options>>().Errors.First().Should().BeOfType<HelpRequestedError>();
            result.Should().BeOfType<Parsed<Simple_Options>>();
            string stringValue = result.As<Parsed<Simple_Options>>().Value.StringValue;
            string shortAndLong = result.As<Parsed<Simple_Options>>().Value.ShortAndLong;
            stringValue.Should().Be("--help");
            shortAndLong.Should().BeOneOf("--help", null, "");
        }

        // TODO: Write tests to consume the following:
        public static IEnumerable<object[]> ConsumedDashDashHelpValidArgsDataWithShortOption =
            ConvertDataToShortOption(ConsumedDashDashHelpValidArgsData);
        public static IEnumerable<object[]> MixOfConsumedAndUnconsumedDashDashHelpValidArgsDataWithShortOption =
            ConvertDataToShortOption(MixOfConsumedAndUnconsumedDashDashHelpValidArgsData);
        public static IEnumerable<object[]> ConsumedDashDashHelpInvalidArgsDataWithShortOption =
            ConvertDataToShortOption(ConsumedDashDashHelpInvalidArgsData);

        // Then with AutoHelp = false, we'd have UnknownArgumentError("help"), which would NOT suppress other errors. So invalid inputs would have more than 1 error.
    }
}
