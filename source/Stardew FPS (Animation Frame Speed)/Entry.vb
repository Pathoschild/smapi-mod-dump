Imports StardewModdingAPI
Imports StardewValley

Public Class Entry
    Inherits [Mod]

    Public Overrides Sub Entry(helper As IModHelper)
        helper.ConsoleCommands.Add("fps", "Usage: fps <number>, like: fps 120 , means run to 120 FPS", AddressOf SetFPS)
    End Sub

    Public Shared Sub SetFPS(command As String, args As String())
        GameRunner.instance.TargetElapsedTime = TimeSpan.FromSeconds(1D / args(0))
        GameRunner.instance.IsFixedTimeStep = True
    End Sub

End Class
