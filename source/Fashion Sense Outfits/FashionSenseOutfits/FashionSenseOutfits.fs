// Copyright (C) 2023 Nihilistzsche
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

namespace FashionSenseOutfits

open System
open System.Collections.Generic
open System.Linq
open Microsoft.FSharp.Collections

open FashionSense.Framework.Interfaces.API
open ContentPatcher

open StardewModdingAPI
open StardewModdingAPI.Events

open StardewValley

open FashionSenseOutfits.Models

type private OutfitDataModel = Dictionary<string, OutfitData>

type public FashionSenseOutfits() =
    inherit Mod()

    static let __assetName: string = "nihilistzsche.FashionSenseOutfits/Outfits"
    static let mutable __fsApi: IApi = null
    static let mutable __cpApi: IContentPatcherAPI = null
    member val private _data: OutfitDataModel = null with get, set 
    member val private _cpConditionsReady = false with get, set
    member val private _lastEvent: Event = null with get, set
    member val private _seenInvalids = [] with get, set
    member val private _emptyRetryNeeded = false with get, set
    member val private _retryCount = 0 with get, set
    
    member private this.OnOneSecondUpdateTicked = EventHandler<OneSecondUpdateTickedEventArgs>(
        fun _ _ ->
            this._retryCount <- this._retryCount + 1
            this.UpdateOutfit()
        )
    
    member private this.IsValid(requestedOutfitId: string): bool*string =
        if String.IsNullOrEmpty(requestedOutfitId) then
            (false, null)
        else
            let outfitIds = __fsApi.GetOutfitIds().Value
            if outfitIds.Count = 0 then
                if not this._emptyRetryNeeded && this._retryCount < 5 then
                    this.Helper.Events.GameLoop.OneSecondUpdateTicked.AddHandler(this.OnOneSecondUpdateTicked)
                    this._emptyRetryNeeded <- true
                (false, null)
            else
                if this._emptyRetryNeeded then
                    this.Helper.Events.GameLoop.OneSecondUpdateTicked.RemoveHandler(this.OnOneSecondUpdateTicked)
                    this._emptyRetryNeeded <- false
                let correctedId = __fsApi.GetOutfitIds().Value.FirstOrDefault(fun outfitId -> outfitId.Equals(requestedOutfitId, StringComparison.OrdinalIgnoreCase))
                (not (isNull correctedId), correctedId)

    member private this.RequestData(_: obj) =
        Game1.content.Load<OutfitDataModel>(__assetName) |> ignore
    
    member inline private _.ValidateAsset<'T when 'T : (member Name : IAssetName)>(e: 'T) =
        e.Name.IsEquivalentTo(__assetName)

    member private this.UpdateOutfit() =
        let requestedOutfitId = this._data["RequestedOutfit"].OutfitId
        let valid, correctedId = this.IsValid(requestedOutfitId)
        if valid then
            let currentOutfitPair = __fsApi.GetCurrentOutfitId()
            if not currentOutfitPair.Key || correctedId <> currentOutfitPair.Value then
                __fsApi.SetCurrentOutfitId(correctedId, this.ModManifest) |> ignore
        else
            if not (List.exists(fun elem -> elem = requestedOutfitId) this._seenInvalids) && requestedOutfitId <> "" && not this._emptyRetryNeeded then 
                this._seenInvalids <- requestedOutfitId :: this._seenInvalids
                this.Monitor.Log($"Given outfit with ID {requestedOutfitId} is invalid.")
            else if this._emptyRetryNeeded && this._retryCount = 5 then
                this.Helper.Events.GameLoop.OneSecondUpdateTicked.RemoveHandler(this.OnOneSecondUpdateTicked)
                this._emptyRetryNeeded <- false

    member private this.OnGameLaunched(_: GameLaunchedEventArgs) =
        __fsApi <- this.Helper.ModRegistry.GetApi<IApi>("PeacefulEnd.FashionSense")
        __cpApi <- this.Helper.ModRegistry.GetApi<IContentPatcherAPI>("Pathoschild.ContentPatcher")
        __cpApi.RegisterToken(this.ModManifest, 
            "CurrentOutfit", 
            Func<string[]>(fun () ->
                match Context.IsWorldReady with
                | true ->
                    let outfitPair = __fsApi.GetCurrentOutfitId()
                    [| if outfitPair.Key then outfitPair.Value else null |]
                | false ->
                    [| null |] 
            )
        )
    
    member private this.OnUpdateTicked(e: UpdateTickedEventArgs) =
        if (not (isNull this._lastEvent) && isNull Game1.CurrentEvent) || (not this._cpConditionsReady && __cpApi.IsConditionsApiReady) then
            if not this._cpConditionsReady then this._cpConditionsReady <- __cpApi.IsConditionsApiReady
            this.RequestData()
        this._lastEvent <- Game1.CurrentEvent
            
    member inline private _.KVP<'T,'U>(k: 'T, v: 'U) =
        KeyValuePair<'T,'U>(k, v)

    member private this.LoadModel: unit -> obj =
        fun() -> OutfitDataModel([ this.KVP("RequestedOutfit", OutfitData(String.Empty)) ])
    
    member private this.OnAssetRequested(e: AssetRequestedEventArgs) =
        if this.ValidateAsset(e) then
            e.LoadFrom(this.LoadModel, AssetLoadPriority.Medium)

    member private this.OnAssetReady(e: AssetReadyEventArgs) =
        if this.ValidateAsset(e) then
            let newData = Game1.content.Load<OutfitDataModel>(__assetName)
            if isNull(this._data) || this._data["RequestedOutfit"].OutfitId <> newData["RequestedOutfit"].OutfitId then
                this._data <- newData
                this.UpdateOutfit()
    
    member private this.OnAssetsInvalidated(e: AssetsInvalidatedEventArgs) =
        if e.Names.Any(fun x -> x.IsEquivalentTo(__assetName)) then
            this.RequestData()

    override this.Entry(helper: IModHelper) =
        helper.Events.GameLoop.GameLaunched.Add(this.OnGameLaunched)
        helper.Events.GameLoop.DayStarted.Add(this.RequestData)
        helper.Events.GameLoop.TimeChanged.Add(this.RequestData)
        helper.Events.GameLoop.UpdateTicked.Add(this.OnUpdateTicked)
        helper.Events.Player.Warped.Add(this.RequestData)
        helper.Events.Content.AssetRequested.Add(this.OnAssetRequested)
        helper.Events.Content.AssetReady.Add(this.OnAssetReady)
        helper.Events.Content.AssetsInvalidated.Add(this.OnAssetsInvalidated)
