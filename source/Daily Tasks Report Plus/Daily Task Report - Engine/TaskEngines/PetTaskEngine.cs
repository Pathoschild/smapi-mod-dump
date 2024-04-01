/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Prism-99/DailyTaskReportPlus
**
*************************************************/

using StardewValley.Characters;
using StardewValley.Locations;
using DailyTasksReport.UI;


namespace DailyTasksReport.TaskEngines
{
    class PetTaskEngine : TaskEngine
    {
        public Farm ?_farm;
        public Pet ?_pet;
        private bool _petBowlFilled;
        public bool _petPetted;

        internal PetTaskEngine(TaskReportConfig config)
        {
            _config = config;
            TaskClass = "Pet";
            SetEnabled();
        }

        public override void Clear()
        {
            SetEnabled();
            _pet = null;
            _petBowlFilled = false;
            _petPetted = false;
        }

        public override List<ReportReturnItem> DetailedInfo()
        {
            return new List<ReportReturnItem> { };
        }

        public override List<ReportReturnItem> GeneralInfo()
        {
            UpdateInfo();

            List<ReportReturnItem> prItems = new List<ReportReturnItem> { };

            if (!Enabled || _pet == null)
                return prItems;


            if (_config.UnpettedPet && !_petPetted)
            {
                prItems.Add(new ReportReturnItem { Label = I18n.Tasks_Pet_Pet() });
            }
            if (_config.UnfilledPetBowl && !_petBowlFilled)
            {
                prItems.Add(new ReportReturnItem { Label = I18n.Tasks_Pet_Bowl() });
            }
            return prItems;
        }

        internal override void FirstScan()
        {
            _farm = Game1.locations.OfType<Farm>().FirstOrDefault();

            _pet = _farm?.characters.OfType<Pet>().FirstOrDefault();

            if (_pet != null) return;

            var location = Game1.locations.OfType<FarmHouse>().FirstOrDefault();
            _pet = location.characters.OfType<Pet>().FirstOrDefault();
        }

        public override void SetEnabled()
        {
            Enabled = _config.UnpettedPet || _config.UnfilledPetBowl;
        }


        public bool IsPetPetted()
        {
            if (_pet == null) return false;
            _petPetted= _pet.grantedFriendshipForPet.Value;
            return _petPetted;
        }

        public bool IsPetBowlFilled()
        {
            if (_pet == null) return false;
            return _pet.GetPetBowl().watered.Value;
        }
        private void UpdateInfo()
        {
            if (_pet == null)
            {
                FirstScan();
                if (_pet == null)
                    return;
            }

            _petPetted = IsPetPetted();
            _petBowlFilled = IsPetBowlFilled();


            Enabled = Enabled && !(_petBowlFilled && _petPetted);
        }


    }
}
