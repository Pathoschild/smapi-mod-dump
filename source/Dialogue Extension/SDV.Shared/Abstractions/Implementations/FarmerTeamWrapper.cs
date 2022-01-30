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
  public class FarmerTeamWrapper : IFarmerTeamWrapper
  {
    public FarmerTeamWrapper(FarmerTeam item) => GetBaseType = item;
    public FarmerTeam GetBaseType { get; }
    public NetFields NetFields { get; }
    public void OnKickOutOfMinesEvent()
    {
    }

    public void OnRequestHorseWarp(long uid)
    {
    }

    public void OnRequestLeoMoveEvent()
    {
    }

    public void MarkCollectedNut(string key)
    {
    }

    public int GetIndividualMoney(IFarmerWrapper who) => 0;

    public void AddIndividualMoney(IFarmerWrapper who, int value)
    {
    }

    public void SetIndividualMoney(IFarmerWrapper who, int value)
    {
    }

    public NetIntDelta GetMoney(IFarmerWrapper who) => null;

    public bool SpecialOrderActive(string special_order_key) => false;

    public bool SpecialOrderRuleActive(string special_rule, ISpecialOrderWrapper order_to_ignore = null) => false;

    public ISpecialOrderWrapper GetAvailableSpecialOrder(int index = 0, string type = "") => null;

    public void CheckReturnedDonations()
    {
    }

    public bool OnDonatedItemWithdrawn(ISalableWrapper salable, IFarmerWrapper who, int amount) => false;

    public bool OnReturnedDonationDeposited(ISalableWrapper deposited_salable) => false;

    public void OnRequestMovieEndEvent(long uid)
    {
    }

    public void OnRequestPetWarpHomeEvent(long uid)
    {
    }

    public void OnRequestSpouseSleepEvent(long uid)
    {
    }

    public void OnRequestAddCharacterEvent(string character_name)
    {
    }

    public void OnAddCharacterEvent(string character_name)
    {
    }

    public void RequestLimitedNutDrops(string key, IGameLocationWrapper location, int x, int y, int limit, int reward_amount = 1)
    {
    }

    public int GetDroppedLimitedNutCount(string key) => 0;

    public void OnRingPhoneEvent(int which_call)
    {
    }

    public void OnEndMovieEvent(long uid)
    {
    }

    public void OnDemolishStableEvent(Guid stable_guid)
    {
    }

    public void DeleteFarmhand(IFarmerWrapper farmhand)
    {
    }

    public IFriendshipWrapper GetFriendship(long farmer1, long farmer2) => null;

    public void AddAnyBroadcastedMail()
    {
    }

    public bool IsMarried(long farmer) => false;

    public bool IsEngaged(long farmer) => false;

    public long? GetSpouse(long farmer) => null;

    public void FestivalPropsRemoved(Rectangle rect)
    {
    }

    public void SendProposal(IFarmerWrapper receiver, ProposalType proposalType, IItemWrapper gift = null)
    {
    }

    public IProposalWrapper GetOutgoingProposal() => null;

    public void RemoveOutgoingProposal()
    {
    }

    public IProposalWrapper GetIncomingProposal() => null;

    public double AverageDailyLuck(IGameLocationWrapper inThisLocation = null) => 0;

    public double AverageLuckLevel(IGameLocationWrapper inThisLocation = null) => 0;

    public double AverageSkillLevel(int skillIndex, IGameLocationWrapper inThisLocation = null) => 0;

    public void Update()
    {
    }

    public bool playerIsOnline(long uid) => false;

    public void SetLocalRequiredFarmers(string checkName, IEnumerable<IFarmerWrapper> required_farmers)
    {
    }

    public void SetLocalReady(string checkName, bool ready)
    {
    }

    public bool IsReady(string checkName) => false;

    public bool IsReadyCheckCancelable(string checkName) => false;

    public bool IsOtherFarmerReady(string checkName, IFarmerWrapper farmer) => false;

    public int GetNumberReady(string checkName) => 0;

    public int GetNumberRequired(string checkName) => 0;

    public void NewDay()
    {
    }
  }
}
