using System.Text.RegularExpressions;
using DaggerfallWorkshop.Game.Questing;

namespace Game.Mods.QuestActionsExtension.Actions
{
    public class UpdateQuestItemApplyMagicTemplate : ActionTemplate
    {
        private Symbol _itemSymbol;
        private int _templateId;

        public UpdateQuestItemApplyMagicTemplate(Quest parentQuest) : base(parentQuest)
        {
        }

        public override string Pattern => @"update-quest-item (?<anItem>[a-zA-Z0-9_.]+) apply-magic-template (?<templateId>\d+)";

        public override IQuestAction CreateNew(string source, Quest parentQuest)
        {
            // Source must match pattern
            Match match = Test(source);
            if (!match.Success)
                return null;


            if (string.IsNullOrEmpty(match.Groups["anItem"].Value))
            {
                SetComplete();
                return null;
            }


            if (string.IsNullOrEmpty(match.Groups["templateId"].Value))
            {
                SetComplete();
                return null;
            }

            return new UpdateQuestItemApplyMagicTemplate(parentQuest)
            {
                _itemSymbol = new Symbol(match.Groups["anItem"].Value),
                _templateId = Parser.ParseInt(match.Groups["templateId"].Value),
            };
        }

        public override void Update(Task _)
        {
            // Get related Item resource
            Item item = ParentQuest.GetItem(_itemSymbol);
            if (item == null)
            {
                // Stop if Item does not exist
                SetComplete();
                return;
            }

            Helpers.ApplyMagicTemplate(item.DaggerfallUnityItem, _templateId);
        }

        public override object GetSaveData()
        {
            return new SaveDataV1
            {
                ItemSymbol = _itemSymbol,
                TemplateId = _templateId,
            };
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null) return;

            var data = (SaveDataV1)dataIn;

            _itemSymbol = data.ItemSymbol;
            _templateId = data.TemplateId;
        }

        [FullSerializer.fsObject("v1")]
        public struct SaveDataV1
        {
            public Symbol ItemSymbol;
            public int TemplateId;
        }
    }
}