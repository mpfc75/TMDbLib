using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TMDbLib.Objects.Changes;
using TMDbLib.Objects.Movies;
using TMDbLib.Utilities.Converters;
using TMDbLibTests.JsonHelpers;
using Xunit;

namespace TMDbLibTests.UtilityTests
{
    public class ChangeItemConverterTest : TestBase
    {
        [Fact]
        public async Task ChangeItemConverter_ChangeItemAdded()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Converters.Add(new ChangeItemConverter());

            ChangeItemAdded original = new ChangeItemAdded();
            original.Iso_639_1 = "en";
            original.Value = "Hello world";

            string json = JsonConvert.SerializeObject(original, settings);
            ChangeItemAdded result = JsonConvert.DeserializeObject<ChangeItemBase>(json, settings) as ChangeItemAdded;

            await Verify(result);
        }

        [Fact]
        public async Task ChangeItemConverter_ChangeItemCreated()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Converters.Add(new ChangeItemConverter());

            ChangeItemCreated original = new ChangeItemCreated();
            original.Iso_639_1 = "en";

            string json = JsonConvert.SerializeObject(original, settings);
            ChangeItemCreated result = JsonConvert.DeserializeObject<ChangeItemBase>(json, settings) as ChangeItemCreated;
            
            await Verify(result);
        }

        [Fact]
        public async Task ChangeItemConverter_ChangeItemDeleted()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Converters.Add(new ChangeItemConverter());

            ChangeItemDeleted original = new ChangeItemDeleted();
            original.Iso_639_1 = "en";
            original.OriginalValue = "Hello world";

            string json = JsonConvert.SerializeObject(original, settings);
            ChangeItemDeleted result = JsonConvert.DeserializeObject<ChangeItemBase>(json, settings) as ChangeItemDeleted;
            
            await Verify(result);
        }

        [Fact]
        public async Task ChangeItemConverter_ChangeItemUpdated()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Converters.Add(new ChangeItemConverter());

            ChangeItemUpdated original = new ChangeItemUpdated();
            original.Iso_639_1 = "en";
            original.OriginalValue = "Hello world";
            original.Value = "Hello world 1234";

            string json = JsonConvert.SerializeObject(original, settings);
            ChangeItemUpdated result = JsonConvert.DeserializeObject<ChangeItemBase>(json, settings) as ChangeItemUpdated;
            
            await Verify(result);
        }

        /// <summary>
        /// Tests the ChangeItemConverter
        /// </summary>
        [Fact]
        public async Task TestChangeItemConverter()
        {
            Movie latestMovie = await TMDbClient.GetMovieLatestAsync();
            IList<Change> changes = await TMDbClient.GetMovieChangesAsync(latestMovie.Id);

            List<ChangeItemBase> changeItems = changes.SelectMany(s => s.Items).ToList();

            ChangeAction[] actions = { ChangeAction.Added, ChangeAction.Created, ChangeAction.Updated };

            Assert.NotEmpty(changeItems);
            Assert.All(changeItems, item => Assert.Contains(item.Action, actions));

            IEnumerable<ChangeItemBase> items = changeItems.Where(s => s.Action == ChangeAction.Added);
            Assert.All(items, item => Assert.IsType<ChangeItemAdded>(item));

            items = changeItems.Where(s => s.Action == ChangeAction.Updated);
            Assert.All(items, item => Assert.IsType<ChangeItemUpdated>(item));

            items = changeItems.Where(s => s.Action == ChangeAction.Created);
            Assert.All(items, item => Assert.IsType<ChangeItemCreated>(item));
        }
    }
}