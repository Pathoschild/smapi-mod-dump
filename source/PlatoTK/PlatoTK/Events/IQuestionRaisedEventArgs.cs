/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using StardewValley;
using System.Collections.Generic;

namespace PlatoTK.Events
{
    public interface IQuestionRaisedEventArgs
    {
        string Question { get; }

        string LastQuestionKey { get; }
        List<Response> Choices { get; }
        bool IsTV { get; }
        void RemoveResponse(Response respone);
        void AddResponse(Response response);
        void SetQuestion(string Question);
        void PaginateResponses();
    }
}
