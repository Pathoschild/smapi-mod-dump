/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/DialogueExtension
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;

namespace SDV.Shared.Abstractions
{
  public interface IFarmerTeamWrapper : IWrappedType<FarmerTeam>
  {
    NetFields NetFields { get; }
    void OnKickOutOfMinesEvent();
    void OnRequestHorseWarp(long uid);
    void OnRequestLeoMoveEvent();
    void MarkCollectedNut(string key);
    int GetIndividualMoney(IFarmerWrapper who);
    void AddIndividualMoney(IFarmerWrapper who, int value);
    void SetIndividualMoney(IFarmerWrapper who, int value);
    NetIntDelta GetMoney(IFarmerWrapper who);
    bool SpecialOrderActive(string special_order_key);
    bool SpecialOrderRuleActive(string special_rule, ISpecialOrderWrapper order_to_ignore = null);
    ISpecialOrderWrapper GetAvailableSpecialOrder(int index = 0, string type = "");
    void CheckReturnedDonations();
    bool OnDonatedItemWithdrawn(ISalableWrapper salable, IFarmerWrapper who, int amount);
    bool OnReturnedDonationDeposited(ISalableWrapper deposited_salable);
    void OnRequestMovieEndEvent(long uid);
    void OnRequestPetWarpHomeEvent(long uid);
    void OnRequestSpouseSleepEvent(long uid);
    void OnRequestAddCharacterEvent(string character_name);
    void OnAddCharacterEvent(string character_name);

    void RequestLimitedNutDrops(
      string key,
      IGameLocationWrapper location,
      int x,
      int y,
      int limit,
      int reward_amount = 1);

    int GetDroppedLimitedNutCount(string key);
    void OnRingPhoneEvent(int which_call);
    void OnEndMovieEvent(long uid);
    void OnDemolishStableEvent(Guid stable_guid);
    void DeleteFarmhand(IFarmerWrapper farmhand);
    IFriendshipWrapper GetFriendship(long farmer1, long farmer2);
    void AddAnyBroadcastedMail();
    bool IsMarried(long farmer);
    bool IsEngaged(long farmer);
    long? GetSpouse(long farmer);
    void FestivalPropsRemoved(Rectangle rect);
    void SendProposal(IFarmerWrapper receiver, ProposalType proposalType, IItemWrapper gift = null);
    IProposalWrapper GetOutgoingProposal();
    void RemoveOutgoingProposal();
    IProposalWrapper GetIncomingProposal();
    double AverageDailyLuck(IGameLocationWrapper inThisLocation = null);
    double AverageLuckLevel(IGameLocationWrapper inThisLocation = null);
    double AverageSkillLevel(int skillIndex, IGameLocationWrapper inThisLocation = null);
    void Update();
    bool playerIsOnline(long uid);
    void SetLocalRequiredFarmers(string checkName, IEnumerable<IFarmerWrapper> required_farmers);
    void SetLocalReady(string checkName, bool ready);
    bool IsReady(string checkName);
    bool IsReadyCheckCancelable(string checkName);
    bool IsOtherFarmerReady(string checkName, IFarmerWrapper farmer);
    int GetNumberReady(string checkName);
    int GetNumberRequired(string checkName);
    void NewDay();
  }
}