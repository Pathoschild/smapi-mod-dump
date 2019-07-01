using System;
using System.Collections.Generic;
using Denifia.Stardew.SendItems.Domain;

namespace Denifia.Stardew.SendItems.Services
{
    public interface IConfigurationService
    {
        string ConnectionString { get; }
        Uri GetApiUri();
        string GetLocalPath();
        bool InDebugMode();
        bool InLocalOnlyMode();
        List<SavedGame> GetSavedGames();

    }
}
