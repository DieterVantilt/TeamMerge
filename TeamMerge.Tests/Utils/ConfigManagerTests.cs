using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamMerge.Settings.Enums;
using TeamMerge.Utils;

namespace TeamMerge.Tests.Utils
{
    [TestClass]
    public class ConfigManagerTests
    {
        private ConfigManager _sut;

        private IConfigFileHelper _configFileHelper;

        [TestInitialize]
        public void Initialize()
        {
            _configFileHelper = MockRepository.GenerateStrictMock<IConfigFileHelper>();

            _sut = new ConfigManager(_configFileHelper);
        }

        [TestMethod]
        public void ConfigManager_GetValue_WhenTypeOfEnum_ThenReturnsEnumWhenDictionaryContainsAnEnum()
        {
            var dictionary = new Dictionary<string, object> { { "test", Branch.Target } };

            _configFileHelper.Expect(x => x.GetDictionary()).Return(dictionary);

            var result = _sut.GetValue<Branch>("test");

            Assert.AreEqual(Branch.Target, result);
        }

        [TestMethod]
        public void ConfigManager_GetValue_WhenTypeOfEnum_ThenReturnsEnumWhenDictionaryContainsInteger()
        {
            var dictionary = new Dictionary<string, object> { { "test", long.Parse("3") } };

            _configFileHelper.Expect(x => x.GetDictionary()).Return(dictionary);

            var result = _sut.GetValue<Branch>("test");

            Assert.AreEqual(Branch.SourceAndTarget, result);
        }

        [TestMethod]
        public void ConfigManager_GetValue_WhenTypeOfString_ThenReturnsStringWhenDictionaryContainsAString()
        {
            var dictionary = new Dictionary<string, object> { { "test", "SaveSetting" } };

            _configFileHelper.Expect(x => x.GetDictionary()).Return(dictionary);

            var result = _sut.GetValue<string>("test");

            Assert.AreEqual("SaveSetting", result);
        }

        [TestMethod]
        public void ConfigManager_GetValue_WhenTypeOfEnumerableString_ThenReturnsEnumerableOfTypeStringWhenDictionaryContainsAnListOfString()
        {
            var dictionary = new Dictionary<string, object> { { "test", new List<string> { "MyString" } } };

            _configFileHelper.Expect(x => x.GetDictionary()).Return(dictionary);

            var result = _sut.GetValue<IEnumerable<string>>("test");

            Assert.IsTrue(result.Any());
            Assert.AreEqual("MyString", result.First());
        }

        [TestMethod]
        public void ConfigManager_GetValue_WhenTypeOfEnumerableString_ThenReturnsEnumerableOfTypeStringWhenDictionaryContainsAnJArrayOfStrings()
        {
            var array = new JArray("Code Review Request", "Code Review Response");

            var dictionary = new Dictionary<string, object> { { "test", array } };

            _configFileHelper.Expect(x => x.GetDictionary()).Return(dictionary);

            var result = _sut.GetValue<IEnumerable<string>>("test");

            Assert.IsTrue(result.Any());
            Assert.AreEqual("Code Review Request", result.First());
            Assert.AreEqual("Code Review Response", result.Last());
        }
    }
}
