Imports StardewModdingAPI
Imports StardewModdingAPI.Events
Imports StardewModdingAPI.Utilities
Imports StardewValley
Imports StardewValley.Locations
Imports TehPers.Core.Api.Enums
Imports TehPers.Core.Api.Weighted
Imports TehPers.Core.Helpers.Static
Imports TehPers.FishingOverhaul.Api

Public Class ModEntry
    Inherits [Mod]

    Public Const BOMB As Integer = 287

    Public Overrides Sub Entry(helper As IModHelper)
        AddHandler helper.Events.GameLoop.GameLaunched, AddressOf RegisterApiEvents
    End Sub

    Private Sub RegisterApiEvents(sender As Object, args As EventArgs)
        Dim api = Helper.ModRegistry.GetApi(Of IFishingApi)("TehPers.FishingOverhaul")
        If api IsNot Nothing Then
            ' Add bombs as catchable
            api.AddTrashData(New BombTrashData())

            ' Add an event handler to check when bombs are caught
            AddHandler api.TrashCaught, AddressOf TrashCaught
        End If
    End Sub

    Private Sub TrashCaught(sender As Object, e As FishingEventArgs)
        If e.ParentSheetIndex = BOMB Then
            ' Explode the bomb
            e.Who.currentLocation.playSound("explosion")
            e.Who.currentLocation.explode(e.Who.getTileLocation(), 3, e.Who)
        End If
    End Sub
End Class

Public Class BombTrashData
    Implements ITrashData

    Public Function GetWeight() As Double Implements IWeighted.GetWeight
        Return 1D
    End Function

    Public ReadOnly Property PossibleIds As IEnumerable(Of Integer) Implements ITrashData.PossibleIds
        Get
            Return TehPers.FishingBomb.ModEntry.BOMB.Yield()
        End Get
    End Property

    Public Function MeetsCriteria(who As Farmer, locationName As String, waterType As WaterType, [date] As SDate, weather As Weather, time As Integer, fishingLevel As Integer, mineLevel As Integer?) As Boolean Implements ITrashData.MeetsCriteria
        Return True
    End Function
End Class
