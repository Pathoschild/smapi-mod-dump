/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using FluentAssertions;
using StardewArchipelago.Locations;

namespace StardewArchipelagoTests
{
    public class Tests
    {
        private LocationNameMatcher _locationNameMatcher;

        [SetUp]
        public void Setup()
        {
            _locationNameMatcher = new LocationNameMatcher();
        }

        [TestCase("Snail", new[]{"Shipsanity: Snail", "Fishsanity: Snail"}, new[] { "Escargot", "Nail" }, TestName = "Snail")]
        [TestCase("Stone", new[] { "Shipsanity: Stone" }, new[] { "Shipsanity: Wood", "Explore Stonehenge" }, TestName = "Stone")]
        [TestCase("Wood", new[] { "Shipsanity: Wood" }, new[] { "Shipsanity: Hardwood", "Craft Wooden Display" }, TestName = "Wood")]
        [TestCase("Juice", new[] { "Shipsanity: Juice" }, new string[0], TestName = "Juice")]
        [TestCase("Win Grange Display", new[] { "Win Grange Display" }, new[] { "Grange Display" }, TestName = "Grange Display")]
        [TestCase("Hardwood", new[] { "Shipsanity: Hardwood" }, new[] { "Robin's Project" }, TestName = "Hardwood")]
        [TestCase("Tomato", new[] { "Harvest Tomato", "Shipsanity: Tomato" }, new string[0], TestName = "Tomato")]
        [TestCase("Wizard", new[] { "Friendsanity: Wizard 4 <3" }, new string[0], TestName = "Wizard")]
        [TestCase("Apple", new[] { "Harvest Apple" }, new[] { "Friendsanity: Apples 1 <3" }, TestName = "Apple")]
        [TestCase("Apples", new[] { "Friendsanity: Apples 10 <3" }, new[] { "Harvest Apple" }, TestName = "Apples")]
        public void GetAllLocationsContainingWordTruePositivesTest(string itemName, string[] locationsMatching, string[] locationsNotMatching)
        {
            // Arrange
            var allLocations = locationsMatching.Union(locationsNotMatching);

            // Act
            var matches = _locationNameMatcher.GetAllLocationsContainingWord(allLocations, itemName);

            // Assert
            matches.Should().NotBeNull();
            matches.Should().BeEquivalentTo(locationsMatching);
        }

        [TestCase("Snail", new[] { "Open Professor Snail Cave" }, TestName = "Professor Snail Cave")]
        [TestCase("Stone", new[] { "Shipsanity: Swirl Stone", "Smashing Stone" }, TestName = "Swirl Stone")]
        [TestCase("Hardwood", new[] { "Shipsanity: Hardwood Display: Amphibian Fossil", "Shipsanity: Wooden Display: Dinosaur Egg", "Craft Hardwood Preservation Chamber", "Craft Hardwood Display", "Shipsanity: Hardwood Fence" }, TestName = "Hardwood Displays")]
        public void GetAllLocationsContainingWordFalsePositivesTest(string itemName, string[] locationsNotMatching)
        {
            // Arrange

            // Act
            var matches = _locationNameMatcher.GetAllLocationsContainingWord(locationsNotMatching, itemName);

            // Assert
            matches.Should().NotBeNull();
            matches.Should().BeEmpty();
        }
        
        [TestCase("Juice", new[] { "Pam Needs Juice" }, TestName = "Pam Juice")]
        [TestCase("Tomato", new[] { "Shipsanity: Tomato Seeds" }, TestName = "Tomato Seeds")]
        [TestCase("Wizard", new[] { "Meet The Wizard" }, TestName = "Meet the Wizard")]
        public void GetAllLocationsContainingWordThematicallyRelatedItemsTest(string itemName, string[] locationsMatching)
        {
            // Arrange

            // Act
            var matches = _locationNameMatcher.GetAllLocationsContainingWord(locationsMatching, itemName);

            // Assert
            matches.Should().NotBeNull();
            matches.Should().BeEquivalentTo(locationsMatching);
        }
    }
}