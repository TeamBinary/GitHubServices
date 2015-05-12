using System;

using GitHubServices.Models;

using NUnit.Framework;

namespace GitHubServices.Test.BusinessLogic
{
    [TestFixture]
    class ReplacerTest
    {
        const string content = @"
""Automatic `Asser.AreEqual()` and automatic `ToString()`.""
Like a JSON serializer on drugs


Table of content
* [1. Entroduction](#1-entroduction)
 * [1.1 Simple example usage](#11-simple-example-usage)
 * [1.2 Generic ToString() usage](#12-generic-tostring-usage)
* [2. Configuration](#2-configuration)
 * [2.1 Stacked configuration principle](#21-stacked-configuration-principle)
 * [2.2 Simple changes](#22-simple-changes)
 * [2.3 Culture specific printing](#23-culture-specific-printing)
 * [2.4 Output as a single line](#24-output-as-a-single-line)
 * [2.5 Field harvesting](#25-field-harvesting)
 * [2.6 Simple value printing](#26-simple-value-printing)
 * [2.7 Output formatting](#27-output-formatting)
* [3. Unit testing](#3-unit-testing)  
 * [3.1 Restricting fields harvested](#31-restricting-fields-harvested)
*  [4. License](#4-license)


# 1. Introduction

this is the intro
and so forth


# 2. Configuration

 StatePrinter is configurable. The configuration can be broken down to three parts each of which represents .

* `IFieldHarvester` deals with how/which fields are harvested from types. E.g. only public fields are printed.
Finally, culture specific printing of dates and numbers are supported.


## 2.1 Stacked configuration principle

The Stateprinter has a configuration object that for the e use. This is due to the FILO tems in the reverse order they lues.

```C#
public static Configuration GetStandardConfiguration()

## 2.2 Simple changes

The `Configuration` class should be rather self-documenting. 
var cfg = Confi";

        [Test]
        public void TestReplace()
        {
            var parser = new TocParser();
            var replacer = new ContentReplacer();

            var toc = parser.MakeToc(content);

            var expected = @"Table of Content
 * [1. Introduction](#1-introduction)
 * [2. Configuration](#2-configuration)
   * [2.1 Stacked configuration principle](#21-stacked-configuration-principle)
   * [2.2 Simple changes](#22-simple-changes)";
            Create.Assert2().PrintIsSame(expected, toc);


            var newVersion = @"""
""Automatic `Asser.AreEqual()` and automatic `ToString()`.""
Like a JSON serializer on drugs


Table of Content
 * [1. Introduction](#1-introduction)
 * [2. Configuration](#2-configuration)
   * [2.1 Stacked configuration principle](#21-stacked-configuration-principle)
   * [2.2 Simple changes](#22-simple-changes)


# 1. Introduction

this is the intro
and so forth


# 2. Configuration

 StatePrinter is configurable. The configuration can be broken down to three parts each of which represents .

* `IFieldHarvester` deals with how/which fields are harvested from types. E.g. only public fields are printed.
Finally, culture specific printing of dates and numbers are supported.


## 2.1 Stacked configuration principle

The Stateprinter has a configuration object that for the e use. This is due to the FILO tems in the reverse order they lues.

```C#
public static Configuration GetStandardConfiguration()

## 2.2 Simple changes

The `Configuration` class should be rather self-documenting. 
var cfg = Confi""";

            Create.Assert().PrintIsSame(newVersion, replacer.TryReplaceToc(content, toc));

            // parsing the result again must yield the same result
            Assert.AreEqual(newVersion, replacer.TryReplaceToc(newVersion, parser.MakeToc(newVersion)));
        }
    }
}